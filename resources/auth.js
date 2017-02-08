var request = require('request');
var locks = require('locks');

function AuthObject(CLIENT_ID, CLIENT_SECRET, scope) {
    this.CLIENT_ID = CLIENT_ID;
    this.CLIENT_SECRET = CLIENT_SECRET;
    this.scope = scope;
    this.lock = locks.createReadWriteLock();
    this.token = "";
    this.expiration = 0;
    this.expirationCond = locks.createCondVariable(0);
    this.resetCond = locks.createCondVariable(false);
}

AuthObject.prototype.getToken = function(callback) {
    // Check expiration: first iteration will require reset
    var authObj = this;
    if (!authObj.resetCond._value && authObj.expiration < Date.now()) {
        authObj.resetCond.set(true);
        authObj.lock.writeLock(function() {
            var options = {
                method: 'POST',
                url: 'https://developer.api.autodesk.com/authentication/v1/authenticate',
                form: {
                    client_id: authObj.CLIENT_ID,
                    client_secret: authObj.CLIENT_SECRET,
                    grant_type: 'client_credentials',
                    scope: authObj.scope
                }
            };

            request(options, function(error, response, body) {
                if (error) {
                    authObj.token = "";
                    authObj.expiration = 0;
                    authObj.lock.unlock();
                    return callback("error getting token", null);
                }
                var parsed_body = JSON.parse(body);
				if (parsed_body.errorCode) {
					return callback(new Error(parsed_body.errorCode + " - " + parsed_body.developerMessage));
				}
                authObj.resetCond.set(false);
                // Expiration good for 9 hours
                var expiration_delay = 9*60*60*1000;
                authObj.expirationCond.set(Date.now() + expiration_delay);
                authObj.token = parsed_body.access_token;
                authObj.expiration = Date.now() + expiration_delay;
                callback(null, authObj.token);
                return authObj.lock.unlock();
            });
        });
    } else {
        authObj.resetCond.wait(function(resetting) {
            return !resetting;
        }, function() {
            authObj.expirationCond.wait(function(exp) {
                return exp > Date.now();
            }, function() {
                authObj.lock.readLock(function() {
                    var tok = authObj.token;
                    authObj.lock.unlock();
                    return callback(null, tok);
                });
            });
        });
    }
};



module.exports = function(config) {
    var auth = {};

    auth.two_leg = function(scope, callback) {

        if (!scope || !Array.isArray(scope)) {
            return callback("Provided scope must be an array.")
        }


        var scope_string = "";
        scope.forEach(function(scopeElement) {
            scope_string += scopeElement + " ";
        })


        var authObj = new AuthObject(config.CLIENT_ID, config.CLIENT_SECRET, scope_string);

		authObj.getToken(function(error, token) {
			if (error) {
				return callback(error, null);
			}
			return callback(null, authObj)

		});

    };

    return auth;

}

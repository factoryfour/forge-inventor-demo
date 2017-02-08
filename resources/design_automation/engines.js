var request = require('request');

module.exports = function(config, authObj) {
    var engines = {};

    engines.getAll = function(callback) {
		authObj.getToken(function(error, token) {
			if (error) {
				return callback(error, null);
			}

			var options = {
	            method: 'GET',
	            url: config.DA_BASE_URL + 'Engines',
				headers: {
					authorization: 'Bearer ' + token
				}
	        };

	        request(options, function(error, response, body) {
	            if (error) return callback(Error(error), null);
				try {
					var parsed = JSON.parse(body);
					return callback(null, parsed);
				} catch (e) {
					return callback("Error parsing response.", null)
				} finally {

				}
	        });
		});
    };

	engines.get = function(id, callback) {
		authObj.getToken(function(error, token) {
			if (error) {
				return callback(error, null);
			}

			var options = {
				method: 'GET',
				url: config.DA_BASE_URL + 'Engines(\'' + id + '\')',
				headers: {
					authorization: 'Bearer ' + token
				}
			};

			request(options, function(error, response, body) {
				if (error) return callback(Error(error), null);
				try {
					var parsed = JSON.parse(body);
					return callback(null, parsed);
				} catch (e) {
					return callback("Error parsing response.", null)
				} finally {

				}
			});
		});
	};

    return engines;

}

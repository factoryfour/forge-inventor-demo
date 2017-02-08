var request = require('request');

module.exports = function(config, authObj) {
    var work_items = {};

    work_items.create = function(workItemConfig, callback) {

        authObj.getToken(function(error, token) {
            if (error) {
                return callback(error, null);
            }

            var options = {
                method: 'POST',
                url: config.DA_BASE_URL + 'WorkItems',
                headers: {
                    authorization: 'Bearer ' + token
                },
                body: workItemConfig,
                json: true
            };
            request(options, function(error, response, body) {
                if (error) return callback(Error(error), null);
                return callback(null, body);
                // try {
                //     var parsed = JSON.parse(body);
                //     return callback(null, parsed);
                // } catch (e) {
                //     return callback("Error parsing response.", null)
                // } finally {

                // }
            });
        });
    };

    work_items.getAll = function(callback) {
        authObj.getToken(function(error, token) {
            if (error) {
                return callback(error, null);
            }

            var options = {
                method: 'GET',
                url: config.DA_BASE_URL + 'WorkItems',
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

    work_items.get = function(id, callback) {
        authObj.getToken(function(error, token) {
            if (error) {
                return callback(error, null);
            }

            var options = {
                method: 'GET',
                url: config.DA_BASE_URL + 'WorkItems(\'' + id + '\')',
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

    return work_items;

}

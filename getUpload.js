const config = {
    "CLIENT_ID": "",
    "CLIENT_SECRET": "",
    "BASE_URL": "https://developer.api.autodesk.com/inventor.io/us-east/v2/"
    // "BASE_URL": "https://developer.api.autodesk.com/autocad.io/us-east/v2/"
}

const forge = require('./index.js');
const auth = forge.auth(config);
const fs = require('fs');
var scope = ['data:read', 'bucket:read', 'code:all']
const request = require('request');

auth.two_leg(scope, function(error, authObj) {
    if (error) {
        throw error;
    }

    var da = forge.da(config, authObj);

    da.app_packages.getUploadUrl(function(error, url) {
        console.log(error);
        console.log(url);
    });
});

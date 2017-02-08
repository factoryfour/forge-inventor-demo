var should = require('should');
var os = require('os');

// Check platform to handle file path issues
var isWin = os.platform().indexOf('win') > -1
if (isWin) {
  var root = __dirname.substring(0, __dirname.lastIndexOf('\\'));
}
else {
  var root = __dirname.substring(0, __dirname.lastIndexOf('/'));
}
const config = require(__dirname + "/get_config.js")(root + '/config.js');
const forge = require(root + '/index.js');

describe('App Package Methods', function() {
    var authObj;
    var da;
    const auth = forge.auth(config);
    var test_id = 'TESTPackage';

    before(function(done) {
        var scope = ['data:read', 'bucket:read', 'code:all']

        auth.two_leg(scope, function(error, cAuthObj) {
            if (error) {
                throw new Error(error.message ? error.message : error);
            }
            authObj = cAuthObj;
            da = forge.da(config, authObj);
            done();
        });
    });

    it('appPackageTests-01 - should be able to list all app_packages', function(done) {
        da.app_packages.getAll(function(error, results) {
            should.not.exist(error);
            should.exist(results);
            if (process.env.VERBOSE == 'loud') {
                console.log(results);
            }
            done();
        });
    });

    it('appPackageTests-02 - should be able to create an app_package', function(done) {
        var filePath = __dirname + '/sample_files/samplePlugin.bundle.zip';
        var packageConfig = require(__dirname + '/sample_configs/app_package.js')(test_id);
		this.slow(4000);

        da.app_packages.pushBundle(filePath, function(error, resource_url) {
            should.not.exist(error);
            if (process.env.VERBOSE == 'loud') {
                console.log(error);
                console.log(resource_url);
            }

			packageConfig['Resource'] = resource_url;

            da.app_packages.create(packageConfig, function(error, success) {
				should.not.exist(error);
                if (process.env.VERBOSE == 'loud') {
                    console.log(success);
                }
                done();
            })
        })
    });

    it('appPackageTests-03 - should be able to get a single app_package', function(done) {
        da.app_packages.get(test_id, function(error, results) {
            should.not.exist(error);
            should.exist(results);
    		if (process.env.VERBOSE == 'loud') {
                console.log(results);
                console.log("===============");
                console.log(results.Parameters);
    		}
            done();
        });
    });

    it('appPackageTests-04 - should be able to delete a single app_package', function(done) {

    	da.app_packages.delete(test_id, function(error, results) {
    		should.not.exist(error);
    		should.exist(results);
    		if (process.env.VERBOSE == 'loud') {
                console.log(results);
    		}
    		done();
    	});
    });
});

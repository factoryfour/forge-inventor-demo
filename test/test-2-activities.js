const should = require('should');
const async = require('async');

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

describe('Activity Methods', function() {
    var authObj;
    var da;
    const auth = forge.auth(config);
	var test_id = 'TESTActivity';
	var test_package_id = 'TESTPackage';
    before(function(done) {
		this.slow(4000);
		this.timeout(5000);

        var scope = ['data:read', 'bucket:read', 'code:all']

        auth.two_leg(scope, function(error, cAuthObj) {
            if (error) {
                throw new Error(error.message ? error.message : error);
            }
            authObj = cAuthObj;
            da = forge.da(config, authObj);

			var filePath = __dirname + '/sample_files/samplePlugin.bundle.zip';
	        var packageConfig = require(__dirname + '/sample_configs/app_package.js')(test_package_id);

	        da.app_packages.pushBundle(filePath, function(error, resource_url) {
	            should.not.exist(error);
	            if (process.env.VERBOSE == 'loud') {
	                console.log(error);
	                console.log(resource_url);
	            }
				packageConfig['Resource'] = resource_url;
				console.log("Sample bundle pushed!");

	            da.app_packages.create(packageConfig, function(error, success) {
					console.log("Sample app package created!");
					should.not.exist(error);
	                if (process.env.VERBOSE == 'loud') {
	                    console.log(success);
	                }
	                done();
	            })
	        });
        });
    });

    it('activityTests-01 - should be able to list all activities', function(done) {
        da.activities.getAll(function(error, results) {
            should.not.exist(error);
            should.exist(results);
			if (process.env.VERBOSE == 'loud') {
	            console.log(results);
			}
            done();
        });
    });

    it('activityTests-02 - should be able to create an activity', function(done) {
        var activityConfig = require(__dirname + "/sample_configs/activity.js")(test_id, test_package_id);

        da.activities.create(activityConfig, function(error, results) {
            should.not.exist(error);
            should.exist(results);
			if (process.env.VERBOSE == 'loud') {
	            console.log(results);
			}
            done();
        });
    });

    it('activityTests-03 - should be able to get a single activity', function(done) {
        da.activities.get(test_id, function(error, results) {
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

	it('activityTests-04 - should be able to delete a single activity', function(done) {
		da.activities.delete(test_id, function(error, results) {
			should.not.exist(error);
			should.exist(results);
			if (process.env.VERBOSE == 'loud') {
	            console.log(results);
			}
			done();
		});
	});

	after(function(done) {
		da.app_packages.delete(test_package_id, function(error, results) {
			console.log("Sample app package deleted!");
    		should.not.exist(error);
    		should.exist(results);
    		if (process.env.VERBOSE == 'loud') {
                console.log(results);
    		}
    		done();
    	});
	});
});

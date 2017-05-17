const os = require('os');

function run_work_item(callback) {

	// Check platform to handle file path issues
	var isWin = os.platform().indexOf('win32') > -1
	if (isWin) {
		var root = __dirname.substring(0, __dirname.lastIndexOf('\\'));
	}
	else {
		var root = __dirname.substring(0, __dirname.lastIndexOf('/'));
	}
	// Initialize Forge interface
	const config = require(__dirname + "/get_config.js")(root + '/config.js');
	const forge = require(root + '/index.js');
	const auth = forge.auth(config);

	// Work item configuration JSON
	var workItemConfig = {
		Arguments: {
			InputArguments: [
				// Using the sample Box part
				{
					Resource: "https://s3-us-west-2.amazonaws.com/inventor-io-samples/Box.ipt",
					Name: "HostDwg",
					StorageProvider: "Generic",
					HttpVerb: "GET"
				},
				// Change the parameters
				{
					Resource:  'data:application/json,{\"d1\":\"0.3 in\", \"d2\":\"0.5 in\"}',
					Name: 'ChangeParameters',
					StorageProvider: 'Generic',
					ResourceKind: 'Embedded'
				}
			],
			// Output arguments
			OutputArguments: [
				{
					Name: "Result",
					StorageProvider: "Generic",
					HttpVerb: "POST"
				}
			]
		},
		ActivityId: "SampleActivity",
		Id: ""
	}

	// Declare scope
	var scope = ['data:read', 'bucket:read', 'code:all']
	// Get the auth token
	auth.two_leg(scope, function (error, cAuthObj) {
		if (error) {
			throw error;
		}
		// Set up design automation with auth object
		var da = forge.da(config, cAuthObj);

		// Create a work item
		da.work_items.create(workItemConfig, function (error, response) {
			// Log results of creating a work item
			console.log(error);
			console.log(response);
			if (error) {
				console.log("ERROR: CREATING WORK ITEM");
				return callback(error, response)
			}
			else {
				// Save the work item id
				var responseId = response.Id;
				// Check the status of the work item at a fixed interval
				var intervalObject = setInterval(function () {
					// Poll the work item's status
					da.work_items.get(responseId, function (error, response) {
						// Stop if there's an error
						if (error) {
							console.log("ERROR: CHECKING STATUS");
							clearInterval(intervalObject)
							return callback(error, response)
						}
						// If it is finished
						else if (!(response.Status == 'Pending' || response.Status == 'InProgress')) {
							clearInterval(intervalObject)
							return callback(error, response)
						}
						// Otherwise, log the status and repeat
						else {
							console.log(response.Status);
						}
					})

				}, 1000); // Check every 1 second
			}
		})
	});

}

// =============================================================================

run_work_item(function (error, response) {
	if (error) console.log(error);
	else {
		// console.log(response);
		console.log(response.Arguments.OutputArguments[0].Resource);
	}
})

module.exports = function(id, app_package) {
	return {
		AppPackages: [app_package],
		HostApplication: '',
		RequiredEngineVersion: '21.17',
		Parameters: {
			InputParameters: [{
				Name: 'HostDwg',
				LocalFileName: 'Box.ipt',
				Optional: null
			}, {
				Name: 'ChangeParameters',
				LocalFileName: 'changeParameters.json',
				Optional: null
			}],
			OutputParameters: [{
				Name: 'Result',
				LocalFileName: 'Output.stl',
				Optional: null
			}]
		},
		Instruction: {
			Script: "hi there",
			CommandLineParameters: 'changeParameters.json Output.stl'
		},
		AllowedChildProcesses: [],
		IsPublic: true,
		Version: 1,
		Timestamp: (new Date()).toISOString(),
		Description: 'A sample activity for testing purposes',
		Id: id
	}
};

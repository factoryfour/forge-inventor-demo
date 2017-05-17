module.exports = function(id, app_package) {
	return {
		AppPackages: [app_package],
		HostApplication: '',
		RequiredEngineVersion: '21.17',
		Parameters: {
			InputParameters: [{
				Name: 'HostDwg',
				LocalFileName: 'inputPart.ipt',
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
			Script: "Changing parameters in file.",
			CommandLineParameters: 'changeParameters.json Output.stl'
		},
		AllowedChildProcesses: [],
		IsPublic: true,
		Version: 1,
		Timestamp: (new Date()).toISOString(),
		Description: 'FactoryFour High Resolution Parameterization',
		Id: id
	}
};

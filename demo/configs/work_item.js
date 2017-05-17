module.exports = function(activity_id) {
    return {
        Arguments: {
            InputArguments: [{
                Resource: "https://s3-us-west-2.amazonaws.com/inventor-io-samples/Box.ipt",
                Name: "HostDwg",
                StorageProvider: "Generic"
            }, {
                Resource: 'data:application/json,{\"d2\":\"0.5 in\", \"d3\":\"0.2 in\"}',
                Name: 'ChangeParameters',
                StorageProvider: 'Generic',
				ResourceKind:'Embedded'
            }],
            OutputArguments: [{
                Name: "Result",
                StorageProvider: "Generic",
                HttpVerb: "POST"
            }]
        },
        ActivityId: activity_id,
        Id: ""
    }
};

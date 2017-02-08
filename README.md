### This repository contains:
- the Forge API wrapper
- a demo script for the design automation API that creates a WorkItem and polls its status until complete
- sample log files detailing different results

### To install and run:
Fill parameters in `config.js`.

```
npm install
npm test
node demo/demo_work_items.js
```

### Provided log files:
- `report_0.log`: a successful request to execute a work item
- `report_1.log`: a failed request with a FailedMissingOutput exception
- `report_2.log`: a failed request with a Succeeded result, but error when writing the .stl file

All work items were run using the following config JSON:
```
{
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
        // Write to a Zip package
        OutputArguments: [
            {
                Name: "Result",
                StorageProvider: "Generic",
                HttpVerb: "POST",
                ResourceKind: "ZipPackage"
            }
        ]
    },
    ActivityId: "SampleActivity",
    Id: ""
}
```

It was noted that the only difference with the successful request was that files were saved and writen to C:\ whereas all failed requests go to Z:\
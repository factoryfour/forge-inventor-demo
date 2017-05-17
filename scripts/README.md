# Change Parameters script

## Execution
``` 
node change_parameters.js ./sample_parameters.json job_name
```

## Formatting the parameters JSON
The parameters JSON file must contain the following fields:
- Part: URL to download the Inventor part to be modified
- Parameters: JSON object with "parameter_name": value

The value field can either be a number or a value with units, represented as a string (eg: 12 or "12 mm")

Sample parameters file:
``` javascript
{
    "Part": "https://static.factoryfour.com/pq/typeF_v12.ipt",
    "Parameters": {
        "SupLat_w": 65,
        "SupLat_h": 20,
        "SupMed_w": 4,
        "InfLatRoundingWidth": 12,
        "InfLat_h": 21,
        "InfMed_h": 17,
        "SupLatRoundingWidth": 12,
        "InfMedRoundingWidth": 12,
        "PupilDistance": 62
    }
}
```

## Output
When if begins processing...
```
{ '@odata.context': 'https://developer.api.autodesk.com/inventor.io/us-east/v2/$metadata#WorkItems/$entity',
  ActivityId: 'SampleActivity',
  Arguments:
   { InputArguments: [ [Object], [Object] ],
     OutputArguments: [ [Object] ] },
  Status: 'Pending',
  StatusDetails: { Report: null },
  AvailabilityZone: null,
  TimeQueued: '2017-03-29T19:58:30.5044755Z',
  TimeInputTransferStarted: null,
  TimeScriptStarted: null,
  TimeScriptEnded: null,
  TimeOutputTransferEnded: null,
  TimeOutputTransferEnded: null,
  BytesTranferredIn: null,
  BytesTranferredOut: null,
  Timestamp: '0001-01-01T00:00:00Z',
  Id: '673acb4115154e38ad6f272af54ab97d' }
```
The work item has started, now let's check on it...
```
    Pending
    Pending
    .
    .
    .
    Pending
    Pending
```
### When it's finished...

First, print some metadata from Forge
```
===== FINISHED: Output from Forge =====
{ 
    '@odata.context': 'https: //developer.api.autodesk.com/inventor.io/us-east/v2/$metadata#WorkItems/$entity',
    ActivityId: 'SampleActivity',
    Arguments: { InputArguments: [
            [Object],
            [Object]
        ],
     OutputArguments: [
            [Object]
        ]
    },
    Status: 'Succeeded',
    StatusDetails: { 
        Report: 'https: //inventorio-prod.s3-us-west-2.amazonaws.com/aces-workitem-reports/[WORK_ITEM_ID]/report.log?a_bunch_of_other_stuff' 
    },
    AvailabilityZone: null,
    TimeQueued: '2017-03-29T19: 58: 30.504Z',
    TimeInputTransferStarted: '2017-03-29T19: 58: 30.691Z',
    TimeScriptStarted: '2017-03-29T19: 58: 34.242Z',
    TimeScriptEnded: '2017-03-29T19: 59: 09.832Z',
    TimeOutputTransferEnded: '2017-03-29T19: 59: 09.988Z',
    BytesTranferredIn: 3349007,
    BytesTranferredOut: 928684,
    Timestamp: '2017-03-29T19: 59: 10.003Z',
    Id: 'asdfasdfasdfasdfasdfasdfasdf'
}
```

Then, print the link to the report file.
```
===== Process Report URL =====
https://inventorio-prod.s3-us-west-2.amazonaws.com/aces-workitem-reports/[WORK_ITEM_ID]/report.log?[a_bunch_of_other_stuff]
```

Finally, print the link to the modified STL
```
===== OUTPUT STL URL =====
https://inventorio-prod.s3-us-west-2.amazonaws.com/aces-workitem-outputs/[WORK_ITEM_ID]/Output.stl?[a_bunch_of_other_stuff]

```
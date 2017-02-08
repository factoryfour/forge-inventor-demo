var request = require('request');

module.exports = function(config, authObj) {
  var da = {};

	da.engines = require('./design_automation/engines.js')(config, authObj);
	da.app_packages = require('./design_automation/app_packages.js')(config, authObj);
	da.work_items = require('./design_automation/work_items.js')(config, authObj);
  da.activities = require('./design_automation/activities.js')(config, authObj);

  return da;

}

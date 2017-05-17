module.exports = function(file) {
	var config;
	try {
		config = require(file);
	} catch (e) {
		config = {
			CLIENT_ID: process.env.CLIENT_ID,
			CLIENT_SECRET: process.env.CLIENT_SECRET,
			DA_BASE_URL: process.env.DA_BASE_URL
		}
	} finally {
		if (!config.CLIENT_ID || !config.CLIENT_SECRET || !config.DA_BASE_URL) {
			throw new Error("Improperly configured config file or environment variables not set!");
		}
		return config;
	}
};

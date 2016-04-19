#include "config.h"

Config* config;

Config* getDefaultConfig() {
	Config* config = new Config();

	config->stepMode = StepFull;
	config->brakeMode = BrakeSmart;

	config->tickTime = 1900;
	config->homeTime = 3000;
	config->fixTime = 3000;

	config->maxSteps = 8000;
	config->homeSteps = 8000;
	config->fixSteps = 8000;

	config->brakeTicks = 10000;

	return config;
}
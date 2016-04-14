#include "config.h"

Config* config;

Config* getDefaultConfig() {
	Config* config = new Config();

	config->stepMode = StepFull;
	config->brakeMode = BrakeSmart;

	config->tickTime = 2000;
	config->homeTime = 3500;
	config->fixTime = 3500;

	config->maxSteps = 8000;
	config->homeSteps = 8000;
	config->fixSteps = 8000;

	return config;
}
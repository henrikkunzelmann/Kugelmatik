#include "config.h"

Config config;

void setDefaultConfig() {
	config.stepMode = StepHalf;
	config.brakeMode = BrakeSmart;
	
	config.tickTime = 3500;
	config.homeTime = 3500;
	config.fixTime = 3500;
	
	config.maxSteps = 8000;
	config.homeSteps = 8000;
	config.fixSteps = 8000;
	
	config.brakeTicks = 10000;

	config.minStepDelta = 10;

	config.turnWaitTime = 20;

	if (!checkConfig(&config))
		internalError(ERROR_INTERNAL_DEFAULT_CONFIG_FAULT);
}

boolean checkConfig(Config* config) {
	boolean correct = true;
	correct &= config->stepMode >= StepHalf && config->stepMode <= StepBoth;
	correct &= config->brakeMode >= BrakeNone && config->brakeMode <= BrakeSmart;

	correct &= config->tickTime >= 500 && config->tickTime <= 5000;
	correct &= config->homeTime >= 500 && config->homeTime <= 5000;
	correct &= config->fixTime >= 500 && config->fixTime <= 5000;

	correct &= config->maxSteps > 0 && config->maxSteps <= 15000;
	correct &= config->homeSteps > 0 && config->homeSteps <= 10000;
	correct &= config->fixSteps > 0 && config->fixSteps <= 10000;

	return correct;
}
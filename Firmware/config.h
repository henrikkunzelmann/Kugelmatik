#ifndef _CONFIG_h
#define _CONFIG_h

#include "arduino.h"
#include "stepper.h"

enum StepMode : byte {
	StepHalf = 1,
	StepFull = 2,
	StepBoth = 3
};

enum BrakeMode : byte {
	BrakeNone,
	BrakeAlways,
	BrakeSmart
};

struct Config {
	StepMode stepMode; 
	BrakeMode brakeMode;
	
	uint32_t tickTime;
	uint32_t homeTime;
	uint32_t fixTime;

	uint32_t maxSteps; // Maximale Anzahl an Schritten die die Firmware maximal machen darf (nach unten)
	uint32_t fixSteps; // Anzahl an Schritten die die Firmware macht um eine Kugel nach unten zu fahren (ignoriert dabei maxSteps)
};

// gibt die Standard Config zurück
Config* getDefaultConfig();

extern Config* config;
#endif
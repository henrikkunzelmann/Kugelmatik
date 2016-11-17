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
	BrakeNone = 0,
	BrakeAlways = 1,
	BrakeSmart = 2
};

struct Config {
	StepMode stepMode; 
	BrakeMode brakeMode;
	
	uint32_t tickTime;
	uint32_t homeTime;
	uint32_t fixTime;

	int16_t maxSteps; // Maximale Anzahl an Schritten die die Firmware maximal machen darf (nach unten)
	int16_t homeSteps;
	int16_t fixSteps; // Anzahl an Schritten die die Firmware macht um eine Kugel nach unten zu fahren (ignoriert dabei maxSteps)

	uint16_t brakeTicks;
};

// gibt die Standard Config zurück
void setDefaultConfig();

// prüft Config auf invalide Werte
boolean checkConfig(Config* config);

extern Config config;
#endif
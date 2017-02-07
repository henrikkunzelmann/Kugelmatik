#pragma once

#include <Arduino.h>
#include <MCP23017.h>

#include "util.h"
#include "config.h"
#include "network.h"

#define MCP_COUNT 8				// Anzahl der MCP Chips
#define MCP_STEPPER_COUNT 4		// Anzahl der Stepper pro MCP Chip

#define IGNORE_MCP_FAULTS false

#define CLUSTER_WIDTH 5		// Anzahl Stepper in der Breite (X)
#define CLUSTER_HEIGHT 6	// Anzahl Stepper in der Höhe (Y)

#define CLUSTER_SIZE (CLUSTER_WIDTH * CLUSTER_HEIGHT)

struct StepperData
{
	int32_t LastRevision;		// letzte Revision der Daten
	int16_t CurrentSteps;		// derzeitige Schritte die der Motor gemacht hat (= Höhe)
	int16_t GotoSteps;			// Schritte zu der der Motor gehen soll (= zu welche Höhe die Kugel fahren soll)
	uint8_t CurrentStepIndex;	// derzeitiger Stepper Wert Index, siehe stepsStepper
	int16_t TickCount;			// derzeitige Tick Anzahl, wenn kleiner als 0 dann wird ein Schritt gemacht und die Variable auf WaitTime gesetzt
	uint8_t WaitTime;			// Wert für TickCount nach jedem Schritt
	uint16_t BrakeTicks;		// Anzahl der Ticks seit letzter Bewegung
};

struct MCPData
{
	StepperData Steppers[MCP_STEPPER_COUNT]; // Schrittmotoren pro MCP
	MCP23017* MCP;
	boolean isOK;
	uint16_t lastGPIOValue;
};

// Anweisung die der Schrittmotor machen soll
#define STEPPER_STEP_COUNT 8 // es gibt 8 Anweisungen für den Schrittmotor, siehe stepsStepper
const uint8_t stepsStepper[STEPPER_STEP_COUNT] = { 0x05, 0x04, 0x06, 0x02, 0x0A, 0x08, 0x09, 0x01 };

// Gibt die Position des MCPs an
const uint8_t mcpPosition[CLUSTER_SIZE] = { 6, 6, 5, 5, 4, 6, 6, 5, 5, 4, 7, 7, 1, 2, 2, 7, 7, 1, 2, 2, 0, 0, 1, 3, 3, 0, 0, 1, 3, 3 };

// Gibt die Position des Steppers an
const uint8_t stepperPosition[CLUSTER_SIZE] = { 2, 3, 2, 3, 1, 1, 0, 1, 0, 0, 1, 0, 3, 2, 3, 2, 3, 2, 1, 0, 2, 3, 1, 2, 3, 1, 0, 0, 1, 0 };

extern MCPData mcps[MCP_COUNT];

void initAllMCPs();			// initialisiert alle MCPs
void initMCP(byte index);	// initialisiert einen MCP

StepperData* getStepper(byte x, byte y);
StepperData* getStepper(int index);

// prüft ob die Höhe eine besondere Bedeutung hat und nicht minStepDelta benutzt wird
boolean isSpecialHeight(uint16_t height);

// überprüft einen Stepper auf richtige Werte
void checkStepper(StepperData* stepper);

// setzt den Schrittmotor auf Standard Werte zurück (Höhe = 0)
void resetStepper(StepperData* stepper);
// setzt den Schrittmotor auf eine bestimmte Höhe welche nicht geprüft wird ob sie größer als MaxSteps ist
void forceStepper(StepperData* stepper, int32_t revision, int16_t height);

// setzt den Schrittmotor auf eine bestimmte Höhe und Wartezeit
void setStepper(StepperData* stepper, int32_t revision, uint16_t height, byte waitTime);

// setzt einen Schrittmotoren auf eine bestimmte Höhe
void setStepper(int32_t revision, byte x, byte y, uint16_t height, byte waitTime); 

// setzt alle Schrittmotoren auf eine bestimmte Höhe
void setAllSteps(int32_t revision, uint16_t height, byte waitTime);

// stoppt alle Schrittmotoren (setzt GotoSteps auf die aktuelle Höhe)
void stopMove();

// spricht die Schrittmotoren an und lässt sie drehen
void updateSteppers(boolean alwaysUseHalfStep);
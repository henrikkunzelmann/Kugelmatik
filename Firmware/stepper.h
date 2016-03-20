#ifndef _STEPPER_h
#define _STEPPER_h

#include "arduino.h"
#include <MCP23017.h>

#include "util.h"
#include "config.h"
#include "network.h"

#define MCP_COUNT 8			// Anzahl der MCP Chips
#define STEPPER_COUNT 4		// Anzahl der Stepper pro MCP Chip

#define CLUSTER_WIDTH 5		// Anzahl Stepper in der Breite (X)
#define CLUSTER_HEIGHT 6	// Anzahl Stepper in der Höhe (Y)

struct StepperData
{
	int32_t LastRevision;		// letzte Revision der Daten
	int32_t CurrentSteps;		// derzeitige Schritte die der Motor gemacht hat
	int32_t GotoSteps;			// Schritte zu der der Motor gehen soll
	uint8_t CurrentStepIndex;	// derzeitiger Stepper Wert Index, siehe stepsStepper
	int32_t TickCount;			// derzeitige Tick Anzahl, wenn kleiner als 0 dann wird ein Schritt gemacht und die Variable auf WaitTime gesetzt
	uint8_t WaitTime;			// Wert für TickCount nach jedem Schritt
};

struct MCPData
{
	StepperData Steppers[STEPPER_COUNT]; // Schrittmotoren pro MCP
	uint8_t MCPAddress;
	MCP23017 MCP;
};

// Anweisung die der Schrittmotor machen soll
#define STEPPER_STEP_COUNT 8 // es gibt 8 Anweisungen für den Schrittmotor, siehe stepsStepper
const uint8_t stepsStepper[STEPPER_STEP_COUNT] = { 0x05, 0x04, 0x06, 0x02, 0x0A, 0x08, 0x09, 0x01 };

// geben die Position des MCPs und des Steppers für jede Kugel an
// erster Index ist die linke untere Kugel, siehe "Cluster_Kugelpositionen.pdf" in "Handbücher"
const uint8_t mcpPosition[CLUSTER_WIDTH * CLUSTER_HEIGHT] = { 6, 6, 5, 5, 4, 6, 6, 5, 5, 4, 7, 7, 1, 2, 2, 7, 7, 1, 2, 2, 0, 0, 1, 3, 3, 0, 0, 1, 3, 3 };
const uint8_t stepperPosition[CLUSTER_WIDTH * CLUSTER_HEIGHT] = { 2, 3, 2, 3, 1, 1, 0, 1, 0, 0, 1, 0, 3, 2, 3, 2, 3, 2, 1, 0, 2, 3, 1, 2, 3, 1, 0, 0, 1, 0 };

extern MCPData mcps[MCP_COUNT];

void initAllMCPs();			// initialisiert alle MCPs
void initMCP(byte index);	// initialisiert einen MCP

// setzt einen Schrittmotoren auf eine bestimmte Höhe
void setStepper(int32_t revision, byte x, byte y, uint16_t height, byte waitTime); 

// setzt alle Schrittmotoren auf eine bestimmte Höhe
void setAllSteps(int32_t revision, uint16_t gotoSteps, byte waitTime);

// stoppt alle Schrittmotoren
void stopMove();

void updateSteppers(boolean alwaysUseHalfStep);
#endif
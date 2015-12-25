// cluster_led.h

#ifndef _CLUSTER_LED_h
#define _CLUSTER_LED_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

#define LED_GREEN 2		// Port für grüne LED (Pin 4 SUBD)
#define LED_RED 3		// Port für rote LED (Pin 5 SUBD)

void turnGreenLedOn();
void turnGreenLedOff();

void turnRedLedOn();
void turnRedLedOff();

void toogleRedLed();
void toogleGreenLed();

void blinkRedLedShort();
void blinkGreenLedShort();
void blinkBothLedsShort();

#endif


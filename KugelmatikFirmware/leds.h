#pragma once

#include <Arduino.h>

#include "watchdog.h"

#define LED_GREEN 2		// Pin für grüne LED 
#define LED_RED 16		// Pin für rote LED

#define LED_STATE_ON false // NodeMCU LEDs leuchtet bei LOW
#define LED_STATE_OFF (!LED_STATE_ON)

// Zeit in Millisekunden für schnelles blinken
#define TIME_FAST 150
#define TIME_SLOW 500

// initialisiert die Pins für die LEDs
void setupLeds(); 

void turnGreenLedOn();
void turnGreenLedOff();

void turnRedLedOn();
void turnRedLedOff();

void toogleRedLed();
void toogleGreenLed();

void blinkRedLedShort(boolean fast);
void blinkGreenLedShort(boolean fast);
void blinkBothLedsShort(boolean fast);
#ifndef _UTIL_h
#define _UTIL_h

#include "arduino.h"
#include <avr/wdt.h> 

#include "leds.h"
#include "constants.h"

extern byte lastError;

void softReset(); // setzt den Chip zurück

// bringt den Chip in den Fehler-Modus und blockiert ihn 
void error(const char* tag, const char* message, bool blinkFast);
void internalError();
void internalError(uint8_t error);
void protocolError(uint8_t error);

char getHexChar(int x);

void usdelay(uint16_t us);

void startTime(uint8_t index);
int32_t endTime(uint8_t index);

// gibt den freien SRAM zurück, siehe http://playground.arduino.cc/Code/AvailableMemory
int freeRam();
#endif
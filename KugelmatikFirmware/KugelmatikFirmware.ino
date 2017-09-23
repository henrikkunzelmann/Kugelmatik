// Kugelmatik V3.1 (ESP32)
// Firmware
//  Henrik Kunzelmann 2016 - 2017
//  Rainer Wieland

// Hardware
//  ESP32
//  MCP23S17
//  L293DNE

// Includes
#include <limits.h>
#include <NewMCP23017.h>

#include "constants.h"
#include "util.h"
#include "leds.h"
#include "config.h"
#include "stepper.h"
#include "network.h"
#include "tick.h"
#include "watchdog.h"
#include "PacketBuffer.h"
#include "BinaryHelper.h"
#include "serial.h"

void setup()
{
	disable_wdt();

	serialBegin();
	serialPrintF("Kugelmatik Firmware booting up, version: ");
	serialPrintln(BUILD_VERSION);

	for (int i = 0; i < 200; i++)
		writeEEPROM((uint8_t)'A');

	initEEPROM();

	writeEEPROM("Kugelmatik");

	setupLeds();
	turnGreenLedOn();

	setDefaultConfig();
	initNetwork();
	initAllMCPs();

	turnGreenLedOff();

	wdt_yield();
	serialPrintlnF("Done booting! Ready.");

	writeEEPROM("ready");
}

void loop()
{
	startTime(TIMER_LOOP);
	runTick(config.tickTime, false);
	loopTime = endTime(TIMER_LOOP);

	wdt_yield();
}
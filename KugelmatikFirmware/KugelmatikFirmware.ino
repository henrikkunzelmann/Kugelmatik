// Kugelmatik V3.1
// Firmware
//  Henrik Kunzelmann 2016 - 2017
//  Rainer Wieland

// Hardware
//  AVR-NET-I/O mit ATmega32 und ENC28J60
//  MCP23S17
//  L293DNE

// Defines
#define ETHERCARD_TCPCLIENT 0
#define ETHERCARD_TCPSERVER 0
#define ETHERCARD_STASH 0

// Includes
#include <limits.h>
#include <EtherCard.h>
#include <I2C.h>
#include <MCP23017.h>

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

void setup()
{
	Serial.begin(1200);
	Serial.print(F("Kugelmatik Firmware booting up, version: "));
	Serial.println(BUILD_VERSION);

	setupLeds();
	turnGreenLedOn();

	setDefaultConfig();
	initNetwork();
	initAllMCPs();

	turnGreenLedOff();

	wdt_yield();
	Serial.println(F("Done booting! Ready."));
}

void loop()
{
	startTime(TIMER_LOOP);
	runTick(config.tickTime, false);
	loopTime = endTime(TIMER_LOOP);
}
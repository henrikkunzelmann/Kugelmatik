// Kugelmatik V3.1
// Firmware
//  Henrik Kunzelmann 2016 - 2017
//  Rainer Wieland

// Hardware
//  AVR-NET-I/O mit ATmega32 und ENC28J60
//  MCP23S17
//  L293DNE

// Project Defines
// #define ETHERCARD_TCPCLIENT 0
// #define ETHERCARD_TCPSERVER 0
// #define ETHERCARD_STASH 0
// #define SERIAL_TX_BUFFER_SIZE 8
// #define SERIAL_RX_BUFFER_SIZE 8

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
#include "serial.h"

void setup()
{
	disable_wdt();

	serialBegin();
	serialPrintF("Kugelmatik Firmware booting up, version: ");
	serialPrintln(BUILD_VERSION);

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
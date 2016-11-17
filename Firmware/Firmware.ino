// Kugelmatik V3.1
// Firmware
//  Henrik Kunzelmann 2016
//  Rainer Wieland

// Hardware
//  AVR-NET-I/O mit ATmega32 und ENC28J60
//  MCP23S17
//  L293DNE

// Defines
#define ENABLE_WATCH_DOG false		// gibt an ob der WatchDog aktiviert sein soll, der Chip wird resetet wenn er sich aufhängt

#define ETHERCARD_TCPCLIENT 0
#define ETHERCARD_TCPSERVER 0
#define ETHERCARD_STASH 0

// Includes
#include <avr/pgmspace.h>
#include <avr/wdt.h> 
#include <avr/sleep.h>
#include <avr/interrupt.h>
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
#include "PacketBuffer.h"
#include "BinaryHelper.h"

void setup()
{
	wdt_disable(); // Watch Dog deaktivieren, da er noch aktiviert sein kann

	Serial.begin(9600);
	Serial.print(F("Kugelmatik Firmware booting up, version: "));
	Serial.println(BUILD_VERSION);

	setupLeds();
	turnGreenLedOn();

	setDefaultConfig();
	initNetwork();
	initAllMCPs();

	turnGreenLedOff();

#if ENABLE_WATCH_DOG
	wdt_enable(WDTO_2S);
#endif

	Serial.println(F("Done booting! Ready."));
}

void loop()
{
	unsigned long procStart = micros();

	updateSteppers(false);

	while (true)
	{
		wdt_reset();
		loopNetwork();

		// schauen ob wir die Stepper updaten müssen
		unsigned long time = micros();
		if (time < procStart) // overflow von micros() handeln
			break;

		if (time - procStart >= config.tickTime)
			break;
	}
}
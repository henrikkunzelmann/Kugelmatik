// Kugelmatik V3.1
// Firmware
//  Henrik Kunzelmann 2016
//  Rainer Wieland

// Hardware
//  AVR-NET-I/O mit ATmega32 und ENC28J60
//  MCP23S17
//  L293DNE

// Defines
#define ENABLE_WATCH_DOG true		// gibt an ob der WatchDog aktiviert sein soll, der Chip wird resetet wenn er sich aufhängt

#define ALLOW_STOP_BUSY true		// gibt an ob der Client "busy"-Befehle beenden darf (z.B. Home)
#define ALLOW_STOP_MOVE true		// gibt an ob der Client die Bewegung stoppen darf
#define RECEIVE_PACKETS_BUSY true	// gibt an ob der Client bei "busy"-Befehle Pakete empfängt

#define BLINK_PACKET false			// Wenn true, dann blinkt die grüne Led wenn ein Kugelmatik Paket verarbeitet wird

// Includes
#include <avr/pgmspace.h>
#include <avr/wdt.h> 
#include <avr/sleep.h>
#include <avr/interrupt.h>
#include <limits.h>
#include <EtherCard.h>
#include <EEPROM.h>
#include <I2C.h>
#include <MCP23017.h>
#include "constants.h"
#include "util.h"
#include "log.h"
#include "leds.h"
#include "config.h"
#include "stepper.h"
#include "network.h"

void setup()
{
	wdt_disable(); // Watch Dog deaktivieren, da er noch aktiviert sein kann

	config = getDefaultConfig();

	setupLeds();

	turnGreenLedOn();
	initAllMCPs();

	delay(LAN_ID * 10); // Init verzögern damit das Netzwerk nicht überlastet wird

	initNetwork();

	turnGreenLedOff();

#if ENABLE_WATCH_DOG
	wdt_enable(WDTO_2S);
#endif
}

void loop()
{
	unsigned long procStart = micros();

	updateSteppers(false);

	while (true)
	{
		// Packet abfragen
		word plen = ether.packetReceive();
		if (plen > 0) // neues Packet empfangen
			ether.packetLoop(plen);

#if BLINK_PACKET
		turnGreenLedOff();
#endif

		wdt_reset();

		// schauen ob wir die Stepper updaten müssen
		unsigned long time = micros();
		if (time < procStart) // overflow von micros() handeln
			break;

		if (time - procStart >= config->tickTime)
			break;
	}
}
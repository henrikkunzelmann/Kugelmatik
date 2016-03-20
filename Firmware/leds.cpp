// 
// 
// 

#include "leds.h"
#include <avr/wdt.h>



/// Funktionen für die LEDs
boolean ledStateGreen = false; // Status der LED für LED_Green (grüne LED)
boolean ledStateRed = false; // Status der LED für LED_Red (rote LED)

void setupLeds()
{
	pinMode(LED_GREEN, OUTPUT);
	pinMode(LED_RED, OUTPUT);
}

// setzt die grüne LED an
void turnGreenLedOn()
{
	ledStateGreen = true;
	digitalWrite(LED_GREEN, ledStateGreen);
}

// setzt die grüne LED aus
void turnGreenLedOff()
{
	ledStateGreen = false;
	digitalWrite(LED_GREEN, ledStateGreen);
}

// wechselt den Status der grünen LED
void toogleGreenLed()
{
	ledStateGreen = !ledStateGreen;
	digitalWrite(LED_GREEN, ledStateGreen);
}

// stellt die rote LED an
void turnRedLedOn()
{
	ledStateRed = true;
	digitalWrite(LED_RED, ledStateRed);
}

// stellt die rote LED aus
void turnRedLedOff()
{
	ledStateRed = false;
	digitalWrite(LED_RED, ledStateRed);
}

// wechselt den Status der rote LED
void toogleRedLed()
{
	ledStateRed = !ledStateRed;
	digitalWrite(LED_RED, ledStateRed);
}

// lässt die grüne Led kurzzeitig blinken
void blinkGreenLedShort()
{
	for (byte i = 0; i < 5; i++)
	{
		turnGreenLedOn();
		delay(200);
		turnGreenLedOff();
		delay(200);
		wdt_reset();
	}
}

// läst die rote Led kurzzeitig blinken
void blinkRedLedShort()
{
	for (byte i = 0; i < 5; i++)
	{
		turnRedLedOn();
		delay(200);
		turnRedLedOff();
		delay(200);
		wdt_reset();
	}
}

// lässt beide LEDs kurzzeitig blinken
void blinkBothLedsShort()
{
	for (byte i = 0; i < 5; i++)
	{
		turnRedLedOn();
		turnGreenLedOn();
		delay(200);
		turnRedLedOff();
		turnGreenLedOff();
		delay(200);
		wdt_reset();
	}
}
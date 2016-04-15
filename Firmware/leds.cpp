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
void blinkGreenLedShort(boolean fast)
{
	int time = fast ? 250 : 500;
	for (byte i = 0; i < 3; i++)
	{
		turnGreenLedOn();
		delay(time);
		turnGreenLedOff();
		delay(time);
		wdt_reset();
	}
}

// läst die rote Led kurzzeitig blinken
void blinkRedLedShort(boolean fast)
{
	int time = fast ? 250 : 500;
	for (byte i = 0; i < 3; i++)
	{
		turnRedLedOn();
		delay(time);
		turnRedLedOff();
		delay(time);
		wdt_reset();
	}
}

// lässt beide LEDs kurzzeitig blinken
void blinkBothLedsShort(boolean fast)
{
	int time = fast ? 250 : 500;
	for (byte i = 0; i < 3; i++)
	{
		turnRedLedOn();
		turnGreenLedOn();
		delay(time);
		turnRedLedOff();
		turnGreenLedOff();
		delay(time);
		wdt_reset();
	}
}
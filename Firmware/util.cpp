#include "util.h"

byte lastError = ERROR_NONE;

void softReset()
{
	Serial.println(F("softReset()"));
	wdt_enable(WDTO_2S);	// Watch Dog aktivieren damit der Chip zurück gesetzt wird
	while (true);			// in Endlosschleife gehen damit der Watch Dog den Chip resetet
}

// bringt den Chip in den Fehler-Modus und blockiert ihn 
void error(const char* tag, const char* message, bool blinkFast)
{
	Serial.print(F("error(tag = "));
	Serial.print(tag);
	Serial.print(F(", message = "));
	Serial.print(message);
	Serial.println(F(")"));

	while (true) {
		toogleRedLed();

		if (blinkFast)
			delay(250);
		else
			delay(500);
		wdt_reset();
	}
}

void internalError()
{
	internalError(ERROR_INTERNAL);
}

void internalError(uint8_t error)
{
	Serial.print(F("protocolError(error = "));
	Serial.print(error);
	Serial.println(")");

	lastError = error;
	blinkRedLedShort(true);
}

void protocolError(uint8_t error)
{
	Serial.print(F("protocolError(error = "));
	Serial.print(error);
	Serial.println(")");

	lastError = error;
	blinkRedLedShort(true);
}

void usdelay(uint16_t us) {
	while (us--) {
		// 4 times * 4 cycles gives 16 cyles = 1 us with 16 MHz clocking
		byte i = 4;
		// this while loop executes with exact 4 cycles:
		while (i--)
			_NOP();
	}
}

int freeRam() {
	extern int __heap_start, *__brkval;
	int v;
	return (int)&v - (__brkval == 0 ? (int)&__heap_start : (int)__brkval);
}

#include "util.h"

uint8_t lastError = ERROR_NONE;

// bringt den Chip in den Fehler-Modus und blockiert ihn 
void error(const char* tag, const char* message, boolean blinkFast)
{
	writeEEPROM("fError");
	writeEEPROM(message);

	serialPrintF("error(tag = ");
	serialPrint(tag);
	serialPrintF(", message = ");
	serialPrint(message);
	serialPrintlnF(")");

	while (true) {
		toogleRedLed();

		if (blinkFast)
			delay(250);
		else
			delay(500);
		wdt_yield();
	}
}

void internalError()
{
	internalError(ERROR_INTERNAL);
}

void internalError(uint8_t error)
{
	writeEEPROM("iError");
	writeEEPROM(error);

	wdt_yield();
	serialPrintF("Error(error = ");
	serialPrint(error);
	serialPrintlnF(")");

	lastError = error;
	blinkRedLedShort(true);
}

void protocolError(uint8_t error)
{
	writeEEPROM("pError");
	writeEEPROM(error);

	wdt_yield();
	serialPrintF("protocolError(error = ");
	serialPrint(error);
	serialPrintlnF(")");

	lastError = error;
	blinkRedLedShort(true);
}

char getHexChar(int32_t x)
{
	x &= 0xF;
	if (x >= 10)
		return 'A' + (x - 10);
	return '0' + x;
}

unsigned long time[TIMER_COUNT];

void startTime(uint8_t index) {
	if (index >= TIMER_COUNT)
		return internalError(ERROR_INTERNAL_INVALID_TIMER);

	time[index] = micros();
}

int32_t endTime(uint8_t index) {
	if (index >= TIMER_COUNT) {
		internalError(ERROR_INTERNAL_INVALID_TIMER);
		return 0;
	}

	unsigned long end = micros();
	if (end > time[index])
		return end - time[index];
	return 0;
}

uint32_t freeRam() {
	return ESP.getFreeHeap();
}

uint32_t logPosition = 16;

void initEEPROM()
{
	logPosition = 16;
}

void writeEEPROM(uint8_t value)
{
#if ENABLE_EEPROM_LOG
	logPosition = logPosition % 1000;
	while (!eeprom_is_ready())
		wdt_yield();

	_EEPUT(logPosition++, value);
#endif
	wdt_yield();
}

void writeEEPROM(const char * str)
{
	uint32_t len = strlen(str);

	for (int i = 0; i < len; i++)
		writeEEPROM((uint8_t)str[i]);

	writeEEPROM((uint8_t)' ');
}
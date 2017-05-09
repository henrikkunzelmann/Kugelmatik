#include "tick.h"

boolean runTick(uint32_t tickTime, boolean isUsedByBusyCommand) {
	unsigned long start = micros();
	updateSteppers(isUsedByBusyCommand);

	while (true) {
		wdt_yield();

		if (!loopNetwork())
			return false;

		// schauen ob wir die Stepper updaten müssen
		unsigned long time = micros();
		if (time < start) // overflow von micros() handeln
			break;

		unsigned long timeRan = time - start;
		if (timeRan >= tickTime)
			break;

		int32_t timeToWait = tickTime - timeRan;
		if (maxNetworkTime >= timeToWait) {
			delayMicroseconds(timeToWait);
			break;
		}
	}
	return true;
}
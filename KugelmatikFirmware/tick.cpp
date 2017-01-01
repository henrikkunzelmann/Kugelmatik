#include "tick.h"

boolean runTick(uint32_t tickTime, boolean useHalfStep) {
	unsigned long start = micros();
	updateSteppers(useHalfStep);

	while (true) {
		wdt_reset();

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
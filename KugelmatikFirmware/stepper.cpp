#include "stepper.h"

MCPData mcps[MCP_COUNT];
StepperData stepperData[MCP_STEPPER_COUNT];

void initAllMCPs()
{
	writeEEPROM("initMcps");

	serialPrintlnF("initAllMCPs()");

	for (uint8_t i = 0; i < MCP_COUNT; i++)
		initMCP(i);
}

void initMCP(uint8_t index)
{
	writeEEPROM("mcp");
	writeEEPROM(index);

	serialPrintF("Init mcp number ");
	serialPrintln(index);

	MCPData* data = &mcps[index];

	for (uint8_t i = 0; i < MCP_STEPPER_COUNT; i++)
	{
		StepperData* stepper = &data->steppers[i];
		memset(stepper, 0, sizeof(StepperData));
	}

	wdt_yield();

	// MCP initialisieren
	data->mcpChip.begin(index);

	// alle Pins (0xFFFF) auf OUTPUT stellen
	data->mcpChip.setPinsOutput(0xFFFFu);

	data->isOK = (data->mcpChip.writeGPIO(0) == 0);
	data->lastGPIOValue = 0;

#if !IGNORE_MCP_FAULTS
	if (!data->isOK)
	{
		serialPrintF("MCP Fault with mcp number ");
		serialPrintln(index);

		internalError(ERROR_MCP_FAULT_1 + index);

#if BLINK_MCP_FAULTS
		turnRedLedOn();
		for (uint8_t i = 0; i <= index; i++)
		{
			turnGreenLedOn();
			delay(TIME_SLOW);
			turnGreenLedOff();
			delay(TIME_SLOW);
		}
		turnRedLedOff();
#endif
	}
#else
	serialPrintlnF("IGNORE_MCP_FAULTS is set to true, skipping MCP boot test");
#endif
	wdt_yield();
}

StepperData* getStepper(uint8_t x, uint8_t y)
{
	if (x >= CLUSTER_WIDTH || y >= CLUSTER_HEIGHT)
		internalError(ERROR_INTERNAL_WRONG_PARAMETER);
	return getStepper(y * CLUSTER_WIDTH + x);
}

StepperData* getStepper(int32_t index)
{
	if (index < 0 || index >= CLUSTER_SIZE)
		internalError(ERROR_INTERNAL_WRONG_PARAMETER);
	MCPData* data = mcps + mcpPosition[index];
	return data->steppers + stepperPosition[index];
}

boolean isSpecialHeight(int32_t height)
{
	return height == 0 || height == config.maxSteps || height % 100 == 0;
}

void checkStepper(StepperData* stepper)
{
	if (stepper->CurrentSteps < 0 || stepper->CurrentSteps > config.maxSteps) {
		internalError(ERROR_INTERNAL_LOOP_VALUES_WRONG);
		stepper->CurrentSteps = 0;
	}
	if (stepper->GotoSteps < 0 || stepper->GotoSteps > config.maxSteps) {
		internalError(ERROR_INTERNAL_LOOP_VALUES_WRONG);
		stepper->GotoSteps = 0;
	}
	if (stepper->CurrentStepIndex >= STEPPER_STEP_COUNT) {
		internalError(ERROR_INTERNAL_LOOP_VALUES_WRONG);
		stepper->CurrentStepIndex = 0;
	}
}

void resetStepper(StepperData* stepper)
{
	stepper->CurrentSteps = 0;
	stepper->GotoSteps = 0;
	stepper->TickCount = 0;
	stepper->WaitTime = 0;
	stepper->BrakeTicks = 0;
}

void forceStepper(StepperData* stepper, int32_t revision, int16_t height)
{
	resetStepper(stepper);
	stepper->LastRevision = revision;
	stepper->GotoSteps = height;
}

void setStepper(StepperData* stepper, int32_t revision, int16_t height, uint8_t waitTime)
{
	if (checkRevision(stepper->LastRevision, revision))
	{
		stepper->LastRevision = revision;

		int32_t diff = abs(stepper->CurrentSteps - (int32_t)height);
		if (isSpecialHeight(height) || diff >= config.minStepDelta)
		{
			stepper->GotoSteps = height;
			stepper->TickCount = 0;
			stepper->WaitTime = waitTime;
		}
	}
	wdt_yield();
}


void setStepper(int32_t revision, uint8_t x, uint8_t y, int16_t height, uint8_t waitTime)
{
	if (x < 0 || x >= CLUSTER_WIDTH)
		return protocolError(ERROR_X_INVALID);
	if (y < 0 || y >= CLUSTER_HEIGHT)
		return protocolError(ERROR_Y_INVALID);
	if (height > config.maxSteps)
		return protocolError(ERROR_INVALID_HEIGHT);

	setStepper(getStepper(x, y), revision, height, waitTime);
}

void setAllSteps(int32_t revision, int16_t height, uint8_t waitTime)
{
	if (height > config.maxSteps)
		return protocolError(ERROR_INVALID_HEIGHT);
	for (int32_t i = 0; i < CLUSTER_SIZE; i++) 
		setStepper(getStepper(i), revision, height, waitTime);
}


void stopMove() {
	serialPrintlnF("stopMove()");

	for (int32_t i = 0; i < CLUSTER_SIZE; i++) {
		StepperData* stepper = getStepper(i);

		// stoppen
		stepper->GotoSteps = stepper->CurrentSteps;
		stepper->TickCount = 0;
	}
}


void updateSteppers(boolean isUsedByBusyCommand)
{
	startTime(TIMER_STEPPER);
	for (uint8_t i = 0; i < MCP_COUNT; i++)
	{
		uint16_t gpioValue = 0;

		MCPData* mcp = &mcps[i];

		// StepperData sichern um bei Fehler wieder zurückzusetzen
		memcpy(stepperData, mcp->steppers, sizeof(stepperData));

		for (uint8_t j = 0; j < MCP_STEPPER_COUNT; j++)
		{
			StepperData* stepper = &mcp->steppers[j];

			// bei einem BusyCommand werden Stepper nicht ueberprueft
			// da diese Befehle besondere Werte als Hoehe setzen
			// und diese ungueltig im normalen Ablauf sind
			if (!isUsedByBusyCommand)
				checkStepper(stepper);

			uint8_t stepSize = 0;
			int32_t diff = abs(stepper->CurrentSteps - stepper->GotoSteps);

			if (diff != 0)
			{
				if (config.stepMode == StepBoth) 
					stepSize = _min(2, diff);		// schauen ob Full oder Half Step gemacht werden soll
				else if (diff >= config.stepMode)	
					stepSize = config.stepMode;	    // Half oder Full Step machen
			}

			// immer HalfStep machen
			if (isUsedByBusyCommand)
				stepSize = _min(stepSize, 1);

			boolean waitStep = stepper->TickCount > 0;
			if (stepper->TickCount > 0)
				stepper->TickCount--;

			// Die gefahrene Richtung zurücksetzen
			if (stepper->TurnWaitTime > 0) {
				stepper->TurnWaitTime--;

				if (stepper->TurnWaitTime == 0)
					stepper->Direction = DirectionNone;
			}

			// bestimmen Welche Richtung wir fahren müssen
			StepperDirection moveDirection = DirectionNone;
			if (stepper->GotoSteps < stepper->CurrentSteps)
				moveDirection = DirectionUp;
			else if (stepper->GotoSteps > stepper->CurrentSteps)
				moveDirection = DirectionDown;

			// Kugel müsste sich in die andere Richtung bewegen
			// nicht bewegen
			if (stepper->Direction != DirectionNone && stepper->Direction != moveDirection)
				waitStep = true;

			bool writeGPIO = false;

			if (stepSize > 0 && !waitStep)
			{
				int8_t stepperIndex = stepper->CurrentStepIndex;
				if (stepperIndex % stepSize > 0) // schauen ob wir noch einen Zwischenschritt machen müssen
					stepSize = 1;

				if (moveDirection == DirectionUp)  // nach oben fahren
				{
					stepper->CurrentSteps -= stepSize;
					stepperIndex -= stepSize;
				}
				else							   // nach unten fahren
				{
					stepper->CurrentSteps += stepSize;
					stepperIndex += stepSize;
				}

				// stepperIndex in den richtigen Bereich bringen (underflow/overflow)
				while (stepperIndex < 0)
					stepperIndex += STEPPER_STEP_COUNT;
				while (stepperIndex >= STEPPER_STEP_COUNT)
					stepperIndex -= STEPPER_STEP_COUNT;

				writeGPIO = true;

				// Werte speichern
				stepper->CurrentStepIndex = (uint8_t)stepperIndex;
				stepper->TickCount = stepper->WaitTime;
				stepper->BrakeTicks = 0;

				// Richtung speichern
				stepper->Direction = moveDirection;
				stepper->TurnWaitTime = config.turnWaitTime;
			}
			else if (config.brakeMode == BrakeSmart)
			{
				if (stepper->BrakeTicks < config.brakeTicks) {
					writeGPIO = true;
					stepper->BrakeTicks++;
				}
			}
			else if (config.brakeMode == BrakeAlways) // Bremse benutzen?
				writeGPIO = true;

			if (writeGPIO)
				gpioValue |= stepsStepper[stepper->CurrentStepIndex] << (4 * j); // jeder Wert in stepsStepper ist 4 Bit lang
			wdt_yield();
		}


		if (!mcp->isOK || mcp->lastGPIOValue != gpioValue) {
			boolean wasOK = mcp->isOK;
			mcp->isOK = (mcp->mcpChip.writeGPIO(gpioValue) == 0);

			if (mcp->isOK)
				mcp->lastGPIOValue = gpioValue;

#if !IGNORE_MCP_FAULTS
			if (wasOK && !mcp->isOK) {
				serialPrintF("MCP Fault with mcp number ");
				serialPrintln(i);

				internalError(ERROR_MCP_FAULT_1 + i);
			}
#endif
		}

		// Stepper zurücksetzen
		if (!mcp->isOK)
			memcpy(mcp->steppers, stepperData, sizeof(mcp->steppers));

		wdt_yield();
	}
	stepperTime = endTime(TIMER_STEPPER);
}
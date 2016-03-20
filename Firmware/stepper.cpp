#include "stepper.h"

MCPData mcps[MCP_COUNT];

void initAllMCPs()
{
	for (byte i = 0; i < MCP_COUNT; i++)
		initMCP(i);
}

void initMCP(byte index)
{
	MCPData* data = &mcps[index];
	data->MCPAddress = index;

	for (byte i = 0; i < STEPPER_COUNT; i++)
	{
		StepperData* stepper = &data->Steppers[i];
		stepper->LastRevision = 0;
		stepper->CurrentSteps = 0;
		stepper->GotoSteps = 0;
		stepper->CurrentStepIndex = 0;
		stepper->TickCount = 0;
		stepper->WaitTime = 0;
	}

	// MCP initialisieren
	data->MCP.begin(index);

	// alle Pins (0xFFFF) auf OUTPUT stellen
	data->MCP.setPinDirOUT(0xFFFF);
}


void setStepper(int32_t revision, byte x, byte y, uint16_t height, byte waitTime)
{
	if (x < 0 || x >= CLUSTER_WIDTH)
	{
		lastError = ERROR_X_INVALID;
		return blinkRedLedShort();
	}
	if (y < 0 || y >= CLUSTER_HEIGHT)
	{
		lastError = ERROR_Y_INVALID;
		return blinkRedLedShort();
	}
	if (height > config->maxSteps)
	{
		lastError = ERROR_INVALID_HEIGHT;
		return blinkRedLedShort();
	}

	byte index = y * CLUSTER_WIDTH + x;

	MCPData* data = &mcps[mcpPosition[index]];
	StepperData* stepper = &data->Steppers[stepperPosition[index]];
	if (checkRevision(stepper->LastRevision, revision))
	{
		stepper->LastRevision = revision;
		stepper->GotoSteps = height;
		stepper->TickCount = 0;
		stepper->WaitTime = waitTime;
	}
}


void setAllSteps(int32_t revision, uint16_t gotoSteps, byte waitTime)
{
	if (gotoSteps > config->maxSteps)
	{
		lastError = ERROR_INVALID_HEIGHT;
		return blinkRedLedShort();
	}

	for (byte i = 0; i < MCP_COUNT; i++) {
		for (byte j = 0; j < STEPPER_COUNT; j++) {
			StepperData* stepper = &mcps[i].Steppers[j];
			if (checkRevision(stepper->LastRevision, revision))
			{
				stepper->LastRevision = revision;
				stepper->GotoSteps = gotoSteps;
				stepper->TickCount = 0;
				stepper->WaitTime = waitTime;
			}
		}
	}
}


void stopMove() {
	for (byte i = 0; i < MCP_COUNT; i++) {
		for (byte j = 0; j < STEPPER_COUNT; j++) {
			StepperData* stepper = &mcps[i].Steppers[j];

			// stoppen
			stepper->GotoSteps = stepper->CurrentSteps;
		}
	}
}


void updateSteppers(boolean alwaysUseHalfStep)
{
	for (byte i = 0; i < MCP_COUNT; i++)
	{
		// Wert für den GPIO
		uint16_t gpioValue = 0;

		for (byte j = 0; j < STEPPER_COUNT; j++)
		{
			StepperData* stepper = &mcps[i].Steppers[j];

			byte stepSize = 0;
			int32_t diff = abs(stepper->CurrentSteps - stepper->GotoSteps);
			if (diff != 0)
			{
				if (config->stepMode == StepBoth) 
					stepSize = min(2, diff); //  schauen ob wir Full oder Half Step gemacht werden soll
				else if (diff >= config->stepMode) // Half oder Full Step machen
					stepSize = config->stepMode;
			}

			if (alwaysUseHalfStep)
				stepSize = min(stepSize, 1);

			if (stepper->WaitTime > 0 && stepper->TickCount >= 0)
				stepper->TickCount--;

			if (stepSize > 0 && (stepper->WaitTime == 0 || stepper->TickCount < 0))
			{
				int8_t stepperIndex = stepper->CurrentStepIndex;
				if (stepperIndex % stepSize > 0) // schauen ob wir noch einen Zwischenschritt machen müssen
					stepSize = 1;

				if (stepper->GotoSteps < stepper->CurrentSteps)  // nach oben fahren
				{
					stepper->CurrentSteps -= stepSize;
					stepperIndex -= stepSize;
				}
				else // nach unten fahren
				{
					stepper->CurrentSteps += stepSize;
					stepperIndex += stepSize;
				}

				// stepperIndex in den richtigen Bereich bringen (underflow/overflow)
				if (stepperIndex < 0)
					stepperIndex = STEPPER_STEP_COUNT - stepSize;
				else if (stepperIndex >= STEPPER_STEP_COUNT)
					stepperIndex = 0;

				gpioValue |= stepsStepper[stepperIndex] << (4 * j); // jeder Wert in stepsStepper ist 4 Bit lang

				stepper->CurrentStepIndex = (byte)stepperIndex;

				stepper->TickCount = stepper->WaitTime;
			}
			else if (config->brakeMode == BrakeAlways) // Bremse benutzen?
				gpioValue |= stepsStepper[stepper->CurrentStepIndex] << (4 * j);
		}
		mcps[i].MCP.writeGPIOS(gpioValue);
	}
}
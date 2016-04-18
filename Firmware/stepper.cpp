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

	for (byte i = 0; i < MCP_STEPPER_COUNT; i++)
	{
		StepperData* stepper = &data->Steppers[i];
		memset(stepper, 0, sizeof(StepperData));
	}

	data->MCP = new MCP23017();

	// MCP initialisieren
	data->MCP->begin(index);

	// alle Pins (0xFFFF) auf OUTPUT stellen
	data->MCP->setPinDirOUT(0xFFFF);
}

StepperData* getStepper(byte x, byte y)
{
	return getStepper(y * CLUSTER_WIDTH + x);
}

StepperData* getStepper(int index)
{
	MCPData* data = mcps + mcpPosition[index];
	return data->Steppers + stepperPosition[index];
}

void resetStepper(StepperData* stepper)
{
	stepper->CurrentSteps = 0;
	stepper->GotoSteps = 0;
	stepper->TickCount = 0;
	stepper->WaitTime = 0;
}

void forceStepper(StepperData* stepper, int32_t revision, int16_t height)
{
	resetStepper(stepper);
	stepper->LastRevision = revision;
	stepper->GotoSteps = height;
}

void setStepper(StepperData* stepper, int32_t revision, uint16_t height, byte waitTime)
{
	if (checkRevision(stepper->LastRevision, revision))
	{
		stepper->LastRevision = revision;
		stepper->GotoSteps = height;
		stepper->TickCount = 0;
		stepper->WaitTime = waitTime;
	}
}


void setStepper(int32_t revision, byte x, byte y, uint16_t height, byte waitTime)
{
	if (x < 0 || x >= CLUSTER_WIDTH)
		return protocolError(ERROR_X_INVALID);
	if (y < 0 || y >= CLUSTER_HEIGHT)
		return protocolError(ERROR_Y_INVALID);
	if (height > config->maxSteps)
		return protocolError(ERROR_INVALID_HEIGHT);

	setStepper(getStepper(x, y), revision, height, waitTime);
}

void setAllSteps(int32_t revision, uint16_t height, byte waitTime)
{
	if (height > config->maxSteps)
		return protocolError(ERROR_INVALID_HEIGHT);
	for (int i = 0; i < CLUSTER_SIZE; i++) 
		setStepper(getStepper(i), revision, height, waitTime);
}


void stopMove() {
	for (int i = 0; i < CLUSTER_SIZE; i++) {
		StepperData* stepper = getStepper(i);

		// stoppen
		stepper->GotoSteps = stepper->CurrentSteps;
		stepper->TickCount = 0;
	}
}


void updateSteppers(boolean alwaysUseHalfStep)
{
	for (byte i = 0; i < MCP_COUNT; i++)
	{
		uint16_t gpioValue = 0;

		for (byte j = 0; j < MCP_STEPPER_COUNT; j++)
		{
			StepperData* stepper = &(mcps[i].Steppers[j]);

			byte stepSize = 0;
			int32_t diff = abs(stepper->CurrentSteps - stepper->GotoSteps);
			if (diff != 0)
			{
				if (config->stepMode == StepBoth) 
					stepSize = min(2, diff);		// schauen ob Full oder Half Step gemacht werden soll
				else if (diff >= config->stepMode)	
					stepSize = config->stepMode;	// Half oder Full Step machen
			}

			if (alwaysUseHalfStep)
				stepSize = min(stepSize, 1);

			bool waitStep = false;
			if (stepper->WaitTime > 0) {
				stepper->TickCount--;
				waitStep = stepper->TickCount >= 0;
			}

			if (stepSize > 0 && !waitStep)
			{
				int8_t stepperIndex = stepper->CurrentStepIndex;
				if (stepperIndex % stepSize > 0) // schauen ob wir noch einen Zwischenschritt machen müssen
					stepSize = 1;

				if (stepper->GotoSteps < stepper->CurrentSteps)  // nach oben fahren
				{
					stepper->CurrentSteps -= stepSize;
					stepperIndex -= stepSize;
				}
				else											// nach unten fahren
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
				stepper->BrakeTicks = 0;
			}
			else if (config->brakeMode == BrakeSmart)
			{
				if (stepper->BrakeTicks++ < config->brakeTicks)
					gpioValue |= stepsStepper[stepper->CurrentStepIndex] << (4 * j);
			}
			else if (config->brakeMode == BrakeAlways) // Bremse benutzen?
				gpioValue |= stepsStepper[stepper->CurrentStepIndex] << (4 * j);
		}
		mcps[i].MCP->writeGPIOS(gpioValue);
	}
}
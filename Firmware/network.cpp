#include "network.h"

const static byte ethernetMac[] = { 0x74, 0x69, 0x69, 0x2D, 0x30, LAN_ID }; // Mac-Adresse des Boards
byte Ethernet::buffer[ETHERNET_BUFFER_SIZE];	// Buffer für die Ethernet Pakete

int32_t configRevision = 0;		// die letzte Revision des Config-Packets
int32_t setDataRevision = 0;	// die letzte Revision des SetData-Packets

byte currentBusyCommand = BUSY_NONE;
boolean stopBusyCommand = false;
byte lastError = ERROR_NONE;


// gibt true zurück wenn revision neuer ist als lastRevision
boolean checkRevision(int32_t lastRevision, int32_t revision)
{
	if (lastRevision >= 0 && revision < 0) // Overflow handeln
		return true;
	return revision > lastRevision;
}

// liest einen unsigned short aus einem char-Array angefangen ab offset Bytes
uint16_t readUInt16(const char* data, int32_t offset)
{
	uint16_t val = 0;
	memcpy(&val, data + offset, sizeof(uint16_t));
	return val;
}

// liest einen int aus einem char-Array angefangen ab offset Bytes
int32_t readInt32(const char* data, int32_t offset)
{
	int32_t val = 0;
	memcpy(&val, data + offset, sizeof(int32_t));
	return val;
}

// schreibt einen unsigned short in einen char-Array angefangen ab offset Bytes
void writeUInt16(char* data, int32_t offset, const uint16_t val)
{
	memcpy(data + offset, &val, sizeof(uint16_t));
}


// schreibt einen int in einen char-Array angefangen ab offset Bytes
void writeInt32(char* data, int32_t offset, const int32_t val)
{
	memcpy(data + offset, &val, sizeof(int32_t));
}

void initNetwork()
{
	uint8_t rev = ether.begin(sizeof Ethernet::buffer, ethernetMac, 28);
	if (rev == 0)
	{
		error(1, "ethernet begin failed");
		return; // wird niemals passieren da error() in eine Endloschleife geht
	}

	if (!ether.dhcpSetup())
	{
		error(2, "dhcp failed");
		return;
	}

	ether.udpServerListenOnPort(&onPacketReceive, PROTOCOL_PORT);
}

void sendAckPacket(int32_t revision)
{
	char packet[] = { 'K', 'K', 'S', 0, PacketAck, 0xDE, 0xAD, 0xBE, 0xEF };
	writeInt32(packet, 5, revision);
	ether.makeUdpReply(packet, sizeof(packet), PROTOCOL_PORT);
}

void sendData(int32_t revision)
{
	char data[HEADER_SIZE + CLUSTER_WIDTH * CLUSTER_HEIGHT * 3];
	data[0] = 'K';
	data[1] = 'K';
	data[2] = 'S';
	data[3] = 0;
	data[4] = PacketGetData;
	writeInt32(data, 5, revision);

	int32_t offsetData = HEADER_SIZE;
	for (byte x = 0; x < CLUSTER_WIDTH; x++) {
		for (byte y = 0; y < CLUSTER_HEIGHT; y++)
		{
			byte index = y * CLUSTER_WIDTH + x;
			MCPData* mcp = &mcps[mcpPosition[index]];
			StepperData* stepper = &mcp->Steppers[stepperPosition[index]];
			writeUInt16(data, offsetData, (uint16_t)stepper->CurrentSteps);

			offsetData += 2;

			data[offsetData++] = stepper->WaitTime;
		}
	}

	if (offsetData > sizeof(data))
	{
		lastError = ERROR_BUFFER_OVERFLOW;
		blinkRedLedShort();
		return;
	}
	ether.makeUdpReply(data, sizeof(data), PROTOCOL_PORT);
}

void onPacketReceive(uint16_t dest_port, uint8_t src_ip[4], uint16_t src_port, const char* data, uint16_t len)
{
	// alle Kugelmatik V3 Pakete
	// sind mindestens HEADER_SIZE Bytes lang
	// und fangen mit "KKS" an
	if (len < HEADER_SIZE || data[0] != 'K' || data[1] != 'K' || data[2] != 'S')
		return;

	boolean isGuaranteed = data[3] > 0;

	// erstes Byte nach dem Magicstring gibt den Paket-Typ an
	char packetType = data[4];

	// fortlaufende ID für die Pakete werden nach dem Magicstring und dem Paket-Typ geschickt
	int32_t  revision = readInt32(data, 5);

	// wenn ein busy-Befehl läuft dann werden nur Ping, Info und Stop verarbeitet
	if (currentBusyCommand != BUSY_NONE && packetType != PacketPing && packetType != PacketInfo && packetType != PacketStop)
		return;

	// isGuaranteed gibt an ob der Sender eine Antwort erwartet
	if (isGuaranteed)
		sendAckPacket(revision);

	int32_t offset = HEADER_SIZE;
	switch (packetType)
	{
	case PacketPing:
		ether.makeUdpReply((char*)data, len, PROTOCOL_PORT); // das Ping-Packet funktioniert gleichzeitig auch als Echo-Funktion
		break;
	case PacketStepper:
		if (len < HEADER_SIZE + 4)
		{
			lastError = ERROR_PACKET_TOO_SHORT;
			return blinkGreenLedShort();
		}
		else
		{
			byte x = (data[offset] >> 4) & 0xF;
			byte y = data[offset] & 0xF;
			offset += 1;

			uint16_t height = readUInt16(data, offset);
			offset += 2;

			byte waitTime = data[offset++];
			setStepper(revision, x, y, height, waitTime);
		}
		break;
	case PacketSteppers:
		if (len < HEADER_SIZE + 5)
		{
			lastError = ERROR_PACKET_TOO_SHORT;
			return blinkGreenLedShort();
		}
		else
		{
			byte length = data[offset];
			offset += 1;

			uint16_t  height = readUInt16(data, offset);
			offset += 2;

			byte waitTime = data[offset++];

			if (len < offset + length)
			{
				lastError = ERROR_PACKET_TOO_SHORT;
				return blinkGreenLedShort();
			}

			for (byte i = 0; i < length; i++)
			{
				byte x = data[offset] >> 4;
				byte y = data[offset] & 0xF;
				setStepper(revision, x, y, height, waitTime);

				offset += 1;
			}
		}
		break;
	case PacketSteppersArray:
		if (len < HEADER_SIZE + 1)
		{
			lastError = ERROR_PACKET_TOO_SHORT;
			return blinkGreenLedShort();
		}
		else
		{
			byte length = data[offset];
			offset += 1;

			if (len < offset + length * 4)
			{
				lastError = ERROR_PACKET_TOO_SHORT;
				return blinkGreenLedShort();
			}

			for (byte i = 0; i < length; i++)
			{
				byte x = data[offset] >> 4;
				byte y = data[offset] & 0xF;
				offset += 1;

				uint16_t  height = readUInt16(data, offset);
				offset += 2;

				byte waitTime = data[offset++];

				setStepper(revision, x, y, height, waitTime);
			}
		}
		break;
	case PacketSteppersRectangle:
		if (len < HEADER_SIZE + 2 + 3)
		{
			lastError = ERROR_PACKET_TOO_SHORT;
			return blinkGreenLedShort();
		}
		else
		{
			byte minX = data[offset] >> 4;
			byte minY = data[offset] & 0xF;
			offset += 1;

			byte maxX = data[offset] >> 4;
			byte maxY = data[offset] & 0xF;
			offset += 1;

			uint16_t height = readUInt16(data, offset);
			offset += 2;

			byte waitTime = data[offset++];

			if (minX < 0 || minY < 0 || maxX >= CLUSTER_WIDTH || maxY >= CLUSTER_HEIGHT)
			{
				lastError = ERROR_INVALID_VALUE;
				return blinkGreenLedShort();
			}
			if (minX > maxX || minY > maxY)
			{
				lastError = ERROR_INVALID_VALUE;
				return blinkGreenLedShort();
			}

			for (byte x = minX; x <= maxX; x++) {
				for (byte y = minY; y <= maxY; y++) {
					setStepper(revision, x, y, height, waitTime);
				}
			}
		}
		break;
	case PacketSteppersRectangleArray:
		if (len < HEADER_SIZE + 2)
		{
			lastError = ERROR_PACKET_TOO_SHORT;
			return blinkGreenLedShort();
		}
		else
		{
			byte minX = data[offset] >> 4;
			byte minY = data[offset] & 0xF;
			offset += 1;

			byte maxX = data[offset] >> 4;
			byte maxY = data[offset] & 0xF;
			offset += 1;

			if (minX < 0 || minX >= CLUSTER_WIDTH || minY < 0 || minY >= CLUSTER_HEIGHT)
			{
				lastError = ERROR_INVALID_VALUE;
				return blinkGreenLedShort();
			}
			if (minX > maxX || minY > maxY)
			{
				lastError = ERROR_INVALID_VALUE;
				return blinkGreenLedShort();
			}

			byte area = (maxX - minX + 1) * (maxY - minY + 1); // +1, da max die letzte Kugel nicht beinhaltet
			if (len < offset + 3 * area)
			{
				lastError = ERROR_PACKET_TOO_SHORT;
				return blinkGreenLedShort();
			}

			// beide for-Schleifen müssen mit dem Client übereinstimmen sonst stimmen die Positionen nicht		
			for (byte x = minX; x <= maxX; x++) {
				for (byte y = minY; y <= maxY; y++)
				{
					uint16_t height = readUInt16(data, offset);
					offset += 2;

					byte waitTime = data[offset++];

					setStepper(revision, x, y, height, waitTime);
				}
			}
		}
		break;
	case PacketAllSteppers:
		if (len < HEADER_SIZE + 3)
		{
			lastError = ERROR_PACKET_TOO_SHORT;
			return blinkGreenLedShort();
		}
		else
		{
			uint16_t height = readUInt16(data, offset);
			offset += 2;
			byte waitTime = data[offset++];
			setAllSteps(revision, height, waitTime);
		}
		break;
	case PacketAllSteppersArray:
		if (len < HEADER_SIZE + 3 * CLUSTER_WIDTH * CLUSTER_HEIGHT)
		{
			lastError = ERROR_PACKET_TOO_SHORT;
			return blinkGreenLedShort();
		}
		else
		{
			// beide for-Schleifen müssen mit dem Client übereinstimmen sonst stimmen die Positionen nicht		
			for (byte x = 0; x < CLUSTER_WIDTH; x++) {
				for (byte y = 0; y < CLUSTER_HEIGHT; y++)
				{
					uint16_t height = readUInt16(data, offset);
					offset += 2;

					byte waitTime = data[offset++];

					setStepper(revision, x, y, height, waitTime);
				}
			}
		}
		break;
	case PacketHome:
		if (len < HEADER_SIZE + 4)
		{
			lastError = ERROR_PACKET_TOO_SHORT;
			return blinkGreenLedShort();
		}
		else
		{
			// 0xABCD wird benutzt damit man nicht ausversehen das Home-Paket schickt (wenn man z.B. den Paket-Type verwechselt)
			int32_t magic = readInt32(data, offset);
			if (magic != 0xABCD)
			{
				lastError = ERROR_INVALID_MAGIC;
				return blinkBothLedsShort();
			}

			// gotoSteps auf -MAX_STEPS setzen damit alle Kugeln voll nach oben fahren (negative Steps sind eigentlich verboten)
			for (byte i = 0; i < MCP_COUNT; i++) {
				for (byte j = 0; j < STEPPER_COUNT; j++)
				{
					StepperData* stepper = &mcps[i].Steppers[j];
					if (checkRevision(stepper->LastRevision, revision))
					{
						stepper->LastRevision = revision;
						stepper->CurrentSteps = 0;
						stepper->GotoSteps = -config->maxSteps;
						stepper->CurrentStepIndex = 0;
						stepper->WaitTime = 0;
						stepper->TickCount = 0;
					}
				}
			}

			currentBusyCommand = BUSY_HOME;

			// alle Stepper nach oben Fahren lassen
			turnRedLedOn();
			for (int i = 0; i <= config->maxSteps; i++)
			{
				if (stopBusyCommand)
					break;

				if (i % 100 == 0)
					toogleGreenLed();
				updateSteppers(true);
				usdelay(config->homeTime); // langsam bewegen

				wdt_reset();

				// Pakete empfangen
#if RECEIVE_PACKETS_BUSY
				ether.packetLoop(ether.packetReceive());
#endif
			}
			currentBusyCommand = BUSY_NONE;
			stopBusyCommand = false;
			turnGreenLedOff();
			turnRedLedOff();

			// alle Stepper zurücksetzen
			for (byte i = 0; i < MCP_COUNT; i++) {
				for (byte j = 0; j < STEPPER_COUNT; j++)
				{
					StepperData* stepper = &mcps[i].Steppers[j];
					stepper->GotoSteps = 0;
					stepper->CurrentSteps = 0;
					stepper->CurrentStepIndex = 0;
					stepper->WaitTime = 0;
					stepper->TickCount = 0;
				}
			}
		}
		break;
	case PacketResetRevision:
		for (byte i = 0; i < MCP_COUNT; i++) {
			for (byte j = 0; j < STEPPER_COUNT; j++) {
				StepperData* stepper = &mcps[i].Steppers[j];
				stepper->LastRevision = 0;
			}
		}
		break;
	case PacketFix:
		if (len < HEADER_SIZE + 5)
		{
			lastError = ERROR_PACKET_TOO_SHORT;
			return blinkGreenLedShort();
		}
		else
		{
			// 0xDCBA wird benutzt damit man nicht ausversehen das Fix-Paket schickt (wenn man z.B. den Paket-Type verwechselt)
			int32_t magic = readInt32(data, offset);
			if (magic != 0xDCBA)
			{
				lastError = ERROR_INVALID_MAGIC;
				return blinkBothLedsShort();
			}
			offset += 4;

			byte x = (data[offset] >> 4) & 0xF;
			byte y = data[offset] & 0xF;
			offset += 1;

			if (x < 0 || x >= CLUSTER_WIDTH)
			{
				lastError = ERROR_X_INVALID;
				return blinkGreenLedShort();
			}
			if (y < 0 || y >= CLUSTER_HEIGHT)
			{
				lastError = ERROR_Y_INVALID;
				return blinkGreenLedShort();
			}

			byte index = y * CLUSTER_WIDTH + x;
			MCPData* data = &mcps[mcpPosition[index]];
			StepperData* stepper = &data->Steppers[stepperPosition[index]];
			if (checkRevision(stepper->LastRevision, revision))
			{
				stepper->LastRevision = revision;
				stepper->CurrentSteps = 0;
				stepper->GotoSteps = config->fixSteps;
				stepper->WaitTime = 0;
				stepper->TickCount = 0;

				turnRedLedOn();
				currentBusyCommand = BUSY_FIX;
				for (int32_t i = 0; i < config->fixSteps; i++)
				{
					if (stopBusyCommand)
						break;
					if (i % 100 == 0)
						toogleGreenLed();
					updateSteppers(true);
					usdelay(config->fixTime); // langsam bewegen

					wdt_reset();

					// Pakete empfangen
#if RECEIVE_PACKETS_BUSY
					ether.packetLoop(ether.packetReceive());
#endif
				}

				currentBusyCommand = BUSY_NONE;
				stopBusyCommand = false;
				turnGreenLedOff();
				turnRedLedOff();

				// Stepper zurücksetzen
				stepper->CurrentSteps = 0;
				stepper->GotoSteps = 0;
				stepper->CurrentStepIndex = 0;
			}
		}
		break;
	case PacketHomeStepper:
		if (len < HEADER_SIZE + 5)
		{
			lastError = ERROR_PACKET_TOO_SHORT;
			return blinkGreenLedShort();
		}
		else
		{
			// 0xDCBA wird benutzt damit man nicht ausversehen das HomeStepper-Paket schickt (wenn man z.B. den Paket-Type verwechselt)
			int32_t magic = readInt32(data, offset);
			if (magic != 0xABCD)
			{
				lastError = ERROR_INVALID_MAGIC;
				return blinkBothLedsShort();
			}
			offset += 4;

			byte x = (data[offset] >> 4) & 0xF;
			byte y = data[offset] & 0xF;
			offset += 1;

			if (x < 0 || x >= CLUSTER_WIDTH)
			{
				lastError = ERROR_X_INVALID;
				return blinkGreenLedShort();
			}
			if (y < 0 || y >= CLUSTER_HEIGHT)
			{
				lastError = ERROR_Y_INVALID;
				return blinkGreenLedShort();
			}

			int index = y * CLUSTER_WIDTH + x;
			MCPData* data = &mcps[mcpPosition[index]];
			StepperData* stepper = &data->Steppers[stepperPosition[index]];
			if (checkRevision(stepper->LastRevision, revision))
			{
				// Stepper so setzen das er komplett hoch fährt
				stepper->LastRevision = revision;
				stepper->CurrentSteps = config->maxSteps;
				stepper->GotoSteps = 0;
				stepper->WaitTime = 0; // WaitTime zurück setzen
				stepper->TickCount = 0;

				currentBusyCommand = BUSY_HOME_STEPPER;
				turnRedLedOn();
				for (int32_t i = 0; i <= config->maxSteps; i++)
				{
					if (stopBusyCommand)
						break;
					if (i % 100 == 0)
						toogleGreenLed();
					updateSteppers(true);
					usdelay(config->homeTime); // langsam bewegen

					wdt_reset();

					// Pakete empfangen
#if RECEIVE_PACKETS_BUSY
					ether.packetLoop(ether.packetReceive());
#endif
				}
				currentBusyCommand = BUSY_NONE;
				stopBusyCommand = false;
				turnGreenLedOff();
				turnRedLedOff();

				// Stepper zurücksetzen
				stepper->CurrentSteps = 0;
				stepper->GotoSteps = 0;
				stepper->CurrentStepIndex = 0;
			}
		}
		break;
	case PacketGetData:
	{
		sendData(revision);
	}
	break;
	case PacketInfo:
	{
		char data[HEADER_SIZE + 17];
		data[0] = 'K';
		data[1] = 'K';
		data[2] = 'S';
		data[3] = 0;
		data[4] = PacketInfo; // Paket-Type Info
		writeInt32(data, 5, revision);

		int32_t offsetData = HEADER_SIZE;
		data[offsetData++] = BUILD_VERSION;

		data[offsetData++] = currentBusyCommand;

		int32_t highestRevision = INT_MIN;
		for (byte i = 0; i < MCP_COUNT; i++)
			for (byte j = 0; j < STEPPER_COUNT; j++) {
				StepperData* stepper = &mcps[i].Steppers[j];

				if (stepper->LastRevision > highestRevision)
					highestRevision = stepper->LastRevision;
			}

		if (configRevision > highestRevision)
			highestRevision = configRevision;

		if (setDataRevision > highestRevision)
			highestRevision = setDataRevision;

		writeInt32(data, offsetData, highestRevision);
		offsetData += 4;

		data[offsetData++] = (uint8_t)config->stepMode;

		writeInt32(data, offsetData, config->tickTime);
		offsetData += 4;

		data[offsetData++] = config->brakeMode;

		data[offsetData++] = lastError;

		writeInt32(data, offsetData, freeRam());
		offset += 4;


		if (offsetData > sizeof(data))
		{
			lastError = ERROR_BUFFER_OVERFLOW;
			blinkRedLedShort();
			return;
		}
		ether.makeUdpReply(data, sizeof(data), PROTOCOL_PORT);
	}
	break;
	case PacketConfig:
		if (len < HEADER_SIZE + 5)
		{
			lastError = ERROR_PACKET_TOO_SHORT;
			return blinkGreenLedShort();
		}
		else
		{
			if (!checkRevision(configRevision, revision))
				break;

			configRevision = revision;

			byte cStepMode = data[offset++];
			if (cStepMode < 1 || cStepMode > 3)
			{
				lastError = ERROR_INVALID_CONFIG_VALUE;
				return blinkBothLedsShort();
			}

			int32_t cTickTime = readInt32(data, offset);
			offset += 4;
			if (cTickTime < 50 || cTickTime > 15000)
			{
				lastError = ERROR_INVALID_CONFIG_VALUE;
				return blinkBothLedsShort();
			}

			boolean cUseBreak = data[offset++] > 0;

			config->stepMode = (StepMode)cStepMode;
			config->tickTime = cTickTime;
			config->brakeMode = cUseBreak ? BrakeNone : BrakeAlways;
		}
		break;
	case PacketBlinkGreen:
		blinkGreenLedShort();
		break;
	case PacketBlinkRed:
		blinkRedLedShort();
		break;
	case PacketStop:
#if ALLOW_STOP_BUSY
		if (currentBusyCommand != BUSY_NONE)
			stopBusyCommand = true;
#endif
#if ALLOW_STOP_MOVE
		stopMove();

		// Client informieren, dass es möglicherweise neue Daten gibt
		sendData(revision);
#endif
		break;
	case PacketSetData:
		if (len < HEADER_SIZE + CLUSTER_WIDTH * CLUSTER_HEIGHT * 2)
		{
			lastError = ERROR_PACKET_TOO_SHORT;
			return blinkGreenLedShort();
		}
		else
		{
			if (!checkRevision(setDataRevision, revision))
				break;

			setDataRevision = revision;

			for (byte x = 0; x < CLUSTER_WIDTH; x++) {
				for (byte y = 0; y < CLUSTER_HEIGHT; y++)
				{
					byte index = y * CLUSTER_WIDTH + x;
					MCPData* mcp = &mcps[mcpPosition[index]];
					StepperData* stepper = &mcp->Steppers[stepperPosition[index]];

					uint16_t height = readUInt16(data, offset);
					offset += 2;

					if (height > config->maxSteps) {
						lastError = ERROR_INVALID_HEIGHT;
						return blinkBothLedsShort();
					}

					stepper->CurrentSteps = height;
					stopMove();
				}
			}
		}
		break;
	default:
		lastError = ERROR_UNKOWN_PACKET;
		return blinkRedLedShort();
	}
#if BLINK_PACKET
	turnGreenLedOn();
#endif
}
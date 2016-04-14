#include "network.h"

const static uint8_t ethernetMac[] = { 0x74, 0x69, 0x69, 0x2D, 0x30, LAN_ID }; // Mac-Adresse des Boards
uint8_t Ethernet::buffer[ETHERNET_BUFFER_SIZE];	

int32_t configRevision = 0;		// die letzte Revision des Config-Packets
int32_t setDataRevision = 0;	// die letzte Revision des SetData-Packets

byte currentBusyCommand = BUSY_NONE;
boolean stopBusyCommand = false;
byte lastError = ERROR_NONE;

PacketBuffer* packet;


// gibt true zurück wenn revision neuer ist als lastRevision
boolean checkRevision(int32_t lastRevision, int32_t revision)
{
	if (lastRevision >= 0 && revision < 0) // Overflow handeln
		return true;
	return revision > lastRevision;
}

void initNetwork()
{
	uint8_t rev = ether.begin(ETHERNET_BUFFER_SIZE, ethernetMac, 28);
	if (rev == 0)
	{
		error("init", "ethernet begin failed", true);
		return; // wird niemals passieren da error() in eine Endloschleife geht
	}

	// warten bis Ethernet Kabel verbunden ist
	while (!ether.isLinkUp())
	{
		toogleGreenLed();
		delay(300);
	}
	turnGreenLedOn();

	if (!ether.dhcpSetup()) 
	{
		error("init", "dhcp failed", false);
		return;
	}

	packet = new PacketBuffer(NULL, 0);
	
	ether.udpServerListenOnPort(&onPacketReceive, PROTOCOL_PORT);
}

void sendPacket()
{
	ether.makeUdpReply((char*)packet->getBuffer(), packet->getPosition(), PROTOCOL_PORT);
}

void writeHeader(bool guarenteed, byte packetType, int32_t revision)
{
	packet->resetPosition();
	packet->setSize(packet->getBufferSize()); // Size überschreiben, da die Size vom Lesen gesetzt wird
	packet->write('K');
	packet->write('K');
	packet->write('S');
	packet->write(guarenteed);
	packet->write(packetType);
	packet->write(revision);
}


bool readPosition(PacketBuffer* packet, byte* x, byte* y)
{
	byte pos = packet->readUint8();
	byte xvalue = (pos >> 4) & 0xF;
	byte yvalue = pos & 0xF;

	if (xvalue < 0 || xvalue >= CLUSTER_WIDTH)
	{
		lastError = ERROR_X_INVALID;
		blinkGreenLedShort();
		return false;
	}
	if (yvalue < 0 || yvalue >= CLUSTER_HEIGHT)
	{
		lastError = ERROR_Y_INVALID;
		blinkGreenLedShort();
		return false;
	}

	*x = xvalue;
	*y = yvalue;
	return true;
}

void sendAckPacket(int32_t revision)
{
	writeHeader(false, PacketAck, revision);
	sendPacket();
}

void sendData(int32_t revision)
{
	writeHeader(false, PacketGetData, revision);

	for (byte x = 0; x < CLUSTER_WIDTH; x++) {
		for (byte y = 0; y < CLUSTER_HEIGHT; y++)
		{
			StepperData* stepper = getStepper(x, y);

			packet->write(max(0, stepper->GotoSteps)); // CurrentSteps));
			packet->write(stepper->WaitTime);
		}
	}

	sendPacket();
}

void onPacketReceive(uint16_t dest_port, uint8_t src_ip[4], uint16_t src_port, const char* data, uint16_t len)
{
	// alle Kugelmatik V3 Pakete
	// sind mindestens HEADER_SIZE Bytes lang
	// und fangen mit "KKS" an
	if (len < HEADER_SIZE || data[0] != 'K' || data[1] != 'K' || data[2] != 'S')
		return;

	// data ist unser Etherner Buffer (verschoben um die UDP Header Länge)
	// wir nutzen den selben Buffer zum Lesen und zum Schreiben
	packet->setBuffer((uint8_t*)data, ETHERNET_BUFFER_SIZE - 28); // 28 Bytes für IP + UDP Header abziehen
	packet->setSize(len);
	packet->seek(3); 

	boolean isGuaranteed = packet->readBoolean();

	// erstes Byte nach dem Magicstring gibt den Paket-Typ an
	uint8_t packetType = packet->readUint8();

	// fortlaufende ID für die Pakete werden nach dem Magicstring und dem Paket-Typ geschickt
	int32_t revision = packet->readInt32();

	// wenn ein busy-Befehl läuft dann werden nur Ping, Info und Stop verarbeitet
	if (currentBusyCommand != BUSY_NONE && packetType != PacketPing && packetType != PacketInfo && packetType != PacketStop)
		return;

	// isGuaranteed gibt an ob der Sender eine Antwort erwartet
	if (isGuaranteed)
		sendAckPacket(revision);

	switch (packetType)
	{
	case PacketPing:
		ether.makeUdpReply((char*)data, len, PROTOCOL_PORT); // das Ping-Packet funktioniert gleichzeitig auch als Echo-Funktion
		break;
	case PacketStepper:
	{
		byte x, y;
		if (!readPosition(packet, &x, &y))
			return;
		uint16_t height = packet->readUint16();
		byte waitTime = packet->readUint8();

		setStepper(revision, x, y, height, waitTime);
		break;
	}
	case PacketSteppers:
	{
		byte length = packet->readUint8();
		uint16_t height = packet->readUint16();
		byte waitTime = packet->readUint8();

		for (byte i = 0; i < length; i++)
		{
			byte x, y;
			readPosition(packet, &x, &y);
			setStepper(revision, x, y, height, waitTime);
		}

		break;
	}
	case PacketSteppersArray:
	{
		byte length = packet->readUint8();

		for (byte i = 0; i < length; i++)
		{
			byte x, y;
			if (!readPosition(packet, &x, &y))
				return;

			uint16_t height = packet->readUint16();
			byte waitTime = packet->readUint8();

			setStepper(revision, x, y, height, waitTime);
		}
		break;
	}
	case PacketSteppersRectangle:
	{
		byte minX, minY;
		if (!readPosition(packet, &minX, &minY))
			return;

		byte maxX, maxY;
		if (!readPosition(packet, &maxX, &maxY))
			return;

		uint16_t height = packet->readUint16();
		byte waitTime = packet->readUint8();

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
		break;
	}
	case PacketSteppersRectangleArray:
	{
		byte minX, minY;
		if (!readPosition(packet, &minX, &minY))
			return;

		byte maxX, maxY;
		if (!readPosition(packet, &maxX, &maxY))
			return;

		if (minX > maxX || minY > maxY)
		{
			lastError = ERROR_INVALID_VALUE;
			return blinkGreenLedShort();
		}

		byte area = (maxX - minX + 1) * (maxY - minY + 1); // +1, da max die letzte Kugel nicht beinhaltet

		// beide for-Schleifen müssen mit dem Client übereinstimmen sonst stimmen die Positionen nicht		
		for (byte x = minX; x <= maxX; x++) {
			for (byte y = minY; y <= maxY; y++)
			{
				uint16_t height = packet->readUint16();
				byte waitTime = packet->readUint8();

				setStepper(revision, x, y, height, waitTime);
			}
		}
		break;
	}
	case PacketAllSteppers:
	{
		uint16_t height = packet->readUint16();
		byte waitTime = packet->readUint8();
		setAllSteps(revision, height, waitTime);
		break;
	}
	case PacketAllSteppersArray:
	{
		// beide for-Schleifen müssen mit dem Client übereinstimmen sonst stimmen die Positionen nicht		
		for (byte x = 0; x < CLUSTER_WIDTH; x++) {
			for (byte y = 0; y < CLUSTER_HEIGHT; y++)
			{
				uint16_t height = packet->readUint16();
				byte waitTime = packet->readUint8();

				setStepper(revision, x, y, height, waitTime);
			}
		}
		break;
	}
	case PacketHome:
	{
		// 0xABCD wird benutzt damit man nicht ausversehen das Home-Paket schickt (wenn man z.B. den Paket-Type verwechselt)
		int32_t magic = packet->readInt32();
		if (magic != 0xABCD)
		{
			lastError = ERROR_INVALID_MAGIC;
			return blinkBothLedsShort();
		}

		stopMove();

		bool stepperSet = false;

		// Stepper für Home Befehl vorbereiten
		for (int i = 0; i < CLUSTER_SIZE; i++) {
			StepperData* stepper = getStepper(i);

			if (checkRevision(stepper->LastRevision, revision)) {
				forceStepper(stepper, revision, -config->homeSteps);
				stepperSet = true;
			}
		}

		if (stepperSet) {
			runBusy(BUSY_HOME, config->homeSteps, config->homeTime);


			// alle Stepper zurücksetzen
			for (int i = 0; i < CLUSTER_SIZE; i++) {
				StepperData* stepper = getStepper(i);
				if (stepper->LastRevision == revision)
					resetStepper(stepper);
			}
		}
			
		break;
	}
	case PacketResetRevision:
	{
		configRevision = 0;
		setDataRevision = 0;

		for (int i = 0; i < CLUSTER_SIZE; i++)
			getStepper(i)->LastRevision = 0;
		break;
	}
	case PacketFix:
	{
		// 0xDCBA wird benutzt damit man nicht ausversehen das Fix-Paket schickt (wenn man z.B. den Paket-Type verwechselt)
		int32_t magic = packet->readInt32();
		if (magic != 0xDCBA)
		{
			lastError = ERROR_INVALID_MAGIC;
			return blinkBothLedsShort();
		}

		byte x, y;
		if (!readPosition(packet, &x, &y))
			return;

		StepperData* stepper = getStepper(x, y);
		if (checkRevision(stepper->LastRevision, revision))
		{
			stopMove();

			forceStepper(stepper, revision, config->fixSteps);
			runBusy(BUSY_FIX, config->fixSteps, config->fixTime);
			resetStepper(stepper);
		}
		break;
	}
	case PacketHomeStepper:
	{
		// 0xDCBA wird benutzt damit man nicht ausversehen das HomeStepper-Paket schickt (wenn man z.B. den Paket-Type verwechselt)
		int32_t magic = packet->readInt32();
		if (magic != 0xABCD)
		{
			lastError = ERROR_INVALID_MAGIC;
			return blinkBothLedsShort();
		}

		byte x, y;
		if (!readPosition(packet, &x, &y))
			return;

		StepperData* stepper = getStepper(x, y);
		if (checkRevision(stepper->LastRevision, revision))
		{
			stopMove();

			forceStepper(stepper, revision, -config->homeSteps);
			runBusy(BUSY_HOME_STEPPER, config->homeSteps, config->homeTime);
			resetStepper(stepper);
		}
		break;
	}
	case PacketGetData:
	{
		sendData(revision);
		break;
	}
	case PacketInfo:
	{
		int32_t highestRevision = INT_MIN;
		for (int i = 0; i < CLUSTER_SIZE; i++) {
			StepperData* stepper = getStepper(i);

			if (stepper->LastRevision > highestRevision)
				highestRevision = stepper->LastRevision;
		}

		if (configRevision > highestRevision)
			highestRevision = configRevision;

		if (setDataRevision > highestRevision)
			highestRevision = setDataRevision;

		writeHeader(false, PacketInfo, revision);

		packet->write((uint8_t)BUILD_VERSION);
		packet->write(currentBusyCommand);
		packet->write(highestRevision);
		packet->write((uint8_t)config->stepMode);
		packet->write(config->tickTime);
		packet->write((uint8_t)config->brakeMode);
		packet->write(lastError);
		packet->write((int16_t)freeRam());

		sendPacket();
		break;
	}
	case PacketConfig:
	{
		if (!checkRevision(configRevision, revision))
			break;

		configRevision = revision;

		byte cStepMode = packet->readUint8();
		if (cStepMode < StepHalf || cStepMode > StepBoth)
		{
			lastError = ERROR_INVALID_CONFIG_VALUE;
			return blinkBothLedsShort();
		}

		int32_t cTickTime = packet->readInt32();
		if (cTickTime < 50 || cTickTime > 15000)
		{
			lastError = ERROR_INVALID_CONFIG_VALUE;
			return blinkBothLedsShort();
		}

		boolean cUseBreak = packet->readBoolean();

		config->stepMode = (StepMode)cStepMode;
		config->tickTime = cTickTime;
		config->brakeMode = cUseBreak ? BrakeAlways : BrakeNone;
		break;
	}
	case PacketBlinkGreen:
		blinkGreenLedShort();
		break;
	case PacketBlinkRed:
		blinkRedLedShort();
		break;
	case PacketStop:
	{
		if (currentBusyCommand != BUSY_NONE)
			stopBusyCommand = true;
		else
		{
			stopMove();

			// Client informieren, dass es möglicherweise neue Daten gibt
			sendData(revision);
		}
		break;
	}
	case PacketSetData:
	{
		if (!checkRevision(setDataRevision, revision))
			break;

		setDataRevision = revision;

		for (byte x = 0; x < CLUSTER_WIDTH; x++) {
			for (byte y = 0; y < CLUSTER_HEIGHT; y++)
			{
				StepperData* stepper = getStepper(x, y);

				uint16_t height = packet->readUint16();
				if (height > config->maxSteps) {
					lastError = ERROR_INVALID_HEIGHT;
					return blinkBothLedsShort();
				}

				stepper->CurrentSteps = height;
				stopMove();
			}
		}
		break;
	}
	default:
		lastError = ERROR_UNKNOWN_PACKET;
		return blinkRedLedShort();
	}
}

void runBusy(uint8_t type, int steps, uint16_t delay)
{
	currentBusyCommand = type;
	turnRedLedOn();
	for (int i = 0; i <= delay; i++)
	{
		if (stopBusyCommand)
			break;

		//if (i % 100 == 0)
		//	toogleGreenLed();

		updateSteppers(true);
		usdelay(delay); 

		wdt_reset();

		// Pakete empfangen
		ether.packetLoop(ether.packetReceive());
	}
	currentBusyCommand = BUSY_NONE;
	stopBusyCommand = false;
	turnGreenLedOff();
	turnRedLedOff();
}
#include "network.h"

int32_t loopTime = 0;
int32_t networkTime = 0;
int32_t maxNetworkTime = 0;
int32_t stepperTime = 0;

const static uint8_t networkMac[] = { 0x74, 0x69, 0x69, 0x2D, 0x30, LAN_ID }; // Mac-Adresse des Boards

int32_t configRevision = 0;		// die letzte Revision des Config-Packets
int32_t setDataRevision = 0;	// die letzte Revision des SetData-Packets
int32_t clearErrorRevision = 0; // die letzte Revision des ClearError-Packets

uint8_t currentBusyCommand = BUSY_NONE;
boolean stopBusyCommand = false;

// PacketBuffer benutzt später den gleichen Buffer wie die Ethernetklasse
// daher wird hier nur ein dummyBuffer gesetzt
uint8_t packetBuffer[NETWORK_BUFFER_SIZE];
PacketBuffer packet(packetBuffer, sizeof(packetBuffer));

WiFiUDP* udp;

char hostName[32];
Config newConfig;

// gibt true zurück wenn revision neuer ist als lastRevision
boolean checkRevision(int32_t lastRevision, int32_t revision)
{
	if (lastRevision >= 0 && revision < 0) // Overflow handeln
		return true;
	return revision > lastRevision;
}

void initNetwork()
{
	serialPrintlnF("initNetwork()");
	writeEEPROM("begin");

	wdt_yield();

	// warten bis Verbindung steht
	/*
	serialPrintlnF("Waiting for network link...");
	writeEEPROM("link");
	WiFi.mode(WIFI_STA);
	WiFi.begin("SSID", "password");
	while (!WiFi.isConnected()) {
		toogleGreenLed();
		delay(300);
	}
	turnGreenLedOn();
	*/

	writeEEPROM("host");

	uint8_t lanID = networkMac[5];
	//delay(lanID * 20); // Init verzögern damit das Netzwerk nicht überlastet wird

	// Hostname generieren (die 00 wird durch die lanID in hex ersetzt)
	// #define DHCP_HOSTNAME_MAX_LEN 32
	memset(hostName, 0, sizeof(hostName));
	strncpy(hostName, "Kugelmatik-00", sizeof(hostName));
	hostName[strlen(hostName) - 2] = getHexChar(lanID >> 4);
	hostName[strlen(hostName) - 1] = getHexChar(lanID);

	serialPrintlnF("Opening own AP");
	WiFi.mode(WIFI_AP);
	WiFi.softAP(hostName, "Kugelmatik");

	// ESP32 crasht beim setzen
	// WiFi.setHostname(hostName);
	// WiFi.softAPsetHostname(hostName);

	serialPrintlnF("Link up...");
	serialPrintF("Using hostname: ");
	serialPrintln(hostName);
	writeEEPROM(hostName);
	wdt_yield();
	
	udp = new WiFiUDP();
	if (!udp->begin(PROTOCOL_PORT))
		serialPrintlnF("udp->begin() failed");

	serialPrintlnF("Network boot done!");
	wdt_yield();
	writeEEPROM("done");
}

boolean loopNetwork() {
	startTime(TIMER_NETWORK);
	wdt_yield();

	// Paket abfragen
	int32_t size = udp->parsePacket();

	if (size > 0) {
		if (size <= packet.getBufferSize()) {
			packet.getError(); // Fehler löschen
			packet.resetPosition();
			packet.setSize(size);
			udp->read(packet.getBuffer(), size);
			udp->flush();

			onPacketReceive();
		}
		else {
			protocolError(ERROR_PACKET_SIZE_BUFFER_OVERFLOW);
			serialPrintlnF("Received packet too large to fit into the buffer");
		}
	}

	boolean linkUp = WiFi.getMode() == WIFI_AP || WiFi.isConnected();
	if (!linkUp) 	// wenn Netzwerk Verbindung getrennt wurde
		stopMove(); // stoppen

	networkTime = endTime(TIMER_NETWORK);
	if (networkTime > maxNetworkTime || maxNetworkTime > networkTime * 4) // maxNetworkTime Windup vermeiden
		maxNetworkTime = networkTime;
	return linkUp;
}

void sendPacket()
{
	if (packet.getError())
		return;
	udp->beginPacket(udp->remoteIP(), udp->remotePort());
	udp->write(packet.getBuffer(), packet.getPosition());
	udp->endPacket();
	wdt_yield();
}

void writeHeader(boolean guarenteed, uint8_t packetType, int32_t revision)
{
	packet.resetPosition();
	packet.setSize(packet.getBufferSize()); // Size überschreiben, da die Size vom Lesen gesetzt wird
	packet.write('K');
	packet.write('K');
	packet.write('S');
	packet.write(guarenteed);
	packet.write(packetType);
	packet.write(revision);
}


boolean readPosition(uint8_t* x, uint8_t* y)
{
	uint8_t pos = packet.readUint8();
	uint8_t xvalue = (pos >> 4) & 0xF;
	uint8_t yvalue = pos & 0xF;

	if (xvalue < 0 || xvalue >= CLUSTER_WIDTH)
	{
		protocolError(ERROR_X_INVALID);
		return false;
	}
	if (yvalue < 0 || yvalue >= CLUSTER_HEIGHT)
	{
		protocolError(ERROR_Y_INVALID);
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

	for (uint8_t x = 0; x < CLUSTER_WIDTH; x++) {
		for (uint8_t y = 0; y < CLUSTER_HEIGHT; y++)
		{
			StepperData* stepper = getStepper(x, y);

			packet.write((uint16_t)_max(0, stepper->CurrentSteps));
			packet.write(stepper->WaitTime);
		}
	}

	sendPacket();
}

void sendInfo(int32_t revision) {
	int32_t highestRevision = INT_MIN;
	for (int32_t i = 0; i < CLUSTER_SIZE; i++) {
		StepperData* stepper = getStepper(i);

		if (stepper->LastRevision > highestRevision)
			highestRevision = stepper->LastRevision;
	}

	if (configRevision > highestRevision)
		highestRevision = configRevision;

	if (setDataRevision > highestRevision)
		highestRevision = setDataRevision;

	if (clearErrorRevision > highestRevision)
		highestRevision = clearErrorRevision;

	writeHeader(false, PacketInfo, revision);

	packet.write((uint8_t)BUILD_VERSION);
	packet.write(currentBusyCommand);
	packet.write(highestRevision);
	packet.write(lastError);
	packet.write((uint64_t)freeRam());

	packet.write((uint16_t)sizeof(Config));
	packet.write((uint8_t*)&config, sizeof(Config));

	uint8_t mcpStatus = 0;
	for (uint8_t i = 0; i < MCP_COUNT; i++)
		if (mcps[i].isOK)
			mcpStatus |= (1 << i);
	packet.write(mcpStatus);

	packet.write(loopTime);
	packet.write(networkTime);
	packet.write(maxNetworkTime);
	packet.write(stepperTime);
	packet.write((int32_t)(millis() / 1000));

	sendPacket();
	wdt_yield();
}

void onPacketReceive()
{
	wdt_yield();

	// Fehler aus dem PacketBuffer löschen
	if (packet.getError())
		serialPrintlnF("packet.getError() == true");

	// alle Kugelmatik V3 Pakete
	// sind mindestens HEADER_SIZE Bytes lang
	// und fangen mit "KKS" an
	if (packet.getSize() < HEADER_SIZE)
		return;
	if (packet.readUint8() != 'K')
		return;
	if (packet.readUint8() != 'K')
		return;
	if (packet.readUint8() != 'S')
		return;

	boolean isGuaranteed = packet.readBoolean();

	// erstes Byte nach dem Magicstring gibt den Paket-Typ an
	uint8_t packetType = packet.readUint8();

	// fortlaufende ID für die Pakete werden nach dem Magicstring und dem Paket-Typ geschickt
	int32_t revision = packet.readInt32();

	// wenn ein busy-Befehl läuft dann werden nur Ping, Info und Stop verarbeitet
	if (currentBusyCommand != BUSY_NONE && packetType != PacketPing && packetType != PacketInfo && packetType != PacketStop)
		return;

	wdt_yield();
	handlePacket(packetType, revision);

	// isGuaranteed gibt an ob der Sender eine Antwort erwartet
	if (isGuaranteed)
		sendAckPacket(revision);

	wdt_yield();
}

void handlePacket(uint8_t packetType, int32_t revision)
{
	switch (packetType)
	{
	case PacketPing:
		// das Ping-Packet funktioniert gleichzeitig auch als Echo-Funktion#
		udp->beginPacket(udp->remoteIP(), udp->remotePort());
		udp->write(packet.getBuffer(), packet.getSize());
		udp->endPacket();
		break;
	case PacketStepper:
	{
		uint8_t x, y;
		if (!readPosition(&x, &y))
			return;

		uint16_t height = packet.readUint16();
		uint8_t waitTime = packet.readUint8();

		if (packet.getError())
			return;

		setStepper(revision, x, y, height, waitTime);
		break;
	}
	case PacketSteppers:
	{
		uint8_t length = packet.readUint8();
		uint16_t height = packet.readUint16();
		uint8_t waitTime = packet.readUint8();

		for (uint8_t i = 0; i < length; i++)
		{
			uint8_t x, y;
			readPosition(&x, &y);

			if (packet.getError())
				return;
			setStepper(revision, x, y, height, waitTime);
		}

		break;
	}
	case PacketSteppersArray:
	{
		uint8_t length = packet.readUint8();

		for (uint8_t i = 0; i < length; i++)
		{
			uint8_t x, y;
			if (!readPosition(&x, &y))
				return;

			uint16_t height = packet.readUint16();
			uint8_t waitTime = packet.readUint8();

			if (packet.getError())
				return;

			setStepper(revision, x, y, height, waitTime);
		}
		break;
	}
	case PacketSteppersRectangle:
	{
		uint8_t minX, minY;
		if (!readPosition(&minX, &minY))
			return;

		uint8_t maxX, maxY;
		if (!readPosition(&maxX, &maxY))
			return;

		uint16_t height = packet.readUint16();
		uint8_t waitTime = packet.readUint8();

		if (minX > maxX || minY > maxY)
			return protocolError(ERROR_INVALID_VALUE);

		if (packet.getError())
			return;

		for (uint8_t x = minX; x <= maxX; x++)
			for (uint8_t y = minY; y <= maxY; y++)
				setStepper(revision, x, y, height, waitTime);
		break;
	}
	case PacketSteppersRectangleArray:
	{
		uint8_t minX, minY;
		if (!readPosition(&minX, &minY))
			return;

		uint8_t maxX, maxY;
		if (!readPosition(&maxX, &maxY))
			return;

		if (minX > maxX || minY > maxY)
			return protocolError(ERROR_INVALID_VALUE);

		// beide for-Schleifen müssen mit dem Client übereinstimmen sonst stimmen die Positionen nicht		
		for (uint8_t x = minX; x <= maxX; x++) {
			for (uint8_t y = minY; y <= maxY; y++) {
				uint16_t height = packet.readUint16();
				uint8_t waitTime = packet.readUint8();

				if (packet.getError())
					return;

				setStepper(revision, x, y, height, waitTime);
			}
		}
		break;
	}
	case PacketAllSteppers:
	{
		uint16_t height = packet.readUint16();
		uint8_t waitTime = packet.readUint8();

		if (packet.getError())
			return;
		setAllSteps(revision, height, waitTime);
		break;
	}
	case PacketAllSteppersArray:
	{
		// beide for-Schleifen müssen mit dem Client übereinstimmen sonst stimmen die Positionen nicht		
		for (uint8_t x = 0; x < CLUSTER_WIDTH; x++) {
			for (uint8_t y = 0; y < CLUSTER_HEIGHT; y++) {
				uint16_t height = packet.readUint16();
				uint8_t waitTime = packet.readUint8();

				if (packet.getError())
					return;
				setStepper(revision, x, y, height, waitTime);
			}
		}
		break;
	}
	case PacketHome:
	{
		// 0xABCD wird benutzt damit man nicht ausversehen das Home-Paket schickt (wenn man z.B. den Paket-Type verwechselt)
		int32_t magic = packet.readInt32();

		if (packet.getError())
			return;
		if (magic != 0xABCD)
			return protocolError(ERROR_INVALID_MAGIC);

		stopMove();

		boolean stepperSet = false;

		// Stepper für Home Befehl vorbereiten
		for (int32_t i = 0; i < CLUSTER_SIZE; i++) {
			StepperData* stepper = getStepper(i);

			if (checkRevision(stepper->LastRevision, revision)) {
				forceStepper(stepper, revision, -config.homeSteps);
				stepperSet = true;
			}
		}

		if (stepperSet) {
			runBusy(BUSY_HOME, config.homeSteps, config.homeTime);


			// alle Stepper zurücksetzen
			for (int32_t i = 0; i < CLUSTER_SIZE; i++) {
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
		clearErrorRevision = 0;

		for (int32_t i = 0; i < CLUSTER_SIZE; i++)
			getStepper(i)->LastRevision = 0;
		break;
	}
	case PacketFix:
	{
		// 0xDCBA wird benutzt damit man nicht ausversehen das Fix-Paket schickt (wenn man z.B. den Paket-Type verwechselt)
		int32_t magic = packet.readInt32();
		if (magic != 0xDCBA)
			return protocolError(ERROR_INVALID_MAGIC);

		uint8_t x, y;
		if (!readPosition(&x, &y))
			return;

		if (packet.getError())
			return;

		StepperData* stepper = getStepper(x, y);
		if (checkRevision(stepper->LastRevision, revision))
		{
			stopMove();

			forceStepper(stepper, revision, config.fixSteps);
			runBusy(BUSY_FIX, config.fixSteps, config.fixTime);
			resetStepper(stepper);
		}
		break;
	}
	case PacketHomeStepper:
	{
		// 0xDCBA wird benutzt damit man nicht ausversehen das HomeStepper-Paket schickt (wenn man z.B. den Paket-Type verwechselt)
		int32_t magic = packet.readInt32();
		if (magic != 0xABCD)
			return protocolError(ERROR_INVALID_MAGIC);

		uint8_t x, y;
		if (!readPosition(&x, &y))
			return;

		if (packet.getError())
			return;

		StepperData* stepper = getStepper(x, y);
		if (checkRevision(stepper->LastRevision, revision))
		{
			stopMove();

			forceStepper(stepper, revision, -config.homeSteps);
			runBusy(BUSY_HOME_STEPPER, config.homeSteps, config.homeTime);
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
		sendInfo(revision);
		break;
	}
	case PacketBlinkGreen:
		blinkGreenLedShort(false);
		break;
	case PacketBlinkRed:
		blinkRedLedShort(false);
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

		serialPrintlnF("PacketSetData received");

		for (uint8_t x = 0; x < CLUSTER_WIDTH; x++) {
			for (uint8_t y = 0; y < CLUSTER_HEIGHT; y++) {
				StepperData* stepper = getStepper(x, y);

				uint16_t height = packet.readUint16();
				if (height > config.maxSteps)
					return protocolError(ERROR_INVALID_HEIGHT);

				if (packet.getError())
					return;

				stepper->CurrentSteps = height;
				stepper->GotoSteps = height;
				stepper->TickCount = 0;
			}
		}
		break;
	}
	case PacketConfig2:
	{
		if (!checkRevision(configRevision, revision))
			break;

		configRevision = revision;

		uint16_t size = packet.readUint16();
		if (size != sizeof(Config))
			return protocolError(ERROR_INVALID_CONFIG_VALUE);

		packet.read((uint8_t*)&newConfig, sizeof(Config));

		if (packet.getError()) 
			return;

		if (!checkConfig(&newConfig))
			return protocolError(ERROR_INVALID_CONFIG_VALUE);

		memcpy(&config, &newConfig, sizeof(Config));
		serialPrintlnF("Config2 set by network!");

		sendInfo(revision);
		break;
	}
	case PacketClearError:
		if (!checkRevision(clearErrorRevision, revision))
			break;

		clearErrorRevision = revision;
		lastError = ERROR_NONE;
		sendInfo(revision);
		break;
	default:
		return protocolError(ERROR_UNKNOWN_PACKET);
	}
}

void runBusy(uint8_t type, int32_t steps, uint32_t delay)
{
	serialPrintF("runBusy(type = ");
	serialPrint(type);
	serialPrintlnF(")");

	currentBusyCommand = type;
	turnRedLedOn();
	for (int32_t i = 0; i <= steps; i++) {
		if (stopBusyCommand)
			break;

		if (i % 100 == 0)
			toogleGreenLed();

		if (!runTick(delay, true))
			break;
	}

	currentBusyCommand = BUSY_NONE;
	stopBusyCommand = false;
	turnGreenLedOff();
	turnRedLedOff();

	// Timer zurück setzen, damit LoopTime und NetworkTime nicht kurzzeitig in die Höhe springt
	startTime(TIMER_LOOP);
	startTime(TIMER_NETWORK); 
}
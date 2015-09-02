// Kugelmatik V3
// Firmware
//  Henrik Kunzelmann 2015
//  Rainer Wieland

// Hardware
//  AVR-NET-I/O mit ATmega32 und ENC28J60
//  MCP23S17
//  L293DNE

// Defines
#define DEBUG
#define ENABLE_LOG true // aktiviert den Log der im EEPROM Speicher erzeugt wird

#define BLINK_LED_ASYNC true
#define WRITE_INTERSTRUCTION_POINTER false
#define ENABLE_TIMER1 (BLINK_LED_ASYNC  || WRITE_INTERSTRUCTION_POINTER)
#define DISABLE_INTERRUPTS (!ENABLE_TIMER1)

#define BUILD_VERSION 10

#define LAN_ID 0x11 // ID des Boards im LAN, wird benutzt um die Mac-Adresse zu generieren
static byte ethernetMac[] = { 0x74, 0x69, 0x69, 0x2D, 0x30, LAN_ID }; // Mac-Adresse des Boards


#define TICK_TIME 2000
#define HOME_TIME 5000
#define FIX_TIME 5000

#define ALLOW_STOP_BUSY true // gibt an ob der Client "busy"-Befehle beenden darf (z.B. Home)
#define RECEIVE_PACKETS_BUSY true // gibt an ob der Client bei "busy"-Befehle Pakete empfängt

#define MCP_COUNT 8 // Anzahl der MCP Chips
#define STEPPER_COUNT 4 // Anzahl der Stepper pro MCP Chip

#define CLUSTER_WIDTH 5 // Anzahl Stepper in der Breite (X)
#define CLUSTER_HEIGHT 6 // Anzahl Stepper in der Höhe (Y)

#define STEP_MODE 1 // 1 = Half Step, 2 = Full Step, 3 = Both
#define MAX_STEPS 8000 // Maximale Anzahl an Steps die die Firmware maximal machen darf (nach unten)
#define FIX_STEPS 20000 // Steps die ein Stepper macht um einen Stepper zu fixen
#define USE_BREAK false // Wenn true, dann bremsen die Schrittmotoren

#define LED_GREEN 2 // Port für grüne LED (Pin 4 SUBD)
#define LED_RED 3 // Port für rote LED (Pin 5 SUBD)

#define BLINK_PACKET false // Wenn true, dann blinkt die grüne Led wenn ein Kugelmatik Paket verarbeitet wird

#define PROTOCOL_PORT 14804 // Port für das Protokoll über UDP

#define ETHERNET_BUFFER_SIZE 700 // Größe des Ethernet Buffers in Bytes

// Includes
#include <avr/pgmspace.h>
#include <avr/wdt.h>
#include <limits.h>
#include <EtherCard.h>
#include <EEPROM.h>
#include <I2C.h>
#include <MCP23017.h>
#include <LiquidCrystal.h>
#include "PacketTypes.h"

struct StepperData
{
    int LastRevision; // letzte Revision der Daten
    int CurrentSteps; // derzeitige Schritte die der Motor gemacht hat
    int GotoSteps; // Schritte zu der der Motor gehen soll
    byte CurrentStepIndex; // derzeitiger Stepper Wert Index, siehe stepsStepper
	int TickCount; // derzeitige Tick Anzahl, wenn kleiner als 0 dann wird ein Schritt gemacht und die Variable auf WaitTime gesetzt
	byte WaitTime; // Wert für TickCount nach jedem Schritt
};

struct MCPData
{
    StepperData Steppers[STEPPER_COUNT]; // Schrittmotoren pro MCP
    int MCPAddress;
    MCP23017 MCP;
}
mcps[MCP_COUNT];

// Anweisung die der Schrittmotor machen soll
#define STEPPER_STEP_COUNT 8 // es gibt 8 Anweisungen für den Schrittmotor, siehe stepsStepper
int stepsStepper[STEPPER_STEP_COUNT] = { 0x05, 0x04, 0x06, 0x02, 0x0A, 0x08, 0x09, 0x01 };

// geben die Position des MCPs und des Steppers für jede Kugel an
// erster Index ist die linke untere Kugel, siehe "Cluster_Kugelpositionen.pdf" in "Handbücher"
byte mcpPosition[CLUSTER_WIDTH * CLUSTER_HEIGHT] = { 6, 6, 5, 5, 4, 6, 6, 5, 5, 4, 7, 7, 1, 2, 2, 7, 7, 1, 2, 2, 0, 0, 1, 3, 3, 0, 0, 1, 3, 3 };
byte stepperPosition[CLUSTER_WIDTH * CLUSTER_HEIGHT] = { 2, 3, 2, 3, 1, 1, 0, 1, 0, 0, 1, 0, 3, 2, 3, 2, 3, 2, 1, 0, 2, 3, 1, 2, 3, 1, 0, 0, 1, 0 };

// Buffer für die Ethernet Pakete
byte Ethernet::buffer[ETHERNET_BUFFER_SIZE];

/// Funktionen für die LEDs
boolean ledStateGreen = false; // Status der LED für LED_Green (grüne LED)
boolean ledStateRed = false; // Status der LED für LED_Red (rote LED)

// setzt die grüne LED an
void turnGreenLedOn()
{
    ledStateGreen = true;
    digitalWrite(LED_GREEN, ledStateGreen);
}

// setzt die grüne LED aus
void turnGreenLedOff()
{
    ledStateGreen = false;
    digitalWrite(LED_GREEN, ledStateGreen);
}

// wechselt den Status der grünen LED
void toogleGreenLed()
{
    ledStateGreen = !ledStateGreen;
    digitalWrite(LED_GREEN, ledStateGreen);
}

// läst die grüne Led kurzzeitig blinken
void blinkGreenLedShort()
{
    for (int i = 0; i < 5; i++)
    {
        turnGreenLedOn();
        delay(200);
        turnGreenLedOff();
        delay(200);
    }
}

// stellt die rote LED an
void turnRedLedOn()
{
    ledStateRed = true;
    digitalWrite(LED_RED, ledStateRed);
}

// stellt die rote LED aus
void turnRedLedOff()
{
    ledStateRed = false;
    digitalWrite(LED_RED, ledStateRed);
}

// wechselt den Status der rote LED
void toogleRedLed()
{
    ledStateRed = !ledStateRed;
    digitalWrite(LED_RED, ledStateRed);
}

// läst die rote Led kurzzeitig blinken
void blinkRedLedShort()
{
    for (int i = 0; i < 5; i++)
    {
        turnRedLedOn();
        delay(200);
        turnRedLedOff();
        delay(200);
    }
}

// lässt die rote Led dauerhaft blinken, wird benutzt um einen Fehler zu zeigen
// code gibt ein "Morse-Code" an, d.h. Bit 0 ist kurz und Bit 1 ist lang, nur die 4 niedrigsten Bits werden benutzt
void blinkRedLed(byte code)
{
    wdt_disable(); // Watch Dog deaktivieren damit die Endlosschleife dauerhaft läuft
    int codeIndex = 0;
    while (true)
    {
        turnRedLedOn();

        if (code & (1 << codeIndex) > 0)
            delay(2000);
        else
            delay(500);

        turnRedLedOff();

        delay(500);

        codeIndex++;
        if (codeIndex >= 4)
            codeIndex = 0;
    }
}

// lässt beide LEDs kurzzeitig blinken
void blinkBothLedsShort()
{
    for (int i = 0; i < 5; i++)
    {
        turnRedLedOn();
        turnGreenLedOn();
        delay(200);
        turnRedLedOff();
        turnGreenLedOff();
        delay(200);
    }
}

// setzt den Chip zurück
void softReset()
{
    wdt_enable(WDTO_2S); // Watch Dog aktivieren damit der Chip resetet wird
    while (true) ; // in Endlosschleife gehen damit der Watch Dog den Chip resetet
}

// folgende Funktionen schreiben Bytes oder Strings in den EEPROM-Speicher um einfache Troubleshooting-Infos zu bringen
#define LOG_BEGIN 32 // ab wann der Log im EEPROM-Speicher beginnen soll (in Bytes)
#define LOG_SIZE 512 // Größe des Logs in Bytes, wird diese Grenze überschritten dann gehen alle weiteren Zeichen wieder an den Anfang (LOG_BEGIN)

int logPosition = 0;

void updateEEPROM(int position, byte val) {
	if (EEPROM.read(position) != val)
		EEPROM.write(position, val);
}

// schreibt einen Byte in den Log
void printLog(byte val)
{
#if ENABLE_LOG
    updateEEPROM(LOG_BEGIN + logPosition, val);
    logPosition++;
    if (logPosition >= LOG_SIZE)
        logPosition = 0;
#endif
}

// schreibt einen nullterminierten String in den Log ('\0')
void printLogString(char* str)
{
    for (int i = 0; str[i] != '\0'; i++)
        printLog(str[i]);
    printLog(' ');
}

// schreibt aus einem char-Array len Bytes in den Log
void printLogCharArray(const char* array, word len)
{
    for (int i = 0; i < len; i++)
        printLog(array[i]);
    printLog(' ');
}

// bringt den Chip in den Fehler-Modus und blockiert ihn 
void error(byte code, char* message)
{
    printLogString(message);
    blinkRedLed(code);
    // Endlosschleife von blinkRedLed
}

// initialisiert einen MCP
void initMCP(int index)
{
    MCPData* data = &mcps[index];
    data->MCPAddress = index;

    for (int i = 0; i < STEPPER_COUNT; i++)
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

// initialisiert alle MCPs
void initAllMCPs()
{
    for (int i = 0; i < MCP_COUNT; i++)
        initMCP(i);
}

// gibt true zurück wenn revision neuer ist als lastRevision
boolean checkRevision(int lastRevision, int revision)
{
    if (lastRevision >= 0 && revision < 0) // Overflow handeln
        return true;
    return revision > lastRevision;
}

// setzt einen Stepper auf eine bestimmte Höhe
void setStepper(int revision, byte x, byte y, unsigned short height, byte waitTime)
{
    if (x < 0 || x >= CLUSTER_WIDTH)
        return blinkRedLedShort();
    if (y < 0 || y >= CLUSTER_HEIGHT)
        return blinkRedLedShort();
    if (height > MAX_STEPS)
        return blinkRedLedShort();

    int index = y * CLUSTER_WIDTH + x;

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

// setzt alle Schrittmotoren auf eine bestimmte Höhe
void setAllSteps(int revision, unsigned short gotoSteps, byte waitTime)
{
    if (gotoSteps >= MAX_STEPS)
        return blinkRedLedShort();

    for (int i = 0; i < MCP_COUNT; i++)
        for (int j = 0; j < STEPPER_COUNT; j++)
        {
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

// liest einen int aus einem char-Array angefangen ab offset Bytes
int readInt(const char* data, int offset)
{
	int val = 0;
    val |= data[offset];
    val |= data[offset + 1] << 8;
	val |= data[offset + 2] << 16;
	val |= data[offset + 3] << 24;
    return val;
}

// liest einen unsigned short aus einem char-Array angefangen ab offset Bytes
unsigned short readUnsignedShort(const char* data, int offset)
{
    unsigned short val = 0;
	val |= data[offset];
	val |= data[offset + 1] << 8;
	return val;
}

// schreibt einen unsigned short in einen char-Array angefangen ab offset Bytes
void writeUShort(char* data, int offset, unsigned short val)
{
	data[offset] = val & 0xFF;
	data[offset + 1] = (val >> 8) & 0xFF;
}


// schreibt einen int in einen char-Array angefangen ab offset Bytes
void writeInt(char* data, int offset, int val)
{
	data[offset] = val & 0xFF;
	data[offset + 1] = (val >> 8) & 0xFF;
	data[offset + 2] = (val >> 16) & 0xFF;
	data[offset + 3] = (val >> 24) & 0xFF;
}

void sendAckPacket(int revision)
{
    char packet[] = { 'K', 'K', 'S', 0, PacketAck, 0, 0, 0, 0 };
    writeInt(packet, 5, revision);
    ether.makeUdpReply(packet, sizeof(packet), PROTOCOL_PORT);
}

#define HEADER_SIZE 9 // Größe des Paket-Headers in Bytes

int configRevision = 0; // die letzte Revision für als die Config gesetzt wurde
byte stepMode = STEP_MODE;
int tickTime = TICK_TIME;
boolean useBreak = USE_BREAK;

boolean busyCommandRunning = false;
boolean stopBusyCommand = false;

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
	
	// wenn ein busy-Befehl läuft dann werden nur Ping, Info und Stop verarbeitet
	if (busyCommandRunning && packetType != PacketPing && packetType != PacketInfo && packetType != PacketStop)
		return;
		

    // fortlaufende ID für die Pakete werden nach dem Magicstring und dem Paket-Typ geschickt
    int revision = readInt(data, 5);

    // isGuaranteed gibt an ob der Sender eine Antwort erwartet
    if (isGuaranteed)
        sendAckPacket(revision);

    int offset = HEADER_SIZE;
    switch (packetType)
    {
        case PacketPing:
            ether.makeUdpReply((char*)data, len, PROTOCOL_PORT); // das Ping-Packet funktioniert gleichzeitig auch als Echo-Funktion
            break;
        case PacketStepper:
            if (len < HEADER_SIZE + 4)
                return blinkGreenLedShort();
            else
            {
                byte x = (data[offset] >> 4) & 0xF;
                byte y = data[offset] & 0xF;
                offset += 1;

                unsigned short height = readUnsignedShort(data, offset);
				offset += 2;
				
				byte waitTime = data[offset++];
                setStepper(revision, x, y, height, waitTime);
            }
            break;
        case PacketSteppers:
            if (len < HEADER_SIZE + 5)
                return blinkGreenLedShort();
            else
            {
                byte length = data[offset];
                offset += 1;

                unsigned short height = readUnsignedShort(data, offset);
                offset += 2;
				
				byte waitTime = data[offset++];

                if (len < offset + length)
                    return blinkGreenLedShort();

                for (int i = 0; i < length; i++)
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
                return blinkGreenLedShort();
            else
            {
                byte length = data[offset];
                offset += 1;

                if (len < offset + length * 4)
                    return blinkGreenLedShort();

                for (int i = 0; i < length; i++)
                {
                    byte x = data[offset] >> 4;
                    byte y = data[offset] & 0xF;
                    offset += 1;

                    unsigned int height = readUnsignedShort(data, offset);
					offset += 2;
										
					byte waitTime = data[offset++];
					
                    setStepper(revision, x, y, height, waitTime);
                }
            }
            break;
        case PacketSteppersRectangle:
            if (len < HEADER_SIZE + 2 + 3)
                return blinkGreenLedShort();
            else
            {
                byte minX = data[offset] >> 4;
                byte minY = data[offset] & 0xF;
                offset += 1;

                byte maxX = data[offset] >> 4;
                byte maxY = data[offset] & 0xF;
                offset += 1;

                unsigned int height = readUnsignedShort(data, offset);
                offset += 2;
				
				byte waitTime = data[offset++];

                if (minX < 0 || minY < 0 || maxX >= CLUSTER_WIDTH || maxY >= CLUSTER_HEIGHT)
                    return blinkBothLedsShort();
                if (minX > maxX || minY > maxY)
                    return blinkBothLedsShort();

                for (byte x = minX; x <= maxX; x++)
                    for (byte y = minY; y <= maxY; y++)
                        setStepper(revision, x, y, height, waitTime);
            }
            break;
        case PacketSteppersRectangleArray:
            if (len < HEADER_SIZE + 2)
                return blinkGreenLedShort();
            else
            {
                byte minX = data[offset] >> 4;
                byte minY = data[offset] & 0xF;
                offset += 1;

                byte maxX = data[offset] >> 4;
                byte maxY = data[offset] & 0xF;
                offset += 1;

                if (minX < 0 || minY < 0 || maxX >= CLUSTER_WIDTH || maxY >= CLUSTER_HEIGHT)
                    return blinkBothLedsShort();
                if (minX > maxX || minY > maxY)
                    return blinkBothLedsShort();

                int area = (maxX - minX + 1) * (maxY - minY + 1); // +1, da max die letzte Kugel nicht beinhaltet
                if (len < offset + 3 * area)
                    return blinkGreenLedShort();

				// beide for-Schleifen müssen mit dem Client übereinstimmen sonst stimmen die Positionen nicht		
                for (byte x = minX; x <= maxX; x++)
                    for (byte y = minY; y <= maxY; y++)
                    {
						unsigned int height = readUnsignedShort(data, offset);
						offset += 2;
						
						byte waitTime = data[offset++];
						
                        setStepper(revision, x, y, height, offset);
                    }
            }
            break;
        case PacketAllSteppers: 
            if (len < HEADER_SIZE + 3)
                return blinkGreenLedShort();
            else
            {
                unsigned int height = readUnsignedShort(data, offset);
				offset += 2;
				byte waitTime = data[offset++];
                setAllSteps(revision, height, waitTime);
            }
            break;
        case PacketAllSteppersArray:
            if (len < HEADER_SIZE + 3 * CLUSTER_WIDTH * CLUSTER_HEIGHT)
                return blinkGreenLedShort();
            else
            {
                // beide for-Schleifen müssen mit dem Client übereinstimmen sonst stimmen die Positionen nicht		
                for (int x = 0; x < CLUSTER_WIDTH; x++)
                    for (int y = 0; y < CLUSTER_HEIGHT; y++)
                    {
						unsigned int height = readUnsignedShort(data, offset);
						offset += 2;
						
						byte waitTime = data[offset++];
						
                        setStepper(revision, x, y, height, waitTime);
                    }
            }
            break;
        case PacketHome:
            if (len < HEADER_SIZE + 4)
                return blinkGreenLedShort();
            else
            {
                // 0xABCD wird benutzt damit man nicht ausversehen das Home-Paket schickt (wenn man z.B. den Paket-Type verwechselt)
                if (readInt(data, offset) != 0xABCD)
                    return blinkBothLedsShort();

                // gotoSteps auf -MAX_STEPS setzen damit alle Kugeln voll nach oben fahren (negative Steps sind eigentlich verboten)
                for (int i = 0; i < MCP_COUNT; i++)
                    for (int j = 0; j < STEPPER_COUNT; j++)
                    {
                        StepperData* stepper = &mcps[i].Steppers[j];
                        if (checkRevision(stepper->LastRevision, revision))
                        {
                            stepper->LastRevision = revision;
							stepper->CurrentSteps = 0;
                            stepper->GotoSteps = -MAX_STEPS;
                            stepper->CurrentStepIndex = 0;
							stepper->WaitTime = 0;
							stepper->TickCount = 0;
                        }
                    }

				busyCommandRunning = true;
                // alle Stepper nach oben Fahren lassen
                turnRedLedOn();
                for (int i = 0; i < MAX_STEPS; i++)
                {
					if (stopBusyCommand)
						break;
										
	                if (i % 100 == 0)
						toogleGreenLed();
	                updateSteppers(true);
	                usdelay(HOME_TIME); // langsam bewegen
					
					// Pakete empfangen
					#if RECEIVE_PACKETS_BUSY
						ether.packetLoop(ether.packetReceive());
					#endif
                }
				busyCommandRunning = false;
				stopBusyCommand = false;
                turnGreenLedOff();
                turnRedLedOff();

				// alle Stepper zurücksetzen
                for (int i = 0; i < MCP_COUNT; i++)
                    for (int j = 0; j < STEPPER_COUNT; j++)
                    {
                        StepperData* stepper = &mcps[i].Steppers[j];
                        stepper->GotoSteps = 0;
                        stepper->CurrentSteps = 0;
                        stepper->CurrentStepIndex = 0;
						stepper->WaitTime = 0;
						stepper->TickCount = 0;
                    }
			}
            break;
        case PacketResetRevision:
            for (int i = 0; i < MCP_COUNT; i++)
                for (int j = 0; j < STEPPER_COUNT; j++) {
	                StepperData* stepper = &mcps[i].Steppers[j];
	                stepper->LastRevision = 0;
                }
            break;
        case PacketFix:
            if (len < HEADER_SIZE + 5)
                return blinkGreenLedShort();
            else
            {
                // 0xDCBA wird benutzt damit man nicht ausversehen das Fix-Paket schickt (wenn man z.B. den Paket-Type verwechselt)
                if (readInt(data, offset) != 0xDCBA)
                    return blinkBothLedsShort();
                offset += 4;

                byte x = (data[offset] >> 4) & 0xF;
                byte y = data[offset] & 0xF;
                offset += 1;

                if (x < 0 || x >= CLUSTER_WIDTH)
                    return blinkBothLedsShort();
                if (y < 0 || y >= CLUSTER_HEIGHT)
                    return blinkBothLedsShort();

                int index = y * CLUSTER_WIDTH + x;
                MCPData* data = &mcps[mcpPosition[index]];
                StepperData* stepper = &data->Steppers[stepperPosition[index]];
                if (checkRevision(stepper->LastRevision, revision))
                {
                    stepper->LastRevision = revision;
					stepper->CurrentSteps = 0;
                    stepper->GotoSteps = FIX_STEPS;
					stepper->WaitTime = 0;
					stepper->TickCount = 0;

					turnRedLedOn();
					busyCommandRunning = true;
                    for (int i = 0; i < FIX_STEPS; i++)
                    {
						if (stopBusyCommand)
							break;
						if (i % 100 == 0)
							toogleGreenLed();
                        updateSteppers(true);
                        usdelay(FIX_TIME); // langsam bewegen
						
						// Pakete empfangen
						#if RECEIVE_PACKETS_BUSY
							ether.packetLoop(ether.packetReceive());
						#endif
                    }
					
					// zweiter Teil kann extra gestoppt werden
					stopBusyCommand = false;

                    delay(1000);
                    stepper->GotoSteps = 0; // wieder komplett hochfahren

                    for (int i = 0; i < FIX_STEPS; i++)
                    {
						if (stopBusyCommand)
							break;
						if (i % 100 == 0)
							toogleGreenLed();
                        updateSteppers(true);
                        usdelay(FIX_TIME); // langsam bewegen
						
						// Pakete empfangen
						#if RECEIVE_PACKETS_BUSY
							ether.packetLoop(ether.packetReceive());
						#endif
                    }
					busyCommandRunning = false;
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
                return blinkGreenLedShort();
            else
            {
                // 0xDCBA wird benutzt damit man nicht ausversehen das HomeStepper-Paket schickt (wenn man z.B. den Paket-Type verwechselt)
                if (readInt(data, offset) != 0xABCD)
                    return blinkBothLedsShort();
                offset += 4;

                byte x = (data[offset] >> 4) & 0xF;
                byte y = data[offset] & 0xF;
                offset += 1;

                if (x < 0 || x >= CLUSTER_WIDTH)
                    return blinkBothLedsShort();
                if (y < 0 || y >= CLUSTER_HEIGHT)
                    return blinkBothLedsShort();

                int index = y * CLUSTER_WIDTH + x;
                MCPData* data = &mcps[mcpPosition[index]];
                StepperData* stepper = &data->Steppers[stepperPosition[index]];
                if (checkRevision(stepper->LastRevision, revision))
                {
					// Stepper so setzen das er komplett hoch fährt
                    stepper->LastRevision = revision;
					stepper->CurrentSteps = MAX_STEPS;
                    stepper->GotoSteps = 0;
					stepper->WaitTime = 0; // WaitTime zurück setzen
					stepper->TickCount = 0;

					busyCommandRunning = true;
					turnRedLedOn();
                    for (int i = 0; i < MAX_STEPS; i++)
                    {
						if (stopBusyCommand)
							break;
						if (i % 100 == 0)
							toogleGreenLed();
                        updateSteppers(true);
                        usdelay(HOME_TIME); // langsam bewegen
						
						// Pakete empfangen
						#if RECEIVE_PACKETS_BUSY
							ether.packetLoop(ether.packetReceive());
						#endif
                    }
					busyCommandRunning = false;
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
				char data[HEADER_SIZE + CLUSTER_WIDTH * CLUSTER_HEIGHT * 3];
				data[0] = 'K';
				data[1] = 'K';
				data[2] = 'S';
				data[3] = 0; 
				data[4] = PacketGetData; // Paket-Type GetData
				writeInt(data, 5, revision);
			
				int offsetData = HEADER_SIZE;
				for (int x = 0; x < CLUSTER_WIDTH; x++)
					for (int y = 0; y < CLUSTER_HEIGHT; y++)
					{
						int index = y * CLUSTER_WIDTH + x;
						MCPData* mcp = &mcps[mcpPosition[index]];
						StepperData* stepper = &mcp->Steppers[stepperPosition[index]];
						writeUShort(data, offsetData, (unsigned short)stepper->CurrentSteps);
					
						offsetData += 2;
						
						data[offsetData++] = stepper->WaitTime;
					}
			
				if (offsetData > sizeof(data))
					error(1, "buffer overflow");
				ether.makeUdpReply(data, sizeof(data), PROTOCOL_PORT); 
			}
			break;
		case PacketInfo:
			{
				char data[HEADER_SIZE + 12];
				data[0] = 'K';
				data[1] = 'K';
				data[2] = 'S';
				data[3] = 0;
				data[4] = PacketInfo; // Paket-Type Info
				writeInt(data, 5, revision);
			
				int offsetData = HEADER_SIZE;
				data[offsetData++] = BUILD_VERSION;
				
				data[offsetData++] = busyCommandRunning ? 1 : 0;
				
				int highestRevision = INT_MIN;
				for (int i = 0; i < MCP_COUNT; i++)
					for (int j = 0; j < STEPPER_COUNT; j++)  {
						StepperData* stepper = &mcps[i].Steppers[j];
						
						if (stepper->LastRevision > highestRevision)
							highestRevision = stepper->LastRevision;
					}
				
				if (configRevision > highestRevision)
					highestRevision = configRevision;
					
				writeInt(data, offsetData, highestRevision);
				offsetData += 4;
				
				data[offsetData++] = stepMode;
				writeInt(data, offsetData, tickTime);
				offsetData += 4;
				
				data[offsetData++] = useBreak ? 1 : 0;
			
				ether.makeUdpReply(data, sizeof(data), PROTOCOL_PORT);
				if (offsetData > sizeof(data))
					error(1, "buffer overflow");
			}
			break;
		case PacketConfig:
			if (len < HEADER_SIZE + 5)
				return blinkGreenLedShort();
			else
			{
				if (!checkRevision(configRevision, revision))
					break;
					
				configRevision = revision;
				
				byte cStepMode = data[offset++];
				if (cStepMode < 1 || cStepMode > 3)
					return blinkBothLedsShort();
					
				int cTickTime = readInt(data, offset);
				offset += 4;
				if (cTickTime < 50 || cTickTime > 15000)
					return blinkBothLedsShort();
					
				boolean cUseBreak = data[offset++] > 0;
					
				stepMode = cStepMode;
				tickTime = cTickTime;
				useBreak = cUseBreak;
			}
			break;
		case PacketBlinkGreen:
			blinkGreenLedShort();
			break;
		case PacketBlinkRed:
			blinkRedLedShort();
			break;
		case PacketStop:
			#ifdef ALLOW_STOP_BUSY
				if (busyCommandRunning)
					stopBusyCommand = true;
			#else
				blinkBothLedsShort();
			#endif
			break;
        default:
            return blinkRedLedShort();
    }
	#if BLINK_PACKET
		turnGreenLedOn();
	#endif
}


void setup()
{
    wdt_disable(); // Watch Dog deaktivieren, da er nicht benutzt wird und durch softReset() noch aktiviert sein kann
    printLogString("init");
	
	#if ENABLE_TIMER1
		printLogString("timer1");

		TCCR1B |= (1 << CS11);
		TCNT1 = 0;
		TIMSK |= (1 << TOIE1);
		sei();
	#endif

    // LEDs setzen
    pinMode(LED_GREEN, OUTPUT);
    pinMode(LED_RED, OUTPUT);

    turnGreenLedOn();
    initAllMCPs();

    delay(LAN_ID * 10); // Init verzögern damit das Netzwerk nicht überlastet wird

    printLogString("ebegin");
    uint8_t rev = ether.begin(sizeof Ethernet::buffer, ethernetMac, 28);
    if (rev == 0)
    {
        error(1, "ethernet begin failed");
        return; // wird niemals passieren da error() in eine Endloschleife geht
    }

    printLogString("dhcp");
    if (!ether.dhcpSetup())
    {
        error(2, "dhcp failed");
        return;
    }

    // IP in den Log schreiben
    printLogString("ip");
    for (int i = 0; i < 4; i++) // IPv4 -> 4 Bytes
        printLog(ether.myip[i]);

    ether.udpServerListenOnPort(&onPacketReceive, PROTOCOL_PORT);

	turnGreenLedOff();
}

void updateSteppers(boolean alwaysUseHalfStep)
{
    for (int i = 0; i < MCP_COUNT; i++)
    {
        // Wert für den GPIO
        unsigned int gpioValue = 0;

        for (int j = 0; j < STEPPER_COUNT; j++)
        {
            StepperData* stepper = &mcps[i].Steppers[j];

			int stepSize = 0;
			int diff = abs(stepper->CurrentSteps - stepper->GotoSteps);
			if (diff != 0) 
			{
				if (stepMode == 3) // StepMode Both, schauen ob wir Full oder Half Step gemacht werden soll
					stepSize = min(2, diff);
				else if (diff >= stepMode) // Half oder Full Step machen
					stepSize = stepMode;
			}
			
			if (alwaysUseHalfStep)
				stepSize = min(stepSize, 1);
			
			if (stepper->WaitTime > 0 && stepper->TickCount >= 0)
				stepper->TickCount--;
			
            if (stepSize > 0 && (stepper->WaitTime == 0 || stepper->TickCount < 0))
            {
                int stepperIndex = stepper->CurrentStepIndex;
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
            else if (useBreak) // Bremse benutzen?
            	gpioValue |= stepsStepper[stepper->CurrentStepIndex] << (4 * j); 
        }
        mcps[i].MCP.writeGPIOS(gpioValue);
    }
}

static void usdelay(unsigned int us)
{
    while (us--)
    {
        // 4 times * 4 cycles gives 16 cyles = 1 us with 16 MHz clocking
        unsigned char i = 4;
        // this while loop executes with exact 4 cycles:
        while (i--)
            asm volatile("nop");
    }
}

void loop()
{	
	#if DISABLE_INTERRUPTS
		noInterrupts();
	#endif
    unsigned long procStart = micros();

    updateSteppers(false);

	while(true)
	{
		// Packet abfragen
		word plen = ether.packetReceive();
		if (plen > 0) // neues Packet empfangen
		{ 
			ether.packetLoop(plen);
		}
			
		#if BLINK_PACKET
			turnGreenLedOff();
		#endif
	
		// schauen ob wir die Stepper updaten müssen
		unsigned long time = micros();
		if (time < procStart) // overflow von micros() handeln
			break;
		
		if (time - procStart >= tickTime)
			break;
	}
	#if DISABLE_INTERRUPTS
		interrupts();
	#endif
}

int tickCount = 0;

ISR(TIMER1_OVF_vect) {
	#if BLINK_LED_ASYNC
		if (tickCount % 128 == 0)
			toogleGreenLed();
	#endif
	#if WRITE_INTERSTRUCTION_POINTER
		if (tickCount % 128 == 0) {
			byte addr1 = 0;
			byte addr2 = 0;
			// Adresse von Stack nehmen
			asm volatile ("pop %0" : "=w"(addr1));
			asm volatile ("pop %0" : "=w"(addr2));
					
			// Adresse wieder auf Stack kopieren
			asm volatile ("push %0" ::  "w"(addr2));
			asm volatile ("push %0" ::  "w"(addr1));
		
			updateEEPROM(1, addr1);
			updateEEPROM(2, addr2);
		}
	#endif
	tickCount++;
}
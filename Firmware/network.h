#ifndef _NETWORK_h
#define _NETWORK_h

#include "Arduino.h"
#include <avr/wdt.h> 
#include <limits.h>
#include <EtherCard.h>

#include "util.h"
#include "constants.h"
#include "config.h"
#include "PacketBuffer.h"

#define LAN_ID 0x11					// ID des Boards im LAN, wird benutzt um die Mac-Adresse zu generieren
#define PROTOCOL_PORT 14804			// Port für das Protokoll über UDP
#define ETHERNET_BUFFER_SIZE 300	// Größe des Ethernet Buffers in Bytes																		
#define HEADER_SIZE 9				// Größe des Paket-Headers in Bytes

#define HEX_STR(x) ((x & HEX) >= 10 ? ('A' + (x - 10)) : ('0' + x))



boolean checkRevision(int32_t lastRevision, int32_t revision);


void initNetwork();

void sendPacket();
void writeHeader(bool guarenteed, byte packetType, int32_t revision);
bool readPosition(PacketBuffer* packet, byte* x, byte* y);

void sendAckPacket(int32_t revision);
void sendData(int32_t revision);
void sendInfo(int32_t revision);

void onPacketReceive(uint16_t dest_port, uint8_t src_ip[4], uint16_t src_port, const char* data, uint16_t len);

void runBusy(uint8_t type, int steps, uint16_t delay);

#endif
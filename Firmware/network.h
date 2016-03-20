#ifndef _NETWORK_h
#define _NETWORK_h

#include "Arduino.h"
#include <avr/wdt.h> 
#include <limits.h>
#include <EtherCard.h>

#include "util.h"
#include "constants.h"
#include "config.h"

#define LAN_ID 0x11					// ID des Boards im LAN, wird benutzt um die Mac-Adresse zu generieren
#define PROTOCOL_PORT 14804			// Port für das Protokoll über UDP
#define ETHERNET_BUFFER_SIZE 550	// Größe des Ethernet Buffers in Bytes																		
#define HEADER_SIZE 9				// Größe des Paket-Headers in Bytes

extern byte lastError;

boolean checkRevision(int32_t lastRevision, int32_t revision);

uint16_t readUInt16(const char* data, int32_t offset);
int32_t readInt32(const char* data, int32_t offset);
void writeUInt16(char* data, int32_t offset, const uint16_t val);
void writeInt32(char* data, int32_t offset, const int32_t val);

void initNetwork();

void sendAckPacket(int32_t revision);
void sendData(int32_t revision);
void onPacketReceive(uint16_t dest_port, uint8_t src_ip[4], uint16_t src_port, const char* data, uint16_t len);

#endif
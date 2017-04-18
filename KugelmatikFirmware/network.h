#pragma once

#include <Arduino.h>

#include <limits.h>
#include <EtherCard.h>

#include "util.h"
#include "constants.h"
#include "stepper.h"
#include "tick.h"
#include "config.h"
#include "watchdog.h"
#include "PacketBuffer.h"

#define LAN_ID 0x11					// ID des Boards im LAN, wird benutzt um die Mac-Adresse zu generieren
#define PROTOCOL_PORT 14804			// Port für das Protokoll über UDP
#define ETHERNET_BUFFER_SIZE 300	// Größe des Ethernet Buffers in Bytes	
#define HEADER_SIZE 9				// Größe des Paket-Headers in Bytes

extern int32_t loopTime;
extern int32_t networkTime;
extern int32_t maxNetworkTime;
extern int32_t stepperTime;

boolean checkRevision(int32_t lastRevision, int32_t revision);

void initNetwork();
boolean loopNetwork();

void sendPacket();
void writeHeader(boolean guarenteed, uint8_t packetType, int32_t revision);
boolean readPosition(PacketBuffer* packet, uint8_t* x, uint8_t* y);

void sendAckPacket(int32_t revision);
void sendData(int32_t revision);
void sendInfo(int32_t revision);

void onPacketReceive(uint16_t dest_port, uint8_t src_ip[4], uint16_t src_port, const char* data, uint16_t len);
void handlePacket(uint8_t packetType, int32_t revision);

void runBusy(uint8_t type, int32_t steps, uint32_t delay);
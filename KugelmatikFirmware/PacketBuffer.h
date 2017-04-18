#pragma once

#include <Arduino.h>

#include "BinaryHelper.h"
#include "util.h"

class PacketBuffer {
private:
	uint8_t* data; // Zeiger auf den Buffer
	uint32_t bufferSize;

	uint32_t position; // aktuelle Position
	uint32_t size; // Länge des Pakets

	boolean error;
	boolean allowRead;

	boolean assertNull(void* pointer);

	boolean assertRead(); // überprüft ob das Lesen zulässig ist
	boolean assertPosition(size_t length); // überprüft die Position
	boolean assertPositionRead(size_t length); // überprüft assertRead und überprüft die Position
	boolean assertPositionWrite(size_t length); // setzt allowRead auf false und überprüft die Position
	uint32_t addPosition(size_t length); // addiert einen Wert zur Position und gibt die alte Position zurück

	void errorMemory();

public:
	explicit PacketBuffer(size_t size);
	explicit PacketBuffer(uint8_t* data, size_t size);
	~PacketBuffer();

	boolean getError();

	void setBuffer(uint8_t* data, size_t size);

	uint8_t* getBuffer() const;
	uint32_t getBufferSize() const;

	uint32_t getPosition() const;
	void seek(uint32_t offset); // überspringt offset Bytes
	void resetPosition();

	// setzt die Position und die Länge des Pakets auf 0
	void reset();

	// gibt die Länge des Pakets in Bytes zurück
	uint32_t getSize() const;

	// setzt die Länge des Pakets in Bytes
	void setSize(uint32_t size);

	boolean readBoolean();
	int8_t readInt8();
	uint8_t readUint8();
	int16_t readInt16();
	uint16_t readUint16();
	int32_t readInt32();
	uint32_t readUint32();
	int64_t readInt64();
	uint64_t readUint64();

	float readFloat();
	double readDouble();

	void read(uint8_t* buffer, size_t length);
	void read(uint8_t* buffer, size_t length, uint32_t offset);
	void read(char* buffer, size_t length, uint32_t offset);

	uint8_t* getBufferRegion(size_t size);

	char* readString();

	void write(boolean value);
	void write(char value);
	void write(int8_t value);
	void write(uint8_t value);
	void write(int16_t value);
	void write(uint16_t value);
	void write(int32_t value);
	void write(uint32_t value);
	void write(int64_t value);
	void write(uint64_t value);

	void write(float value);
	void write(double value);

	void write(uint8_t* buffer, size_t length);
	void write(uint8_t* buffer, size_t length, uint32_t offset);

	void writeString(char* str);
	void writeString(const char* str);
};
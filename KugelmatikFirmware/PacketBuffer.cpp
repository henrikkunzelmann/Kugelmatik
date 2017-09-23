#include "PacketBuffer.h"

PacketBuffer::PacketBuffer(size_t size) {
	this->data = new uint8_t[size];
	this->bufferSize = size;

	this->position = 0;
	this->size = size;

	this->error = false;
	this->allowRead = true;

	if (this->data == NULL)
		errorMemory();
}

PacketBuffer::PacketBuffer(uint8_t* data, size_t size) {
	setBuffer(data, size);
	this->error = false;
}

void PacketBuffer::setBuffer(uint8_t* data, size_t size) {
	if (!assertNull(data))
		return;

	this->data = data;
	this->bufferSize = size;

	this->position = 0;
	this->size = size;
	this->allowRead = true;
}


PacketBuffer::~PacketBuffer()
{
	if (data)
		free(data);
}

boolean PacketBuffer::getError() {
	boolean err = error;
	this->error = false;
	return err;
}

uint8_t* PacketBuffer::getBuffer() const {
	return data;
}

uint32_t PacketBuffer::getBufferSize() const {
	return bufferSize;
}

boolean PacketBuffer::assertNull(void* pointer) {
	if (pointer != NULL)
		return true;

	internalError();
	error = true;
	return false;
}

boolean PacketBuffer::assertRead() {
	if (error)
		return false;

	if (!allowRead) {
		error = true;
		internalError(ERROR_NOT_ALLOWED_TO_READ);
	}
	return allowRead;
}

boolean PacketBuffer::assertPosition(size_t length) {
	if (error)
		return false;

	if (position + length > size) {
		error = true;
		protocolError(ERROR_PACKET_TOO_SHORT);
		return false;
	}
	if (position + length > bufferSize) {
		error = true;
		protocolError(ERROR_PACKET_TOO_SHORT);
		return false;
	}
	return true;
}

boolean PacketBuffer::assertPositionRead(size_t length) {
	if (!assertRead())
		return false;
	return assertPosition(length);
}

boolean PacketBuffer::assertPositionWrite(size_t length) {
	allowRead = false;
	return assertPosition(length);
}

uint32_t PacketBuffer::addPosition(size_t length) {
	uint32_t old = position;
	if (assertPosition(length))
		position += length;
	return old;
}

void PacketBuffer::errorMemory() {
	writeEEPROM("memError");
	internalError();
	error = true;
}

uint32_t PacketBuffer::getPosition() const {
	return position;
}

void PacketBuffer::seek(uint32_t offset) {
	if (assertPosition(offset))
		position += offset;
}

void PacketBuffer::resetPosition() {
	position = 0;
	allowRead = true;
}

void PacketBuffer::reset() {
	resetPosition();
	setSize(0);
}

uint32_t PacketBuffer::getSize() const {
	return size;
}

void PacketBuffer::setSize(uint32_t size) {
	if (size > bufferSize) {
		internalError(ERROR_PACKET_SIZE_BUFFER_OVERFLOW);
		error = true;
		return;
	}

	this->size = size;
}

boolean PacketBuffer::readBoolean() {
	return readUint8() > 0;
}

int8_t PacketBuffer::readInt8() {
	if (!assertPositionRead(sizeof(int8_t)))
		return 0;

	return data[addPosition(sizeof(int8_t))];
}

uint8_t PacketBuffer::readUint8() {
	if (!assertPositionRead(sizeof(uint8_t)))
		return 0;

	return data[addPosition(sizeof(uint8_t))];
}

int16_t PacketBuffer::readInt16() {
	if (!assertPositionRead(sizeof(int16_t)))
		return 0;

	return BinaryHelper::readInt16(data, addPosition(sizeof(int16_t)));
}

uint16_t PacketBuffer::readUint16() {
	if (!assertPositionRead(sizeof(uint16_t)))
		return 0;

	return BinaryHelper::readUint16(data, addPosition(sizeof(uint16_t)));
}

int32_t PacketBuffer::readInt32() {
	if (!assertPositionRead(sizeof(int32_t)))
		return 0;

	return BinaryHelper::readInt32(data, addPosition(sizeof(int32_t)));
}

uint32_t PacketBuffer::readUint32() {
	if (!assertPositionRead(sizeof(uint32_t)))
		return 0;

	return BinaryHelper::readUint32(data, addPosition(sizeof(uint32_t)));
}

int64_t PacketBuffer::readInt64() {
	if (!assertPositionRead(sizeof(int64_t)))
		return 0;

	return BinaryHelper::readInt64(data, addPosition(sizeof(int64_t)));
}

uint64_t PacketBuffer::readUint64() {
	if (!assertPositionRead(sizeof(uint64_t)))
		return 0;

	return BinaryHelper::readUint64(data, addPosition(sizeof(uint64_t)));
}

float PacketBuffer::readFloat() {
	if (!assertPositionRead(sizeof(float)))
		return 0;

	return BinaryHelper::readFloat(data, addPosition(sizeof(float)));
}

double PacketBuffer::readDouble() {
	if (!assertPositionRead(sizeof(double)))
		return 0;

	return BinaryHelper::readDouble(data, addPosition(sizeof(double)));
}

void PacketBuffer::read(uint8_t* buffer, size_t length) {
	read(buffer, length, 0);
}

void PacketBuffer::read(uint8_t* buffer, size_t length, uint32_t offset) {
	if (!assertNull(buffer))
		return;

	uint8_t* dest = buffer + offset;

	if (!assertPositionRead(length)) {
		memset(dest, 0, length);
		return;
	}

	uint8_t* source = this->data + addPosition(length);
	memcpy(dest, source, length);
}

void PacketBuffer::read(char* buffer, size_t length, uint32_t offset) {
	read((uint8_t*)buffer, length, offset);
}

uint8_t* PacketBuffer::getBufferRegion(size_t size) {
	if (!assertPosition(size))
		return this->data;

	return this->data + addPosition(size);
}

char* PacketBuffer::readString() {
	uint16_t length = readUint16();
	if (error)
		return NULL;

	size_t size = length * sizeof(char);

	char* str = (char*)malloc(size + 1);
	if (str == NULL) {
		errorMemory();
		return NULL;
	}

	read(str, size, 0);

	if (error) {
		free(str);
		return NULL;
	}

	str[size] = '\0';
	return str;
}

void PacketBuffer::write(char value) {
	if (assertPositionWrite(sizeof(value)))
		data[addPosition(sizeof(value))] = value;
}

void PacketBuffer::write(uint8_t value) {
	if (assertPositionWrite(sizeof(value)))
		data[addPosition(sizeof(value))] = value;
}

void PacketBuffer::write(uint16_t value) {
	if (assertPositionWrite(sizeof(value)))
		BinaryHelper::writeUint16(data, addPosition(sizeof(value)), value);
}

void PacketBuffer::write(uint32_t value) {
	if (assertPositionWrite(sizeof(value)))
		BinaryHelper::writeUint32(data, addPosition(sizeof(value)), value);
}

void PacketBuffer::write(uint64_t value) {
	if (assertPositionWrite(sizeof(value)))
		BinaryHelper::writeUint64(data, addPosition(sizeof(value)), value);
}

void PacketBuffer::write(int8_t value) {
	if (assertPositionWrite(sizeof(value)))
		data[addPosition(sizeof(value))] = value;
}

void PacketBuffer::write(int16_t value) {
	if (assertPositionWrite(sizeof(value)))
		BinaryHelper::writeInt16(data, addPosition(sizeof(value)), value);
}

void PacketBuffer::write(int32_t value) {
	if (assertPositionWrite(sizeof(value)))
		BinaryHelper::writeInt32(data, addPosition(sizeof(value)), value);
}

void PacketBuffer::write(int64_t value) {
	if (assertPositionWrite(sizeof(value)))
		BinaryHelper::writeInt64(data, addPosition(sizeof(value)), value);
}

void PacketBuffer::write(float value) {
	if (assertPositionWrite(sizeof(value)))
		BinaryHelper::writeFloat(data, addPosition(sizeof(value)), value);
}

void PacketBuffer::write(double value) {
	if (assertPositionWrite(sizeof(value)))
		BinaryHelper::writeDouble(data, addPosition(sizeof(value)), value);
}

void PacketBuffer::write(uint8_t* buffer, size_t length) {
	write(buffer, length, 0);
}

void PacketBuffer::write(uint8_t* buffer, size_t length, uint32_t offset) {
	if (!assertNull(buffer))
		return;

	if (!assertPositionWrite(length))
		return;

	uint8_t* dest = this->data + addPosition(length);
	uint8_t* source = buffer + offset;

	memcpy(dest, source, length);
}

void PacketBuffer::writeString(char* str) {
	if (!assertNull(str))
		return;

	size_t size = sizeof(char) * strlen(str);
	if (!assertPositionWrite(size + sizeof(uint16_t)))
		return;

	write((uint16_t)strlen(str));
	write((uint8_t*)str, size);
}

void PacketBuffer::writeString(const char* str) {
	writeString((char*)str);
}
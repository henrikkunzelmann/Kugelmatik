#include "BinaryHelper.h"

void BinaryHelper::changeByteOrder(uint8_t* buffer, int32_t offset, size_t length) {
#if NEED_SWAPPING
	int32_t end = offset + length;
	int32_t swapsLeft = length / 2;

	while (swapsLeft > 0) {
		uint8_t* a = buffer + (offset + swapsLeft - 1);
		uint8_t* b = buffer + (end - swapsLeft);

		uint8_t temp = *a;
		*a = *b;
		*b = temp;
		swapsLeft--;
	}
#endif
}

void BinaryHelper::writeInt16(uint8_t* buf, int32_t offset, int16_t val) {
	memcpy(buf + offset, &val, sizeof(val));
	changeByteOrder(buf, offset, sizeof(val));
}

int16_t BinaryHelper::readInt16(uint8_t* buf, int32_t offset) {
	int16_t value;
	memcpy(&value, buf + offset, sizeof(value));
	changeByteOrder((uint8_t*)&value, 0, sizeof(value));
	return value;
}

void BinaryHelper::writeUint16(uint8_t* buf, int32_t offset, uint16_t val) {
	memcpy(buf + offset, &val, sizeof(val));
	changeByteOrder(buf, offset, sizeof(val));
}

uint16_t BinaryHelper::readUint16(uint8_t* buf, int32_t offset) {
	uint16_t value;
	memcpy(&value, buf + offset, sizeof(value));
	changeByteOrder((uint8_t*)&value, 0, sizeof(value));
	return value;
}

void BinaryHelper::writeInt32(uint8_t* buf, int32_t offset, int32_t val) {
	memcpy(buf + offset, &val, sizeof(val));
	changeByteOrder(buf, offset, sizeof(val));
}

int32_t BinaryHelper::readInt32(uint8_t* buf, int32_t offset) {
	int32_t value;
	memcpy(&value, buf + offset, sizeof(value));
	changeByteOrder((uint8_t*)&value, 0, sizeof(value));
	return value;
}

void BinaryHelper::writeUint32(uint8_t* buf, int32_t offset, uint32_t val) {
	memcpy(buf + offset, &val, sizeof(val));
	changeByteOrder(buf, offset, sizeof(val));
}

uint32_t BinaryHelper::readUint32(uint8_t* buf, int32_t offset) {
	uint32_t value;
	memcpy(&value, buf + offset, sizeof(value));
	changeByteOrder((uint8_t*)&value, 0, sizeof(value));
	return value;
}

void BinaryHelper::writeInt64(uint8_t* buf, int32_t offset, int64_t val) {
	memcpy(buf + offset, &val, sizeof(val));
	changeByteOrder(buf, offset, sizeof(val));
}


int64_t BinaryHelper::readInt64(uint8_t* buf, int32_t offset) {
	int64_t value;
	memcpy(&value, buf + offset, sizeof(value));
	changeByteOrder((uint8_t*)&value, 0, sizeof(value));
	return value;
}


void BinaryHelper::writeUint64(uint8_t* buf, int32_t offset, uint64_t val) {
	memcpy(buf + offset, &val, sizeof(val));
	changeByteOrder(buf, offset, sizeof(val));
}


uint64_t BinaryHelper::readUint64(uint8_t* buf, int32_t offset) {
	uint64_t value;
	memcpy(&value, buf + offset, sizeof(value));
	changeByteOrder((uint8_t*)&value, 0, sizeof(value));
	return value;
}

void BinaryHelper::writeFloat(uint8_t* buf, int32_t offset, float val) {
	memcpy(buf + offset, &val, sizeof(float));
}

float BinaryHelper::readFloat(uint8_t* buf, int32_t offset) {
	float value;
	memcpy(&value, buf + offset, sizeof(value));
	return value;
}

void BinaryHelper::writeDouble(uint8_t* buf, int32_t offset, double val) {
	memcpy(buf + offset, &val, sizeof(double));
}

double BinaryHelper::readDouble(uint8_t* buf, int32_t offset) {
	double value;
	memcpy(&value, buf + offset, sizeof(value));
	return value;
}

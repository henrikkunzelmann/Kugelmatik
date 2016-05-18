#ifndef CONSTANTS_H_
#define CONSTANTS_H_

#define BUILD_VERSION 15

#define PacketPing 1
#define PacketAck 2
#define PacketStepper 3
#define PacketSteppers 4
#define PacketSteppersArray 5
#define PacketSteppersRectangle 6
#define PacketSteppersRectangleArray 7
#define PacketAllSteppers 8
#define PacketAllSteppersArray 9

#define PacketHome 10

#define PacketResetRevision 11
#define PacketFix 12

#define PacketHomeStepper 13

#define PacketGetData 14
#define PacketInfo 15
#define PacketConfig 16

#define PacketBlinkGreen 17
#define PacketBlinkRed 18

#define PacketStop 19

#define PacketSetData 20

#define PacketConfig2 21

#define BUSY_NONE 0
#define BUSY_HOME 1
#define BUSY_FIX 2
#define BUSY_HOME_STEPPER 3

#define ERROR_NONE 0
#define ERROR_PACKET_TOO_SHORT 1
#define ERROR_X_INVALID 2
#define ERROR_Y_INVALID 3
#define ERROR_INVALID_MAGIC 4
#define ERROR_BUFFER_OVERFLOW 5
#define ERROR_UNKNOWN_PACKET 6
#define ERROR_NOT_RUNNING_BUSY 7
#define ERROR_INVALID_CONFIG_VALUE 8
#define ERROR_INVALID_HEIGHT 9
#define ERROR_INVALID_VALUE 10
#define ERROR_NOT_ALLOWED_TO_READ 11
#define ERROR_PACKET_SIZE_BUFFER_OVERFLOW 12
#define ERROR_MCP_FAULT_1 13
#define ERROR_MCP_FAULT_2 14
#define ERROR_MCP_FAULT_3 15
#define ERROR_MCP_FAULT_4 16
#define ERROR_MCP_FAULT_5 17
#define ERROR_MCP_FAULT_6 18
#define ERROR_MCP_FAULT_7 19
#define ERROR_MCP_FAULT_8 20
#define ERROR_INTERNAL 255

#endif
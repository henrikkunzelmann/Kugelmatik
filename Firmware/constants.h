#ifndef CONSTANTS_H_
#define CONSTANTS_H_


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

#define BUSY_NONE 0
#define BUSY_HOME 1
#define BUSY_FIX 2
#define BUSY_HOME_STEPPER 3


#endif /* CONSTANTS_H_ */
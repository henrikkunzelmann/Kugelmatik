#pragma once

#define ENABLE_SERIAL 0
#define SERIAL_BAUDRATE 1200

#if ENABLE_SERIAL
#define serialBegin() do { Serial.begin(SERIAL_BAUDRATE); } while (0)
#define serialPrint(val) do { Serial.print(val); } while (0)
#define serialPrintln(val) do { Serial.println(val); } while (0)
#define serialPrintF(val) do { Serial.print(F(val)); } while (0)
#define serialPrintlnF(val) do { Serial.println(F(val)); } while (0)
#else
#define serialBegin() do { } while (0)
#define serialPrint(val) do { } while (0)
#define serialPrintln(val) do { } while (0)
#define serialPrintF(val) do { } while (0)
#define serialPrintlnF(val) do { } while (0)
#endif
#pragma once

#ifdef ARDUINO_AVR_NET_IO
#include <avr/wdt.h>
#define wdt_yield() do { wdt_reset(); } while (0)
#define disable_wdt() do { wdt_disable(); } while (0)
#else
#define wdt_yield() do { yield(); } while(0)
#define disable_wdt() do {} while(0)
#endif
#pragma once


#ifdef ARDUINO_AVR_NET_IO
#include <avr/wdt.h> 
#define wdt_yield() do { wdt_reset(); } while (0)
#else
#define wdt_yield() do {} while(0)
#endif
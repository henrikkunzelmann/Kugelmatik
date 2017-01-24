#pragma once

#include <Arduino.h>

#include "network.h"
#include "util.h"
#include "constants.h"
#include "stepper.h"

// lässt einen Tick laufen (spricht Stepper und Netzwerk an) und wartet bis tickTime vergangen ist
boolean runTick(uint32_t tickTime, boolean useHalfStep);
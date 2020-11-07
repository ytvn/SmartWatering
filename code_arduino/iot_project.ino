#include <HardwareSerial.h>
#include <lmic.h>
#include <hal/hal.h>
#include <SPI.h>
// #include <Wire.h>
// #include <math.h>

// LoRaWAN NwkSKey, network session key
static const PROGMEM u1_t NWKSKEY[16] = {0x3E, 0x35, 0x50, 0xDC, 0xB4, 0x4E, 0x9A, 0x62, 0xAA, 0x52, 0x61, 0x25, 0x8E, 0x4F, 0x9C, 0x48};
// LoRaWAN AppSKey, application session key
static const u1_t PROGMEM APPSKEY[16] = {0x72, 0x70, 0x22, 0x7A, 0x70, 0x1D, 0x4A, 0xDB, 0x4A, 0x99, 0x02, 0x77, 0x69, 0xFF, 0x6B, 0x05};
// LoRaWAN end-device address (DevAddr)
static const u4_t DEVADDR = {0x26041F99};
// These callbacks are only used in over-the-air activation, so they are
// left empty here (we cannot leave them out completely unless
// DISABLE_JOIN is set in config.h, otherwise the linker will complain).
void os_getArtEui(u1_t *buf) {}
void os_getDevEui(u1_t *buf) {}
void os_getDevKey(u1_t *buf) {}

static osjob_t sendjob;
// Schedule data trasmission in every this many seconds (might become longer due to duty
// cycle limitations).
// we set 10 seconds interval
const unsigned TX_INTERVAL = 10; // Fair Use policy of TTN requires update interval of at least several min. We set update interval here of 1 min for testing

// Pin mapping according to Cytron LoRa Shield RFM
// Pin mapping according to Cytron LoRa Shield RFM
const lmic_pinmap lmic_pins = {
	.nss = 18,
	.rxtx = LMIC_UNUSED_PIN,
	.rst = 14,
	.dio = {26, 35, 34},
};

// downlink
#define LED_1 17
#define LED_2 19

void onEvent(ev_t ev)
{
	Serial.print(os_getTime());
	Serial.print(": ");
	switch (ev)
	{
	case EV_TXCOMPLETE:
		Serial.printf("EV_TXCOMPLETE (includes waiting for RX windows)\r\n");
		// Schedule next transmission
		os_setTimedCallback(&sendjob, os_getTime() + sec2osticks(TX_INTERVAL), do_send);
		break;
	case EV_RXCOMPLETE:
		//------ Added ----------------
		if (LMIC.dataLen == 1)
		{
			uint8_t result = LMIC.frame[LMIC.dataBeg + 0];
			if (result == 0)
			{
				Serial.println("RESULT 0");
				digitalWrite(LED_1, LOW);
				digitalWrite(LED_2, LOW);
			}
			if (result == 1)
			{
				Serial.println("RESULT 1");
				digitalWrite(LED_1, HIGH);
				digitalWrite(LED_2, LOW);
			}
			if (result == 2)
			{
				Serial.println("RESULT 2");
				digitalWrite(LED_1, LOW);
				digitalWrite(LED_2, HIGH);
			}
			if (result == 3)
			{
				Serial.println("RESULT 3");
				digitalWrite(LED_1, HIGH);
				digitalWrite(LED_2, HIGH);
			}
		}
		Serial.println();

		break;
	default:
		Serial.printf("Unknown event\r\n");
		break;
	}
}

void do_send(osjob_t *j)
{

	// Check if there is not a current TX/RX job running
	if (LMIC.opmode & OP_TXRXPEND)
	{
		Serial.printf("OP_TXRXPEND, not sending\r\n");
	}
	else if (!(LMIC.opmode & OP_TXRXPEND))
	{

		// byte dữ liệu
		// coi cấu hình call back  ttn
		uint8_t buff[6];
		buff[0] = 0; //getdistance();
		buff[1] = 1; //getdistance_servo();
		buff[2] = 2; //getdistance();
		buff[3] = 3;
		buff[4] = 4; //getdistance();
		buff[5] = 5;

		// Prepare upstream data transmission at the next possible time.
		LMIC_setTxData2(1, buff, sizeof(buff), 0);
		Serial.printf("Packet queued\r\n");
	}
	// Next TX is scheduled after TX_COMPLETE event.
}

void setup()
{
	Serial.begin(115200);
	Serial.printf("Starting...\r\n");

	delay(1000); // delay 1s

	// LMIC init
	os_init();
	// Reset the MAC state. Session and pending data transfers will be discarded.
	LMIC_reset();
	// Set static session parameters. Instead of dynamically establishing a session
	// by joining the network, precomputed session parameters are be provided.
	uint8_t appskey[sizeof(APPSKEY)];
	uint8_t nwkskey[sizeof(NWKSKEY)];
	memcpy_P(appskey, APPSKEY, sizeof(APPSKEY));
	memcpy_P(nwkskey, NWKSKEY, sizeof(NWKSKEY));
	LMIC_setSession(0x1, DEVADDR, nwkskey, appskey);

	// Disable ADR
	LMIC_setAdrMode(false);

	// Disable channel 1-8
	for (uint8_t i = 1; i < 9; i++)
	{
		LMIC_disableChannel(i);
	}
#if defined(CFG_eu868)
	// Set up the channels used by the Things Network, which corresponds
	// to the defaults of most gateways. Without this, only three base
	// channels from the LoRaWAN specification are used, which certainly
	// works, so it is good for debugging, but can overload those
	// frequencies, so be sure to configure the full frequency range of
	// your network here (unless your network autoconfigures them).
	// Setting up channels should happen after LMIC_setSession, as that
	// configures the minimal channel set.
	LMIC_setupChannel(0, 433175000, DR_RANGE_MAP(DR_SF9, DR_SF9), BAND_CENTI); // g-band
/*LMIC_setupChannel(1, 433375000, DR_RANGE_MAP(DR_SF12, DR_SF7B), BAND_CENTI);      // g-band
    LMIC_setupChannel(2, 433575000,   DR_RANGE_MAP(DR_SF12, DR_SF7),  BAND_CENTI);      // g-band
    LMIC_setupChannel(3,433775000, DR_RANGE_MAP(DR_SF12, DR_SF7),  BAND_CENTI);      // g-band
    LMIC_setupChannel(4,433975000, DR_RANGE_MAP(DR_SF12, DR_SF7),  BAND_CENTI);      // g-band
    LMIC_setupChannel(5,  434175000,  DR_RANGE_MAP(DR_SF12, DR_SF7),  BAND_CENTI);      // g-band
    LMIC_setupChannel(6, 434375000,  DR_RANGE_MAP(DR_SF12, DR_SF7),  BAND_CENTI);      // g-band
    LMIC_setupChannel(7, 434575000,  DR_RANGE_MAP(DR_SF12, DR_SF7),  BAND_CENTI);      // g-band
    LMIC_setupChannel(8,   434775000 ,  DR_RANGE_MAP(DR_FSK,  DR_FSK),  BAND_MILLI);      // g2-band*/
// TTN defines an additional channel at 869.525Mhz using SF9 for class B
// devices' ping slots. LMIC does not have an easy way to define set this
// frequency and support for class B is spotty and untested, so this
// frequency is not configured here.
#elif defined(CFG_us915)
	// NA-US channels 0-71 are configured automatically
	// but only one group of 8 should (a subband) should be active
	// TTN recommends the second sub band, 1 in a zero based count.
	// https://github.com/TheThingsNetwork/gateway-conf/blob/master/US-global_conf.json
	LMIC_selectSubBand(1);
#endif

	// Disable link check validation
	LMIC_setLinkCheckMode(0);

	// TTN uses SF9 for its RX2 window.
	LMIC.dn2Dr = DR_SF9;

	// Set data rate and transmit power for uplink
	LMIC_setDrTxpow(DR_SF9, 14);
	Serial.printf("LMIC setup done!\r\n");
	// Start job
	do_send(&sendjob);
}

void loop()
{

	os_runloop_once();

#ifdef SEND_BY_BUTTON
	if (digitalRead(0) == LOW)
	{
		while (digitalRead(0) == LOW)
			;
		do_send(&sendjob);
	}
#endif
}

// #include <Wire.h>
// #include <math.h>
#include <HardwareSerial.h>
#include <lmic.h>
#include <hal/hal.h>
#include <SPI.h>
#include <DHT.h>
#include <WiFi.h>

const char *ssid = "DESKTOP-EKAHPDC 1971";
const char *password = "11223344";

#define DHTPIN 2
#define DHTTYPE DHT11
DHT dht(DHTPIN, DHTTYPE);
const int trigPin = 16;
const int echoPin = 17;
const int MoisturePin = 13;
const int WaterPumpPin = 21;
const int buzzPin = 15;

long duration;
int distance, oldDistance;

//Triggers
float temMin, temMax = 40, humMin = 30, humMax, moiMin = 30, moiMax;

//Sensor value
float HumidityValue = 0, TemperatureValue = 0, MoistureValue = 0, WaterLevelValue = 0;

//VariableValues ID
const int TemperatureId = 3;
const int HumidityId = 4;
const int MoistureId = 5;
const int WaterLevelId = 6;

// String DataToSend;
int Interval = 2000;
int priority = 0;
const uint16_t port = 13000;
const char *host = "192.168.137.32";
WiFiClient client;

void init()
{
	Serial.println("Connecting to..");
	WiFi.begin(ssid, password);
	while (WiFi.status() != WL_CONNECTED)
	{
		delay(500);
		Serial.print(".");
	}
	Serial.println("Connected to the Wifi network");

	dht.begin();
	pinMode(MoisturePin, INPUT);
	pinMode(trigPin, OUTPUT); // Sets the trigPin as an Output
	pinMode(echoPin, INPUT);  // Sets the echoPin as an Input
	pinMode(WaterPumpPin, OUTPUT);
	pinMode(buzzPin, OUTPUT);
	digitalWrite(WaterPumpPin, HIGH); // vì chân D3 nối với relay mà relay tích cực thấp nên mặc định chân D3 phải tích cực cao để ngắt relay

}

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
		readMoistureSensor();
		readDHTSensor();
		getDistance();
		// byte dữ liệu
		// coi cấu hình call back  ttn
		uint8_t buff_tem[2];
		buff_tem[0] = TemperatureId;
		buff_tem[1] = TemperatureValue;
		LMIC_setTxData2(1, buff_tem, sizeof(buff_tem), 0);
		Serial.print("TemperatureValue: ");
		Serial.print(TemperatureValue);
		Serial.printf(" Packet tem queued\r\n");

		uint8_t buff_hum[2];
		buff_hum[0] = HumidityId;
		buff_hum[1] = HumidityValue;
		LMIC_setTxData2(1, buff_hum, sizeof(buff_hum), 0);
		Serial.print("HumidityValue: ");
		Serial.print(HumidityValue);
		Serial.printf(" Packet hum queued\r\n");

		uint8_t buff_moi[2];
		buff_moi[0] = MoistureId;
		buff_moi[1] = MoistureValue;
		LMIC_setTxData2(1, buff_moi, sizeof(buff_moi), 0);
		Serial.print("MoistureValue: ");
		Serial.print(MoistureValue);
		Serial.printf(" Packet moi queued\r\n");

		uint8_t buff_lv[2];
		buff_lv[0] = WaterLevelId;
		buff_lv[1] = WaterLevelValue;
		LMIC_setTxData2(1, buff_lv, sizeof(buff_lv), 0);
		Serial.print("WaterLevelValue: ");
		Serial.print(WaterLevelValue);
		Serial.printf(" Packet level queued\r\n\n");
	}
	// Next TX is scheduled after TX_COMPLETE event.
}

void setup()
{
	Serial.begin(115200);
	Serial.printf("Starting...\r\n");
	init();
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

	if (WiFi.status() == WL_CONNECTED)
	{
		socketConnection();
	}
}

void socketConnection()
{
	// Use WiFiClient class to create TCP connections
	if (!client.connected())
	{
		Serial.println("connection failed");
		client.connect(host, port);
		client.print(getChipId());
		delay(5000);
	}
	String result = "";
	while (client.available())
	{
		char ch = static_cast<char>(client.read());
		result += ch;
	}
	if (result != "")
	{
		Serial.println(result);
		//    handle(result);
	}
}

void readDHTSensor()
{
	HumidityValue = dht.readHumidity();
	TemperatureValue = dht.readTemperature();
	if (isnan(HumidityValue) || isnan(TemperatureValue))
	{
		Serial.println("fail to read sensor");
		HumidityValue = 0;
		TemperatureValue = 0;
	}
}

void getDistance()
{
	//// Clears the trigPin
	digitalWrite(trigPin, LOW);
	delayMicroseconds(2);

	// Sets the trigPin on HIGH state for 10 micro seconds
	digitalWrite(trigPin, HIGH);
	delayMicroseconds(10);
	digitalWrite(trigPin, LOW);

	// Reads the echoPin, returns the sound wave travel time in microseconds
	duration = pulseIn(echoPin, HIGH);

	// Calculating the distance
	distance = duration * 0.034 / 2;

	WaterLevelValue = distance;
}

void readMoistureSensor()
{
	MoistureValue = analogRead(MoisturePin);
	MoistureValue = (100 - ((MoistureValue / 4095.00) * 100)); // Convert to moisture percentage
}

int getChipId()
{
	uint32_t id = 0;
	for (int i = 0; i < 17; i = i + 8)
	{
		id |= ((ESP.getEfuseMac() >> (40 - i)) & 0xff) << i;
	}
	return (int)id;
}

void trigger()
{
	if (priority >= 1)
		return;
	if (MoistureValue < moiMin || TemperatureValue > temMax)
	{
		digitalWrite(WaterPumpPin, LOW); // turn the LED on (HIGH is the voltage level)
		delay(Interval);
		digitalWrite(WaterPumpPin, HIGH);
	}
	else
	{
		digitalWrite(WaterPumpPin, HIGH);
	}
}

void handle(String data)
{
	char AB[50];
	data.toCharArray(AB, 50);
	String command = strtok(AB, ":");
	String value[10];
	int i = 0;
	while (command != "")
	{
		value[i] = command;
		data = data.substring(command.length() + 1);
		data.toCharArray(AB, 50);
		command = strtok(AB, ":");
		i++;
	}

	if (value[0] == "T") //T:1:30:50
	{
		char tem[8];
		if (value[1] == String(TemperatureId))
		{
			value[2].toCharArray(tem, value[2].length() + 1);
			temMin = atof(tem);
			value[3].toCharArray(tem, value[3].length() + 1);
			temMax = atof(tem);
		}
		else if (value[1] == String(HumidityId))
		{
			value[2].toCharArray(tem, value[2].length() + 1);
			humMin = atof(tem);
			value[3].toCharArray(tem, value[3].length() + 1);
			humMax = atof(tem);
		}
		else if (value[1] == String(MoistureId))
		{
			value[2].toCharArray(tem, value[2].length() + 1);
			moiMin = atof(tem);
			value[3].toCharArray(tem, value[3].length() + 1);
			moiMax = atof(tem);
		}
	}
	else if (value[0] == "S") // S:1:1
	{
		if (value[1] == "D2")
		{
			digitalWrite(WaterPumpPin, LOW);
			delay(5000);
			digitalWrite(WaterPumpPin, HIGH);
		}
		else if (value[1] == "D1")
		{
			digitalWrite(buzzPin, HIGH);
			delay(5000);
			digitalWrite(buzzPin, LOW);
		}
	}
	else if (value[0] == "C") // S:1:1
	{
		if (value[1] == WaterPumpPin)
		{
			if (value[2] == "1")
			{
				priority = 2;
				digitalWrite(WaterPumpPin, LOW);
			}
			else
			{
				priority = 0;
				digitalWrite(WaterPumpPin, HIGH);
			}
		}
		else if (value[1] == "D1")
		{
			if (value[2] == "1")
			{
				priority = 2;
				digitalWrite(buzzPin, HIGH);
			}
			else
			{
				priority = 0;
				digitalWrite(buzzPin, LOW);
			}
		}
	}
}
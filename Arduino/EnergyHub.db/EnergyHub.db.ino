#include <SPI.h>
#include <WiFiNINA.h>

#include "arduino_secrets.h"
#include "arduino_secrets.h" 
char ssid[] = SECRET_SSID;
char pass[] = SECRET_PASS;
int status = WL_IDLE_STATUS;

void setup() {
  Serial.begin(9600);
  while (!Serial) {
    ; // wait for serial port to connect. Needed for native USB port only
  }

  Serial.print("WL_CONNECTED: ");
  Serial.println(WL_CONNECTED);


  while (status != WL_CONNECTED) {
    status = WiFi.begin(ssid, pass);
    Serial.println("Connecting...");
    Serial.print("Status: ");
    Serial.println(status);
    delay(5000);
  }

  Serial.println("Connected!!");
}

void loop(){

}
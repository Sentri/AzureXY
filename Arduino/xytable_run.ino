#include <SPI.h>
#include <WiFi.h>
#include <Stepper.h>
#include <XYTable.h>


/***********************
 *  CONFIG
 ***********************/
// WIFI SSID and password
char ssid[] = "Sentri S5 WiFi";
char pass[] = "dznv3884";
// Server information
char server[] = "azurexy.azurewebsites.net";
char getLine[] = "GET /Queue/Fetch/?id=2&token=ZZy9tA4oVUmT9zzH HTTP/1.1";
char hostLine[] = "Host: azurexy.azurewebsites.net";
/**********************
 * END OF CONFIG
 **********************/

XYTable table;
int status = WL_IDLE_STATUS;     // the Wifi radio's status
WiFiClient client;
unsigned long lastConnectionTime = 0;
const unsigned long postingInterval = 10L * 1000L;
String incoming = "";
int readState = 0;

void connect_wifi() {
  status = WiFi.begin(ssid, pass);
  if ( status != WL_CONNECTED) { 
    while(true);
  }
}

void disconnect_wifi() {
  WiFi.disconnect();
}

void httpRequest() {
  // close any connection before send a new request.
  // This will free the socket on the WiFi shield
  client.stop();

  // if there's a successful connection:
  if (client.connect(server, 80)) {
    // send the HTTP PUT request:
    client.println(getLine);
    client.println(hostLine);
    client.println("User-Agent: ArduinoWiFi/1.1");
    client.println("Connection: close");
    client.println();

    // note the time that the connection was made:
    lastConnectionTime = millis();
    incoming = "";
    readState = 0;
  } else {
    // if you couldn't make a connection
  }
}

void parse_input() {
  while (client.available()) {
    char c = client.read();
    if (c == 13) {
      // do nothing for CR
    } else if (c == 10) {
      // line feed
      parse_line();
      incoming = "";
    } else {
      incoming.concat(c);
    }
  }
}

void parse_line() {
  Serial.print("PARSE:");
  Serial.println(incoming);
  if (readState == 0) {
    if (incoming.startsWith("XYB10")) {
      readState = 1;
      table.init();
    } 
  } else if (readState == 1) {
    if (incoming.startsWith("M")) {
      int xx = incoming.substring(1,4).toInt();
      int yy = incoming.substring(4,7).toInt();
      table.move(xx,yy);
    } else if (incoming.startsWith("D")) {
      table.pen_down();
    } else if (incoming.startsWith("U")) {
      table.pen_up();
    } else if (incoming.startsWith("E")) {
      readState = 0;
    }
  }
  
}

void setup() {
  Serial.begin(9600);
  connect_wifi();
  table.init();
}

void loop() {
  parse_input();
  if (millis() - lastConnectionTime > postingInterval) {
    httpRequest();
  }
}

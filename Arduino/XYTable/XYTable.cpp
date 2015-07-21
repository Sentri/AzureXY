#include "XYTable.h"

XYTable::XYTable() : Horizontal(72, motor1Pin1, motor1Pin2, motor1Pin3, motor1Pin4),
	Vertical(72, motor2Pin1, motor2Pin2, motor2Pin3, motor2Pin4)
{
	
	// enable motor power and pen pins
	pinMode(motorEnablePin, OUTPUT);
	pinMode(penMotorPin1, OUTPUT);
	pinMode(penMotorPin2, OUTPUT);
	pinMode(motorSwitchPin, INPUT);
	
	// set motor speeds
	Horizontal.setSpeed(50);
	Vertical.setSpeed(250);
}

void XYTable::move(int xPos, int yPos) {
  // check outside bounds
  if (xPos < 0 || yPos < 0 || xPos > xMax || yPos > yMax) {
    return;
  } else {
    float yF = float(yPos);
    int yScaled = int(yF * 3.92);
    move_to(xPos, yScaled);
  }
}

void XYTable::move_to(int x, int y) {
  motors_on();
  xPoint = float(horizontalPosition);
  yPoint = float(verticalPosition);
  float xTarget = float(x);
  float yTarget = float(y);
  float xDelta = xTarget - xPoint;
  float yDelta = yTarget - yPoint;
  int steps = int(max(abs(xDelta), abs(yDelta)));
  if (steps < 1) {
    return;
  }
  float xD = xDelta / float(steps);
  float yD = yDelta / float(steps);
  int i = 0;
  for (i = 0; i < steps; i++) {
    xPoint += xD;
    yPoint += yD;
    jump_to(int(xPoint), int(yPoint));
  }
  motors_off();
}

void XYTable::jump_to(int x, int y) {
  int a = x - horizontalPosition;
  if (a != 0) {
    horizontal_move(a);
  }
  a = y - verticalPosition;
  if (a != 0) {
    vertical_move(a);
  }
}

void XYTable::motors_on() {
  digitalWrite(motorEnablePin, HIGH);
  delay(20);
}

void XYTable::motors_off() {
  digitalWrite(motorEnablePin, LOW);
  delay(20);
}

void XYTable::pen_down() {
  if (penDown == 1) {
    return;
  }
  motors_on();
  digitalWrite(penMotorPin1, HIGH);
  digitalWrite(penMotorPin2, LOW);
  delay(600);
  digitalWrite(penMotorPin1, LOW);
  motors_off();
  penDown = 1;
}

void XYTable::pen_up() {
  if (penDown == 0) {
    return;
  }
  motors_on();
  digitalWrite(penMotorPin1, LOW);
  digitalWrite(penMotorPin2, HIGH);
  delay(500);
  digitalWrite(penMotorPin2, LOW);
  motors_off();
  penDown = 0;
}

void XYTable::vertical_move(int steps) {
  Vertical.step(steps);
  verticalPosition += steps;
}

void XYTable::horizontal_move(int steps) {
  Horizontal.step(steps);
  horizontalPosition += steps;
}

void XYTable::init() {
  pen_up();
  motors_on();
  initialize_vertical();
  initialize_horizontal();
  initialize_pen();
  motors_off();
}

void XYTable::initialize_pen() {
  digitalWrite(penMotorPin1, HIGH);
  digitalWrite(penMotorPin2, LOW);
  delay(2000);
  digitalWrite(penMotorPin1, LOW);
  digitalWrite(penMotorPin2, HIGH);
  delay(500);
  digitalWrite(penMotorPin2, LOW);  
  penDown = 0;
}

void XYTable::initialize_vertical() {
  int val = HIGH;
  do {
    Vertical.step(-5);
    val = digitalRead(motorSwitchPin);
  } while (val == HIGH);
  do {
    Vertical.step(1);
    val = digitalRead(motorSwitchPin);
  } while (val == LOW);
  verticalPosition = 0;
  yPoint = 0.0;
}

void XYTable::initialize_horizontal() {
  int val = HIGH;
  do {
    Horizontal.step(-5);
    val = digitalRead(motorSwitchPin);
  } while (val == HIGH);
  do {
    Horizontal.step(1);
    val = digitalRead(motorSwitchPin);
  } while (val == LOW);
  horizontalPosition = 0;
  xPoint = 0.0;
}
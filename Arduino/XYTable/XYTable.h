#ifndef XYTable_h
#define XYTable_h

#include "Arduino.h"
#include "Stepper.h"

// Horizontal = motor 1
#define motor1Pin1 14
#define motor1Pin2 15
#define motor1Pin3 16
#define motor1Pin4 17 

// Vertical = motor 2
#define motor2Pin1 2
#define motor2Pin2 3
#define motor2Pin3 5
#define motor2Pin4 6

#define motorEnablePin 8
#define motorSwitchPin 9

#define penMotorPin1 18
#define penMotorPin2 19

#define xMax 640
#define yMax 720

class XYTable
{
	public:
		XYTable();
		void init();
		void move(int xPos, int yPos);
		void pen_down();
		void pen_up();
	
	private:
		void move_to(int x, int y);
		void jump_to(int x, int y);
		void motors_on();
		void motors_off();
		void vertical_move(int steps);
		void horizontal_move(int steps);
		void initialize_pen();
		void initialize_vertical();
		void initialize_horizontal();
		
		int horizontalPosition = 0;
		int verticalPosition = 0;
		float xPoint = 0.0;
		float yPoint = 0.0;
		int penDown = 1;
		Stepper Horizontal;
		Stepper Vertical;
};

#endif
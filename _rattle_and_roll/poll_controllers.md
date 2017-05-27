---
layout: baremetal-page
title: Subroutine PollControllers
---

Address $D279 is the start of a subroutine that polls the current state of the two controllers.

The controller state is polled from the hardware by writing a 1 to $4016 (to begin reading) followed by writing a 0 to $4016 (to freeze the current reading). Then, the individual buttons are read from $4016 and $4017, shifted into the C register, and then shifted into scratch RAM at $04/$05.

Each player's controller is stored in two bytes in RAM. Bytes $16/$17 store the current buttons that are being held down by player 1/2. Bytes $18/19 store the buttons that have recently been pressed by player 1/2. This is calculated as current state XOR previous state (the buttons that have changed) AND current state (the buttons that have changed, and are currently held). Some game logic needs to be able to tell the difference; for example, pressing and holding A will only make a snake jump once, and A must be released before the snake may jump again.


	PollControllers
	D279	LDX #$01 	; Poll the hardware
		STX $4016
		DEX
		STX $4016
		LDX #$08	; Eight buttons
	loop	LDA $4016	; Read/store button from controller one
		ROR A
		ROL $04
		LDA $4017	; Read/store buttons from controller two
		ROR A
		ROL $05
		DEX
		BNE loop	; Loop for eight buttons
		LDA $04 	; Compute pressed/held state of controller one
		TAX
		EOR $16
		STX $16
		AND $04
		STA $18
		LDA $05 	; Compute pressed/held state of controller two
		TAX
		EOR $17
		STX $17
		AND $05
		STA $19
		RTS
	D2AA

The bits of each register designate the following buttons:

	7   6   5   4  | 3   2   1   0
	A   B   SEL ST | U   D   L   R

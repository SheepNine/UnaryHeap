---
layout: baremetal-page
title: RunLengthDecoder subroutine
---

The map data for any non-snake-mountain map is stored in CHR RAM in a run-length encoded format.

There is a code fragment that is run whenever a particular map needs to be decoded:

	C5DB	LDX #$18
		JSR ChrRomBlit
		JSR $0700
	C5E3

CHR ROM blit 18 transfers CHR ROM page 3, starting at 0F53 to CHR ROM page 4, ending at 0052, to RAM at 0700-07FF. The run-length decoder subroutine is then available at address $0700. The subroutine determines which map data to decode from the values of $AA (the current level, for bonus maps) or $C5 (the current pond). The data is decoded and stored in $0200-$02FF.

	0700	LDA #$10
		STA PPU_CTRL

		LDA #$06	; Init CHR ROM to page 6
		JSR SetChrBank0

		LDA $C5 	; If in a bonus pond, load pond + 3
		BEQ notpond
		CLC
		ADC #$03
		BNE always
	notpond	LDA $AA 	; Otherwise, load current level

	always	ASL A
		STA $8F
		TAY
		
		LDA $075B,Y	; Point PPU at RLE data start address
		STA PPU_ADDR
		LDA $075C,Y
		STA PPU_ADDR
		
		LDA #$02
		STA $C7
		
		LDA PPU_DATA	; Prime the pump
		LDX #$00	; Init bytes written

	nextrun	LDA PPU_DATA	; Read run value, write first run value
		STA $0200,X
		LDA PPU_DATA	; Read run length, store
		STA $04
		LDA $0200,X	; Prime A with run value
	more	INX
		BEQ done	; Quit if 256 bytes written
		STA $0200,X	; Write next run value
		DEC $04 	; Decrement run length remaining
		BNE more	; Either contine with current run,
		BEQ nextrun	;   or proceed with next run
	done	RTS
	0749

The table starting at 075B gives the starting address of each run-length encoded map:
	
	Y	075B,Y	075C,Y
	00	01	DA
	02	02	2A
	04	02	9E
	06	03	0E
	08	06	78
	0A	06	FE
	0C	07	74
	0E	07	DC
	10	07	DC

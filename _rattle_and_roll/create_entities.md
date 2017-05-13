---
layout: baremetal-page
title: Subroutine CreateEntities
---

After ChrRomBlit(5A) is run, a subroutine has been copied to RAM starting at address $0200 hat is used to initialize the parameters for the entities of the current stage. 

	0200	LDA #$06	; Set Chr Rom Page 6
		JSR SetChrBank0

		CLC		; Set up list index in Y
		LDA $C5
		BEQ no_add
		ADC #$03
	no_add	ADC $AA
		ASL A
		TAY

		LDA PPU_STATUS	; Reset VBlank bit

		LDA $0255,Y	; Load address from table
		STA PPU_ADDR
		LDA $0254,Y
		STA PPU_ADDR
		LDA $0256,Y	; Load $AC and X with byte count of entity data
		SEC
		SBC $0254,Y
		STA $AC
		TAX

		LDY #$00	; Load $AB with 0
		STY $AB

		LDA PPU_DATA	; Prime the pump

	loop	LDA PPU_DATA	; Load four bytes from CHR ROM into RAM
		STA $0653,Y
		LDA PPU_DATA
		STA $0654,Y
		LDA PPU_DATA
		STA $0655,Y
		LDA PPU_DATA
		STA $0656,Y
		TYA		; Increment Y by 4
		CLC
		ADC #$04
		TAY
		TXA		; Decrement X by 4, loop if not negative
		SBC #$03	; NB: Carry is clear here, no overflow occurs
		TAX
		BCS loop

	0253	RTS

The addresses of the entity data are located at $0254:

		$0254,X	$0255,X
	00	F6	08	level 1
	02	6D	09	level 2
	04	EB	09	level 3
	06	93	0A	level 4
	08	2D	0B	level 5
	0A	CE	0B	level 6
	0C	76	0C	level 7
	0E	09	0D	level 8
	10	17	0D	level 9
	12	B1	0D	level 10
	14	21	0E	level 11
	16	59	0E	pond 1
	18	8A	0E	pond 2
	1A	C9	0E	pond 3
	1C	08	0F	pond 4
	1E	4E	0F	pond 5
	20	78	0F	???
		77

---
layout: baremetal-page
title: Utility subroutine ChrRomBlit
---

Address $C3D7 is the start of a utility subroutine that is responsible for a block transfer of data from CHR ROM to RAM. It has seventeen different blocks of data that can be transferred; the particular block to be transferred is selected by the value of the X register, which should be set to a multiple of six.

	C3D7	LDA $C410,X	; Load $09 $08 with destination address
		STA $08
		LDA $C411,X
		STA $09
		JSR ResetPpuCtrlAndMask
		LDA $C412,X	; Load CHR ROM page
		JSR SetChrBank0
		LDA PPU_STATUS	; Reset VBlank bit
		LDA $C413,X	; Load ppu_addr with source address
		STA PPU_ADDR
		LDA $C414,X
		STA PPU_ADDR
		LDA $C415,X	; Load block count
		TAX
		LDA PPU_DATA	; Prime the pump
		LDY #$00
	loop	LDA PPU_DATA
		STA ($08),Y
		INY
		BNE loop
		INC $09
		DEX
		BNE loop
		RTS
	C410

Address $C3B7 is a thunk for ChrRomBlit, which calls it with X set to #$5A.

	C3B7	LDX #$5A
		BNE ChrRomBlit

Address $C3BB is a thunk for ChrRomBlit, which calls it with X set to #$06.

	C3BB	LDX #$06
		BNE ChrRomBlit

Address $C410 is the start of the table listing the destination address, source CHR ROM page, source address on that CHR ROM page, and number of 256-byte blocks to transfer.

		Dest	Addr	ChrBnk0	Src	Addr	nBlocks
	X	C410,X	C411,X	C412,X	C413,X	C414,X	C415,X
	00	FF	03	03	03	90	04
	06	00	02	03	02	B0	01
	0C	00	02	03	0D	F0	01
	12	00	02	03	0E	7F	01
	18	00	07	03	0F	53	01
	1E	00	07	03	0B	A0	01
	24	00	07	05	06	70	01
	2A	00	06	05	07	70	01 
	30	53	06	05	08	64	01
	36	53	06	01	0B	60	01
	3C	A0	06	04	0B	60	01
	42	00	07	07	04	C0	01
	48	00	07	04	0C	F0	01
	4E	00	02	05	0C	F0	01
	54	00	02	05	05	60	01
	5A	00	02	02	0F	07	01
	60	00	07	06	08	46	01

## See also

* [SetChrBank0](set_chr_bank_0.html)
* [ResetPpuCtrlAndMask](reset_ppu_ctrl_mask.html)

---
layout: baremetal-page
title: Subroutine BlitBgArrangement / ChrRomBlit(06)
---

After ChrRomBlit(06) is run, a subroutine has been copied to ram starting at address $0200 that copies a collection of bytes defining an 'arrangement' into PPU RAM. The arrangement format starts with two bytes, denoting the upper-left corner of the arrangmenet in PPU RAM, followed by two bytes giving the width and height of the arrangment. It is then followed by width * height bytes giving the data that is copied into PPU RAM.

The PPU address is advanced by #$20 minus the arrangement width after each row of the arrangement is copied, so that it is positioned as a rectangle on the screen.

The method takes as input the Y register, giving the offset from the start of RAM page $0200 where the arrangement is located.

	0200	LDA $0200,Y	; Init PPU_ADDR to upper-left
		STA PPU_ADDR
		INY 
		LDA $0200,Y
		STA PPU_ADDR
		INY 

		LDA $0200,Y	; Init arrangement width/height
		STA $04
		INY 
		LDA $0200,Y
		STA $05
		
	nextrow	LDX $04
	nextcol	INY		; Copy one row
		LDA $0200,Y
		STA PPU_DATA
		DEX 
		BNE nextcol
		
		LDA #$20	; Advance PPU_ADDR to start of next row
		SEC 
		SBC $04
		TAX 
	loop	LDA PPU_DATA
		DEX 
		BNE loop
		
		DEC $05 	; Blit another row if not done
		BNE nextrow
		
		RTS
	0236

ChriRomBlit(06) also copies three arrangements into RAM page 2: one for 'SNAKE' (Y = #$36), used in the main titles, and two for portraits of RATTLE (Y = #$8E) and ROLL (Y = #$B6) used in the main titles, between-level score roll-up and final score screens.

	0236 Background Arrangement (written to 2066):
		87 88 89 8A 87 88 8B 8C 87 88 8B 8C 8D 8E 8F 90 87 88 89 8A 00
		91 92 93 94 95 96 97 98 95 92 99 98 95 9A 9B 9C 95 92 9D 00 00
		9E 9F A0 A1 A2 A3 A4 A1 A5 A6 A7 A1 A5 A6 A8 A9 A2 AA AB AC 00
		AD AE AE AF B0 B1 B2 B3 B0 B1 B2 B3 B0 B1 B2 B3 AD AE AE AF 00

	028E Background Arrangement (written to 2206):
		00 C2 C3 C4 C5 00
		C6 C7 C8 C9 CA CB
		CC CD CE CF D0 D1
		D2 D3 D4 D5 D6 D7
		D8 D9 DA DB 00 00
		00 DC DD DE 00 00

	02B6 Background Arrangement (written to 2215):
		00 DF E0 E1 E2 E3
		E4 E5 E6 E7 E8 E9
		EA EB EC ED EE EF
		00 F0 F1 F2 F3 F4
		00 F5 F6 F7 F8 00
		00 00 F9 FA FB 00

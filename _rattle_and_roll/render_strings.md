---
layout: baremetal-page
title: RenderStrings subroutine
---

Address $80B6 is the start of a subroutine to take a list of strings and render them into the NES PPU nametable.

The method takes the X, Y, and A registers as parameters. The address of the start of the list of strings is stored in X (low byte) and Y (high byte), and the number of strings to render is stored in A.

Strings are formatted as two bytes, specifying the address of the upper-left corner of the string in the PPU nametable. They are followed by the character data.

	80B6	STX $08 	; Store parameters
		STY $09
		LDY #$00
		STA $04
		
	80BE	LDA ($08),Y 	; Read dest. address of upper-left corner of string
		STA $0C
		INY
		
		LDA ($08),Y
		STA $0D
		INY
		
		STY $0F 	; Render top half of string
		LDA #$00
		JSR $E2C9
		
		LDY $0F 	; Render bottom half of string
		LDA #$02
		JSR $E2C9
		
		DEC $04 	; Loop if more strings remain
		BNE $80BE
		
		RTS
	80DB

RenderStrings() uses a utility subroutine, RenderHalfString(), located at $E2C9. It assumes the string data to be stored at ($08),Y and the destination address at $0C/$0D. When called, it will render either the top or bottom half of the current string, depending on whether A is set to 0 or 2. It will also update the destination address so that, when called the second time, it is correct for the bottom half of the string.

	E2C9	STA $0E

		LDA $0C 	; Set PPU_ADDR to destination
		STA PPU_ADDR
		LDA $0D
		STA PPU_ADDR

	E2D5	LDA ($08),Y 	; Read next character
		AND #$7F
		STA $05
		
		SEC		; Convert character code to lookup index
		SBC #$36
		ASL A
		ASL A
		ORA $0E
		TAX
		
		LDA $E306,X 	; Transfer first tile
		STA PPU_DATA
		
		LDA $05 	; If it is 'I', don't transfer two tiles
		CMP #$49
		BEQ $E2F5
		
		LDA $E307,X 	; Transfer second tile
		STA PPU_DATA
		
	E2F5	LDA ($08),Y 	; Check for bit 7 denoting end of string
		INY
		ROL A
		BCC $E2D5 	; Loop if not done
		
		LDA $0D 	; Increment destination address by #$20
		ADC #$1F 	; Carry set here
		STA $0D
		BCC $E305
		INC $0C
	E305	RTS

There are forty characters: space, 0-9, A-Z, '-I', '.', and 'tm'. Characters are numbered from #$36 to #$5D. The last character of a string is denoted with its high bit set.

Address $E306 gives the tile mapping for the characters to be copied into the nametable. It is $A0 bytes long, and lists each character's tiles in UL-UR-LL-LR format.

None of the strings in Snake Rattle 'n' Roll contain a Q or an X; if they were to appear, they would render as W or K instead.

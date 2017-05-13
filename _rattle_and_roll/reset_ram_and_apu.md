---
layout: baremetal-page
title: Subroutine ResetRamAndApu
---

After ChrRomBlit(5A) is run, a subroutine has been copied to RAM starting at address $02C7 that can be used to clear most of the contents of RAM to zeroes. (RAM page 1 is excepted, since clearing it would destroy the subroutine return address, and RAM page 2 is excepted, since clearing it would destroy the code out from under the method as it is being run.) In addition, this method resets the state of the various APU registers.

The method tacitly assumes that X is set to zero when called; this is true immediately after ChrRomBlit.

	02C7	TXA
		LDX #$17	; Zero out APU registers
	loop_a	STA $4000,X
		DEX
		BPL loop_a

		LDA #$C0	; Set bits in select APU registers
		STA $4010
		LDA #$80
		STA $4017
		LDA #$0F
		STA $4015

		LDX #$00	; Memset most of ram to zero
		TXA
	loop_b	STA $00,X
		STA $0300,X
		STA $0400,X
		STA $0500,X
		STA $0600,X
		STA $0700,X
		INX
		BNE loop_b

	02F6	RTS


---
layout: baremetal-page
title: Utility subroutine ResetPpuCtrlAndMask
---

Address $8242 is the start of a utility subroutine that resets the PPU control and PPU mask registers. Specifically, PPU control is set to #$10, and PPU mask is set to #$00. These values are copied into RAM at $00 and $01, respectively, so that other code blocks can load them to sense the current state of these registers.

	8242	LDA #$10
		STA PPU_CTRL
		STA $00
		LDA #$00
		STA PPU_MASK
		STA $01
		RTS
	8251	

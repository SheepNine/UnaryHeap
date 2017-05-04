---
layout: baremetal-page
title: Utility subroutine SetChrBank0
---

Address $FFC9 is the start of a utility subroutine that simply takes the current value in the accumulator and transfers it over the serial link to the MMC Chr Bank 0 register.

	FFC9	STA $BFFF
		LSR A
		STA $BFFF
		LSR A
		STA $BFFF
		LSR A
		STA $BFFF
		LSR A
		STA $BFFF
		RTS

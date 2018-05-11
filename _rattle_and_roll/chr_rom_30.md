---
layout: baremetal-page
title: ChrRomBlit(30) - Main titles
---

Blit $30 copies one page from CHR ROM 5 to RAM starting at $0653, although the actual interesting data is only $9C bytes long: address $0900 in CHR ROM 5 points to tile data. This page is only used during the opening sequence before the game starts.

While this page is in memory, address $0653 is the start of a method that will print strings '1 PLAYER', '2 PLAYERS' and 'PRESS  START' to the screen. It also has all of the strings necessary for the main title screen.

    CHR  RAM
         PRT12PS
    0864 0653    LDX #$C9
                 LDY #$06
                 LDA #$03
                 JMP PRTSTRS

    086D 065C    .DATA  29 88 4E 49 4E 54 45 4E 44 CF                         ; 'NINTENDO'
    0877 0666    .DATA  2A 08 50 52 45 53 45 4E 54 D3                         ; 'PRESENTS'
    0881 0670    .DATA  20 E3 52 41 54 54 4C 45 36 4E 36 52 4F 4C 4C DD       ; 'RATTLE N ROLL(tm)'
    0891 0680    .DATA  21 83 43 4F 50 59 52 49 47 48 54 36 38 40 3F C0       ; 'COPYRIGHT 1989'
    08A1 0690    .DATA  21 E8 52 41 52 45 36 4C 54 44 DC                      ; 'RARE LTD.'
    08AC 069B    .DATA  22 85 4C 49 43 45 4E 53 45 C4                         ; 'LICENSED'
    08B6 06A5    .DATA  22 97 54 CF                                           ; 'TO'
    08BA 06A9    .DATA  22 E5 4E 49 4E 54 45 4E 44 CF                         ; 'NINTENDO'
    08C4 06B3    .DATA  22 F7 42 D9                                           ; 'BY'
    08C8 06B7    .DATA  23 42 52 41 52 45 36 43 4F 49 4E 5B 54 36 49 4E 43 DC ; 'RARE COIN-IT INC.'

    08DA 06C9    .DATA  21 A7 38 36 50 4C 41 59 45 D2                         ; '1_PLAYER'
    08E4 06D3    .DATA  22 27 39 36 50 4C 41 59 45 52 D3                      ; '2_PLAYERS'
    08EF 06DE    .DATA  23 04 50 52 45 53 53 36 36 53 54 41 52 D4             ; 'PRESS  START'
    08FD 06EC    .DATA  A5 73 49                                              ; garbage bytes
    0900 06EF

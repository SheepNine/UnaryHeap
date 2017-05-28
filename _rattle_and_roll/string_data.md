---
layout: baremetal-page
title: String Data
---

String data for scrolling screens (the main title screen, the score roll-up between levels, the game over screen, and the game won screen) is stored in CHR RAM.

Each string is prepended by two bytes, specifying the nametable address of the upper-left corner of the string. Then, the code points for the characters are given. The final character in the string is the character with bit 7 set; this is stripped off the character before rendering.

The strings are listed below. Note that these strings are blitted into RAM using the ChrRomBlit subroutine, and so the addresses listed will not be the addresses by which they are addressed.

# CHR ROM Page 3

		    L  E  V  E  L     0  0
	39A  21 08  4C 45 56 45 4C 36 37 B7

	            0  0  0  0  0  0
	3A4  29 42  37 37 37 37 37 37 B6

	            0  0  0  0  0  0
	3AD  29 52  37 37 37 37 37 37 B6

	            C  O  M  P  L  E  T  E  D
	6FB  21 67  43 4F 4D 50 4C 45 54 45 C4

	            F  I  N  A  L     S  C  O  R  E
	706  21 66  46 49 4E 41 4C 36 53 43 4F 52 C5

	            G  A  M  E     O  V  E  R
	713  20 47  47 41 4D 45 36 4F 56 45 D2

	            F  A  N  T  A  S  T  I  C
	728  20 47  46 41 4E 54 41 53 54 49 C3

	            B  R  I  L  L  I  A  N  T
	733  20 48  42 52 49 4C 4C 49 41 4E D4

	            A  M  A  Z  I  N  G
	73E  20 4A  41 4D 41 5A 49 4E C7

	            T  E  R  R  I  F  I  C
	747  20 49  54 45 52 52 49 46 49 C3

	            M  A  G  I  C
	751  20 4C  4D 41 47 49 C3

	            S  E  N  S  A  T  I  O  N  A  L
	758  20 46  53 45 4E 53 41 54 49 4F 4E 41 CC

	            P  H  E  N  O  M  E  N  A  L
	765  20 46  50 48 45 4E 4F 4D 45 4E 41 CC

	            I  N  C  R  E  D  I  B  L  E
	771  20 47  49 4E 43 52 45 44 49 42 4C C5

	            W  O  W
	77D  20 4D  57 4F D7

	            A  W  E  S  O  M  E
	782  20 49  41 57 45 53 4F 4D C5


# CHR ROM Page 5

	            B  O  N  U  S
	581  21 8B  42 4F 4E 55 D3

	            W  A  R  P     T  O
	588  21 89  57 41 52 50 36 54 CF

	            L  E  V  E  L     3
	591  22 09  4C 45 56 45 4C 36 BA

	            H  I  P  P  E  T  Y     H  I  P
	80A  28 C6  48 49 50 50 45 54 59 36 48 49 D0

		    H  I  P  P  E  T  Y     H  O  P
	817  29 46  48 49 50 50 45 54 59 36 48 4F D0

	            R  A  T  T  L  E     N     R  O  L  L
	824  29 C3  52 41 54 54 4C 45 36 4E 36 52 4F 4C CC

	            A  R  E     A  T     T  H  E     T  O  P
	833  2A 42  41 52 45 36 41 54 36 54 48 45 36 54 4F D0

	            S  E  E     Y  O  U
	843  2B 09  53 45 45 36 59 4F D5

	            S  O  O  N
	84C  2B 8C  53 4F 4F CE

	            S  N  A  K  E  S     I  N     S  P  A  C  E
	852  20 C1  53 4E 41 4B 45 53 36 49 4E 36 53 50 41 43 C5

	            N  I  N  T  E  N  D  O
	86D  29 88  4E 49 4E 54 45 4E 44 CF

	            P  R  E  S  E  N  T  S
	877  2A 08  50 52 45 53 45 4E 54 D3

	            R  A  T  T  L  E     N     R  O  L  L  tm
	881  20 E3  52 41 54 54 4C 45 36 4E 36 52 4F 4C 4C DD

	            C  O  P  Y  R  I  G  H  T     1  9  8  9
	891  21 83  43 4F 50 59 52 49 47 48 54 36 38 40 3F C0

	            R  A  R  E     L  T  D  .
	8A1  21 E8  52 41 52 45 36 4C 54 44 DC

	            L  I  C  E  N  S  E  D
	8AC  22 85  4C 49 43 45 4E 53 45 C4

	            T  O
	8B6  22 97  54 CF

	            N  I  N  T  E  N  D  O
	8BA  22 E5  4E 49 4E 54 45 4E 44 CF

	            B  Y
	8C4  22 F7  42 D9

	            R  A  R  E     C  O  I  N  -I  T    I  N  C  .
	8C8  23 42  52 41 52 45 36 43 4F 49 4E 5B 54 36 49 4E 43 DC

	            1     P  L  A  Y  E  R
	8DA  21 A7  38 36 50 4C 41 59 45 D2

	            2     P  L  A  Y  E  R  S
	8E4  22 27  39 36 50 4C 41 59 45 52 D3

	            P  R  E  S  S        S  T  A  R  T
	8EF  23 04  50 52 45 53 53 36 36 53 54 41 52 D4

# PRG ROM

	             R  A  T  T  L  E
	E3A6  23 22  52 41 54 54 4C C5

	             R  O  L  L
	E3AE  23 34  52 4F 4C CC

	                S  T  A  R  R  I  N  G
	E3B4  21 66  36 53 54 41 52 52 49 4E 47 B6

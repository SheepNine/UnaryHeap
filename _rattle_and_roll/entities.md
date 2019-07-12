---
layout: baremetal-page
title: Entities
---

There are $40 different values for 'entity type'. For each entity type, there is a jump table address located in a table at address $8B0E, and a control byte in a table located at address $8B8E.

The meanings of each entity type are as follows:

|ID|AI addr|Ctrl|?|JumpOn|?|?|TVel|Nmnc|AI|Description|Image|
|01|96F8|24|0|0|1|0|4|RATL|SNEK|Player one snake head|![](../static/images/entities/01 02 SNEK closed.gif)![](../static/images/entities/01 02 SNEK open.gif)![](../static/images/entities/01 02 SNEK lid.gif)![](../static/images/entities/01 02 SNEK ouch.gif)|
|02|96F8|24|0|0|1|0|4|ROLL|    |Player two snake head|![](../static/images/entities/01 02 SNEK misc.gif)|
|05|D302|84|1|0|0|0|4|TAIL|TAIL|Snake tail segment|![](../static/images/entities/05 TAIL.gif)|
|1B|C048|84|1|0|0|0|4|PAIN|PAIN|Snake tail segment floating away|![](../static/images/entities/1B PAIN.gif)|
|31|AFEA|A4|1|0|1|0|4|PBLY|PBLY|Nibbly Pibbly|![](../static/images/entities/31 PBLY.gif)|
|32|AFEA|04|0|0|0|0|4|PJMP|PBLY|Pibbleboing|![](../static/images/entities/32 PJMP.gif)|
|06|AFCA|24|0|0|1|0|4|PJOG|PJOG|Pibblejogger (transmutes into other pibble types on other levels)|![](../static/images/entities/06 PJOG.gif)|
|09|B272|24|0|0|1|0|4|PGOO|PGOO|Pibblesplat|![](../static/images/entities/09 PGOO.gif)|
|0C|AFEA|21|0|0|1|0|1|PBAT|PBLY|Pibblebat|![](../static/images/entities/0C PBAT.gif)|
|30|CC4B|02|0|0|0|0|2|FISH|FISH|Pibblefish, swimming or as part of exit splash|![](../static/images/entities/30 FISH.gif)|
|33|AFEA|04|0|0|0|0|4|HELI|PBLY|Pibblecopter|![](../static/images/entities/33 HELI.gif)|
|29|CB48|82|1|0|0|0|2|SPIT|SPIT|Spit-out pibbley chunk|![](../static/images/entities/29 SPIT.gif)|
|2A|CB87|81|1|0|0|0|1|WING|WING|Spit-out pibblebat wing|![](../static/images/entities/2A WING.gif)|
|21|C1F9|02|0|0|0|0|2|PDSP|PDSP|Nibbly Pibbly dispenser|![](../static/images/entities/21 PDSP 2D BDSP.gif)|
|07|B4A6|A4|1|0|1|0|4|DPBL|DPBL|Nibbly Pibbly being ejected from a dispenser|![](../static/images/entities/07 DPBL.gif)|
|3C|CC2C|84|1|0|0|0|4|PEGG|PEGG|Pibblefish egg|![](../static/images/entities/3C PEGG.gif)|
|2F|CBBD|FF|1|1|1|1|F|WEED|WEED|Pond seaweed|![](../static/images/entities/2F WEED.gif)|
|16|BB5D|24|0|0|1|0|4|BOMB|BOMB|Bomb|![](../static/images/entities/16 BOMB.gif)|
|17|BDAF|04|0|0|0|0|4|BANG|BANG|Explosion (from bomb)|![](../static/images/entities/17 BANG 24 EBNG.gif)|
|24|BDAF|02|0|0|0|0|2|EBNG|BANG|Explosion (from enemy)|![](../static/images/entities/17 BANG 24 EBNG.gif)|
|18|BE37|94|1|0|0|1|4|FLAK|FLAK|Explosion shrapnel|![](../static/images/entities/18 FLAK.gif)|
|1C|C0B5|04|0|0|0|0|4|TIME|DROP|Clock dropped item|![](../static/images/entities/1C TIME.gif)|
|1D|C0B5|02|0|0|0|0|2|DMND|DROP|Diamond dropped item, tail fin|![](../static/images/entities/1D DMND.gif)![](../static/images/entities/1D TFIN.gif)|
|1F|C0B5|02|0|0|0|0|2|RVRS|DROP|Reverse dropped item (not present in data)|![](../static/images/entities/1F RVRS.gif)|
|20|C0B5|02|0|0|0|0|2|FSTR|DROP|Speed Up dropped item|![](../static/images/entities/20 FSTR.gif)|
|1E|C0B5|02|0|0|0|0|2|LIFE|DROP|Extra life dropped item|![](../static/images/entities/1E LIFE.gif)|
|2B|C0B5|03|0|0|0|0|3|DETH|DROP|Fake extra life dropped item|![](../static/images/entities/2B DETH.gif)|
|34|C634|04|0|0|0|0|4|ITEM|ITEM|Bonus item pick-up|![](../static/images/entities/34 ITEM tongue.gif)![](../static/images/entities/1C TIME.gif)![](../static/images/entities/1D DMND.gif)![](../static/images/entities/1E LIFE.gif)![](../static/images/entities/1F RVRS.gif)![](../static/images/entities/20 FSTR.gif)|
|11|B907|32|0|0|1|1|2|JAWS|JAWS|Shark on levels 1 and 2|![](../static/images/entities/11 JAWS.gif)|
|0D|B6E5|00|0|0|0|0|0|DOZR|DOZR|Snakedozer|![](../static/images/entities/0D DOZR.gif)|
|0E|B759|11|0|0|0|1|1|RAZR|RAZR|Bladez|![](../static/images/entities/0E RAZR.gif)|
|14|BA7F|10|0|0|0|1|0|CSHN|CSHN|Pin cushion|![](../static/images/entities/14 CSHN.gif)|
|15|BB14|A4|1|0|1|0|4|PINN|PINN|Ejected cushion pin|![](../static/images/entities/15 PINN.gif)|
|2D|AE8C|01|0|0|0|0|1|BDSP|AE8C|Bell dispenser|![](../static/images/entities/21 PDSP 2D BDSP.gif)|
|19|C78B|08|0|0|0|0|8|DBEL|DBEL|Bell being ejected from a dispenser|![](../static/images/entities/19 DBEL 3B BELL.gif)|
|36|C7C7|08|0|0|0|0|8|ANVL|ANVL|Anvilz|![](../static/images/entities/36 ANVL.gif)|
|3A|9F1D|04|0|0|0|0|4|BALL|BALL|Beach ball/Snowball/Asteroid|![](../static/images/entities/3A BALL beachball.gif)![](../static/images/entities/3A BALL snowball.gif)|
|38|B9D5|04|0|0|0|0|4|STRE|STRE|Still Metal Tree|![](../static/images/entities/38 STRE 39 MTRE.gif)|
|13|B9D9|61|0|1|1|0|1|SLDR|SLDR|Krazy Seat/Ice cube|![](../static/images/entities/13 SLDR seat 22 SEAT 2C SPNR.gif)![](../static/images/entities/13 SLDR icecube.gif)|
|22|B986|62|0|1|1|0|2|SEAT|SEAT|Krazy Seat from lid|![](../static/images/entities/13 SLDR seat 22 SEAT 2C SPNR.gif)|
|2C|B9B3|43|0|1|0|0|3|SPNR|SPNR|Krazy seat going in circles|![](../static/images/entities/13 SLDR seat 22 SEAT 2C SPNR.gif)|
|35|C68A|44|0|1|0|0|4|PACR|PACR|Record/Mushroom/Bubble|![](../static/images/entities/35 PACR record.gif)![](../static/images/entities/35 PACR mushroom.gif)![](../static/images/entities/35 PACR bubble.gif)|
|3B|C68A|44|0|1|0|0|4|BELL|PACR|Hopping Bell|![](../static/images/entities/19 DBEL 3B BELL.gif)|
|39|C68A|04|0|0|0|0|4|MTRE|PACR|Mobile metal Tree|![](../static/images/entities/38 STRE 39 MTRE.gif)|
|1A|BE8E|24|0|0|1|0|4|FOOT|FOOT|Big Foot|![](../static/images/entities/1A FOOT warm.gif)![](../static/images/entities/1A FOOT cool.gif)|
|10|B8CE|02|0|0|0|0|2|FLAG|FLAG|Icefoot's flag on level 11|![](../static/images/entities/10 FLAG.gif)|
|0A|B5C0|04|0|0|0|0|4|DOOR|DOOR|Entrance/exit door|![](../static/images/entities/0A DOOR.gif)|
|0B|B65A|21|0|0|1|0|1|SCAL|SCAL|Weigh-scale|![](../static/images/entities/0B SCAL.gif)|
|25|C9B5|02|0|0|0|0|2|CRPT|CRPT|Magic carpet|![](../static/images/entities/25 CRPT.gif)|
|37|9EA1|04|0|0|0|0|4|WJET|WJET|Water Jet|![](../static/images/entities/37 WJET.gif)|
|3D|0700|04|0|0|0|0|4|UFOB|UFOB|UFO (body)|![](../static/images/entities/3D UFOB.gif)|
|3E|077C|04|0|0|0|0|4|UFOC|UFOC|UFO (canopy|![](../static/images/entities/3E UFOC.gif)|
|3F|9E0B|1F|0|0|0|1|F|RCKT|RCKT|Warp rocket to level 8|![](../static/images/entities/3F RCKT.gif)|
|28|CB2A|0F|0|0|0|0|F|SKOR|SKOR|Floating points value|![](../static/images/entities/28 SKOR.gif)|
|08|AF3D|04|0|0|0|0|4|LIDD|LIDD|Flipping lid|![](../static/images/entities/08 LIDD.gif)|
|0F|B88E|11|0|0|0|1|1|RIPL|RIPL|Water splash|![](../static/images/entities/0F RIPL.gif)|
|12|B959|91|1|0|0|1|1|ARGG|ARGG|Letters for ARG|![](../static/images/entities/12 ARGH.gif)|
|2E|CBA6|84|1|0|0|0|4|BUBL|BUBL|Underwarter snake breath|![](../static/images/entities/2E BUBL.gif)|
|00|C8F6|00|0|0|0|0|0|NULL|NULL|No entity. Code is used for non-null entities caught in a black hole|
|23|C858|02|0|0|0|0|2|HOLE|HOLE|Black hole
|26|C1D7|02|0|0|0|0|2|XTRA|XTRA|Bonus stage context
|27|CAEB|02|0|0|0|0|2|TRAP|TRAP|BigFoot motion sensor
|03|96F8|24|0|0|1|0|4|||(Unused)|
|04|96F8|24|0|0|1|0|4|||(Unused)|
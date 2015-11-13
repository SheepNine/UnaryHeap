program Pac_Man_II (Input, Output, MapFile, ScoreFile, ListFile);
uses
  C_Consts, Crt;
const
  SBAR_BORDER = DkGrey;
  SBAR_TEXT = White;
  NOT_WALL = [' ','.','*'];
  WALL = ['W','╨','╥','╡','╞','╚','╔','╗',
          '╝','║','═','╩','╦','╣','╠','╬'];
  STARTING_LIVES = 3;
  QUIT_KEY = [chr(27), 'q', 'Q'];
  PAUSE_KEY = [chr(13), ' '];
  FRAMERATE = 30;
  PAC_SPEED = 10;
  MAPROTATION = 'maps.lst';
  SCORESTOREAD = 'scores.sco';
  SCOREFOR1UP = 30000;
  VOIDHACKEDSCORES = TRUE;
type
  DirType = (UP, DOWN, LEFT, RIGHT);
  CheatType = (AtariMode, GodMode, GlowGhosts, WallFlash,
               DotFlash, FadeMode, DarkMode, SoundEnabled);
  CheatArray = array [AtariMode..SoundEnabled] of boolean;
  MapListArray = array [1..6] of String;
  ModeType = (Standard, Fade, Nightmare, Mob);
  ModeTypeSet = set of ModeType;
  WorldType = (PacMapOnly, Extended);
  CharacterDataType = record
                        X : 1..28;
                        Y : 1..31;
                        Dir : DirType;
                        Caught : boolean;
                      end;
  MapType = array [1..28, 1..31] of char;
  GhostStats = array [1..8] of CharacterDataType;
  WarpType = array [1..2] of CharacterDataType;
  ScoreType = array[1..5] of record
                               Name : String[16];
                               Score : real;
                             end;
var
  MapFile, ScoreFile, ListFile : text;
  PacMap : MapType;
  Range, PowerTime, Level, DotCount, Count,
  DotsInMap, MonCount, Lives, Max_Ghosts : integer;
  Score, OneUpScore, PacWait, GhostWait : real;
  Ghosts, GhostsOrigin : GhostStats;
  PacMan, Origin, Cage : CharacterDataType;
  Warps : WarpType;
  EnteredKey : Char;
  AltDir : DirType;
  Error, ExtendedMaps : boolean;
  Cheats : CheatArray;
  HiScores : ScoreType;
  MapList : MapListArray;
  MapSelected : String;
{----------------------------------------------------------------------}
{- Initialization Procedures ------------------------------------------}
{----------------------------------------------------------------------}
procedure Init_Game (var Lives, Level, Max_Ghosts : integer;
                     var Score, OneUpScore : Real;
                     var Cheats : CheatArray;
                     var HiScores : ScoreType;
                     var MapList : MapListArray);
var 
  Count : integer;
  ScoreKey : real;
begin
  Lives := STARTING_LIVES;
  Level := 0;
  Cheats[AtariMode] := TRUE;
  Cheats[GodMode] := FALSE;
  Cheats[GlowGhosts] := FALSE;
  Cheats[WallFlash] := FALSE;
  Cheats[DotFlash] := FALSE;
  Cheats[FadeMode] := FALSE;
  Cheats[DarkMode] := FALSE;
  Cheats[SoundEnabled] := FALSE;
  Max_Ghosts := 4;
  Score := 0;
  OneUpScore := SCOREFOR1UP;
  Assign (ScoreFile, SCORESTOREAD);
  Reset (ScoreFile);
  for Count := 1 to 5 do
  begin
    ReadLn (ScoreFile, HiScores[Count].Name);
    ReadLn (ScoreFile, HiScores[Count].Score);
  end;
  ReadLn (ScoreFile, ScoreKey);
  Close (ScoreFile);
  if (trunc(ScoreKey) <> ((trunc(HiScores[5].Score) mod 97) * 100000000)
                       + ((trunc(HiScores[2].Score) mod 13) * 1000000)
                       + ((trunc(HiScores[4].Score) mod 31) * 10000)
                       + ((trunc(HiScores[1].Score) mod 17) * 100)
                       + (trunc(HiScores[3].Score) mod 47))
      and VOIDHACKEDSCORES
  then begin
    HiScores[1].Name := 'HACKING';
    HiScores[2].Name := 'SCORE';
    HiScores[3].Name := 'FILE';
    HiScores[4].Name := 'NOT';
    HiScores[5].Name := 'ALLOWED!';
    for Count := 1 to 5 do
      HiScores[Count].Score := 100 * (6 - Count);
  end;
  Assign (ListFile, MAPROTATION);
  Reset (ListFile);
  for Count := 1 to 6 do
    ReadLn (ListFile, Maplist[Count]);
  Close (ListFile);
end; {Init_Game}
{----------------------------------------------------------------------}
procedure Clear_Map (var PacMap : MapType);
var
  X,Y : Integer;
begin
  for X := 1 to 28 do
    for Y := 1 to 31 do
      PacMap[X,Y] := ' ';
end; {Clear_Map}
{----------------------------------------------------------------------}
procedure Read_Map (var PacMap : MapType;
                    var Ghosts, GhostsOrigin : GhostStats;
                    var PacMan, Origin, Cage : CharacterDataType;
                    var Warps : WarpType;
                    var DotsInMap, MonCount : integer;
                    var Error : boolean;
                    Max_Ghosts : integer;
                    MapSelected : String);
var
  X, Y, X1, Y1, WarpCount : integer;
  Block : set of DirType;
  Scan : array [-2..2, -2..2] of char;
  ModMap : MapType;
  {- - - - - - - - - -}
  procedure Scan_Map (X,Y : integer; 
                      var ModMap : MapType;
                      var Error : Boolean);
  begin
    if ((X = 1) or (X = 28) or (Y = 1) or (Y = 31))
        and not Error and (ModMap[X,Y] <> 'X')
    then begin
      ClrScr;
      WriteLn ('Error 1 : Map not closed at (',X:2,':',Y:2,')');
      ReadLn;
      Error := TRUE;
    end;
    if (ModMap[X,Y] = 'X')
    then if (X <> 1) and (X <> 28) and (Y <> 1) and (Y <> 31)
	       and not Error
      then begin
        ClrScr;
        WriteLn ('Error: Warp not on edge of screen');
        ReadLn;
        Error := TRUE;
      end;
    if (ModMap[X,Y] = 'C') and not Error
    then begin
      ClrScr;
      WriteLn ('Error: Cage not closed');
      ReadLn;
      Error := TRUE;
    end;
    if not Error and (ModMap[X,Y] <> 'X')
    then begin
      ModMap[X,Y] := '#';
      if (X > 1)
	then if ModMap[X-1,Y] in NOT_WALL + ['S'] + ['M'] + ['C'] + ['X']
        then Scan_Map(X-1,Y,ModMap,Error);
      if (X < 28)
	then if ModMap[X+1,Y] in NOT_WALL + ['S'] + ['M'] + ['C'] + ['X']
        then Scan_Map(X+1,Y,ModMap,Error);
      if (Y > 1)
	then if ModMap[X,Y-1] in NOT_WALL + ['S'] + ['M'] + ['C'] + ['X']
        then Scan_Map(X,Y-1,ModMap,Error);
      if (Y < 31)
	then if ModMap[X,Y+1] in NOT_WALL + ['S'] + ['M'] + ['C'] + ['X']
        then Scan_Map(X,Y+1,ModMap,Error);
    end;
  end; {Scan_Map}
  {= = = = = = = = = =}
begin
  Clear_Map (PacMap);
  Clear_Map (ModMap);
  Cage.Caught := FALSE;
  Origin.Caught := FALSE;
  Warps[1].Caught := FALSE;
  Warps[1].Dir := DOWN;
  Warps[1] := Warps[2];
  Error := FALSE;
  X := 0;
  MonCount := 0;
  WarpCount := 0;
  DotsInMap := 0;
{=-=-= Part 1: Read the map from the file, check for any errors =-=-=}
  Assign (MapFile, MapSelected);
  Reset (MapFile);
  for Y := 1 to 31 do
  begin
  X := 1;
  while not EoLN (MapFile) do
  begin
    Read (MapFile, PacMap[X,Y]);
    ModMap[X,Y] := PacMap[X,Y];
    if not (PacMap [X,Y] in ['W','C','M','S','.','*','X'])
    then PacMap[X,Y] := ' ';
    if PacMap[X,Y] = 'S' 
    then begin
      if Origin.Caught and not Error
        then begin
        ClrScr;
        WriteLn ('Error: Multiple starts found');
        ReadLn;
        Error := TRUE;
      end
      else begin
        Origin.X := X;
        Origin.Y := Y;
        Origin.Caught := TRUE;
        PacMap[X,Y] := ' ';
      end;
    end;
    if (PacMap[X,Y] = 'X')
    then begin
      WarpCount := WarpCount + 1;
      if (WarpCount < 3) 
	then begin
        Warps[WarpCount].X := X;
        Warps[WarpCount].Y := Y;
        Warps[WarpCount].Caught := TRUE;
        Warps[WarpCount].Dir := UP;
        PacMap[X,Y] := ' ';
      end
      else if not Error 
	then begin
        ClrScr;
        WriteLn ('Error: Too many warps found');
        ReadLn;
        Error := TRUE;
      end;
    end;
    if (PacMap[X,Y] = 'M')
    then if (MonCount < MAX_GHOSTS)
    then begin
      MonCount := MonCount + 1;
      GhostsOrigin[MonCount].X := X;
      GhostsOrigin[MonCount].Y := Y;
      PacMap[X,Y] := ' ';
    end
    else begin
      PacMap[X,Y] := ' ';
      end;
      if PacMap[X,Y] in ['.', '*'] 
      then DotsInMap := DotsInMap + 1;
      if PacMap[X,Y] = 'C'
      then begin
        if Cage.Caught and not Error
        then begin
          ClrScr;
          WriteLn ('Error: Multiple Cages found');
          ReadLn;
          Error := TRUE;
          PacMap[X,Y] := ' ';
        end
      else begin
        Cage.X := X;
        Cage.Y := Y;
        Cage.Caught := TRUE;
        PacMap[X,Y] := ' ';
      end;
    end;
    X := X + 1;
  end;
  ReadLn (MapFile);
  end;
  Close (MapFile);
  if (WarpCount = 1) and not Error 
  then begin
    ClrScr;
    WriteLn ('Error: Only one warp found');
    ReadLn;
    Error := TRUE;
  end;
  if not Cage.Caught and not Error 
  then begin
    ClrScr;
    WriteLn ('Error: No cage found');
    ReadLn;
    Error := TRUE;
  end;
  if not Origin.Caught and not Error 
  then begin
    ClrScr;
    WriteLn ('Error: No start found');
    ReadLn;
    Error := TRUE;
  end;
  Scan_Map(Origin.X,Origin.Y,ModMap,Error);
  for Y := 1 to 31 do
    for X := 1 to 28 do
    begin
      if (ModMap[X,Y] in ['*','.']) and not Error 
      then begin
        ClrScr;
        WriteLn ('Error: Inaccessible dot at (',X:2,':',Y:2,')');
        ReadLn;
        Error := TRUE;
      end;
      if (ModMap[X,Y] = 'M') and not Error 
      then begin
        ClrScr;
        WriteLn ('Error: Inaccessible ghost at (',X:2,':',Y:2,')');
        ReadLn;
        Error := TRUE;
      end;
    end;
{=-=-= Part 2: convert the walls from ugly w's to beautiful lines =-=-=}
  for Y := 1 to 31 do
    for X := 1 to 28 do
    begin
{=-= Map adjecent walls =-=}
      Block := [];
      if (Y > 1) 
      then if PacMap[X,Y-1] in WALL 
      then Block := Block + [UP];
      if (Y < 31)
      then if PacMap[X,Y+1] in WALL
      then Block := Block + [DOWN];
      if (X > 1)
      then if PacMap[X-1,Y] in WALL
      then Block := Block + [LEFT];
      if (X < 28)
      then if PacMap[X+1,Y] in WALL 
      then Block := Block + [RIGHT];
{=-= Map scannable areas to pervent extra-array accessing =-=}
      for X1 := -2 to 2 do
        for Y1 := -2 to 2 do
          Scan [X1,Y1] := 'x';
      for X1 := -2 to 2 do
        for Y1 := -2 to 2 do
          if (X1+X in [1..28]) and (Y1+Y in [1..31])
          then Scan [X1,Y1] := '-';
      for X1 := -2 to 2 do
        for Y1 := -2 to 2 do
          if (Scan[X1,Y1] = '-')
          then if (PacMap[X+X1,Y+Y1] in WALL)
          then Scan[X1,Y1] := 'W';
{=-= Hollow out two-wall thickness areas =-=}
      if (Scan[-2,0] + Scan[-1,0] + Scan[1,0] 
          + Scan[-1,-1] + Scan[-1,1] = '-W-WW') and (Block <> [Left])
      then Block := Block - [Left];
      if (Scan[-1,0] + Scan[1,0] + Scan[2,0] 
          + Scan[1,-1] + Scan[1,1] = '-W-WW') and (Block <> [Right])
      then Block := Block - [Right];
      if (Scan[0,-2] + Scan[0,-1] + Scan[0,1]
          + Scan[-1,-1] + Scan[1,-1] = '-W-WW') and (Block <> [Up])
      then Block := Block - [Up];
      if (Scan[0,-1] + Scan[0,1] + Scan[0,2]
          + Scan[-1,1] + Scan[1,1] = '-W-WW') and (Block <> [Down])
      then Block := Block - [Down];
{=-= Reconnect corners =-=}
      if (Block = [Right]) or (Block = [Left])
      then begin
        if (Y > 1)
        then if PacMap[X, Y-1] in Wall 
        then Block := Block + [Up];
        if (Y < 31)
        then if PacMap[X,Y+1] in Wall
        then Block := Block + [Down];
      end;
      if (Block = [Up]) or (Block = [Down])
      then begin
        if (X > 1)
        then if PacMap[X-1, Y] in Wall
        then Block := Block + [Left];
        if (X < 28)
        then if PacMap[X+1,Y] in Wall
        then Block := Block + [Right];
      end;
{=-= Convert 4-ways to 2-wall hollow corners =-=}
      if (Block = [Up, Down, Left, Right])
      then begin
        if (Scan[1,1] = '-') and (Scan [-1,-1] = 'W') 
            and (Scan[-1,1] = 'W') and (Scan [1,-1] = 'W')
        then Block := [Right, Down];
        if (Scan[-1,1] = '-') and (Scan [1,-1] = 'W') 
            and (Scan[1,1] = 'W') and (Scan [-1,-1] = 'W')
        then Block := [Left, Down];
        if (Scan[1,-1] = '-') and (Scan [-1,1] = 'W') 
            and (Scan[1,1] = 'W') and (Scan [-1,-1] = 'W')
        then Block := [Right, Up];
        if (Scan[-1,-1] = '-') and (Scan [1,1] = 'W')
            and (Scan[-1,1] = 'W') and (Scan [1,-1] = 'W')
        then Block := [Left, Up];
      end;
{=-= Remove interior connections to 2-walls =-=}
      if (Block = [Up, Left, Down])
          and ((Scan[-2,-1] = '-') or (Scan[-2,1] = '-')) 
          and ((Scan[-1,-1] = 'W') and (Scan[-1,1] = 'W'))
      then Block := [Up,Down];
      if (Block = [Up, Right, Down])
          and ((Scan[2,-1] = '-') or (Scan[2,1] = '-')) 
          and ((Scan[1,-1] = 'W') and (Scan[1,1] = 'W'))
      then Block := [Up,Down];
      if (Block = [Left, Right, Down])
          and ((Scan[-1,2] = '-') or (Scan[1,2] = '-'))
          and ((Scan[-1,1] = 'W') and (Scan[1,1] = 'W'))
      then Block := [Left,Right];
      if (Block = [Left, Right, Up])
          and ((Scan[-1,-2] = '-') or (Scan[1,-2] = '-'))
          and ((Scan[-1,-1] = 'W') and (Scan[1,-1] = 'W'))
      then Block := [Left,Right];
{=-= Convert map based on Block data =-=}
      if PacMap[X,Y] in [' ','.','*']
      then Block := [];
      if not (PacMap[X,Y] in [' ','.','*']) and (Block = [])
      then PacMap[X,Y] := '█';
      if Block = [UP] 
      then PacMap[X,Y] := '║';
      if Block = [DOWN] 
      then PacMap[X,Y] := '║';
      if Block = [LEFT]
      then PacMap[X,Y] := '═';
      if Block = [RIGHT]
      then PacMap[X,Y] := '═';
      if Block = [UP,DOWN]
      then PacMap[X,Y] := '║';
      if Block = [UP,LEFT]
      then PacMap[X,Y] := '╝';
      if Block = [UP,RIGHT]
      then PacMap[X,Y] := '╚';
      if Block = [DOWN,LEFT]
      then PacMap[X,Y] := '╗';
      if Block = [DOWN,RIGHT]
      then PacMap[X,Y] := '╔';
      if Block = [LEFT,RIGHT]
      then PacMap[X,Y] := '═';
      if Block = [UP,DOWN,LEFT]
      then PacMap[X,Y] := '╣';
      if Block = [UP,DOWN,RIGHT]
      then PacMap[X,Y] := '╠';
      if Block = [UP,LEFT,RIGHT]
      then PacMap[X,Y] := '╩';
      if Block = [DOWN,LEFT,RIGHT]
      then PacMap[X,Y] := '╦';
      if Block = [UP,DOWN,LEFT,RIGHT]
      then PacMap[X,Y] := '╬';
    end;
end; {Read_Map}
{----------------------------------------------------------------------}
function Get_Speed (Speed : integer) : real;
begin
  Get_Speed := FRAMERATE / Speed;
end; {GetSpeed}
{----------------------------------------------------------------------}
procedure GoTo_Next_Level (var DotCount, Level, PowerTime : integer;
                           var Ghosts : GhostStats;
                           var PacWait, GhostWait : real;
                           DotsInMap, MonCount : integer);
var 
  Count : integer;
begin
  DotCount := DotsInMap;
  Level := Level + 1;
  PowerTime := 0;
  for Count := 1 to MonCount do
  with Ghosts[Count] do
  begin
    X := 1;
    Y := 1;
    Dir := Up;
    Caught := FALSE;
  end;
end; {GoTo_Next_Level}
{----------------------------------------------------------------------}
procedure Init_Ghosts (var Ghosts, GhostsOrigin : GhostStats;
                       MonCount : integer);
var 
  Count : integer;
begin
  for Count := 1 to MonCount do
  with Ghosts[Count] do
  begin
    X := GhostsOrigin[Count].X;
    Y := GhostsOrigin[Count].Y;
    Caught := FALSE;
  end;
end; {Init_Ghosts}
{----------------------------------------------------------------------}
procedure Init_PacMan (var PacMan, Origin : CharacterDataType;
                       var AltDir : DirType);
begin
  with PacMan do
  begin
    X := Origin.X;
    Y := Origin.Y;
    Caught := FALSE;
    Dir := LEFT;
  end;
  AltDir := LEFT;
end; {Init_PacMan}
{----------------------------------------------------------------------}
{- Graphic Procedures -------------------------------------------------}
{----------------------------------------------------------------------}
procedure Draw_Map (var PacMap : MapType;
                        PacMan : CharacterDataType;
                        DotsInMap, DotCount, Level : integer;
                        Cheats : CheatArray;
                        PacWait : Real;
                        ExtendedMaps : boolean);
var
  X,Y, Range : integer;
  Special : boolean;
begin
  Range := PacMan.Y - 12;
  if Range < 0
  then Range := 0;
  if Range > 7 
  then Range := 7;
  for Y := 1 to 24 do
  begin
    GoToXY (16, Y);
    for X := 1 to 28 do
    begin
      Special := FALSE;
{=-=-=-= Found a wall section =-=-=-=}
      if not (PacMap[X, Y+Range] in NOT_WALL)
	then begin
        Special := TRUE;
        if ExtendedMaps
        then TextColor (((Level-1) mod 6) + 1)
        else TextColor (DkBlue);
        if Cheats[WallFlash]
        then TextColor (Random(14)+1);
        if Cheats[FadeMode] and (Random(DotsInMap) > Dotcount)
        then TextColor(Black);
        if Cheats[DarkMode] 
	      and (not (abs(X - PacMan.X) in [0,1,2,3]) 
		or not (abs((Y+range) - PacMan.Y) in [0,1,2,3]))
        then TextColor (Black);
        if Cheats[AtariMode]
        then Write (PacMap[X, Y+Range])
        else Write ('█');
      end;
{=-=-=-= Found a dot =-=-=-=}
      if PacMap[X,Y+Range] = '.' then
      begin
        Special := TRUE;
        TextColor (LtYellow);
        if Cheats[DotFlash]
        then TextColor (Random(14)+1);
        if Cheats[FadeMode] and (Random(DotsInMap) > Dotcount)
        then TextColor(Black);
        if (Cheats[DarkMode])
        then begin
          TextColor (Black);
          if (Random(4) = 0)
          then TextColor (DkGrey);
        end;
        if Cheats[DarkMode]
            and not (not (abs(X - PacMan.X) in [0,1,2,3])
		or not (abs((Y+range) - PacMan.Y) in [0,1,2,3]))
        then TextColor (Yellow);
        Write ('∙');
      end;
{=-=-=-= Found a super dot =-=-=-=}
      if PacMap[X,Y+Range] = '*' then
      begin
        Special := TRUE;
        TextColor (LtYellow);
        if Cheats[DotFlash]
        then TextColor (Random(15)+1);
        if (Cheats[DarkMode])
        then begin
          TextColor (Black);
          if (Random(4) = 0)
          then TextColor (DkGrey);
        end;
        if Cheats[DarkMode]
            and not (not (abs(X - PacMan.X) in [0,1,2,3])
            or not (abs((Y+range) - PacMan.Y) in [0,1,2,3]))
        then TextColor (LtYellow);
        if Cheats[AtariMode]
          then Write ('Θ')
          else Write ('■');
      end;
      if not Special
	then Write (Pacmap[X,Y+Range]);
    end;
    WriteLn;
  end;
end; {Draw_Map}
{----------------------------------------------------------------------}
procedure Draw_Player (Pacman : CharacterDataType;
                       PowerTime : integer;
                       Cheats : CheatArray);
var
  Range : integer;
begin
  Range := PacMan.Y - 12;
  if (Range < 0)
  then Range := 0;
  if (Range > 7)
  then Range := 7;
  GoToXY (PacMan.X + 15, PacMan.Y - Range);
  TextColor (White);
  if (PowerTime > 0)
  then Textcolor (Random(7)+9);
  if (Cheats[AtariMode])
  then Write ('')
  else Write ('█');
end; {Draw_Player}
{----------------------------------------------------------------------}
procedure Draw_Ghosts (Ghosts : GhostStats;
                       PacMan : CharacterDataType;
                       MonCount, PowerTime : integer;
                       Cheats : CheatArray;
                       PacWait : Real);
var
  Count, Range : integer;
begin
  Range := PacMan.Y - 12;
  if Range < 0 
  then Range := 0;
  if Range > 7 
  then Range := 7;
  for Count := 1 to MAX_GHOSTS do
  if (Ghosts[Count].Y > Range) and (Ghosts[Count].Y - Range < 25) 
  then begin
    if Cheats[GlowGhosts] 
    then TextBackground (random(8)+1);
    GoToXY (Ghosts[Count].X + 15, Ghosts[Count].Y - Range);
    TextColor (12 - MAX_GHOSTS + Count);
    if (PowerTime > 0)
    then TextColor (White);
    if (PowerTime < 90) and Odd(PowerTime div 4)
    then TextColor (LtBlue);
    if Ghosts[Count].Caught
    then TextColor (LtGrey);
    if Cheats[DarkMode]
    then begin
      if (not (abs(Ghosts[Count].X - PacMan.X) in [0,1,2,3])
          or not (abs((Ghosts[Count].Y) - PacMan.Y) in [0,1,2,3]))
      then begin
        TextColor (0);
        if (Random(4) = 0)
        then TextColor (DkGrey);
      end;
    end;
    if Cheats[AtariMode]
    then case Odd(trunc(PacWait) mod 2) of
      TRUE : 
       Write ('Ω');
      FALSE : 
        Write ('∩');
    end
    else Write ('█');
    if Cheats[GlowGhosts]
    then TextBackground (0);
  end;
end; {Draw_Ghosts}
{----------------------------------------------------------------------}
procedure Draw_StatusBar (Level, Lives, PowerTime : integer;
                          Score : Real;
                          Cheats : CheatArray;
                          HiScores : ScoreType);
var 
  Count : integer;
begin
  TextColor (SBAR_BORDER);
  GoToXY (51, 2);
  Write ('╔══════════════════╗');
{=-=-=-=}
  GoToXY (51, 3);
  Write ('║      PACMAN      ║');
{=-=-=-}
  GoToXY (51, 4);
  Write ('║By Chris MacGregor║');
{=-=-=-=}
  GoToXY (51, 5);
  Write ('╟──────────────────╢');
{=-=-=-=}
  GoToXY (51, 6);
  Write ('║');
  TextColor (SBAR_TEXT);
  if Cheats[GodMode]
  then TextColor (12);
  Write ('LEVEL :         ', Level:2);
  TextColor (SBAR_BORDER);
  Write ('║');
{=-=-=-=}
  GoToXY (51, 7);
  Write ('║');
  TextColor (SBAR_TEXT);
  if Cheats[GodMode]
  then TextColor (12);
  Write ('SCORE : ', Score:10:0);
  TextColor (SBAR_BORDER);
  Write ('║');
{=-=-=-=}
  GoToXY (51, 8);
  Write ('║');
  if (PowerTime = 0) 
  then begin
    TextColor (SBAR_TEXT);
    if Cheats[GodMode]
    then TextColor (12);
    Write ('LIVES : ');
    for Count := 1 to 11 - Lives do
      Write (' ');
    if Lives < 12
    then for Count := 1 to Lives - 1 do
      Write ('')
    else Write (' ', Lives-1:2, ' ')
  end
  else begin
    for Count := 1 to 18 - PowerTime div 10 do
      Write ('░');
    TextColor (random(8)+8);
    for Count := 1 to PowerTime div 10 do
      Write ('█');
  end;
  TextColor (SBAR_BORDER);
  Write ('║');
{=-=-=-=}
  GoToXY (51, 9);
  Write ('╚══════════════════╝');
{=-=-=-=}
  GoToXY (51, 10);
  Write ('╔══════════════════╗');
{=-=-=-=}
  GoToXY (51, 11);
  Write ('║   HIGH  SCORES   ║');
{=-=-=-=}
  GoToXY (51, 12);
  Write ('╟──────────────────╢');
{=-=-=-=}
  for Count := 1 to 5 do
  begin
    GoToXY (51, 11 + 2 * Count);
    Write ('║ ');
    TextColor (Count+8);
    Write (HiScores[Count].Name :16);
    TextColor (SBAR_BORDER);
    Write (' ║');
    GoToXY (51, 12 + 2 * Count);
    Write ('║ ');
    TextColor (Count+8);
    Write (Hiscores[Count].Score :16 :0);
    TextColor (SBAR_BORDER);
    Write (' ║');
  end;
{=-=-=-=}
  GoToXY (51, 23);
  Write ('╚══════════════════╝');
end; {Draw_StatusBar}
{----------------------------------------------------------------------}
procedure Refresh_Screen;
begin
  Draw_Map (PacMap, PacMan, DotsInMap, DotCount, Level,
            Cheats, PacWait, ExtendedMaps);
  Draw_Player (Pacman, PowerTime, Cheats);
  Draw_Ghosts (Ghosts, PacMan, MonCount,
               PowerTime, Cheats, PacWait);
  Draw_StatusBar (Level, Lives, PowerTime, Score, Cheats, HiScores);
  GoToXY (80,25);
end; {Refresh_Screen}
{----------------------------------------------------------------------}
{- Game physics procedures --------------------------------------------}
{----------------------------------------------------------------------}
function Opposite_Dir (Orig_Dir : DirType) : DirType;
begin
  case Orig_Dir of
    UP :
     Opposite_Dir := DOWN;
    DOWN :
     Opposite_Dir := UP;
    LEFT :
     Opposite_Dir := RIGHT;
    RIGHT :
     Opposite_Dir := LEFT;
  end;
end; {Opposite_Dir}
{----------------------------------------------------------------------}
procedure Pause (ToPrint : String;
                 Color : integer;
                 var EnteredKey : char);
begin
  GoToXY (30 - Length(ToPrint) div 2, 11);
  TextColor (Color);
  Write (ToPrint);
  repeat
    EnteredKey := ReadKey;
  until EnteredKey in PAUSE_KEY + QUIT_KEY;
end; {Pause}
{----------------------------------------------------------------------}
procedure Return_Ghosts (var Ghosts : GhostStats;
                         GhostsOrigin : GhostStats;
                         MonCount : integer;
                         OnlyCaught : boolean);
var
  Count : integer;
begin
  for Count := 1 to MonCount do
    if (OnlyCaught and Ghosts[Count].Caught) or not OnlyCaught
    then with Ghosts[Count] do
    begin
      X := GhostsOrigin[Count].X;
      Y := GhostsOrigin[Count].Y;
      Caught := FALSE;
    end;
end; {Home_Ghosts}
{----------------------------------------------------------------------}
procedure Move_Characters (var Ghosts : GhostStats;
                           var PacMan : CharacterDataType;
                           PacMap : MapType;
                           MonCount : integer;
                           MoveGhosts, MovePacMan : boolean;
                           AltDir : DirType);
var
  Count : integer;
  Moved : boolean;
begin
  if MoveGhosts
  then for Count := 1 to MonCount do
    with Ghosts[Count] do
    case Dir of
      UP :
       Y := Y - 1;
      DOWN : 
       Y := Y + 1;
      LEFT :
       X := X - 1;
      RIGHT :
       X := X + 1;
    end;
  if MovePacMan
  then with PacMan do
  begin
    Moved := FALSE;
    if AltDir = Opposite_Dir(Dir)
    then AltDir := Dir;
    case AltDir of
      UP : 
       if PacMap[X,Y-1] in NOT_WALL
       then begin
         Y := Y - 1;
         Dir := AltDir;
         Moved := TRUE;
       end;
      DOWN :
       if PacMap[X,Y+1] in NOT_WALL
       then begin
         Y := Y + 1;
         Dir := AltDir;
         Moved := TRUE;
       end;
      LEFT :
       if PacMap[X-1,Y] in NOT_WALL
       then begin
         X := X - 1;
         Dir := AltDir;
         Moved := TRUE;
       end;
      RIGHT :
       if PacMap[X+1,Y] in NOT_WALL
       then begin
         X := X + 1;
         Dir := AltDir;
         Moved := TRUE;
       end;
    end;
    if not Moved
    then case Dir of
      UP :
       if PacMap[X,Y-1] in NOT_WALL 
       then Y := Y - 1;
      DOWN :
       if PacMap[X,Y+1] in NOT_WALL
       then Y := Y + 1;
      LEFT :
       if PacMap[X-1,Y] in NOT_WALL
       then X := X - 1;
      RIGHT :
       if PacMap[X+1,Y] in NOT_WALL
       then X := X + 1;
    end;
  end;
end; {Move}
{----------------------------------------------------------------------}
procedure Ghost_AI (var Ghosts : GhostStats;
                        PacMap : MapType;
                        MonCount, Level : integer;
                        PacMan : CharacterDataType);
var
  Count, Rand : integer;
  LegalDirs : set of DirType;
  Temp : DirType;
begin
  for Count := 1 to MonCount do
  with Ghosts[Count] do
  begin
{=-=-= Set all directions to possible =-=-=}
    LegalDirs := [UP, DOWN, LEFT, RIGHT];
{=-=-= Prevent Ghosts from going back on themselves =-=-=}
    LegalDirs := LegalDirs - [Opposite_Dir(Dir)];
{=-=-= Check which directions are walls =-=-=}
    if not (PacMap[X,Y-1] in NOT_WALL)
    then LegalDirs := LegalDirs - [UP];
    if not (PacMap[X,Y+1] in NOT_WALL)
    then LegalDirs := LegalDirs - [DOWN];
    if not (PacMap[X-1,Y] in NOT_WALL)
    then LegalDirs := LegalDirs - [LEFT];
    if not (PacMap[X+1,Y] in NOT_WALL)
    then LegalDirs := LegalDirs - [RIGHT];
{=-=-= Remove directions that are opposite of the player =-=-=}
    if (PacMan.X > X) and (Left in LegalDirs)
        and (LegalDirs <> [Left]) and (Random(Level) = 0)
    then LegalDirs := LegalDirs - [Left];
    if (PacMan.X < X) and (Right in LegalDirs)
        and (LegalDirs <> [Right]) and (Random(Level) = 0)
    then LegalDirs := LegalDirs - [Right];
    if (PacMan.Y > Y) and (Up in LegalDirs)
        and (LegalDirs <> [Up]) and (Random(Level) = 0)
    then LegalDirs := LegalDirs - [Up];
    if (PacMan.Y < Y) and (Down in LegalDirs)
        and (LegalDirs <> [Down]) and (Random(Level) = 0)
    then LegalDirs := LegalDirs - [Down];
{=-=-= If in a dead end, turn around, else, pick a dir =-=-=}
    if LegalDirs = []
    then Dir := Opposite_Dir(Dir)
    else begin
      repeat
        Rand := Random(4);
        case Rand of
          0 :
           Temp := UP;
          1 :
           Temp := DOWN;
          2 :
           Temp := LEFT;
          3 :
           Temp := RIGHT;
        end;
      until Temp in LegalDirs;
      Dir := Temp;
    end;
  end;
end; {Ghost_AI}
{----------------------------------------------------------------------}
procedure Process_KeyStroke (var EnteredKey : char;
                             var PacMan : CharacterDataType;
                             var AltDir : DirType;
                             PacMap : MapType;
                             var Cheats : CheatArray);
begin
  with PacMan do
  case EnteredKey of
    '2','P' :
     if PacMap[X,Y+1] in NOT_WALL
     then begin
       Dir := DOWN;
       AltDir := DOWN;
     end
     else AltDir := DOWN;
    '4','K' :
     if PacMap[X-1,Y] in NOT_WALL
     then begin
       Dir := LEFT;
       AltDir := LEFT;
     end
     else AltDir := LEFT;
    '6','M' :
     if PacMap[X+1,Y] in NOT_WALL
     then begin
       Dir := RIGHT;
       AltDir := RIGHT;
     end
     else AltDir := RIGHT;
    '8','H' : 
     if PacMap[X,Y-1] in NOT_WALL
     then begin
       Dir := UP;
       AltDir := UP;
     end
    else AltDir := UP;
{=-=-= Cheats =-=-=}
    'w' :
     Cheats[WallFlash] := not Cheats[WallFlash];
    'd' :
     Cheats[DotFlash] := not Cheats[DotFlash];
    'a' :
     Cheats[AtariMode] := not Cheats[AtariMode];
    'g' :
     Cheats[GodMode] := not Cheats[GodMode];
    'm' :
     Cheats[GlowGhosts] := not Cheats[GlowGhosts];
    'f' : 
     Cheats[FadeMode] := not Cheats[FadeMode];
    'n' :
     Cheats[DarkMode] := not Cheats[DarkMode];
    ' ', chr(13) :
     Pause ('-=PAUSED=-', SBAR_BORDER, EnteredKey);
    's' :
     Cheats[SoundEnabled] := not Cheats[SoundEnabled];
  end;
end; {ProcessKeyStroke}
{----------------------------------------------------------------------}
procedure Eat_Dot (var Score, OneUpScore : Real;
                   var DotCount, PowerTime, Lives : integer;
                   var PacMap : MapType;
                   PacMan : CharacterDataType;
                   Cheats : CheatArray);
begin
{=-=-= Eat a normal dot =-=-=}
  if PacMap[PacMan.X, PacMan.Y] = '.'
  then begin
    if not Cheats[GodMode]
    then Score := Score + 100;
    if Cheats[SoundEnabled]
    then if Odd (DotCount)
    then Sound (220)
    else Sound (200);
    DotCount := DotCount - 1;
    PacMap[PacMan.X, PacMan.Y] := ' ';
  end;
{=-=-= Eat a super dot =-=-=}
  if PacMap[PacMan.X, PacMan.Y] = '*'
  then begin
    if not Cheats[GodMode]
    then Score := Score + 500;
    if Cheats[SoundEnabled]
    then Sound (440);
    DotCount := DotCount - 1;
    PowerTime := FRAMERATE * 6;
    PacMap[PacMan.X, PacMan.Y] := ' ';
  end;
end; {Eat_Dot}
{----------------------------------------------------------------------}
procedure Bump (var Lives : integer;
                var Ghosts : GhostStats;
                var PacMan, Cage : CharacterDataType;
                var Score, OneUpScore : real;
                MonCount : integer;
                MoveGhosts : boolean;
                Cheats : CheatArray);
var
  CaughtCount, Count, SubCount, WarpCount : integer;
  CatchBonus : real;
  {- - - - - -}
  function TwoToOne (var Number:integer) : integer;
  begin
    if Number = 1
    then TwoToOne := 2
    else TwoToOne := 1;
  end; {TwoToOne}
  {= = = = = =}
begin
  CaughtCount := 0;
  for Count := 1 to MonCount do
  if Ghosts[Count].Caught
  then CaughtCount := CaughtCount + 1;
  for WarpCount := 1 to 2 do
  if (PacMan.X = Warps[WarpCount].X)
      and (PacMan.Y = Warps[WarpCount].Y)
      and Warps[WarpCount].Caught and not MoveGhosts
  then begin
     Pacman.X := Warps[TwoToOne(WarpCount)].X;
     PacMan.Y := Warps[TwoToOne(WarpCount)].Y;
     Warps[TwoToOne(WarpCount)].Caught := FALSE;
  end;
  if Warps[1].Caught or Warps[2].Caught
  then begin
     Warps[1].Caught := TRUE;
     Warps[2].Caught := TRUE;
  end;
  for Count := 1 to MonCount do
  begin
    if (Ghosts[Count].X = PacMan.X) and (Ghosts[Count].Y = PacMan.Y)
    then begin
      if (PowerTime > 0)
      then begin
        CatchBonus := 200;
        for SubCount := 1 to CaughtCount do
          CatchBonus := 2 * CatchBonus;
        if not Cheats[GodMode]
        then Score := Score + CatchBonus;
        Ghosts[Count].X := Cage.X;
        Ghosts[Count].Y := Cage.Y;
        Ghosts[Count].Caught := TRUE;
      end;
      if (PowerTime = 0) and not Cheats[GodMode]
      then PacMan.Caught := TRUE;
    end;
    for WarpCount := 1 to 2 do
    if (Ghosts[Count].X = Warps[WarpCount].X)
        and (Ghosts[Count].Y = Warps[WarpCount].Y)
        and Warps[WarpCount].Caught and MoveGhosts
    then begin
       Ghosts[Count].X := Warps[TwoToOne(WarpCount)].X;
       Ghosts[Count].Y := Warps[TwoToOne(WarpCount)].Y;
       Warps[TwoToOne(WarpCount)].Caught := FALSE;
    end;
    if Warps[1].Caught or Warps[2].Caught
    then begin
       Warps[1].Caught := TRUE;
       Warps[2].Caught := TRUE;
    end;
  end;
end; {Bump}
{----------------------------------------------------------------------}
procedure Play_Level;
var
  Count, CaughtCount, CatchBonus, SubCount : integer;
begin
  repeat
    if PacMan.Caught then
    begin
      Init_PacMan (PacMan, Origin, AltDir);
      Init_Ghosts (Ghosts, GhostsOrigin, MonCount);
      PacWait := Get_Speed(10);
      GhostWait := Get_Speed(Level+3);
      Refresh_Screen;
      Pause ('READY!', 14, EnteredKey);
    end;
    PacWait := PacWait - 1;
    GhostWait := GhostWait - 1;
    if (Score >= OneUpScore)
    then begin
      OneUpScore := OneUpScore + SCOREFOR1UP;
      Lives := Lives + 1;
      if Lives > 100
      then Lives := 100;
    end;
    if (PowerTime > 0)
    then begin
      PowerTime := PowerTime - 1;
      if (PowerTime = 0)
      then Return_Ghosts (Ghosts, GhostsOrigin, MonCount, TRUE);
    end;
    if (GhostWait < 1)
    then begin
      if PowerTime > 0
      then GhostWait := GhostWait + Get_Speed (3)
      else GhostWait := GhostWait +
      Get_Speed (Level div 2+3);
      if (GhostWait < -1)
      then GhostWait := -1;
      Ghost_AI (Ghosts, PacMap, MonCount, Level, PacMan);
      Move_Characters (Ghosts, PacMan, PacMap, MonCount,
                       TRUE, FALSE, AltDir);
      Bump (Lives, Ghosts, PacMan, Cage, Score,
            OneUpScore, MonCount, TRUE, Cheats);
    end;
    if KeyPressed
    then begin
      EnteredKey := ReadKey;
      if EnteredKey = chr(0)
      then EnteredKey := ReadKey;
      Process_KeyStroke (EnteredKey, PacMan, AltDir, PacMap, Cheats);
    end;
    if (PacWait < 1) and not PacMan.Caught
    then begin
      PacWait := PacWait + Get_Speed (PAC_SPEED);
      Move_Characters (Ghosts, PacMan, PacMap, MonCount,
                       FALSE, TRUE, AltDir);
      Eat_Dot (Score, OneUpScore, DotCount, PowerTime,
               Lives, PacMap, PacMan, Cheats);
      Bump (Lives, Ghosts, PacMan, Cage, Score,
            OneUpScore, MonCount, FALSE, Cheats);
      if (PowerTime > 0) and Cheats[SoundEnabled]
      then begin
        case PowerTime of
          1..29 : if (PowerTime mod 1) = 0
          then Sound (2000);
          31..59 : if (PowerTime mod 2) = 0
          then Sound (2000);
          61..89 : if (PowerTime mod 4) = 0
          then Sound (2000);
        end;
       if PowerTime mod 30 = 0
       then Sound (1000);
      end;
    end;
    Refresh_Screen;
    if PacMan.Caught
    then Lives := Lives - 1;
    Delay (500 div FRAMERATE);
    NoSound;
    Delay (500 div FRAMERATE);
    if (PacMan.Caught) or (DotCount = 0)
    then Delay (2000 - 1000 div FRAMERATE);
  until (Lives = 0) or (DotCount = 0) or (EnteredKey in QUIT_KEY);
end; {Game}
{----------------------------------------------------------------------}
procedure Menu (var EnteredKey : char;
                var Max_Ghosts : integer;
                var Cheats : CheatArray;
                var ExtendedMaps : boolean);
var
  GameMode : ModeTypeSet;
  Maps : WorldType;
  Pointer : integer;
  {- - - - - - - - - -}
  procedure Draw_Menu (GameMode : ModeTypeSet;
                       Maps : WorldType;
                       Pointer : integer);
  begin
    ClrScr;
{=-=-=}
    TextColor (SBAR_TEXT);
    if GameMode = []
    then TextColor (LtGreen);
    WriteLn ('  Standard Mode');
{=-=-=}
    TextColor (SBAR_TEXT);
    if Fade in GameMode
    then TextColor (LtGreen);
    WriteLn ('  Fade Mode');
{=-=-=}
    TextColor (SBAR_TEXT);
    if Nightmare in GameMode
    then TextColor (LtGreen);
    WriteLn ('  NightMare Mode');
{=-=-=}
    TextColor (SBAR_TEXT);
    if Mob in GameMode
    then TextColor (LtGreen);
    WriteLn ('  Mob Mode');
{=-=-=}
    TextColor (SBAR_TEXT);
    if Maps = PacMapOnly
    then TextColor (LtGreen);
    WriteLn ('  PacMan Maps');
{=-=-=}
    TextColor (SBAR_TEXT);
    if Maps = Extended
    then TextColor (LtGreen);
    WriteLn ('  Extended Maps');
{=-=-=}
    GoToXY (1, Pointer);
    Write ('->');
    GoToXY (80,25);
  end; {Menu_Draw}
  {= = = = = = = = = =}
begin
  GameMode := [];
  Maps := PacMapOnly;
  Pointer := 1;
  repeat
    Draw_Menu (GameMode, Maps, Pointer);
    GoToXY (80, 25);
    EnteredKey := ReadKey;
    if EnteredKey = Chr(0)
    then EnteredKey := ReadKey;
    if (EnteredKey in ['8','H']) and (Pointer > 1)
    then Pointer := Pointer - 1;
    if (EnteredKey in ['2','P']) and (Pointer < 6)
    then Pointer := Pointer + 1;
    if (EnteredKey = ' ') then
    case Pointer of
      1 :
       GameMode := [];
      2 :
       if Fade in GameMode
       then GameMode := GameMode - [Fade]
       else GameMode := GameMode + [Fade];
      3 :
       if NightMare in GameMode
       then GameMode := GameMode - [NightMare]
       else GameMode := GameMode + [NightMare];
      4 :
       if Mob in GameMode
       then GameMode := GameMode - [Mob]
       else GameMode := GameMode + [Mob];
      5 :
       if Maps = Extended
       then Maps := PacMapOnly;
      6 :
       if Maps = PacMapOnly
       then Maps := Extended;
    end;
  until (EnteredKey in [chr(27)] + [chr(13)]);
  if Fade in GameMode
  then Cheats[FadeMode] := TRUE;
  if NightMare in GameMode
  then Cheats[DarkMode] := TRUE;
  if Mob in GameMode
  then Max_Ghosts := 8;
  if Maps = PacMapOnly
  then ExtendedMaps := FALSE
  else ExtendedMaps := TRUE;
end; {Menu}
{----------------------------------------------------------------------}
procedure Update_Scores (var HiScores : ScoreType;
                         Score : real);
var
  Count, Sub : integer;
  Found : Boolean;
begin
  Found := FALSE;
  for Count := 1 to 5 do
    if (not Found) and (Score > HiScores[Count].Score)
    then begin
      Found := TRUE;
      for Sub := 5 downto Count do
      begin
        HiScores[Sub].Score := HiScores[Sub-1].Score;
        HiScores[Sub].Name := HiScores[Sub-1].Name;
      end;
      TextColor (LtYellow);
      ClrScr;
      WriteLn ('Congratulations!  You''ve earned the #', Count,
               ' high score!');
      Write ('Please enter your name: ');
      repeat
        ReadLn (HiScores[Count].Name);
      until HiScores[Count].Name <> '';
      HiScores[Count].Score := Score;
    end;
end;
{======================================================================}
{= Main Program =======================================================}
{======================================================================}
begin
  Randomize;
  repeat
    Init_Game (Lives, Level, Max_Ghosts, Score,
               OneUpScore, Cheats, HiScores, MapList);
    Menu (EnteredKey, Max_Ghosts, Cheats, ExtendedMaps);
{=-=-=-= Game starts playing here =-=-=-=}
    if not (EnteredKey in QUIT_KEY)
    then repeat
      if ExtendedMaps
      then MapSelected := MapList[(Level mod 6)+1]
      else MapSelected := 'pacmap.map';
      Read_Map (PacMap, Ghosts, GhostsOrigin, PacMan,
                Origin, Cage, Warps, DotsInMap,
                MonCount, Error, Max_Ghosts, MapSelected);
      GoTo_Next_Level (DotCount, Level, PowerTime, Ghosts,
                       PacWait, GhostWait, DotsInMap, MonCount);
      if Error
      then Lives := 0
      else begin
        PacMan.Caught := TRUE;
        ClrScr;
        Play_Level;
      end;
    until (Lives = 0) or (EnteredKey in QUIT_KEY);
{=-=-=-= Game ends here =-=-=-=}
    Update_Scores (HiScores, Score);
    Assign (ScoreFile, SCORESTOREAD);
    ReWrite (ScoreFile);
    for Count := 1 to 5 do
    begin
      WriteLn (ScoreFile, HiScores[Count].Name);
      WriteLn (ScoreFile, HiScores[Count].Score);
    end;
    WriteLn (ScoreFile, ((trunc(HiScores[5].Score) mod 97) * 100000000) +
                        ((trunc(HiScores[2].Score) mod 13) * 1000000) +
                        ((trunc(HiScores[4].Score) mod 31) * 10000) +
                        ((trunc(HiScores[1].Score) mod 17) * 100) +
                         (trunc(HiScores[3].Score) mod 47));
    Close (ScoreFile);
    if EnteredKey <> Chr(27)
    then EnteredKey := ' ';
  until (EnteredKey in QUIT_KEY);
end.
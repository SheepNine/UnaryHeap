program Moving_Maze (input, output);
uses
    Crt;
const
     Delevoper = FALSE;
type
    tileframe = array [1..8, 1..8, 1..4] of integer;
    location = array [1..4] of integer;
    possible = array [1..7, 1..7] of integer;
    namearray = array [1..4] of string[10];
var
   maze : tileframe;
   PossibleMoves, Playercoins : possible;
   PlayerX, PlayerY, PlayerScores : location;
   Offset_X, PlayerCount : integer;
   FourPlayer, SixTreasures, TeamPlay : boolean;
   Playernames : namearray;
{============================================================================}
{== Pre-Game Map Setup Procedures ===========================================}
{============================================================================}
procedure ClearMazeTiles (Var Maze : tileframe);
          {pre: none
          post: all entries in maze are given the value '1'}
var Countx, County, Countdir : integer;
begin
     for Countx := 1 to 8 do
         for County := 1 to 8 do
             for Countdir := 1 to 4 do
                 Maze [Countx, County, Countdir] := 1;
end; {ClearMazeTiles}
{----------------------------------------------------------------------------}
procedure ClearPossiblemoves (Var Possiblemoves : possible);
          {pre: none
          post: all entries in possiblemoves are given the value '0'}
var Countx, County, Countdir : integer;
begin
     for Countx := 1 to 7 do
         for County := 1 to 7 do
             Possiblemoves [Countx, CountY] := 0;
end; {ClearMazeTiles}
{----------------------------------------------------------------------------}
procedure SetConstantTiles (Var Maze : Tileframe);
          {pre: none
          post: maze tiles that are not moved in the game (1357, 1357)
          are given their constant numbers}
begin
     Maze[1,1,3] := 0;
     Maze[1,1,4] := 0;
     Maze[1,3,4] := 0;
     Maze[1,5,4] := 0;
     Maze[1,7,1] := 0;
     Maze[1,7,4] := 0;
     Maze[3,1,3] := 0;
     Maze[3,7,1] := 0;
     Maze[5,1,3] := 0;
     Maze[5,7,1] := 0;
     Maze[7,1,2] := 0;
     Maze[7,1,3] := 0;
     Maze[7,3,2] := 0;
     Maze[7,5,2] := 0;
     Maze[7,7,1] := 0;
     Maze[7,7,2] := 0;
end; {SetConstantTiles}
{----------------------------------------------------------------------------}
procedure SetPlayerCoins (var Playercoins : Possible; FourPlayer, SixTreasures : boolean);
          {pre: None
          post: Playercoins holds the locations of each player's objectives
                via cartesian coordinates}
var CountX, CountY : integer;
begin
     for CountX := 1 to 7 do
         for CountY := 1 to 7 do
             PlayerCoins [CountX, CountY] := 0;
     PlayerCoins [3,1] :=1;
     PlayerCoins [5,3] :=1;
     PlayerCoins [7,5] :=1;
     PlayerCoins [1,3] :=2;
     PlayerCoins [3,5] :=2;
     PlayerCoins [5,7] :=2;
     if FourPlayer
     then begin
               PlayerCoins [3,7] :=3;
               PlayerCoins [5,5] :=3;
               PlayerCoins [7,3] :=3;
               PlayerCoins [1,5] :=4;
               PlayerCoins [3,3] :=4;
               PlayerCoins [5,1] :=4;
          end;
     if SixTreasures
     then begin
               PlayerCoins [3,7] :=2;
               PlayerCoins [5,5] :=1;
               PlayerCoins [7,3] :=1;
               PlayerCoins [1,5] :=2;
               PlayerCoins [3,3] :=2;
               PlayerCoins [5,1] :=1;
          end;
     if TeamPlay
     then begin
               PlayerCoins [3,1] :=1;
               PlayerCoins [3,3] :=1;
               PlayerCoins [3,5] :=2;
               PlayerCoins [3,7] :=2;
               PlayerCoins [5,1] :=1;
               PlayerCoins [5,3] :=1;
               PlayerCoins [5,5] :=2;
               PlayerCoins [5,7] :=2;
               PlayerCoins [1,3] :=1;
               PlayerCoins [1,5] :=2;
               PlayerCoins [7,3] :=1;
               PlayerCoins [7,5] :=2;
          end;
end;
{----------------------------------------------------------------------------}
Procedure SetVariableTiles (Var Maze : Tileframe);
          {pre: Maze contains all ones
          post: the maze tiles that are moved during the game are set}
var Typeframe : array [1..8, 1..7] of integer;
    CountX, CountY, Primed : integer;
    Typecount : array [1..9] of integer;
begin
     {First, clear all the variables to 0:}
     for CountX := 1 to 7 do
         for CountY := 1 to 7 do
             Typeframe [CountX, CountY] := 0;
     for CountX := 1 to 4 do
         TypeCount [CountX] := 7;
     for CountX := 5 to 9 do
         TypeCount [CountX] := 2;
     TypeCount[9] := 0;
     if Delevoper then
     begin
          for CountX := 1 to 4 do
              TypeCount [CountX] := 0;
          for CountX := 5 to 9 do
              TypeCount [CountX] := 0;
          TypeCount[9] := 36;
     end;
     {Now, decide what each piece will be:}
     for CountX := 1 to 7 do
         for CountY := 1 to 7 do
             if not (Odd(CountX) and Odd(CountY))
                then begin
                          repeat
                                Primed := random (9) + 1
                          until typecount[primed] > 0;
                          Typecount[Primed] := Typecount[Primed] - 1;
                          Typeframe [CountX, CountY] := Primed;
                     end;
     repeat
           Primed := random (9) + 1
     until typecount[primed] > 0;
     Typecount[Primed] := Typecount[Primed] - 1;
     Typeframe [8, 1] := Primed;
     {Finally, set the pieces in place:}
     for CountX := 1 to 8 do
         for CountY := 1 to 7 do
             begin
                  if Typeframe[CountX, CountY] = 1
                     then begin
                               Maze [CountX, CountY, 1] := 0;
                               Maze [CountX, CountY, 2] := 0;
                          end;
                  if Typeframe[CountX, CountY] = 2
                     then begin
                               Maze [CountX, CountY, 2] := 0;
                               Maze [CountX, CountY, 3] := 0;
                          end;
                  if Typeframe[CountX, CountY] = 3
                     then begin
                               Maze [CountX, CountY, 3] := 0;
                               Maze [CountX, CountY, 4] := 0;
                          end;
                  if Typeframe[CountX, CountY] = 4
                     then begin
                               Maze [CountX, CountY, 1] := 0;
                               Maze [CountX, CountY, 4] := 0;
                          end;
                  if Typeframe[CountX, CountY] = 5
                     then Maze [CountX, CountY, 1] := 0;
                  if Typeframe[CountX, CountY] = 6
                     then Maze [CountX, CountY, 2] := 0;
                  if Typeframe[CountX, CountY] = 7
                     then Maze [CountX, CountY, 3] := 0;
                  if Typeframe[CountX, CountY] = 8
                     then Maze [CountX, CountY, 4] := 0;
             end;
end; {SetVariableTiles}
{----------------------------------------------------------------------------}
procedure SetPlayerLocations (var PlayerX, PlayerY : location; FourPlayer, TeamPlay : boolean);
          {pre:none
          post:playerX,Y holds the cartesian coordinates for each player's
               starting location}
begin
     PlayerX[1] := 1;
     PlayerY[1] := 7;
     PlayerX[2] := 7;
     PlayerY[2] := 1;
     if FourPlayer
     then begin
          PlayerX[3] := 1;
          PlayerY[3] := 1;
          PlayerX[4] := 7;
          PlayerY[4] := 7;
     end;
     if TeamPlay
     then begin
          PlayerX[4] := 1;
          PlayerY[4] := 1;
          PlayerX[3] := 7;
          PlayerY[3] := 7;
     end;
end; {SetPlayerLocations}
{============================================================================}
{== Board Drawing Procedure =================================================}
{============================================================================}
procedure DrawBoard (Maze : Tileframe; PossibleMoves : Possible;
                    PlayerX, PlayerY : location; FourPlayer : boolean);
          {pre: none
          post: draws the game board}
var Count, CountX, CountY, ScoreBob : integer;
begin
     ClrScr;
     {First, draw the frame:}
     GoToXY (1+Offset_X,1);
     TextColor (4);
     Write ('ษออออออออAAออออออออออBBออออออออออCCออออออออปษอXTRAอป');
     for Count := 2 to 22 do
     begin
          GoToXY (1+Offset_X, Count);
          Write ('บ');
          GoToXY (44+Offset_X, Count);
          Write ('บ');
     end;
     GoToXY (1+Offset_X, 23);
     Write ('ศออออออออIIออออออออออHHออออออออออGGออออออออผ');
     for Count := 2 to 4 do
     begin
          GoToXY (45+Offset_X, Count);
          Write ('บ');
          GoToXY (52+Offset_X, Count);
          Write ('บ');
     end;
     GoToXY (45+Offset_X, 5);
     Write ('ศออออออผ');
     GoToXY (1+Offset_X, 6);
     Write ('L');
     GoToXY (44+Offset_X, 6);
     Write ('D');
     GoToXY (1+Offset_X, 12);
     Write ('K');
     GoToXY (44+Offset_X,12);
     Write ('E');
     GoToXY (1+Offset_X, 18);
     Write ('J');
     GoToXY (44+Offset_X, 18);
     Write ('F');
     {Now, output the score board}
     GoToXY (59, 6);
     WriteLn ('ษอออออSCORESอออออป');
     for Count := 7 to 10 do
     begin          {69}
          GoToXY (59, Count);
          Write ('บ');
          if ((Count - 6 in [1,2]) and not FourPlayer) or FourPlayer then begin
          case TeamPlay of
               FALSE : case Count-6 of
                       1 : TextColor(12);
                       2 : TextColor(9);
                       3 : TextColor(14);
                       4 : TextColor(10);
                       end;
               TRUE  : case Count-6 of
                       1 : TextColor(12);
                       2 : TextColor(9);
                       3 : TextColor(12);
                       4 : TextColor(9);
                       end;
          end;
          Write (Count-6, '- ', PlayerNames[Count-6]);
          for ScoreBob := 1 to 10-Length(PlayerNames[Count-6]) do
              Write (' ');
          Write (':', PlayerScores[Count-6]:2);
          end
          else Write ('                ');
          TextColor (4);
          Write ('บ');
     end;
     GoToXY (59, 11);
     WriteLn ('ศออออออออออออออออผ');
     {Now, draw every map tile:}
     for CountX := 1 to 7 do
         for CountY := 1 to 7 do
             begin
                  Textcolor (7);
                  if not(Odd(countX) and Odd(CountY))
                     then Textcolor (8);
                  if PossibleMoves[CountX, CountY] = 1
                     then Textcolor (14);
                  GoToXY (-4+6*CountX+Offset_X,4+3*(7-CountY));
                  Write ('ÛÛ');
                  if maze[CountX, CountY, 3] = 1
                     then Write ('  ')
                     else Write ('ÛÛ');
                  Write ('ÛÛ');
                  GoToXY (-4+6*CountX+Offset_X,3+3*(7-CountY));
                  if maze [CountX, CountY, 4] = 1
                     then Write ('  ')
                     else Write ('ÛÛ');
                  Write ('  ');
                  if maze [CountX, CountY, 2] = 1
                     then Write ('  ')
                     else Write ('ÛÛ');
                  GoToXY (-4+6*CountX+Offset_X,2+3*(7-CountY));
                  Write ('ÛÛ');
                  if maze[CountX, CountY, 1] = 1
                     then Write ('  ')
                     else Write ('ÛÛ');
                  Write ('ÛÛ');
             end;
     GoToXY (46+Offset_X,4);
     Write ('ÛÛ');
     if maze[8, 1, 3] = 1
        then Write ('  ')
        else Write ('ÛÛ');
     Write ('ÛÛ');
     GoToXY (46+Offset_X,3);
     if maze [8, 1, 4] = 1
        then Write ('  ')
        else Write ('ÛÛ');
     Write ('  ');
     if maze [8, 1, 2] = 1
        then Write ('  ')
        else Write ('ÛÛ');
     GoToXY (46+Offset_X,2);
     Write ('ÛÛ');
     if maze[8, 1, 1] = 1
        then Write ('  ')
        else Write ('ÛÛ');
     Write ('ÛÛ');
     {Next, draw the players:}
     for Count := 1 to 4 do
         begin
              if not TeamPlay
                 then case Count of
                           1 : TextColor(12);
                           2 : TextColor(9);
                           3 : TextColor(14);
                           4 : TextColor(10);
                      end;
              if TeamPlay
                 then case Count of
                           1 : TextColor(12);
                           2 : TextColor(9);
                           3 : TextColor(12);
                           4 : TextColor(9);
                      end;
              if PlayerX[count] <> 0
              then begin
                   GoToXY (-2+6*PlayerX[Count]+Offset_X,3+3*(7-PlayerY[Count]));
                   Write ('',Count );
              end;
         end;
     {Now, put in the player objectives:}
     for CountX := 1 to 7 do
         for CountY := 1 to 7 do
             if PlayerCoins [CountX, CountY] > 0 then
                begin
                     case PlayerCoins [CountX, CountY] of
                          1 : TextColor(12);
                          2 : TextColor(9);
                          3 : TextColor(14);
                          4 : TextColor(10);
                     end;
                     GoToXY (-2+6*CountX+Offset_X,3+3*(7-CountY));
                     case PlayerCoins [CountX, CountY] of
                          1 : Write ('้');
                          2 : Write ('่');
                          3 : Write ('');
                          4 : Write ('๊');
                     end;
                end;
end; {DrawBoard}
{============================================================================}
{== Tile Sliding Procedures =================================================}
{============================================================================}
procedure SlideDown (var map : tileframe; X : integer; var PlayerX, PlayerY : location);
          {pre: X is 2,4 or 6
          post: column 2,4 or 6 is slid down one piece}
var CountY, CountDir : integer;
begin
     {First, move the bottom most piece into the placeholder spot:}
     for CountDir := 1 to 4 do
         map[8,2,CountDir] := map[X, 1, CountDir];
     {Then, move the rest of the stack down:}
     for CountY := 1 to 7 do
         for CountDir := 1 to 4 do
             map [X, CountY, CountDir] := map [X, CountY+1, CountDir];
     {Now, place the spare piece at the top:}
     for CountDir := 1 to 4 do
         map[X, 7, CountDir] := map [8, 1, CountDir];
     {Finally, move the placeholder piece to the spare piece:}
     for CountDir := 1 to 4 do
         map[8,1,CountDir] := map[8,2,CountDir];
     {And clear the placeholder to prevent weird things:}
     for CountDir := 1 to 4 do
         map[8,2,CountDir] := 0;
     {Move players}
     for CountY := 1 to 4 do
         begin
              if (PlayerX[CountY] = X) and (PlayerY[CountY] = 1)
              then PlayerY[CountY] := 7
              else if (PlayerX[CountY] = X) and (PlayerY[CountY] > 1) then
                      PlayerY[CountY] := PlayerY[CountY] - 1;
         end;
end; {SlideDown}
{----------------------------------------------------------------------------}
procedure SlideUp (var map : tileframe; X : integer; var PlayerX, PlayerY : location);
          {pre: X is 2,4 or 6
          post: column 2,4 or 6 is slid up one piece}
var CountY, CountDir : integer;
begin
     {First, move the top piece into the placeholder spot:}
     for CountDir := 1 to 4 do
         map[8,2,CountDir] := map[X, 7, CountDir];
     {Then, move the rest of the stack up:}
     for CountY := 7 downto 1 do
         for CountDir := 1 to 4 do
             map [X, CountY, CountDir] := map [X, CountY-1, CountDir];
     {Now, place the spare piece at the bottom:}
     for CountDir := 1 to 4 do
         map[X, 1, CountDir] := map [8, 1, CountDir];
     {Finally, move the placeholder piece to the spare piece:}
     for CountDir := 1 to 4 do
         map[8,1,CountDir] := map[8,2,CountDir];
     {And clear the placeholder to prevent weird things:}
     for CountDir := 1 to 4 do
         map[8,2,CountDir] := 0;
     for CountY := 1 to 4 do
         begin
              if (PlayerX[CountY] = X) and (PlayerY[CountY] = 7)
              then PlayerY[CountY] := 1
              else if (PlayerX[CountY] = X) and (PlayerY[CountY] < 7) then
                      PlayerY[CountY] := PlayerY[CountY] + 1;
         end;
end; {SlideUp}
{----------------------------------------------------------------------------}
procedure SlideRight (var map : tileframe; Y : integer; var PlayerX, PlayerY : location);
          {pre: Y is 2,4 or 6
          post: column 2,4 or 6 is slid right one piece}
var CountX, CountDir : integer;
begin
     {First, move the rightmost piece into the placeholder spot:}
     for CountDir := 1 to 4 do
         map[8,2,CountDir] := map[7, Y, CountDir];
     {Then, move the rest of the stack right:}
     for CountX := 7 downto 1 do
         for CountDir := 1 to 4 do
             map [CountX, Y, CountDir] := map [CountX-1, Y, CountDir];
     {Now, place the spare piece at the left:}
     for CountDir := 1 to 4 do
         map[1, Y, CountDir] := map [8, 1, CountDir];
     {Finally, move the placeholder piece to the spare piece:}
     for CountDir := 1 to 4 do
         map[8,1,CountDir] := map[8,2,CountDir];
     {And clear the placeholder to prevent weird things:}
     for CountDir := 1 to 4 do
         map[8,2,CountDir] := 0;
     for CountX := 1 to 4 do
         begin
              if (PlayerY[CountX] = Y) and (PlayerX[CountX] = 7)
              then PlayerX[CountX] := 1
              else if (PlayerY[CountX] = Y) and (PlayerX[CountX] < 7) then
                      PlayerX[CountX] := PlayerX[CountX] + 1;
         end;
end; {SlideUp}
{----------------------------------------------------------------------------}
procedure SlideLeft (var map : tileframe; Y : integer; var PlayerX, PlayerY : location);
          {pre: Y is 2,4 or 6
          post: column 2,4 or 6 is slid left one piece}
var CountX, CountDir : integer;
begin
     {First, move the leftmost piece into the placeholder spot:}
     for CountDir := 1 to 4 do
         map[8,2,CountDir] := map[1, Y, CountDir];
     {Then, move the rest of the stack left:}
     for CountX := 1 to 7 do
         for CountDir := 1 to 4 do
             map [CountX, Y, CountDir] := map [CountX+1, Y, CountDir];
     {Now, place the spare piece at the right:}
     for CountDir := 1 to 4 do
         map[7, Y, CountDir] := map [8, 1, CountDir];
     {Finally, move the placeholder piece to the spare piece:}
     for CountDir := 1 to 4 do
         map[8,1,CountDir] := map[8,2,CountDir];
     {And clear the placeholder to prevent weird things:}
     for CountDir := 1 to 4 do
         map[8,2,CountDir] := 0;
     for CountX := 1 to 4 do
         begin
              if (PlayerY[CountX] = Y) and (PlayerX[CountX] = 1)
              then PlayerX[CountX] := 7
              else if (PlayerY[CountX] = Y) and (PlayerX[CountX] > 1) then
                      PlayerX[CountX] := PlayerX[CountX] - 1;
         end;
end; {SlideUp}
{----------------------------------------------------------------------------}
procedure ChooseSlide (var maze : tileframe; player : integer; TeamPlay : Boolean);
          {Pre: player is 1-4
          Post: player X chooses which column to slide, in which direction}
var entered : char;
begin
     case TeamPlay of
          FALSE : case Player of
                       1 : TextColor(12);
                       2 : TextColor(9);
                       3 : TextColor(14);
                       4 : TextColor(10);
                  end;
          TRUE  : case Player of
                       1 : TextColor(12);
                       2 : TextColor(9);
                       3 : TextColor(12);
                       4 : TextColor(9);
                  end;
     end;
     GoToXY (18, 24);
     Write ('Player ', player, ', choose which piece to slide.');
     repeat
           entered := readkey;
           {Convert all entries to uppercase:}
           if entered > 'L'
              then entered := chr(ord(entered) - (ord('a')-ord('A')));
     until entered in ['A','B','C','D','E','F','G','H','I','J','K','K','L'];
     case Entered of
        'A' : SlideDown (maze, 2, PlayerX, PlayerY);
        'B' : SlideDown (maze, 4, PlayerX, PlayerY);
        'C' : SlideDown (maze, 6, PlayerX, PlayerY);
        'D' : SlideLeft (maze, 6, PlayerX, PlayerY);
        'E' : SlideLeft (maze, 4, PlayerX, PlayerY);
        'F' : SlideLeft (maze, 2, PlayerX, PlayerY);
        'G' : SlideUp (maze, 6, PlayerX, PlayerY);
        'H' : SlideUp (maze, 4, PlayerX, PlayerY);
        'I' : SlideUp (maze, 2, PlayerX, PlayerY);
        'J' : SlideRight (maze, 2, PlayerX, PlayerY);
        'K' : SlideRight (maze, 4, PlayerX, PlayerY);
        'L' : SlideRight (maze, 6, PlayerX, PlayerY);
     end;
end; {ChooseSlide}
{============================================================================}
{== Possible movement generator and selector ================================}
{============================================================================}
procedure FindPossible (maze : tileframe; var Possiblemoves : possible;
                        PlayerCoins : possible; X, Y, Player : integer;
                        FourPlayer, TeamPlay : boolean);
          {Pre: X and Y are 1-7, Player is 1-4
          Post: all possible finishing positions are stored in PossibleMoves}
var found : boolean;
    CountX, CountY : integer;
begin
     Possiblemoves[X,Y] := 1;
     repeat
           Found := FALSE;
           for CountX := 1 to 7 do
               for CountY := 1 to 7 do
                   begin
                        if PossibleMoves[CountX, CountY] = 1
                           then if CountX-1 > 0
                           then if (Maze[CountX, CountY, 4] = 1) and
                                   (Maze[CountX-1, CountY, 2] = 1) and
                                   (PossibleMoves[CountX-1, CountY] = 0)
                                   then begin
                                        PossibleMoves[CountX-1, CountY] := 1;
                                        Found := TRUE;
                                        end;
                        if PossibleMoves[CountX, CountY] = 1
                           then if CountX+1 < 8
                           then if (Maze[CountX, CountY, 2] = 1) and
                                   (Maze[CountX+1, CountY, 4] = 1) and
                                   (PossibleMoves[CountX+1, CountY] = 0)
                                   then begin
                                        PossibleMoves[CountX+1, CountY] := 1;
                                        Found := TRUE;
                                        end;
                        if PossibleMoves[CountX, CountY] = 1
                           then if CountY-1 > 0
                           then if (Maze[CountX, CountY, 3] = 1) and
                                   (Maze[CountX, CountY-1, 1] = 1) and
                                   (PossibleMoves[CountX, CountY-1] = 0)
                                   then begin
                                        PossibleMoves[CountX, CountY-1] := 1;
                                        Found := True;
                                        end;
                        if PossibleMoves[CountX, CountY] = 1
                           then if CountY+1 < 8
                           then if (Maze[CountX, CountY, 1] = 1) and
                                   (Maze[CountX, CountY+1, 3] = 1) and
                                   (PossibleMoves[CountX, CountY+1] = 0)
                                   then begin
                                        PossibleMoves[CountX, CountY+1] := 1;
                                        Found := True;
                                        end;
                   end;
     until found = false;
     for CountX := 1 to 4 do
         PossibleMoves[PlayerX[CountX], PlayerY[CountX]] := 0;
     PossibleMoves [X, Y] := 1;
     case TeamPlay of
          FALSE : for CountX := 1 to 7 do
                      for CountY := 1 to 7 do
                          if ((PlayerCoins[CountX, CountY] <> 0) and
                             (PlayerCoins[CountX, CountY] <> Player))
                             then PossibleMoves[CountX, CountY] := 0;
          TRUE  : for CountX := 1 to 7 do
                      for CountY := 1 to 7 do
                          if (PlayerCoins[CountX, CountY] <> 0) and
                             ((PlayerCoins[CountX, CountY] <> Player) and
                             (PlayerCoins[CountX, CountY] <> Player - 2))
                             then PossibleMoves[CountX, CountY] := 0;
     end;
     ClrScr;
     DrawBoard(Maze, PossibleMoves, PlayerX, PlayerY, FourPlayer);
end; {FindPossible}
{----------------------------------------------------------------------------}
procedure ChoosePossible (maze : tileframe; possiblemoves : possible;
                         var PlayerX, PlayerY : location; Player : integer;
                         TeamPlay : Boolean);
          {Pre: Player is 1-4,  PlayerX,Y is 1-7, Possiblemoves has at least
                one element set to 1
          Post: the player has chosen a place to move, which is stored in
                PlayerX,Y}
var CursorX, CursorY : integer;
    KeyPressed : Char;
    {- - - - - - - -}
    procedure RedrawCursor (CursorX, CursorY : integer;
                           PlayerX, PlayerY : Location;
                           PossibleMoves : Possible;
                           Maze : Tileframe);
    begin
         ClrScr;
         DrawBoard(Maze, PossibleMoves, PlayerX, PlayerY, FourPlayer);
         TextColor (18);
         if PossibleMoves[CursorX, CursorY] = 0
            then TextColor (20);
         GoToXY (-4+6*CursorX+Offset_X,2+3*(7-CursorY));
         Write ('Û');
         GoToXY (1+6*CursorX+Offset_X,2+3*(7-CursorY));
         Write ('Û');
         GoToXY (-4+6*CursorX+Offset_X,4+3*(7-CursorY));
         Write ('Û');
         GoToXY (1+6*CursorX+Offset_X,4+3*(7-CursorY));
         Write ('Û');
    end;
    {- - - - - - - -}
    procedure MoveCursor (var CursorX, CursorY : integer;
                         KeyPressed : char);
    begin
         if (KeyPressed in ['8','H']) and (CursorY < 7)
            then CursorY := CursorY + 1;
         if (KeyPressed in ['2','P']) and (CursorY > 1)
            then CursorY := CursorY - 1;
         if (KeyPressed in ['4','K']) and (CursorX > 1)
            then CursorX := CursorX - 1;
         if (KeyPressed in ['6','M']) and (CursorX < 7)
            then CursorX := CursorX + 1;
    end;
    {= = = = = = = =}
begin
     CursorX := PlayerX[Player];
     CursorY := PlayerY[Player];
     RedrawCursor (CursorX, CursorY, PlayerX, PlayerY, Possiblemoves, Maze);
     case TeamPlay of
          FALSE : case Player of
                       1 : TextColor(12);
                       2 : TextColor(9);
                       3 : TextColor(14);
                       4 : TextColor(10);
                  end;
          TRUE  : case Player of
                       1 : TextColor(12);
                       2 : TextColor(9);
                       3 : TextColor(12);
                       4 : TextColor(9);
                  end;
     end;
     GoToXY (18, 24);
     Write ('Player ', player, ', choose where you will move.');
     repeat
     KeyPressed := ReadKey;
     if KeyPressed = ' '
        then KeyPressed := ReadKey;
     MoveCursor (CursorX, CursorY, KeyPressed);
     RedrawCursor (CursorX, CursorY, PlayerX, PlayerY, Possiblemoves, Maze);
     case TeamPlay of
          FALSE : case Player of
                       1 : TextColor(12);
                       2 : TextColor(9);
                       3 : TextColor(14);
                       4 : TextColor(10);
                  end;
          TRUE  : case Player of
                       1 : TextColor(12);
                       2 : TextColor(9);
                       3 : TextColor(12);
                       4 : TextColor(9);
                  end;
     end;
     GoToXY (18, 24);
     Write ('Player ', player, ', choose where you will move.');
     until (KeyPressed = Chr(13)) and (PossibleMoves[CursorX, CursorY] = 1);
     PlayerX[Player] := CursorX;
     PlayerY[Player] := CursorY;
     if (PlayerCoins[PlayerX[Player], PlayerY[Player]] = Player) or
     ((TeamPlay = TRUE) and (PlayerCoins[PlayerX[Player], PlayerY[Player]] = Player - 2)
     and (PlayerCoins[PlayerX[Player], PlayerY[Player]] <> 0))
        then begin
                  PlayerCoins[PlayerX[Player], PlayerY[Player]] := 0;
                  PlayerScores[Player] := PlayerScores[Player] + 1;
             end;
end; {Choosepossible}
{============================================================================}
{== Miscellaneous functions =================================================}
{============================================================================}
function Check_Game_Over (PlayerCoins : possible; FourPlayer, TeamPlay : boolean) : boolean;
         {Pre: None
         Post: Returns the player that has no elements of his number in PlayerCoins
         NOTE: Based on game rules, only one player should EVER have no entires
               in playercoins}
var CountX, CountY, CountP, Score, Exit, Temp : integer;
begin
     case FourPlayer of
          True : temp := 4;
          False : temp := 2;
     end;
     if TeamPlay then
        temp := 2;
     Exit := 0;
     for CountP := 1 to temp do
     begin
          Score := 0;
          for CountX := 1 to 7 do
              for CountY := 1 to 7 do
                  if PlayerCoins [CountX, CountY] = CountP then
                     Score := Score + 1;
          if Score = 0 then
             Exit := Exit + 1;
     end;
     if Exit > 0
        then Check_Game_Over := TRUE
        else Check_Game_Over := FALSE;
end; {Check_Game_Over}
{----------------------------------------------------------------------------}
function Winner (PlayerCoins : possible; FourPlayer : Boolean) : integer;
         {Pre: none
         Post: Returns which player has no entries in PlayerCoins (Sister function
               on Check_Game_Over}
var CountX, CountY, CountP, Score, Temp : integer;
begin
     case FourPlayer of
          TRUE : Temp := 4;
          FALSE : Temp := 2;
     end;
     if TeamPlay
        then Temp := 2;
     for CountP := 1 to Temp do
     begin
          Score := 0;
          for CountX := 1 to 7 do
              for CountY := 1 to 7 do
                  if PlayerCoins [CountX, CountY] = CountP then
                     Score := Score + 1;
          if Score = 0 then
             Winner := CountP;
     end;
end; {Winner}
{----------------------------------------------------------------------------}
procedure GameOptions (var FourPlayer, SixTreasures, TeamPlay : boolean);
var blank : char;
begin
     TextColor (7);
     Write ('Do you want 2 players or 4 players? (Answer 2 or 4) ');
     repeat
           blank := readkey;
     until blank in ['2','4'];
     WriteLn (blank);
     case blank of
          '2' : FourPlayer := FALSE;
          '4' : FourPlayer := TRUE;
     end;
     if not FourPlayer
     then begin
          TeamPlay := FALSE;
          Write ('Do you want to get 3 treasures or 6 treasures? (Answer 3 or 6) ');
          repeat
                blank := readkey;
          until blank in ['3','6'];
          WriteLn (blank);
          case blank of
               '3' : SixTreasures := FALSE;
               '6' : SixTreasures := TRUE;
          end;
     end;
     if FourPlayer
     then begin
          SixTreasures := FALSE;
          Write ('Do you want teams or free for all? (Answer T or F) ');
          repeat
                blank := readkey;
                if blank = 'f' then blank := 'F';
                if blank = 't' then blank := 'T';
          until blank in ['F','T'];
          WriteLn (blank);
          case blank of
               'F' : TeamPlay := FALSE;
               'T' : TeamPlay := TRUE;
          end;
     end;
end;
{============================================================================}
{== And, finally, the main program ==========================================}
{============================================================================}
begin
     Randomize;
     TextBackGround (0);
     TextColor (7);
     for PlayerCount := 1 to 4 do
         PlayerScores [PlayerCount] := 0;
     PlayerNames[1] := 'AL';
     PlayerNames[2] := 'Bob';
     PlayerNames[3] := 'Brett';
     PlayerNames[4] := 'Chris';
     ClrScr;
     GameOptions (FourPlayer, SixTreasures, TeamPlay);
     Offset_X := 14;
     ClearMazeTiles(Maze);
     SetConstantTiles(Maze);
     SetPlayerCoins (Playercoins, FourPlayer, SixTreasures);
     SetVariableTiles(Maze);
     SetPlayerLocations(PlayerX, PlayerY, FourPlayer, TeamPlay);
     PlayerCount := 0;
     repeat
           PlayerCount := PlayerCount + 1;
           if PlayerCount = 5 then
              PlayerCount := 1;
           if (PlayerCount > 2) and not (FourPlayer)
              then PlayerCount := 1;
           ClearPossibleMoves (Possiblemoves);
           DrawBoard(Maze, PossibleMoves, PlayerX, PlayerY, FourPlayer);
           ChooseSlide (Maze, PlayerCount, TeamPlay);
           FindPossible (Maze, PossibleMoves, PlayerCoins, PlayerX[PlayerCount],
                         PlayerY[PlayerCount], PlayerCount, FourPlayer, TeamPlay);
           ChoosePossible (maze, possiblemoves, PlayerX, PlayerY, PlayerCount, TeamPlay);
     until Check_Game_Over (PlayerCoins, FourPlayer, TeamPlay);
     ClrScr;
     GoToXY (32, 12);
     case Winner(PlayerCoins, FourPlayer) of
          1 : TextColor(12);
          2 : TextColor(9);
          3 : TextColor(14);
          4 : TextColor(10);
     end;
     if TeamPlay
        then Write ('Team ', Winner(PlayerCoins, FourPlayer), ' wins!!!')
        else Write ('Player ', Winner(PlayerCoins, FourPlayer), ' wins!!!');
     ReadLn;
end.
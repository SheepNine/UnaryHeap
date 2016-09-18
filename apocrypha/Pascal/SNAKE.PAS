program SnakeGame (input, output);
uses Crt;
type SegmentPtr = ^Segment;
     Segment = record
                     X, Y : integer;
                     NextSeg : SegmentPtr;
               end;
     DotSpot = record
                     X, Y : integer;
               end;
     Direction = (Up, Down, Left, Right);
var
   SnakeLength, SnakeLTotal, Score, Count : integer;
   Snake, Food : SegmentPtr;
   SnakeFacing : Direction;
   GameOver, SoundOn : Boolean;
   X : Char;
{----------------------------------------------------------------------------}
procedure DrawBoard (Snake : SegmentPtr; Food : SegmentPtr; Gameover : Boolean);
var X,Y, PrevX, PrevY, PrevD, NextD : integer;
    Temp : SegmentPtr;
begin
     ClrScr;
     TextColor (8);
     GoToXY (1,1);
     TextColor (7);
     if GameOver
        then TextColor (4);
     Write ('л(', Snake^.X:2, ',', Snake^.Y:2, ')лллллллллллллллллллллллллллллллллллллллллллллллллллллллллллллллллллллллл');
     for Y := 2 to 23 do
         begin
              GoToXY (1, Y);
              Write ('л');
              GoToXY (80, Y);
              Write ('л');
         end;
     GoToXY (1,24);
     Write ('лллллллллллллллллллллллллллллллллллллллллллллллллллллллллллллллллллллллллллллллл');
     TextColor (15);
     Write ('Length : ',SnakeLength: 2,'    Score : ', Score : 6);
     Temp := Snake;
     PrevX := 0;
     PrevY := 0;
     PrevD := 0;
     repeat
           TextColor (2);
           GoToXY (Temp^.X, Temp^.Y);
           if Temp^.X > PrevX
              then PrevD := 4;
           if Temp^.X < PrevX
              then PrevD := 2;
           if Temp^.Y > PrevY
              then PrevD := 3;
           if Temp^.Y < PrevY
              then PrevD := 1;
           if Temp^.NextSeg <> NIL
              then begin
                  if Temp^.X > Temp^.NextSeg^.X
                     then NextD := 4;
                  if Temp^.X < Temp^.NextSeg^.X
                     then NextD := 2;
                  if Temp^.Y > Temp^.NextSEg^.Y
                     then NextD := 3;
                  if Temp^.Y < Temp^.NextSEg^.Y
                     then NextD := 1;
                   end
                   else NextD := 0;
           case PrevD of
                0: Write ('*');
                1: case NextD of
                   0 : Write ('*');
                   2 : Write ('Щ');
                   3 : Write ('К');
                   4 : Write ('Л');
                   end;
                2: case NextD of
                   0 : Write ('*');
                   1 : Write ('Щ');
                   3 : Write ('Ш');
                   4 : Write ('Э');
                   end;
                3: case NextD of
                   0 : Write ('*');
                   1 : Write ('К');
                   2 : Write ('Ш');
                   4 : Write ('М');
                   end;
                4: case NextD of
                   0 : Write ('*');
                   1 : Write ('Л');
                   2 : Write ('Э');
                   3 : Write ('М');
                   end;
           end;
           PrevX := Temp^.X;
           PrevY := Temp^.Y;
           Temp := Temp^.NextSeg;
     until Temp = Nil;
     GoToXY (Snake^.X, Snake^.Y);
     if GameOver
        Then TextColor (12);
     Write ('');
     GoToXY (Food^.X, Food^.Y);
     TextColor (9);
     Write ('щ');
     GoToXY (1,1);
end; {DrawBoard}
{----------------------------------------------------------------------------}
procedure MoveSegment (var Snake : SegmentPtr; SnakeFacing : Direction;
                      var GameOver : Boolean);
          {Pre : Snake is not nil
          Post : The last element in list Snake is moved to the first element}
var Moving, PreMoving, Checking : SegmentPtr;
begin
     Moving := Snake^.NextSeg;
     PreMoving := Snake;
     while Moving^.NextSeg <> NIL do
           begin
                PreMoving := Premoving^. NextSeg;
                Moving := Moving^.NextSeg;
           end;
     Moving^.NextSeg := Snake;
     Snake := Moving;
     PreMoving^.NextSeg := NIL;
     Snake^.X := Snake^.NextSeg^.X;
     Snake^.Y := Snake^.NextSEg^.Y;
     case SnakeFacing of
          Up    : Snake^.Y := Snake^.NextSeg^.Y - 1;
          Down  : Snake^.Y := Snake^.NextSeg^.Y + 1;
          Left  : Snake^.X := Snake^.NextSeg^.X - 1;
          Right : Snake^.X := Snake^.NextSeg^.X + 1;
     end;
     if (Snake^.X = Food^.X) and (Snake^.Y = Food^.Y)
        then begin
                  if SoundOn
                  then begin
                            Sound(1000);
                            Delay (50);
                            Sound(1100);
                            Delay (50);
                            Sound (1200);
                            Delay (50);
                            Sound (1300);
                            Delay (50);
                            NoSound;
                       end;
                  Score := Score + 100 + SnakeLTotal * 5;
                  Food^.X := random (78) + 2;
                  Food^.Y := random (22) + 2;
                  Food^.NextSeg := NIL;
                  SnakeLTotal := SnakeLTotal + Random(7) + 1;
             end;
     Checking := Snake^.NextSeg;
     while Checking^.NextSeg <> NIL do
           begin
                if (Checking^.X = Snake^.X) and (Checking^.Y = Snake^.Y)
                   then GameOver := TRUE;
                Checking := Checking^.NextSeg;
           end;
     if Snake^.Y = 1 then GameOver := TRUE;
     if Snake^.Y = 24 then GameOver := TRUE;
     if Snake^.X = 1 then GameOver := TRUE;
     if Snake^.X = 80 then GameOver := TRUE;
end; {MoveSegment}
{----------------------------------------------------------------------------}
procedure AddSegment (var snake : SegmentPtr; var SnakeLength : integer);
          {Pre: Snake is not NIL
          Post: Another segment has been added to snake}
var Moving : SegmentPtr;
begin
     SnakeLength := SnakeLength + 1;
     Moving := Snake;
     while Moving^.NextSeg <> NIL do
           Moving := Moving^.NextSeg;
     New (Moving^.NextSeg);
     Moving^.NextSeg^.X := Moving^.X;
     Moving^.NextSeg^.Y := Moving^.X;
     Moving^.NextSeg^.NextSeg := NIL;
end;
{----------------------------------------------------------------------------}
procedure InitVariables (var GameOver : Boolean; var Food, Snake : SegmentPtr;
                        var SnakeLength, SnakeLTotal, Score : integer;
                        var SnakeFacing : direction; var SoundOn : Boolean);
begin
     GameOver := FALSE;
     New (Food);
     Food^.X := random (60) + 10;
     Food^.Y := random (10) + 5;
     Food^.NextSeg := NIL;
     New (Snake);
     Snake^.X := 40;
     Snake^.Y := 12;
     New (Snake^.NextSeg);
     Snake^.NextSeg^.X := 40;
     Snake^.NextSeg^.Y := 13;
     New (Snake^.NextSeg^.NextSeg);
     Snake^.NextSeg^.NextSeg^.X := 40;
     Snake^.NextSeg^.NextSeg^.Y := 14;
     Snake^.NextSeg^.NextSeg^.NextSeg := NIL;
     Score := 0;
     SnakeLength := 3;
     SnakeLTotal := 3;
     SnakeFacing := Up;
     SoundOn := FALSE;
end; {InitVariables}
{============================================================================}
begin
     Randomize;
     InitVariables (GameOver, Food, Snake, SnakeLength, SnakeLTotal, Score, SnakeFacing, SoundOn);
     {=-=-=-=GAME START=-=-=-=}
     repeat
           DrawBoard(Snake, Food, GameOver);
           Delay (75);
           if SnakeLTotal > SnakeLength
              then AddSegment (Snake, SnakeLength);
           MoveSegment (Snake, SnakeFacing, GameOver);
           for count := 1 to 10 do
           begin
               if KeyPressed then X := readkey;
               if X = ' ' then X := readkey;
               case X of
                        'H','h' : if SnakeFacing <> down
                                     then SnakeFacing := up;
                        'P','p' : if SnakeFacing <> up
                                     then SnakeFacing := down;
                        'M','m' : if SnakeFacing <> left
                                     then SnakeFacing := right;
                        'K','k' : if SnakeFacing <> right
                                     then SnakeFacing := left;
               end;
           end;
     until (X in ['Q','q']) or GameOver;
     DrawBoard(Snake, Food, GameOver);
     GoToXY (40-17 div 2, 12);
     Textcolor (12);
     Write ('G A M E   O V E R');
     ReadLn;
     {=-=-=-=GAME END=-=-=-=}
end.
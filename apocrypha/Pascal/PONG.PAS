program Ping (input, output);
uses Crt;
type up_down = (up, down);
     left_right = (left, right);
var PaddleH, Count, PaddleC, BallX, BallY, BallTime, Level : integer;
    BallV : left_right;
    BallH : up_down;
{----------------------------------------------------------------------------}
procedure Paddle_Refresh (PaddleH, PaddleC : integer);
begin
     TextBackGround (1);
     GoToXY (1,1);
     Write ('                                                                               ');
     GoToXY (PaddleC, 1);
     Write ('ÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜ');
     GoToXY (1,25);
     Write ('                                                                               ');
     GoToXY (PaddleH, 25);
     Write ('ßßßßßßßßßßßßßßßßßßßß');
end;
{----------------------------------------------------------------------------}
procedure Ball_Refresh (BallX, BallY : integer);
begin
     GoToXY (BallX, BallY);
     Write ('ÛÛ');
end;
{----------------------------------------------------------------------------}
procedure Move_Ball (var BallX, BallY : integer;
                     var BallH : up_down;
                     var BallV : left_right);
begin
     if BallX = 1 then
        BallV := right;
     if BallX = 79 then
        BallV := left;
     if BallY = 2 then
        BallH := down;
     if BallY = 24 then
        BallH := up;
     case BallH of
          up : BallY := BallY - 1;
          down : BallY := BallY + 1;
     end;
     case BallV of
          left : BallX := BallX - 2;
          right : BallX := BallX + 2;
     end;
end;
{----------------------------------------------------------------------------}
procedure Move_Player_Paddle (var PaddleH : integer);
var Entered : Char;
begin
     Entered := 's';
     if KeyPressed
        then Entered := ReadKey;
     if Entered = ' '
        then Entered := ReadKey;
     Case Entered of
          'K' : if PaddleH > 2
                   then PaddleH := PaddleH - 1;
          'M' : if PaddleH < 60
                   then PaddleH := PaddleH + 1;
     end;
end;
{----------------------------------------------------------------------------}
procedure Move_Computer_Paddle (var PaddleC : integer; BallX : integer);
begin
     if (BallX > PaddleC + 10) and (PaddleC < 60)
        then PaddleC := PaddleC + 1;
     if (BallX < PaddleC + 10) and (PaddleC > 2)
        then PaddleC := PaddleC - 1;
end;
{============================================================================}
begin
     Level := 1;
     BallX := 3;
     BallY := 3;
     BallH := down;
     BallV := right;
     PaddleH := 40;
     PaddleC := 25;
     repeat
           for count := 1 to 10 do
               begin
                    Move_Player_Paddle (PaddleH);
                    if Count in [10]
                       then Move_Computer_Paddle (PaddleC, BallX);
                    delay (10 - Level);
                    Paddle_Refresh (PaddleH, PaddleC);
               end;
           ClrScr;
           Move_Ball (BallX, BallY, BallH, BallV);
           Ball_Refresh (BallX, BallY);
           Paddle_Refresh (PaddleH, PaddleC);
     until 0 > 1
end.
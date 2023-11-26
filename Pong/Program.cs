using System.Numerics;
using System.Runtime.InteropServices;
using Raylib_cs;

namespace Pong;

class Program
{
    private static int _p1Pos;
    private static int _p2Pos;
    private static (int X, int Y) ballPos;
    private static (int X, int Y) ballSpeed;
    private static int ballAngle = 0;
    private static bool _gameStarted;
    
    private const int PlayerWidth = 15;
    private const int PlayerHeight = 75;
    private const float PlayerSpeed = 1;
    private const int BallSize = 10;
    private const int screenWidth = 858;
    private const int screenHeight = 525;
    private const int FPS = 30;

    private const KeyboardKey p1UP = KeyboardKey.KEY_W;
    private const KeyboardKey p1DOWN = KeyboardKey.KEY_S;
    private const KeyboardKey p2UP = KeyboardKey.KEY_UP;
    private const KeyboardKey p2DOWN = KeyboardKey.KEY_DOWN;

    private static Font _silkFont = Raylib.LoadFont("resources/slkscre.ttf");
    
    public static void Main()
    {
        Raylib.InitWindow(858, 525, "PONG");
        SetDefaultGameState();
        Raylib.SetTargetFPS(FPS);
        
        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);

            if (!_gameStarted)
            {
                Raylib.DrawTextEx(_silkFont, "PONG", new Vector2(screenWidth / 2 - 130, 150), 90, 15, Color.WHITE);
                if (Raylib.IsKeyDown(p1DOWN) || Raylib.IsKeyDown(p1UP) || Raylib.IsKeyDown(p2DOWN) || Raylib.IsKeyDown(p2UP))
                {
                    _gameStarted = true;
                    Random rand = new Random();
                    ballAngle = rand.Next(45, 135) * (rand.Next(0, 2) * 2 - 1);
                    BallAngleToSpeed();
                    //Console.WriteLine($"{ballSpeed.X}, {ballSpeed.Y}, {ballAngle}");
                }
            }
            else
            {
                //Get direction of player movement
                int p1Movement = Convert.ToInt32((bool)Raylib.IsKeyDown(p1DOWN)) - Convert.ToInt32((bool)Raylib.IsKeyDown(p1UP));
                int p2Movement = Convert.ToInt32((bool)Raylib.IsKeyDown(p2DOWN)) - Convert.ToInt32((bool)Raylib.IsKeyDown(p2UP));
                
                //Move the players
                _p1Pos += (int)Math.Round(p1Movement * PlayerSpeed * 5);
                _p2Pos += (int)Math.Round(p2Movement * PlayerSpeed * 5);
                
                //Check if a player is out of bounds
                if (_p1Pos < 0) { _p1Pos = 0;}
                else if (_p1Pos > screenHeight - PlayerHeight) { _p1Pos = screenHeight - PlayerHeight;}
                if (_p2Pos < 0) { _p2Pos = 0;}
                else if (_p2Pos > screenHeight - PlayerHeight) { _p2Pos = screenHeight - PlayerHeight;}
                
                //Move the ball
                ballPos.X += ballSpeed.X;
                ballPos.Y += ballSpeed.Y;
                
                //Check if the ball hit an edge
                if (ballPos.Y > screenHeight - BallSize)
                {
                    ballSpeed.Y = -ballSpeed.Y;
                }
                else if (ballPos.Y < 0)
                {
                    ballSpeed.Y = -ballSpeed.Y;
                }
                
                //Check if the ball hit a player
                //  According to original PONG, a ball bounces 75deg when it hits the corner,
                //  0deg when it hits the middle
                //  In this version, I use 60deg, as signified by the *30f in the ugly equation
                if (ballPos.X < 10 + PlayerWidth && ballPos.Y > _p1Pos - BallSize && ballPos.Y < _p1Pos + PlayerHeight)
                {
                    ballPos.X = 10 + PlayerWidth;
                    Console.WriteLine("P1 hit");
                    int bounceCoeficient = ballPos.Y - _p1Pos + BallSize;
                    int relativeBounceoffAngle =
                        (int)Math.Round((
                            Math.Cos( 2 * Math.PI * bounceCoeficient / (PlayerHeight + BallSize) )
                             + 1) * 30f);     
                    if (ballAngle > 90)
                    {
                        ballAngle = relativeBounceoffAngle;
                    }
                    else
                    {
                        ballAngle = 90 + relativeBounceoffAngle;
                    }
                    BallAngleToSpeed();
                    Console.WriteLine(ballAngle);
                }
                else if (ballPos.X > screenWidth - 10 - PlayerWidth - BallSize && ballPos.Y > _p2Pos - BallSize && ballPos.Y < _p2Pos + PlayerHeight)
                {
                    ballPos.X = screenWidth - 10 - PlayerWidth - BallSize;
                    Console.WriteLine("P2 hit");
                    int bounceCoeficient = ballPos.Y - _p2Pos + BallSize;
                    int relativeBounceoffAngle =
                        (int)Math.Round((
                            Math.Cos( 2 * Math.PI * bounceCoeficient / (PlayerHeight + BallSize) )
                            + 1) * 30f);     
                    if (ballAngle < -90)
                    {
                        ballAngle = -relativeBounceoffAngle;
                    }
                    else
                    {
                        ballAngle = -90 - relativeBounceoffAngle;
                    }
                    BallAngleToSpeed();
                    Console.WriteLine(ballAngle);
                }
            }
            
            //DRAW PLAYERS & BALL
            Raylib.DrawRectangle(10, _p1Pos, PlayerWidth, PlayerHeight, Color.WHITE);
            Raylib.DrawRectangle(screenWidth - PlayerWidth - 10, _p2Pos, PlayerWidth, PlayerHeight, Color.WHITE);
            Raylib.DrawRectangle(ballPos.X, ballPos.Y, BallSize, BallSize, Color.WHITE);
            
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    static void SetDefaultGameState()
    {
        _p1Pos = screenHeight / 2 - PlayerHeight / 2;
        _p2Pos = screenHeight / 2 - PlayerHeight / 2;
        ballPos = (screenWidth / 2 - BallSize / 2, screenHeight / 2 - BallSize / 2);
        ballSpeed = (0, 0);
    }

    static void BallAngleToSpeed()
    {
        ballSpeed.X = (int)Math.Round(Math.Sin(ballAngle * 0.0175f) * 10);   //*0.0175 to convert to radians
        ballSpeed.Y = (int)Math.Round(-Math.Cos(ballAngle * 0.0175f) * 10);
    }
}
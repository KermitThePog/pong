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
    private const float BallSpeedModifier = 1;
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
                    ballAngle = rand.Next(0, 360);
                    ballSpeed.X = (int)Math.Round(Math.Sin(ballAngle * 0.0175f));   //*0.0175 to convert to rad
                    ballSpeed.Y = (int)Math.Round(-Math.Cos(ballAngle * 0.0175f));
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
                ballPos.X += (int)Math.Round(ballSpeed.X * BallSpeedModifier * 4);
                ballPos.Y += (int)Math.Round(ballSpeed.Y * BallSpeedModifier * 4);
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
        _gameStarted = false;
        _p1Pos = screenHeight / 2 - PlayerHeight / 2;
        _p2Pos = screenHeight / 2 - PlayerHeight / 2;
        ballPos = (screenWidth / 2 - BallSize / 2, screenHeight / 2 - BallSize / 2);
        ballSpeed = (0, 0);
    }
}
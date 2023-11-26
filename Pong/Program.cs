using System.Numerics;
using System.Runtime.InteropServices;
using Raylib_cs;

namespace Pong;

class Program
{
    private static int p1Pos;
    private static int p2Pos;
    private static (int X, int Y) ballPos;
    private static (int X, int Y) ballSpeed;
    private static int ballAngle;
    private static bool gameStarted;
    private static bool menu = true;
    private static int p1Score;
    private static int p2Score;
    
    private const int PlayerWidth = 15;
    private const int PlayerHeight = 75;
    private const float PlayerSpeed = 1;
    private const int BallSize = 10;
    private const int ScreenWidth = 858;
    private const int ScreenHeight = 525;
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

            if (!gameStarted)
            {
                if(menu) {Raylib.DrawTextEx(_silkFont, "PONG", 
                    new Vector2(ScreenWidth / 2 - 130, 150), 90, 15, Color.WHITE);}

                Raylib.DrawTextEx(_silkFont, "Press W/S/UP/DOWN to serve",
                    new Vector2(ScreenWidth / 2 - 265, ScreenHeight / 2 + BallSize * 3), 
                    20, 10, Color.LIGHTGRAY);
                Raylib.DrawTextEx(_silkFont, "Press R to restart the game",
                    new Vector2(ScreenWidth / 2 - 253, ScreenHeight / 2 + BallSize * 3 + 30), 
                    20, 10, Color.LIGHTGRAY);
                
                if (Raylib.IsKeyDown(p1DOWN) || Raylib.IsKeyDown(p1UP) || Raylib.IsKeyDown(p2DOWN) || Raylib.IsKeyDown(p2UP))
                {
                    gameStarted = true;
                    menu = false;
                    Random rand = new Random();
                    ballAngle = rand.Next(45, 135) * (rand.Next(0, 2) * 2 - 1);
                    BallAngleToSpeed();
                    //Console.WriteLine($"{ballSpeed.X}, {ballSpeed.Y}, {ballAngle}");
                }
            }
            else
            {
                //Check if the game is restarted
                if (Raylib.IsKeyDown(KeyboardKey.KEY_R))
                {
                    SetDefaultGameState();
                    gameStarted = false;
                    menu = true;
                    p1Score = 0;
                    p2Score = 0;
                }
                
                //Get direction of player movement
                int p1Movement = Convert.ToInt32((bool)Raylib.IsKeyDown(p1DOWN)) - Convert.ToInt32((bool)Raylib.IsKeyDown(p1UP));
                int p2Movement = Convert.ToInt32((bool)Raylib.IsKeyDown(p2DOWN)) - Convert.ToInt32((bool)Raylib.IsKeyDown(p2UP));
                
                //Move the players
                p1Pos += (int)Math.Round(p1Movement * PlayerSpeed * 5);
                p2Pos += (int)Math.Round(p2Movement * PlayerSpeed * 5);
                
                //Check if a player is out of bounds
                if (p1Pos < 0) { p1Pos = 0;}
                else if (p1Pos > ScreenHeight - PlayerHeight) { p1Pos = ScreenHeight - PlayerHeight;}
                if (p2Pos < 0) { p2Pos = 0;}
                else if (p2Pos > ScreenHeight - PlayerHeight) { p2Pos = ScreenHeight - PlayerHeight;}
                
                //Move the ball
                ballPos.X += ballSpeed.X;
                ballPos.Y += ballSpeed.Y;
                
                //Check if the ball hit an edge
                if (ballPos.Y is > ScreenHeight - BallSize or < 0)
                {
                    ballSpeed.Y = -ballSpeed.Y;
                    ballAngle = Math.Sign(ballAngle) * (90 - (Math.Abs(ballAngle) - 90));
                }
                
                //Check if the ball hit a player
                //  According to original PONG, a ball bounces 75deg when it hits the corner,
                //  0deg when it hits the middle
                //  In this version, I use 60deg, as signified by the *30f in the ugly equation
                //  Relative bounce-off angle is calculated in degrees, with 0 being horizontal
                if (ballPos.X < 10 + PlayerWidth && ballPos.Y > p1Pos - BallSize && ballPos.Y < p1Pos + PlayerHeight)
                {
                    ballPos.X = 10 + PlayerWidth;
                    //Console.WriteLine("P1 hit");
                    int bounceCoeficient = ballPos.Y - p1Pos + BallSize;
                    int relativeBounceoffAngle =
                        (int)Math.Round((
                            Math.Cos( 2 * Math.PI * bounceCoeficient / (PlayerHeight + BallSize) )
                             + 1) * 30f);   
                    Console.WriteLine($"relative {relativeBounceoffAngle}");
                    if (ballAngle < -90)
                    {
                        ballAngle = 90 + relativeBounceoffAngle;
                    }
                    else
                    {
                        ballAngle = 90 - relativeBounceoffAngle;
                    }
                    BallAngleToSpeed();
                    Console.WriteLine(ballAngle);
                }
                else if (ballPos.X > ScreenWidth - 10 - PlayerWidth - BallSize && ballPos.Y > p2Pos - BallSize && ballPos.Y < p2Pos + PlayerHeight)
                {
                    ballPos.X = ScreenWidth - 10 - PlayerWidth - BallSize;
                    //Console.WriteLine("P2 hit");
                    int bounceCoeficient = ballPos.Y - p2Pos + BallSize;
                    int relativeBounceoffAngle =
                        (int)Math.Round((
                            Math.Cos( 2 * Math.PI * bounceCoeficient / (PlayerHeight + BallSize) )
                            + 1) * 30f);    
                    Console.WriteLine($"relative {relativeBounceoffAngle}");
                    if (ballAngle > 90)
                    {
                        ballAngle = -90 - relativeBounceoffAngle;
                    }
                    else
                    {
                        ballAngle = -90 + relativeBounceoffAngle;
                    }
                    BallAngleToSpeed();
                    Console.WriteLine(ballAngle);
                }
                
                //Check if a ball scores
                if (ballPos.X > ScreenWidth-PlayerWidth && ballAngle > 0)
                {
                    p1Score++;
                    SetDefaultGameState();
                    gameStarted = false;
                    //Console.WriteLine(p1Score);
                }
                else if (ballPos.X < 0 && ballAngle < 0)
                {
                    p2Score++;
                    SetDefaultGameState();
                    gameStarted = false;
                    //Console.WriteLine(p2Score);
                }
                
            }
            
            //Draw players & ball
            Raylib.DrawRectangle(10, p1Pos, PlayerWidth, PlayerHeight, Color.WHITE);
            Raylib.DrawRectangle(ScreenWidth - PlayerWidth - 10, p2Pos, PlayerWidth, PlayerHeight, Color.WHITE);
            Raylib.DrawRectangle(ballPos.X, ballPos.Y, BallSize, BallSize, Color.WHITE);

            if (!menu)
            {
                //Draw scores
                Raylib.DrawTextEx(_silkFont, p1Score.ToString(),new Vector2(ScreenWidth/2 - 100, 50), 50, 5, Color.LIGHTGRAY);
                Raylib.DrawTextEx(_silkFont, p2Score.ToString(),new Vector2(ScreenWidth/2 + 100, 50), 50, 5, Color.LIGHTGRAY);
            }
            
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    static void SetDefaultGameState()
    {
        p1Pos = ScreenHeight / 2 - PlayerHeight / 2;
        p2Pos = ScreenHeight / 2 - PlayerHeight / 2;
        ballPos = (ScreenWidth / 2 - BallSize / 2, ScreenHeight / 2 - BallSize / 2);
        ballSpeed = (0, 0);
    }

    static void BallAngleToSpeed()
    {
        ballSpeed.X = (int)Math.Round(Math.Sin(ballAngle * 0.0175f) * 10);   //*0.0175 to convert to radians
        ballSpeed.Y = (int)Math.Round(-Math.Cos(ballAngle * 0.0175f) * 10);
        Console.WriteLine($"Ball angle is {ballAngle}");
    }
}
using System.Numerics;
using Raylib_cs;

namespace Pong;

class Program
{
    private static int _playerOnePos;
    private static int _playerTwoPos;
    private const int PlayerWidth = 20;
    private const int PlayerHeight = 50;
    private static bool _gameStarted = false;

    private static Font _silkFont = Raylib.LoadFont("resources/slkscre.ttf");
    
    public static void Main()
    {
        Raylib.InitWindow(858, 525, "Pong");

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);

            if (!_gameStarted)
            {
                Raylib.DrawTextEx(_silkFont, "PONG", new Vector2(0, 217), 90, 15, Color.WHITE);
            }
            
            //DRAW PLAYERS
            

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}
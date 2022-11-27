using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;  
namespace SnakeGame
{
    internal class Program
    {
        private const int MapWidth =15;
        private const int MapHeight =15;

        private const int ScreenWidth = MapWidth*5;
        private const int ScreenHeight = MapHeight*5;

        private const int FrameMs = 200;

        private const ConsoleColor BorderColor = ConsoleColor.Gray;

        private const ConsoleColor HeadColor = ConsoleColor.DarkGreen;
        private const ConsoleColor BodyColor = ConsoleColor.Green;
        private const ConsoleColor FoodColor = ConsoleColor.Red;
        

        private static readonly Random random = new Random();
        static void Main()
        {
            SetWindowSize(ScreenWidth, ScreenHeight);
            SetBufferSize(ScreenWidth, ScreenHeight);
            CursorVisible = false;
            while (true)
            {
                StartGame();
                Thread.Sleep(1000 );
                ReadKey();
            }
            

        }
        static void StartGame()
        {
            Clear();
            DrawBorder();
            Direction currentDirection = Direction.Right;
            var snake = new Snake(10, 5, HeadColor, BodyColor);

            Pixel food = GenFood(snake);
            food.Draw();
            int score = 0;

            int lagMs = 0;

            var sw = new Stopwatch();
            while (true)
            {
                sw.Restart();
                Direction oldMove = currentDirection;
                while (sw.ElapsedMilliseconds <= FrameMs- lagMs)
                {
                    if (currentDirection == oldMove)
                    {
                        currentDirection = ReadMovement(currentDirection);
                    }

                }

                sw.Restart();

                if (snake.Head.X==food.X&&snake.Head.Y ==food.Y)
                {
                    snake.Move(currentDirection, true);

                    food = GenFood(snake);
                    food.Draw();
                    score++;

                    Task.Run(() => Beep(600,200));
                }
                else
                {
                    snake.Move(currentDirection);
                }


                if (snake.Head.X == MapWidth - 1 || snake.Head.X == 0 || snake.Head.Y == MapHeight - 1 || snake.Head.Y == 0 || snake.Body.Any(b => b.X == snake.Head.X && b.Y == snake.Head.Y))
                    break;

                lagMs = (int)sw.ElapsedMilliseconds;
            }
            snake.Clear();
            food.Clear();
            SetCursorPosition(ScreenWidth / 3, ScreenHeight / 2);

            WriteLine($"Game Over, score: {score}");
            
            Task.Run(() => Beep(200, 600));
        }

        static Pixel GenFood(Snake snake)
        {
            Pixel food;
            do
            {
                food = new Pixel(random.Next(1,MapWidth-2),random.Next(1,MapHeight-2),FoodColor); 
            } while (snake.Head.X == food.X && snake.Head.Y == food.Y||snake.Body.Any(b=>b.X==food.X&&b.Y==food.Y));
            return food;
        }

        static Direction ReadMovement(Direction currentDirection)
        {
            if(!KeyAvailable)
                return currentDirection;
            ConsoleKey key = ReadKey(true).Key;
            currentDirection = key switch
            {

                ConsoleKey.UpArrow when currentDirection != Direction.Down => Direction.Up,
                ConsoleKey.DownArrow when currentDirection != Direction.Up => Direction.Down,
                ConsoleKey.RightArrow when currentDirection != Direction.Left => Direction.Right,
                ConsoleKey.LeftArrow when currentDirection != Direction.Right => Direction.Left,
                _ => currentDirection
            };
            return currentDirection;
        }
        static void DrawBorder()
        {
            for (int i = 0; i < MapWidth; i++)
            {
                new Pixel(i,0,BorderColor).Draw();
                new Pixel(i, MapHeight-1, BorderColor).Draw();
            }
            for (int i = 0; i < MapHeight; i++)
            {
                new Pixel(0, i, BorderColor).Draw();
                new Pixel(MapWidth-1, i, BorderColor).Draw();
            }
        }
    }
}

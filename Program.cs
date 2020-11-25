using System;
using System.Threading;
using System.Collections.Generic;
using timers = System.Timers;
using System.Text;

namespace SnakeCLI
{
    class Program
    {
        char floorChar = (char) 46;
        char snakeChar = (char) 35;
        char foodChar = (char) 64;

        private char[,] board;
        private int boardWidth;
        private int boardHeight;
        private int points;
        private Random random;
        private SpeedCurve speedCurve;

        private LinkedList<(int w, int h)> snake;
        private Heading heading;
        private Heading? requestHeading;

        private Thread detectPlayerInput;
        private timers.Timer t;

        public Program(int width, int height, int bombPct)
        {
            boardWidth = width;
            boardHeight = height;
            board = new char[boardWidth, boardHeight];

            points = 0;
            random = new Random();

            speedCurve = new SpeedCurve(50, 50, 400);

            for(int x = 0; x < boardWidth; x++) {
                for(int y = 0; y < boardHeight; y++) {
                    board[x,y] = floorChar;
                }
            }

            snake = new LinkedList<(int x, int y)>();
            snake.AddFirst((4,5));
            snake.AddLast((5,5));
            heading = Heading.EAST;
            requestHeading = null;

            SpawnChar(foodChar);

            ThreadStart ts = DetectInput;
            detectPlayerInput = new Thread(ts);
            detectPlayerInput.Start();

            t = new timers.Timer();
            t.Interval = speedCurve.CalculateY(points);
            t.Elapsed += Update;
            t.Start();

            Console.Clear();
        }

        public void DetectInput()
        {
            while(true) {
                var input = Console.ReadKey(false).Key;
                switch(input)
                {
                    case ConsoleKey.Escape:
                        GameOver();
                        break;
                    case ConsoleKey.UpArrow:
                        requestHeading = Heading.NORTH;
                        break;
                    case ConsoleKey.DownArrow:
                        requestHeading = Heading.SOUTH;
                        break;
                    case ConsoleKey.LeftArrow:
                        requestHeading = Heading.WEST;
                        break;
                    case ConsoleKey.RightArrow:
                        requestHeading = Heading.EAST;
                        break;
                }
            }
        }

        static void Main(string[] args)
        {
            int width = int.Parse(args[0]);
            int height = int.Parse(args[1]);
            Program program = new Program(width, height, 0);       

            while(true);
        }

        private void SpawnChar(char c)
        {
            bool emptyCell = false;
            while(!emptyCell)
            {
                int y = random.Next(boardHeight);
                int x = random.Next(boardWidth);
                var charAtPosition = board[x,y];
                if(charAtPosition == floorChar)
                {
                    emptyCell = true;
                    board[x,y] = c;
                }
            }
        }

        private void GameOver()
        {
            try
            {
                t.Stop();
                Console.WriteLine();
                Console.WriteLine("GAMEOVER!");
                Environment.Exit(0);
            } catch (Exception e) {
                Console.Error.Write(e.InnerException);
            }
        }

        private void Update(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (requestHeading == Heading.NORTH && heading != Heading.SOUTH) heading = Heading.NORTH;
            else if (requestHeading == Heading.SOUTH && heading != Heading.NORTH) heading = Heading.SOUTH;
            else if (requestHeading == Heading.EAST && heading != Heading.WEST) heading = Heading.EAST;
            else if (requestHeading == Heading.WEST && heading != Heading.EAST) heading = Heading.WEST;
            requestHeading = null;

            int nextPositionX = snake.First.Value.w;
            int nextPositionY = snake.First.Value.h;
            switch(heading)
            {
                case Heading.NORTH:
                    nextPositionY -= 1;
                    break;
                case Heading.SOUTH:
                    nextPositionY += 1;
                    break;
                case Heading.EAST:
                    nextPositionX += 1;
                    break;
                case Heading.WEST:
                    nextPositionX -= 1;
                    break;
            }
            (int w, int h) nextPositionTuple = (nextPositionX, nextPositionY);

            int nextPositionXCalculated = Math.Abs(mod(nextPositionX, boardWidth));
            int nextPositionYCalculated = Math.Abs(mod(nextPositionY, boardHeight));
            (int w, int h) nextPositionCalculatedTuple = (nextPositionXCalculated, nextPositionYCalculated);

            char charAtNextPositon = board[nextPositionXCalculated, nextPositionYCalculated];
            bool eating = false;
            if(charAtNextPositon == foodChar) eating = true;
            else if (charAtNextPositon == snakeChar) GameOver();

            MoveTo(nextPositionCalculatedTuple, eating);

            Console.SetCursorPosition(0,0);
            PrintBoard();
        }

        private void MoveTo((int w, int h) nextPosition, bool eating)
        {
            snake.AddFirst(nextPosition);
            board[snake.First.Value.w,snake.First.Value.h] = snakeChar;
            if(!eating)
            {
                board[snake.Last.Value.w,snake.Last.Value.h] = floorChar;
                snake.RemoveLast();
            }
            else
            {
                points++;
                SpeedUp();
                SpawnChar(foodChar);
                //if(random.Next(100) < riskOfBomb) SpawnGameObject(GameObjectEnum.BOMB);
            }
        }

        private void SpeedUp()
        {
            t.Interval = speedCurve.CalculateY(points);
        }

        private void PrintBoard()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for(int y = 0; y < boardHeight; y++) {
                for(int x = 0; x < boardWidth; x++) {
                    stringBuilder.Append(board[x,y]);
                }
                stringBuilder.Append("\n");
            }
            stringBuilder.Append("\n");
            stringBuilder.Append("Score: ");
            stringBuilder.Append(points);

            stringBuilder.Append("\n");
            stringBuilder.Append("Time bwtween updates: ");
            stringBuilder.Append(speedCurve.CalculateY(points));

            Console.WriteLine(stringBuilder);
        }

        int mod(int x, int m) {return (x%m + m)%m;}
    }

    public enum Heading
    {
        NORTH,
        SOUTH,
        WEST,
        EAST
    }
}

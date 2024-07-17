using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;

class Enemy
{
    public int X { get; set; }
    public int Y { get; set; }
    public int StartX { get; set; }
    public int StartY { get; set; }
    public int EndX { get; set; }
    public int EndY { get; set; }
    public bool IsHorizontal { get; set; }
    public bool MovingPositive { get; set; }

    public Enemy(int startX, int startY, int endX, int endY, bool isHorizontal)
    {
        StartX = startX;
        StartY = startY;
        EndX = endX;
        EndY = endY;
        X = startX;
        Y = startY;
        IsHorizontal = isHorizontal;
        MovingPositive = true;
    }

    public void Move()
    {
        if (IsHorizontal)
        {
            if (MovingPositive)
            {
                Y++;
                if (Y >= EndY)
                {
                    Y = EndY;
                    MovingPositive = false;
                }
            }
            else
            {
                Y--;
                if (Y <= StartY)
                {
                    Y = StartY;
                    MovingPositive = true;
                }
            }
        }
        else
        {
            if (MovingPositive)
            {
                X++;
                if (X >= EndX)
                {
                    X = EndX;
                    MovingPositive = false;
                }
            }
            else
            {
                X--;
                if (X <= StartX)
                {
                    X = StartX;
                    MovingPositive = true;
                }
            }
        }
    }
}

class StealthGame
{
    static char[,] gameArea = new char[40, 120];
    static int playerX = 2;
    static int playerY = 2;
    static List<Enemy> enemies = new List<Enemy>();
    static Stopwatch stopwatch = new Stopwatch();
    static Stopwatch enemyTimer = new Stopwatch();
    static bool gameRunning = true;
    static int exclamationSize = 1;
    static List<Tuple<int, int>> exclamationPoints = new List<Tuple<int, int>>();
    static Stopwatch exclamationTimer = new Stopwatch();

    static int exclamationInterval = 1000;
    static int enemyMoveInterval = 1000;

    static void Main(string[] args)
    {
        Console.WriteLine("Please select the game difficulty\n1. Easy\n2. Medium\n3. Hard");
        int difficulty = SelectDifficulty();

        SetDifficultyParameters(difficulty);

        Console.Clear();
        Console.WriteLine("Please press ALT+ENTER to go fullscreen and press any key to start.");
        Console.ReadKey();
        InitializeGameArea(gameArea);
        SetFinishPoint();
        Console.Clear();
        stopwatch.Start();
        DrawEntireGameArea(gameArea);
        DrawPlayer(playerX, playerY, 'C');

        enemies.Add(new Enemy(11, 5, 11, 25, true));
        enemies.Add(new Enemy(20, 94, 20, 108, true)); 
        enemies.Add(new Enemy(12, 114, 12, 114, true)); 
        enemies.Add(new Enemy(12, 70, 12, 78, true)); 
        enemies.Add(new Enemy(27, 6, 33, 6, false)); 
        enemies.Add(new Enemy(5, 55, 13, 55, false)); 
        enemies.Add(new Enemy(35, 31, 35, 31, false));
        enemies.Add(new Enemy(8, 90, 8, 90, false));
        enemies.Add(new Enemy(33, 83, 35, 83, false));
        enemies.Add(new Enemy(33, 110, 33, 110, false));
        enemies.Add(new Enemy(18, 47, 18, 47, false));
        enemies.Add(new Enemy(26, 72, 26, 72, false));
        enemies.Add(new Enemy(25, 92, 25, 92, false));
        enemies.Add(new Enemy(26, 111, 26, 111, false));
        enemies.Add(new Enemy(38, 44, 38, 54, true)); 

        exclamationTimer.Start();
        enemyTimer.Start();

        while (gameRunning)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                MovePlayer(keyInfo.Key);
                CheckGameOver();
                if (!gameRunning) return;
            }

            if (exclamationTimer.ElapsedMilliseconds >= exclamationInterval)
            {
                exclamationSize = Math.Max(1, exclamationSize - 2);
                ClearExclamations();
                DrawExclamations(exclamationSize);
                exclamationTimer.Restart();
            }

            if (enemyTimer.ElapsedMilliseconds >= enemyMoveInterval)
            {
                foreach (var enemy in enemies)
                {
                    DrawEnemy(enemy.X, enemy.Y, ' ');  
                    enemy.Move();
                    DrawEnemy(enemy.X, enemy.Y, 'E'); 
                }
                CheckGameOver();
                if (!gameRunning) return;
                enemyTimer.Restart();
            }
        }
    }

    static int SelectDifficulty()
    {
        while (true)
        {
            string input = Console.ReadLine();
            if (input == "1" || input == "2" || input == "3")
            {
                return int.Parse(input);
            }
            else
            {
                Console.WriteLine("Invalid selection! Please use the 1, 2, or 3 keys to make a selection.");
            }
        }
    }

    static void SetDifficultyParameters(int difficulty)
    {
        switch (difficulty)
        {
            case 1: // Easy
                exclamationInterval = 500;
                enemyMoveInterval = 1500;
                break;
            case 2: // Medium
                exclamationInterval = 1000;
                enemyMoveInterval = 1000;
                break;
            case 3: // Hard
                exclamationInterval = 1500;
                enemyMoveInterval = 500;
                break;
        }
    }

    static void CheckGameOver()
    {
        foreach (var enemy in enemies)
        {
            if (playerX == enemy.X && playerY == enemy.Y)
            {
                GameOver();
                return;
            }

            foreach (var point in exclamationPoints)
            {
                if (enemy.X == point.Item1 && enemy.Y == point.Item2)
                {
                    GameOver();
                    return;
                }
            }
        }

        if (playerX == gameArea.GetLength(0) - 2 && playerY == gameArea.GetLength(1) - 2)
        {
            stopwatch.Stop();
            gameRunning = false;
            DisplayEndMessage(true);
        }
    }

    static void GameOver()
    {
        stopwatch.Stop();
        Console.Clear();
        gameRunning = false;
        DisplayEndMessage(false);
    }

    static void ClearExclamations()
    {
        foreach (var point in exclamationPoints)
        {
            if (gameArea[point.Item1, point.Item2] == '!')
            {
                gameArea[point.Item1, point.Item2] = ' ';
                Console.SetCursorPosition(point.Item2, point.Item1);
                Console.Write(' ');
            }
        }
        exclamationPoints.Clear();
    }

    static void DrawExclamations(int size)
    {
        if (size == 1) return;
        int halfSize = size / 2;
        for (int i = -halfSize; i <= halfSize; i++)
        {
            for (int j = -halfSize; j <= halfSize; j++)
            {
                if (i == 0 && j == 0) continue; 
                int newX = playerX + i, newY = playerY + j;
                if (newX >= 0 && newX < gameArea.GetLength(0) && newY >= 0 && newY < gameArea.GetLength(1) && gameArea[newX, newY] == ' ')
                {
                    gameArea[newX, newY] = '!';
                    Console.SetCursorPosition(newY, newX);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write('!');
                    Console.ResetColor();
                    exclamationPoints.Add(new Tuple<int, int>(newX, newY));
                }
            }
        }
    }

    static void InitializeGameArea(char[,] area)
    {
        Random rand = new Random(146);
        FillAreaWithBlankSpace(area);
        CreateComplexObstacles(rand, area, 50);
        CreateRandomObstacles(rand, area, 100);
        CreateLargeObstacles(rand, area, 15);
        CreateNarrowPassages(rand, area, 20);
        CreateBorders(area);
    }

    static void FillAreaWithBlankSpace(char[,] area)
    {
        for (int i = 0; i < area.GetLength(0); i++)
        {
            for (int j = 0; j < area.GetLength(1); j++)
            {
                area[i, j] = ' ';
            }
        }
    }

    static void CreateComplexObstacles(Random rand, char[,] area, int count)
    {
        for (int i = 0; i < count; i++)
        {
            int x = rand.Next(1, gameArea.GetLength(0) - 1);
            int y = rand.Next(1, gameArea.GetLength(1) - 1);
            int patternType = rand.Next(0, 4);
            DrawPattern(x, y, patternType, area);
        }
    }

    static void DrawPattern(int x, int y, int type, char[,] area)
    {
        switch (type)
        {
            case 0:
                if (x + 2 < area.GetLength(0) && y + 2 < area.GetLength(1))
                {
                    for (int dx = -2; dx <= 2; dx++)
                    {
                        if (x + dx >= 0 && x + dx < area.GetLength(0))
                            area[x + dx, y] = '#';
                        if (y + dx >= 0 && y + dx < area.GetLength(1))
                            area[x, y + dx] = '#';
                    }
                }
                break;
            case 1: 
                if (x + 3 < area.GetLength(0) && y + 3 < area.GetLength(1))
                {
                    for (int dx = 0; dx < 3; dx++)
                    {
                        for (int dy = 0; dy < 3; dy++)
                        {
                            if (dx == 0 || dy == 0 || dx == 2 || dy == 2)
                                area[x + dx, y + dy] = '#';
                        }
                    }
                }
                break;
            case 2:
                if (x + 3 < area.GetLength(0) && y + 3 < area.GetLength(1))
                {
                    for (int dx = 0; dx < 3; dx++)
                    {
                        area[x + dx, y + dx] = '#';
                    }
                }
                break;
            case 3:
                if (x + 2 < area.GetLength(0) && y + 2 < area.GetLength(1))
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        if (x + dx >= 0 && x + dx < area.GetLength(0) && y - 1 >= 0 && y + 1 < area.GetLength(1))
                        {
                            area[x + dx, y] = '#';
                            area[x, y + dx] = '#';
                        }
                    }
                }
                break;
        }
    }

    static void CreateRandomObstacles(Random rand, char[,] area, int count)
    {
        for (int i = 0; i < count; i++)
        {
            int x = rand.Next(1, gameArea.GetLength(0) - 1);
            int y = rand.Next(1, gameArea.GetLength(1) - 1);
            area[x, y] = '#';
        }
    }

    static void CreateLargeObstacles(Random rand, char[,] area, int count)
    {
        for (int k = 0; k < count; k++)
        {
            int width = rand.Next(10, 20);
            int height = rand.Next(5, 10);
            int startX = rand.Next(1, gameArea.GetLength(0) - height - 1);
            int startY = rand.Next(1, gameArea.GetLength(1) - width - 1);

            for (int i = startX; i < startX + height; i++)
            {
                for (int j = startY; j < startY + width; j++)
                {
                    area[i, j] = '#';
                }
            }
        }
    }

    static void CreateNarrowPassages(Random rand, char[,] area, int count)
    {
        for (int k = 0; k < count; k++)
        {
            int startRow = rand.Next(1, gameArea.GetLength(0) - 1);
            int length = rand.Next(5, 20);
            int direction = rand.Next(0, 2); 

            for (int i = 0; i < length; i++)
            {
                if (direction == 0) 
                {
                    if (startRow + i < gameArea.GetLength(0) - 1)
                    {
                        area[startRow + i, 1] = '#'; 
                    }
                }
                else 
                {
                    int startCol = rand.Next(1, gameArea.GetLength(1) - 1);
                    if (startCol + i < gameArea.GetLength(1) - 1)
                    {
                        area[1, startCol + i] = '#';
                    }
                }
            }
        }
    }

    static void CreateBorders(char[,] area)
    {
        for (int i = 0; i < area.GetLength(0); i++)
        {
            area[i, 0] = area[i, area.GetLength(1) - 1] = '#';
        }
        for (int j = 0; j < area.GetLength(1); j++)
        {
            area[0, j] = area[area.GetLength(0) - 1, j] = '#';
        }
    }

    static void SetFinishPoint()
    {
        gameArea[gameArea.GetLength(0) - 2, gameArea.GetLength(1) - 2] = 'F';
    }

    static void DisplayEndMessage(bool won)
    {
        Console.Clear();
        if (won)
        {
            Console.WriteLine("Congratulations, you won!");
        }
        else
        {
            Console.WriteLine("You lost!");
        }
        Console.WriteLine("Time: {0}:{1}", stopwatch.Elapsed.Minutes, stopwatch.Elapsed.Seconds);
        Console.WriteLine("Console Stealth Game By Efe Yıldırım");
    }

    static void DrawEntireGameArea(char[,] area)
    {
        Console.Clear(); 
        for (int i = 0; i < area.GetLength(0); i++)
        {
            for (int j = 0; j < area.GetLength(1); j++)
            {
                if (area[i, j] == 'F')
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(area[i, j]);
                }
                else if (area[i, j] == '#')
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(area[i, j]);
                }
                else
                {
                    Console.ResetColor();
                    Console.Write(area[i, j]);
                }
            }
            Console.WriteLine();
        }
        Console.SetCursorPosition(gameArea.GetLength(1) + 1, 2);
        Console.WriteLine("Console Stealth Game By Efe Yıldırım");
        Console.ResetColor();
    }

    static void DrawPlayer(int x, int y, char playerChar)
    {
        Console.SetCursorPosition(y, x);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(playerChar);
        Console.ResetColor();
    }

    static void DrawEnemy(int x, int y, char enemyChar)
    {
        Console.SetCursorPosition(y, x);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(enemyChar);
        Console.ResetColor();
    }

    static void MovePlayer(ConsoleKey key)
    {
        int newX = playerX;
        int newY = playerY;

        switch (key)
        {
            case ConsoleKey.UpArrow:
                newX--;
                break;
            case ConsoleKey.DownArrow:
                newX++;
                break;
            case ConsoleKey.LeftArrow:
                newY--;
                break;
            case ConsoleKey.RightArrow:
                newY++;
                break;
        }

        if (newX >= 0 && newY >= 0 && newX < gameArea.GetLength(0) && newY < gameArea.GetLength(1) && gameArea[newX, newY] != '#')
        {
            ClearExclamations();
            DrawPlayer(playerX, playerY, ' ');
            playerX = newX;
            playerY = newY;
            DrawPlayer(playerX, playerY, 'C');
            exclamationSize = Math.Min(15, exclamationSize + 2);
            DrawExclamations(exclamationSize);
            exclamationTimer.Restart();
        }
    }
}

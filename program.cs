namespace TicTacToe
{
    class Program
    {
        // The game board as a 2D array
        static char[,] board = new char[3, 3];

        // Cursor position on the board
        static int cursorX = 0;
        static int cursorY = 0;

        // Symbols for players (assigned dynamically)
        static char HUMAN;
        static char AI;
        const char EMPTY = ' ';

        // Check if the player chose to go first or second
        static bool playerChoseFirst;

        // Track game mode: true for minimax AI, false for Easy AI
        static bool useMinimax;

        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            // Ask the player to choose game mode and first/second turn
            useMinimax = AskGameMode();
            playerChoseFirst = AskInitialTurnOrder();

            while (true)
            {
                Console.Clear();
                InitializeBoard();

                // Determine who goes first and assign symbols
                bool isHumanTurn;
                if (playerChoseFirst)
                {
                    HUMAN = 'X';
                    AI = 'O';
                    isHumanTurn = true;
                }
                else
                {
                    HUMAN = 'O';
                    AI = 'X';
                    isHumanTurn = false;
                }

                playerChoseFirst = !playerChoseFirst; // Alternate for next game

                while (true)
                {
                    Console.Clear();
                    DrawBoard();

                    if (CheckWin(HUMAN))
                    {
                        Console.WriteLine("\nYou win!");
                        break;
                    }
                    else if (CheckWin(AI))
                    {
                        Console.WriteLine("\nYou lose!");
                        break;
                    }
                    else if (IsBoardFull())
                    {
                        Console.WriteLine("\nIt's a draw!");
                        break;
                    }

                    if (isHumanTurn)
                    {
                        HumanMove();
                        isHumanTurn = false;
                    }
                    else
                    {
                        Console.WriteLine("\nComputer is thinking...");
                        Thread.Sleep(500);
                        if (useMinimax)
                        {
                            AITurn(); // Use Minimax for AI
                        }
                        else
                        {
                            EasyBotTurn(); // Use easy AI
                        }
                        isHumanTurn = true;
                    }
                }

                Console.WriteLine("\nPress 'R' to restart or any other key to exit.");
                var key = Console.ReadKey();
                if (key.KeyChar != 'r' && key.KeyChar != 'R')
                    break; // Exit the game loop if the player does not press 'R'
            }
        }

        // Ask the player to choose the game mode (Minimax or Easy AI)
        static bool AskGameMode()
        {
            Console.WriteLine("Choose game mode: (1) Minimax AI (Optimal) or (2) Easy AI (First Available Move)");
            char choice = Console.ReadKey().KeyChar;
            Console.Clear();

            return choice == '1';
        }

        // Ask the player if they want to go first or second
        static bool AskInitialTurnOrder()
        {
            Console.WriteLine("Do you want to go first? (y/n)");
            char choice = Console.ReadKey().KeyChar;
            Console.Clear();

            return choice == 'y' || choice == 'Y';
        }

        // Initialize the board with empty spaces
        static void InitializeBoard()
        {
            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 3; x++)
                    board[y, x] = EMPTY;
        }

        // Draw the board and cursor
        static void DrawBoard()
        {
            Console.WriteLine("Use arrow keys to move, Enter to select.\n");

            for (int y = 0; y < 3; y++)
            {
                Console.Write(" ");
                for (int x = 0; x < 3; x++)
                {
                    // Highlight the current cursor position
                    if (x == cursorX && y == cursorY)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write($" {board[y, x]} ");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write($" {board[y, x]} ");
                    }

                    if (x < 2)
                        Console.Write("|");
                }
                Console.WriteLine();

                if (y < 2)
                    Console.WriteLine(" ---+---+---");
            }
        }

        // Handle player's move
        static void HumanMove()
        {
            while (true)
            {
                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.LeftArrow:
                        if (cursorX > 0) cursorX--;
                        Console.Clear();
                        DrawBoard();
                        break;
                    case ConsoleKey.RightArrow:
                        if (cursorX < 2) cursorX++;
                        Console.Clear();
                        DrawBoard();
                        break;
                    case ConsoleKey.UpArrow:
                        if (cursorY > 0) cursorY--;
                        Console.Clear();
                        DrawBoard();
                        break;
                    case ConsoleKey.DownArrow:
                        if (cursorY < 2) cursorY++;
                        Console.Clear();
                        DrawBoard();
                        break;
                    case ConsoleKey.Enter:
                        if (board[cursorY, cursorX] == EMPTY)
                        {
                            board[cursorY, cursorX] = HUMAN;
                            return;
                        }
                        else
                        {
                            Console.Beep();
                        }
                        break;
                }
            }
        }

        // AI's turn using the minimax algorithm
        static void AITurn()
        {
            int bestScore = int.MinValue;
            int moveX = -1;
            int moveY = -1;

            // Evaluate all possible moves
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    if (board[y, x] == EMPTY)
                    {
                        board[y, x] = AI;
                        int score = Minimax(board, 0, false);
                        board[y, x] = EMPTY;

                        if (score > bestScore)
                        {
                            bestScore = score;
                            moveX = x;
                            moveY = y;
                        }
                    }
                }
            }

            // Make the best move
            if (moveX != -1 && moveY != -1)
                board[moveY, moveX] = AI;
        }

        // AI's turn for Easy Bot (chooses the first available move)
        static void EasyBotTurn()
        {
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    if (board[y, x] == EMPTY)
                    {
                        board[y, x] = AI;
                        return;
                    }
                }
            }
        }

        // Minimax algorithm to calculate the best move
        static int Minimax(char[,] newBoard, int depth, bool isMaximizing)
        {
            if (CheckWin(AI))
                return 10 - depth;
            if (CheckWin(HUMAN))
                return depth - 10;
            if (IsBoardFull())
                return 0;

            if (isMaximizing)
            {
                int bestScore = int.MinValue;
                for (int y = 0; y < 3; y++)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        if (newBoard[y, x] == EMPTY)
                        {
                            newBoard[y, x] = AI;
                            int score = Minimax(newBoard, depth + 1, false);
                            newBoard[y, x] = EMPTY;
                            bestScore = Math.Max(score, bestScore);
                        }
                    }
                }
                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;
                for (int y = 0; y < 3; y++)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        if (newBoard[y, x] == EMPTY)
                        {
                            newBoard[y, x] = HUMAN;
                            int score = Minimax(newBoard, depth + 1, true);
                            newBoard[y, x] = EMPTY;
                            bestScore = Math.Min(score, bestScore);
                        }
                    }
                }
                return bestScore;
            }
        }

        // Check if a player has won
        static bool CheckWin(char player)
        {
            // Rows and Columns
            for (int i = 0; i < 3; i++)
            {
                if ((board[i, 0] == player &&
                     board[i, 1] == player &&
                     board[i, 2] == player) ||
                    (board[0, i] == player &&
                     board[1, i] == player &&
                     board[2, i] == player))
                {
                    return true;
                }
            }

            // Diagonals
            if ((board[0, 0] == player &&
                 board[1, 1] == player &&
                 board[2, 2] == player) ||
                (board[0, 2] == player &&
                 board[1, 1] == player &&
                 board[2, 0] == player))
            {
                return true;
            }

            return false;
        }

        // Check if the board is full
        static bool IsBoardFull()
        {
            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 3; x++)
                    if (board[y, x] == EMPTY)
                        return false;
            return true;
        }
    }
}

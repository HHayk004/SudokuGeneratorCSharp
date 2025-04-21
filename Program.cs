using System;
using System.Collections.Generic;
using System.Security.Cryptography;

class SudokuGenerator
{
    private Random rand;

    private int size;

    public SudokuGenerator()
    {
        rand = new Random(); // Initialize Random in the constructor
    }

    public int[,] GetSudoku(int n)
    {
        int[,] board = InitialBoard(n);
        size = n;


        List<Action<int[,]>> shuffleFunctions = new List<Action<int[,]>>()
        {
            MirrorX, MirrorY, Clockwise, CounterClockwise, Transpose, CounterTranspose
        };

        for (int i = 0; i < 20; i++)
        {
            int index = rand.Next(shuffleFunctions.Count);
            shuffleFunctions[index](board);
        }

        List<Action<int[,]>> shuffleRowColFunctions = new List<Action<int[,]>>()
        {
            ShuffleRows, ShuffleCols, ShuffleRowBlocks, ShuffleColBlocks
        };

        for (int i = 0; i < 30; i++)
        {
            int index = rand.Next(shuffleRowColFunctions.Count);
            shuffleRowColFunctions[index](board);
        }

        NumSwapping(board);

        return board;
    }
    private int[,] InitialBoard(int n)
    {
        int[,] board = new int[n, n];
        for (int i = 0; i < n; ++i)
        {
            board[0, i] = i + 1;
        }

        for (int i = 1; i < n; ++i)
        {
            for (int j = 1; j < n; ++j)
            {
                board[i, j] = board[i - 1, j - 1];
            }

            board[i, 0] = board[i - 1, n - 1];
        }

        return board;
    }

    private void MirrorX(int[,] board)
    {
        for (int i = 0; i < size / 2; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int temp = board[i, j];
                board[i, j] = board[size - 1 - i, j];
                board[size - 1 - i, j] = temp;
            }
        }
    }

    private void MirrorY(int[,] board)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size / 2; j++)
            {
                int temp = board[i, j];
                board[i, j] = board[i, size - 1 - j];
                board[i, size - 1 - j] = temp;
            }
        }
    }

    private void Clockwise(int[,] board)
    {
        Transpose(board);
        MirrorY(board);
    }

    private void CounterClockwise(int[,] board)
    {
        MirrorY(board);
        Transpose(board);
    }

    private void Transpose(int[,] board)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = i + 1; j < size; j++)
            {
                int temp = board[i, j];
                board[i, j] = board[j, i];
                board[j, i] = temp;
            }
        }
    }

    private void CounterTranspose(int[,] board)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size - i - 1; j++)
            {
                int temp = board[i, j];
                board[i, j] = board[size - 1 - j, size - 1 - i];
                board[size - 1 - j, size - 1 - i] = temp;
            }
        }
    }

    private void ShuffleRows(int[,] board)
    {
        int blockSize = Convert.ToInt32(Math.Sqrt(size));
        for (int i = 0; i < blockSize; i++)
        {
            int row1 = rand.Next(blockSize) + i * blockSize;
            int row2 = rand.Next(blockSize) + i * blockSize;

            for (int j = 0; j < size; j++)
            {
                int temp = board[row1, j];
                board[row1, j] = board[row2, j];
                board[row2, j] = temp;
            }
        }
    }

    private void ShuffleCols(int[,] board)
    {
        int blockSize = Convert.ToInt32(Math.Sqrt(size));
        for (int i = 0; i < blockSize; i++)
        {
            int col1 = rand.Next(blockSize) + i * blockSize;
            int col2 = rand.Next(blockSize) + i * blockSize;

            for (int j = 0; j < size; j++)
            {
                int temp = board[j, col1];
                board[j, col1] = board[j, col2];
                board[j, col2] = temp;
            }
        }
    }

    private void ShuffleRowBlocks(int[,] board)
    {
        int blockSize = Convert.ToInt32(Math.Sqrt(size));
        int rowBlock1 = rand.Next(blockSize) * blockSize;
        int rowBlock2 = rand.Next(blockSize) * blockSize;

        for (int i = 0; i < blockSize; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int temp = board[rowBlock1 + i, j];
                board[rowBlock1 + i, j] = board[rowBlock2 + i, j];
                board[rowBlock2 + i, j] = temp;
            }
        }
    }

    private void ShuffleColBlocks(int[,] board)
    {
        int blockSize = Convert.ToInt32(Math.Sqrt(size));
        int colBlock1 = rand.Next(blockSize) * blockSize;
        int colBlock2 = rand.Next(blockSize) * blockSize;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < blockSize; j++)
            {
                int temp = board[i, colBlock1 + j];
                board[i, colBlock1 + j] = board[i, colBlock2 + j];
                board[i, colBlock2 + j] = temp;
            }
        }
    }

    private int[] GenerateShuffle()
    {
        int[] numbers = new int[size];
        for (int i = 0; i < size; ++i)
        {
            numbers[i] = i + 1;
        }

        // Shuffle using manual range control
        for (int i = size - 1; i > 0; --i)
        {
            int j = rand.Next(0, i + 1); // Choose index from 0 to i
            // Swap numbers[j] and numbers[i]
            int temp = numbers[j];
            numbers[j] = numbers[i];
            numbers[i] = temp;
        }

        return numbers;
    }

    private void NumSwapping(int[,] board)
    {
        int[] shuffled = GenerateShuffle();
        for (int i = 0; i < size; ++i)
        {
            for (int j = 0; j < size; ++j)
            {
                board[i, j] = shuffled[board[i, j] - 1]; // Correct index mapping for 1-9
            }
        }
    }

    public void PrintBoard(int[,] board)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Console.Write(board[i, j] + " ");
            }
            Console.WriteLine();
        }
    }

    static void Main()
    {
        SudokuGenerator generator = new SudokuGenerator(); // Create an instance
        int[,] board = generator.GetSudoku(16);  // Generate a Sudoku board
        generator.PrintBoard(board);  // Print the board
    }
}

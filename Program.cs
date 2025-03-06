using System;
using System.Collections.Generic;

class SudokuGenerator
{
    private static Random rand = new Random();

    public static int[,] GetSudoku()
    {
        int[,] board = {
            {1, 2, 3, 4, 5, 6, 7, 8, 9},
            {7, 8, 9, 1, 2, 3, 4, 5, 6},
            {4, 5, 6, 7, 8, 9, 1, 2, 3},
            {3, 1, 2, 6, 4, 5, 9, 7, 8},
            {9, 7, 8, 3, 1, 2, 6, 4, 5},
            {6, 4, 5, 9, 7, 8, 3, 1, 2},
            {2, 3, 1, 5, 6, 4, 8, 9, 7},
            {8, 9, 7, 2, 3, 1, 5, 6, 4},
            {5, 6, 4, 8, 9, 7, 2, 3, 1}
        };

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

        return board;
    }

    private static void MirrorX(int[,] board)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                int temp = board[i, j];
                board[i, j] = board[8 - i, j];
                board[8 - i, j] = temp;
            }
        }
    }

    private static void MirrorY(int[,] board)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                int temp = board[i, j];
                board[i, j] = board[i, 8 - j];
                board[i, 8 - j] = temp;
            }
        }
    }

    private static void Clockwise(int[,] board)
    {
        Transpose(board);
        MirrorY(board);
    }

    private static void CounterClockwise(int[,] board)
    {
        MirrorY(board);
        Transpose(board);
    }

    private static void Transpose(int[,] board)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = i + 1; j < 9; j++)
            {
                int temp = board[i, j];
                board[i, j] = board[j, i];
                board[j, i] = temp;
            }
        }
    }

    private static void CounterTranspose(int[,] board)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9 - i - 1; j++)
            {
                int temp = board[i, j];
                board[i, j] = board[8 - j, 8 - i];
                board[8 - j, 8 - i] = temp;
            }
        }
    }

    private static void ShuffleRows(int[,] board)
    {
        int blockSize = 3;
        for (int i = 0; i < blockSize; i++)
        {
            int row1 = rand.Next(blockSize) + i * blockSize;
            int row2 = rand.Next(blockSize) + i * blockSize;

            for (int j = 0; j < 9; j++)
            {
                int temp = board[row1, j];
                board[row1, j] = board[row2, j];
                board[row2, j] = temp;
            }
        }
    }

    private static void ShuffleCols(int[,] board)
    {
        int blockSize = 3;
        for (int i = 0; i < blockSize; i++)
        {
            int col1 = rand.Next(blockSize) + i * blockSize;
            int col2 = rand.Next(blockSize) + i * blockSize;

            for (int j = 0; j < 9; j++)
            {
                int temp = board[j, col1];
                board[j, col1] = board[j, col2];
                board[j, col2] = temp;
            }
        }
    }

    private static void ShuffleRowBlocks(int[,] board)
    {
        int blockSize = 3;
        int rowBlock1 = rand.Next(blockSize) * blockSize;
        int rowBlock2 = rand.Next(blockSize) * blockSize;

        for (int i = 0; i < blockSize; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                int temp = board[rowBlock1 + i, j];
                board[rowBlock1 + i, j] = board[rowBlock2 + i, j];
                board[rowBlock2 + i, j] = temp;
            }
        }
    }

    private static void ShuffleColBlocks(int[,] board)
    {
        int blockSize = 3;
        int colBlock1 = rand.Next(blockSize) * blockSize;
        int colBlock2 = rand.Next(blockSize) * blockSize;

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < blockSize; j++)
            {
                int temp = board[i, colBlock1 + j];
                board[i, colBlock1 + j] = board[i, colBlock2 + j];
                board[i, colBlock2 + j] = temp;
            }
        }
    }

    public static void PrintBoard(int[,] board)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Console.Write(board[i, j] + " ");
            }
            Console.WriteLine();
        }
    }

    static void Main()
    {
        int[,] board = GetSudoku();
        PrintBoard(board);
    }
}

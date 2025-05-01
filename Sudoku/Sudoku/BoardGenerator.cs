using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    internal class BoardGenerator
    {
        private Random rand;

        private int Size;

        public BoardGenerator()
        {
            rand = new Random(); // Initialize Random in the constructor
        }

        public int[,] GetBoard(int n)
        {
            int[,] board = InitialBoard(n);
            Size = n;

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
            int blockSize = Convert.ToInt32(Math.Sqrt(n));

            for (int i = 0; i < n; ++i)
            {
                board[0, i] = i + 1;
            }

            for (int i = 1; i < blockSize; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    board[i, j] = board[i - 1, (j + blockSize) % n];
                }
            }

            for (int i = 1; i < blockSize; ++i)
            {
                for (int j = 0; j < blockSize; ++j)
                {
                    for (int k = 0; k < n; ++k)
                    {
                        board[i * blockSize + j, k] = board[(i - 1) * blockSize + j, (k + 1) % n];
                    }
                }
            }

            return board;
        }

        private void MirrorX(int[,] board)
        {
            for (int i = 0; i < Size / 2; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    int temp = board[i, j];
                    board[i, j] = board[Size - 1 - i, j];
                    board[Size - 1 - i, j] = temp;
                }
            }
        }

        private void MirrorY(int[,] board)
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size / 2; j++)
                {
                    int temp = board[i, j];
                    board[i, j] = board[i, Size - 1 - j];
                    board[i, Size - 1 - j] = temp;
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
            for (int i = 0; i < Size; i++)
            {
                for (int j = i + 1; j < Size; j++)
                {
                    int temp = board[i, j];
                    board[i, j] = board[j, i];
                    board[j, i] = temp;
                }
            }
        }

        private void CounterTranspose(int[,] board)
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size - i - 1; j++)
                {
                    int temp = board[i, j];
                    board[i, j] = board[Size - 1 - j, Size - 1 - i];
                    board[Size - 1 - j, Size - 1 - i] = temp;
                }
            }
        }

        private void ShuffleRows(int[,] board)
        {
            int blockSize = Convert.ToInt32(Math.Sqrt(Size));
            for (int i = 0; i < blockSize; i++)
            {
                int row1 = rand.Next(blockSize) + i * blockSize;
                int row2 = rand.Next(blockSize) + i * blockSize;

                for (int j = 0; j < Size; j++)
                {
                    int temp = board[row1, j];
                    board[row1, j] = board[row2, j];
                    board[row2, j] = temp;
                }
            }
        }

        private void ShuffleCols(int[,] board)
        {
            int blockSize = Convert.ToInt32(Math.Sqrt(Size));
            for (int i = 0; i < blockSize; i++)
            {
                int col1 = rand.Next(blockSize) + i * blockSize;
                int col2 = rand.Next(blockSize) + i * blockSize;

                for (int j = 0; j < Size; j++)
                {
                    int temp = board[j, col1];
                    board[j, col1] = board[j, col2];
                    board[j, col2] = temp;
                }
            }
        }

        private void ShuffleRowBlocks(int[,] board)
        {
            int blockSize = Convert.ToInt32(Math.Sqrt(Size));
            int rowBlock1 = rand.Next(blockSize) * blockSize;
            int rowBlock2 = rand.Next(blockSize) * blockSize;

            for (int i = 0; i < blockSize; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    int temp = board[rowBlock1 + i, j];
                    board[rowBlock1 + i, j] = board[rowBlock2 + i, j];
                    board[rowBlock2 + i, j] = temp;
                }
            }
        }

        private void ShuffleColBlocks(int[,] board)
        {
            int blockSize = Convert.ToInt32(Math.Sqrt(Size));
            int colBlock1 = rand.Next(blockSize) * blockSize;
            int colBlock2 = rand.Next(blockSize) * blockSize;

            for (int i = 0; i < Size; i++)
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
            int[] numbers = new int[Size];
            for (int i = 0; i < Size; ++i)
            {
                numbers[i] = i + 1;
            }

            // Shuffle using manual range control
            for (int i = Size - 1; i > 0; --i)
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
            for (int i = 0; i < Size; ++i)
            {
                for (int j = 0; j < Size; ++j)
                {
                    board[i, j] = shuffled[board[i, j] - 1];
                }
            }
        }

        public int[,] CreatePuzzle(int[,] board, int removeCellsCount)
        {
            int[,] puzzle = new int[Size, Size];
            Array.Copy(board, puzzle, board.Length);
            int[,] coordinates = GenerateRandomCoordinates(removeCellsCount);
            for (int i = 0; i < removeCellsCount; ++i)
            {
                puzzle[coordinates[i, 0], coordinates[i, 1]] = 0;
            }
            return puzzle;
        }

        public int[,] GenerateRandomCoordinates(int n)
        {
            if (n > Size * Size)
            {
                throw new ArgumentException("n cannot be greater than the total number of positions in the grid.");
            }

            HashSet<string> coordinatesSet = new HashSet<string>();
            int[,] coordinatesList = new int[n, 2];

            // Keep generating random coordinates until we have n unique ones
            for (int i = 0; i < n; ++i)
            {
                int row = rand.Next(0, Size); // Random row
                int col = rand.Next(0, Size); // Random column

                if (!coordinatesSet.Contains($"{row},{col}"))
                {
                    coordinatesSet.Add($"{row},{col}");
                    coordinatesList[i, 0] = row;  // Assign row
                    coordinatesList[i, 1] = col;
                }

                else
                {
                    --i;
                }
            }

            return coordinatesList;
        }
    }
}

using System;
using System.Collections.Generic;

namespace Sudoku
{
    internal class BoardGenerator
    {
        private Random rand;
        private int Size;

        public BoardGenerator()
        {
            rand = new Random();
        }

        public List<List<int>> GetBoard(int n)
        {
            Size = n;
            var board = InitialBoard(n);

            List<Action<List<List<int>>>> shuffleFunctions = new List<Action<List<List<int>>>>()
            {
                MirrorX, MirrorY, Clockwise, CounterClockwise, Transpose, CounterTranspose
            };

            for (int i = 0; i < 20; i++)
            {
                int index = rand.Next(shuffleFunctions.Count);
                shuffleFunctions[index](board);
            }

            List<Action<List<List<int>>>> shuffleRowColFunctions = new List<Action<List<List<int>>>>()
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

        private List<List<int>> InitialBoard(int n)
        {
            var board = new List<List<int>>(n);
            int blockSize = (int)Math.Sqrt(n);

            for (int i = 0; i < n; i++)
            {
                board.Add(new List<int>(new int[n]));
            }

            for (int i = 0; i < n; ++i)
            {
                board[0][i] = i + 1;
            }

            for (int i = 1; i < blockSize; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    board[i][j] = board[i - 1][(j + blockSize) % n];
                }
            }

            for (int i = 1; i < blockSize; ++i)
            {
                for (int j = 0; j < blockSize; ++j)
                {
                    for (int k = 0; k < n; ++k)
                    {
                        board[i * blockSize + j][k] = board[(i - 1) * blockSize + j][(k + 1) % n];
                    }
                }
            }

            return board;
        }

        private void MirrorX(List<List<int>> board)
        {
            for (int i = 0; i < Size / 2; i++)
            {
                var tempRow = board[i];
                board[i] = board[Size - 1 - i];
                board[Size - 1 - i] = tempRow;
            }
        }

        private void MirrorY(List<List<int>> board)
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size / 2; j++)
                {
                    int temp = board[i][j];
                    board[i][j] = board[i][Size - 1 - j];
                    board[i][Size - 1 - j] = temp;
                }
            }
        }

        private void Clockwise(List<List<int>> board)
        {
            Transpose(board);
            MirrorY(board);
        }

        private void CounterClockwise(List<List<int>> board)
        {
            MirrorY(board);
            Transpose(board);
        }

        private void Transpose(List<List<int>> board)
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = i + 1; j < Size; j++)
                {
                    int temp = board[i][j];
                    board[i][j] = board[j][i];
                    board[j][i] = temp;
                }
            }
        }

        private void CounterTranspose(List<List<int>> board)
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size - i - 1; j++)
                {
                    int temp = board[i][j];
                    board[i][j] = board[Size - 1 - j][Size - 1 - i];
                    board[Size - 1 - j][Size - 1 - i] = temp;
                }
            }
        }

        private void ShuffleRows(List<List<int>> board)
        {
            int blockSize = (int)Math.Sqrt(Size);
            for (int i = 0; i < blockSize; i++)
            {
                int row1 = rand.Next(blockSize) + i * blockSize;
                int row2 = rand.Next(blockSize) + i * blockSize;

                var tempRow = board[row1];
                board[row1] = board[row2];
                board[row2] = tempRow;
            }
        }

        private void ShuffleCols(List<List<int>> board)
        {
            int blockSize = (int)Math.Sqrt(Size);
            for (int i = 0; i < blockSize; i++)
            {
                int col1 = rand.Next(blockSize) + i * blockSize;
                int col2 = rand.Next(blockSize) + i * blockSize;

                for (int j = 0; j < Size; j++)
                {
                    int temp = board[j][col1];
                    board[j][col1] = board[j][col2];
                    board[j][col2] = temp;
                }
            }
        }

        private void ShuffleRowBlocks(List<List<int>> board)
        {
            int blockSize = (int)Math.Sqrt(Size);
            int rowBlock1 = rand.Next(blockSize) * blockSize;
            int rowBlock2 = rand.Next(blockSize) * blockSize;

            for (int i = 0; i < blockSize; i++)
            {
                var tempRow = board[rowBlock1 + i];
                board[rowBlock1 + i] = board[rowBlock2 + i];
                board[rowBlock2 + i] = tempRow;
            }
        }

        private void ShuffleColBlocks(List<List<int>> board)
        {
            int blockSize = (int)Math.Sqrt(Size);
            int colBlock1 = rand.Next(blockSize) * blockSize;
            int colBlock2 = rand.Next(blockSize) * blockSize;

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < blockSize; j++)
                {
                    int temp = board[i][colBlock1 + j];
                    board[i][colBlock1 + j] = board[i][colBlock2 + j];
                    board[i][colBlock2 + j] = temp;
                }
            }
        }

        private List<int> GenerateShuffle()
        {
            List<int> numbers = new List<int>(Size);
            for (int i = 0; i < Size; ++i)
            {
                numbers.Add(i + 1);
            }

            for (int i = Size - 1; i > 0; --i)
            {
                int j = rand.Next(0, i + 1);
                int temp = numbers[j];
                numbers[j] = numbers[i];
                numbers[i] = temp;
            }

            return numbers;
        }

        private void NumSwapping(List<List<int>> board)
        {
            List<int> shuffled = GenerateShuffle();
            for (int i = 0; i < Size; ++i)
            {
                for (int j = 0; j < Size; ++j)
                {
                    board[i][j] = shuffled[board[i][j] - 1];
                }
            }
        }

        public void GenerateRandomCoordinates(int n, List<(int row, int col)> coordinatesList, List<(int row, int col)> firstCoords)
        {
            HashSet<(int, int)> coordinatesSet = new HashSet<(int, int)>();

            foreach ((int i, int j) in firstCoords)
            {
                coordinatesSet.Add((i, j));
            }

            while (n != 0)
            {
                int row = rand.Next(0, Size);
                int col = rand.Next(0, Size);

                if (!coordinatesSet.Contains((row, col)))
                {
                    coordinatesSet.Add((row, col));
                    coordinatesList.Add((row, col));
                    --n;
                }
            }
        }

        public List<List<int>> CreatePuzzle(List<List<int>> board, int missingFields)
        {
            List<List<int>> puzzle = null;
            List<(int, int)> coordsToRemove = null;
            List<(int, int)> firstCoords = new List<(int, int)>();
            Solver solver = new Solver();
            int count;

            for (int i = 0; i < 1; ++i)
            {
                do
                {
                    puzzle = new List<List<int>>();

                    foreach (var row in board)
                    {
                        puzzle.Add(new List<int>(row));
                    }

                    coordsToRemove = new List<(int, int)>();
                    GenerateRandomCoordinates(missingFields, coordsToRemove, firstCoords);

                    if (i == 1)
                    {
                        foreach ((int row, int col) in firstCoords)
                        {
                            puzzle[row][col] = 0;
                        }
                    }

                    foreach (var (row, col) in coordsToRemove)
                    {
                        puzzle[row][col] = 0;
                    }

                } while (solver.CountSolutions(puzzle, 2) != 1);

                if (i == 0)
                {
                    foreach ((int row, int col) in coordsToRemove)
                    {
                        firstCoords.Add((row, col));
                    }
                }
            }
            return puzzle;
        }

    }
}

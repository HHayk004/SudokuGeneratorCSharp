using System;
using System.Numerics;
using System.Collections.Generic;

namespace Sudoku
{
    public class Solver
    {
        private (int row, int col, int box)[] emptyCellsArray;
        private int emptyCellsCount;
        private int[] rowUsed = new int[9];
        private int[] colUsed = new int[9];
        private int[] boxUsed = new int[9];
        private int solutionCount = 0;
        private int limit = 2; // default upper bound for solutions
        private static readonly int[] BitCounts = new int[512];

        static Solver()
        {
            for (int i = 0; i < 512; i++)
            {
                BitCounts[i] = PopCount(i);
            }
        }

        public int CountSolutions(List<List<int>> board, int solutionLimit = 2)
        {
            solutionCount = 0;
            limit = solutionLimit;
            Array.Clear(rowUsed, 0, 9);
            Array.Clear(colUsed, 0, 9);
            Array.Clear(boxUsed, 0, 9);

            List<(int, int, int)> emptyCells = new List<(int, int, int)>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int val = board[i][j];
                    int boxIndex = (i / 3) * 3 + (j / 3);
                    if (val == 0)
                    {
                        emptyCells.Add((i, j, boxIndex));
                    }
                    else
                    {
                        int digit = val - 1;
                        int mask = 1 << digit;
                        rowUsed[i] |= mask;
                        colUsed[j] |= mask;
                        boxUsed[boxIndex] |= mask;
                    }
                }
            }

            emptyCellsArray = emptyCells.ToArray();
            emptyCellsCount = emptyCells.Count;
            Backtrack(board);

            return solutionCount;
        }

        private void Backtrack(List<List<int>> board)
        {
            if (solutionCount >= limit)
                return;

            if (emptyCellsCount == 0)
            {
                solutionCount++;
                return;
            }

            int bestIndex = -1;
            int minCandidates = 10;

            for (int i = 0; i < emptyCellsCount; i++)
            {
                var (row, col, box) = emptyCellsArray[i];
                int used = rowUsed[row] | colUsed[col] | boxUsed[box];
                int candidates = 9 - BitCounts[used & 0x1FF];
                if (candidates < minCandidates)
                {
                    minCandidates = candidates;
                    bestIndex = i;
                }
            }

            var (rBest, cBest, boxBest) = emptyCellsArray[bestIndex];
            int usedMask = rowUsed[rBest] | colUsed[cBest] | boxUsed[boxBest];
            int available = ~usedMask & 0x1FF;

            while (available != 0)
            {
                int digit = TrailingZeroCount(available);
                available ^= 1 << digit;

                int mask = 1 << digit;
                board[rBest][cBest] = digit + 1;
                rowUsed[rBest] |= mask;
                colUsed[cBest] |= mask;
                boxUsed[boxBest] |= mask;
                emptyCellsArray[bestIndex] = emptyCellsArray[--emptyCellsCount];

                Backtrack(board);

                board[rBest][cBest] = 0;
                rowUsed[rBest] &= ~mask;
                colUsed[cBest] &= ~mask;
                boxUsed[boxBest] &= ~mask;
                emptyCellsArray[emptyCellsCount++] = emptyCellsArray[bestIndex];
                emptyCellsArray[bestIndex] = (rBest, cBest, boxBest);
            }
        }

        private static int TrailingZeroCount(int n)
        {
            if (n == 0) return 32;
            int count = 0;
            while ((n & 1) == 0)
            {
                count++;
                n >>= 1;
            }
            return count;
        }

        private static int PopCount(int n)
        {
            int count = 0;
            while (n != 0)
            {
                count += n & 1;
                n >>= 1;
            }
            return count;
        }
    }


}
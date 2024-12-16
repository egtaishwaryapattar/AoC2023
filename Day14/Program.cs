using System;
using System.Globalization;
using System.Numerics;

namespace Day11
{
    static class Program
    {
        private enum Direction
        {
            North,
            West, 
            South,
            East
        }

        private class Point
        {
            public int X;
            public int Y;
        }

        static void Main(string[] args)
        {
            var filename = "PuzzleInput.txt";
            var lines = File.ReadAllLines(filename);
            var matrix = ConvertLinesToMatrix(lines);
            //PrintMatrix(matrix);
            const int kNumCycles = 1000000000;
            
            var (barriers, balls) = GetLocationOfBarriersAndBalls(matrix);

            var numCyclesCompleted = 0;
            var sumAfterCycle = new List<int>();
            var loopSize = 0;
            var loop = new List<int>();

            for (var i = 0; i < kNumCycles; i++)
            {
                balls = RollBoard(matrix, Direction.North, barriers, balls);
                balls = RollBoard(matrix, Direction.West, barriers, balls);
                balls = RollBoard(matrix, Direction.South, barriers, balls);
                balls = RollBoard(matrix, Direction.East, barriers, balls);

                var latestSum = GetSumOfBalls(balls, lines.Length);
                sumAfterCycle.Add(latestSum);
                numCyclesCompleted++;

                // every 100 cycles, check if we have got a repeating pattern (want to have two complete cycles)
                if (numCyclesCompleted % 25 == 0)
                {
                    Console.WriteLine($"Num cycles completed: {numCyclesCompleted}");
                    // reverse to make it more efficient to search the latest params
                    sumAfterCycle.Reverse();
                    var numRepeatsFound = 0;
                    var indices = new int[3];

                    for (int index = 0; index < sumAfterCycle.Count; index++)
                    {
                        if (sumAfterCycle[index] == latestSum)
                        {
                            indices[numRepeatsFound] = index;
                            numRepeatsFound++;
                            if (numRepeatsFound == 3) break;
                        }
                    }

                    if (numRepeatsFound == 3)
                    {
                        // have found 2 cycles - check that they are equal length
                        Console.WriteLine($"Repeats found at {indices[0]} {indices[1]} {indices[2]}");
                        if (indices[1] - indices[0] == indices[2] - indices[1])
                        {
                            loopSize = indices[1] - indices[0]; 
                            Console.WriteLine($"Loop size is {loopSize}");

                            bool loopFound = true;
                            loop.Clear();

                            // create a loop so that if extended to the full list, it won't duplicate the last number
                            for (int j = 1; j <= loopSize; j++)
                            {
                                var a = sumAfterCycle[j];
                                var b = sumAfterCycle[j + loopSize];
                                if (a != b)
                                {
                                    loopFound = false;
                                    break;
                                }
                                loop.Add(a);
                            }

                            Console.WriteLine($"Loop found = {loopFound}");
                            if (loopFound)
                            {
                                // reverse the list back
                                sumAfterCycle.Reverse();
                                break;
                            }
                        }
                    }

                    // reverse the list back
                    sumAfterCycle.Reverse();
                }
            }

            // reverse the loop as we identified in reverse order
            loop.Reverse();
            Console.WriteLine("Loop found is:");
            foreach (var value in loop)
            {
                Console.Write($"{value}, ");
            }
            Console.WriteLine();

            // calculate which value in the loop we would end up at
            var cyclesLeft = kNumCycles - numCyclesCompleted;
            var remainder = cyclesLeft % loopSize;
            var finalSum = loop[remainder];
            Console.WriteLine($"The remainder is {remainder} so the sum after {kNumCycles} is {finalSum}");

            //Console.WriteLine();
            //foreach (var value in sumAfterCycle)
            //{
            //    Console.Write($"{value}, ");
            //}
            //Console.WriteLine();
        }

        static char[,] ConvertLinesToMatrix(string[] lines)
        {
            var rows = lines.Length;
            var cols = lines[0].Length;

            char[,] matrix = new char[rows, cols];

            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    matrix[row, col] = (lines[row])[col];
                }
            }
            return matrix;
        }

        // first Item is Barriers. Second item is balls
        static Tuple<List<Point>, List<Point>> GetLocationOfBarriersAndBalls(char[,] matrix)
        {
            var barriers = new List<Point>();
            var balls = new List<Point>();

            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                for (var j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i, j] == '#')
                    {
                        barriers.Add(new Point{X = i, Y = j});
                    }
                    else if (matrix[i, j] == 'O')
                    {
                        balls.Add(new Point { X = i, Y = j });
                    }
                }
            }

            return new Tuple<List<Point>, List<Point>>(barriers, balls);
        }

        static List<Point> RollBoard(char[,] matrix, Direction direction, List<Point> barriers, List<Point> balls)
        {
            var numRows = matrix.GetLength(0);
            var numCols = matrix.GetLength(1);

            switch (direction)
            {
                case Direction.North:
                    // sort balls in ascending Row order
                    balls.Sort(ComparePointsByX);

                    foreach (var ball in balls)
                    {
                        if (ball.X > 0) // exclude the top row as they won't shift when the board is tilted north
                        {
                            // decrement the row until we hit a barrier, ball
                            for (var row = ball.X - 1; row >= 0; row--)
                            {
                                if (IsCellOccupied(new Point { X = row, Y = ball.Y }, barriers, balls))
                                {
                                    // found an end point so stop rolling ball
                                    break;
                                }
                                else
                                {
                                    // no ball or barrier so update position
                                    //matrix[ball.X, ball.Y] = '.';
                                    //matrix[row, ball.Y] = 'O';
                                    ball.X = row;
                                }
                            }
                        }
                    }
                    break;

                case Direction.West:
                    // sort balls in ascending col order
                    balls.Sort(ComparePointsByY);

                    foreach (var ball in balls)
                    {
                        if (ball.Y > 0) // exclude the first col as they won't shift when the board is tilted west
                        {
                            // decrement the col until we hit a barrier, ball
                            for (var col = ball.Y - 1; col >= 0; col--)
                            {
                                if (IsCellOccupied(new Point { X = ball.X, Y = col }, barriers, balls))
                                {
                                    // found an end point so stop rolling ball
                                    break;
                                }
                                else
                                {
                                    // no ball or barrier so update position
                                    //matrix[ball.X, ball.Y] = '.';
                                    //matrix[ball.X, col] = 'O';
                                    ball.Y = col;
                                }
                            }
                        }
                    }
                    break;

                case Direction.South:
                    // sort balls in descending Row order
                    balls.Sort(ComparePointsByX);
                    balls.Reverse();

                    foreach (var ball in balls)
                    {
                        if (ball.X < numRows - 1) // exclude the bottom row as they won't shift when the board is tilted south
                        {
                            // increment the row until we hit a barrier, ball
                            for (var row = ball.X + 1; row < numRows; row++)
                            {
                                if (IsCellOccupied(new Point { X = row, Y = ball.Y }, barriers, balls))
                                {
                                    // found an end point so stop rolling ball
                                    break;
                                }
                                else
                                {
                                    // no ball or barrier so update position
                                    //matrix[ball.X, ball.Y] = '.';
                                    //matrix[row, ball.Y] = 'O';
                                    ball.X = row;
                                }
                            }
                        }
                    }
                    break;

                case Direction.East:
                    // sort balls in descending col order
                    balls.Sort(ComparePointsByY);
                    balls.Reverse();

                    foreach (var ball in balls)
                    {
                        if (ball.Y < numCols - 1) // exclude the last col as they won't shift when the board is tilted east
                        {
                            // decrement the col until we hit a barrier, ball
                            for (var col = ball.Y + 1; col < numCols; col++)
                            {
                                if (IsCellOccupied(new Point { X = ball.X, Y = col }, barriers, balls))
                                {
                                    // found an end point so stop rolling ball
                                    break;
                                }
                                else
                                {
                                    // no ball or barrier so update position
                                    //matrix[ball.X, ball.Y] = '.';
                                    //matrix[ball.X, col] = 'O';
                                    ball.Y = col;
                                }
                            }
                        }
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
            
            //Console.WriteLine($"Direction rolled: {direction}");
            //PrintMatrix(matrix);
            return balls;
        }

        static int ComparePointsByX(Point a, Point b)
        {
            if (a.X == b.X) return 0;
            if (a.X > b.X) return 1;
            return -1; // a.X < b.X
        }

        static int ComparePointsByY(Point a, Point b)
        {
            if (a.Y == b.Y) return 0;
            if (a.Y > b.Y) return 1;
            return -1; // a.X < b.X
        }

        static bool IsCellOccupied(Point point, List<Point> barriers, List<Point> balls)
        {
            return barriers.Any(barrier => barrier.X == point.X && barrier.Y == point.Y) 
                   || balls.Any(ball => ball.X == point.X && ball.Y == point.Y);
        }

        static int GetSumOfBalls(List<Point> balls, int numLines)
        {
            var sum = 0;
            foreach (var ball in balls)
            {
                sum += numLines - ball.X;
            }
            return sum;
        }

        static void PrintMatrix(char[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write($"{matrix[i, j]}");
                }
                Console.Write("\n");
            }
        }

    }
}
using System;
using System.Globalization;
using System.Numerics;

namespace Day11
{
    static class Program
    {
        private class Galaxy
        {
            public int Number;
            public Tuple<int, int> Position;
        }

        static void Main(string[] args)
        {
            string filename = "PuzzleInput.txt";
            var lines = File.ReadAllLines(filename);
            var matrix = ConvertLinesToMatrix(lines);

            var galaxies = ApplyGravitationalEffects(matrix, 1000000);
            var shortestPaths = GetShortestPathBetweenGalaxies(galaxies);

            // get sum of shortest paths
            ulong sum = 0;
            foreach (var path in shortestPaths)
            {
                sum += (ulong)path;
            }

            Console.WriteLine($"Sum of shortest paths = {sum}");
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

        static List<Galaxy> ApplyGravitationalEffects(char[,] matrix, int expansionFactor)
        {
            var numRows = matrix.GetLength(0);
            var numCols = matrix.GetLength(1);

            // determine how many rows and columns need to be expanded
            var emptyRows = new List<int>();
            for (var i = 0; i < numRows; i++)
            {
                bool doesRowHaveGalaxy = false;
                for (int j = 0; j < numCols; j++)
                {
                    if (matrix[i, j] == '#')
                    {
                        doesRowHaveGalaxy = true;
                        break;
                    }
                }
                if (!doesRowHaveGalaxy)
                {
                    emptyRows.Add(i);
                }
            }

            var emptyCols = new List<int>();
            for (var j = 0; j < numCols; j++)
            {
                bool doesColHaveGalaxy = false;
                for (int i = 0; i < numRows; i++)
                {
                    if (matrix[i, j] == '#')
                    {
                        doesColHaveGalaxy = true;
                        break;
                    }
                }
                if (!doesColHaveGalaxy)
                {
                    emptyCols.Add(j);
                }
            }

            // find galaxy locations in expanded matrix
            var galaxyNum = 1;
            var galaxies = new List<Galaxy>();
            var expRow = 0;
            for (var i = 0; i < numRows; i++)
            {
                // reset cols
                var expCol = 0;

                for (var j = 0; j < numCols; j++)
                {
                    if (matrix[i, j] == '#')
                    {
                        // a galaxy column won't be empty
                        var num = galaxyNum.ToString();
                        galaxies.Add(new Galaxy{Number = galaxyNum, Position = new Tuple<int, int>(expRow, expCol)});
                        galaxyNum++;
                        expCol++;
                    }
                    else
                    {
                        expCol = emptyCols.Contains(j)
                            ? expCol + expansionFactor
                            : expCol + 1; 
                    }
                }

                expRow = emptyRows.Contains(i)
                    ? expRow + expansionFactor
                    : expRow + 1;
            }

            return galaxies;
        }

        static List<int> GetShortestPathBetweenGalaxies(List<Galaxy> galaxies)
        {
            // if there are 9 galaxies, there are 8+7+6+5+4+3+2+1=36 paths
            var shortestPaths = new List<int>();
            for (var i = 0; i < galaxies.Count; i++)
            {
                var galaxyA = galaxies[i];

                for (var j = i + 1; j < galaxies.Count; j++)
                {
                    var galaxyB = galaxies[j];

                    // find the number of steps between them
                    var x = Math.Abs(galaxyB.Position.Item2 - galaxyA.Position.Item2);
                    var y = Math.Abs(galaxyB.Position.Item1 - galaxyA.Position.Item1);
                    var steps = x + y;

                    //Console.WriteLine($"Steps between Galaxy {galaxyA.Number} and Galaxy {galaxyB.Number} = {steps}");
                    shortestPaths.Add(steps);
                }
            }
            return shortestPaths;
        }

        static void PrintMatrix(char[,] matrix)
        {
            var numRows = matrix.GetLength(0);
            var numCols = matrix.GetLength(1);

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    Console.Write($"{matrix[i, j]}");
                }
                Console.Write("\n");
            }
        }
    }
}
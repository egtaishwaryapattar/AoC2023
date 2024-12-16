using System;
using System.Data;
using System.Text;

namespace Day3
{
    static class Program
    {
        private static char[,] _matrix;

        static void Main(string[] args)
        {
            var filename = "PuzzleInput.txt";
            var lines = File.ReadAllLines(filename);

            ConvertLinesToMatrix(lines);
            Part2();
        }

        static void ConvertLinesToMatrix(string[] lines)
        {
            var rows = lines.Length;
            var cols = lines[0].Length;

            _matrix = new char[rows, cols];

            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    _matrix[row, col] = (lines[row])[col];
                }
            }
        }

        static void Part1()
        {
            var partNumbers = FindPartNumbers();

            var sum = 0;
            foreach (var part in partNumbers)
            {
                sum += part;
            }

            Console.WriteLine($"Sum of part numbers is {sum}");
        }

        static void Part2()
        {
            var gearRatios = GetGearRatios();
            var sum = 0;
            foreach (var ratio in gearRatios)
            {
                sum += ratio;
            }

            Console.WriteLine($"Sum of part numbers is {sum}");
        }

        static List<int> GetGearRatios()
        {
            var gearRatios = new List<int>();
            
            var numRows = _matrix.GetLength(0);
            var numCols = _matrix.GetLength(1);

            for (var row = 0; row < numRows; row++)
            {
                for (var col = 0; col < numCols; col++)
                {
                    if (_matrix[row, col] == '*')
                    {
                        var surroundingNumbers = GetSurroundingPartNumbers(row, col);

                        if (surroundingNumbers.Count == 2)
                        {
                            var gearRatio = surroundingNumbers[0] * surroundingNumbers[1];
                            gearRatios.Add(gearRatio);
                        }
                    }
                }
            }

            return gearRatios;
        }

        static List<int> FindPartNumbers()
        {
            var partNumbers = new List<int>();
            var numRows = _matrix.GetLength(0);
            var numCols = _matrix.GetLength(1);

            for (var row = 0; row < numRows; row++)
            {
                for (var col = 0; col < numCols; col++)
                {
                    if (IsDigit(_matrix[row, col]))
                    {
                        // found the start digit of a number - find out what the number is
                        var numAsString = new StringBuilder();
                        var numFound = false;
                        var startCol = col;
                        var numDigits = 0;

                        while (!numFound)
                        {
                            if (IsDigit(_matrix[row, col]))
                            {
                                numAsString.Append(_matrix[row, col]);
                                numDigits++;
                                col++;
                                if (col == numCols) numFound = true;
                            }
                            else
                            {
                                //Console.WriteLine($"Found number: {numAsString.ToString()}");
                                numFound = true;
                            }
                        }

                        // find if there is a symbol around it
                        if (IsPartNumber(row, startCol, numDigits))
                        {
                            var number = Convert.ToInt16(numAsString.ToString());
                            partNumbers.Add(number);
                            //Console.WriteLine($"{number} is a part number");
                        }
                        else
                        {
                            Console.WriteLine($"{numAsString.ToString()} is NOT a part number");
                        }
                    }
                }
            }

            return partNumbers;
        }

        static bool IsDigit(char c)
        {
            if (c == '0') return true;
            if (c == '1') return true;
            if (c == '2') return true;
            if (c == '3') return true;
            if (c == '4') return true;
            if (c == '5') return true;
            if (c == '6') return true;
            if (c == '7') return true;
            if (c == '8') return true;
            if (c == '9') return true;
            return false;
        }

        static bool IsSymbol(char c)
        {
            return !IsDigit(c) && c != '.';
        }

        static bool IsPartNumber(int row, int startCol, int numDigits)
        {
            var symbolFound = false;

            for (var i = 0; i < numDigits; i++)
            {
                if (i == 0)
                {
                    symbolFound = CheckToLeftOfFirstDigit(row, startCol + i);
                    if (symbolFound) break;
                }
                if (i == numDigits - 1)
                {
                    symbolFound = CheckToRightOfLastDigit(row, startCol + i);
                    if (symbolFound) break;
                }

                symbolFound = CheckAboveAndBelowDigit(row, startCol + i);
                if (symbolFound) break;
            }

            return symbolFound;
        }

        static bool CheckToLeftOfFirstDigit(int row, int col)
        {
            if (col - 1 >= 0)
            {
                // check left
                var isSymbol = IsSymbol(_matrix[row, col - 1]);
                if (isSymbol) return true;

                // check north west
                if (row - 1 >= 0)
                {
                    isSymbol = IsSymbol(_matrix[row - 1, col - 1]);
                    if (isSymbol) return true;
                }

                // check south west
                if (row + 1 < _matrix.GetLength(0))
                {
                    isSymbol = IsSymbol(_matrix[row + 1, col - 1]);
                    if (isSymbol) return true;
                }
            }

            return false;
        }

        static bool CheckToRightOfLastDigit(int row, int col)
        {
            var numCols = _matrix.GetLength(1);
            if (col + 1 < numCols)
            {
                // check right
                var isSymbol = IsSymbol(_matrix[row, col + 1]);
                if (isSymbol) return true;

                // check north east
                if (row - 1 >= 0)
                {
                    isSymbol = IsSymbol(_matrix[row - 1, col + 1]);
                    if (isSymbol) return true;
                }

                // check south east
                if (row + 1 < _matrix.GetLength(0))
                {
                    isSymbol = IsSymbol(_matrix[row + 1, col + 1]);
                    if (isSymbol) return true;
                }
            }

            return false;
        }

        static bool CheckAboveAndBelowDigit(int row, int col)
        {
            // check above
            if (row - 1 >= 0)
            {
                var isSymbol = IsSymbol(_matrix[row - 1, col]);
                if (isSymbol) return true;
            }

            // check below
            if (row + 1 < _matrix.GetLength(0))
            {
                var isSymbol = IsSymbol(_matrix[row + 1, col]);
                if (isSymbol) return true;
            }

            return false;
        }

        static List<int> GetSurroundingPartNumbers(int row, int col)
        {
            var numRows = _matrix.GetLength(0);
            var numCols = _matrix.GetLength(1);
            var partNums = new List<int>();

            // check top row for number(s)
            if (row - 1 >= 0)
            {
                var nums = GetNumberInRow(row - 1, col);
                if (nums.Count != 0)
                {
                    partNums.AddRange(nums);
                }
            }

            // check left for number
            if (col - 1 >= 0)
            {
                if (IsDigit(_matrix[row, col - 1]))
                {
                    var value = GetNumberToLeftOfDigit(row, col - 1);
                    partNums.Add(value);
                }
            }

            // check right for number
            if (col + 1 < numCols)
            {
                if (IsDigit(_matrix[row, col + 1]))
                {
                    var value = GetNumberToRightOfDigit(row, col + 1);
                    partNums.Add(value);
                }
            }

            // check bottom row for number(s)
            if (row + 1 < numRows)
            {
                var nums = GetNumberInRow(row + 1, col);
                if (nums.Count != 0)
                {
                    partNums.AddRange(nums);
                }
            }

            return partNums;
        }

        static int GetValueForChars(List<char> chars)
        {
            var sb = new StringBuilder();
            foreach (var c in chars)
            {
                sb.Append(c);
            }

            return Convert.ToInt16(sb.ToString());
        }

        // row and col refer to position to start from
        static int GetNumberToRightOfDigit(int row, int col)
        {
            var numCols = _matrix.GetLength(1);
            var c = _matrix[row, col];
            var chars = new List<char> { c };

            if (col + 1 < numCols)
            {
                var b = _matrix[row, col + 1];
                if (IsDigit(b))
                {
                    chars.Add(b);
                    if (col + 2 < numCols)
                    {
                        var a = _matrix[row, col + 2];
                        if (IsDigit(a))
                        {
                            chars.Add(a);
                        }
                    }
                }
            }
            return GetValueForChars(chars);
        }

        static int GetNumberToLeftOfDigit(int row, int col)
        {
            var c = _matrix[row, col];
            var chars = new List<char> { c };
            if (col - 1 >= 0)
            {
                var b = _matrix[row, col - 1];
                if (IsDigit(b))
                {
                    chars.Add(b);
                    if (col - 2 >= 0)
                    {
                        var a = _matrix[row, col - 2];
                        if (IsDigit(a))
                        {
                            chars.Add(a);
                        }
                    }
                }
            }
            chars.Reverse();
            return GetValueForChars(chars);
        }

        static List<int> GetNumberInRow(int row, int middleCol)
        {
            bool leftIsDigit = false;
            bool middleIsDigit = IsDigit(_matrix[row,middleCol]);
            bool rightIsDigit = false;

            if (middleCol - 1 >= 0)
            {
                leftIsDigit = IsDigit(_matrix[row, middleCol - 1]);
            }

            if (middleCol + 1 < _matrix.GetLength(1))
            {
                rightIsDigit = IsDigit(_matrix[row, middleCol + 1]);
            }

            // if all 3 are digits, they make up a number
            if (leftIsDigit && middleIsDigit && rightIsDigit)
            {
                var chars = new List<char>
                {
                    _matrix[row, middleCol - 1],
                    _matrix[row, middleCol],
                    _matrix[row, middleCol + 1]
                };
                var value = GetValueForChars(chars);
                return new List<int> { value };
            }

            // if there is one number on the left
            if (leftIsDigit && middleIsDigit && !rightIsDigit)
            {
                var value = GetNumberToLeftOfDigit(row, middleCol);
                return new List<int> { value }; 
            }

            // if there is one number on the right
            if (!leftIsDigit && middleIsDigit && rightIsDigit)
            {
                var value = GetNumberToRightOfDigit(row, middleCol);
                return new List<int> { value };
            }

            // there could be a number just in the middle
            if (!leftIsDigit && middleIsDigit && !rightIsDigit)
            {
                return new List<int>() { Convert.ToInt16((_matrix[row, middleCol]).ToString()) };
            }

            // or there could be a number of the left and/or right
            var numbers = new List<int>();

            if (leftIsDigit)
            {
                var value = GetNumberToLeftOfDigit(row, middleCol - 1);
                numbers.Add(value);
            }

            if (rightIsDigit)
            {
                var value = GetNumberToRightOfDigit(row, middleCol + 1);
                numbers.Add(value);
            }

            return numbers;
        }
    }
}
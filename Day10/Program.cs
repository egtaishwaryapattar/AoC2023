using System;
using System.Linq;
using System.Numerics;

namespace Day9
{
    static class Program
    {
        private enum Direction
        {
            Up,
            Right,
            Down,
            Left
        }

        private class CellInfo
        {
            public char Char;
            public Tuple<int, int> Position;
            public Direction Direction;
        }

        private static char[,] inputFile;

        static void Main(string[] args)
        {
            var filename = "PuzzleInput.txt";
            var lines = File.ReadAllLines(filename);

            // Part 1
            var validRoute = GetValidLoop(lines);

            // Part2
            ConvertLinesToMatrix(lines);
            ReplaceNonRouteCells(validRoute);

            var numInside = GetTilesInsideRoute(validRoute);
            PrintMatrix();
            Console.WriteLine($"Tiles inside route = {numInside}");
        }
        static List<CellInfo> GetValidLoop(string[] lines)
        {
            var startPoint = GetStartPoint(lines);

            // from the start position, there are multiple places you can go (up, right, down, left)
            var upRoute = GetPossibleRoute(lines, startPoint, Direction.Up);
            var rightRoute = GetPossibleRoute(lines, startPoint, Direction.Right);
            var downRoute = GetPossibleRoute(lines, startPoint, Direction.Down);
            var leftRoute = GetPossibleRoute(lines, startPoint, Direction.Left);

            var allRoutes = new List<List<CellInfo>>
            {
                upRoute,
                rightRoute,
                downRoute,
                leftRoute
            };

            foreach (var route in allRoutes)
            {
                var length = route.Count;

                if (length > 1) // all will start with 'S' at the beginning
                {
                    if (route[length - 1].Char == 'S')
                    {
                        // found the route that loops.furthest away point is the midpoint.
                        int midpoint;
                        if (length % 2 == 1)
                        {
                            // odd number
                            midpoint = (length - 1) / 2;
                        }
                        else
                        {
                            // even number
                            midpoint = length / 2;
                        }
                        Console.WriteLine($"Number of steps to midpoint in loop is {midpoint}");
                        return route;
                    }
                }
            }

            throw new Exception("Valid route not found");
        }

        static Tuple<int, int> GetStartPoint(string[] lines)
        {
            for (var i = 0; i < lines.Length; i++)
            {
                var startCol = lines[i].IndexOf('S');
                if (startCol != -1)
                {
                    return new Tuple<int, int>(i, startCol);
                }
            }

            throw new Exception("Start Point not found in file");
        }

        static List<CellInfo> GetPossibleRoute(string[] lines, Tuple<int, int> startPoint, Direction startDirection)
        {
            // create loop and initialise with starting point
            var path = new List<CellInfo> { new CellInfo{Char = 'S', Position = startPoint} };
            
            var currentRow = startPoint.Item1;
            var currentCol = startPoint.Item2;
            var direction = startDirection;
            var endOfLoop = false;

            while (!endOfLoop)
            {
                // look at the character in the next position
                Tuple<int, int> nextPos;
                string validChars;

                switch (direction)
                {
                    case Direction.Up:
                        nextPos = new Tuple<int, int>(currentRow - 1, currentCol);
                        validChars = "|7F";
                        break;
                    case Direction.Right:
                        nextPos = new Tuple<int, int>(currentRow, currentCol + 1);
                        validChars = "-J7";
                        break;
                    case Direction.Down:
                        nextPos = new Tuple<int, int>(currentRow + 1, currentCol);
                        validChars = "|JL";
                        break;
                    case Direction.Left:
                        nextPos = new Tuple<int, int>(currentRow, currentCol - 1);
                        validChars = "-LF";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(startDirection), startDirection, null);
                }
                
                // get next character
                char nextChar;
                try
                {
                    var line = lines[nextPos.Item1];
                    nextChar = line[nextPos.Item2];
                }
                catch (Exception e)
                {
                    // the next character to go to may be outside the edge of the board
                    endOfLoop = true;
                    break;
                }

                // check if the character in the next position is valid
                if (validChars.Contains(nextChar))
                {
                    direction = GetNextDirection(nextChar, direction);
                    currentRow = nextPos.Item1;
                    currentCol = nextPos.Item2;
                    path.Add(new CellInfo { Char = nextChar, Position = nextPos, Direction = direction });
                }
                else
                {
                    if (nextChar == 'S')
                    {
                        path.Add(new CellInfo { Char = nextChar, Position = nextPos});
                    }
                    endOfLoop = true;
                }
            }
            return path;
        }

        static Direction GetNextDirection(char c, Direction currentDirection)
        {
            switch (currentDirection)
            {
                case Direction.Up:
                    // valid chars are: "|7F"
                    if (c == '|') return Direction.Up;
                    if (c == '7') return Direction.Left;
                    if (c == 'F') return Direction.Right;
                    break;
                case Direction.Right:
                    // valid chars are: "-J7"
                    if (c == '-') return Direction.Right;
                    if (c == 'J') return Direction.Up;
                    if (c == '7') return Direction.Down;
                    break;
                case Direction.Down:
                    // valid chars are: "|JL"
                    if (c == '|') return Direction.Down;
                    if (c == 'J') return Direction.Left;
                    if (c == 'L') return Direction.Right;
                    break;
                case Direction.Left:
                    // valid chars are: "-LF"
                    if (c == '-') return Direction.Left;
                    if (c == 'L') return Direction.Up;
                    if (c == 'F') return Direction.Down;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currentDirection), currentDirection, null);
            }
            throw new ArgumentOutOfRangeException(nameof(currentDirection), currentDirection, null);
        }

        static void ConvertLinesToMatrix(string[] lines)
        {
            var rows = lines.Length;
            var cols = lines[0].Length;

            char[,] matrix = new char[rows, cols];

            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    matrix[row,col] = (lines[row])[col];
                }
            }
            inputFile = matrix;
        }

        static int GetTilesInsideRoute(List<CellInfo> route)
        {
            var numI = 0;
            var numO = 0;

            // using the right hand rule. Assuming the path is going clockwise, label everything on the left as O and on the right as I
            // handle the S last as we don't know what the symbol is and it is likely that at the end, all the O's and I's are populated
            for (var i = 1; i < route.Count; i++) // omit the first S
            {
                if (route[i].Char == '|')
                {
                    if (route[i].Direction == Direction.Up)
                    {
                        numI += PopulateCellsWithChar('I', route[i].Position, Direction.Right, route);
                        numO += PopulateCellsWithChar('O', route[i].Position, Direction.Left, route);
                    }
                    else if (route[i].Direction == Direction.Down)
                    {
                        numI += PopulateCellsWithChar('I', route[i].Position, Direction.Left, route);
                        numO += PopulateCellsWithChar('O', route[i].Position, Direction.Right, route);
                    }
                    else { throw new Exception("Invalid direction for |"); }
                }
                else if (route[i].Char == '-') 
                {
                    if (route[i].Direction == Direction.Right)
                    {
                        numI += PopulateCellsWithChar('I', route[i].Position, Direction.Down, route);
                        numO += PopulateCellsWithChar('O', route[i].Position, Direction.Up, route);
                    }
                    else if (route[i].Direction == Direction.Left)
                    {
                        numI += PopulateCellsWithChar('I', route[i].Position, Direction.Up, route);
                        numO += PopulateCellsWithChar('O', route[i].Position, Direction.Down, route);
                    }
                    else { throw new Exception("Invalid direction for -"); }
                }
                else if (route[i].Char == 'J')
                {
                    if (route[i].Direction == Direction.Up)
                    {
                        numI += PopulateCellsWithChar('I', route[i].Position, Direction.Right, route);
                        numI += PopulateCellsWithChar('I', route[i].Position, Direction.Down, route);
                        numO += PopulateCellsWithChar('O', route[i].Position, Direction.Left, route);
                        numO += PopulateCellsWithChar('O', route[i].Position, Direction.Up, route);
                    }
                    else if (route[i].Direction == Direction.Left)
                    {
                        numI += PopulateCellsWithChar('I', route[i].Position, Direction.Up, route);
                        numI += PopulateCellsWithChar('I', route[i].Position, Direction.Left, route);
                        numO += PopulateCellsWithChar('O', route[i].Position, Direction.Down, route);
                        numO += PopulateCellsWithChar('O', route[i].Position, Direction.Right, route);
                    }
                    else { throw new Exception("Invalid direction for J"); }
                }
                else if (route[i].Char == 'F')
                {
                    if (route[i].Direction == Direction.Right)
                    {
                        numI += PopulateCellsWithChar('I', route[i].Position, Direction.Down, route);
                        numI += PopulateCellsWithChar('I', route[i].Position, Direction.Right, route);
                        numO += PopulateCellsWithChar('O', route[i].Position, Direction.Up, route);
                        numO += PopulateCellsWithChar('O', route[i].Position, Direction.Left, route);
                    }
                    else if (route[i].Direction == Direction.Down)
                    {
                        numI += PopulateCellsWithChar('I', route[i].Position, Direction.Left, route);
                        numI += PopulateCellsWithChar('I', route[i].Position, Direction.Up, route);
                        numO += PopulateCellsWithChar('O', route[i].Position, Direction.Right, route);
                        numO += PopulateCellsWithChar('O', route[i].Position, Direction.Down, route);
                    }
                    else { throw new Exception("Invalid direction for F"); }
                }
                else if (route[i].Char == 'L')
                {
                    if (route[i].Direction == Direction.Up)
                    {
                        numI += PopulateCellsWithChar('I', route[i].Position, Direction.Right, route);
                        numI += PopulateCellsWithChar('I', route[i].Position, Direction.Up, route);
                        numO += PopulateCellsWithChar('O', route[i].Position, Direction.Left, route);
                        numO += PopulateCellsWithChar('O', route[i].Position, Direction.Down, route);
                    }
                    else if (route[i].Direction == Direction.Right)
                    {
                        numI += PopulateCellsWithChar('I', route[i].Position, Direction.Down, route);
                        numI += PopulateCellsWithChar('I', route[i].Position, Direction.Left, route);
                        numO += PopulateCellsWithChar('O', route[i].Position, Direction.Up, route);
                        numO += PopulateCellsWithChar('O', route[i].Position, Direction.Right, route);
                    }
                    else { throw new Exception("Invalid direction for L"); }
                }
                else if (route[i].Char == '7')
                {
                    if (route[i].Direction == Direction.Down)
                    {
                        numI += PopulateCellsWithChar('I', route[i].Position, Direction.Left, route);
                        numI += PopulateCellsWithChar('I', route[i].Position, Direction.Down, route);
                        numO += PopulateCellsWithChar('O', route[i].Position, Direction.Right, route);
                        numO += PopulateCellsWithChar('O', route[i].Position, Direction.Up, route);
                    }
                    else if (route[i].Direction == Direction.Left)
                    {
                        numI += PopulateCellsWithChar('I', route[i].Position, Direction.Up, route);
                        numI += PopulateCellsWithChar('I', route[i].Position, Direction.Right, route);
                        numO += PopulateCellsWithChar('O', route[i].Position, Direction.Down, route);
                        numO += PopulateCellsWithChar('O', route[i].Position, Direction.Left, route);
                    }
                    else { throw new Exception("Invalid direction for 7"); }
                }
                else if (route[i].Char == 'S') {/*ignore*/}
                else {throw new Exception("Invalid character"); }
            }

            // figure out whether I or O corresponds to the outside - look along the first row for I or O
            var outsideChar = ' ';
            for (var i = 0; i < inputFile.GetLength(1); i++)
            {
                if (inputFile[0,i] == 'I')
                {
                    outsideChar = 'I';
                    Console.WriteLine("Outside char is I");
                    break;
                }

                if (inputFile[0, i] == 'O')
                {
                    outsideChar = 'O';
                    Console.WriteLine("Outside char is O");
                    break;
                }
            }

            // return the INSIDE
            if (outsideChar == 'O') return numI;
            if (outsideChar == 'I') return numO;
            throw new Exception("Didn't find an O or I on the outside");
        }

        // row and col corresponds to the cell we want to compare and check if it exists on the route
        static bool IsCellOnRoute(List<CellInfo> route, int row, int col)
        {
            foreach (var cell in route)
            {
                if (cell.Position.Item1 == row
                    && cell.Position.Item2 == col)
                {
                    return true;
                }
            }
            return false;
        }

        static int PopulateCellsWithChar(char populateChar, Tuple<int, int> currentPos, Direction direction, 
            List<CellInfo> route)
        {
            var numRows = inputFile.GetLength(0);
            var numCols = inputFile.GetLength(1);
            var numCharsWritten = 0;

            // populate cells until we hit the route, another element of the same character, or the edge of the board
            switch (direction)
            {
                case Direction.Up:
                    for (var row = currentPos.Item1 - 1; row >= 0; row--)
                    {
                        if (inputFile[row, currentPos.Item2] == '.')
                        {
                            inputFile[row, currentPos.Item2] = populateChar;
                            numCharsWritten++; 
                        }
                        else if (inputFile[row, currentPos.Item2] != populateChar)
                        {
                            // allow to hop over character that is same as populatedChar.
                            // But if it is not this character, then we found end point of populating
                            break;
                        }
                    }
                    break;
                case Direction.Right:
                    for (var col = currentPos.Item2 + 1; col < numCols; col++)
                    {
                        if (inputFile[currentPos.Item1, col] == '.')
                        {
                            inputFile[currentPos.Item1, col] = populateChar;
                            numCharsWritten++;
                        }
                        else if (inputFile[currentPos.Item1, col] != populateChar)
                        {
                            // allow to hop over character that is same as populatedChar.
                            // But if it is not this character, then we found end point of populating
                            break;
                        }
                    }
                    break;
                case Direction.Down:
                    for (var row = currentPos.Item1 + 1; row < numRows; row++)
                    {
                        if (inputFile[row, currentPos.Item2] == '.')
                        {
                            inputFile[row, currentPos.Item2] = populateChar;
                            numCharsWritten++;
                        }
                        else if (inputFile[row, currentPos.Item2] != populateChar)
                        {
                            // allow to hop over character that is same as populatedChar.
                            // But if it is not this character, then we found end point of populating
                            break;
                        }
                    }
                    break;
                case Direction.Left:
                    for (var col = currentPos.Item2 - 1; col >= 0; col--)
                    {
                        if (inputFile[currentPos.Item1, col] == '.')
                        {
                            inputFile[currentPos.Item1, col] = populateChar;
                            numCharsWritten++;
                        }
                        else if (inputFile[currentPos.Item1, col] != populateChar)
                        {
                            // allow to hop over character that is same as populatedChar.
                            // But if it is not this character, then we found end point of populating
                            break;
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            return numCharsWritten;
        }

        static void ReplaceNonRouteCells(List<CellInfo> route)
        {
            var numRows = inputFile.GetLength(0);
            var numCols = inputFile.GetLength(1);

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    if (!IsCellOnRoute(route, i, j))
                    {
                        inputFile[i, j] = '.';
                    }
                }
            }
        }

        static void PrintMatrix()
        {
            var numRows = inputFile.GetLength(0);
            var numCols = inputFile.GetLength(1);

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    Console.Write($"{inputFile[i,j]}" );
                }
                Console.Write("\n");
            }
        }
    }
}
using System;
using System.Globalization;
using System.Numerics;
using static Day16.Program;

namespace Day16
{
    static class Program
    {
        private enum DirectionOfTravel
        {
            Up, Down, Left, Right
        }

        private enum MirrorType
        {
            ForwardSlash,   //      '/'
            Backslash,      //      '\'
            Vertical,       //      '|'
            Horizontal      //      '-'
        }

        public class EnergizedCell
        {
            public Tuple<int, int> Position;
            public bool ContainsMirror;
            public bool HorizontalPassage;
            public bool VerticalPassage;
        }

        private static char[,] _matrix;
        private static List<EnergizedCell> _energizedCells = new List<EnergizedCell>();

        static void Main(string[] args)
        {
            var filename = "PuzzleInput.txt";
            var lines = File.ReadAllLines(filename);
            
            Part2(lines);
        }

        static void Part1(string[] lines)
        {
            _matrix = ConvertLinesToMatrix(lines);
            //PrintMatrix(_matrix);
            FollowLightPath(new Tuple<int, int>(0, 0), DirectionOfTravel.Right);

            //Console.WriteLine("With light paths:");
            //PrintMatrix(_matrix);

            Console.WriteLine($"Number of energized cells are: {_energizedCells.Count}");
        }

        static void Part2(string[] lines)
        {
            var maxEnergizedCells = 0;
            var numRows = lines.Length;
            var numCols = lines[0].Length;

            // start with rows - going left or right
            for (var i = 0; i < numRows; i++)
            {
                // start from left edge going right
                _matrix = ConvertLinesToMatrix(lines);
                _energizedCells.Clear();
                FollowLightPath(new Tuple<int, int>(i, 0), DirectionOfTravel.Right);
                if (_energizedCells.Count > maxEnergizedCells)
                {
                    maxEnergizedCells = _energizedCells.Count;
                }

                // start from right edge going left
                _matrix = ConvertLinesToMatrix(lines);
                _energizedCells.Clear();
                FollowLightPath(new Tuple<int, int>(i, numCols - 1), DirectionOfTravel.Left);
                if (_energizedCells.Count > maxEnergizedCells)
                {
                    maxEnergizedCells = _energizedCells.Count;
                }
            }

            // cols - going up or down
            for (var j = 0; j < numCols; j++)
            {
                // start from top edge going down
                _matrix = ConvertLinesToMatrix(lines);
                _energizedCells.Clear();
                FollowLightPath(new Tuple<int, int>(0, j), DirectionOfTravel.Down);
                if (_energizedCells.Count > maxEnergizedCells)
                {
                    maxEnergizedCells = _energizedCells.Count;
                }

                // start from bottom edge going up
                _matrix = ConvertLinesToMatrix(lines);
                _energizedCells.Clear();
                FollowLightPath(new Tuple<int, int>(numRows - 1, j), DirectionOfTravel.Up);
                if (_energizedCells.Count > maxEnergizedCells)
                {
                    maxEnergizedCells = _energizedCells.Count;
                }
            }

            Console.WriteLine($"Max Number of energized cells is {maxEnergizedCells}");
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

        static void FollowLightPath(Tuple<int, int> startingPos, DirectionOfTravel startingDirection)
        {
            // follow light path until we hit a wall or path of light traveling in the same direction

            var row = startingPos.Item1;
            var col = startingPos.Item2;
            var direction = startingDirection;
            bool endPointFound = false;

            while (!endPointFound)
            {
                // c can be a '.' to indicate untraveled empty spot,
                // a mirror '/' '\' or a splitter '|' '-'
                // or '#' to represent light has traveled here
                var c = _matrix[row, col];

                if (c == '.')
                {
                    _matrix[row, col] = '#';
                    //Console.WriteLine("Cell has been energized");
                    //PrintMatrix(_matrix);

                    _energizedCells.Add(new EnergizedCell()
                    {
                        Position = new Tuple<int, int>(row, col), 
                        VerticalPassage = direction == DirectionOfTravel.Up || direction == DirectionOfTravel.Down, 
                        HorizontalPassage = direction == DirectionOfTravel.Left || direction == DirectionOfTravel.Right
                    });
                    
                    ((row, col), endPointFound) = GetNextPosition(row, col, direction);
                }
                else if (c == '#')
                {
                    // found a light path - determine if we are crossing it or following it
                    foreach (var cell in _energizedCells)
                    {
                        if (cell.Position.Item1 == row
                            && cell.Position.Item2 == col)
                        {
                            if (direction == DirectionOfTravel.Up || direction == DirectionOfTravel.Down)
                            {
                                if (cell.VerticalPassage)
                                {
                                    endPointFound = true;
                                    break;
                                }
                                cell.VerticalPassage = true;
                            }
                            else
                            {
                                if (cell.HorizontalPassage)
                                {
                                    endPointFound = true;
                                    break;
                                }
                                cell.HorizontalPassage = true;
                            }

                            ((row, col), endPointFound) = GetNextPosition(row, col, direction);
                            break;
                        }
                    }
                }
                else
                {
                    // will be a mirror or splitter
                    if (!HasCellBeenEnergized(row, col))
                    {
                        _energizedCells.Add(new EnergizedCell()
                        {
                            Position = new Tuple<int, int>(row, col),
                            ContainsMirror = true
                        });
                    }
                    
                    // based on mirror type, decide what to do next and update next position and direction
                    switch (direction)
                    {
                        case DirectionOfTravel.Up:
                            switch (GetMirrorType(c))
                            {
                                case MirrorType.ForwardSlash:
                                    direction = DirectionOfTravel.Right;
                                    ((row, col), endPointFound) = GetNextPosition(row, col, direction);
                                    break;

                                case MirrorType.Backslash:
                                    direction = DirectionOfTravel.Left;
                                    ((row, col), endPointFound) = GetNextPosition(row, col, direction);
                                    break;

                                case MirrorType.Vertical:
                                    ((row, col), endPointFound) = GetNextPosition(row, col, direction);
                                    break;

                                case MirrorType.Horizontal:
                                    // split so one goes left and one goes right with the current position as the start position
                                    FollowLightPath(new Tuple<int, int>(row,col), DirectionOfTravel.Left);
                                    FollowLightPath(new Tuple<int, int>(row,col), DirectionOfTravel.Right);
                                    endPointFound = true; 
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            break;

                        case DirectionOfTravel.Down:
                            switch (GetMirrorType(c))
                            {
                                case MirrorType.ForwardSlash:
                                    direction = DirectionOfTravel.Left;
                                    ((row, col), endPointFound) = GetNextPosition(row, col, direction);
                                    break;

                                case MirrorType.Backslash:
                                    direction = DirectionOfTravel.Right;
                                    ((row, col), endPointFound) = GetNextPosition(row, col, direction);
                                    break;

                                case MirrorType.Vertical:
                                    ((row, col), endPointFound) = GetNextPosition(row, col, direction);
                                    break;

                                case MirrorType.Horizontal:
                                    // split so one goes left and one goes right with the current position as the start position
                                    FollowLightPath(new Tuple<int, int>(row, col), DirectionOfTravel.Left);
                                    FollowLightPath(new Tuple<int, int>(row, col), DirectionOfTravel.Right);
                                    endPointFound = true;
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            break;

                        case DirectionOfTravel.Left:
                            switch (GetMirrorType(c))
                            {
                                case MirrorType.ForwardSlash:
                                    direction = DirectionOfTravel.Down;
                                    ((row, col), endPointFound) = GetNextPosition(row, col, direction);
                                    break;

                                case MirrorType.Backslash:
                                    direction = DirectionOfTravel.Up;
                                    ((row, col), endPointFound) = GetNextPosition(row, col, direction);
                                    break;

                                case MirrorType.Vertical:
                                    // split so one goes up and one goes down with the current position as the start position
                                    FollowLightPath(new Tuple<int, int>(row, col), DirectionOfTravel.Up);
                                    FollowLightPath(new Tuple<int, int>(row, col), DirectionOfTravel.Down);
                                    endPointFound = true;
                                    break;

                                case MirrorType.Horizontal:
                                    ((row, col), endPointFound) = GetNextPosition(row, col, direction);
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            break;

                        case DirectionOfTravel.Right:
                            switch (GetMirrorType(c))
                            {
                                case MirrorType.ForwardSlash:
                                    direction = DirectionOfTravel.Up;
                                    ((row, col), endPointFound) = GetNextPosition(row, col, direction);
                                    break;

                                case MirrorType.Backslash:
                                    direction = DirectionOfTravel.Down;
                                    ((row, col), endPointFound) = GetNextPosition(row, col, direction);
                                    break;

                                case MirrorType.Vertical:
                                    // split so one goes up and one goes down with the current position as the start position
                                    FollowLightPath(new Tuple<int, int>(row, col), DirectionOfTravel.Up);
                                    FollowLightPath(new Tuple<int, int>(row, col), DirectionOfTravel.Down);
                                    endPointFound = true;
                                    break;

                                case MirrorType.Horizontal:
                                    ((row, col), endPointFound) = GetNextPosition(row, col, direction);
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    
                }

            }
        }

        static Tuple<Tuple<int, int>, bool> GetNextPosition(int row, int col, DirectionOfTravel direction)
        {
            bool endPointFound = false;

            // find next position and direction
            switch (direction)
            {
                case DirectionOfTravel.Up:
                    // go up (decrement the row)
                    row = row - 1;
                    if (row < 0) endPointFound = true;
                    break;

                case DirectionOfTravel.Down:
                    // go down (increment the row)
                    row = row + 1;
                    if (row >= _matrix.GetLength(0)) endPointFound = true;
                    break;

                case DirectionOfTravel.Left:
                    // go left (decrement the column)
                    col = col - 1;
                    if (col < 0) endPointFound = true;
                    break;

                case DirectionOfTravel.Right:
                    // go right (increment the column)
                    col = col + 1;
                    if (col >= _matrix.GetLength(1)) endPointFound = true;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            var nextPos = new Tuple<int, int>(row, col);
            return new Tuple<Tuple<int, int>, bool>(nextPos, endPointFound);
        }

        static MirrorType GetMirrorType(char c)
        {
            if (c == '/') return MirrorType.ForwardSlash;
            if (c == '\\') return MirrorType.Backslash;
            if (c == '|') return MirrorType.Vertical;
            if (c == '-') return MirrorType.Horizontal;

            throw new Exception("Unrecognised mirror type");
        }

        static bool HasCellBeenEnergized(int row, int col)
        {
            var cellEnergized = false;

            foreach (var cell in _energizedCells)
            {
                if (cell.Position.Item1 == row
                    && cell.Position.Item2 == col)
                {
                    cellEnergized = true;
                    break;
                }
            }
            return cellEnergized;
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
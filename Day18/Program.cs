using System;
using System.Drawing;
using System.Globalization;
using System.Numerics;

namespace Day18
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

        private class Trench
        {
            public Direction Direction;
            public Tuple<int, int> Position; // relative position
            public bool IsBoundary;
        }

        static void Main(string[] args)
        {
            var filename = "Test1.txt";
            var lines = File.ReadAllLines(filename);

            var trenches = GetTrenchesPart2(lines);
            Console.WriteLine($"Size of trench dug up is {trenches.Count}");

            trenches = DigUpTrenchInterior(trenches, true);
            Console.WriteLine($"Size of trench dug up is {trenches.Count}");
        }

        static List<Trench> GetTrenchesPart1(string[] lines)
        {
            var trenches = new List<Trench>();

            // start the position from 0,0. should also be the end position
            var position = Tuple.Create(0, 0);

            foreach (var line in lines)
            {
                var splitString = line.Split(' ');

                Direction direction;
                switch (splitString[0])
                {
                    case "R":
                        direction = Direction.Right;
                        break;
                    case "L":
                        direction = Direction.Left;
                        break;
                    case "U":
                        direction = Direction.Up;
                        break;
                    case "D":
                        direction = Direction.Down;
                        break;
                    default:
                        throw new Exception("Invalid direction");
                        break;
                }
                
                for (var i = 0; i < Convert.ToInt16(splitString[1]); i++)
                {
                    position = GetNextPosition(position, direction);
                    trenches.Add(new Trench(){ Direction = direction, Position = position, IsBoundary = true});
                }
            }

            return trenches;
        }

        static List<Trench> GetTrenchesPart2(string[] lines)
        {
            var trenches = new List<Trench>();

            // start the position from 0,0. should also be the end position
            var position = Tuple.Create(0, 0);

            foreach (var line in lines)
            {
                var splitString = line.Split(' ');

                var colourString = (splitString[2]).Substring(2, 6);
                var numSteps = Int32.Parse(colourString.Substring(0, 5), System.Globalization.NumberStyles.HexNumber);

                Direction direction;
                switch (colourString[5])
                {
                    case '0':
                        direction = Direction.Right;
                        break;
                    case '2':
                        direction = Direction.Left;
                        break;
                    case '3':
                        direction = Direction.Up;
                        break;
                    case '1':
                        direction = Direction.Down;
                        break;
                    default:
                        throw new Exception("Invalid direction");
                        break;
                }
                
                for (var i = 0; i < numSteps; i++)
                {
                    position = GetNextPosition(position, direction);
                    trenches.Add(new Trench() { Direction = direction, Position = position, IsBoundary = true });
                }
            }

            return trenches;
        }

        static Tuple<int, int> GetNextPosition(Tuple<int, int> currentPos, Direction direction)
        {
            Tuple<int, int> nextPos;

            switch (direction)
            {
                case Direction.Up:
                    nextPos = new Tuple<int, int>(currentPos.Item1 - 1, currentPos.Item2);
                    break;
                case Direction.Right:
                    nextPos = new Tuple<int, int>(currentPos.Item1, currentPos.Item2 + 1);
                    break;
                case Direction.Down:

                    nextPos = new Tuple<int, int>(currentPos.Item1 + 1, currentPos.Item2);
                    break;
                case Direction.Left:
                    nextPos = new Tuple<int, int>(currentPos.Item1, currentPos.Item2 - 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            return nextPos;
        }

        static List<Trench> DigUpTrenchInterior(List<Trench> boundaryTrenches, bool clockwise)
        {
            // if clockwise, follow right hand rule
            // if anti-clockwise, follow left hand rule

            var filledTrenches = new List<Trench>();

            // get filled trenches
            foreach (var trench in boundaryTrenches)
            {
                var currentPos = trench.Position;
                switch (trench.Direction)
                {
                    case Direction.Up:
                        filledTrenches = GetFilledTrenches(currentPos, boundaryTrenches, filledTrenches, clockwise ? Direction.Right : Direction.Left);
                        break;

                    case Direction.Right:
                        filledTrenches = GetFilledTrenches(currentPos, boundaryTrenches, filledTrenches, clockwise ? Direction.Down : Direction.Up);
                        break;

                    case Direction.Down:
                        filledTrenches = GetFilledTrenches(currentPos, boundaryTrenches, filledTrenches, clockwise ? Direction.Left : Direction.Right);
                        break;

                    case Direction.Left:
                        filledTrenches = GetFilledTrenches(currentPos, boundaryTrenches, filledTrenches, clockwise ? Direction.Up : Direction.Down);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            filledTrenches.AddRange(boundaryTrenches);
            return filledTrenches;
        }

        static List<Trench> GetFilledTrenches(Tuple<int,int> currentPos, List<Trench> boundary, List<Trench> filled, Direction direction)
        {
            // add new filled positions to filled trench until we hit a boundary
            var boundaryFound = false;
            var row = currentPos.Item1;
            var col = currentPos.Item2;

            while (!boundaryFound)
            {
                // determine next pos
                switch (direction)
                {
                    case Direction.Up:
                        row = row - 1;
                        break;
                    case Direction.Right:
                        col = col + 1;
                        break;
                    case Direction.Down:
                        row = row + 1;
                        break;
                    case Direction.Left:
                        col = col - 1;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
                }

                // check if a boundary is found
                if (DoesPositionExistInList(row, col, boundary))
                {
                    boundaryFound = true;
                }
                else if (!DoesPositionExistInList(row, col, filled))
                {
                    filled.Add(new Trench(){Position = new Tuple<int, int>(row, col), IsBoundary = false});
                }
            }

            return filled;
        }

        static bool DoesPositionExistInList(int row, int col, List<Trench> trenches)
        {
            var exists = false;

            foreach (var trench in trenches)
            {
                if (trench.Position.Item1 == row
                    && trench.Position.Item2 == col)
                {
                    exists = true;
                    break;
                }
            }

            return exists;
        }
    }

}
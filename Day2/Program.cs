using System;
using System.Numerics;

namespace Day2
{
    static class Program
    {
        static void Main(string[] args)
        {
            // want to find games which are possible with the below number of cubes
            const int red = 12;
            const int green = 13;
            const int blue = 14;

            var filename = "PuzzleInput.txt";
            var lines = File.ReadAllLines(filename);

            var validGameSum = 0;
            var gamePowerSum = 0;

            for (var i = 0; i < lines.Length; i++)
            {
                var gameNumber = i + 1; // Note: games start with index of 1, not 0
                var maxCubes = GetMaxNumCubesPerGame(lines[i]);
                //Console.WriteLine(
                //    $"Max cubes for Game {gameNumber}: {maxCubes.Item1} red, {maxCubes.Item2} green, {maxCubes.Item3} blue");

                // Part 1
                if (maxCubes.Item1 <= red
                    && maxCubes.Item2 <= green
                    && maxCubes.Item3 <= blue)
                {
                    // valid game
                    //Console.WriteLine($"Game {gameNumber} is a valid game");
                    validGameSum += gameNumber;
                }

                // Part 2
                var power = maxCubes.Item1 * maxCubes.Item2 * maxCubes.Item3;
                gamePowerSum += power;
            }

            Console.WriteLine($"Part 1: Sum of valid games is {validGameSum}");
            Console.WriteLine($"Part 2: Sum of powers in {gamePowerSum}");
        }

        static Tuple<int, int, int> GetMaxNumCubesPerGame(string line)
        {
            var maxRed = 0;
            var maxGreen = 0;
            var maxBlue = 0;

            // discard bit of string with 'Game X: '
            var temp = line.Split(": ");
            var gameInfo = temp[1];

            // split the game info into sets
            var sets = gameInfo.Split("; ");

            // in each set, identify the colours
            foreach (var set in sets)
            {
                var colours = set.Split(", ");
                foreach (var colour in colours)
                {
                    // colouredCube consists of the number in [0] and colour in [1]
                    var colouredCube = colour.Split(' ');
                    var num = Convert.ToInt16(colouredCube[0]);

                    if (colouredCube[1] == "red")
                    {
                        if (num > maxRed) maxRed = num;
                    }
                    else if (colouredCube[1] == "green")
                    {
                        if (num > maxGreen) maxGreen = num;
                    }
                    else if (colouredCube[1] == "blue")
                    {
                        if (num > maxBlue) maxBlue = num;
                    }
                    else
                    {
                        throw new Exception("Colour not identified");
                    }
                }
            }

            return new Tuple<int, int, int>(maxRed, maxGreen, maxBlue);
        }
    }
}
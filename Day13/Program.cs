using System;
using System.Globalization;
using System.Numerics;
using System.Text;
using static Day16.Program;

namespace Day16
{
    static class Program
    {
        static void Main(string[] args)
        {
            var filename = "PuzzleInput.txt";
            var lines = File.ReadAllLines(filename);

            var patterns = GetPatterns(lines);

            var calculation = 0;
            foreach (var pattern in patterns)
            {
                //PrintPattern(pattern);

                var numCols = CheckForVerticalRefection(pattern);
                if (numCols != -1)
                {
                    //Console.WriteLine($"Vertical reflection found after numCols = {numCols}");
                    calculation = calculation + numCols;
                }
                else
                {
                    var numRows = CheckForHorizontalReflection(pattern);
                    if (numRows != -1)
                    {
                        //Console.WriteLine($"Horizontal reflection found after numRows = {numRows}");
                        calculation = calculation + (100 * numRows);
                    }
                    else
                    {
                        Console.WriteLine("No reflections found");
                    }
                }

            }

            Console.WriteLine($"Calculation = {calculation}");
        }

        static List<List<string>> GetPatterns(string[] lines)
        {
            var patterns = new List<List<String>> { new List<string>() };
            var index = 0;

            foreach (var line in lines)
            {
                if (line != "")
                {
                    patterns[index].Add(line);
                }
                else
                {
                    // separator between patterns
                    patterns.Add(new List<string>());
                    index++;
                }
            }
            
            return patterns;
        }

        // returns -1 if no reflection is found
        // otherwise returns number of columns to the left of the vertical line
        static int CheckForVerticalRefection(List<string> pattern)
        {
            var invertedPattern = GetPatternByColumns(pattern);
            return CheckForHorizontalReflection(invertedPattern);
        }

        // return -1 if no reflection
        // otherwise returns number of rows above the horizontal line
        static int CheckForHorizontalReflection(List<string> pattern)
        {
            var numRows = pattern.Count;

            for (var i = 0; i < numRows
                 - 1; i++)
            {
                if (pattern[i] == pattern[i + 1])
                {
                    // check which is smaller, distance to top or bottom
                    var distanceFromTop = i;
                    var distanceFromBottom = (numRows - 1) - (i + 1);

                    var stepsToCheck = distanceFromTop < distanceFromBottom ? distanceFromTop : distanceFromBottom;

                    var reflectionFound = true;

                    // check that as we move away from the reflection line, all the other lines are the same
                    for (var pos = 1; pos <= stepsToCheck; pos++)
                    {
                        if (pattern[i - pos] != pattern[i + 1 + pos])
                        {
                            reflectionFound = false;
                            break;
                        }
                    }

                    if (reflectionFound)
                    {
                        // return number of rows above. Add 1 to account for 0 index
                        return i + 1;
                    }
                }
            }

            return -1;
        }

        static List<string> GetPatternByColumns(List<string> pattern)
        {
            // reconstruct the pattern (a list of rows) into a list of columns
            var invertedPattern = new List<string>();
            var numRows = pattern.Count;
            var numCols = pattern[0].Length;

            for (var j = 0; j < numCols; j++)
            {
                var patternCol = new StringBuilder();

                for (var i = 0; i < numRows; i++)
                {
                    var character = (pattern[i])[j];
                    patternCol.Append(character);
                }

                invertedPattern.Add(patternCol.ToString());
            }
            //Console.WriteLine("Inverted pattern");
            //PrintPattern(invertedPattern);
            return invertedPattern;
        }

        static void PrintPattern(List<string> pattern)
        {
            foreach (var line in pattern)
            {
                Console.WriteLine(line);
            }
        }
    }
}
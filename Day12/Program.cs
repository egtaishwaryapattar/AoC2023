using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Numerics;

namespace Day12
{
    static class Program
    {
        static void Main(string[] args)
        {
            string filename = "Test1.txt";
            var lines = File.ReadAllLines(filename);
            var sumOfCombinations = 0;

            foreach (var line in lines)
            {
                // split substring into springs and values
                var springInfo = line.Split(' ');
                var springLocations = springInfo[0];
                var springValues = springInfo[1];

                var springValuesSplit = springValues.Split(',');
                var values = new List<int>();
                var minNumberCellsRequired = 0;
                foreach (var value in springValuesSplit)
                {
                    var valueAsInt = Convert.ToInt16(value);
                    values.Add(valueAsInt);
                    minNumberCellsRequired += valueAsInt;
                }

                minNumberCellsRequired += values.Count - 1; // add the number of spaces required

                // identify possible combinations of springs
                // e.g. .??..??...?##. 1,1,3 has 4 combinations
                //      ?#?#?#?#?#?#?#? 1,3,1,6 - 1 arrangement

                if (springLocations.Length == minNumberCellsRequired)
                {
                    // can only be one combination
                    Console.WriteLine($"For {line} : Combination = 1");
                    sumOfCombinations = sumOfCombinations + 1;
                }
                else if (springLocations.Length > minNumberCellsRequired)
                {
                    var numCombos = 0;

                    for (int i = 0; i < springLocations.Length; i++)
                    {
                        var substring = springLocations.Substring(i, springLocations.Length);

                        // no point populating 
                        if (substring[0] == '.')
                        {
                            // no point putting a '#' for a known safe spring don't try to look for a combination starting from here
                            if (substring.Length <= minNumberCellsRequired)
                            {
                                // won't find any further valid combos if the substring is getting smaller than the minimum required string size
                                break;
                            }

                        }
                        else 
                        {
                            if (substring.Length == minNumberCellsRequired)
                            {
                                // got to the least possible place to allocate the required springs
                                // only if the combo fits will it be valid and add 1, otherwise don't increment sum
                                char[] createdCombo = new char[substring.Length];

                                numCombos = numCombos + 1;
                                break;
                            }
                            else
                            {
                                // TODO: identify location of '#'s in substring
                                // TODO: identify location of '.'s in substring to know to skip over these points
                                // TODO: create a combination and ensure that the '#' and '.' are in the right place to satisfy
                            }

                            // break out of the loop if we reach a '#' because we don't want to keep making the substring shorter and exclude this

                        }
                    }

                    Console.WriteLine($"For {line} : Combination = X");
                }
                else
                {
                    // if springLocation.Length < minNumberOfCells, there are 0 combinations
                    Console.WriteLine($"For {line} , Combination = 0");
                }
            }
        }
    }
}
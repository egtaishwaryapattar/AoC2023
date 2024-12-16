using System;
using System.Globalization;
using System.IO;
using System.Numerics;

namespace Day15
{
    static class Program
    {
        private class Lens : IEquatable<Lens>
        {
            public string Label = "";
            public int FocalLength = 0; // can have value 1 to 9

            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                Lens objAsPart = obj as Lens;
                if (objAsPart == null) return false;
                else return Equals(objAsPart);
            }

            public bool Equals(Lens other)
            {
                return (this.Label.Equals(other.Label));
            }
        }

        private class Box
        {
            public List<Lens> Lenses = new List<Lens>();

            public Box()
            {
                Lenses = new List<Lens>();
            }
        }

        static void Main(string[] args)
        {
            var filename = "PuzzleInput.txt";
            var lines = File.ReadAllLines(filename);
            if (lines.Length != 1) throw new Exception("There should only be 1 line in the file");

            var seq = lines[0].Split(',');
            Part2(seq);
        }

        static void Part1(string[] seq)
        {
            //var hashValue = RunHashAlgorithm("HASH");

            var seqHash = 0;
            foreach (var step in seq)
            {
                seqHash += RunHashAlgorithm(step);
            }

            Console.WriteLine($"Sum of HASH for sequence is : {seqHash}");
        }

        static void Part2(string[] seq)
        {
            // create box 0 to 255
            var boxes = new Box[256];
            for (var index = 0; index < 256; index++)
            {
                boxes[index] = new Box();
            }

            foreach (var step in seq)
            {
                if (step.Contains('-'))
                {
                    // get label 
                    var label = step.Split('-')[0];

                    // identify box number
                    var boxNumber = RunHashAlgorithm(label);

                    // get box and, if it contains a lens with the matching label, remove it
                    boxes[boxNumber].Lenses.Remove(new Lens() {Label = label, FocalLength = 0}); // don't care what the focal length is when removing 
                }
                else if (step.Contains('='))
                {
                    // get label and focal length
                    var splitString = step.Split('=');
                    var label = splitString[0];
                    var focalLength = Convert.ToInt16(splitString[1]);

                    // identify box number
                    var boxNumber = RunHashAlgorithm(label);

                    // get box. If it already contains lens with matching label, update the Focal length. Otherwise add lens
                    bool lensAlreadyExists = false;
                    foreach (var lens in boxes[boxNumber].Lenses)
                    {
                        if (lens.Label == label)
                        {
                            // lens exists so update focal length number
                            lensAlreadyExists = true;
                            lens.FocalLength = focalLength;
                            break;
                        }
                    }

                    if (!lensAlreadyExists)
                    {
                        boxes[boxNumber].Lenses.Add(new Lens() { Label = label, FocalLength = focalLength });
                    }
                }
            }

            // calculate sum
            var focusingPowerSum = 0;
            for (var i = 0; i < 256; i++)
            {
                var box = boxes[i];

                for (var j = 0; j < box.Lenses.Count; j++)
                {
                    // The focusing power of a single lens is the result of multiplying together:
                    //      One plus the box number of the lens in question.
                    //      The slot number of the lens within the box: 1 for the first lens, 2 for the second lens, and so on.
                    //      The focal length of the lens.
                    
                    var focusingPower = (i + 1) * (j + 1) * box.Lenses[j].FocalLength;
                    //Console.WriteLine($"{box.Lenses[j].Label} focusing power = {focusingPower}");
                    focusingPowerSum += focusingPower;
                }
            }

            Console.WriteLine($"Sum of all focusing power = {focusingPowerSum}");
        }

        static int RunHashAlgorithm(string str)
        {
            // Determine the ASCII code for the current character of the string.
            // Increase the current value by the ASCII code you just determined.
            // Set the current value to itself multiplied by 17.
            // Set the current value to the remainder of dividing itself by 256.

            var currentValue = 0;

            foreach (var c in str)
            {
                //Console.WriteLine($"ASCII value of {c} is {(int)c}");
                currentValue += (int)c; // casting to int should get ascii value
                currentValue = currentValue * 17;
                currentValue = currentValue % 256;
                //Console.WriteLine($"Result of HASH algorithm for {c} is {currentValue}");
            }

            //Console.WriteLine($"Result of HASH algorithm for {str} is {currentValue}");
            return currentValue;
        }
    }
}
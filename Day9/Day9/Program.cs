using System;
using System.Numerics;

namespace Day9
{
    static class Program
    {
        // this program intends to be run every day
        static void Main(string[] args)
        {
            string filename = "PuzzleInput.txt";
            Part2(filename);
        }

        static void Part2(string filename)
        {
            var sumOfPrevNums = 0;
            var lines = File.ReadAllLines(filename);

            foreach (var line in lines)
            {
                var sequence = GetSequence(line);
                var prevNum = GetPrevNumberInSequence(sequence);
                Console.WriteLine($"Previous Number in sequence is {prevNum}");
                sumOfPrevNums += prevNum;
            }

            Console.WriteLine($"Sum of all next numbers is {sumOfPrevNums}");
        }

        static void Part1(string filename)
        {
            var sumOfNextNums = 0;
            var lines = File.ReadAllLines(filename);

            foreach (var line in lines)
            {
                var sequence = GetSequence(line);
                var nextNum = GetNextNumberInSequence(sequence);
                Console.WriteLine($"Next Number in sequence is {nextNum}");
                sumOfNextNums += nextNum;
            }

            Console.WriteLine($"Sum of all next numbers is {sumOfNextNums}");
        }

        static List<int> GetSequence(string line)
        {
            var extractedNumbers = line.Split(' ');
            var sequence = new List<int>();
            foreach (var number in extractedNumbers)
            {
                sequence.Add(Convert.ToInt32(number));
            }

            return sequence;
        }

        static int GetNextNumberInSequence(List<int> sequence)
        {
            if (AreAllNumsZero(sequence))
            {
                return 0;
            }
            else
            {
                int sequenceCount = sequence.Count;
                var diffs = new List<int>();

                for (var i = 0; i < sequenceCount - 1; i++)
                {
                    diffs.Add(sequence[i + 1] - sequence[i]);
                }

                // add last number in sequence to next number in the sequence of differences
                return sequence[sequenceCount - 1] + GetNextNumberInSequence(diffs);
            }
        }

        static int GetPrevNumberInSequence(List<int> sequence)
        {
            if (AreAllNumsZero(sequence))
            {
                return 0;
            }
            else
            {
                var diffs = new List<int>();

                for (var i = 0; i < sequence.Count - 1; i++)
                {
                    diffs.Add(sequence[i + 1] - sequence[i]);
                }

                // add last number in sequence to next number in the sequence of differences
                return sequence[0] - GetPrevNumberInSequence(diffs);
            }
        }

        static bool AreAllNumsZero(List<int> diffs)
        {
            foreach(var diff in diffs)
            {
                if (diff != 0) return false;
            }

            return true;
        }

    }
}
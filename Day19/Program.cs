using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace Day19
{
    static class Program
    {
        private class Rating
        {
            public int X;
            public int M;
            public int A;
            public int S;

            public int GetSumOfRatings()
            {
                return X + M + A + S;
            }
        }

        private enum RatingType
        {
            X, M, A, S
        }

        private class Workflow
        {
            public bool IsEndpoint;
            public RatingType Rating;
            public bool IsGreater;      // refers to the sign - won't find equals to
            public int Value;
            public string NextStep;
        }

        private class AcceptedKey
        {
            public string Key;
            public int Count;
        }

        private class Bounds
        {
            public int[] xRange = new int[2] {1,4000};
            public int[] mRange = new int[2] { 1, 4000 };
            public int[] aRange = new int[2] { 1, 4000 };
            public int[] sRange = new int[2] { 1, 4000 };

            // index 0 is lower boundary, index 1 is upper boundary. Range is inclusive of both numbers
            public ulong GetCombinations()
            {
                return (ulong)(xRange[1] - xRange[0] + 1) * (ulong)(mRange[1] - mRange[0] + 1) *
                       (ulong)(aRange[1] - aRange[0] + 1) * (ulong)(sRange[1] - sRange[0] + 1);
            }

            public void PrintBounds()
            {
                Console.WriteLine($"{xRange[0]}\t{xRange[1]}\t\t{mRange[0]}\t{mRange[1]}\t\t{aRange[0]}\t{aRange[1]}\t\t{sRange[0]}\t{sRange[1]}\t\t");
            }
        }

        private static Dictionary<string, List<Workflow>> _workflows = new Dictionary<string, List<Workflow>>();
        private static List<Rating> _ratings = new List<Rating>();
        private static List<AcceptedKey> _keysWithAccept = new List<AcceptedKey>();

        static void Main(string[] args)
        {
            var filename = "PuzzleInput.txt";
            var lines = File.ReadAllLines(filename);

            GetWorkflowsAndRatings(lines);
            Part2();
        }

        static void Part1()
        {
            var sumOfAcceptedWorkflows = 0;
            foreach (var rating in _ratings)
            {
                var accepted = IterateRatingThroughWorkflow(rating);

                if (accepted)
                {
                    var sum = rating.GetSumOfRatings();
                    Console.WriteLine($"Sum for accepted rating = {sum}");
                    sumOfAcceptedWorkflows += sum;
                }
            }

            Console.WriteLine($"Sum for all accepted rating = {sumOfAcceptedWorkflows}");
        }

        static void Part2()
        {
            ulong distinctCombo = 0;
            var pathsBounds = new List<Bounds>();

            foreach (var keyAccept in _keysWithAccept)
            {
                var numAcceptsInKey = 0;

                // find the workflow entry for key
                var workflow = _workflows[keyAccept.Key];
                
                for (var i = 0; i < workflow.Count; i++)
                {
                    // find an A before passing it on to Follow Acceptable Path
                    if (workflow[i].NextStep == "A")
                    {
                        var bounds = FollowAcceptablePath(workflow, i, keyAccept.Key);
                        pathsBounds.Add(bounds);
                        var combos = bounds.GetCombinations();
                        //Console.WriteLine($"Number of combinations for a path: {combos}");

                        distinctCombo += combos;
                        numAcceptsInKey++;

                        if (numAcceptsInKey == keyAccept.Count)
                        {
                            break; // move onto the next keyAccept in _keysWithAccept
                        }
                    }
                }
            }

            Console.WriteLine($"Number of distinct combinations is: {distinctCombo}");
            Console.WriteLine();

            foreach (var bounds in pathsBounds)
            {
                bounds.PrintBounds();
            }
        }

        static Bounds FollowAcceptablePath(List<Workflow> workflow, int index, string key)
        {
            var bounds = new Bounds();
            bool startPointFound = false;
            bool firstStepDone = false;
            string prevKey = "";

            while (!startPointFound)
            {
                // find the conditions to get to this A
                for (var i = index; i >= 0; i--)
                {
                    if (!workflow[i].IsEndpoint)
                    {
                        // if it isn't an endpoint, there will be conditions that need to be satisfied.
                        // identify conditions to reduce the limits to get to A
                        if ((workflow[i].NextStep == "A" && !firstStepDone)
                            || workflow[i].NextStep == prevKey)
                        {
                            // this condition had to be satisfied

                            switch (workflow[i].Rating)
                            {
                                case RatingType.X:
                                    if (workflow[i].IsGreater)
                                    {
                                        var newLowerBound = workflow[i].Value + 1;
                                        if (newLowerBound > bounds.xRange[0]) bounds.xRange[0] = newLowerBound; 
                                    }
                                    else
                                    {
                                        var newUpperBound = workflow[i].Value - 1;
                                        if (newUpperBound < bounds.xRange[1]) bounds.xRange[1] = newUpperBound; 
                                    }
                                    break;

                                case RatingType.M:
                                    if (workflow[i].IsGreater)
                                    {
                                        var newLowerBound = workflow[i].Value + 1;
                                        if (newLowerBound > bounds.mRange[0]) bounds.mRange[0] = newLowerBound;
                                    }
                                    else
                                    {
                                        var newUpperBound = workflow[i].Value - 1;
                                        if (newUpperBound < bounds.mRange[1]) bounds.mRange[1] = newUpperBound;
                                    }
                                    break;

                                case RatingType.A:
                                    if (workflow[i].IsGreater)
                                    {
                                        var newLowerBound = workflow[i].Value + 1;
                                        if (newLowerBound > bounds.aRange[0]) bounds.aRange[0] = newLowerBound;
                                    }
                                    else
                                    {
                                        var newUpperBound = workflow[i].Value - 1;
                                        if (newUpperBound < bounds.aRange[1]) bounds.aRange[1] = newUpperBound;
                                    }
                                    break;

                                case RatingType.S:
                                    if (workflow[i].IsGreater)
                                    {
                                        var newLowerBound = workflow[i].Value + 1;
                                        if (newLowerBound > bounds.sRange[0]) bounds.sRange[0] = newLowerBound;
                                    }
                                    else
                                    {
                                        var newUpperBound = workflow[i].Value - 1;
                                        if (newUpperBound < bounds.sRange[1]) bounds.sRange[1] = newUpperBound;
                                    }
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            firstStepDone = true; // in the pathway to 'in' if we encounter any other conditions for A, we want them to be false
                        }
                        else
                        {
                            // this condition is not met

                            switch (workflow[i].Rating)
                            {
                                case RatingType.X:
                                    if (workflow[i].IsGreater)
                                    {
                                        var newUpperBound = workflow[i].Value;
                                        if (newUpperBound < bounds.xRange[1]) bounds.xRange[1] = newUpperBound;
                                    }
                                    else
                                    {
                                        var newLowerBound = workflow[i].Value;
                                        if (newLowerBound > bounds.xRange[0]) bounds.xRange[0] = newLowerBound;
                                    }
                                    break;

                                case RatingType.M:
                                    if (workflow[i].IsGreater)
                                    {
                                        var newUpperBound = workflow[i].Value;
                                        if (newUpperBound < bounds.mRange[1]) bounds.mRange[1] = newUpperBound;
                                    }
                                    else
                                    {
                                        var newLowerBound = workflow[i].Value;
                                        if (newLowerBound > bounds.mRange[0]) bounds.mRange[0] = newLowerBound;
                                    }
                                    break;

                                case RatingType.A:
                                    if (workflow[i].IsGreater)
                                    {
                                        var newUpperBound = workflow[i].Value;
                                        if (newUpperBound < bounds.aRange[1]) bounds.aRange[1] = newUpperBound;
                                    }
                                    else
                                    {
                                        var newLowerBound = workflow[i].Value;
                                        if (newLowerBound > bounds.aRange[0]) bounds.aRange[0] = newLowerBound;
                                    }
                                    break;

                                case RatingType.S:
                                    if (workflow[i].IsGreater)
                                    {
                                        var newUpperBound = workflow[i].Value;
                                        if (newUpperBound < bounds.sRange[1]) bounds.sRange[1] = newUpperBound;
                                    }
                                    else
                                    {
                                        var newLowerBound = workflow[i].Value;
                                        if (newLowerBound > bounds.sRange[0]) bounds.sRange[0] = newLowerBound;
                                    }
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            firstStepDone = true;
                        }
                    }
                    else
                    {
                        firstStepDone = true;
                    }
                }

                if (key == "in")
                {
                    startPointFound = true;
                }
                else
                {
                    // find the next workflow, key and index value
                    foreach (var item in _workflows)
                    {
                        var nextFound = false;
                        for (var j = 0; j < item.Value.Count; j++)
                        {
                            if (item.Value[j].NextStep == key)
                            {
                                workflow = item.Value;
                                index = j;
                                prevKey = key;
                                key = item.Key;
                                nextFound = true;
                                break;
                            }
                        }

                        if (nextFound) break;
                    }
                }
            }

            return bounds;
        }
        
        static void GetWorkflowsAndRatings(string[] lines)
        {
            var allWorkflowsFound = false;

            foreach (var line in lines)
            {
                if (!allWorkflowsFound)
                {
                    if (line == "")
                    {
                        allWorkflowsFound = true;
                    }
                    else
                    {
                        // format is px{a<2006:qkq,m>2090:A,rfg}
                        var split = line.Split('{');
                        var key = split[0];

                        var str = split[1].Remove(split[1].Length - 1); // get rid of '}'
                        var workflowsAsStr = str.Split(',');

                        var workflows = new List<Workflow>();
                        foreach (var workflow in workflowsAsStr)
                        {
                            if (workflow.Contains(':'))
                            {
                                // is a sequence
                                var seq = workflow.Split(':');
                                var condition = seq[0];
                                var nextStep = seq[1];
                                workflows.Add(new Workflow()
                                {
                                    IsEndpoint = false,
                                    Rating = GetRatingType(condition[0]),
                                    IsGreater = condition[1] == '>',
                                    Value = Convert.ToInt16(condition.Substring(2, seq[0].Length - 2)),
                                    NextStep = nextStep,
                                });

                                if (nextStep == "A")
                                {
                                    AddKeyAsAcceptableRoute(key);
                                }
                            }
                            else
                            {
                                workflows.Add(new Workflow(){ IsEndpoint = true, NextStep = workflow});
                                if (workflow == "A")
                                {
                                    AddKeyAsAcceptableRoute(key);
                                }
                            }
                        }

                        _workflows.Add(key, workflows);
                    }
                }
                else
                {
                    // format is : {x=787,m=2655,a=1222,s=2876}
                    var bracesRemoved = line.Substring(1, line.Length - 2);
                    var values = bracesRemoved.Split(',');

                    _ratings.Add(new Rating()
                    {
                        X = Convert.ToInt16((values[0].Split('='))[1]),
                        M = Convert.ToInt16((values[1].Split('='))[1]),
                        A = Convert.ToInt16((values[2].Split('='))[1]),
                        S = Convert.ToInt16((values[3].Split('='))[1])
                    });
                }
            }
        }

        static void AddKeyAsAcceptableRoute(string key)
        {
            var keyFound = false;

            foreach (var acceptedKey in _keysWithAccept.Where(acceptedKey => acceptedKey.Key == key))
            {
                keyFound = true;
                acceptedKey.Count++;
                break;
            }

            if (!keyFound)
            {
                _keysWithAccept.Add(new AcceptedKey() {Key = key, Count = 1});
            }
        }

        static RatingType GetRatingType(char c)
        {
            if (c == 'x') return RatingType.X;
            if (c == 'm') return RatingType.M;
            if (c == 'a') return RatingType.A;
            if (c == 's') return RatingType.S;
            throw new Exception("Rating type not found");
        }

        static bool IterateRatingThroughWorkflow(Rating rating)
        {
            bool accepted = false;
            bool endPointFound = false;
            string key = "in";

            while (!endPointFound)
            {
               var workflows = _workflows[key];

               foreach (var workflow in workflows)
               {
                   if (workflow.IsEndpoint
                       || IsWorkflowConditionMet(rating, workflow))
                   {
                       if (workflow.NextStep == "R")
                       {
                           endPointFound = true;
                           break;
                       }
                       else if (workflow.NextStep == "A")
                       {
                           accepted = true;
                           endPointFound = true;
                           break;
                       }
                       else
                       {
                           key = workflow.NextStep;
                           break;
                       }
                   }
               }
            }

            return accepted;
        }

        static bool IsWorkflowConditionMet(Rating rating, Workflow workflow)
        {
            switch (workflow.Rating)
            {
                case RatingType.X:
                    return workflow.IsGreater 
                        ? rating.X > workflow.Value 
                        : rating.X < workflow.Value;

                    break;
                case RatingType.M:
                    return workflow.IsGreater
                        ? rating.M > workflow.Value
                        : rating.M < workflow.Value;
                    break;
                case RatingType.A:
                    return workflow.IsGreater
                        ? rating.A > workflow.Value
                        : rating.A < workflow.Value;
                    break;
                case RatingType.S:
                    return workflow.IsGreater
                        ? rating.S > workflow.Value
                        : rating.S < workflow.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
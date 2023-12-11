using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace adventofcode
{
    public class Day16
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Day16(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Part1()
        {
            var input = await ReadInput();
            var allRanges = input.Rules.Values.SelectMany(y => y).ToList();
            var ret = input.NearbyTicket.SelectMany(x => InvalidNumbers(x, allRanges)).Sum();

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        [Fact]
        public async Task Part2()
        {
            var input = await ReadInput();
            var allRanges = input.Rules.Values.SelectMany(y => y).ToList();
            var allValidTickets = input.NearbyTicket.Where(x => !InvalidNumbers(x, allRanges).Any()).Concat(new List<IList<int>>{input.YourTicket}).ToList();
            var potentialIndexes = new List<(string rule, IList<int> potentialIndexes)>();
            foreach (var inputRule in input.Rules)
            {
                var indexes = new List<int>();
                potentialIndexes.Add((inputRule.Key, indexes));
                for (int i = 0; i < input.Rules.Count; i++)
                {
                    var positionNumbers = allValidTickets.Select(x => x[i]).ToList();
                    if (!InvalidNumbers(positionNumbers, inputRule.Value).Any())
                    {
                        indexes.Add(i);
                    }
                }
            }

            potentialIndexes = potentialIndexes.OrderBy(x => x.potentialIndexes!.Count).ToList();
            var allocation = new int[potentialIndexes.Count];
            var canAllocate = TryAllocate(potentialIndexes.Select(x => x.potentialIndexes).ToList(),0, allocation);
            if (canAllocate)
            {
                var finalAllocation = potentialIndexes.Select((x, i) => (x.rule, index: allocation[i]))
                    .ToDictionary(x => x.rule, x => x.index);

                var ret = finalAllocation.Where(x => x.Key.StartsWith("departure")).Select(x => input.YourTicket[x.Value]).Aggregate(1L, (x, y) => x * y);
         
                _testOutputHelper.WriteLine($"Result is : {ret}");
            }
        }

        private static bool TryAllocate(IList<IList<int>> manyIndexesMap, int currentIndex, int[] allocatedIndexes)
        {
            if (currentIndex >= manyIndexesMap.Count)
                return true;

            var current = manyIndexesMap[currentIndex];

            var canAllocate = false;
            foreach (var index in current.Except(allocatedIndexes.Take(currentIndex)))
            {
                allocatedIndexes[currentIndex] = index;
                if (TryAllocate(manyIndexesMap, currentIndex + 1, allocatedIndexes))
                {
                    canAllocate = true;
                    break;
                }
            }

            return canAllocate;
        }

        private static IEnumerable<int> InvalidNumbers(IList<int> ticket, IList<Range> rules)
        {
            return ticket.Where(y => rules.All(z => !z.Has(y)));
        }

        private static async Task<InputModel> ReadInput()
        {
            using var stream = File.OpenText("./Files/day16_input.txt");
            var fileText = await stream.ReadToEndAsync();
            var ret = new InputModel();
            var statge = ParseStage.Rules;

            foreach (var line in fileText.Split("\n", StringSplitOptions.RemoveEmptyEntries))
            {
                switch (line)
                {
                    case "your ticket:":
                        statge = ParseStage.YourTicket;
                        break;
                    case "nearby tickets:":
                        statge = ParseStage.NearbyTickets;
                        break;
                    default:

                        switch (statge)
                        {
                            case ParseStage.Rules:
                                var ruleName = line.Substring(0, line.IndexOf(":", StringComparison.Ordinal));
                                var regex = new Regex("(\\d+)-(\\d+)");

                                ret.Rules.Add(ruleName, regex.Matches(line).Select(x => new Range(int.Parse(x.Groups[1].Value), int.Parse(x.Groups[2].Value))).ToList());
                                break;
                            case ParseStage.YourTicket:
                                ret.YourTicket = line.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                                break;
                            case ParseStage.NearbyTickets:
                                ret.NearbyTicket.Add(line.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList());
                                break;
                        }

                        break;
                }
            }

            return ret;
        }

        enum ParseStage
        {
            Rules = 0, YourTicket = 1, NearbyTickets = 2
        }

        class InputModel
        {
            public InputModel()
            {
                Rules = new Dictionary<string, IList<Range>>();
                NearbyTicket = new List<IList<int>>();
            }

            public Dictionary<string, IList<Range>> Rules { get; set; }

            public IList<int> YourTicket { get; set; }
            public IList<IList<int>> NearbyTicket { get; set; }
        }

        class Range
        {
            public int Min { get; }
            public int Max { get; }

            public Range(int min, int max)
            {
                Min = min;
                Max = max;
            }

            public bool Has(int number)
            {
                return number >= Min && number <= Max;
            }
        }
    }
}

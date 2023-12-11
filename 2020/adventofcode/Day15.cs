using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace adventofcode
{
    public class Day15
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Day15(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Part1()
        {
            var input = await ReadInput();
            var dictionary = input
                .Take(input.Count - 1)
                .Select((x, i) => (Index: i, Number: x))
                .ToDictionary(x => x.Number, x => x.Index + 1);

            var ret = GetPart1Result(dictionary, input.Count, input.Last(), 2020);

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        [Fact]
        public async Task Part2()
        {
            var input = await ReadInput();
            var dictionary = input
                .Take(input.Count - 1)
                .Select((x, i) => (Index: i, Number: x))
                .ToDictionary(x => x.Number, x => x.Index + 1);

            var ret = GetPart1Result(dictionary, input.Count, input.Last(), 30000000);

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        private int GetPart1Result(Dictionary<int, int> dictionary, int currentIteration, int lastNumber, int max)
        {
            while (currentIteration != max)
            {
                if (dictionary.TryGetValue(lastNumber, out var turnNumber))
                {
                    dictionary[lastNumber] = currentIteration;
                    lastNumber = currentIteration - turnNumber;
                }
                else
                {
                    dictionary.Add(lastNumber, currentIteration);
                    lastNumber = 0;
                }

                currentIteration++;
            }

            return lastNumber;
        }

        private static async Task<IList<int>> ReadInput()
        {
            using var stream = File.OpenText("./Files/day15_input.txt");
            var fileText = await stream.ReadToEndAsync();
            return fileText.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        }
    }
}

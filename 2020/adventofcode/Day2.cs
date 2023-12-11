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
    public class Day2
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Day2(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Part1()
        {
            var input = await ReadInput();
            var ret = input
                .Select(ParseModel)
                .Count(x =>
                {
                    var occurrence = x.Value.Count(y => y == x.Symbol);
                    return occurrence >= x.Min && occurrence <= x.Max;
                });

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        [Fact]
        public async Task Part2()
        {
            var input = await ReadInput();
            var ret = input
                .Select(ParseModel)
                .Count(x => x.Value[x.Min- 1] == x.Symbol ^ x.Value[x.Max - 1] == x.Symbol);

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        class DataModel
        {
            public int Min { get; set; }
            public int Max { get; set; }
            public char Symbol { get; set; }
            public string Value { get; set; }
        }

        private DataModel ParseModel(string raw)
        {
            var regex = new Regex("([\\d]+)-([\\d]+)\\s(.):\\s(.+)");
            var match = regex.Match(raw);

            return new DataModel
            {
                Min = int.Parse(match.Groups[1].Value),
                Max = int.Parse(match.Groups[2].Value),
                Symbol = match.Groups[3].Value.First(),
                Value = match.Groups[4].Value,
            };
        }

        private static async Task<IList<string>> ReadInput()
        {
            using var stream = File.OpenText("./Files/day2_input.txt");
            var input = (await stream.ReadToEndAsync())
                .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .ToArray();
            return input;
        }
    }
}

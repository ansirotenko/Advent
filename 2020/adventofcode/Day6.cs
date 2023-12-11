using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace adventofcode
{
    public class Day6
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Day6(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Part1()
        {
            var input = await ReadInput();
            var ret = input.Select(x => x.Replace("\n", "").Distinct().Count()).Sum();

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        [Fact]
        public async Task Part2()
        {
            var input = await ReadInput();
            var ret = input.Select(x =>
            {
                var answers = x.Split("\n", StringSplitOptions.RemoveEmptyEntries)
                    .Select(y => y.OrderBy(z => z).ToArray())
                    .ToArray();

                var ret = answers.Skip(1).Aggregate(answers.First().AsEnumerable(), (current, next) => current.Intersect(next));
                return ret.Count();
            }).Sum();

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        private static async Task<IList<string>> ReadInput()
        {
            using var stream = File.OpenText("./Files/day6_input.txt");
            var fileText = await stream.ReadToEndAsync();
            return fileText.Split("\n\n", StringSplitOptions.RemoveEmptyEntries).ToArray();
        }
    }
}

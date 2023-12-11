using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace adventofcode
{
    public class Day13
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Day13(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Part1()
        {
            var input = await ReadInput();
            var table = input.BusIds
                .Where(x => x.HasValue)
                .Select(x => (Id: x.Value, Time: Next(x.Value, input.TimeStamp) - input.TimeStamp))
                .OrderBy(x => x.Time)
                .ToList();
            var ret = table.First().Id * table.First().Time;

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        [Fact]
        public async Task Part2()
        {
            var input = await ReadInput();

            var ret = 1UL;
            var iterations = 0UL;
            var debugPow = 1000000UL;
            while (true)
            {
                iterations++;
                var currentValue = ret - 1;
                var isFound = true;
                for (var i = 0; i < input.BusIds.Length; i++)
                {
                    if (input.BusIds[i].HasValue && (Next(input.BusIds[i].Value, currentValue) - currentValue) != 1)
                    {
                        isFound = false;
                        ret += input.BusIds.Take(i).Where(x => x.HasValue).Aggregate(1UL, (x, y) => x * y.Value);
                        break;
                    }

                    currentValue++;
                }

                if (isFound)
                {
                    break;
                }

                if (iterations > debugPow)
                {
                    _testOutputHelper.WriteLine($"Iterations are : {iterations}");
                    debugPow *= 10;
                }
            }

            _testOutputHelper.WriteLine($"Result is : {ret}");
            _testOutputHelper.WriteLine($"Iterations are : {iterations}");
        }

        private ulong Next(ulong id, ulong value)
        {
            return (value / id + 1) * id;
        }

        private static async Task<(ulong TimeStamp, ulong?[] BusIds)> ReadInput()
        {
            using var stream = File.OpenText("./Files/day13_input.txt");
            var fileText = await stream.ReadToEndAsync();
            var split = fileText.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            var timestamp = split.First();
            var busIds = split.Last()
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x == "x" ? null : (ulong?)ulong.Parse(x))
                .ToArray();
            return (ulong.Parse(timestamp), busIds);
        }
    }
}

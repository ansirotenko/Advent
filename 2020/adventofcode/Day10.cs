using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace adventofcode
{
    public class Day10
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Day10(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Part1()
        {
            var input = await ReadInput();
            var index = 0;
            var currentJoltage = 0;
            var diff1 = 0;
            var diff2 = 0;
            var diff3 = 1;

            while (index < input.Count)
            {
                var diff = input[index] - currentJoltage;
                switch (diff)
                {
                    case 1:
                        diff1++;
                        break;
                    case 2:
                        diff2++;
                        break;
                    case 3:
                        diff3++;
                        break;
                    default:
                        throw new ArgumentException("Wrong input");
                }

                currentJoltage = input[index];
                index++;
            }

            _testOutputHelper.WriteLine($"Result is : {diff1 * diff3}");
        }

        [Fact]
        public async Task Part2()
        {
            var input = await ReadInput();
            input = new[] {0}.Concat(input).ToArray();
            var cache = new long[input.Count];
            var ret = CalculateArrangements(input, input.Count - 1, cache);
            var test = input.Zip(cache);

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        private static long CalculateArrangements(IList<int> input, int index, long[] cache)
        {
            if (index == 0)
                return 1;

            if (cache[index] != 0)
                return cache[index];

            var ret = 0L;
            var i = index - 3;
            while (i != index )
            {
                if (i >= 0 && i < index && input[index] - input[i] < 4)
                {
                    ret += CalculateArrangements(input, i, cache);
                }

                i++;
            }

            cache[index] = ret;

            return ret;
        }

        private static async Task<IList<int>> ReadInput()
        {
            using var stream = File.OpenText("./Files/day10_input.txt");
            var fileText = await stream.ReadToEndAsync();
            return fileText.Split("\n", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).OrderBy(x => x).ToArray();
        }
    }
}

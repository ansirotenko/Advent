using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace adventofcode
{
    public class Day9
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Day9(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Part1()
        {
            var input = await ReadInput();
            const int range = 25;

            for (int i = range; i < input.Count; i++)
            {
                var current = input[i];
                var isCorrect = false;

                for (int j = i - 1; j >= i - range; j--)
                {
                    for (int k = i - 1; k >= i - range; k--)
                    {
                        if (j == k)
                            continue;
                        if (input[j] + input[k] == current)
                        {
                            isCorrect = true;
                            break;
                        }
                        if (isCorrect)
                            break;
                    }
                }

                if (!isCorrect)
                {
                    _testOutputHelper.WriteLine($"Result is : {current}");
                    break;
                }
            }
        }


        [Fact]
        public async Task Part2()
        {
            var input = await ReadInput();
            const int invalid = 31161678;

            for (int j = 0; j < input.Count - 1; j++)
            {
                long check = invalid;
                for (int k = j + 1; k < input.Count; k++)
                {
                    check = check - input[k];
                    if (check > 0)
                        continue;
                    if (check < 0)
                        break;

                    var array = input.Skip(j).Take(k - j).ToArray();
                    _testOutputHelper.WriteLine($"Result is : {array.Min() + array.Max()}");
                }
            }
        }

        private static async Task<IList<long>> ReadInput()
        {
            using var stream = File.OpenText("./Files/day9_input.txt");
            var fileText = await stream.ReadToEndAsync();
            return fileText.Split("\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(long.Parse)
                .ToList();
        }
    }
}

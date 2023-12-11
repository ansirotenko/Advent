using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace adventofcode
{
    public class Day3
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Day3(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Part1()
        {
            var input = await ReadInput();
            var ret = Calculate(input, 1, 3);
            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        [Fact]
        public async Task Part2()
        {
            var input = await ReadInput();
            var data = new (int deltaRow, int deltaCol)[]
            {
                (1, 1),
                (1, 3),
                (1, 5),
                (1, 7),
                (2, 1)
            };

            var ret = data.Select(x => Calculate(input, x.deltaRow, x.deltaCol));
            _testOutputHelper.WriteLine($"Result is : {ret.Aggregate(1L, (x, y) => x * y)}");
        }

        private int Calculate(bool[][] input, int deltaRow, int deltaCol)
        {
            var row = 0;
            var col = 0;
            var ret = 0;

            while (row < input.Length)
            {
                if (input[row][col])
                {
                    ret++;
                }

                col = (col + deltaCol) % input[row].Length;
                row = (row + deltaRow);
            }

            return ret;
        }

        private static async Task<bool[][]> ReadInput()
        {
            using var stream = File.OpenText("./Files/day3_input.txt");
            return (await stream.ReadToEndAsync())
                .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Select(y => y == '#').ToArray())
                .ToArray();
        }
    }
}

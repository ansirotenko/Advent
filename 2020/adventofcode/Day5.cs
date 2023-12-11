using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace adventofcode
{
    public class Day5
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Day5(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        private const int MaxCols = 8;

        [Fact]
        public async Task Part1()
        {
            var input = await ReadInput();
            var ret = input.Select(x => x.Row * MaxCols + x.Col).Max();

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        [Fact]
        public async Task Part2()
        {
            var input = await ReadInput();
            var ret = input
                .GroupBy(x => x.Row)
                .Select(x => new {row = x.Key, cols = x.Select(y => y.Col).OrderBy(y => y).ToArray()})
                .OrderBy(x => x.row)
                .ToArray();

            for (int i = 1; i < ret.Length - 2; i++)
            {
                var current = ret[i];
                var prev = ret[i-1];
                var next = ret[i + 1];
                if (current.cols.Length < MaxCols && prev.cols.Length == MaxCols && next.cols.Length == MaxCols)
                {
                    _testOutputHelper.WriteLine($"Result is : {current.row * MaxCols + current.cols.Select((col, index) => col == index ? -1 : index).First(y => y != -1)}");

                }
            }
        }

        private static async Task<IList<DataModel>> ReadInput()
        {
            using var stream = File.OpenText("./Files/day5_input.txt");
            var fileText = await stream.ReadToEndAsync();
            return fileText.Split("\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(ParseRow)
                .ToArray();
        }

        private static DataModel ParseRow(string value)
        {
            var row = value.Substring(0, 7).Reverse().Select((y, i) => (byte) (y == 'B' ? 1 : 0) * (int) Math.Pow(2, i)).Sum();
            var col = value.Substring(7, 3).Reverse().Select((y, i) => (byte) (y == 'R' ? 1 : 0) * (int) Math.Pow(2, i)).Sum();

            return new DataModel(row, col);
        }

        class DataModel
        {
            public int Row { get; }
            public int Col { get; }

            public DataModel(int row, int col)
            {
                Row = row;
                Col = col;
            }
        }
    }
}

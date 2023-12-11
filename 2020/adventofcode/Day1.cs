using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace adventofcode
{
    public class Day1
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Day1(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Part1()
        {
            var input = await ReadInput();
            var ret = Helper(input, 0, input.Length - 1, 2020);
            foreach (var result in ret)
            {
                _testOutputHelper.WriteLine($"Result is : {result[0] * result[1]}, left is {result[0]}, right is {result[1]}");
            }
        }

        [Fact]
        public async Task Part2()
        {
            var input = await ReadInput();
            var ret = new List<int[]>();

            const int goal = 2020;
            const int oneThirdGoal = goal / 3 + 1;
            var startIndex = 0;

            while (true)
            {
                var startValue = input[startIndex];

                if (startValue > oneThirdGoal || startValue + 2 == input.Length - 1)
                    break;

                var currentRet = Helper(input, startIndex + 1, input.Length - 1, goal - startValue)
                    .Select(x => new[] {startValue}.Concat(x).ToArray());

                ret.AddRange(currentRet);

                startIndex++;
            }

            foreach (var result in ret)
            {
                _testOutputHelper.WriteLine($"Result is : {result[0] * result[1] * result[2]}, left is {result[0]}, mid is {result[1]}, right is {result[2]}");
            }
        }

        private IEnumerable<int[]> Helper(int[] ordered, int fromIndex, int toIndex, int goal)
        {
            var ret = new List<int[]>();
            var halfGoal = goal / 2 + 1;
            var leftIndex = fromIndex;
            var rightIndex = toIndex;

            while (true)
            {
                var left = ordered[leftIndex];
                var right = ordered[rightIndex];
                if (left > halfGoal || right < halfGoal || leftIndex + 1 == rightIndex)
                    break;

                var sum = left + right;
                if (sum == goal)
                {
                    ret.Add(new []{left, right });
                }

                if (sum > goal)
                {
                    rightIndex--;
                }
                else
                {
                    leftIndex++;
                }
            }

            return ret;
        }
        private static async Task<int[]> ReadInput()
        {
            using var stream = File.OpenText("./Files/day1_input.txt");
            var input = (await stream.ReadToEndAsync())
                .Split(new[] {"\n"}, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .OrderBy(x => x)
                .ToArray();
            return input;
        }
    }
}

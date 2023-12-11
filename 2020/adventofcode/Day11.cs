using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace adventofcode
{
    public class Day11
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Day11(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Part1()
        {
            var input = await ReadInput();
            var iteration = 0;
            var prev = input;
            while (true)
            {
                var next = NextIteration(prev, 4, GetNeighbors1);
                if (AreEqual(prev, next))
                {
                    break;
                }

                prev = next;
                iteration++;
            }


            var ret = prev.Sum(x => x.Count(y => y == Occupied));

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        [Fact]
        public async Task Part2()
        {
            var input = await ReadInput();
            var iteration = 0;
            var prev = input;
            while (true)
            {
                var next = NextIteration(prev, 5, GetNeighbors2);
                if (AreEqual(prev, next))
                {
                    break;
                }

                prev = next;
                iteration++;
            }


            var ret = prev.Sum(x => x.Count(y => y == Occupied));

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        private const char Occupied = '#';
        private const char Empty = 'L';
        private const char Floor = '.';

        private string GetString(char[][] input)
        {
            return string.Join("\n", input.Select(x => new string(x)));
        }

        private char[][] NextIteration(char[][] prev, int occupiedThreshold, Func<int, int, char[][], IList<char>> getNeighbors)
        {
            var ret = new char[prev.Length][];

            for (int i = 0; i < prev.Length; i++)
            {
                ret[i] = new char[prev[i].Length];
                for (int j = 0; j < prev[i].Length; j++)
                {
                    var neighbors = getNeighbors(i, j, prev);
                    switch (prev[i][j])
                    {
                        case Empty:
                            ret[i][j] = neighbors.All(x => x == Empty || x == Floor) ? Occupied : Empty; 
                            break;
                        case Occupied:
                            ret[i][j] = neighbors.Count(x => x == Occupied) >= occupiedThreshold ? Empty : Occupied;
                            break;
                        default:
                            ret[i][j] = prev[i][j];
                            break;
                    }
                }
            }

            return ret;
        }

        private bool AreEqual(char[][] prev, char[][] next)
        {
            for (int i = 0; i < prev.Length; i++)
            {
                for (int j = 0; j < prev[i].Length; j++)
                {
                    if (prev[i][j] != next[i][j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static IList<char> GetNeighbors1(int i, int j, char[][] input)
        {
            var ret = new List<char>();
            for (int k = -1; k <= 1; k++)
            {
                var index0 = i + k;
                if (index0 < 0 || index0 >= input.Length)
                    continue;

                for (int l = -1; l <= 1; l++)
                {
                    var index1 = j + l;
                    if (index1 < 0 || index1 >= input[i].Length)
                        continue;

                    if (k == 0 && l == 0)
                        continue;

                    ret.Add(input[index0][index1]);
                }
            }

            return ret;
        }

        private static IList<char> GetNeighbors2(int i, int j, char[][] input)
        {
            var ret = new List<char>();
            for (int k = -1; k <= 1; k++)
            {
                for (int l = -1; l <= 1; l++)
                {
                    if (k == 0 && l == 0)
                        continue;

                    var index0 = i;
                    var index1 = j;
                    while (true)
                    {
                        index0 += k;
                        index1 += l;

                        if (index0 < 0 || index0 >= input.Length || index1 < 0 || index1 >= input[i].Length)
                        {
                            break;
                        }

                        if (input[index0][index1] != Floor)
                        {
                            ret.Add(input[index0][index1]);
                            break;
                        }
                    }
                }
            }

            return ret;
        }

        private static async Task<char[][]> ReadInput()
        {
            using var stream = File.OpenText("./Files/day11_input.txt");
            var fileText = await stream.ReadToEndAsync();
            return fileText.Split("\n", StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToArray()).ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace adventofcode
{
    public class Day17
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Day17(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Part1()
        {
            var input = await ReadInput();
            var startState = input
                .SelectMany((line, yIndex) =>
                {
                    return line.Select((cell, xIndex) => cell == '#' ? new CubeCell(0, yIndex, xIndex) : null)
                        .Where(x => x != null);
                })
                .ToHashSet();

            var currentState = startState;
            for (int i = 0; i < 6; i++)
            {
                currentState = NextIteration(currentState, 3);
            }

            _testOutputHelper.WriteLine($"Result is : {currentState.Count}");
        }

        [Fact]
        public async Task Part2()
        {
            var input = await ReadInput();
            var startState = input
                .SelectMany((line, yIndex) =>
                {
                    return line.Select((cell, xIndex) => cell == '#' ? new CubeCell(0, 0, yIndex, xIndex) : null)
                        .Where(x => x != null);
                })
                .ToHashSet();

            var currentState = startState;
            for (int i = 0; i < 6; i++)
            {
                currentState = NextIteration(currentState, 4);
            }

            _testOutputHelper.WriteLine($"Result is : {currentState.Count}");
        }

        private static HashSet<CubeCell> NextIteration(HashSet<CubeCell> prevIterationActiveCells, int dimension)
        {
            var activeCells = new List<CubeCell>();
            var ranges = Enumerable.Range(0, dimension)
                .Select(i => Range.FromArray(prevIterationActiveCells.Select(x => x.Coordinates[i]).ToArray()).IncrementBorders())
                .ToArray();

            LoopOver(ranges, (cell) =>
            {
                var isActive = prevIterationActiveCells.Contains(cell);
                var neighbors = GetNeighbors(cell, dimension);
                var activeNeighborsCount = neighbors.Count(prevIterationActiveCells.Contains);

                if (isActive)
                {
                    if (activeNeighborsCount == 2 || activeNeighborsCount == 3)
                    {
                        activeCells.Add(cell.Copy());
                    }
                }
                else
                {
                    if (activeNeighborsCount == 3)
                    {
                        activeCells.Add(cell.Copy());
                    }
                }

            });

            return activeCells.ToHashSet();
        }

        private static IList<CubeCell> GetNeighbors(CubeCell center, int dimension)
        {
            var range = new Range(-1, 1);
            var ret = new List<CubeCell>();

            LoopOver(Enumerable.Range(0, dimension).Select(x => range).ToArray(), (cell) =>
            {
                if (!cell.Equals(CubeCell.Zero(dimension)))
                {
                    ret.Add(cell.Copy().Add(center));
                }
            });

            return ret;
        }

        private static void LoopOver(IList<Range> ranges, Action<CubeCell> action)
        {
            LoopOverInner(ranges, 0, CubeCell.Zero(ranges.Count).Copy(), action);
        }

        private static void LoopOverInner(IList<Range> ranges, int coordinateIndex, CubeCell cell, Action<CubeCell> action)
        {
            var range = ranges[coordinateIndex];
            for (int i = range.Min; i <= range.Max; i++)
            {
                cell.Coordinates[coordinateIndex] = i;
                if (coordinateIndex == ranges.Count - 1)
                {
                    action(cell);
                }
                else
                {
                    LoopOverInner(ranges, coordinateIndex + 1, cell, action);
                }
            }
        }


        private static async Task<IList<string>> ReadInput()
        {
            using var stream = File.OpenText("./Files/day17_input.txt");
            var fileText = await stream.ReadToEndAsync();

            return fileText.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        }

        [DebuggerDisplay("{Min,nq}-{Max,nq}")]
        class Range
        {
            public int Min { get; }
            public int Max { get; }

            public Range(int min, int max)
            {
                Min = min;
                Max = max;
            }

            public Range IncrementBorders()
            {
                return new Range(Min - 1, Max + 1);
            }

            public static Range FromArray(IList<int> values)
            {
                return values.Any() ? new Range(values.Min(), values.Max()) : new Range(0, 0);
            }
        }

        [DebuggerDisplay("{ToString(),nq}")]
        class CubeCell
        {
            public int[] Coordinates { get; }
            public CubeCell(params int [] coordinates)
            {
                Coordinates = coordinates;
            }

            private static Dictionary<int, CubeCell> cache = new Dictionary<int, CubeCell>();

            public static CubeCell Zero(int dimension)
            {
                if (!cache.TryGetValue(dimension, out var ret))
                {
                    ret = new CubeCell(new int[dimension]);
                    cache[dimension] = ret;
                }

                return ret;
            }

            public CubeCell Add(CubeCell other)
            {
                var newCoordinates = new int[Coordinates.Length];
                for (int i = 0; i < Coordinates.Length; i++)
                {
                    newCoordinates[i] = Coordinates[i] + other.Coordinates[i];
                }
                return new CubeCell(newCoordinates);
            }

            public CubeCell Copy()
            {
                var newCoordinates = new int[Coordinates.Length];
                for (int i = 0; i < Coordinates.Length; i++)
                {
                    newCoordinates[i] = Coordinates[i];
                }
                return new CubeCell(newCoordinates);
            }

            public override bool Equals(object? obj)
            {
                return obj is CubeCell other && Equals(other);
            }

            public bool Equals(CubeCell other)
            {
                if (other.Coordinates.Length != Coordinates.Length)
                    return false;

                for (int i = 0; i < Coordinates.Length; i++)
                {
                    if (Coordinates[i] != other.Coordinates[i])
                        return false;
                }

                return true;
            }

            public override int GetHashCode()
            {
                return Coordinates.Aggregate(0, (ret, current) => ret * 100 + current);
            }

            public override string ToString()
            {
                return string.Join(",", Coordinates);
            }
        }
    }
}

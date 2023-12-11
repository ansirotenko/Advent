using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace adventofcode
{
    public class Day24
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Day24(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Part1()
        {
            var input = await ReadInput();
            var cache = GetFloor(input);

            var ret = cache.Count(x => !x.Value.isWhite);

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        [Fact]
        public async Task Part2()
        {
            var input = await ReadInput();
            var cache = GetFloor(input);

            for (int i = 0; i < 100; i++)
            {
                var newCache = new Dictionary<HexCoordinate, (bool isWhite, int flipCount)>();
                var maxRadius = 2 + cache
                    .Where(x => !x.Value.isWhite)
                    .Max(x => x.Key.Radius);

                for (int radius = 0; radius < maxRadius; radius++)
                {
                    if (radius == 0)
                    {
                        var start = new HexCoordinate(radius, 0);
                        newCache[start] = (IsWhiteWithAdjusted(start, cache), -1);
                    }

                    for (int offset = 0; offset < radius * 6; offset++)
                    {
                        var current = new HexCoordinate(radius, offset);
                        newCache[current] = (IsWhiteWithAdjusted(current, cache), -1);
                    }
                }

                cache = newCache;
            }

            var ret = cache.Count(x => !x.Value.isWhite);

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        private static bool IsWhiteWithAdjusted(HexCoordinate coordinate, Dictionary<HexCoordinate, (bool isWhite, int flipCount)> cache)
        {
            var neighbors = Enumerable.Range(0, 6).Select(x => Next(coordinate, x)).ToList();

            var isWhite = IsWhiteSingle(coordinate, cache);
            var blackNeighbors = neighbors.Count(x => !IsWhiteSingle(x, cache));

            return isWhite && blackNeighbors != 2 || !isWhite && (blackNeighbors == 0 || blackNeighbors > 2);
        }

        private static bool IsWhiteSingle(HexCoordinate coordinate,  Dictionary<HexCoordinate, (bool isWhite, int flipCount)> cache)
        {
            return !cache.TryGetValue(coordinate, out var stats) || stats.isWhite;
        }

        private static Dictionary<HexCoordinate, (bool isWhite, int flipCount)> GetFloor(IList<List<int>> input)
        {
            var start = new HexCoordinate(0, 0);
            var cache = new Dictionary<HexCoordinate, (bool isWhite, int flipCount)>
            {
                { start, (true, 0) }
            };

            foreach (var directions in input)
            {
                var current = start;
                foreach (var direction in directions)
                {
                    current = Next(current, direction);
                }

                if (!cache.ContainsKey(current))
                {
                    cache.Add(current, (true, 0));
                }

                var stats = cache[current];
                cache[current] = (!stats.isWhite, stats.flipCount + 1);
            }

            return cache;
        }

        private static HexCoordinate Next(HexCoordinate prev, int direction)
        {
            if (prev.Radius == 0)
                return new HexCoordinate(1, direction);

            var closestCorner = prev.Offset / prev.Radius;
            var delta = RoundRotate(direction - closestCorner, 6);
            var offsetFromClosestCorner = prev.Offset - prev.Radius * closestCorner;
            
            var radiusPlusOne = prev.Radius + 1;
            var radiusMinusOne = prev.Radius - 1;
            var sameRadius = prev.Radius;

            if (offsetFromClosestCorner == 0)
            {
                return delta switch
                {
                    0 => new HexCoordinate(radiusPlusOne, closestCorner * radiusPlusOne),
                    1 => new HexCoordinate(radiusPlusOne, closestCorner * radiusPlusOne + 1),
                    2 => new HexCoordinate(sameRadius, closestCorner * sameRadius + 1),
                    3 => new HexCoordinate(radiusMinusOne, closestCorner * radiusMinusOne),
                    4 => new HexCoordinate(sameRadius, closestCorner * sameRadius - 1),
                    5 => new HexCoordinate(radiusPlusOne, closestCorner * radiusPlusOne - 1),
                    _ => throw new ArgumentException("Something totally wrong")
                };
            }

            return delta switch
            {
                0 => new HexCoordinate(radiusPlusOne, closestCorner * radiusPlusOne + offsetFromClosestCorner),
                1 => new HexCoordinate(radiusPlusOne, closestCorner * radiusPlusOne + 1 + offsetFromClosestCorner),
                2 => new HexCoordinate(sameRadius, closestCorner * sameRadius + 1 + offsetFromClosestCorner),
                3 => new HexCoordinate(radiusMinusOne, closestCorner * radiusMinusOne + offsetFromClosestCorner),
                4 => new HexCoordinate(radiusMinusOne, closestCorner * radiusMinusOne - 1 + offsetFromClosestCorner),
                5 => new HexCoordinate(sameRadius, closestCorner * sameRadius - 1 + offsetFromClosestCorner),
                _ => throw new ArgumentException("Something totally wrong")
            };
        }

        private static int RoundRotate(int value, int max)
        {
            if (max == 0)
                return value;

            value = value % max;
            return value < 0 ? max + value : value;
        }

        private static async Task<IList<List<int>>> ReadInput()
        {
            using var stream = File.OpenText("./Files/day24_input.txt");
            var fileText = await stream.ReadToEndAsync();
            return fileText
                .Split("\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(x =>
                {
                    var ret = new List<int>();

                    while (true)
                    {
                        if (x.Length == 0)
                        {
                            break;
                        }

                        var parseMapRecord = ParseMap.Single(y => x.StartsWith(y.Key));
                        ret.Add(parseMapRecord.Value);
                        x = x.Substring(parseMapRecord.Key.Length);
                    }

                    return ret;
                })
                .ToArray();
        }

        //private static Dictionary<string, Direction> ParseMap = new Dictionary<string, Direction>
        //{
        //    {"e", Direction.East},
        //    {"se", Direction.SouthEast},
        //    {"sw", Direction.SouthWest},
        //    {"w", Direction.West},
        //    {"nw", Direction.NorthWest},
        //    {"ne", Direction.NorthEast}
        //};

        private static Dictionary<string, int> ParseMap = new Dictionary<string, int>
        {
            {"e", 0},
            {"se", 1},
            {"sw", 2},
            {"w", 3},
            {"nw", 4},
            {"ne", 5}
        };

        class HexCoordinate
        {
            public int Radius { get; }
            public int Offset { get; }

            public HexCoordinate(int radius, int offset)
            {
                Radius = radius;
                Offset = RoundRotate(offset, Radius * 6);
            }

            public override bool Equals(object? obj)
            {
                return obj is HexCoordinate other && Equals(other);
            }

            public bool Equals(HexCoordinate other)
            {
                return Radius == other.Radius && Offset == other.Offset;
            }

            public override int GetHashCode()
            {
                return Radius * Radius + Offset;
            }

            public override string ToString()
            {
                return $"Radius: {Radius}, Offset: {Offset}";
            }
        }
    }
}

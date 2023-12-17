using Xunit;
using FluentAssertions;

namespace AoC2023;

public class Day5
{
    struct Range
    {
        public long Start { get; set; }
        public long End { get; set; }
    }
    record Map(long Dest, long Source, long Range);
    record Data(long[] Seeds, List<List<Map>> Categories);

    private Data Parse(string input)
    {
        var split = input.Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.None);

        var seeds = split[0]
                        .Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1]
                        .Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(long.Parse)
                        .ToArray();

        var categories = new List<List<Map>>();
        int i = 2;
        while (i < split.Length)
        {
            var name = split[i].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0];
            i++; // name
            var maps = new List<Map>();
            while (i < split.Length && !string.IsNullOrEmpty(split[i]))
            {
                var map = split[i].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
                maps.Add(new Map(map[0], map[1], map[2]));
                i++;
            }
            categories.Add(maps.OrderBy(x => x.Source).ToList());
            i++; // empty line
        }

        return new Data(seeds, categories);
    }

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int expected)
    {
        var data = Parse(input);

        var ret = long.MaxValue;

        foreach (var seed in data.Seeds)
        {
            var s = seed;
            foreach (var maps in data.Categories)
            {
                foreach (var map in maps)
                {
                    if (s >= map.Source && s < map.Source + map.Range)
                    {
                        s = map.Dest + (s - map.Source);
                        break;
                    }
                }
            }

            if (s < ret)
            {
                ret = s;
            }
        }

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            seeds: 79 14 55 13

                            seed-to-soil map:
                            50 98 2
                            52 50 48

                            soil-to-fertilizer map:
                            0 15 37
                            37 52 2
                            39 0 15

                            fertilizer-to-water map:
                            49 53 8
                            0 11 42
                            42 0 7
                            57 7 4

                            water-to-light map:
                            88 18 7
                            18 25 70

                            light-to-temperature map:
                            45 77 23
                            81 45 19
                            68 64 13

                            temperature-to-humidity map:
                            0 69 1
                            1 0 69

                            humidity-to-location map:
                            60 56 37
                            56 93 4
                            """,
                            35
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day5.txt"),
                            424490994
                        },
       };


    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, int expected)
    {
        var data = Parse(input);

        var ranges = Enumerable.Range(0, data.Seeds.Length / 2)
            .Select(i => new Range { Start = data.Seeds[i * 2], End = data.Seeds[i * 2] + data.Seeds[i * 2 + 1] });

        foreach (var maps in data.Categories)
        {
            ranges = ranges.SelectMany(r => MapRange(r, maps)).ToArray();
        }

        IEnumerable<Range> MapRange(Range range, List<Map> maps)
        {
            if (range.End <= maps.First().Source || range.Start >= maps.Last().Source + maps.Last().Range)
            {
                yield return range;
                yield break;
            }

            var start = range.Start;
            if (range.Start < maps.First().Source)
            {
                start = maps.First().Source;
                yield return new Range { Start = range.Start, End = maps.First().Source };
            }

            int i = 0;
            while (start > maps[i].Source + maps[i].Range)
            {
                i++;
            }
            start = Math.Max(maps[i].Source, range.Start);

            while (i < maps.Count)
            {
                if (range.End > maps[i].Source + maps[i].Range)
                {
                    yield return new Range { Start = maps[i].Dest + start - maps[i].Source, End = maps[i].Dest + maps[i].Range };
                    i++;
                    if (i == maps.Count)
                    {
                        break;
                    }
                    else
                    {
                        start = maps[i].Source;
                    }
                }
                else
                {
                    yield return new Range { Start = maps[i].Dest + start - maps[i].Source, End = maps[i].Dest + range.End - maps[i].Source };
                    yield break;
                }
            }

            yield return new Range { Start = maps.Last().Source + maps.First().Range, End = range.End };
        }

        var ret = ranges.Select(x => x.Start).Min();

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
        new List<object[]>
       {
            new object[]
                        {
                            """
                            seeds: 79 14 55 13

                            seed-to-soil map:
                            50 98 2
                            52 50 48

                            soil-to-fertilizer map:
                            0 15 37
                            37 52 2
                            39 0 15

                            fertilizer-to-water map:
                            49 53 8
                            0 11 42
                            42 0 7
                            57 7 4

                            water-to-light map:
                            88 18 7
                            18 25 70

                            light-to-temperature map:
                            45 77 23
                            81 45 19
                            68 64 13

                            temperature-to-humidity map:
                            0 69 1
                            1 0 69

                            humidity-to-location map:
                            60 56 37
                            56 93 4
                            """,
                            46
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day5.txt"),
                            15290096
                        },
       };
}

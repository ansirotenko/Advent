using Xunit;
using FluentAssertions;
namespace AoC2023;

public class Day22
{
    private Brick[] Parse(string input)
    {
        return input.Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select((r, i) =>
                {
                    var split = r.Split(new[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                    var p1 = split[0].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                    var p2 = split[1].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

                    return new Brick(new Range(p1[0], p2[0]), new Range(p1[1], p2[1]), new Range(p1[2], p2[2]));
                })
                .OrderBy(b => b.Z.Start)
                .ToArray();
    }

    record Range
    {
        public Range(int start, int end)
        {
            Start = start <= end ? start : end;
            End = start <= end ? end : start;
        }

        public int Start { get; init; }
        public int End { get; init; }
    }

    record Brick
    {
        public Brick(Range x, Range y, Range z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Range X { get; init; }
        public Range Y { get; init; }
        public Range Z { get; set; }
        public HashSet<Brick> Above { get; init; } = new HashSet<Brick>();
        public HashSet<Brick> Below { get; init; } = new HashSet<Brick>();
    }

    private void Fall(Brick[] data)
    {
        var maxY = data.Select(b => b.Y.End).Max() + 1;
        var maxX = data.Select(b => b.X.End).Max() + 1;

        var grid = Enumerable.Range(0, maxX).Select(_ => Enumerable.Range(0, maxY).Select(x => null as Brick).ToArray()).ToArray();

        foreach (var brick in data)
        {
            var z = 0;
            for (int x = brick.X.Start; x <= brick.X.End; x++)
            {
                for (int y = brick.Y.Start; y <= brick.Y.End; y++)
                {
                    if (grid[x][y] != null && grid[x][y]!.Z.End > z)
                    {
                        z = grid[x][y]!.Z.End;
                    }
                }
            }

            var delta = brick.Z.Start - z - 1;
            brick.Z = brick.Z with { Start = brick.Z.Start - delta, End = brick.Z.End - delta };

            for (int x = brick.X.Start; x <= brick.X.End; x++)
            {
                for (int y = brick.Y.Start; y <= brick.Y.End; y++)
                {
                    if (grid[x][y] != null && grid[x][y]!.Z.End == z)
                    {
                        if (grid[x][y]!.Above.Add(brick))
                        {
                            brick.Below.Add(grid[x][y]!);
                        }
                    }
                    grid[x][y] = brick;
                }
            }
        }
    }

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var data = Parse(input);

        Fall(data);

        var ret = 0L;
        foreach (var brick in data)
        {
            if (brick.Above.All(ba => ba.Below.Count > 1))
            {
                ret++;
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
                            1,0,1~1,2,1
                            0,0,2~2,0,2
                            0,2,3~2,2,3
                            0,0,4~0,2,4
                            2,0,5~2,2,5
                            0,1,6~2,1,6
                            1,1,8~1,1,9
                            """,
                            5
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day22.txt"),
                            416
                        },
       };


    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var data = Parse(input);

        Fall(data);

        var visited = new HashSet<Brick>();

        var ret = 0L;
        foreach (var brick in data)
        {
            visited.Clear();
            ret += brick.Above.Where(ba => ba.Below.Count == 1).Sum(b => GetAllAbove(visited, b)); 
        }

        ret.Should().Be(expected);
    }

    private int GetAllAbove(HashSet<Brick> visited, Brick brick)
    {
        if (visited.Add(brick)) 
        {
            return 1 + brick.Above.Where(x => x.Below.All(below => visited.Contains(below))).Sum(b => GetAllAbove(visited, b));
        }
    
        return 0;
    }

    public static IEnumerable<object[]> TestCases2 =>
        new List<object[]>
        {
            new object[]
                        {
                            """
                            1,0,1~1,2,1
                            0,0,2~2,0,2
                            0,2,3~2,2,3
                            0,0,4~0,2,4
                            2,0,5~2,2,5
                            0,1,6~2,1,6
                            1,1,8~1,1,9
                            """,
                            7
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day22.txt"),
                            60963
                        },
       };
}

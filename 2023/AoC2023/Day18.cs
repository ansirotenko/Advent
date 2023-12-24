using Xunit;
using FluentAssertions;
using System.Reflection;
namespace AoC2023;

public class Day18
{
    private Data[] Parse(string input)
    {
        return input.Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(r =>
                {
                    var split = r.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    var direction = split[0] switch
                    {
                        "R" => Direction.Right,
                        "L" => Direction.Left,
                        "U" => Direction.Top,
                        "D" => Direction.Bottom,
                        _ => throw new ArgumentException("Wrong direction")
                    };
                    var length = int.Parse(split[1]);
                    var color = split[2].Substring(1, split[2].Length - 2);
                    return new Data(direction, length, color);
                })
                .ToArray();
    }

    enum Direction { Top, Right, Bottom, Left }
    record Data(Direction Direction, int Length, string Color);

    record Point(int I, int J);

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var data = Parse(input);
        var ret = Solve(data);
        ret.Should().Be(expected);
    }

    private long Solve(Data[] data)
    {
        var points = GetAllPoints(data);

        var mapi = points.GroupBy(x => x.I).ToDictionary(x => x.Key, x => x.ToList());
        var mapJ = points.GroupBy(x => x.J).ToDictionary(x => x.Key, x => new Stack<Point>(x.OrderByDescending(x => x.I)));
        var pq = new Stack<int>(points.Select(x => x.I).Distinct().OrderByDescending(x => x));

        var ret = 0L;
        while (pq.Count != 0)
        {
            var iStart = pq.Pop();
            if (mapi.Remove(iStart, out var row))
            {
                var sortedRow = row.OrderBy(x => x.J).ToArray();
                for (int k = 0; k < sortedRow.Length / 2; k++)
                {
                    var jStart = sortedRow[k * 2].J;
                    var jFinish = sortedRow[k * 2 + 1].J;
                    var jStartPointBegin = mapJ[jStart].Pop();
                    var jFinishPointBegin = mapJ[jFinish].Pop();
                    if (jStartPointBegin.I != iStart)
                    {
                        throw new ArgumentException("wrong start");
                    }
                    if (jFinishPointBegin.I != iStart)
                    {
                        throw new ArgumentException("wrong finish");
                    }

                    if (!mapJ[jStart].TryPeek(out var jStartPointEnd))
                    {
                        jStartPointEnd = new Point(int.MaxValue, jStart);
                    }

                    if (!mapJ[jFinish].TryPeek(out var jFinishPointEnd))
                    {
                        jFinishPointEnd = new Point(int.MaxValue, jFinish);
                    }
                    var iFinish = pq.Peek();

                    ret += (iFinish - iStart + 1L) * (jFinish - jStart + 1L);

                    if (jStartPointEnd.I != iFinish)
                    {
                        var pt = new Point(iFinish, jStart);
                        mapi[iFinish].Add(pt);
                        mapJ[jStart].Push(pt);
                    }
                    else
                    {
                        mapJ[jStart].Pop();
                        mapi[iFinish].Remove(jStartPointEnd);
                    }

                    if (jFinishPointEnd.I != iFinish)
                    {
                        var pt = new Point(iFinish, jFinish);
                        mapi[iFinish].Add(pt);
                        mapJ[jFinish].Push(pt);
                    }
                    else
                    {
                        mapJ[jFinish].Pop();
                        mapi[iFinish].Remove(jFinishPointEnd);
                    }

                    var secondRow = mapi![iFinish].OrderBy(x => x.J).ToArray();
                    for (int t = 0; t < secondRow.Length / 2; t++)
                    {
                        var jb = secondRow[t * 2].J;
                        var je = secondRow[t * 2 + 1].J;

                        if (jb < jStart && je < jStart || jb > jFinish && je > jFinish)
                            continue;

                        ret -= Math.Min(je, jFinish) - Math.Max(jb, jStart) + 1L;
                    }
                }
            }
        }

        return ret;
    }

    private List<Point> GetAllPoints(Data[] data)
    {
        var points = new List<Point>();
        int i = 0;
        int j = 0;
        foreach (var (d, l, c) in data!)
        {
            switch (d)
            {
                case Direction.Top:
                    i -= l;
                    break;
                case Direction.Bottom:
                    i += l;
                    break;
                case Direction.Left:
                    j -= l;
                    break;
                case Direction.Right:
                    j += l;
                    break;
            }
            points.Add(new Point(i, j));
        }

        return points;
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            R 6 (#70c710)
                            D 5 (#0dc571)
                            L 2 (#5713f0)
                            D 2 (#d2c081)
                            R 2 (#59c680)
                            D 2 (#411b91)
                            L 5 (#8ceee2)
                            U 2 (#caa173)
                            L 1 (#1b58a2)
                            U 2 (#caa171)
                            R 2 (#7807d2)
                            U 3 (#a77fa3)
                            L 2 (#015232)
                            U 2 (#7a21e3)
                            """,
                            62
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day18.txt"),
                            38188
                        },
       };


    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var data = Parse(input);
        data = data
            .Select((d) =>
            {
                var newLength = 0;
                for(int i = 1; i < 6; i++) {
                    newLength *= 16;
                    
                    if (d.Color[i] >= '0' && d.Color[i] <= '9') {
                        newLength += d.Color[i] - '0';
                    } else {
                        newLength += d.Color[i] - 'a' + 10;
                    }
                }

                var newDirection = d.Color[6] switch {
                    '0' => Direction.Right,
                    '1' => Direction.Bottom,
                    '2' => Direction.Left,
                    '3' => Direction.Top,
                    _ => throw new ArgumentException("Wrong data")
                };

                return new Data(newDirection, newLength, d.Color);
            })
            .ToArray();

        var ret = Solve(data);
        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
        new List<object[]>
       {
            new object[]
                        {
                            """
                            R 6 (#70c710)
                            D 5 (#0dc571)
                            L 2 (#5713f0)
                            D 2 (#d2c081)
                            R 2 (#59c680)
                            D 2 (#411b91)
                            L 5 (#8ceee2)
                            U 2 (#caa173)
                            L 1 (#1b58a2)
                            U 2 (#caa171)
                            R 2 (#7807d2)
                            U 3 (#a77fa3)
                            L 2 (#015232)
                            U 2 (#7a21e3)
                            """,
                            952408144115
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day18.txt"),
                            93325849869340
                        },
       };
}

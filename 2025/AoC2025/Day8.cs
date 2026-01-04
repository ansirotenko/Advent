using Xunit;
using FluentAssertions;

namespace AoC2025;

public class Day8
{
    record Data(long X, long Y, long Z)
    {
        public long Distance(Data other)
        {
            return (X - other.X) * (X - other.X) + (Y - other.Y) * (Y - other.Y) + (Z - other.Z) * (Z - other.Z);
        }
    };

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int iterations, long expected)
    {
        var data = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x =>
                {
                    var split = x.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    return new Data(long.Parse(split[0]), long.Parse(split[1]), long.Parse(split[2]));
                })
                .ToList();
        var circuits = data
            .Select((x, i) => (Data: x, Label: i))
            .ToDictionary(x => x.Label, x => new List<Data>{x.Data});
        var labels = circuits
            .ToDictionary(x => x.Value.First(), x => x.Key);

        var distances = data.Take(data.Count - 1)
            .Select((d, i) => (
                Data: d,
                Distances: data.Skip(i + 1)
                    .Select(subd => (Distance: subd.Distance(d), Data: subd))
                    .OrderBy(x => x.Distance)
                    .ToList()
            ))
            .ToList();

        for (int i = 0; i < iterations; i ++)
        {
            var toConnect = distances.Where(x => x.Distances.Any()).OrderBy(x => x.Distances.First().Distance).First();
            var label = labels[toConnect.Data];
            var otherLabel = labels[toConnect.Distances.First().Data];
            if (label != otherLabel)
            {
                 foreach(var d in circuits[otherLabel])
                {
                    labels[d] = label;
                }
                circuits[label].AddRange(circuits[otherLabel]);
                circuits.Remove(otherLabel);
            }
            toConnect.Distances.RemoveAt(0);
        }

        var ret = circuits.Values
                .Select(x => x.Count)
                .OrderByDescending(x => x)
                .Take(3)
                .Aggregate(1L, (curr, next) => curr * next);

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            162,817,812
                            57,618,57
                            906,360,560
                            592,479,940
                            352,342,300
                            466,668,158
                            542,29,236
                            431,825,988
                            739,650,466
                            52,470,668
                            216,146,977
                            819,987,18
                            117,168,530
                            805,96,715
                            346,949,466
                            970,615,88
                            941,993,340
                            862,61,35
                            984,92,344
                            425,690,689
                            """,
                            10,
                            40
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day8.txt"),
                            1000,
                            175440L
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var data = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x =>
                {
                    var split = x.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    return new Data(long.Parse(split[0]), long.Parse(split[1]), long.Parse(split[2]));
                })
                .ToList();
        var circuits = data
            .Select((x, i) => (Data: x, Label: i))
            .ToDictionary(x => x.Label, x => new List<Data>{x.Data});
        var labels = circuits
            .ToDictionary(x => x.Value.First(), x => x.Key);

        var distances = data.Take(data.Count - 1)
            .Select((d, i) => (
                Data: d,
                Distances: data.Skip(i + 1)
                    .Select(subd => (Distance: subd.Distance(d), Data: subd))
                    .OrderBy(x => x.Distance)
                    .ToList()
            ))
            .ToList();

        Data last = null;
        Data otherLast = null;
        while (circuits.Count() != 1)
        {
            var toConnect = distances.Where(x => x.Distances.Any()).OrderBy(x => x.Distances.First().Distance).First();
            last = toConnect.Data;
            otherLast = toConnect.Distances.First().Data;
            var label = labels[last];
            var otherLabel = labels[otherLast];
            if (label != otherLabel)
            {
                 foreach(var d in circuits[otherLabel])
                {
                    labels[d] = label;
                }
                circuits[label].AddRange(circuits[otherLabel]);
                circuits.Remove(otherLabel);
            }
            toConnect.Distances.RemoveAt(0);
        }

        var ret = last!.X * otherLast!.X;

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
             new object[]
                        {
                            """
                            162,817,812
                            57,618,57
                            906,360,560
                            592,479,940
                            352,342,300
                            466,668,158
                            542,29,236
                            431,825,988
                            739,650,466
                            52,470,668
                            216,146,977
                            819,987,18
                            117,168,530
                            805,96,715
                            346,949,466
                            970,615,88
                            941,993,340
                            862,61,35
                            984,92,344
                            425,690,689
                            """,
                            25272
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day8.txt"),
                            3200955921L
                        },
    };
}

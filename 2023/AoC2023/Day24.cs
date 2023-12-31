using Xunit;
using FluentAssertions;
namespace AoC2023;

public class Day24
{
    private HailStone[] Parse(string input)
    {
        return input.Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(r =>
                {
                    var split = r.Split(new[] { "@" }, StringSplitOptions.RemoveEmptyEntries);

                    var coord = split[0].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => long.Parse(x.Trim())).ToArray();
                    var velocity = split[1].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x.Trim())).ToArray();

                    return new HailStone(coord[0], coord[1], coord[2], velocity[0], velocity[1], velocity[2]);
                })
                .ToArray();
    }

    record HailStone(decimal X, decimal Y, decimal Z, int VX, int VY, int VZ);

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected, long start, long end)
    {
        var data = Parse(input);

        var ret = 0L;
        for (var i = 0; i < data.Length; i++)
        {
            var h1 = data[i];
            for (var j = i + 1; j < data.Length; j++)
            {
                var h2 = data[j];
                if (h1.VX * h2.VY != h2.VX * h1.VY)
                {
                    var numX = h2.VX * (h1.VX * h1.Y - h1.X * h1.VY) - h1.VX * (h2.VX * h2.Y - h2.X * h2.VY);
                    var denumX = h1.VX * h2.VY - h2.VX * h1.VY;
                    var x = numX / denumX;
                    if (x < start || x > end)
                    {
                        continue;
                    }

                    var h3 = h1.VX == 0 ? h2 : h1;
                    var numY = h3.VY * numX + denumX * (h3.Y * h3.VX - h3.X * h3.VY);
                    var denumY = h3.VX * denumX;
                    var y = numY / denumY;
                    if (y < start || y > end)
                    {
                        continue;
                    }

                    if ((x - h1.X) * h1.VX < 0 || (y - h1.Y) * h1.VY < 0 ||
                        (x - h2.X) * h2.VX < 0 || (y - h2.Y) * h2.VY < 0)
                    {
                        continue;
                    }

                    ret++;
                }
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
                            19, 13, 30 @ -2,  1, -2
                            18, 19, 22 @ -1, -1, -2
                            20, 25, 34 @ -2, -2, -4
                            12, 31, 28 @ -1, -2, -1
                            20, 19, 15 @  1, -5, -3
                            """,
                            2,
                            7,
                            27
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day24.txt"),
                            15558,
                            200000000000000,
                            400000000000000
                        },
       };


    // [Theory]
    // [MemberData(nameof(TestCases2))]
    // public void Part2(string input, long expected)
    // {
    //     var data = Parse(input);
    //     var h1 = data[0];
    //     var h2 = data[1];
    //     var maxT = 1000;

    //     var count = 0;
    //     var dict = new Dictionary<int, List<int>>();
    //     for (var t1 = 0; t1 < maxT; t1++)
    //     {
    //         for (var t2 = 0; t2 < maxT; t2++)
    //         {
    //             if (t1 != t2)
    //             {
    //                 var deltaT = t2 - t1;
    //                 var coordX = ((h1.X + h1.VX * t1) * t2 - (h2.X + h2.VX * t2) * t1) / deltaT;
    //                 var coordY = ((h1.Y + h1.VY * t1) * t2 - (h2.Y + h2.VY * t2) * t1) / deltaT;
    //                 var coordZ = ((h1.Z + h1.VZ * t1) * t2 - (h2.Z + h2.VZ * t2) * t1) / deltaT;

    //                 var velX = (h2.X + h2.VX * t2 - h1.X - h1.VX * t1) / deltaT;
    //                 var velY = (h2.Y + h2.VY * t2 - h1.Y - h1.VY * t1) / deltaT;
    //                 var velZ = (h2.Z + h2.VZ * t2 - h1.Z - h1.VZ * t1) / deltaT;

    //                 if (coordX == Math.Floor(coordX) &&
    //                     coordY == Math.Floor(coordY) &&
    //                     coordZ == Math.Floor(coordZ) &&
    //                     velX == Math.Floor(velX) &&
    //                     velY == Math.Floor(velY) &&
    //                     velZ == Math.Floor(velZ))
    //                 {
    //                     var ok = data.Skip(2).All(h =>
    //                     {
    //                         var nx = h.X - coordX;
    //                         var ny = h.Y - coordY;
    //                         var nz = h.Z - coordZ;
    //                         var dx = velX - h.VX;
    //                         var dy = velY - h.VY;
    //                         var dz = velZ - h.VZ;

    //                         return (dx == 0 && nx == 0 || dx != 0 && nx / dx == Math.Floor(nx / dx)) &&
    //                                (dy == 0 && ny == 0 || dy != 0 && ny / dy == Math.Floor(ny / dy)) &&
    //                                (dz == 0 && nz == 0 || dz != 0 && nz / dz == Math.Floor(nz / dz));
    //                     });
    //                     if (ok)
    //                     {
    //                         count++;
    //                         if (!dict.TryGetValue(t1, out var lst))
    //                         {
    //                             dict[t1] = lst = new List<int>();
    //                         }

    //                         lst.Add(t2);
    //                     }
    //                 }
    //             }
    //         }
    //     }

    //     var ret = 0L;
    //     ret.Should().Be(expected);
    // }

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var data = Parse(input);
        var h1 = data[2];
        var h2 = data[1];

        var vyMin = int.MaxValue;
        var vxMin = int.MaxValue;
        var vyMax = int.MinValue;
        var vxMax = int.MinValue;

        foreach (var d in data)
        {
            vyMin = Math.Min(vyMin, d.VY);
            vxMin = Math.Min(vxMin, d.VX);
            vyMax = Math.Max(vyMax, d.VY);
            vxMax = Math.Max(vxMax, d.VX);
        }

        var count = 0;
        decimal retvxn = 0;
        decimal retvyn = 0;
        decimal retvzn = 0;
        decimal retxn = 0;
        decimal retyn = 0;
        decimal retzn = 0;
        for (var vxn = vyMin; vxn <= vyMax; vxn++)
        {
            for (var vyn = vxMin; vyn <= vxMax; vyn++)
            {
                var t2num = (h1.VX - vxn) * (h2.Y - h1.Y) - (h1.VY - vyn) * (h2.X - h1.X);
                var t2denum = (h1.VY - vyn) * (h2.VX - vxn) - (h1.VX - vxn) * (h2.VY - vyn);

                if (t2denum != 0 && t2num / t2denum == Math.Floor(t2num / t2denum))
                {
                    var t2 = t2num / t2denum;
                    if (t2 <= 0)
                        continue;

                    var t1num = (h1.VX - vxn) != 0
                                ? (h2.X - h1.X + (h2.VX - vxn) * t2)
                                : (h2.Y - h1.Y + (h2.VY - vyn) * t2);

                    var t1denum = (h1.VX - vxn) != 0
                                ? (h1.VX - vxn)
                                : (h1.VY - vyn);

                    if (t1denum != 0 && t1num / t1denum == Math.Floor(t1num / t1denum))
                    {
                        var t1 = t1num / t1denum;
                        if (t1 <= 0)
                            continue;

                        if (t2 != t1)
                        {
                            var vzn = (h2.Z + h2.VZ * t2 - h1.Z - h1.VZ * t1) / (t2 - t1);
                            if (vzn == Math.Floor(vzn))
                            {
                                var xn = h2.X + h2.VX * t2 - vxn * t2;
                                var yn = h2.Y + h2.VY * t2 - vyn * t2;
                                var zn = h2.Z + h2.VZ * t2 - vzn * t2;

                                if (xn == h1.X + h1.VX * t1 - vxn * t1 &&
                                    yn == h1.Y + h1.VY * t1 - vyn * t1 &&
                                    zn == h1.Z + h1.VZ * t1 - vzn * t1)
                                {
                                    var ok = data.Skip(2).All(h =>
                                                                   {
                                                                       var nx = h.X - xn;
                                                                       var ny = h.Y - yn;
                                                                       var nz = h.Z - zn;
                                                                       var dx = vxn - h.VX;
                                                                       var dy = vyn - h.VY;
                                                                       var dz = vzn - h.VZ;

                                                                       return (dx == 0 && nx == 0 || nx != 0 && dx != 0 && nx / dx == Math.Floor(nx / dx)) &&
                                                                              (dy == 0 && ny == 0 || ny != 0 && dy != 0 && ny / dy == Math.Floor(ny / dy)) &&
                                                                              (dz == 0 && nz == 0 || nz != 0 && dz != 0 && nz / dz == Math.Floor(nz / dz)) &&
                                                                              (dx != 0 || dy != 0 || dz != 0);
                                                                   });
                                    if (ok)
                                    {
                                        count++;
                                        retxn = xn;
                                        retyn = yn;
                                        retzn = zn;
                                        retvxn = vxn;
                                        retvyn = vyn;
                                        retvzn = vzn;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        var ret = retxn + retyn + retzn;
        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
        new List<object[]>
       {
            new object[]
                        {
                            """
                            19, 13, 30 @ -2,  1, -2
                            18, 19, 22 @ -1, -1, -2
                            20, 25, 34 @ -2, -2, -4
                            12, 31, 28 @ -1, -2, -1
                            20, 19, 15 @  1, -5, -3
                            """,
                            47
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day24.txt"),
                            765636044333842
                        },
       };
}

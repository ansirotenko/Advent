using Xunit;
using FluentAssertions;
using System.Text.RegularExpressions;
using System.Globalization;

namespace AoC2024;

public class Day13
{
    record Case(
        int AShiftX,
        int AShiftY,
        int BShiftX,
        int BShiftY,
        int PrizeX,
        int PrizeY
    ) {}

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var cases = new List<Case>();
        var regexp = new Regex("Button A: X\\+([0-9]+), Y\\+([0-9]+)\\W+Button B: X\\+([0-9]+), Y\\+([0-9]+)\\W+Prize: X=([0-9]+), Y=([0-9]+)");
        foreach(Match match in regexp.Matches(input)) {
            cases.Add(new Case(
                int.Parse(match.Groups[1].ToString()),
                int.Parse(match.Groups[2].ToString()),
                int.Parse(match.Groups[3].ToString()),
                int.Parse(match.Groups[4].ToString()),
                int.Parse(match.Groups[5].ToString()),
                int.Parse(match.Groups[6].ToString())
            ));
        }
        long ret = 0;
        foreach (var c in cases) {
            long jnom = c.BShiftY * c.PrizeX - c.BShiftX * c.PrizeY;
            long jdenom = c.BShiftY * c.AShiftX - c.BShiftX * c.AShiftY;
            if (jdenom != 0 && jnom % jdenom == 0) {
                long j = jnom / jdenom;
                
                long inom = c.PrizeY - c.AShiftY * j;
                long idenom = c.BShiftY;
                if (j >= 0 && idenom > 0 && inom % idenom == 0) {
                    long i = inom / idenom;

                    if (i >= 0 && i <= 100 && j <= 100) {
                        ret += i + j * 3;
                    }
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
                            Button A: X+94, Y+34
                            Button B: X+22, Y+67
                            Prize: X=8400, Y=5400

                            Button A: X+26, Y+66
                            Button B: X+67, Y+21
                            Prize: X=12748, Y=12176

                            Button A: X+17, Y+86
                            Button B: X+84, Y+37
                            Prize: X=7870, Y=6450

                            Button A: X+69, Y+23
                            Button B: X+27, Y+71
                            Prize: X=18641, Y=10279
                            """,
                            480
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day13.txt"),
                            29438
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var cases = new List<Case>();
        var regexp = new Regex("Button A: X\\+([0-9]+), Y\\+([0-9]+)\\W+Button B: X\\+([0-9]+), Y\\+([0-9]+)\\W+Prize: X=([0-9]+), Y=([0-9]+)");
        foreach(Match match in regexp.Matches(input)) {
            cases.Add(new Case(
                int.Parse(match.Groups[1].ToString()),
                int.Parse(match.Groups[2].ToString()),
                int.Parse(match.Groups[3].ToString()),
                int.Parse(match.Groups[4].ToString()),
                int.Parse(match.Groups[5].ToString()),
                int.Parse(match.Groups[6].ToString())
            ));
        }
        long ret = 0;
        foreach (var c in cases) {
            long x = c.PrizeX + 10000000000000;
            long y = c.PrizeY + 10000000000000;
            long jnom = c.BShiftY * x - c.BShiftX * y;
            long jdenom = c.BShiftY * c.AShiftX - c.BShiftX * c.AShiftY;
            if (jdenom != 0 && jnom % jdenom == 0) {
                long j = jnom / jdenom;
                
                long inom = y - c.AShiftY * j;
                long idenom = c.BShiftY;
                if (j >= 0 && idenom > 0 && inom % idenom == 0) {
                    long i = inom / idenom;

                    if (i >= 0) {
                        ret += i + j * 3;
                    }
                }
            }
        }

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
                        new object[]
                        {
                            """
                            Button A: X+94, Y+34
                            Button B: X+22, Y+67
                            Prize: X=8400, Y=5400

                            Button A: X+26, Y+66
                            Button B: X+67, Y+21
                            Prize: X=12748, Y=12176

                            Button A: X+17, Y+86
                            Button B: X+84, Y+37
                            Prize: X=7870, Y=6450

                            Button A: X+69, Y+23
                            Button B: X+27, Y+71
                            Prize: X=18641, Y=10279
                            """,
                            875318608908L
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day13.txt"),
                            104958599303720L
                        },
    };
}

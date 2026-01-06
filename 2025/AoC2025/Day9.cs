using Xunit;
using FluentAssertions;

namespace AoC2025;

public class Day9
{
    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var rows = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x =>
            {
                var split = x.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                return (X: long.Parse(split[0]), Y:long.Parse(split[1]));
            })
            .ToArray();
        var ret = 0L;

        for (var i = 0; i < rows.Length - 1; i ++)
        {
            for (var j = i + 1; j < rows.Length; j ++)
            {
                var minX = Math.Min(rows[i].X, rows[j].X);
                var maxX = Math.Max(rows[i].X, rows[j].X);
                var minY = Math.Min(rows[i].Y, rows[j].Y);
                var maxY = Math.Max(rows[i].Y, rows[j].Y);
                var curr = (maxX - minX + 1) * (maxY - minY + 1);
                if (curr > ret)
                {
                    ret = curr;
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
                            7,1
                            11,1
                            11,7
                            9,7
                            9,5
                            2,5
                            2,3
                            7,3
                            """,
                            50
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day9.txt"),
                            4741848414L
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
         var rows = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x =>
            {
                var split = x.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                return (X: int.Parse(split[0]), Y:int.Parse(split[1]));
            })
            .ToArray();
        var ret = 0L;   


        for (var i = 0; i < rows.Length - 1; i ++)
        {
            for (var j = i + 1; j < rows.Length; j ++)
            {
                var minX = Math.Min(rows[i].X, rows[j].X);
                var maxX = Math.Max(rows[i].X, rows[j].X);
                var minY = Math.Min(rows[i].Y, rows[j].Y);
                var maxY = Math.Max(rows[i].Y, rows[j].Y);
                var curr = (maxX - minX + 1) * (maxY - minY + 1);
                var acceptable = true;

                for (var k = 0; k < rows.Length; k++) 
                {
                    var subMinX = Math.Min(rows[k].X, rows[(k + 1) % rows.Length].X);
                    var subMaxX = Math.Max(rows[k].X, rows[(k + 1) % rows.Length].X);
                    var subMinY = Math.Min(rows[k].Y, rows[(k + 1) % rows.Length].Y);
                    var subMaxY = Math.Max(rows[k].Y, rows[(k + 1) % rows.Length].Y);
                    if (subMaxY >= minY+1 && subMinY <= maxY-1 && subMaxX >= minX+1 && subMinX <= maxX-1) {
                        acceptable = false;
                        break;
                    }
                }

                if (acceptable && curr > ret)
                {
                    ret = curr;
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
                            7,1
                            11,1
                            11,7
                            9,7
                            9,5
                            2,5
                            2,3
                            7,3
                            """,
                            24
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day9.txt"),
                            1508918480L
                        },
    };
}

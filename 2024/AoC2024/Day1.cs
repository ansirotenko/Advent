using Xunit;
using FluentAssertions;

namespace AoC2024;

public class Day1
{
    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int expected)
    {
        var rows = input.Split(new []{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);
        var left = new List<int>(rows.Length);
        var right = new List<int>(rows.Length);
        for (int i = 0; i < rows.Length; i++) {
            var split = rows[i].Split(new []{"   "}, StringSplitOptions.RemoveEmptyEntries);
            left.Add(int.Parse(split[0]));
            right.Add(int.Parse(split[1]));
        };
        left.Sort();
        right.Sort();

        int ret = 0;
        for (int i = 0; i < rows.Length; i++) {
            if (left[i] > right[i]) {
                ret += left[i] - right[i];
            } else {
                ret += right[i] - left[i];
            }
        };

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            3   4
                            4   3
                            2   5
                            1   3
                            3   9
                            3   3
                            """,
                            11
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day1.txt"),
                            1873376
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, int expected)
    {
        var rows = input.Split(new []{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);
        var left = new int[rows.Length];
        var right = new Dictionary<int, int>();
        for (int i = 0; i < rows.Length; i++) {
            var split = rows[i].Split(new []{"   "}, StringSplitOptions.RemoveEmptyEntries);
            left[i] = int.Parse(split[0]);
            var rightNum = int.Parse(split[1]);
            if (right.ContainsKey(rightNum)) {
                right[rightNum]++;
            } else {
                right[rightNum] = 1;
            }
        };

        int ret = 0;
        foreach (var num in left) {
            if (right.ContainsKey(num)) {
                ret += num * right[num];
            }
        };

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            3   4
                            4   3
                            2   5
                            1   3
                            3   9
                            3   3
                            """,
                            31
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day1.txt"),
                            18997088
                        },
    };
}

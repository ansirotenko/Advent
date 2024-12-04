using Xunit;
using FluentAssertions;

namespace AoC2024;

public class Day2
{
    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int expected)
    {
        var ret = input
            .Split(new []{"\n\r", "\n"}, StringSplitOptions.RemoveEmptyEntries)
            .Select(row => {
                var levels = row
                    .Split(new []{" "}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(level => int.Parse(level))
                    .ToList();
                var sign = 0;
                for (var i = 0; i < levels.Count - 1; i++) {
                    var delta = levels[i] - levels[i + 1];
                    if (sign == 0) {
                        sign = delta < 0 ? -1 : 1;
                    }
                    if (sign * delta <= 0 || sign * delta > 3) {
                        return false;
                    }
                }
                return true;
            })
            .Count(x => x);

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            7 6 4 2 1
                            1 2 7 8 9
                            9 7 6 2 1
                            1 3 2 4 5
                            8 6 4 4 1
                            1 3 6 7 9
                            """,
                            2
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day2.txt"),
                            472
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, int expected)
    {
        var ret = input
            .Split(new []{"\n\r", "\n"}, StringSplitOptions.RemoveEmptyEntries)
            .Select(row => {
                var levels = row
                    .Split(new []{" "}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(level => int.Parse(level))
                    .ToList();

                for (int i = -1; i < levels.Count; i ++) {
                    if (TestDelta(i)) {
                        return true;
                    }
                }

                return false;
                
                bool TestDelta(int index) {
                    var sign = 0;
                    var start = index == 0 ? 1 : 0;
                    var end = index == levels.Count -1 ? levels.Count - 2 : levels.Count - 1;
                    for (var i = start; i < end; i++) {
                        if (index != i) {
                            var nextIndex = i + 1 == index 
                                            ? i + 2
                                            : i + 1;
                            var delta = levels[i] - levels[nextIndex];
                            if (sign == 0) {
                                sign = delta < 0 ? -1 : 1;
                            }
                            if (sign * delta <= 0 || sign * delta > 3) {
                                return false;
                            }
                        }
                    }
                    return true;
                }
            })
            .Count(x => x);

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
           new object[]
                        {
                            """
                            7 6 4 2 1
                            1 2 7 8 9
                            9 7 6 2 1
                            1 3 2 4 5
                            8 6 4 4 1
                            1 3 6 7 9
                            """,
                            4
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day2.txt"),
                            520
                        },
    };
}

using Xunit;
using FluentAssertions;

namespace AoC2024;

public class Day25
{
    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int expected)
    {
        var parts = input.Split(["\r\n\r\n", "\n\n"], StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries))
            .ToArray();
        var locks = parts.Where(x => x.First().All(y => y == '#')).ToArray();
        var keys = parts.Where(x => x.Last().All(y => y == '#')).ToArray();
        var ret = 0;
        foreach(var k in keys) {
            foreach(var l in locks) {
                var n = k.Length;
                var m = k.First().Length;
                if (Enumerable.Range(0, n *m ).All(i => k[i / m][i % m] == '.' ||  l[i / m][i % m] == '.')) {
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
                            #####
                            .####
                            .####
                            .####
                            .#.#.
                            .#...
                            .....

                            #####
                            ##.##
                            .#.##
                            ...##
                            ...#.
                            ...#.
                            .....

                            .....
                            #....
                            #....
                            #...#
                            #.#.#
                            #.###
                            #####

                            .....
                            .....
                            #.#..
                            ###..
                            ###.#
                            ###.#
                            #####

                            .....
                            .....
                            .....
                            #....
                            #.#..
                            #.#.#
                            #####
                            """,
                            3
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day25.txt"),
                            3301
                        },
    };
}

using Xunit;
using FluentAssertions;

namespace AoC2024;

public class Day4
{
    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int expected)
    {
        (int dx, int dy)[] directions = new [] {
            (1, 0),
            (0, 1),
            (-1, 0),
            (0, -1),
            (1, 1),
            (1, -1),
            (-1, 1),
            (-1, -1),
        };
        string restString = "MAS";

        var rows = input.Split(new []{"\n\r", "\n"}, StringSplitOptions.RemoveEmptyEntries);
        var ret = 0;
        for (int i = 0; i < rows.Length; i++) {
            for (int j = 0; j < rows[i].Length; j++)
            {
                if (rows[i][j] == 'X') {
                    foreach(var (dx, dy) in directions) {
                        int k = 0;
                        int nextI = i;
                        int nextJ = j;
                        while(k < restString.Length) {
                            nextI += dx;
                            nextJ += dy;
                            if (nextI < 0 || nextI >= rows.Length || nextJ < 0 || nextJ >= rows[nextI].Length || rows[nextI][nextJ] != restString[k]) {
                                break;
                            }
                            k++;
                        }
                        if (k == restString.Length) {
                            ret++;
                        }
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
                            MMMSXXMASM
                            MSAMXMSMSA
                            AMXSXMAAMM
                            MSAMASMSMX
                            XMASAMXAMM
                            XXAMMXXAMA
                            SMSMSASXSS
                            SAXAMASAAA
                            MAMMMXMMMM
                            MXMXAXMASX
                            """,
                            18
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day4.txt"),
                            2662
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, int expected)
    {
        (int dx, int dy)[] directions = new [] {
            (1, 1),
            (1, -1),
            (-1, -1),
            (-1, 1),
        };

        var rows = input.Split(new []{"\n\r", "\n"}, StringSplitOptions.RemoveEmptyEntries);
        var ret = 0;
        var buff = new char[4];
        for (int i = 0; i < rows.Length; i++) {
            for (int j = 0; j < rows[i].Length; j++)
            {
                if (rows[i][j] == 'A') {

                    int k = 0;
                    foreach(var (dx, dy) in directions) {
                        var nextI = i + dx;
                        var nextJ = j + dy;
                        if (nextI < 0 || 
                            nextI >= rows.Length || 
                            nextJ < 0 || 
                            nextJ >= rows[nextI].Length
                        ) 
                        {
                            break;
                        }
                        buff[k] = rows[nextI][nextJ];
                        k++;
                    }
                    if (k == directions.Length) {
                        int m = 2;
                        int s = 2;
                        foreach(var c in buff) {
                            if (c == 'M') {
                                m--;
                            }
                            if (c == 'S') {
                                s--;
                            }
                        }
                        if (m == 0 && s == 0) {
                            for(k = 0; k < directions.Length - 1; k++) {
                                if (buff[k] == buff[k+1]) {
                                    ret++;
                                    break;
                                }
                            }
                        }
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
                            MMMSXXMASM
                            MSAMXMSMSA
                            AMXSXMAAMM
                            MSAMASMSMX
                            XMASAMXAMM
                            XXAMMXXAMA
                            SMSMSASXSS
                            SAXAMASAAA
                            MAMMMXMMMM
                            MXMXAXMASX
                            """,
                            9
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day4.txt"),
                            2034
                        },
    };
}

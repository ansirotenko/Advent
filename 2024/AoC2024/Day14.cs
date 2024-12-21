using Xunit;
using FluentAssertions;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Text;

namespace AoC2024;

public class Day14
{
    class Case{
        public int I { get; set; }
        public int J { get; set; }
        public int Vi { get; set; }
        public int Vj { get; set; }
    }

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int n, int m, int expected)
    {
        var cases = new List<Case>();
        var regexp = new Regex("p=+([0-9]+),([0-9]+) v=([-|0-9]+),([-|0-9]+)");
        foreach(Match match in regexp.Matches(input)) {
            cases.Add(new Case{
                J = int.Parse(match.Groups[1].ToString()),
                I = int.Parse(match.Groups[2].ToString()),
                Vj = int.Parse(match.Groups[3].ToString()),
                Vi = int.Parse(match.Groups[4].ToString())
            });
        }
        int topLeft = 0;
        int topRight = 0;
        int bottomLeft = 0;
        int bottomRight = 0;
        foreach (var c in cases) {
            int j = (c.J + 100 * c.Vj) % m;
            if (j < 0) {
                j += m;
            }
            int i = (c.I + 100 * c.Vi) % n;
            if (i < 0) {
                i += n;
            }
            if (i != n / 2 && j != m / 2) {
                if (i < n / 2) {
                    if (j < m / 2) {
                        topLeft++;
                    } else {
                        topRight++;
                    }
                } else {
                    if (j < m / 2) {
                        bottomLeft++;
                    } else {
                        bottomRight++;
                    }
                }
            }
        }
        int ret = topLeft * topRight * bottomLeft * bottomRight;

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            p=0,4 v=3,-3
                            p=6,3 v=-1,-3
                            p=10,3 v=-1,2
                            p=2,0 v=2,-1
                            p=0,0 v=1,3
                            p=3,0 v=-2,-2
                            p=7,6 v=-1,-3
                            p=3,0 v=-1,-2
                            p=9,3 v=2,3
                            p=7,3 v=-1,2
                            p=2,4 v=2,-3
                            p=9,5 v=-3,-3
                            """,
                            7,
                            11,
                            12
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day14.txt"),
                            103,
                            101,
                            224969976
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, int n, int m, int expected)
    {
        var cases = new List<Case>();
        var regexp = new Regex("p=+([0-9]+),([0-9]+) v=([-|0-9]+),([-|0-9]+)");
        foreach(Match match in regexp.Matches(input)) {
            cases.Add(new Case{
                J = int.Parse(match.Groups[1].ToString()),
                I = int.Parse(match.Groups[2].ToString()),
                Vj = int.Parse(match.Groups[3].ToString()),
                Vi = int.Parse(match.Groups[4].ToString())
            });
        }
        (int di, int dj)[] directions = new [] {
            (1, 0),
            (1, -1),
            (0, -1),
            (-1, -1),
            (-1, 0),
            (-1, 1),
            (0, 1),
            (1, 1),
        };
        var grid = Enumerable.Range(0, n).Select(i => new int[m]).ToArray();
        var ret = 0;
        foreach (var c in cases) {
            grid[c.I][c.J]++;
        }
        while(true) {
            foreach (var c in cases) {
                int j = (c.J + c.Vj) % m;
                if (j < 0) {
                    j += m;
                }
                int i = (c.I + c.Vi) % n;
                if (i < 0) {
                    i += n;
                }
                grid[c.I][c.J]--;
                grid[i][j]++;
                c.J = j;
                c.I = i;
            }
            ret++;
            var neighbours0 = 0;
            var neighbours1 = 0;
            var neighbours2plus = 0;
            foreach (var c in cases) {
                var neighbours = 0;
                foreach(var direction in directions) {
                    var ni = c.I + direction.di;
                    var nj = c.J + direction.dj;
                    if (ni >= 0 && ni < n && nj >=0 && nj < m && grid[ni][nj] > 0) {
                        neighbours++;
                    }
                }
                switch (neighbours) {
                    case 0: 
                        neighbours0++;
                        break;
                    case 1: 
                        neighbours1++;
                        break;
                    default: 
                        neighbours2plus++;
                        break;
                }
            }
            if (neighbours2plus >= cases.Count * 2 / 3) {
                PrintGrid();
                break;
            }
        }

        void PrintGrid(){
            var sb = new StringBuilder();
            for (int i = 0; i < n; i++) {
                for (int j = 0; j < m; j++) {
                    sb.Append(grid[i][j] == 0 ? '.' : 'X');
                } 
                sb.AppendLine();
            }
            sb.AppendLine("================================================================================================================");
            Debug.WriteLine(sb);
        }

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
            new object[]
                        {
                            File.ReadAllText("Inputs/Day14.txt"),
                            103,
                            101,
                            7892
                        },
    };
}

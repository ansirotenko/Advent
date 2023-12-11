using System.IO;
using Xunit;
using FluentAssertions;

namespace AoC2023;

public class Day1
{
    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int expected)
    {
        var ret = input.Split(new []{"\n\r", "\n"}, StringSplitOptions.RemoveEmptyEntries)
            .Select(str => {
                var first = str.Select(c => c - '0').FirstOrDefault(n => n >= 0 && n <= 9);
                var last = str.Select(c => c - '0').LastOrDefault(n => n >= 0 && n <= 9);
                return first * 10 + last;
            })
            .Sum();
        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            1abc2
                            pqr3stu8vwx
                            a1b2c3d4e5f
                            treb7uchet
                            """,
                            142
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day1.txt"),
                            55538
                        },
       };

    private Dictionary<string, int> digits = new Dictionary<string, int>
    {
        {"one", 1},
        {"two", 2},
        {"three", 3},
        {"four", 4},
        {"five", 5},
        {"six", 6},
        {"seven", 7},
        {"eight", 8},
        {"nine", 9},
        {"zero", 0},
    };

    public class TextNode
    {
        public int? Value { get; set; }
        public Dictionary<char, TextNode> Children { get; set; } = new();
    }

    private TextNode GetTextNodeRoot()
    {
        var root = new TextNode();
        foreach(var (key, value) in digits) 
        {
            var current = root;
            foreach(var c in key) 
            {
                if (!current.Children.TryGetValue(c, out var next)) {
                    current.Children[c] = new TextNode();
                }
                current = current.Children[c];
            }

            current.Value = value;
        }

        return root;
    }

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, int expected)
    {
        var root = GetTextNodeRoot();
        var ret = input.Split(new []{"\n\r", "\n"}, StringSplitOptions.RemoveEmptyEntries)
            .Select(str => {
                var first = 0;
                for(int i = 0; i < str.Length; i++)
                {
                    if (Check(str, i, ref first)) 
                    {
                        break;
                    }
                }

                var last = 0;
                for(int i = str.Length - 1; i >= 0; i--)
                {
                    if (Check(str, i, ref last)) 
                    {
                        break;
                    }
                }
                return first * 10 + last;
            })
            .Sum();
        ret.Should().Be(expected);

        bool Check(string str, int i, ref int ret)
        {
            var c = str[i] - '0';
            if (c >= 0 && c <= 9)
            {
                ret = c;
                return true;
            }
            var current = root;
            while (i < str.Length && current.Children.ContainsKey(str[i]))
            {
                current = current.Children[str[i]];
                i++;
            }
            if (current.Value.HasValue)
            {
                ret = current.Value.Value;
                return true;
            }

            return false;
        }
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            two1nine
                            eightwothree
                            abcone2threexyz
                            xtwone3four
                            4nineeightseven2
                            zoneight234
                            7pqrstsixteen
                            """,
                            281
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day1.txt"),
                            54875
                        },
       };
}

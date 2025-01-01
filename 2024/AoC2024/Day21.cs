using Xunit;
using FluentAssertions;

namespace AoC2024;

public class Day21
{
    record Point(int I, int J);
    class Pad {
        public static Dictionary<Point, char> NumKeys = new Dictionary<Point, char>{
                    {new Point(0, 0), '7'},
                    {new Point(0, 1), '8'},
                    {new Point(0, 2), '9'},
                    {new Point(1, 0), '4'},
                    {new Point(1, 1), '5'},
                    {new Point(1, 2), '6'},
                    {new Point(2, 0), '1'},
                    {new Point(2, 1), '2'},
                    {new Point(2, 2), '3'},
                    {new Point(3, 1), '0'},
                    {new Point(3, 2), 'A'},
                };
        public static Dictionary<Point, char> RoboKeys = new Dictionary<Point, char>{
                    {new Point(0, 1), '^'},
                    {new Point(0, 2), 'A'},
                    {new Point(1, 0), '<'},
                    {new Point(1, 1), 'v'},
                    {new Point(1, 2), '>'},
                };

        public Dictionary<char, Dictionary<char, List<string>>> Cache { get; }

        public Pad(Dictionary<Point, char> keys) {
            Cache = keys.ToDictionary(
                c1 => c1.Value,
                c1 => keys.ToDictionary(
                    c2 => c2.Value,
                    c2 => {
                        var ret = new List<string>();
                        Shuffle(c1.Key, c2.Key, new Stack<char>(), ret, keys);
                        return ret;
                    } 
                ) 
            );
        }

        public static void Shuffle(Point current, Point target, Stack<char> trace, List<string> result, Dictionary<Point, char> keys) {
            if (keys.ContainsKey(current)) {
                if (current == target) {
                    trace.Push('A');
                    result.Add(new string(trace.Reverse().ToArray()));
                    trace.Pop();
                } else {
                    if (current.I != target.I) {
                        var newChar = current.I > target.I ? '^' : 'v';
                        var newI = current.I > target.I ? current.I - 1 : current.I + 1;
                        trace.Push(newChar);
                        Shuffle(current with {I = newI}, target, trace, result, keys);
                        trace.Pop();
                    }
                    if (current.J != target.J) {
                        var newChar = current.J > target.J ? '<' : '>';
                        var newJ = current.J > target.J ? current.J - 1 : current.J + 1;
                        trace.Push(newChar);
                        Shuffle(current with { J = newJ}, target, trace, result, keys);
                        trace.Pop();
                    }
                }
            } else {
                if (1 == 1) {

                }
            }
        }
    }

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int expected)
    {
        var codes = input.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);
        var ret = 0;
        var roboPad = new Pad(Pad.RoboKeys);
        var numPad = new Pad(Pad.NumKeys);

        foreach(var code in codes) {
            var shortest = Solve(
                [numPad, roboPad, roboPad], 0, code
            );

            ret += int.Parse(code.Substring(0, code.Length - 1)) * shortest;
        }

        int Solve(Pad[] pads, int padIndex, string code) {
            var prev = 'A';
            var ret = 0;
            foreach(var c in code) {
                ret += pads[padIndex].Cache[prev][c]
                    .Select(x => padIndex == pads.Length - 1 ? x.Length : Solve(pads, padIndex + 1, x))
                    .Min();
                prev = c;
            }
            return ret;
        }

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            029A
                            980A
                            179A
                            456A
                            379A
                            """,
                            126384
                       },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day21.txt"),
                            184716
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var codes = input.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);
        var ret = 0L;
        var roboPad = new Pad(Pad.RoboKeys);
        var numPad = new Pad(Pad.NumKeys);
        var allPads = new []{numPad}.Concat(Enumerable.Range(0, 25).Select(x => roboPad)).ToArray();
        var cache = allPads.Select(x => new Dictionary<string, long>()).ToArray();

        foreach(var code in codes) {
            var shortest = Solve(allPads, 0, code);

            ret += int.Parse(code.Substring(0, code.Length - 1)) * shortest;
        }

        long Solve(Pad[] pads, int padIndex, string code) {
            var prev = 'A';
            if (!cache[padIndex].TryGetValue(code, out var ret)) {
                ret = 0;
                foreach(var c in code) {
                    ret += pads[padIndex].Cache[prev][c]
                        .Select(x => padIndex == pads.Length - 1 ? x.Length : Solve(pads, padIndex + 1, x))
                        .Min();
                    prev = c;
                }
                cache[padIndex][code] = ret;
            }
            
            return ret;
        }

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
            new object[]
                        {
                            File.ReadAllText("Inputs/Day21.txt"),
                            229403562787554L
                        },
    };
}

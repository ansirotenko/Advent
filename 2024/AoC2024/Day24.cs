using Xunit;
using FluentAssertions;
using System.Text.RegularExpressions;

namespace AoC2024;

public class Day24
{
    class Data {
        public Dictionary<string, Rule> Rules { get; }
        public Dictionary<string, bool> State { get; }

        public Data(string input) {
            Rules = new Dictionary<string, Rule>();
            State = new Dictionary<string, bool>();
            var stateRegex = new Regex("([a-z0-9]{3}):\\s(0|1)");
            foreach(Match match in stateRegex.Matches(input)) {
                State[match.Groups[1].Value] = match.Groups[2].Value == "1"; 
            }
            var ruleRegex = new Regex("([a-z0-9]{3})\\s(AND|OR|XOR)\\s([a-z0-9]{3})\\s->\\s([a-z0-9]{3})");
            foreach(Match match in ruleRegex.Matches(input)) {
                var left = match.Groups[1].Value;
                var op = match.Groups[2].Value;
                var right = match.Groups[3].Value;
                var outer = match.Groups[4].Value;
                if (left[0] == 'y') {
                    var tmp = left;
                    left = right;
                    right = tmp;
                }

                Rules[match.Groups[4].Value] = new Rule(left, op, right, outer);
            }
        }
    }

    record Rule(string Left, string Op, string Right, string Out);

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var data = new Data(input);
        long ret = Solve(data);
        ret.Should().Be(expected);
    }

    long Solve(Data data) {
        long ret = 0L;
        var stack = new Stack<string>();
        foreach(var zName in data.Rules.Keys.Where(key => key[0] == 'z').OrderByDescending(x => x)) {
            stack.Push(zName);
            while(stack.Any()) {
                var ruleOut = stack.Peek();
                var rule = data.Rules[stack.Peek()];
                if (data.State.TryGetValue(rule.Left, out var left)) {
                    if (data.State.TryGetValue(rule.Right, out var right)) {
                        data.State[ruleOut] = rule.Op switch {
                            "AND" => left && right,
                            "OR" => left || right,
                            "XOR" => left ^ right,
                            _ => throw new ArgumentException()
                        };
                        stack.Pop();
                    } else {
                        stack.Push(rule.Right);
                    }
                } else {
                    stack.Push(rule.Left);
                }
            }
            ret = (ret << 1) + (data.State[zName] ? 1 : 0);
        }
        return ret;
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            x00: 1
                            x01: 1
                            x02: 1
                            y00: 0
                            y01: 1
                            y02: 0

                            x00 AND y00 -> z00
                            x01 XOR y01 -> z01
                            x02 OR y02 -> z02
                            """,
                            4L
                        },
            new object[]
                        {
                            """
                            x00: 1
                            x01: 0
                            x02: 1
                            x03: 1
                            x04: 0
                            y00: 1
                            y01: 1
                            y02: 1
                            y03: 1
                            y04: 1

                            ntg XOR fgs -> mjb
                            y02 OR x01 -> tnw
                            kwq OR kpj -> z05
                            x00 OR x03 -> fst
                            tgd XOR rvg -> z01
                            vdt OR tnw -> bfw
                            bfw AND frj -> z10
                            ffh OR nrd -> bqk
                            y00 AND y03 -> djm
                            y03 OR y00 -> psh
                            bqk OR frj -> z08
                            tnw OR fst -> frj
                            gnj AND tgd -> z11
                            bfw XOR mjb -> z00
                            x03 OR x00 -> vdt
                            gnj AND wpb -> z02
                            x04 AND y00 -> kjc
                            djm OR pbm -> qhw
                            nrd AND vdt -> hwm
                            kjc AND fst -> rvg
                            y04 OR y02 -> fgs
                            y01 AND x02 -> pbm
                            ntg OR kjc -> kwq
                            psh XOR fgs -> tgd
                            qhw XOR tgd -> z09
                            pbm OR djm -> kpj
                            x03 XOR y03 -> ffh
                            x00 XOR y04 -> ntg
                            bfw OR bqk -> z06
                            nrd XOR fgs -> wpb
                            frj XOR qhw -> z04
                            bqk OR frj -> z07
                            y03 OR x01 -> nrd
                            hwm AND bqk -> z03
                            tgd XOR rvg -> z12
                            tnw OR pbm -> gnj
                            """,
                            2024L
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day24.txt"),
                            60714423975686L
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, string expected)
    {
        var data = new Data(input);
        var xors = data.Rules.Values.Where(x => x.Op == "XOR").ToArray();
        var ors = data.Rules.Values.Where(x => x.Op == "OR").ToArray();
        var ands = data.Rules.Values.Where(x => x.Op == "AND").ToArray();
        var badRules = new HashSet<string>();

        foreach(var or in ors) {
            if (or.Out[0] == 'z' && or.Out != "z45") {
                badRules.Add(or.Out);
            }
        }

        foreach(var and in ands) {
            if (and.Out[0] == 'z') {
                badRules.Add(and.Out);
            } else {
                if (and.Left[0] == 'x' && and.Left != "x00") {
                    if (ors.All(x => x.Left != and.Out && x.Right != and.Out)) {
                        badRules.Add(and.Out);
                    }
                }
            }
        }
        foreach(var xor in xors) {
            if (xor.Left[0] != 'x') {
                if (xor.Out[0] != 'z' || xor.Out == "z45") {
                    badRules.Add(xor.Out);
                }
            } else {
                if (xor.Left == "x00") {
                    if (xor.Out != "z00") {
                        badRules.Add(xor.Out);
                    }
                } else {
                    if (xors.All(x => x.Left != xor.Out && x.Right != xor.Out) || ands.All(a => a.Left != xor.Out && a.Right != xor.Out)) {
                        badRules.Add(xor.Out);
                    }
                }
            }
        }
        var ret = string.Join(",", badRules!.OrderBy(x => x));
        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
            new object[]
                        {
                            File.ReadAllText("Inputs/Day24.txt"),
                            "cgh,frt,pmd,sps,tst,z05,z11,z23"
                        },
    };
}

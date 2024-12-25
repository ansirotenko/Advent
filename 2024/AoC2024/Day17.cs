using Xunit;
using FluentAssertions;
using System.Text.RegularExpressions;

namespace AoC2024;

public class Day17
{
    record State(long A, long B, long C, long I);
    record Deubgger(State State, int[] Program);

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, string expected)
    {
        var regexp = new Regex("Register A: ([-|0-9]+)\\W+Register B: ([-|0-9]+)\\W+Register C: ([-|0-9]+)\\W+Program: ([0-9|,]+)");
        var match = regexp.Match(input);
        var debugger = new Deubgger(
            new State(
                long.Parse(match.Groups[1].Value),
                long.Parse(match.Groups[2].Value),
                long.Parse(match.Groups[3].Value),
                0
            ),
            match.Groups[4].Value.Split(new []{","}, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray()
        );
        var ret = new List<long>();
        var i = debugger.State.I;
        var a = debugger.State.A;
        var b = debugger.State.B;
        var c = debugger.State.C;
        while(i < debugger.Program.Length) {
            switch(debugger.Program[i]) {
                case 0: 
                    a = Dv(debugger.Program[i+1]);
                    i = i + 2;
                    break;
                case 1:
                    b = b ^ debugger.Program[i+1];
                    i = i + 2;
                    break;
                case 2:
                    b = ComboOperand(debugger.Program[i+1]) % 8;
                    i = i + 2;
                    break;
                case 3:
                    if (a == 0) {
                        i = i + 2;
                    } else {
                        i = debugger.Program[i+1];
                    }
                    break;
                case 4:
                    b = b ^ c;
                    i = i + 2;
                    break;
                case 5:
                    ret.Add(ComboOperand(debugger.Program[i+1]) % 8);
                    i = i + 2;
                    break;
                case 6:
                    b = Dv(debugger.Program[i+1]);
                    i = i + 2;
                    break;
                case 7:
                    c = Dv(debugger.Program[i+1]);
                    i = i + 2;
                    break;
            }
        }
        
        long ComboOperand(int operand) {
            return operand switch {
                4 => a,
                5 => b,
                6 => c,
                7 => throw new ArgumentException(),
                _ => operand
            };
        }
        long Dv(int operand) {
            var comboOperand = ComboOperand(operand);
            if (comboOperand > 32) {
                return 0;
            }
            return a / (1 << (int)comboOperand);
        }

        string.Join(",", ret).Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            Register A: 729
                            Register B: 0
                            Register C: 0

                            Program: 0,1,5,4,3,0
                            """,
                            "4,6,3,5,6,3,5,2,1,0"
                        },
            new object[]
                        {
                            """
                            Register A: 117440
                            Register B: 0
                            Register C: 0

                            Program: 0,3,5,4,3,0
                            """,
                            "0,3,5,4,3,0"
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day17.txt"),
                            "7,4,2,5,1,4,6,0,4"
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var regexp = new Regex("Register A: ([-|0-9]+)\\W+Register B: ([-|0-9]+)\\W+Register C: ([-|0-9]+)\\W+Program: ([0-9|,]+)");
        var match = regexp.Match(input);
        var debugger = new Deubgger(
            new State(
                int.Parse(match.Groups[1].Value),
                int.Parse(match.Groups[2].Value),
                int.Parse(match.Groups[3].Value),
                0
            ),
            match.Groups[4].Value.Split(new []{","}, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray()
        );

        var ret = Find(0, debugger.Program.Length - 1);
        if (ret == -1) {
            ret = Find(1, debugger.Program.Length - 1);
        }

        ret.Should().Be(expected);

        long Find(long lastAns, int n) {
            if (n == -1) {
                return lastAns;
            }
            long a;
            long b = debugger.State.B;
            long c = debugger.State.C;
            for(int t = 0; t < 8; t++) {
                a = (lastAns << 3) + t;
                var i = 0;
                var hasOut = false;
                while (!hasOut) {
                    switch(debugger.Program[i]) {
                        case 0: 
                            a = a >> (int)ComboOperand(debugger.Program[i+1]);
                            i = i + 2;
                            break;
                        case 1:
                            b = b ^ debugger.Program[i+1];
                            i = i + 2;
                            break;
                        case 2:
                            b = ComboOperand(debugger.Program[i+1]) % 8;
                            i = i + 2;
                            break;
                        case 3:
                            if (a == 0) {
                                i = i + 2;
                            } else {
                                i = debugger.Program[i+1];
                            }
                            break;
                        case 4:
                            b = b ^ c;
                            i = i + 2;
                            break;
                        case 5:
                            hasOut = true;
                            if (ComboOperand(debugger.Program[i+1]) % 8 == debugger.Program[n]) {
                                var nextAns = Find(a, n -1);
                                if (nextAns != -1) {
                                    return nextAns;
                                }
                            }
                            i = i + 2;
                            break;
                        case 6:
                            b = a >> (int)ComboOperand(debugger.Program[i+1]);
                            i = i + 2;
                            break;
                        case 7:
                            c = a >> (int)ComboOperand(debugger.Program[i+1]);
                            i = i + 2;
                            break;
                    }
                }
            }
            return -1;
            
            long ComboOperand(int operand) {
                return operand switch {
                    4 => a,
                    5 => b,
                    6 => c,
                    7 => throw new ArgumentException(),
                    _ => operand
                };
            }
        }
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
            // new object[]
            //             {
            //                 """
            //                 Register A: 2024
            //                 Register B: 0
            //                 Register C: 0

            //                 Program: 0,3,5,4,3,0
            //                 """,
            //                 117440L
            //             },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day17.txt"),
                            164278764924605L
                        },
    };
}

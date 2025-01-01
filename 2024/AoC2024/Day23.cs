using Xunit;
using FluentAssertions;
using System.Text.RegularExpressions;

namespace AoC2024;

public class Day23
{
    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int expected)
    {
        var map = new Dictionary<string, List<string>>();
        var regex = new Regex("([a-z]{2})-([a-z]{2})");
        foreach(Match match in regex.Matches(input)) {
            var pc1 = match.Groups[1].Value;
            var pc2 = match.Groups[2].Value;
            if (!map.ContainsKey(pc1)) {
                map[pc1] = new List<string>();
            }
            map[pc1].Add(pc2);
            if (!map.ContainsKey(pc2)) {
                map[pc2] = new List<string>();
            }
            map[pc2].Add(pc1);
        }
        var result = new List<List<string>>();
        var stack = new Stack<string>();
        Search(stack, map.Keys.ToList(), result);

        void Search(Stack<string> trace, List<string> net, List<List<string>> result) {
            if (trace.Count == 3) {
                if (net.Contains(trace.Last())) {
                    result.Add(trace.ToList());
                }
            } else {
                var tmp = new Dictionary<string, List<string>>();
                foreach(var subPc in net) {
                    if (map.Remove(subPc, out var subNet)) {
                        tmp[subPc] = subNet;
                        trace.Push(subPc);
                        Search(trace, subNet, result);
                        trace.Pop(); 
                    }
                }
                foreach(var t in tmp) {
                    map[t.Key] = t.Value;
                }
            }
        }

        var ret = result.Count(x => x.Any(y => y[0] == 't'));
        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            kh-tc
                            qp-kh
                            de-cg
                            ka-co
                            yn-aq
                            qp-ub
                            cg-tb
                            vc-aq
                            tb-ka
                            wh-tc
                            yn-cg
                            kh-ub
                            ta-co
                            de-co
                            tc-td
                            tb-wq
                            wh-td
                            ta-ka
                            td-qp
                            aq-cg
                            wq-ub
                            ub-vc
                            de-ta
                            wq-aq
                            wq-vc
                            wh-yn
                            ka-de
                            kh-ta
                            co-tc
                            wh-qp
                            tb-vc
                            td-yn
                            """,
                            7
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day23.txt"),
                            1358
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, string expected)
    {
        var map = new Dictionary<string, List<string>>();
        var regex = new Regex("([a-z]{2})-([a-z]{2})");
        foreach(Match match in regex.Matches(input)) {
            var pc1 = match.Groups[1].Value;
            var pc2 = match.Groups[2].Value;
            if (!map.ContainsKey(pc1)) {
                map[pc1] = new List<string>();
            }
            map[pc1].Add(pc2);
            if (!map.ContainsKey(pc2)) {
                map[pc2] = new List<string>();
            }
            map[pc2].Add(pc1);
        }
        var trace = new Stack<string>();
        var bestCluster = Search(trace, map.Keys.ToArray());

        List<string> Search(Stack<string> trace, string[] net) {
            if (!net.Any()) {
                return trace.ToList();
            } else {
                var tmp = new Dictionary<string, List<string>>();
                var bestCluster = new List<string>();
                foreach(var subPc in net) {
                    if (map.Remove(subPc, out var subNet)) {
                        tmp[subPc] = subNet;
                        trace.Push(subPc);
                        var cluster = Search(trace, subNet.Intersect(net).ToArray());
                        if (cluster.Count > bestCluster.Count) {
                            bestCluster = cluster;
                        }
                        trace.Pop(); 
                    }
                }
                foreach(var t in tmp) {
                    map[t.Key] = t.Value;
                }

                return bestCluster;
            }
        }

        var ret = string.Join(",", bestCluster.Order());
        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            kh-tc
                            qp-kh
                            de-cg
                            ka-co
                            yn-aq
                            qp-ub
                            cg-tb
                            vc-aq
                            tb-ka
                            wh-tc
                            yn-cg
                            kh-ub
                            ta-co
                            de-co
                            tc-td
                            tb-wq
                            wh-td
                            ta-ka
                            td-qp
                            aq-cg
                            wq-ub
                            ub-vc
                            de-ta
                            wq-aq
                            wq-vc
                            wh-yn
                            ka-de
                            kh-ta
                            co-tc
                            wh-qp
                            tb-vc
                            td-yn
                            """,
                            "co,de,ka,ta"
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day23.txt"),
                            "cl,ei,fd,hc,ib,kq,kv,ky,rv,vf,wk,yx,zf"
                        },
    };
}

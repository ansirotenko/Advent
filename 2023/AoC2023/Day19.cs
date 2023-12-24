using Xunit;
using FluentAssertions;
using System.Collections.Immutable;
namespace AoC2023;

public class Day19
{
    private Data Parse(string input)
    {
        var topSplit = input.Replace("\r", "").Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

        var rules = topSplit[0]
                    .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .ToDictionary(
                        row => row.Substring(0, row.IndexOf('{')),
                        row =>
                        {
                            return row.Substring(row.IndexOf('{') + 1, row.Length - row.IndexOf('{') - 2)
                                .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(x =>
                                {
                                    var split = x.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                                    if (split.Length == 1)
                                    {
                                        return new Rule('_', '_', long.MinValue, x);
                                    }

                                    var conditionStr = split[0];
                                    return new Rule(conditionStr[0], conditionStr[1], long.Parse(conditionStr.Substring(2)), split[1]);
                                })
                                .ToArray();
                        });

        var parts = topSplit[1]
                    .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(row =>
                    {
                        return row.Substring(1, row.Length - 2)
                            .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                            .ToDictionary(x => x[0], x => long.Parse(x.Substring(2)));
                    })
                    .ToArray();

        return new Data(rules, parts);
    }

    record Data(Dictionary<string, Rule[]> Rules, Dictionary<char, long>[] Parts);

    record Rule(char Attribute, char Operator, long Value, string ForwardRule);

    record Range(long Start, long End);

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var data = Parse(input);

        var ret = 0L;
        foreach (var part in data.Parts)
        {
            var ruleName = "in";
            while (ruleName != "A" && ruleName != "R")
            {
                var rules = data.Rules[ruleName];
                foreach (var rule in rules)
                {
                    if (rule.Attribute == '_')
                    {
                        ruleName = rule.ForwardRule;
                        break;
                    }
                    var actualValue = part[rule.Attribute];
                    var isMatch = rule.Operator switch
                    {
                        '=' => actualValue == rule.Value,
                        '>' => actualValue > rule.Value,
                        '<' => actualValue < rule.Value,
                        _ => throw new ArgumentException($"Unexpected operator {rule.Operator}")
                    };

                    if (isMatch)
                    {
                        ruleName = rule.ForwardRule;
                        break;
                    }
                }
            }

            if (ruleName == "A")
            {
                ret += part.Values.Sum();
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
                            px{a<2006:qkq,m>2090:A,rfg}
                            pv{a>1716:R,A}
                            lnx{m>1548:A,A}
                            rfg{s<537:gd,x>2440:R,A}
                            qs{s>3448:A,lnx}
                            qkq{x<1416:A,crn}
                            crn{x>2662:A,R}
                            in{s<1351:px,qqz}
                            qqz{s>2770:qs,m<1801:hdj,R}
                            gd{a>3333:R,R}
                            hdj{m>838:A,pv}

                            {x=787,m=2655,a=1222,s=2876}
                            {x=1679,m=44,a=2067,s=496}
                            {x=2036,m=264,a=79,s=2244}
                            {x=2461,m=1339,a=466,s=291}
                            {x=2127,m=1623,a=2188,s=1013}
                            """,
                            19114
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day19.txt"),
                            532551
                        },
       };



    record Crawler(ImmutableDictionary<char, Range> Intevals, string Rule);

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var data = Parse(input);
        var min = 1L;
        var max = 4000L;

        var st = new Stack<Crawler>();
        var dictionary = ImmutableDictionary<char, Range>.Empty
                            .SetItem('x', new Range(min, max))
                            .SetItem('m', new Range(min, max))
                            .SetItem('a', new Range(min, max))
                            .SetItem('s', new Range(min, max));
        st.Push(new Crawler(dictionary, "in"));

        var ret = 0L;
        while (st.Any())
        {
            var currentCrawler = st.Pop();
            if (currentCrawler.Rule == "A")
            {
                ret += currentCrawler.Intevals.Values.Aggregate(1L, (acc, curr) => acc * (curr.End - curr.Start + 1));
            }
            else
            {
                if (currentCrawler.Rule != "R")
                {
                    var continuation = new List<Crawler> { currentCrawler };
                    foreach (var rule in data.Rules[currentCrawler.Rule])
                    {
                        if (rule.Attribute == '_')
                        {
                            foreach (var c in continuation)
                            {
                                st.Push(c with { Rule = rule.ForwardRule });
                            }
                        }
                        else
                        {
                            var nextContinuation = new List<Crawler>();
                            switch (rule.Operator)
                            {
                                case '=':
                                    foreach (var c in continuation)
                                    {
                                        if (c.Intevals[rule.Attribute].Start >= rule.Value && c.Intevals[rule.Attribute].End <= rule.Value)
                                        {
                                            st.Push(c with
                                            {
                                                Rule = rule.ForwardRule,
                                                Intevals = c.Intevals.SetItem(rule.Attribute, new Range(rule.Value, rule.Value))
                                            });
                                        }

                                        if (c.Intevals[rule.Attribute].Start < rule.Value)
                                        {
                                            nextContinuation.Add(c with
                                            {
                                                Intevals = c.Intevals.SetItem(rule.Attribute, new Range(c.Intevals[rule.Attribute].Start, Math.Min(rule.Value - 1, c.Intevals[rule.Attribute].End)))
                                            });
                                        }

                                        if (c.Intevals[rule.Attribute].End > rule.Value)
                                        {
                                            nextContinuation.Add(c with
                                            {
                                                Intevals = c.Intevals.SetItem(rule.Attribute, new Range(Math.Max(rule.Value + 1, c.Intevals[rule.Attribute].Start), c.Intevals[rule.Attribute].End))
                                            });
                                        }
                                    }

                                    break;
                                case '>':
                                    foreach (var c in continuation)
                                    {
                                        if (c.Intevals[rule.Attribute].End > rule.Value)
                                        {
                                            st.Push(c with
                                            {
                                                Rule = rule.ForwardRule,
                                                Intevals = c.Intevals.SetItem(rule.Attribute, new Range(Math.Max(rule.Value + 1, c.Intevals[rule.Attribute].Start), c.Intevals[rule.Attribute].End))
                                            });
                                        }
                                        if (c.Intevals[rule.Attribute].Start <= rule.Value)
                                        {
                                            nextContinuation.Add(c with
                                            {
                                                Intevals = c.Intevals.SetItem(rule.Attribute, new Range(c.Intevals[rule.Attribute].Start, Math.Min(rule.Value, c.Intevals[rule.Attribute].End)))
                                            });
                                        }
                                    }

                                    break;
                                case '<':
                                    foreach (var c in continuation)
                                    {
                                        if (c.Intevals[rule.Attribute].Start < rule.Value)
                                        {
                                            st.Push(c with
                                            {
                                                Rule = rule.ForwardRule,
                                                Intevals = c.Intevals.SetItem(rule.Attribute, new Range(c.Intevals[rule.Attribute].Start, Math.Min(rule.Value - 1, c.Intevals[rule.Attribute].End)))
                                            });
                                        }
                                        if (c.Intevals[rule.Attribute].End >= rule.Value)
                                        {
                                            nextContinuation.Add(c with
                                            {
                                                Intevals = c.Intevals.SetItem(rule.Attribute, new Range(Math.Max(rule.Value, c.Intevals[rule.Attribute].Start), c.Intevals[rule.Attribute].End))
                                            });
                                        }
                                    }

                                    break;
                            }
                            continuation = nextContinuation;
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
                            px{a<2006:qkq,m>2090:A,rfg}
                            pv{a>1716:R,A}
                            lnx{m>1548:A,A}
                            rfg{s<537:gd,x>2440:R,A}
                            qs{s>3448:A,lnx}
                            qkq{x<1416:A,crn}
                            crn{x>2662:A,R}
                            in{s<1351:px,qqz}
                            qqz{s>2770:qs,m<1801:hdj,R}
                            gd{a>3333:R,R}
                            hdj{m>838:A,pv}

                            {x=787,m=2655,a=1222,s=2876}
                            {x=1679,m=44,a=2067,s=496}
                            {x=2036,m=264,a=79,s=2244}
                            {x=2461,m=1339,a=466,s=291}
                            {x=2127,m=1623,a=2188,s=1013}
                            """,
                            167409079868000
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day19.txt"),
                            134343280273968
                        },
       };
}

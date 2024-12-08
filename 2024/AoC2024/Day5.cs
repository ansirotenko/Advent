using Xunit;
using FluentAssertions;

namespace AoC2024;

public class Day5
{
    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int expected)
    {
        var data = Data.Parse(input);
        var ret = data.Book.Sum(pages =>
        {
            var visited = new HashSet<int>();
            foreach (var page in pages)
            {
                if (data.Rules.TryGetValue(page, out var order))
                {
                    foreach (var o in order)
                    {
                        if (visited.Contains(o))
                        {
                            return 0;
                        }
                    }
                }
                visited.Add(page);
            }
            return pages[pages.Count / 2];
        });

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            47|53
                            97|13
                            97|61
                            97|47
                            75|29
                            61|13
                            75|53
                            29|13
                            97|29
                            53|29
                            61|53
                            97|53
                            61|29
                            47|13
                            75|47
                            97|75
                            47|61
                            75|61
                            47|29
                            75|13
                            53|13

                            75,47,61,53,29
                            97,61,53,29,13
                            75,29,13
                            75,97,47,61,53
                            61,13,29
                            97,13,75,29,47
                            """,
                            143
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day5.txt"),
                            5275
                        },
    };

    record Data(Dictionary<int, List<int>> Rules, List<List<int>> Book)
    {
        public static Data Parse(string input)
        {
            var rows = input.Split(new[] { "\n\r", "\n" }, StringSplitOptions.TrimEntries);
            var rules = new Dictionary<int, List<int>>();
            var book = new List<List<int>>();

            bool isRules = true;
            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row))
                {
                    isRules = false;
                }
                else
                {
                    if (isRules)
                    {
                        var split = row.Split('|');
                        var left = int.Parse(split[0]);
                        var right = int.Parse(split[1]);
                        if (!rules.TryGetValue(left, out var order))
                        {
                            order = new List<int>();
                            rules[left] = order;
                        }
                        order.Add(right);
                    }
                    else
                    {
                        book.Add(row.Split(',').Select(int.Parse).ToList());
                    }
                }
            }

            return new Data(rules, book);
        }
    }

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, int expected)
    {
        var data = Data.Parse(input);
        var ret = data.Book.Sum(pages =>
        {
            var visited = new List<int>();
            foreach (var page in pages)
            {
                if (data.Rules.TryGetValue(page, out var order))
                {
                    foreach (var o in order)
                    {
                        if (visited.Contains(o))
                        {
                            Dictionary<int, HashSet<int>> fullRules = pages.ToDictionary(x => x, x => (HashSet<int>)null!);
                            foreach(var key in pages) {
                                Traverse(key, data, fullRules);
                            }

                            var sorted = Sort(pages, fullRules).ToList();
                            return sorted[sorted.Count / 2];
                        }
                    }
                }
                visited.Add(page);
            }
            return 0;
        });

        ret.Should().Be(expected);
    }

    private void Traverse(int visiting, Data data, Dictionary<int, HashSet<int>> fullRules) {
        if (fullRules[visiting] != null) {
            return;
        }

        if (!data.Rules.ContainsKey(visiting)) {
            fullRules[visiting] = new HashSet<int>();
            return;
        }

        fullRules[visiting] = data.Rules[visiting]
            .SelectMany(x => {
                if (!fullRules.ContainsKey(x)) {
                    return Enumerable.Empty<int>();
                }
                Traverse(x, data, fullRules);

                return new []{x}.Concat(fullRules[x]);
            })
            .ToHashSet();
    }

    private IEnumerable<int> Sort(List<int> pool, Dictionary<int, HashSet<int>> fullRules) {
        if (pool.Count() <= 1) {
            return pool;
        }
        var first = pool.First();
        pool.Remove(first);

        if (fullRules.TryGetValue(first, out var order)) {
            var right = pool.Intersect(order!).ToList();
            foreach(var n in right) {
                pool.Remove(n);
            }
            return Sort(pool, fullRules).Concat(new []{first}).Concat(Sort(right, fullRules));

        } else {
            return Sort(pool, fullRules).Concat(new []{first});
        }
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
                       new object[]
                        {
                            """
                            47|53
                            97|13
                            97|61
                            97|47
                            75|29
                            61|13
                            75|53
                            29|13
                            97|29
                            53|29
                            61|53
                            97|53
                            61|29
                            47|13
                            75|47
                            97|75
                            47|61
                            75|61
                            47|29
                            75|13
                            53|13

                            75,47,61,53,29
                            97,61,53,29,13
                            75,29,13
                            75,97,47,61,53
                            61,13,29
                            97,13,75,29,47
                            """,
                            123
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day5.txt"),
                            6191
                        },
    };
}

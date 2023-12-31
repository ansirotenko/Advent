using Xunit;
using FluentAssertions;
using System.Text;
using System.Reflection.Metadata;
namespace AoC2023;

public class Day25
{
    private Dictionary<string, HashSet<string>> Parse(string input)
    {
        var ret = new Dictionary<string, HashSet<string>>();
        foreach (var row in input.Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
        {
            var split = row.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            var name = split[0].Trim();
            var deps = split[1].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            if (!ret.ContainsKey(name))
            {
                ret[name] = new HashSet<string>();
            }

            foreach (var dep in deps)
            {
                if (!ret.ContainsKey(dep))
                {
                    ret[dep] = new HashSet<string>();
                }

                ret[name].Add(dep);
                ret[dep].Add(name);
            }
        }

        return ret;
    }

    // [Theory]
    // [MemberData(nameof(TestCases1))]
    // public void Part1(string input, long expected)
    // {
    //     var data = Parse(input);

    //     var toBeDeleted = new HashSet<(string, string)>();
    //     foreach (var (name, deps) in data)
    //     {
    //         var suspects = deps.Where(d =>
    //         {
    //             return !deps.Where(dd => dd != d)
    //                         .SelectMany(dd => data[dd].Concat(data[dd].Where(ddd => ddd != name).SelectMany(ddd => data[ddd])))
    //                         .Contains(d);
    //         }).ToArray();

    //         foreach (var susp in suspects)
    //         {
    //             if (name.CompareTo(susp) < 0)
    //             {
    //                 toBeDeleted.Add((name, susp));
    //             }
    //             else
    //             {
    //                 toBeDeleted.Add((susp, name));
    //             }
    //         }
    //     }

    //     var test = data.GroupBy(x => x.Value.Count).ToDictionary(x => x.Key, x => x.Select(y => y.Key).ToArray()).ToArray();
    //     var sb = new StringBuilder();

    //     for (int i = 0; i < test.Length; i++)
    //     {
    //         for (int j = i + 1; j < test.Length; j++)
    //         {
    //             var count = test[i].Value.SelectMany(d => data[d]).Intersect(test[j].Value).Count();
    //             sb.AppendLine($"{test[i].Key} - {test[j].Key} : {count}");
    //         }
    //     }

    //     var dbg = sb.ToString();

    //     var ret = -1L;

    //     ret.Should().Be(expected);
    // }

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var data = Parse(input);
        var allNodes = data.Keys.ToArray();
        var stringToNum = new Dictionary<string, int>();
        for (int i = 0; i < allNodes.Length; i++)
        {
            stringToNum.Add(allNodes[i], i);
        }
        var ribs = new Dictionary<(int, int), int>();
        for (int i = 0; i < allNodes.Length; i++)
        {
            for (int j = i + 1; j < allNodes.Length; j++)
            {
                ribs[(i, j)] = 0;
            }
        }

        var dataInt = data.ToDictionary(x => stringToNum[x.Key], x => x.Value.Select(y => stringToNum[y]).ToList());
        WalkEachToEach(dataInt, ribs);
        foreach (var (k, v) in dataInt)
        {
            v.Reverse();
        }
        WalkEachToEach(dataInt, ribs);

        var test = ribs.OrderByDescending(x => x.Value).ToArray();

        foreach (var rib in ribs.OrderByDescending(x => x.Value).Take(3).Select(x => x.Key))
        {
            dataInt[rib.Item1].Remove(rib.Item2);
            dataInt[rib.Item2].Remove(rib.Item1);
        }

        var visitedGroup1 = new HashSet<int> { 0 };
        var qGroup1 = new Queue<int>();
        qGroup1.Enqueue(0);
        var group1Count = 1;

        while (qGroup1.Any())
        {
            var index = qGroup1.Dequeue();

            foreach (var depIndex in dataInt[index])
            {
                if (visitedGroup1.Add(depIndex))
                {
                    group1Count++;
                    qGroup1.Enqueue(depIndex);
                }
            }
        }

        long ret = group1Count * (allNodes.Length - group1Count);

        ret.Should().Be(expected);
    }

    private void WalkEachToEach(Dictionary<int, List<int>> dataInt, Dictionary<(int, int), int> ribs)
    {
        foreach (var node in dataInt.Keys)
        {
            var visited = new HashSet<int> { node };
            var q = new Queue<int>();
            q.Enqueue(node);

            while (q.Any())
            {
                var index = q.Dequeue();

                foreach (var depIndex in dataInt[index])
                {
                    if (visited.Add(depIndex))
                    {
                        ribs[(Math.Min(index, depIndex), Math.Max(index, depIndex))]++;
                        q.Enqueue(depIndex);
                    }
                }
            }
        }
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            jqt: rhn xhk nvd
                            rsh: frs pzl lsr
                            xhk: hfx
                            cmg: qnr nvd lhk bvb
                            rhn: xhk bvb hfx
                            bvb: xhk hfx
                            pzl: lsr hfx nvd
                            qnr: nvd
                            ntq: jqt hfx bvb xhk
                            nvd: lhk
                            lsr: lhk
                            rzs: qnr cmg lsr rsh
                            frs: qnr lhk lsr
                            """,
                            54
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day25.txt"),
                            551196
                        },
       };
}

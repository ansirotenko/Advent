using Xunit;
using FluentAssertions;
namespace AoC2023;

public class Day23
{
    private char[][] Parse(string input)
    {
        return input.Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(r => r.ToArray())
                .ToArray();
    }

    record Node
    {
        public Node(int id)
        {
            Id = id;
            Next = new Dictionary<int, long>();
        }

        public int Id { get; init; }
        public Dictionary<int, long> Next { get; }
    }

    private (int Di, int Dj)[] indexDirections = { (0, -1), (-1, 0), (0, 1), (1, 0) };
    private Dictionary<char, (int Di, int Dj)> slide = new Dictionary<char, (int Di, int Dj)>
    {
        {'<', (0, -1)},
        {'>', (0, 1)},
        {'v', (1, 0)},
        {'^', (-1, 0)},
    };


    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var data = Parse(input);

        var visited = Enumerable.Range(0, data.Length)
                                               .Select(_ => Enumerable.Range(0, data[0].Length)
                                                               .Select(_ => false)
                                                               .ToArray())
                                               .ToArray();
        var j = data[0].TakeWhile(x => x == '#').Count();
        var ret = Solve1(0, j, data, visited);
        ret.Should().Be(expected);
    }

    private long Solve1(int i, int j, char[][] data, bool[][] visited)
    {
        if (i < 0 || i >= data.Length || j < 0 || j >= data[0].Length || data[i][j] == '#' || visited[i][j])
            return -1;

        if (i == data.Length - 1)
            return 0;

        visited[i][j] = true;

        var ret = -1L;
        if (slide.TryGetValue(data[i][j], out var d))
        {
            ret = Solve1(i + d.Di, j + d.Dj, data, visited);
        }
        else
        {

            foreach (var (di, dj) in indexDirections)
            {
                var ni = i + di;
                var nj = j + dj;
                ret = Math.Max(ret, Solve1(ni, nj, data, visited));
            }
        }

        visited[i][j] = false;

        if (ret < 0)
            return -1;

        return 1 + ret;
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            #.#####################
                            #.......#########...###
                            #######.#########.#.###
                            ###.....#.>.>.###.#.###
                            ###v#####.#v#.###.#.###
                            ###.>...#.#.#.....#...#
                            ###v###.#.#.#########.#
                            ###...#.#.#.......#...#
                            #####.#.#.#######.#.###
                            #.....#.#.#.......#...#
                            #.#####.#.#.#########v#
                            #.#...#...#...###...>.#
                            #.#.#v#######v###.###v#
                            #...#.>.#...>.>.#.###.#
                            #####v#.#.###v#.#.###.#
                            #.....#...#...#.#.#...#
                            #.#########.###.#.#.###
                            #...###...#...#...#.###
                            ###.###.#.###v#####v###
                            #...#...#.#.>.>.#.>.###
                            #.###.###.#.###.#.#v###
                            #.....###...###...#...#
                            #####################.#
                            """,
                            94
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day23.txt"),
                            2430
                        },
       };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var data = Parse(input);
        var n = data.Length;
        var m = data[0].Length;
        var start = data[0].TakeWhile(x => x == '#').Count();
        var end = (n - 1) * m + data[n - 1].TakeWhile(x => x == '#').Count();

        var nodes = GetNodes(data, start, end, n, m);

        var visited = new HashSet<int>();
        var ret = Solve2(start, end, nodes, visited);
        ret.Should().Be(expected);
    }

    private Dictionary<int, Node> GetNodes(char[][] data, int start, int end, int n, int m)
    {
        var ret = new Dictionary<int, Node>();

        var visited = Enumerable.Range(0, n)
                                       .Select(_ => Enumerable.Range(0, m)
                                                       .Select(_ => false)
                                                       .ToArray())
                                       .ToArray();

        ret[start] = new Node(start);
        ret[end] = new Node(end);
        visited[start / m][start % m] = true;

        var st = new Stack<int>();
        st.Push(start);

        while (st.Any())
        {
            var key = st.Pop();
            var node = ret[key];
            var i = key / m;
            var j = key % m;

            foreach (var (di, dj) in indexDirections)
            {
                var ni = i + di;
                var nj = j + dj;

                if (ni >= 0 && ni < data.Length && nj >= 0 && nj < data[0].Length && data[ni][nj] != '#' && !visited[ni][nj])
                {
                    int nextI = ni;
                    int nextJ = nj;
                    int length = -1;
                    int block = 2;

                    while (block == 2)
                    {
                        ni = nextI;
                        nj = nextJ;
                        length++;
                        block = 0;
                        foreach (var (ddi, ddj) in indexDirections)
                        {
                            var nni = ni + ddi;
                            var nnj = nj + ddj;

                            if (nni < 0 || nni >= data.Length || nnj < 0 || nnj >= data[0].Length || data[nni][nnj] == '#')
                            {
                                block++;
                            }
                            else
                            {
                                if (!visited[nni][nnj] || (ret.TryGetValue(nni * m + nnj, out var nnext) && nnext != node))
                                {
                                    nextI = nni;
                                    nextJ = nnj;
                                }
                            }
                        }

                        visited[ni][nj] = true;
                    }

                    var dbg1 = string.Join("\n", visited.Select(v => new string(v.Select(x => x ? '#' : ' ').ToArray())));

                    var nextKey = ni * m + nj;
                    if (!ret.TryGetValue(nextKey, out var nextNode))
                    {
                        ret[nextKey] = nextNode = new Node(nextKey);
                        st.Push(nextKey);
                    }

                    node.Next.TryGetValue(nextKey, out var oldLength);
                    node.Next[nextKey] = Math.Max(oldLength, length);
                    nextNode.Next[key] = node.Next[nextKey];
                }
            }
        }

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                if (data[i][j] == '#' && visited[i][j])
                {
                    Assert.Fail("wall was visited");
                }
                if (data[i][j] != '#' && !visited[i][j])
                {
                    Assert.Fail("empty space was not visited");
                }
            }
        }

        return ret;
    }

    private long Solve2(int key, int end, Dictionary<int, Node> nodes, HashSet<int> visisted)
    {
        if (key == end)
        {
            return 0;
        }

        visisted.Add(key);

        var ret = -1L;
        foreach (var next in nodes[key].Next)
        {
            if (!visisted.Contains(next.Key))
            {
                var solution = Solve2(next.Key, end, nodes, visisted);
                if (solution >= 0)
                {
                    ret = Math.Max(ret, next.Value + solution);
                }
            }
        }
        visisted.Remove(key);
        if (ret > 0)
            return ret + 1;

        return -1;
    }

    public static IEnumerable<object[]> TestCases2 =>
        new List<object[]>
        {
            new object[]
                        {
                            """
                            #.#####################
                            #.......#########...###
                            #######.#########.#.###
                            ###.....#.>.>.###.#.###
                            ###v#####.#v#.###.#.###
                            ###.>...#.#.#.....#...#
                            ###v###.#.#.#########.#
                            ###...#.#.#.......#...#
                            #####.#.#.#######.#.###
                            #.....#.#.#.......#...#
                            #.#####.#.#.#########v#
                            #.#...#...#...###...>.#
                            #.#.#v#######v###.###v#
                            #...#.>.#...>.>.#.###.#
                            #####v#.#.###v#.#.###.#
                            #.....#...#...#.#.#...#
                            #.#########.###.#.#.###
                            #...###...#...#...#.###
                            ###.###.#.###v#####v###
                            #...#...#.#.>.>.#.>.###
                            #.###.###.#.###.#.#v###
                            #.....###...###...#...#
                            #####################.#
                            """,
                            154
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day23.txt"),
                            6534
                        },
       };
}

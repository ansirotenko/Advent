using Xunit;
using FluentAssertions;

namespace AoC2024;

public class Day19
{
    class Node {
        public Dictionary<char, Node> Children { get; private set; }
        public bool HasStop { get; private set; }

        private Node() {
            Children = new Dictionary<char, Node>();
            HasStop = false;
        }

        public static Node Root() => new Node();

        public static void Insert(Node node, string input, int index) {
            if (index == input.Length) {
                node.HasStop = true;
            } else {
                if (!node.Children.TryGetValue(input[index], out var childNode)) {
                    childNode = new Node();
                    node.Children[input[index]] = childNode;
                }

                Insert(childNode, input, index + 1);
            }
        }
    }

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int expected)
    {
        var rows = input.Split(new []{"\r\n\r\n", "\n\n"}, StringSplitOptions.RemoveEmptyEntries);
        var towels = rows[0].Split(new []{", "}, StringSplitOptions.RemoveEmptyEntries);
        var root = Node.Root();
        foreach(var towel in towels) {
            Node.Insert(root, towel, 0);
        }
        var logos = rows[1].Split(new []{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);
        
        bool Find(Node node, string input, int index) {
            if (input.Length == index) {
                return node.HasStop;
            }

            if (node.HasStop && Find(root, input, index)) {
                return true;
            }

            return node.Children.TryGetValue(input[index], out var subNode) && Find(subNode, input, index + 1);
        }

        var ret = logos.Count(l => Find(root, l, 0));
        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            r, wr, b, g, bwu, rb, gb, br

                            brwrr
                            bggr
                            gbbr
                            rrbgbr
                            ubwu
                            bwurrg
                            brgr
                            bbrgwb
                            """,
                            6
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day19.txt"),
                            355
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var rows = input.Split(new []{"\r\n\r\n", "\n\n"}, StringSplitOptions.RemoveEmptyEntries);
        var towels = rows[0].Split(new []{", "}, StringSplitOptions.RemoveEmptyEntries);
        var root = Node.Root();
        foreach(var towel in towels) {
            Node.Insert(root, towel, 0);
        }
        var logos = rows[1].Split(new []{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);
        
        long Find(Node node, string input, int index, long[] cache) {
            if (input.Length == index) {
                return node.HasStop ? 1 : 0;
            }

            long ret = 0;
            if (node.HasStop) {
                if (cache[index] == -1) {
                    cache[index] = Find(root, input, index, cache);
                }
                ret += cache[index];
            }

            if (node.Children.TryGetValue(input[index], out var subNode)) {
                ret += Find(subNode, input, index + 1, cache);
            }
            return ret;
        }

        var ret = logos.Sum(l => {
            var cache = l.Select(x => -1L).ToArray();
            return Find(root, l, 0, cache);
        });
        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
             new object[]
                        {
                            """
                            r, wr, b, g, bwu, rb, gb, br

                            brwrr
                            bggr
                            gbbr
                            rrbgbr
                            ubwu
                            bwurrg
                            brgr
                            bbrgwb
                            """,
                            16L
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day19.txt"),
                            732978410442050L
                        },
    };
}

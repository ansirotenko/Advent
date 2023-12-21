using Xunit;
using FluentAssertions;
namespace AoC2023;

public class Day15
{
    private string[] Parse(string input)
    {
        return input.Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .SelectMany(row => row.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            .ToArray()
            ;
    }

    long GetHash(string str)
    {
        var ret = 0L;

        foreach (var c in str)
        {
            ret += (int)c;
            ret *= 17;
            ret = ret % 256;
        }

        return ret;
    }

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var data = Parse(input);

        var acc = data.Select(GetHash).ToArray();
        var ret = acc.Sum();

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            HASH
                            """,
                            52
                        },
            new object[]
                        {
                            """
                            rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7
                            """,
                            1320
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day15.txt"),
                            495972
                        },
       };

    record BoxItem
    {
        public BoxItem(string label, long focalLength)
        {
            Label = label;
            FocalLength = focalLength;
        }

        public string Label { get; set; }
        public long FocalLength { get; set; }
    }

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var data = Parse(input);

        var boxes = Enumerable.Range(0, 256).Select(i => new List<BoxItem>()).ToArray();

        foreach (var d in data)
        {
            var split = d.Split(new[] { "=", "-" }, StringSplitOptions.RemoveEmptyEntries);
            var label = split[0];
            var hash = GetHash(label);

            var existed = boxes[hash].FirstOrDefault(x => x.Label == label);
            if (split.Length == 1)
            {
                if (existed != null)
                {
                    boxes[hash].Remove(existed);
                }
            }
            else
            {
                var focalLength = long.Parse(split[1]);
                if (existed == null)
                {
                    boxes[hash].Add(new BoxItem(label, focalLength));
                }
                else
                {
                    existed.FocalLength = focalLength;
                }
            }
        }

        var ret = boxes.Select((b, i) =>
        {
            return (i + 1) * b.Select((v, j) => v.FocalLength * (j + 1)).Sum();
        }).Sum();

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
        new List<object[]>
        {
             new object[]
                        {
                            """
                            rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7
                            """,
                            145
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day15.txt"),
                            245223
                        },
       };
}

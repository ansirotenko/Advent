using Xunit;
using FluentAssertions;
using System.Runtime.InteropServices.JavaScript;

namespace AoC2024;

public class Day9
{
    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var ret = 0L;
        int iLeft = 0;
        int cLeft = 0;
        int iRight = input.Length - 1;
        if (iRight % 2 != 0) {
            iRight--;
        }
        int cRight = input[iRight] - '0';
        int i = 0;
        while(iLeft < iRight) {
            if (iLeft % 2 == 0) {
                cLeft = input[iLeft] - '0';
                for (int k = 0; k < cLeft; k++) {
                    AddNum(iLeft);
                }
                iLeft++;
                cLeft = input[iLeft] - '0';
            } else {
                if (cLeft > 0) {
                    if (cRight > 0) {
                        AddNum(iRight);
                        cLeft--;
                        cRight--;
                    } else {
                        iRight--;
                        iRight--;
                        cRight = input[iRight] - '0';
                    }
                } else {
                    iLeft++;
                }
            }
        }
        if (iRight == iLeft) {
            for (int k = 0; k < cRight; k++) {
                AddNum(iRight);
            }
        }

        void AddNum(int index) {
            ret += index / 2 * i++;
        }

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                {
                    "12345",
                    60L
                },
            new object[]
                        {
                            "2333133121414131402",
                            1928L
                        },
            new object[]
                        {
                            "90909",
                            513L
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day9.txt"),
                            6401092019345L
                        }
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var ret = 0L;
        int len = input.Length;
        if (len % 2 == 0) {
            len--;
        }
        var data = input.Substring(0, len).Select(x => x - '0').ToArray();
        var capacity = Enumerable.Range(0, (len - 1) / 2).Select(x => new List<int>()).ToArray();
        for (int disk = len - 1; disk > 0; disk -= 2) {
            for (int space = 1; space < disk; space += 2) {
                if (data[space] >= data[disk]) {
                    capacity[space / 2].Add(disk);
                    data[space] -= data[disk];
                    data[disk] = -1;
                    break;
                }
            }
        }
        int i = 0;
        for (int j = 0; j < len; j++) {
            if (j % 2 != 0) {
                foreach(var d in capacity[j / 2]) {
                    var diskSpace = input[d] - '0';
                    for (int k = 0; k < diskSpace; k++) {
                        ret += (d / 2) * i++;
                    }
                }
                i += data[j];
            } else {
                var diskSpace = input[j] - '0';
                if (data[j] == -1) {
                    i += diskSpace;
                } else {
                    for (int k = 0; k < diskSpace; k++) {
                        ret += (j / 2) * i++;
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
                            "90909",
                            513L
                        },
            new object[]
                        {
                            "2333133121414131402",
                            2858L
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day9.txt"),
                            6431472344710L
                        }
    };
}

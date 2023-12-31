using Xunit;
using FluentAssertions;

namespace AoC2023;

public class Day7
{
    record Data(string Hand, int Bid);
    private static Dictionary<char, int> cardRanks1 = new(){
        {'2', 0},
        {'3', 1},
        {'4', 2},
        {'5', 3},
        {'6', 4},
        {'7', 5},
        {'8', 6},
        {'9', 7},
        {'T', 8},
        {'J', 9},
        {'Q', 10},
        {'K', 11},
        {'A', 12},
    };

    private static Dictionary<char, int> cardRanks2 = new(){
        {'J', 0},
        {'2', 1},
        {'3', 2},
        {'4', 3},
        {'5', 4},
        {'6', 5},
        {'7', 6},
        {'8', 7},
        {'9', 8},
        {'T', 9},
        {'Q', 10},
        {'K', 11},
        {'A', 12},
    };
    private static Dictionary<int, int> comboRanks = new(){
        {5, 7},
        {41, 6},
        {32, 5},
        {311, 4},
        {221, 3},
        {2111, 2},
        {11111, 1}
    };

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int expected)
    {
        var data = input.Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(row =>
                        {
                            var s = row.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            return new Data(s[0], int.Parse(s[1]));
                        })
                        .ToArray();

        var ret = data
        .Select(x => (Data: x, Order: GetOrder(x.Hand)))
        .OrderByDescending(x => x.Order)
        .Select((x, i) => x.Data.Bid * (data.Length - i))
        .Sum();

        ret.Should().Be(expected);

        int GetOrder(string hand)
        {
            int combo = hand.GroupBy(x => x).Select(x => x.Count()).OrderByDescending(x => x).Aggregate(0, (acc, x) =>
            {
                return acc * 10 + x;
            });

            int cardsRank = hand.Aggregate(0, (acc, x) =>
            {
                return acc * 13 + cardRanks1[x];
            });

            return comboRanks[combo] * (int)Math.Pow(13, 5) + cardsRank;
        }
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            32T3K 765
                            T55J5 684
                            KK677 28
                            KTJJT 220
                            QQQJA 483
                            """,
                            6440
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day7.txt"),
                            247961593
                        },
       };


    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, int expected)
    {
        var data = input.Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(row =>
                        {
                            var s = row.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            return new Data(s[0], int.Parse(s[1]));
                        })
                        .ToArray();

        var ret = data
        .Select(x => (Data: x, Order: GetOrder(x.Hand)))
        .OrderByDescending(x => x.Order)
        .Select((x, i) => x.Data.Bid * (data.Length - i))
        .Sum();

        ret.Should().Be(expected);

        int GetOrder(string hand)
        {
            var jCount = hand.Count(x => x == 'J');
            var comboArr = hand.Where(x => x != 'J').GroupBy(x => x).Select(x => x.Count()).OrderByDescending(x => x).ToArray();
            if (comboArr.Any()){
                comboArr[0] = comboArr[0] + jCount;
            } else {
                comboArr = new []{5};
            }

            int combo = comboArr.Aggregate(0, (acc, x) =>
            {
                return acc * 10 + x;
            });

            int cardsRank = hand.Aggregate(0, (acc, x) =>
            {
                return acc * 13 + cardRanks2[x];
            });

            return comboRanks[combo] * (int)Math.Pow(13, 5) + cardsRank;
        }
    }

    public static IEnumerable<object[]> TestCases2 =>
        new List<object[]>
        {
            new object[]
                        {
                            """
                            32T3K 765
                            T55J5 684
                            KK677 28
                            KTJJT 220
                            QQQJA 483
                            """,
                            5905
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day7.txt"),
                            248750699
                        },
       };
}

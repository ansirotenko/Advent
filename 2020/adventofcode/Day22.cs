using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace adventofcode
{
    public class Day22
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Day22(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Part1()
        {
            var input = await ReadInput();
            var player1 = input[0];
            var player2 = input[1];
            GameId = 0;
            var ret = PlayGame(player1, player2, Part1WinCondition) ? GetScore(player1) : GetScore(player2);

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        private static bool Part1WinCondition(int p1Card, Queue<int> p1Deck, int p2Card, Queue<int> p2Deck)
        {
            return p1Card > p2Card;
        }

        [Fact]
        public async Task Part2()
        {
            var input = await ReadInput();
            var player1 = input[0];
            var player2 = input[1];
            GameId = 0;
            var ret = PlayGame(player1, player2, Part2WinCondition) ? GetScore(player1) : GetScore(player2);

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        private static int GameId = 0;

        private static bool Part2WinCondition(int p1Card, Queue<int> p1Deck, int p2Card, Queue<int> p2Deck)
        {
            if (p1Deck.Count < p1Card || p2Deck.Count < p2Card)
            {
                return p1Card > p2Card;
            }
            
            return PlayGame(new Queue<int>(p1Deck.Take(p1Card)), new Queue<int>(p2Deck.Take(p2Card)), Part2WinCondition);
        }

        private int GetScore(Queue<int> player)
        {
            return player.Reverse().Select((y, i) => y * (i + 1)).Sum();
        }

        private static bool PlayGame(Queue<int> p1Deck, Queue<int> p2Deck, Func<int, Queue<int>, int, Queue<int>, bool> winCondition)
        {
            GameId++;
            var gameId = GameId;
            var p1LogHistory = new HashSet<string>();
            var p2LogHistory = new HashSet<string>();
            var round = 0;
            while (true)
            {
                if (!p1Deck.Any())
                {
                    return false;
                }

                if (!p2Deck.Any())
                {
                    return true;
                }

                var p1Log = string.Join(",", p1Deck);
                var p2Log = string.Join(",", p2Deck);
                if (p1LogHistory.Contains(p1Log) || p2LogHistory.Contains(p2Log))
                {
                    return true;
                }

                p1LogHistory.Add(p1Log);
                p2LogHistory.Add(p2Log);


                var p1Card = p1Deck.Dequeue();
                var p2Card = p2Deck.Dequeue();

                if (winCondition(p1Card, p1Deck, p2Card, p2Deck))
                {
                    p1Deck.Enqueue(p1Card);
                    p1Deck.Enqueue(p2Card);
                }
                else
                {
                    p2Deck.Enqueue(p2Card);
                    p2Deck.Enqueue(p1Card);
                }

                round++;
            }
        }

        private static async Task<IList<Queue<int>>> ReadInput()
        {
            using var stream = File.OpenText("./Files/day22_input.txt");
            var fileText = await stream.ReadToEndAsync();
            return fileText
                .Split("\n\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => new Queue<int>(x
                    .Split("\n", StringSplitOptions.RemoveEmptyEntries)
                    .Skip(1)
                    .Select(int.Parse))
                )
                .ToArray();
        }
    }
}

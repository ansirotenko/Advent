using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace adventofcode
{
    public class Day23
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Day23(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Part1()
        {
            var input = await ReadInput();
            var cache = GetCache(input);

            for (int i = 0; i < 100; i++)
            {
                input = Iterate(input, cache, 9);
            }

            input = cache[1];
            var ret = "";
            while (input.Next != input)
            {
                ret += input.PopNext();
            }

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        [Fact]
        public async Task Part2()
        {
            var input = await ReadInput();
            var last = input;
            while (last.Next != input)
            {
                last = last.Next;
            }
            var startMax = 9;
            for (int i = 0; i < 1000000 - startMax; i++)
            {
                last = last.PushNext(i + 1 + startMax);
            }

            var cache = GetCache(input);

            for (int i = 0; i < 10000000; i++)
            {
                input = Iterate(input, cache, 1000000);
            }

            input = cache[1];
            var ret = (long)input.Next.Value * input.Next.Next.Value;
            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        private static Dictionary<int, CircleGraph> GetCache(CircleGraph input)
        {
            var ret = new Dictionary<int, CircleGraph>();
            var current = input;
            while (current.Next != input)
            {
                ret[current.Value] = current;
                current = current.Next;
            }

            ret[current.Value] = current;

            return ret;
        }

        private static CircleGraph Iterate(CircleGraph input, Dictionary<int, CircleGraph> cache, int max)
        {
            var destinationValue = input.Value - 1;
            var removed = Enumerable.Range(0, 3).Select(x => input.PopNext()).ToList();
            CircleGraph destination;

            while (true)
            {
                if (destinationValue <= 0)
                {
                    destinationValue = max;
                }

                if (removed.Contains(destinationValue))
                {
                    destinationValue--;
                }
                else
                {
                    destination = cache[destinationValue];
                    break;
                }
            }

            removed.Aggregate(destination, (x, y) => x.PushNextCached(y, cache));

            return input.Next;
        }

        class CircleGraph
        {
            public CircleGraph Next { get; private set; }
            public int Value { get; private set; }

            public CircleGraph(CircleGraph next, int value)
            {
                Next = next;
                Value = value;
            }

            public int PopNext()
            {
                if (Next == this)
                {
                    throw new ArgumentException("Too narrow graph");
                }

                var value = Next.Value;
                Next = Next.Next;

                return value;
            }

            public CircleGraph PushNext(int value)
            {
                Next = new CircleGraph(Next, value);
                return Next;
            }

            public CircleGraph PushNextCached(int value, Dictionary<int, CircleGraph> cache)
            {
                var newNext = cache[value];
                newNext.Next = Next;
                Next = newNext;
                return newNext;
            }

            public void Close(CircleGraph graph)
            {
                Next = graph;
            }
        }

        private static async Task<CircleGraph> ReadInput()
        {
            using var stream = File.OpenText("./Files/day23_input.txt");
            var fileText = await stream.ReadToEndAsync();
            var numbers = fileText.Select(x => x - '0').ToList();
            var ret = new CircleGraph(null, numbers.First());
            var last = numbers.Skip(1).Aggregate(ret, (c, n) => c.PushNext(n));
            last.Close(ret);
            return ret;
        }
    }
}

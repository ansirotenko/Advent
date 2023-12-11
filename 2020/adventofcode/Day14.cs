using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace adventofcode
{
    public class Day14
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Day14(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Part1()
        {
            var input = await ReadInput();

            var memory = new Dictionary<long, long>();
            string mask = null;
            var index = 0;
            while (index < input.Count)
            {
                var newMask = TryGetMask(input[index]);
                if (!string.IsNullOrEmpty(newMask))
                {
                    mask = newMask;
                }
                else
                {
                    UpdateMemory(input[index], mask, memory, DecoderV1);
                }

                index++;
            }

            _testOutputHelper.WriteLine($"Result is : {memory.Values.Sum()}");
        }


        [Fact]
        public async Task Part2()
        {
            var input = await ReadInput();

            var memory = new Dictionary<long, long>();
            string mask = null;
            var index = 0;
            while (index < input.Count)
            {
                var newMask = TryGetMask(input[index]);
                if (!string.IsNullOrEmpty(newMask))
                {
                    mask = newMask;
                }
                else
                {
                    UpdateMemory(input[index], mask, memory, DecoderV2);
                }

                index++;
            }

            _testOutputHelper.WriteLine($"Result is : {memory.Values.Sum()}");
        }

        private static readonly Regex MaskRegex = new Regex("mask\\s=\\s(.+)$");
        private static string TryGetMask(string value)
        {
            return MaskRegex.IsMatch(value) ? MaskRegex.Match(value).Groups[1].Value : null;
        }

        private static readonly Regex MemoryRegex = new Regex("mem\\[(\\d+)\\]\\s=\\s(\\d+)$");
        private static void UpdateMemory(string value, string mask, Dictionary<long, long> memory, Action<long, long, string, Dictionary<long, long>> action)
        {
            if (string.IsNullOrEmpty(mask))
            {
                throw new ArgumentException("Empty mask");
            }

            var match = MemoryRegex.Match(value);
            var address = long.Parse(match.Groups[1].Value);
            var number = long.Parse(match.Groups[2].Value);

            action(address, number, mask, memory);
        }

        private static void DecoderV1(long address, long number, string mask, Dictionary<long, long> memory)
        {
            for (int i = 0; i < mask.Length; i++)
            {
                var symbol = mask[^(i + 1)];
                switch (symbol)
                {
                    case '0':
                        number = number & (long.MaxValue ^ (1L << i));
                        break;
                    case '1':
                        number = number | (1L << i);
                        break;
                }
            }

            memory[address] = number;
        }


        private static void DecoderV2(long address, long number, string mask, Dictionary<long, long> memory)
        {
            for (int i = 0; i < mask.Length; i++)
            {
                var symbol = mask[^(i + 1)];
                switch (symbol)
                {
                    case '1':
                        address = address | (1L << i);
                        break;
                }
            }

            var floatingPositions = mask.Reverse().Select((x, i) => x == 'X' ? i : -1).Where(x => x != -1).ToList();
            var floatIndex = (1 << floatingPositions.Count) - 1;
            while (floatIndex >= 0)
            {
                var newAddress = address;
                var tmp = floatIndex;

                foreach (var t in floatingPositions)
                {
                    newAddress = tmp % 2 == 0
                        ? newAddress & (long.MaxValue ^ (1L << t))
                        : newAddress | (1L << t);

                    tmp >>= 1;
                }
                memory[newAddress] = number;

                floatIndex--;
            }
        }

        //private static string NumberToString(long number, int length)
        //{
        //    var chars = new char[length];
        //    var index = 0;
        //    while (index < length)
        //    {
        //        chars[^(index + 1)] = number % 2 == 0 ? '0' : '1';
        //        index++;
        //    }

        //    return new string(chars);
        //}

        private static async Task<IList<string>> ReadInput()
        {
            using var stream = File.OpenText("./Files/day14_input.txt");
            var fileText = await stream.ReadToEndAsync();
            return fileText.Split("\n", StringSplitOptions.RemoveEmptyEntries).ToArray();
        }
    }
}

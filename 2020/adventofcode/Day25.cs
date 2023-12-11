using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace adventofcode
{
    public class Day25
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Day25(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Test()
        {
            Assert.Equal(5764801, Next(7, 8));
            Assert.Equal(17807724, Next(7, 11));
            Assert.Equal(14897079, Next(5764801, 11));
            Assert.Equal(14897079, Next(17807724, 8));

            Assert.Equal(11, GetLoopSize(5764801, 14897079));
            Assert.Equal(8, GetLoopSize(17807724, 14897079));
            Assert.Equal(11, GetLoopSize(7, 17807724));
            Assert.Equal(8, GetLoopSize(7, 5764801));
        }

        [Fact]
        public async Task Part1()
        {
            var input = await ReadInput();
            var cardLoopSize = GetLoopSize(7, input.Card);
            var doorLoopSize = GetLoopSize(7, input.Door);

            var cardKey = Next(input.Card, doorLoopSize);
            var doorKey = Next(input.Door, cardLoopSize);

            var ret = cardKey;

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        private static long Next(long subjectNumber, long loopSize)
        {
            var value = 1L;

            for (long i = 0; i < loopSize; i++)
            {
                value = (value * subjectNumber) % 20201227;
            }

            return value;
        }

        private static long GetLoopSize(long subjectNumber, long destination)
        {
            var ret = 0L;
            var value = 1L;
            while (destination != value)
            {
                ret++;
                value = (value * subjectNumber) % 20201227;
            }

            return ret;
        }

        private static async Task<(long Card, long Door)> ReadInput()
        {
            using var stream = File.OpenText("./Files/day25_input.txt");
            var fileText = await stream.ReadToEndAsync();
            var split = fileText.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            return (long.Parse(split[0]), long.Parse(split[1]));
        }
    }
}

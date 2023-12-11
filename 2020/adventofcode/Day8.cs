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
    public class Day8
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Day8(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Part1()
        {
            var input = await ReadInput();

            var ret = Execute(input, -1);

            _testOutputHelper.WriteLine($"Result is : {ret.accumulator}");
        }


        [Fact]
        public async Task Part2()
        {
            var input = await ReadInput();

            for (int i = 0; i < input.Count; i++)
            {
                var programLine = input[i];
                if (programLine.Command == JmpCommand || programLine.Command == NopCommand)
                {
                    var ret = Execute(input, i);
                    if (ret.index >= input.Count)
                    {
                        _testOutputHelper.WriteLine($"Result is : {ret.accumulator}");
                       break;
                    }
                }
            }

        }

        const string JmpCommand = "jmp";
        const string NopCommand = "nop";
        const string AccCommand = "acc";

        private static (int accumulator, int index) Execute(IList<DataModel> program, int indexToChange)
        {
            var accumulator = 0;
            var index = 0;
            var visitedIndexes = new List<int>();

            while (!visitedIndexes.Contains(index) && index < program.Count)
            {
                var programLine = program[index];
                visitedIndexes.Add(index);
                var originalCommand = programLine.Command;
                var modifiedCommand = index == indexToChange
                    ? originalCommand == NopCommand ? JmpCommand : NopCommand
                    : originalCommand;

                switch (modifiedCommand)
                {
                    case AccCommand:
                        accumulator += programLine.Value;
                        index++;
                        break;
                    case NopCommand:
                        index++;
                        break;
                    case JmpCommand:
                        index += programLine.Value;
                        break;
                    default:
                        throw new NotImplementedException($"command {originalCommand} is not implemented");
                }
            }

            return (accumulator, index);
        }

        private static async Task<IList<DataModel>> ReadInput()
        {
            using var stream = File.OpenText("./Files/day8_input.txt");
            var fileText = await stream.ReadToEndAsync();
            var regex = new Regex("(.+?)\\s((\\+|\\-)\\d+)$");
            return fileText.Split("\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(x =>
                {
                    var match = regex.Match(x);
                    return new DataModel {Command = match.Groups[1].Value, Value = int.Parse(match.Groups[2].Value)};
                })
                .ToList();
        }

        class DataModel
        {
            public string Command { get; set; }
            public int Value { get; set; }
        }
    }
}

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
    public class Day19
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Day19(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Part1()
        {
            var input = await ReadInput();
            var ret = input.Samples.Count(x => x.Length == GetMatchIndex(x, 0, x.Length, 0, input.Rules, 0));

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        [Fact]
        public async Task Part2()
        {
            var input = await ReadInput();
            input.Rules[8] = new Rule(new []{new []{42, 8}, new[] { 42 }});
            input.Rules[11] = new Rule(new []{new []{42, 11, 31}, new []{ 42, 31 }});

            var ret = input.Samples.Count(x => x.Length == GetMatchIndex(x, 0, x.Length, 0, input.Rules, 0));
            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        private int GetMatchIndex(string sample, int sampleIndex, int sampleLength, int ruleIndex, IList<Rule> rules, int depth)
        {
            if (sampleIndex >= sampleLength)
            {
                return -1;
            }

            var rule = rules[ruleIndex];
            if (rule.IsText)
            {
                var textIndex = 0;
                while (textIndex < rule.Text.Length)
                {
                    if (textIndex + sampleIndex >= sampleLength || sample[textIndex + sampleIndex] != rule.Text[textIndex])
                    {
                        return -1;
                    }
                    textIndex++;
                }

                return textIndex + sampleIndex;
            }

            var bestGuess = -1;
            foreach (var numbersGroup in rule.NumbersGroups)
            {
                int tempIndex;
                var loopMemory = new int[numbersGroup.Length];
                var index = 0;

                while (index < numbersGroup.Length)
                {
                    loopMemory[index] = sampleLength;
                    index++;
                }
                index = 0;

                while (true)
                {
                    var ret = GetMatchIndex(sample, index == 0 ? sampleIndex : loopMemory[index - 1] + 1, loopMemory[index], numbersGroup[index], rules, depth + 1);
                    if (ret == -1 || depth == 0 && index == numbersGroup.Length - 1 && ret < sampleLength)
                    {
                        if (index == 0)
                        {
                            tempIndex = -1;
                            break;
                        }

                        loopMemory[index] = sampleLength;
                        index--;
                    }
                    else
                    {
                        if (index == numbersGroup.Length - 1)
                        {
                            tempIndex = ret;
                            break;
                        }

                        loopMemory[index] = ret - 1;
                        index++;
                    }
                }

                bestGuess = bestGuess > tempIndex ? bestGuess : tempIndex;
            }

            return bestGuess;
        }

        private static readonly Regex AlphabeticalRegex = new Regex("([a-z]+)");

        private static async Task<InputModel> ReadInput()
        {
            using var stream = File.OpenText("./Files/day19_input.txt");
            var fileText = await stream.ReadToEndAsync();
            var indexOfSeparator = fileText.IndexOf("\n\n", StringComparison.Ordinal);
            var rules = fileText.Substring(0, indexOfSeparator)
                .Split("\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(x =>
                {
                    var colonIndex = x.IndexOf(":", StringComparison.Ordinal);

                    var index = int.Parse(x.Substring(0, colonIndex));
                    var ruleText = x.Substring(colonIndex + 1);
                    var alphabeticalMatch = AlphabeticalRegex.Match(ruleText);
                    var rule = alphabeticalMatch.Groups.Count > 1
                        ? new Rule(alphabeticalMatch.Groups[1].Value)
                        : new Rule(ruleText
                            .Split("|", StringSplitOptions.RemoveEmptyEntries)
                            .Select(y =>
                                y.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray())
                            .ToArray());

                    return (Index: index, Rule: rule);
                })
                .OrderBy(x => x.Index)
                .ToList();

            for (int i = 0; i < rules.Count; i++)
            {
                if (i != rules[i].Index)
                    rules.Insert(i, (i, new Rule(new int[0][])));
            }

            var samples = fileText.Substring(indexOfSeparator + 2)
                .Split("\n", StringSplitOptions.RemoveEmptyEntries);

            return new InputModel(rules.Select(x => x.Rule).ToArray(), samples);
        }

        class InputModel
        {
            public IList<Rule> Rules { get; }

            public IList<string> Samples { get; }

            public InputModel(IList<Rule> rules, IList<string> samples)
            {
                Rules = rules;
                Samples = samples;
            }
        }

        class Rule
        {
            public int[][] NumbersGroups { get; }
            public string Text { get; }
            public bool IsText { get; }

            public Rule(string text)
            {
                Text = text;
                IsText = true;
            }

            public Rule(int[][] numbersGroups)
            {
                NumbersGroups = numbersGroups;
                IsText = false;
            }

            public override string ToString()
            {
                return IsText ? $"\"{Text}\"" : string.Join(" | ", NumbersGroups.Select(x => string.Join(" ", x)));
            }
        }
    }
}

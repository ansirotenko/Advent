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
    public class Day4
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Day4(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Part1()
        {
            var input = await ReadInput();
            var ret = input.Count(x => x.Keys.Intersect(requiredValues.Keys).Count() == requiredValues.Count);

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        [Fact]
        public async Task Part2()
        {
            var input = await ReadInput();
            var ret = input.Count(x => requiredValues.All(y => x.TryGetValue(y.Key, out var value) && y.Value(value)));

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        private static Dictionary<string, Func<string, bool>> requiredValues = new Dictionary<string, Func<string, bool>> 
        {
            {
                //(Birth Year)
                "byr", (x) => x.Length == 4 && int.TryParse(x, out var year) && year >= 1920 && year <= 2002
            },
            {
                //(Issue Year)
                "iyr", (x) => x.Length == 4 && int.TryParse(x, out var year) && year >= 2010 && year <= 2020
            },
            {
                //(Expiration Year)
                "eyr", (x) => x.Length == 4 && int.TryParse(x, out var year) && year >= 2020 && year <= 2030
            },
            {
                //(Height)
                "hgt", (x) =>
                {
                    var regex = new Regex("^(\\d+)(cm|in)$");
                    var match = regex.Match(x);
                    if (match.Groups.Count < 3)
                        return false;

                    if (!int.TryParse(match.Groups[1].Value, out var number))
                        return false;

                    var unit = match.Groups[2].Value;

                    return unit switch
                    {
                        "cm" => number >= 150 && number <= 193,
                        "in" => number >= 59 && number <= 76,
                        _ => false
                    };
                }
            },
            {
                //(Hair Color)
                "hcl", (x) => new Regex("^#([0-9]|[a-f]){6}$").IsMatch(x)
            },
            {
                //(Eye Color)
                "ecl", (x) => eyeColors.Contains(x)
            },
            {
                //(Passport ID)
                "pid", (x) => new Regex("^[0-9]{9}$").IsMatch(x)
            }
        };

        private static string[] eyeColors = { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" };

        private static Dictionary<string, Func<string, bool>> optionalValues = new Dictionary<string, Func<string, bool>>{
            {
                //(Country ID)
                "cid", (x) => true
            }
        };

        private static async Task<IList<Dictionary<string, string>>> ReadInput()
        {
            using var stream = File.OpenText("./Files/day4_input.txt");
            var fileText = await stream.ReadToEndAsync();
            var regex = new Regex("(\\w*):([^:]*)(\\s|\\n|$)");
            return fileText.Split("\n\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => regex.Matches(x).ToDictionary(y => y.Groups[1].Value, y => y.Groups[2].Value))
                .ToArray();
        }
    }
}

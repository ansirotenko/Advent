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
    public class Day7
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Day7(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Part1()
        {
            var input = await ReadInput();

            var visitedParents = new List<string>();
            var ret = ParentsCount(input.First(x => x.Color == "shiny gold"), visitedParents);

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        private static int ParentsCount(DataModel data, IList<string> visitedParents)
        {
            var ret = 0;

            foreach (var parent in data.Parents)
            {
                if (!visitedParents.Contains(parent.Color))
                {
                    visitedParents.Add(parent.Color);

                    ret += 1 + ParentsCount(parent, visitedParents);
                }
            }

            return ret;
        }

        [Fact]
        public async Task Part2()
        {
            var input = await ReadInput();

            var ret = ChildrenCount(input.First(x => x.Color == "shiny gold"));

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }
        

        private static int ChildrenCount(DataModel data)
        {
            return data.Children.Sum(x => x.count + x.count * ChildrenCount(x.child));
        }

        private static async Task<IList<DataModel>> ReadInput()
        {
            using var stream = File.OpenText("./Files/day7_input.txt");
            var fileText = await stream.ReadToEndAsync();
            var lines = fileText.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            var parsed = new Dictionary<string, DataModel>();
            foreach (var line in lines)
            {
                Parse(line, parsed);
            }

            return parsed.Values.ToList();
        }

        private static void Parse(string value, Dictionary<string, DataModel> existed)
        {
            var topRegex = new Regex("(.+?)\\sbags\\scontain\\s(.+?)(\\.|$)");
            var topMatch = topRegex.Match(value);
            var color = topMatch.Groups[1].Value;
            var childrenValue = topMatch.Groups[2].Value;
            var childrenRegex = new Regex("(\\d+)\\s(.+?)\\sbags*(,\\s|$)");
            var children = childrenRegex.Matches(childrenValue)
                .ToDictionary(x => x.Groups[2].Value, x => int.Parse(x.Groups[1].Value));

            if (!existed.TryGetValue(color, out var parent))
            {
                parent = new DataModel { Color = color };
                existed.Add(color, parent);
            }

            foreach (var childRecord in children)
            {
                if (!existed.TryGetValue(childRecord.Key, out var child))
                {
                    child = new DataModel { Color = childRecord.Key };
                    existed.Add(childRecord.Key, child);
                }

                parent.Children.Add((childRecord.Value, child));
                child.Parents.Add(parent);
            }
        }

        class DataModel
        {
            public DataModel()
            {
                Parents = new List<DataModel>();
                Children = new List<(int count, DataModel child)>();
            }

            public string Color { get; set; }
            public IList<DataModel> Parents { get; set; }
            public IList<(int count, DataModel child)> Children { get; set; }
        }
    }
}

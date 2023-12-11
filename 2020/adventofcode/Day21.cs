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
    public class Day21
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Day21(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Part1()
        {
            var input = await ReadInput();
            
            Identify(input);
            
            var ret = input.SelectMany(x => x.Ingredients).Count();

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }


        [Fact]
        public async Task Part2()
        {
            var input = await ReadInput();

            var poisonDictionary = new Dictionary<string, string>();

            Identify(input, poisonDictionary);

            var ret = string.Join(",", poisonDictionary.OrderBy(x => x.Key).Select(x => x.Value));

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        private void Identify(IList<Food> input, Dictionary<string, string> poisonDictionary = null)
        {
            var index = 0;
            var allAllergens = input.SelectMany(x => x.Allergens).Distinct().ToList();
            while (true)
            {
                var allergen = allAllergens[index];

                var suspiciousIngredients = input
                    .Where(x => x.Allergens.Contains(allergen))
                    .Select(x => (IEnumerable<string>)x.Ingredients)
                    .Aggregate((x, y) => x.Intersect(y))
                    .ToList();

                if (suspiciousIngredients.Count == 1)
                {
                    var definitelyAllergyIngredient = suspiciousIngredients.First();
                    allAllergens.Remove(allergen);
                    foreach (var food in input)
                    {
                        food.Allergens.Remove(allergen);
                        food.Ingredients.Remove(definitelyAllergyIngredient);
                    }

                    if (poisonDictionary != null)
                    {
                        poisonDictionary.Add(allergen, definitelyAllergyIngredient);
                    }
                }

                if (!allAllergens.Any())
                {
                    break;
                }

                index = (index + 1) % allAllergens.Count;
            }
        }

        private static readonly Regex Regex = new Regex("(.+)\\s\\(contains\\s(.+)\\)");

        private static async Task<IList<Food>> ReadInput()
        {
            using var stream = File.OpenText("./Files/day21_input.txt");
            var fileText = await stream.ReadToEndAsync();
            return fileText
                .Split("\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(x =>
                {
                    var match = Regex.Match(x);
                    var ingredients = match.Groups[1].Value.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();
                    var allergens = match.Groups[2].Value.Split(", ", StringSplitOptions.RemoveEmptyEntries).ToList();
                    return new Food(ingredients, allergens);
                })
                .ToArray();
        }

        class Food
        {
            public List<string> Ingredients { get; }
            public List<string> Allergens { get; }

            public Food(List<string> ingredients, List<string> allergens)
            {
                Ingredients = ingredients;
                Allergens = allergens;
            }
        }
    }
}

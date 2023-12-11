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
    public class Day20
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Day20(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Part1()
        {
            var input = await ReadInput();
            var assembled = Assemble(input);
            var ret = (long)assembled.First().First().Id *
                      assembled.First().Last().Id *
                      assembled.Last().First().Id *
                      assembled.Last().Last().Id;

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        [Fact]
        public async Task Part2()
        {
            var input = await ReadInput();
            var assembled = Assemble(input);
            var combined = CombineAssembled(assembled);

            var test = new[]
                {
                    "                  # ",
                    "#    ##    ##    ###",
                    " #  #  #  #  #  #   "
                }
                .Select(x => x.Select(y => y == '#').ToArray())
                .ToArray();

            var rotationAttempts = 0;
            var flipAttempts = 0;
            var testMatches = new List<string>();
            while (true)
            {
                var matches = 0;
                for (int i = 0; i < combined.Matrix.Length - test.Length; i++)
                {
                    for (int j = 0; j < combined.Matrix[0].Length - test[0].Length; j++)
                    {
                        var isMatch = true;
                        var toBeChanged = new List<(int i, int j)>();
                        for (int k = 0; k < test.Length; k++)
                        {
                            for (int l = 0; l < test[0].Length; l++)
                            {
                                if (test[k][l])
                                {
                                    if (combined.Matrix[i + k][j + l] >= 0)
                                    {
                                        toBeChanged.Add((i + k, j + l));
                                    }
                                    else
                                    {
                                        isMatch = false;
                                        break;
                                    }
                                }
                            }
                            if (!isMatch)
                            {
                                break;
                            }
                        }

                        if (isMatch)
                        {
                            matches++;
                            foreach (var valueTuple in toBeChanged)
                            {
                                combined.Matrix[valueTuple.i][valueTuple.j]++;
                            }
                        }
                    }
                }

                if (matches > 0)
                {
                    testMatches.Add($"Rotations: {rotationAttempts}, flips: {flipAttempts}, matches: {matches}");
                }

                rotationAttempts++;
                if (rotationAttempts > 3)
                {
                    flipAttempts++;
                    if (flipAttempts > 2)
                    {
                        break;
                    }

                    combined = flipAttempts % 2 == 0 ? combined.FlipHorizontal() : combined.FlipVertical();
                    rotationAttempts = 0;
                }
                else
                {
                    combined = combined.Rotate();
                }
            }

            var ret = combined.Matrix.Sum(x => x.Count(y => y == 0));

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        private static ImageModel<T>[][] Assemble<T>(IList<ImageModel<T>> input)
        {
            var edgeBorders = input
                .SelectMany(x => x.Borders)
                .GroupBy(x => x)
                .Where(x => x.Count() == 1)
                .Select(x => x.Key)
                .ToArray();

            var corners = input
                .Where(x => x.Borders.Intersect(edgeBorders).Count() == 2)
                .ToList();
            input = input.ToList();

            var dimension = edgeBorders.Length / 4;

            var ret = new ImageModel<T>[dimension][];
            for (int i = 0; i < dimension; i++)
            {
                ret[i] = new ImageModel<T>[dimension];

                for (int j = 0; j < dimension; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        ret[i][j] = corners.First();
                        input.Remove(ret[i][j]);
                        while (!edgeBorders.Contains(ret[i][j].Borders[0]) || !edgeBorders.Contains(ret[i][j].Borders[1]))
                        {
                            ret[i][j] = ret[i][j].Rotate();
                        }
                    }
                    else
                    {
                        var topPiece = i == 0 ? null : ret[i - 1][j];
                        var topBorders = topPiece == null ? edgeBorders : topPiece.Borders;
                        var leftPiece = j == 0 ? null : ret[i][j - 1];
                        var leftBorders = leftPiece == null ? edgeBorders : (topPiece == null ? new []{leftPiece.Right} : leftPiece.Borders);

                        ret[i][j] = input.Single(x => x.Borders.Intersect(topBorders).Any() && x.Borders.Intersect(leftBorders).Any());
                        input.Remove(ret[i][j]);

                        var counter = 0;
                        var flipAttempts = 0;
                        while ((topPiece == null ? !edgeBorders.Contains(ret[i][j].Top) : !ret[i][j].Top.ExactEqual(topPiece.Bottom)) ||
                               (leftPiece == null ? !edgeBorders.Contains(ret[i][j].Left) : !ret[i][j].Left.ExactEqual(leftPiece.Right)))
                        {
                            if (counter > 3)
                            {
                                ret[i][j] = flipAttempts switch
                                {
                                    0 => ret[i][j].FlipHorizontal(),
                                    1 => ret[i][j].FlipVertical(),
                                    _ => throw new ArgumentException("Piece miss match")
                                };
                                flipAttempts++;
                                counter = 0;
                            }
                            else
                            {
                                ret[i][j] = ret[i][j].Rotate();
                                counter++;
                            }
                        }
                    }
                }
            }

            return ret;
        }

        private static ImageModel<int> CombineAssembled(ImageModel<bool>[][] assembled)
        {
            var singleDimension = assembled.First().First().Dimension;
            var rangeInside = Enumerable.Range(1, singleDimension - 2).ToArray();

            var ret = assembled.SelectMany((row, i) =>
            {
                return rangeInside.Select(ii =>
                {
                    return row.SelectMany((x, j) =>
                    {
                        return rangeInside.Select(jj => x.Matrix[ii][jj] ? 0 : -1);
                    })
                    .ToArray();
                });
            })
            .ToArray();

            return new ImageModel<int>(0, ret, x => x > -1);
        }

        private static readonly Regex IdRegex = new Regex("Tile (\\d+):");
        private static async Task<IList<ImageModel<bool>>> ReadInput()
        {
            using var stream = File.OpenText("./Files/day20_input.txt");
            var fileText = await stream.ReadToEndAsync();
            return fileText
                .Split("\n\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(x =>
                {
                    var splitByRows = x.Split("\n", StringSplitOptions.RemoveEmptyEntries);

                    var id = int.Parse(IdRegex.Match(splitByRows.First()).Groups[1].Value);
                    var matrix = splitByRows.Skip(1).Select(y => y.Select(z => z == '#').ToArray()).ToArray();

                    return new ImageModel<bool>(id, matrix, b => b);
                }).ToArray();
        }

        class ImageModel<T>
        {
            public int Id { get; }
            public T[][] Matrix { get; private set; }
            public Func<T, bool> IsPositive { get; }
            public int Dimension { get; private set; }
            public IList<BorderModel> Borders { get; private set; }
            public BorderModel Left => Borders[0];
            public BorderModel Top => Borders[1];
            public BorderModel Right => Borders[2];
            public BorderModel Bottom => Borders[3];

            public ImageModel(int id, T[][] matrix, Func<T, bool> isPositive)
            {
                Id = id;
                Matrix = matrix;
                IsPositive = isPositive;
                Dimension = matrix.Length;
                Borders = new []
                {
                    GetBorder(matrix.Select(x => x[0]).ToArray()),
                    GetBorder(matrix[0]),
                    GetBorder(matrix.Select(x => x[Dimension - 1]).ToArray()),
                    GetBorder(matrix[Dimension - 1].ToArray())
                };
            }

            private BorderModel GetBorder(T[] array)
            {
                return new BorderModel(NumberFromBytes(array), NumberFromBytes(array.Reverse().ToArray()));
            }

            public ImageModel<T> FlipHorizontal()
            {
                var matrix = Matrix.Select(x => x.Reverse().ToArray()).ToArray();
                return new ImageModel<T>(Id, matrix, IsPositive);
            }

            public ImageModel<T> FlipVertical()
            {
                var matrix = Matrix.Reverse().ToArray();
                return new ImageModel<T>(Id, matrix, IsPositive);
            }

            public ImageModel<T> Rotate()
            {
                var matrix = Enumerable.Range(0, Dimension).Reverse().Select(i => Matrix.Select(x => x[i]).ToArray()).ToArray();
                return new ImageModel<T>(Id, matrix, IsPositive);
            }

            public override string ToString()
            {
                var ret = new List<string>();
                ret.Add($"Tile {Id}:");
                foreach (var row in Matrix)
                {
                    ret.Add(new string(row.Select(x => IsPositive(x) ? '#' : '.').ToArray()));
                }
                ret.Add(string.Empty);
                ret.Add($"Left: {NumberToString(Left.First)}, {NumberToString(Left.Second)}");
                ret.Add($"Top: {NumberToString(Top.First)}, {NumberToString(Top.Second)}");
                ret.Add($"Right: {NumberToString(Right.First)}, {NumberToString(Right.Second)}");
                ret.Add($"Bottom: {NumberToString(Bottom.First)}, {NumberToString(Bottom.Second)}");

                return string.Join("\n", ret);
            }

            private string NumberToString(int number)
            {
                var ret = new char[Dimension];

                for (int i = 0; i < Dimension; i++)
                {
                    ret[^(i + 1)] = number % 2 == 0 ? '.' : '#';
                    number = number >> 1;
                }

                return new string(ret);
            }

            private int NumberFromBytes(T[] array)
            {
                var ret = 0;

                for (int i = 0; i < Dimension; i++)
                {
                    ret = (ret << 1) + (IsPositive(array[i]) ? 1 : 0);
                }

                return ret;
            }
        }
        
        class BorderModel
        {
            public int First { get; }
            public int Second { get; }

            public BorderModel(int first, int second)
            {
                First = first;
                Second = second;
            }

            public bool ExactEqual(BorderModel other)
            {
                return First == other.First && Second == other.Second;
            }

            public override bool Equals(object? obj)
            {
                return obj is BorderModel other && Equals(other);
            }

            protected bool Equals(BorderModel other)
            {
                return First == other.First && Second == other.Second || First == other.Second && Second == other.First;
            }

            public override int GetHashCode()
            {
                return First + Second;
            }

            public override string ToString()
            {
                return $"{First}, {Second}";
            }
        }
    }
}

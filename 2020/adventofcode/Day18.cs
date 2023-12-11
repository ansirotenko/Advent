using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace adventofcode
{
    public class Day18
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Day18(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Part1()
        {
            var input = await ReadInput();
            var operatorPrios = new Dictionary<MathOperator, int>
            {
                {MathOperator.Plus, 1},
                {MathOperator.Multiply, 1}
            };
            var ret = input.Select(x => new Parser(x, operatorPrios).Parse().Evaluate()).Sum();

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        [Fact]
        public async Task Part2()
        {
            var input = await ReadInput();
            var operatorPrios = new Dictionary<MathOperator, int>
            {
                {MathOperator.Plus, 2},
                {MathOperator.Multiply, 1}
            };
            var ret = input.Select(x => new Parser(x, operatorPrios).Parse().Evaluate()).Sum();

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        private static async Task<IList<string>> ReadInput()
        {
            using var stream = File.OpenText("./Files/day18_input.txt");
            var fileText = await stream.ReadToEndAsync();
            return fileText.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        }

        private static Dictionary<char, MathOperator> Operators = new Dictionary<char, MathOperator>
        {
            {'+', MathOperator.Plus},
            {'*', MathOperator.Multiply}
        };

        private static Dictionary<MathOperator, char> OperatorsReverse = new Dictionary<MathOperator, char>
        {
            {MathOperator.Plus, '+'},
            {MathOperator.Multiply, '*'}
        };

        class Parser
        {
            public string Value { get; }
            public Dictionary<MathOperator, int> Priorities { get; }
            public Parser(string value, Dictionary<MathOperator, int> priorities)
            {
                Value = value.Replace(" ", "");
                Priorities = priorities;
            }

            public MathExpression Parse()
            {
                return Parse(0, Value.Length);
            }

            private MathExpression Parse(int index, int untilIndex)
            {
                if (index >= untilIndex)
                {
                    throw new ArgumentException("Empty expressions are not allowed");
                }

                var binaryPriority = Priorities.Values.Min();
                TryReadNextBinaryOrUnary(index, untilIndex, binaryPriority, out var firstResult);

                TryReadBinary(firstResult.index, untilIndex, firstResult.Expression, binaryPriority, out var secondResult);

                return secondResult.Expression;
            }

            private bool TryReadNextBinaryOrUnary(int index, int untilIndex, int binaryPriority, out (MathExpression Expression, int index) ret)
            {
                if (TryReadUnary(index, untilIndex, out var unaryResult))
                {
                    index = unaryResult.index;
                }
                else
                {
                    throw new ArgumentException($"Unexpected character '{Value[index]}' at position {index}, expected unary expression");
                }

                if (index >= untilIndex)
                {
                    ret = (unaryResult.Expression, index);
                    return true;
                }

                if (!Operators.TryGetValue(Value[index], out var currentOperator))
                {
                    throw new ArgumentException($"Unexpected character '{Value[index]}' at position {index}, expected operator.");
                }

                if (binaryPriority < Priorities[currentOperator])
                {
                    return TryReadBinary(index, untilIndex, unaryResult.Expression, binaryPriority + 1, out ret);
                }

                ret = (unaryResult.Expression, index);
                return true;
            }

            private bool TryReadBinary(int index, int untilIndex, MathExpression left, int binaryPriority, out (MathExpression Expression, int index) ret)
            {
                if (index >= untilIndex)
                {
                    ret = (left, index);
                    return true;
                }

                if (!Operators.TryGetValue(Value[index], out var currentOperator))
                {
                    ret = default;
                    return false;
                }

                if (binaryPriority > Priorities[currentOperator])
                {
                    ret = (left, index);
                    return true;
                }

                index++;

                MathExpression currentExpression;
                if (TryReadNextBinaryOrUnary(index, untilIndex, binaryPriority, out var rightResult))
                {
                    index = rightResult.index;
                    currentExpression = new BinaryMathExpression(currentOperator, left, rightResult.Expression);
                }
                else
                {
                    throw new ArgumentException("Missing right part of the binary expression");
                }

                if (TryReadBinary(index, untilIndex, currentExpression, binaryPriority, out ret))
                {
                    return true;
                }

                ret = (currentExpression, index);
                return true;
            }

            private bool TryReadUnary(int index, int untilIndex, out (MathExpression Expression, int index) ret)
            {
                if (TryReadNextDigit(index, untilIndex, out var digitResult))
                {
                    ret = digitResult;
                    return true;
                }
                if (TryReadNextParentheses(index, untilIndex, out var parenthesesResult))
                {
                    ret = parenthesesResult;
                    return true;
                }

                ret = default;
                return false;
            }

            private bool TryReadNextParentheses(int index, int untilIndex, out (MathExpression Expression, int index) ret)
            {
                if (index >= untilIndex || Value[index] != '(')
                {
                    ret = default;
                    return false;
                }

                index++;
                var startIndex = index;
                var balance = -1;

                while (index < untilIndex && balance != 0)
                {
                    if (Value[index] == '(')
                    {
                        balance--;
                    }
                    else
                    {
                        if (Value[index] == ')')
                        {
                            balance++;
                        }
                    }
                    index++;
                }

                if (index >= untilIndex && balance != 0)
                {
                    throw new ArgumentException("Wrong balance of parentheses");
                }

                ret = (new ParenthesesMathExpression(Parse(startIndex, index - 1)), index);
                return true;
            }

            private bool TryReadNextDigit(int index, int untilIndex, out (MathExpression Expression, int index) ret)
            {
                var numberString = string.Empty;
                while (index < untilIndex && Value[index] >= '0' && Value[index] <= '9')
                {
                    numberString = numberString + Value[index];
                    index++;
                }

                if (string.IsNullOrEmpty(numberString))
                {
                    ret = default;
                    return false;
                }

                ret = (new DigitMathExpression(int.Parse(numberString)), index);
                return true;
            }
        }

        abstract class MathExpression
        {
            public abstract long Evaluate();
        }

        abstract class UnaryMathExpression : MathExpression
        {
        }

        class ParenthesesMathExpression: UnaryMathExpression
        {
            public MathExpression Expression { get; }
            public ParenthesesMathExpression(MathExpression expression)
            {
                Expression = expression;
            }

            public override long Evaluate()
            {
                return Expression.Evaluate();
            }

            public override string ToString()
            {
                return $"( {Expression} )";
            }
        }

        class DigitMathExpression : UnaryMathExpression
        {
            public long Number { get; }

            public DigitMathExpression(long number)
            {
                Number = number;
            }

            public override long Evaluate()
            {
                return Number;
            }

            public override string ToString()
            {
                return Number.ToString();
            }
        }

        class BinaryMathExpression: MathExpression
        {
            public MathOperator Operator { get; }
            public MathExpression Left { get; }
            public MathExpression Right { get; }

            public BinaryMathExpression(MathOperator op, MathExpression left, MathExpression right)
            {
                Operator = op;
                Left = left;
                Right = right;
            }

            public override long Evaluate()
            {
                if (Operator == MathOperator.Multiply)
                {
                    return Left.Evaluate() * Right.Evaluate();
                }

                if (Operator == MathOperator.Plus)
                {
                    return Left.Evaluate() + Right.Evaluate();
                }

                throw new ArgumentException("Not supported operator");
            }

            public override string ToString()
            {
                return $"[{Left}] {OperatorsReverse[Operator]} [{Right}]";
            }
        }
        enum MathOperator
        {
            Plus, Multiply
        }

    }
}

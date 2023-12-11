using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace adventofcode
{
    public class Day12
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Day12(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Part1()
        {
            var input = await ReadInput();
            var currentState = new CurrentState(0, 0, Direction.East);
            foreach (var instruction in input)
            {
                currentState = Instructions[instruction.Direction](currentState, instruction.Value);
            }

            var ret = Math.Abs(currentState.X) + Math.Abs(currentState.Y);

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        [Fact]
        public async Task Part2()
        {
            var input = await ReadInput();
            var currentState = new CurrentStateWithWaypoint(0, 0, 10, -1);

            foreach (var instruction in input)
            {
                currentState = InstructionsBackSide[instruction.Direction](currentState, instruction.Value);
            }

            var ret = Math.Abs(currentState.X) + Math.Abs(currentState.Y);

            _testOutputHelper.WriteLine($"Result is : {ret}");
        }

        private static async Task<IList<(Direction Direction, int Value)>> ReadInput()
        {
            using var stream = File.OpenText("./Files/day12_input.txt");
            var fileText = await stream.ReadToEndAsync();
            return fileText
                .Split("\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => (ParseHelper[x.Substring(0, 1)], int.Parse(x.Substring(1))))
                .ToArray();
        }

        enum Direction
        {
            North = 0, East = 1, South = 2, West = 3, Right = 4, Left = 5, Forward = 6
        }

        private static readonly Dictionary<string, Direction> ParseHelper = new Dictionary<string, Direction>
        {
            {"F", Direction.Forward},
            {"R", Direction.Right},
            {"L", Direction.Left},
            {"N", Direction.North},
            {"S", Direction.South},
            {"W", Direction.West},
            {"E", Direction.East},
        };

        private static readonly Dictionary<Direction, Func<CurrentState, int, CurrentState>> Instructions =
            new Dictionary<Direction, Func<CurrentState, int, CurrentState>>
            {
                {Direction.Forward, (c, v) => Instructions[c.Direction](c, v)},
                {Direction.Right, (c, v) => new CurrentState(c.X, c.Y, (Direction)(((int)c.Direction + v / 90) % 4))},
                {Direction.Left, (c, v) =>
                    {
                        var newDirection = ((int) c.Direction - v / 90) % 4;
                        if (newDirection < 0)
                        {
                            newDirection = 4 + newDirection;
                        }

                        return new CurrentState(c.X, c.Y, (Direction) newDirection);
                    }
                },
                {Direction.North, (c, v) => new CurrentState(c.X, c.Y - v, c.Direction)},
                {Direction.South, (c, v) => new CurrentState(c.X, c.Y + v, c.Direction)},
                {Direction.West, (c, v) => new CurrentState(c.X - v, c.Y, c.Direction)},
                {Direction.East, (c, v) => new CurrentState(c.X + v, c.Y, c.Direction)}
            };

        private static readonly Dictionary<Direction, Func<CurrentStateWithWaypoint, int, CurrentStateWithWaypoint>> InstructionsBackSide =
            new Dictionary<Direction, Func<CurrentStateWithWaypoint, int, CurrentStateWithWaypoint>>
            {
                {Direction.Forward, (c, v) => new CurrentStateWithWaypoint(c.X + c.WX * v, c.Y + c.WY * v, c.WX, c.WY)},
                {Direction.Right, (c, v) => Rotate(c, v)},
                {Direction.Left, (c, v) => Rotate(c, -v)},
                {Direction.North, (c, v) => new CurrentStateWithWaypoint(c.X, c.Y, c.WX, c.WY - v)},
                {Direction.South, (c, v) => new CurrentStateWithWaypoint(c.X, c.Y, c.WX, c.WY + v)},
                {Direction.West, (c, v) => new CurrentStateWithWaypoint(c.X, c.Y, c.WX - v, c.WY)},
                {Direction.East, (c, v) => new CurrentStateWithWaypoint(c.X, c.Y, c.WX + v, c.WY)},
            };

        private static CurrentStateWithWaypoint Rotate(CurrentStateWithWaypoint c, int degrees)
        {
            var rad = degrees * Math.PI / 180;
            return new CurrentStateWithWaypoint(c.X, c.Y,
                (int)Math.Round(c.WX * Math.Cos(rad) - c.WY * Math.Sin(rad)),
                (int)Math.Round(c.WX * Math.Sin(rad) + c.WY * Math.Cos(rad)));
        }

        class CurrentState
        {
            public int X { get; }
            public int Y { get; }
            public Direction Direction { get; }

            public CurrentState(int x, int y, Direction direction)
            {
                X = x;
                Y = y;
                Direction = direction;
            }
        }

        class CurrentStateWithWaypoint
        {
            public int X { get; }
            public int Y { get; }
            public int WX { get; }
            public int WY { get; }

            public CurrentStateWithWaypoint(int x, int y, int wx, int wy)
            {
                X = x;
                Y = y;
                WX = wx;
                WY = wy;
            }
        }
    }
}

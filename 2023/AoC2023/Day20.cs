using Xunit;
using FluentAssertions;
using System.Text;
namespace AoC2023;

public class Day20
{
    private Data Parse(string input)
    {
        var allModules = input.Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select<string, Module>(r =>
            {
                var split = r.Split(new[] { " -> " }, StringSplitOptions.RemoveEmptyEntries);
                var destinations = split[1].Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                if (split[0] == "broadcaster")
                {
                    return new BroadCastModule(split[0], destinations);
                }
                else
                {
                    var name = split[0].Substring(1);
                    if (split[0][0] == '%')
                    {
                        return new FlipFlop(name, destinations);
                    }

                    return new Conjunction(name, destinations);
                }
            }).ToArray();

        var moduleMap = allModules.ToDictionary(x => x.Name, x => x);
        var broadCastModule = allModules.OfType<BroadCastModule>().Single();

        foreach (var m in allModules)
        {
            foreach (var d in m.Destinations)
            {
                if (!moduleMap.TryGetValue(d, out var module))
                {
                    moduleMap.Add(d, new Module(d, Array.Empty<string>()));
                }
                else
                {
                    if (module is Conjunction conj)
                    {
                        conj.Dependencies.Add(m.Name, false);
                    }
                }
            }
        }

        return new Data(broadCastModule, moduleMap);
    }

    record Data(BroadCastModule Broadcast, Dictionary<string, Module> Modules);
    record Impulse(string From, string To, bool isHigh);
    record Module(string Name, string[] Destinations);
    record BroadCastModule(string Name, string[] Destinations) : Module(Name, Destinations);
    record Conjunction(string Name, string[] Destinations) : Module(Name, Destinations)
    {
        public Dictionary<string, bool> Dependencies { get; init; } = new Dictionary<string, bool>();
    }
    record FlipFlop(string Name, string[] Destinations) : Module(Name, Destinations)
    {
        public bool IsOn { get; set; } = false;
    }


    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var data = Parse(input);
        var snapshots = new HashSet<string>();
        var lowHistory = new List<long> { 0 };
        var highHistory = new List<long> { 0 };

        while (snapshots.Count <= 1000)
        {
            if (!snapshots.Add(GetSnapshot(data)))
            {
                break;
            }
            else
            {
                var q = new Queue<Impulse>();
                var low = data.Broadcast.Destinations.Length + 1;
                var high = 0;
                foreach (var d in data.Broadcast.Destinations)
                {
                    q.Enqueue(new Impulse(data.Broadcast.Name, d, false));
                }

                while (q.Any())
                {
                    var (from, to, signal) = q.Dequeue();
                    var toModule = data.Modules[to];
                    switch (toModule)
                    {
                        case Conjunction conj:
                            conj.Dependencies[from] = signal;

                            var nextSignal = conj.Dependencies.Values.Any(x => !x);

                            foreach (var d in toModule.Destinations)
                            {
                                q.Enqueue(new Impulse(to, d, nextSignal));
                            }

                            if (nextSignal)
                            {
                                high += toModule.Destinations.Length;
                            }
                            else
                            {
                                low += toModule.Destinations.Length;
                            }

                            break;
                        case FlipFlop flipFlop:
                            if (!signal)
                            {
                                foreach (var d in toModule.Destinations)
                                {
                                    q.Enqueue(new Impulse(to, d, !flipFlop.IsOn));
                                }

                                if (flipFlop.IsOn)
                                {
                                    low += toModule.Destinations.Length;
                                }
                                else
                                {
                                    high += toModule.Destinations.Length;
                                }
                                flipFlop.IsOn = !flipFlop.IsOn;
                            }
                            break;
                    }
                }

                lowHistory.Add(lowHistory.Last() + low);
                highHistory.Add(highHistory.Last() + high);
            }
        }

        var totalLow = lowHistory.Last() * (1000 / snapshots.Count) + lowHistory[1000 - snapshots.Count * (1000 / snapshots.Count)];
        var totalHigh = highHistory.Last() * (1000 / snapshots.Count) + highHistory[1000 - snapshots.Count * (1000 / snapshots.Count)];
        var ret = totalLow * totalHigh;
        ret.Should().Be(expected);
    }

    private string GetSnapshot(Data data)
    {
        var sb = new StringBuilder();
        foreach (var m in data.Modules.Values.OrderBy(x => x.Name))
        {
            switch (m)
            {
                case Conjunction conj:
                    sb.Append(GetSnapshot(conj));
                    break;
                case FlipFlop flipFlop:
                    sb.Append(GetSnapshot(flipFlop));
                    break;
            }
        }

        return sb.ToString();
    }

    private string GetSnapshot(Conjunction conjunction)
    {
        return $"{conjunction.Name}:{string.Join(",", conjunction.Dependencies.Keys.OrderBy(x => x).Select(k => $"{(conjunction.Dependencies[k] ? 1 : 0)}"))} ";
    }

    private string GetSnapshot(FlipFlop flipFlop)
    {
        return $"{flipFlop.Name}:{(flipFlop.IsOn ? '+' : '-')} ";
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            broadcaster -> a, b, c
                            %a -> b
                            %b -> c
                            %c -> inv
                            &inv -> a
                            """,
                            32000000
                        },
            new object[]
                        {
                            """
                            broadcaster -> a
                            %a -> inv, con
                            &inv -> b
                            %b -> con
                            &con -> output
                            """,
                            11687500
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day20.txt"),
                            832957356
                        },
       };

    class Range
    {
        public Range(long zero, long one)
        {
            Zero = zero;
            One = one;
            Count = 1;
        }

        public long Zero { get; set; }
        public long One { get; set; }
        public long Count { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Range range &&
                   Zero == range.Zero && One == range.One;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Zero, One);
        }
    }

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var data = Parse(input);
        var rxDependencies = data.Modules.Values.Where(x => x.Destinations.Contains("rx")).OfType<Conjunction>().ToArray();
        rxDependencies = data.Modules.Values.Where(x => x.Destinations.Any(y => rxDependencies.Any(z => z.Name == y))).OfType<Conjunction>().ToArray();

        var highImpulse = rxDependencies.OfType<Conjunction>().ToDictionary(x => x.Name, x => 0L);
        var highImpulseSent = rxDependencies.OfType<Conjunction>().Select(x => x.Name).ToHashSet();

        var index = 0;
        while (true)
        {
            index++;
            var q = new Queue<Impulse>();
            foreach (var d in data.Broadcast.Destinations)
            {
                q.Enqueue(new Impulse(data.Broadcast.Name, d, false));
            }
            while (q.Any())
            {
                var (from, to, signal) = q.Dequeue();
                var toModule = data.Modules[to];
                if (highImpulseSent.Contains(from) && signal)
                {
                    highImpulseSent.Remove(from);
                    highImpulse[from] = index;
                }

                switch (toModule)
                {
                    case Conjunction conj:
                        conj.Dependencies[from] = signal;
                        var nextSignal = conj.Dependencies.Values.Any(x => !x);
                        foreach (var d in toModule.Destinations)
                        {
                            q.Enqueue(new Impulse(to, d, nextSignal));
                        }
                        break;
                    case FlipFlop flipFlop:
                        if (!signal)
                        {
                            foreach (var d in toModule.Destinations)
                            {
                                q.Enqueue(new Impulse(to, d, !flipFlop.IsOn));
                            }
                            flipFlop.IsOn = !flipFlop.IsOn;
                        }
                        break;
                }
            }

            if (!highImpulseSent.Any())
                break;
        };

        var ret = highImpulse.Values.Aggregate(1L, (acc, c) => acc * c);
        
        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
        new List<object[]>
        {
            new object[]
                        {
                            File.ReadAllText("Inputs/Day20.txt"),
                            240162699605221
                        },
       };
}

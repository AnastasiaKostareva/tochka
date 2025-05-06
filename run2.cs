using System;
using System.Collections.Generic;
using System.Linq;


class Program
{
    // Константы для символов ключей и дверей
    static readonly char[] keys_char = Enumerable.Range('a', 26).Select(i => (char)i).ToArray();
    static readonly char[] doors_char = keys_char.Select(char.ToUpper).ToArray();

    // Метод для чтения входных данных
    static List<List<char>> GetInput()
    {
        var data = new List<List<char>>();
        string line;
        while ((line = Console.ReadLine()) != null && line != "")
        {
            data.Add(line.ToCharArray().ToList());
        }
        return data;
    }


    static int Solve(List<List<char>> data)
    {
        var result = -1;
        var n = data.Count;
        var m = 0;
        var positionsRobots = new List<(int, int)>();
        var positionsKeys = new HashSet<(int, int)>();
        var queue = new Queue<BFSState>();
        var visited = new HashSet<string>();

        for (int i = 0; i < n; i++)
        {
            var str = data[i];
            m = data[i].Count;
            for (int j = 0; j < str.Count; j++)
            {
                if (data[i][j] == '@')
                {
                    positionsRobots.Add((i, j));
                }
                if (keys_char.Contains(data[i][j]))
                    positionsKeys.Add((i, j));
            }
        }

        var startBFSState = new BFSState(positionsRobots[0], positionsRobots[1], positionsRobots[2], positionsRobots[3], new HashSet<char>(), 0);
        queue.Enqueue(startBFSState);
        while (queue.Count != 0)
        {
            var state = queue.Dequeue();
            if (state.HavingKeys.Count == positionsKeys.Count)
                return state.countStep;

            if (CheckNoValidCoord(state.Robot1X, state.Robot1Y, n, m, data, positionsKeys, state.HavingKeys) || CheckNoValidCoord(state.Robot2X, state.Robot2Y, n, m, data, positionsKeys, state.HavingKeys)
                || CheckNoValidCoord(state.Robot3X, state.Robot3Y, n, m, data, positionsKeys, state.HavingKeys) || CheckNoValidCoord(state.Robot4X, state.Robot4Y, n, m, data, positionsKeys, state.HavingKeys))
                continue;
            var d = new List<(int, int)> { (1, 0), (0, 1), (-1, 0), (0, -1) };
            foreach (var permutation in d)
            {
                var dx = permutation.Item1;
                var dy = permutation.Item2;
                var newState1 = CheckKey(new BFSState((state.Robot1X + dx, state.Robot1Y + dy), (state.Robot2X, state.Robot2Y), (state.Robot3X, state.Robot3Y), (state.Robot4X, state.Robot4Y), state.HavingKeys, state.countStep + 1), data, positionsKeys);
                var newState2 = CheckKey(new BFSState((state.Robot1X, state.Robot1Y), (state.Robot2X + dx, state.Robot2Y + dy), (state.Robot3X, state.Robot3Y), (state.Robot4X, state.Robot4Y), state.HavingKeys, state.countStep + 1), data, positionsKeys);
                var newState3 = CheckKey(new BFSState((state.Robot1X, state.Robot1Y), (state.Robot2X, state.Robot2Y), (state.Robot3X + dx, state.Robot3Y + dy), (state.Robot4X, state.Robot4Y), state.HavingKeys, state.countStep + 1), data, positionsKeys);
                var newState4 = CheckKey(new BFSState((state.Robot1X, state.Robot1Y), (state.Robot2X, state.Robot2Y), (state.Robot3X, state.Robot3Y), (state.Robot4X + dx, state.Robot4Y + dy), state.HavingKeys, state.countStep + 1), data, positionsKeys);
                var key = GetStateKey(newState1);
                if (!visited.Contains(key))
                {
                    visited.Add(key);
                    queue.Enqueue(newState1);
                }
                key = GetStateKey(newState2);
                if (!visited.Contains(key))
                {
                    visited.Add(key);
                    queue.Enqueue(newState2);
                }
                key = GetStateKey(newState3);
                if (!visited.Contains(key))
                {
                    visited.Add(key);
                    queue.Enqueue(newState3);
                }
                key = GetStateKey(newState4);
                if (!visited.Contains(key))
                {
                    visited.Add(key);
                    queue.Enqueue(newState4);
                }
            }

        }

        return result;
    }

    static string GetStateKey(BFSState state)
    {
        return $"{state.Robot1X},{state.Robot1Y},{state.Robot2X},{state.Robot2Y},{state.Robot3X},{state.Robot3Y},{state.Robot4X},{state.Robot4Y}," +
            $"{string.Join(",", state.HavingKeys.OrderBy(k => k))}";
    }

    static BFSState CheckKey(BFSState state, List<List<char>> data, HashSet<(int, int)> positionsKeys)
    {
        var newKeys = new HashSet<char>(state.HavingKeys);
        var robots = new List<(int, int)> {
            (state.Robot1X, state.Robot1Y),
            (state.Robot2X, state.Robot2Y),
            (state.Robot3X, state.Robot3Y),
            (state.Robot4X, state.Robot4Y)
        };
        foreach (var robot in robots)
        {
            if (positionsKeys.Contains(robot))
                newKeys.Add(data[robot.Item1][robot.Item2]);
        }
        if (newKeys.Count != state.HavingKeys.Count)
            return new BFSState(robots[0], robots[1], robots[2], robots[3], newKeys, state.countStep);
        return state;
    }

    static bool CheckNoValidCoord(int RobotX, int RobotY, int n, int m, List<List<char>> data, HashSet<(int, int)> positionsKeys, HashSet<char> HavingKeys)
    {
        if (RobotX < 0 || RobotX >= n || RobotY < 0 || RobotY >= m)
            return true;
        var state = data[RobotX][RobotY];
        if (!(state == '.' || state == '@' || positionsKeys.Contains((RobotX, RobotY)) || (char.IsUpper(state) && HavingKeys.Contains(char.ToLower(state)))))
            return true;

        return false;
    }

    static void Main()
    {
        var data = GetInput();
        int result = Solve(data);

        if (result == -1)
        {
            Console.WriteLine("No solution found");
        }
        else
        {
            Console.WriteLine(result);
        }
    }
}

public class BFSState
{
    public int Robot1X;
    public int Robot1Y;
    public int Robot2X;
    public int Robot2Y;
    public int Robot3X;
    public int Robot3Y;
    public int Robot4X;
    public int Robot4Y;
    public HashSet<char> HavingKeys;
    public int countStep;

    public BFSState((int, int) Robot1, (int, int) Robot2, (int, int) Robot3, (int, int) Robot4, HashSet<char> HavingKeys, int countStep)
    {
        Robot1X = Robot1.Item1;
        Robot1Y = Robot1.Item2;
        Robot2X = Robot2.Item1;
        Robot2Y = Robot2.Item2;
        Robot3X = Robot3.Item1;
        Robot3Y = Robot3.Item2;
        Robot4X = Robot4.Item1;
        Robot4Y = Robot4.Item2;
        this.HavingKeys = HavingKeys;
        this.countStep = countStep;
    }
}

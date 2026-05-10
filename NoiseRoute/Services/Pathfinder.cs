using NoiseRoute.Models;

namespace NoiseRoute.Services;

public delegate double Heuristic(
    in PointInt2D nextPoint,
    in PointInt2D goal,
    in DirectionInt currentDirection,
    in DirectionInt nextDirection,
    in double[,] noiseMap,
    in int noiseRadius);

public sealed class Pathfinder
{
    private static readonly Dictionary<DirectionInt, DirectionInt[]> DirectionsMap = new() {
        [DirectionInt.Top] = [DirectionInt.TopLeft, DirectionInt.Top, DirectionInt.TopRight],
        [DirectionInt.TopLeft] = [DirectionInt.Left, DirectionInt.TopLeft, DirectionInt.Top],
        [DirectionInt.Left] = [DirectionInt.BottomLeft, DirectionInt.Left, DirectionInt.TopLeft],
        [DirectionInt.BottomLeft] = [DirectionInt.Bottom, DirectionInt.BottomLeft, DirectionInt.Left],
        [DirectionInt.Bottom] = [DirectionInt.BottomRight, DirectionInt.Bottom, DirectionInt.BottomLeft],
        [DirectionInt.BottomRight] = [DirectionInt.Right, DirectionInt.BottomRight, DirectionInt.Bottom],
        [DirectionInt.Right] = [DirectionInt.BottomRight, DirectionInt.Right, DirectionInt.TopRight],
        [DirectionInt.TopRight] = [DirectionInt.Right, DirectionInt.TopRight, DirectionInt.Top],
    };

    private static double GetDistance(in PointInt2D a, in PointInt2D b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private static double GetDirectionCost(in DirectionInt currentDirection, in DirectionInt nextDirection)
    {
        return currentDirection == nextDirection ? 10 : 50;
    }

    public static double DefaultHeuristic(
        in PointInt2D nextPoint,
        in PointInt2D goal,
        in DirectionInt _,
        in DirectionInt _1,
        in double[,] _2,
        in int _3)
    {
        return GetDistance(nextPoint, goal);
    }

    public static double NoiseSensitiveHeuristic(
        in PointInt2D nextPoint,
        in PointInt2D goal,
        in DirectionInt currentDirection,
        in DirectionInt nextDirection,
        in double[,] noiseMap,
        in int noiseRadius)
    {
        var absoluteNoiseValue = noiseMap[nextPoint.Y, nextPoint.X];

        if (absoluteNoiseValue == -1)
            return 5000;

        var maximumNoiseValue = GetMaxNoiseInCircleRadius(
            nextPoint,
            noiseMap,
            noiseRadius);

        return GetDirectionCost(currentDirection, nextDirection) * 0.35
            + GetDistance(nextPoint, goal) * 0.2
            + maximumNoiseValue * 0.45;
    }

    public static double GetMaxNoiseInCircleRadius(
        in PointInt2D center,
        in double[,] noiseMap,
        in int radius)
    {
        int width = noiseMap.GetLength(1);
        int height = noiseMap.GetLength(0);

        double maxNoise = 0;
        int radiusSq = radius * radius;

        int minX = Math.Max(0, center.X - radius);
        int maxX = Math.Min(width - 1, center.X + radius);
        int minY = Math.Max(0, center.Y - radius);
        int maxY = Math.Min(height - 1, center.Y + radius);

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                int dx = x - center.X;
                int dy = y - center.Y;

                if (dx * dx + dy * dy > radiusSq)
                    continue;

                double noise = noiseMap[y, x];
                if (noise > maxNoise)
                    maxNoise = noise;
            }
        }

        return maxNoise;
    }

    public static List<PointInt2D> FindPath(
        in DirectionInt startDirection,
        in PointInt2D start,
        in PointInt2D goal,
        in double[,] noiseMap,
        in int noiseRadius,
        in Heuristic heuristic)
    {
        int h = noiseMap.GetLength(0);
        int w = noiseMap.GetLength(1);

        var open = new PriorityQueue<PathNode, double>();
        var all = new Dictionary<(int x, int y), PathNode>((int)(w * h * 0.2));
        var closed = new HashSet<(int x, int y)>((int)(w * h * 0.2));

        var startNode = new PathNode(start.X, start.Y, startDirection) {
            Cost = 0
        };

        open.Enqueue(startNode, startNode.Cost);
        all[(start.X, start.Y)] = startNode;

        while (open.Count > 0)
        {
            var current = open.Dequeue();

            if (current.X == goal.X && current.Y == goal.Y)
                return Reconstruct(current);

            closed.Add((current.X, current.Y));

            foreach (var availableDirection in DirectionsMap[current.Direction])
            {
                var nx = current.X + availableDirection.DX;
                var ny = current.Y + availableDirection.DY;

                if (nx <= 0 || ny <= 0 || nx >= w || ny >= h)
                    continue;

                if (closed.Contains((nx, ny)))
                    continue;

                var newPoint = new PointInt2D(nx, ny);
                var hCost = heuristic(
                    newPoint,
                    goal,
                    current.Direction,
                    availableDirection,
                    noiseMap,
                    noiseRadius);

                if (!all.TryGetValue((nx, ny), out var neighbor))
                {
                    neighbor = new PathNode(nx, ny, availableDirection);
                    all[(nx, ny)] = neighbor;
                }

                var tentativeG = current.Cost + hCost;

                if (neighbor.Parent == null || tentativeG < neighbor.Cost)
                {
                    neighbor.Parent = current;
                    neighbor.Cost = tentativeG;
                    neighbor.Direction = availableDirection;

                    open.Enqueue(neighbor, neighbor.Cost);
                }
            }
        }

        return [];
    }

    private static List<PointInt2D> Reconstruct(PathNode node)
    {
        var path = new List<PointInt2D>();
        var current = node;

        while (current != null)
        {
            path.Add(new PointInt2D(current.X, current.Y));
            current = current.Parent;
        }

        path.Reverse();
        return path;
    }
}

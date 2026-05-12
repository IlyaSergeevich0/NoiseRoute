using NoiseRoute.Models;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

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
        in DirectionInt currentDirection,
        in DirectionInt nextDirection,
        in double[,] _,
        in int __)
    {
        return GetDistance(nextPoint, goal) * 0.3
            + GetDirectionCost(currentDirection, nextDirection) * 0.7;
    }

    public static double NoiseSensitiveHeuristic(
        in PointInt2D nextPoint,
        in PointInt2D goal,
        in DirectionInt currentDirection,
        in DirectionInt nextDirection,
        in double[,] noiseMap,
        in int noiseRadius)
    {
        var noiseValue = GetMaxNoiseInRadius(
            nextPoint,
            noiseMap,
            noiseRadius);

        return GetDirectionCost(currentDirection, nextDirection) * 0.35
            + GetDistance(nextPoint, goal) * 0.05
            + noiseValue * 0.6;
    }

    private static double GetMaxNoiseInRadius(
        in PointInt2D center,
        in double[,] noiseMap,
        in int radius)
    {
        var width = noiseMap.GetLength(1);
        var height = noiseMap.GetLength(0);

        var maxNoise = 0.0;
        var radiusSq = radius * radius;

        var minX = Math.Max(0, center.X - radius);
        var maxX = Math.Min(width - 1, center.X + radius);
        var minY = Math.Max(0, center.Y - radius);
        var maxY = Math.Min(height - 1, center.Y + radius);

        for (var y = minY; y <= maxY; y++)
        {
            for (var x = minX; x <= maxX; x++)
            {
                var dx = x - center.X;
                var dy = y - center.Y;

                if (dx * dx + dy * dy > radiusSq)
                    continue;

                var noise = noiseMap[y, x];
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
        var directionsMap = new Dictionary<DirectionInt, DirectionInt[]>() {
            [DirectionInt.Top] = [DirectionInt.TopLeft, DirectionInt.Top, DirectionInt.TopRight],
            [DirectionInt.TopLeft] = [DirectionInt.Left, DirectionInt.TopLeft, DirectionInt.Top],
            [DirectionInt.Left] = [DirectionInt.BottomLeft, DirectionInt.Left, DirectionInt.TopLeft],
            [DirectionInt.BottomLeft] = [DirectionInt.Bottom, DirectionInt.BottomLeft, DirectionInt.Left],
            [DirectionInt.Bottom] = [DirectionInt.BottomRight, DirectionInt.Bottom, DirectionInt.BottomLeft],
            [DirectionInt.BottomRight] = [DirectionInt.Right, DirectionInt.BottomRight, DirectionInt.Bottom],
            [DirectionInt.Right] = [DirectionInt.BottomRight, DirectionInt.Right, DirectionInt.TopRight],
            [DirectionInt.TopRight] = [DirectionInt.Right, DirectionInt.TopRight, DirectionInt.Top],
        };

        var height = noiseMap.GetLength(0);
        var width = noiseMap.GetLength(1);

        var open = new PriorityQueue<PathNode, double>();
        var closed = new bool[height, width];
        var nodes = new PathNode?[height, width];

        var startNode = new PathNode {
            X = start.X,
            Y = start.Y,
            Direction = startDirection,
            Cost = 0,
            Heuristic = heuristic(start, goal, startDirection, startDirection, noiseMap, noiseRadius)
        };

        nodes[start.Y, start.X] = startNode;
        open.Enqueue(startNode, startNode.TotalCost);

        while (open.Count > 0)
        {
            var current = open.Dequeue();

            if (closed[current.Y, current.X])
                continue;

            if (current.X == goal.X && current.Y == goal.Y)
                return Reconstruct(current);

            closed[current.Y, current.X] = true;
            
            foreach (var availableDirection in directionsMap[current.Direction])
            {
                var nx = current.X + availableDirection.DX;
                var ny = current.Y + availableDirection.DY;

                if (nx <= 0 || ny <= 0 || nx >= width || ny >= height)
                    continue;

                if (closed[ny, nx])
                    continue;

                if (noiseMap[ny, nx] == -1)
                    continue;

                var newPoint = new PointInt2D(nx, ny);
                var currentHeuristic = heuristic(newPoint, goal, current.Direction, availableDirection, noiseMap, noiseRadius);
                var tentativeCost = current.Cost + currentHeuristic;
                var neighbor = nodes[ny, nx];

                if (neighbor is null)
                {
                    neighbor = new PathNode { 
                        X = nx, 
                        Y = ny,
                        Cost = double.PositiveInfinity
                    };
                    nodes[ny, nx] = neighbor;
                }

                if (tentativeCost >= neighbor.Cost)
                    continue;

                neighbor.Parent = current;
                neighbor.Cost = tentativeCost;
                neighbor.Direction = availableDirection;
                neighbor.Heuristic = currentHeuristic;

                open.Enqueue(neighbor, neighbor.TotalCost);
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

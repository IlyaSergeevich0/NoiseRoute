using NoiseRoute.Models;

namespace NoiseRoute.Services;

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

    private static double GetDirectionCost(DirectionInt current, DirectionInt next)
    {
        return current == next ? 1 : 1.5;
    }

    public List<PointInt2D> FindPath(double[,] noiseMap, DirectionInt startDirection, PointInt2D start, PointInt2D goal)
    {
        int h = noiseMap.GetLength(0);
        int w = noiseMap.GetLength(1);

        var open = new List<PathNode>();
        var all = new Dictionary<(int x, int y), PathNode>();
        var closed = new HashSet<(int x, int y)>();

        var startNode = new PathNode(start.X, start.Y, startDirection) {
            GCost = 0
        };

        open.Add(startNode);
        all[(start.X, start.Y)] = startNode;

        while (open.Count > 0)
        {
            var current = open.OrderBy(n => n.GCost).First();
            open.Remove(current);

            if (current.X == goal.X && current.Y == goal.Y)
                return Reconstruct(current);

            closed.Add((current.X, current.Y));

            foreach (var availableDirection in DirectionsMap[current.Direction])
            {
                var nx = current.X + availableDirection.DX;
                var ny = current.Y + availableDirection.DY;

                if (nx < 30 || ny < 30 || nx >= w - 50 || ny >= h - 30)
                    continue;

                if (closed.Contains((nx, ny)))
                    continue;

                var newPoint = new PointInt2D(nx, ny);
                var stepCost = GetDirectionCost(current.Direction, availableDirection) * 0.05
                    + Heuristic(newPoint, goal) * 0.05
                    + noiseMap[ny, nx] * 0.9;

                if (!all.TryGetValue((nx, ny), out var neighbor))
                {
                    neighbor = new PathNode(nx, ny, availableDirection);
                    all[(nx, ny)] = neighbor;
                }

                var tentativeG = current.GCost + stepCost;

                if (neighbor.Parent == null || tentativeG < neighbor.GCost)
                {
                    neighbor.Parent = current;
                    neighbor.GCost = tentativeG;
                    neighbor.Direction = availableDirection;

                    if (!open.Contains(neighbor))
                        open.Add(neighbor);
                }
            }
        }

        return [];
    }

    private static double Heuristic(PointInt2D a, PointInt2D b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        return Math.Sqrt(dx * dx + dy * dy);
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

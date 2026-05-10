namespace NoiseRoute.Models;

public sealed class PathNode(int x, int y, DirectionInt direction)
{
    public int X { get; } = x;
    public int Y { get; } = y;
    public double Cost { get; set; }
    public DirectionInt Direction { get; set; } = direction;
    public PathNode? Parent { get; set; }

    public bool SameAs(PathNode other)
    {
        return X == other.X && Y == other.Y;
    }
}

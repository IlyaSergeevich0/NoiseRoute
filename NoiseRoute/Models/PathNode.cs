namespace NoiseRoute.Models;

public sealed class PathNode
{
    public int X { get; }
    public int Y { get; }
    public double GCost { get; set; }
    public DirectionInt Direction { get; set; }
    public PathNode? Parent { get; set; }

    public PathNode(int x, int y, DirectionInt direction)
    {
        X = x;
        Y = y;
        Direction = direction;
    }
}

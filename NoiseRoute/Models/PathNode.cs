namespace NoiseRoute.Models;

public sealed class PathNode
{
    public double TotalCost => Cost + Heuristic;

    public int X { get; init; }
    public int Y { get; init; }
    public DirectionInt Direction { get; set; }
    public double Cost { get; set; }
    public double Heuristic { get; set; }
    public PathNode? Parent { get; set; }
}

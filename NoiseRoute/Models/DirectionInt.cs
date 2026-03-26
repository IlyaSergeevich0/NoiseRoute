namespace NoiseRoute.Models;

public readonly record struct DirectionInt(int DX, int DY)
{
    public static readonly DirectionInt Top = new(0, 1);
    public static readonly DirectionInt TopLeft = new(-1, 1);
    public static readonly DirectionInt Left = new(-1, 0);
    public static readonly DirectionInt BottomLeft = new(-1, -1);
    public static readonly DirectionInt Bottom = new(0, -1);
    public static readonly DirectionInt BottomRight = new(1, -1);
    public static readonly DirectionInt Right = new(1, 0);
    public static readonly DirectionInt TopRight = new(1, 1);
}

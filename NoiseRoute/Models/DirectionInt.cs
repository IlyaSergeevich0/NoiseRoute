namespace NoiseRoute.Models;

public readonly struct DirectionInt(int dx, int dy)
{
    public readonly int DX = dx;
    public readonly int DY = dy;

    public static readonly DirectionInt Top = new(0, 1);
    public static readonly DirectionInt TopLeft = new(-1, 1);
    public static readonly DirectionInt Left = new(-1, 0);
    public static readonly DirectionInt BottomLeft = new(-1, -1);
    public static readonly DirectionInt Bottom = new(0, -1);
    public static readonly DirectionInt BottomRight = new(1, -1);
    public static readonly DirectionInt Right = new(1, 0);
    public static readonly DirectionInt TopRight = new(1, 1);

    public static bool operator ==(DirectionInt first, DirectionInt second)
    {
        return first.DX == second.DX && first.DY == second.DY;
    }

    public static bool operator !=(DirectionInt first, DirectionInt second)
    {
        return first.DX != second.DX || first.DY != second.DY;
    }
}

namespace NoiseRoute.Models;

public sealed class MapZone
{
    public const double NoiseSmoothing = 0.1;

    public int X { get; init; }
    public int Y { get; init; }
    public double Radius { get; init; }
    public double SplRef { get; init; }
    public MapZoneType Type { get; init; }

    public double GetNoiseAt(int x, int y)
    {
        var dx = X - x;
        var dy = Y - y;
        var distance = Math.Sqrt(dx * dx + dy * dy);
        var spl = SplRef - 20 * Math.Log10(distance);


        return Math.Clamp((spl - 40) / (SplRef - 40), 0, 1) * Radius;
    }
}

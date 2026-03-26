using NoiseRoute.Models;
using NoiseRoute.Services;
using OxyPlot;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NoiseRoute.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
    public PlotModel? PlotModel
    {
        get => field;
        set { field = value; OnPropertyChanged(); }
    }

    public string Status
    {
        get => field;
        set { field = value; OnPropertyChanged(); }
    } = "";

    public MainViewModel()
    {
        Generate();
    }

    public void Generate()
    {
        const int Width = 500;
        const int Height = 500;

        var mapGen = new MapGenerator();
        var noiseGen = new NoiseMapGenerator();
        var pathfinder = new Pathfinder();
        var plotService = new PlotService();

        var map = mapGen.GenerateZones(Width, Height);
        var noise = noiseGen.BuildNoiseMap(map, Width, Height);

        var startDirection = DirectionInt.Top;
        var start = new PointInt2D(50, 50);
        var goal = new PointInt2D(400, 450);

        var path = pathfinder.FindPath(noise, startDirection, start, goal);
        PlotModel = plotService.BuildModel(noise, path, start, goal);

        Status = $"Точек в пути: {path.Count}";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

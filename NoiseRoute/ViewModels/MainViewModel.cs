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

    public MainViewModel() { }

    public void Generate()
    {
        Status = "Генерация...";

        const int Width = 1316;
        const int Height = 771;

        var noiseGen = new NoiseMapGenerator();
        var pathfinder = new Pathfinder();
        var plotService = new PlotService();

        var noise = noiseGen.BuildNoiseMap("D:\\1-Uni\\0-VKRB\\src\\NoiseMap-1.png", Width, Height);

        var startDirection = DirectionInt.Top;
        var start = new PointInt2D(143, 78);
        // var start = new PointInt2D(95, 300);
        var goal = new PointInt2D(890, 437);

        var path = pathfinder.FindPath(noise, startDirection, start, goal);
        PlotModel = plotService.BuildModel(noise, path, start, goal);

        Status = $"Точек в пути: {path.Count}";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

using NoiseRoute.Models;
using NoiseRoute.Services;
using OxyPlot;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace NoiseRoute.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
    public int StartX
    {
        get;
        set {
            if (field == value) return;
            field = value;
            OnPropertyChanged();
        }
    } = 143;

    public int StartY
    {
        get;
        set {
            if (field == value) return;
            field = value;
            OnPropertyChanged();
        }
    } = 78;

    public int GoalX
    {
        get;
        set {
            if (field == value) return;
            field = value;
            OnPropertyChanged();
        }
    } = 1050;

    public int GoalY
    {
        get;
        set {
            if (field == value) return;
            field = value;
            OnPropertyChanged();
        }
    } = 475;

    public int NoiseRadius
    {
        get;
        set {
            if (field == value) return;
            field = value;
            OnPropertyChanged();
        }
    } = 10;

    public PointInt2D StartPoint => new(StartX, StartY);
    public PointInt2D GoalPoint => new(GoalX, GoalY);

    public PlotModel? PlotModel
    {
        get;
        set { field = value; OnPropertyChanged(); }
    }

    public BitmapSource? NoiseMap
    {
        get;
        set { field = value; OnPropertyChanged(); }
    }

    public string Status
    {
        get;
        set { field = value; OnPropertyChanged(); }
    } = "";

    public bool IsAvailable
    {
        get;
        set { field = value; OnPropertyChanged(); }
    } = true;

    public MainViewModel() { }

    public async Task GenerateAsync()
    {
        if (!IsAvailable)
            return;

        const int Width = 1316;
        const int Height = 771;

        var (noiseMap, noiseMapImage) = NoiseMapGenerator.BuildNoiseMap("..\\..\\..\\NoiseMap-1.png", Width, Height);

        var startDirection = DirectionInt.Top;
        var start = StartPoint;
        var goal = GoalPoint;

        if (!Validator.ValidatePoint("Начальная", start, noiseMap)
            || !Validator.ValidatePoint("Целевая", goal, noiseMap))
            return;

        IsAvailable = false;
        Status = "Генерация...";

        var defaultPathTask = Task.Run(() => Pathfinder.FindPath(startDirection, start, goal, noiseMap, NoiseRadius, Pathfinder.DefaultHeuristic));
        var optimizedPathTask = Task.Run(() => Pathfinder.FindPath(startDirection, start, goal, noiseMap, NoiseRadius, Pathfinder.NoiseSensitiveHeuristic));

        await Task.WhenAll(defaultPathTask, optimizedPathTask);

        var defaultPath = defaultPathTask.Result;
        var optimizedPath = optimizedPathTask.Result;

        NoiseMap = noiseMapImage;
        PlotModel = PlotService.BuildModel(noiseMap, NoiseRadius, defaultPath, optimizedPath, start, goal);

        Status = $"Точек в пути\nСтандартный путь: {defaultPath.Count}\nОптимальный путь: {optimizedPath.Count}";
        IsAvailable = true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

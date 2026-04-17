using NoiseRoute.Models;
using NoiseRoute.Services;
using OxyPlot;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace NoiseRoute.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{    
    public int StartX {
        get;
        set {
            if (field == value) return;
            field = value;
            OnPropertyChanged();
        }
    } = 143;

    public int StartY {
        get;
        set {
            if (field == value) return;
            field = value;
            OnPropertyChanged();
        }
    } = 78;

    public int GoalX {
        get;
        set {
            if (field == value) return;
            field = value;
            OnPropertyChanged();
        }
    } = 890;

    public int GoalY {
        get;
        set {
            if (field == value) return;
            field = value;
            OnPropertyChanged();
        }
    } = 437;

    public PointInt2D StartPoint => new(StartX, StartY);
    public PointInt2D GoalPoint => new(GoalX, GoalY);

    public PlotModel? PlotModel {
        get;
        set { field = value; OnPropertyChanged(); }
    }

    public BitmapSource? NoiseMap {
        get;
        set { field = value; OnPropertyChanged(); }
    }

    public string Status {
        get;
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

        var (noiseMap, noiseMapImage) = noiseGen.BuildNoiseMap("..\\..\\..\\NoiseMap-1.png", Width, Height);

        var startDirection = DirectionInt.Top;
        var start = StartPoint;        
        var goal = GoalPoint;
        var path = pathfinder.FindPath(noiseMap, startDirection, start, goal);
        NoiseMap = noiseMapImage;
        PlotModel = plotService.BuildModel(noiseMap, path, start, goal);

        Status = $"Точек в пути: {path.Count}";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

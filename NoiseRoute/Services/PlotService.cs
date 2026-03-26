using NoiseRoute.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace NoiseRoute.Services;

public sealed class PlotService
{
    public PlotModel BuildModel(double[,] noiseMap, List<PointInt2D> path, PointInt2D start, PointInt2D goal)
    {
        int h = noiseMap.GetLength(0);
        int w = noiseMap.GetLength(1);

        var model = new PlotModel { Title = "Карта шума и маршрут" };

        model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Minimum = 0, Maximum = w - 1 });
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = 0, Maximum = h - 1 });

        model.Axes.Add(new LinearColorAxis {
            Position = AxisPosition.Right,
            Palette = OxyPalettes.Viridis(256)
        });

        var heat = new HeatMapSeries {
            X0 = 0,
            X1 = w - 1,
            Y0 = 0,
            Y1 = h - 1,
            Interpolate = true,
            RenderMethod = HeatMapRenderMethod.Bitmap,
            Data = noiseMap
        };

        model.Series.Add(heat);

        if (path.Count > 0)
        {
            var line = new LineSeries {
                Color = OxyColors.Red,
                StrokeThickness = 2.5,
                Title = "Маршрут"
            };

            foreach (var p in path)
                line.Points.Add(new DataPoint(p.X, p.Y));

            model.Series.Add(line);
        }

        var startSeries = new ScatterSeries {
            MarkerType = MarkerType.Circle,
            MarkerFill = OxyColors.LimeGreen,
            MarkerSize = 100,
            Title = "Старт"
        };
        startSeries.Points.Add(new ScatterPoint(start.X, start.Y));
        model.Series.Add(startSeries);

        var goalSeries = new ScatterSeries {
            MarkerType = MarkerType.Square,
            MarkerFill = OxyColors.OrangeRed,
            MarkerSize = 5,
            Title = "Цель"
        };
        goalSeries.Points.Add(new ScatterPoint(goal.X, goal.Y));
        model.Series.Add(goalSeries);

        return model;
    }
}

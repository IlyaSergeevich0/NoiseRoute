using NoiseRoute.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;

namespace NoiseRoute.Services;

public sealed class PlotService
{
    public static PlotModel BuildModel(
        in double[,] noiseMap,
        in int noiseRadius,
        in List<PointInt2D> defaultPath,
        in List<PointInt2D> optimizedPath,
        in PointInt2D start,
        in PointInt2D goal)
    {
        int h = noiseMap.GetLength(0);
        int w = noiseMap.GetLength(1);

        var model = new PlotModel {
            Title = "Карта маршрутов",
            IsLegendVisible = true
        };
        var legend = new Legend {
            LegendPosition = LegendPosition.RightTop,
            LegendPlacement = LegendPlacement.Inside,
            LegendOrientation = LegendOrientation.Vertical,
            LegendBackground = OxyColors.Snow.ChangeOpacity(0.65)
        };

        model.Legends.Add(legend);
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Minimum = 0, Maximum = w - 1, IsZoomEnabled = false });
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = 0, Maximum = h - 1, IsZoomEnabled = false });

        if (defaultPath.Count > 0)
        {
            var line = new LineSeries {
                Color = OxyColors.Magenta,
                StrokeThickness = 3,
                Title = "Обычный маршрут",
                RenderInLegend = true
            };
            var lineZone = new LineSeries {
                Color = OxyColors.Magenta.ChangeOpacity(0.18),
                StrokeThickness = noiseRadius * 2,
                Title = "Зона шумового воздействия",
                RenderInLegend = true
            };

            foreach (var p in defaultPath)
            {
                var dataPoint = new DataPoint(p.X, p.Y);

                line.Points.Add(dataPoint);
                lineZone.Points.Add(dataPoint);
            }

            model.Series.Add(line);
            model.Series.Add(lineZone);
        }

        if (optimizedPath.Count > 0)
        {
            var line = new LineSeries {
                Color = OxyColors.Aqua,
                StrokeThickness = 3,
                Title = "Оптимальный маршрут",
                RenderInLegend = true

            };
            var lineZone = new LineSeries {
                Color = OxyColors.Aqua.ChangeOpacity(0.18),
                StrokeThickness = noiseRadius * 2,
                Title = "Зона шумового воздействия",
                RenderInLegend = true
            };

            foreach (var p in optimizedPath)
            {
                line.Points.Add(new DataPoint(p.X, p.Y));
                lineZone.Points.Add(new DataPoint(p.X, p.Y));
            }

            model.Series.Add(line);
            model.Series.Add(lineZone);
        }

        var startSeries = new ScatterSeries {
            MarkerType = MarkerType.Diamond,
            MarkerFill = OxyColors.Yellow,
            MarkerSize = 3.5,
            Title = "Старт",
            RenderInLegend = true
        };
        startSeries.Points.Add(new ScatterPoint(start.X, start.Y));
        model.Series.Add(startSeries);

        var goalSeries = new ScatterSeries {
            MarkerType = MarkerType.Diamond,
            MarkerFill = OxyColors.Orange,
            MarkerSize = 3.5,
            Title = "Цель",
            RenderInLegend = true
        };
        goalSeries.Points.Add(new ScatterPoint(goal.X, goal.Y));
        model.Series.Add(goalSeries);

        return model;
    }
}

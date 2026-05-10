using NoiseRoute.Models;
using System.Windows;

namespace NoiseRoute.Services;

public class Validator
{
    public static bool ValidatePoint(
        in string name,
        in PointInt2D point,
        in double[,] noiseMap)
    {
        if (point.Y < 0
            || point.Y >= noiseMap.GetLength(0)
            || point.X < 0
            || point.X >= noiseMap.GetLength(1))
        {
            MessageBox.Show(
                $"{name} точка находится за пределами карты!",
                "Ошибка валидации",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return false;
        }

        if (noiseMap[point.Y, point.X] == -1)
        {
            MessageBox.Show(
                $"{name} точка расположена в недопустимом месте!",
                "Ошибка валидации",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return false;
        }

        return true;
    }
}

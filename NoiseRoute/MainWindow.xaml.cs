using NoiseRoute.ViewModels;
using System.Windows;

namespace NoiseRoute;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _vm;
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await _vm.GenerateAsync();
        }
        catch (Exception exception)
        {
            MessageBox.Show($"Ошибка: {exception.Message}",
                            "Ошика генерации",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error,
                            MessageBoxResult.OK);
        }
    }
}

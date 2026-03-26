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

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        _vm.Generate();
    }
}

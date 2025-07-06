using AcademicPlannerApp.ViewModels;

namespace AcademicPlannerApp;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Llama al comando para cargar datos cuando la página aparece
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
    }
}
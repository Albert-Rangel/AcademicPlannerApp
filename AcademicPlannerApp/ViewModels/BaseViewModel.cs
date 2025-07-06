using CommunityToolkit.Mvvm.ComponentModel;

namespace AcademicPlannerApp.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        bool isBusy;

        [ObservableProperty]
        string title;
    }
}
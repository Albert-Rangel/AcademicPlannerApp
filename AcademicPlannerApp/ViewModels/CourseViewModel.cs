using CommunityToolkit.Mvvm.ComponentModel;
using AcademicPlannerApp.Data.Models;

namespace AcademicPlannerApp.ViewModels
{
    public partial class CourseViewModel : ObservableObject
    {
        // Propiedad que expone el modelo subyacente
        public Course Course { get; private set; }

        public int Id => Course.Id; // Esta es una propiedad de solo lectura que obtiene el ID del modelo subyacente. No necesita ser observable.

        // Esta es la propiedad Observable que se enlaza a la UI
        [ObservableProperty]
        string? name; // Esto generará automáticamente la propiedad pública 'Name'

        [ObservableProperty]
        string? code;

        [ObservableProperty]
        string? description;

        public CourseViewModel(Course course)
        {
            Course = course;
            Name = course.Name; // Asigna al 'Name' (la propiedad generada por ObservableProperty)
            Code = course.Code;
            Description = course.Description;
        }

        // Métodos parciales generados automáticamente por ObservableProperty
        // Se llaman cuando la propiedad 'Name' (la generada) cambia.
        partial void OnNameChanged(string? value) => Course.Name = value;
        partial void OnCodeChanged(string? value) => Course.Code = value;
        partial void OnDescriptionChanged(string? value) => Course.Description = value;
    }
}
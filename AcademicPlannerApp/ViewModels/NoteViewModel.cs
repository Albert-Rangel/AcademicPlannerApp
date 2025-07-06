using CommunityToolkit.Mvvm.ComponentModel;
using AcademicPlannerApp.Data.Models;
using System;

namespace AcademicPlannerApp.ViewModels
{
    public partial class NoteViewModel : ObservableObject
    {
        public Note Note { get; private set; }

        public int Id => Note.Id;

        [ObservableProperty]
        string? title;

        [ObservableProperty]
        string? content;

        [ObservableProperty]
        DateTime createdDate; // No es necesario que sea observable si no se edita desde la UI

        public NoteViewModel(Note note)
        {
            Note = note;
            Title = note.Title;
            Content = note.Content;
            CreatedDate = note.CreatedDate;
        }

        partial void OnTitleChanged(string? value) => Note.Title = value;
        partial void OnContentChanged(string? value) => Note.Content = value;
        // No hay OnCreatedDateChanged porque generalmente no se cambia después de la creación
    }
}
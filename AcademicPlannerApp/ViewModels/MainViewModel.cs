using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using AcademicPlannerApp.Data.Models;
using AcademicPlannerApp.Data.Services;
using AcademicPlannerApp.Services;
using System.Linq;
using System;
using System.Collections.Generic;

namespace AcademicPlannerApp.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly DataAccessService _dataService;
        private readonly IDialogService _dialogService;

        // Colecciones para la UI
        public ObservableCollection<CourseViewModel> Courses { get; }
        public ObservableCollection<AssignmentViewModel> Assignments { get; }
        public ObservableCollection<NoteViewModel> Notes { get; }

        // Listas para almacenar todos los datos cargados (para filtrado)
        private List<Assignment> AllAssignments { get; set; } = new List<Assignment>();
        private List<Note> AllNotes { get; set; } = new List<Note>();

        // Propiedades seleccionadas
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCourseCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteCourseCommand))]
        [NotifyCanExecuteChangedFor(nameof(NewAssignmentCommand))]
        [NotifyCanExecuteChangedFor(nameof(NewNoteCommand))]
        CourseViewModel? selectedCourse;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveAssignmentCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteAssignmentCommand))]
        AssignmentViewModel? selectedAssignment;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveNoteCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteNoteCommand))]
        NoteViewModel? selectedNote;


        public MainViewModel(DataAccessService dataService, IDialogService dialogService)
        {
            _dataService = dataService;
            _dialogService = dialogService;

            Title = "Planificador Académico"; // Título de la página

            Courses = new ObservableCollection<CourseViewModel>();
            Assignments = new ObservableCollection<AssignmentViewModel>();
            Notes = new ObservableCollection<NoteViewModel>();

            // Cargar datos al iniciar el ViewModel (o puedes usar un comando LoadDataAsync en el constructor de la Page)
            // LoadDataCommand.Execute(null); // O llamar a LoadDataAsync() directamente
        }

        // --- Comandos ---

        [RelayCommand]
        private async Task LoadDataAsync()
        {
            IsBusy = true;
            try
            {
                Courses.Clear();
                Assignments.Clear();
                Notes.Clear();

                var courses = await _dataService.GetCoursesAsync();
                foreach (var c in courses)
                {
                    Courses.Add(new CourseViewModel(c));
                }

                AllAssignments = await _dataService.GetAssignmentsAsync();
                AllNotes = await _dataService.GetNotesAsync();

                LoadAssignmentsForSelectedCourse();
                LoadNotesForSelectedCourse();
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlert("Error", $"Error al cargar datos: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void NewCourse()
        {
            SelectedCourse = new CourseViewModel(new Course { Name = "Nuevo Curso" });
        }

        [RelayCommand(CanExecute = nameof(CanExecuteSaveCourse))]
        private async Task SaveCourseAsync()
        {
            IsBusy = true;
            try
            {
                if (SelectedCourse == null) return;
                await _dataService.SaveCourseAsync(SelectedCourse.Course);
                await LoadDataAsync(); // Recargar todo para actualizar las listas
                SelectedCourse = Courses.FirstOrDefault(c => c.Id == SelectedCourse.Id); // Volver a seleccionar por ID si se generó uno nuevo
                if (SelectedCourse == null && Courses.Any()) SelectedCourse = Courses.First();
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlert("Error", $"Error al guardar curso: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        private bool CanExecuteSaveCourse() => SelectedCourse != null && !IsBusy;


        [RelayCommand(CanExecute = nameof(CanExecuteDeleteCourse))]
        private async Task DeleteCourseAsync()
        {
            IsBusy = true;
            try
            {
                if (SelectedCourse == null || SelectedCourse.Id == 0) return;

                bool confirm = await _dialogService.DisplayConfirmation("Confirmar", "¿Estás seguro de que quieres eliminar este curso y sus asignaciones/notas?", "Sí", "No");
                if (confirm)
                {
                    await _dataService.DeleteCourseAsync(SelectedCourse.Id);
                    await LoadDataAsync();
                    SelectedCourse = null; // Deseleccionar
                }
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlert("Error", $"Error al eliminar curso: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        private bool CanExecuteDeleteCourse() => SelectedCourse != null && SelectedCourse.Id != 0 && !IsBusy;


        [RelayCommand(CanExecute = nameof(CanExecuteNewAssignment))]
        private void NewAssignment()
        {
            SelectedAssignment = new AssignmentViewModel(new Assignment
            {
                DueDate = DateTime.Today,
                Status = AssignmentStatus.Pending,
                Priority = AssignmentPriority.Low,
                Title = "Nueva Tarea",
                CourseId = SelectedCourse?.Id // Asocia al curso seleccionado
            });
        }
        private bool CanExecuteNewAssignment() => SelectedCourse != null && !IsBusy;


        [RelayCommand(CanExecute = nameof(CanExecuteSaveAssignment))]
        private async Task SaveAssignmentAsync()
        {
            IsBusy = true;
            try
            {
                if (SelectedAssignment == null) return;
                SelectedAssignment.Assignment.CourseId = SelectedCourse?.Id; // Asegura que el CourseId se asigne
                await _dataService.SaveAssignmentAsync(SelectedAssignment.Assignment);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlert("Error", $"Error al guardar tarea: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        private bool CanExecuteSaveAssignment() => SelectedAssignment != null && !IsBusy;


        [RelayCommand(CanExecute = nameof(CanExecuteDeleteAssignment))]
        private async Task DeleteAssignmentAsync()
        {
            IsBusy = true;
            try
            {
                if (SelectedAssignment == null || SelectedAssignment.Id == 0) return;
                bool confirm = await _dialogService.DisplayConfirmation("Confirmar", "¿Estás seguro de que quieres eliminar esta tarea?", "Sí", "No");
                if (confirm)
                {
                    await _dataService.DeleteAssignmentAsync(SelectedAssignment.Id);
                    await LoadDataAsync();
                    SelectedAssignment = null;
                }
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlert("Error", $"Error al eliminar tarea: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        private bool CanExecuteDeleteAssignment() => SelectedAssignment != null && SelectedAssignment.Id != 0 && !IsBusy;


        [RelayCommand(CanExecute = nameof(CanExecuteNewNote))]
        private void NewNote()
        {
            SelectedNote = new NoteViewModel(new Note
            {
                CreatedDate = DateTime.Now,
                Title = "Nueva Nota",
                CourseId = SelectedCourse?.Id // Asocia a la nota seleccionada
            });
        }
        private bool CanExecuteNewNote() => SelectedCourse != null && !IsBusy;


        [RelayCommand(CanExecute = nameof(CanExecuteSaveNote))]
        private async Task SaveNoteAsync()
        {
            IsBusy = true;
            try
            {
                if (SelectedNote == null) return;
                SelectedNote.Note.CourseId = SelectedCourse?.Id; // Asegura que el CourseId se asigne
                await _dataService.SaveNoteAsync(SelectedNote.Note);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlert("Error", $"Error al guardar nota: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        private bool CanExecuteSaveNote() => SelectedNote != null && !IsBusy;


        [RelayCommand(CanExecute = nameof(CanExecuteDeleteNote))]
        private async Task DeleteNoteAsync()
        {
            IsBusy = true;
            try
            {
                if (SelectedNote == null || SelectedNote.Id == 0) return;
                bool confirm = await _dialogService.DisplayConfirmation("Confirmar", "¿Estás seguro de que quieres eliminar esta nota?", "Sí", "No");
                if (confirm)
                {
                    await _dataService.DeleteNoteAsync(SelectedNote.Id);
                    await LoadDataAsync();
                    SelectedNote = null;
                }
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlert("Error", $"Error al eliminar nota: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        private bool CanExecuteDeleteNote() => SelectedNote != null && SelectedNote.Id != 0 && !IsBusy;


        // Cuando cambia el curso seleccionado, actualiza las listas de asignaciones y notas
        partial void OnSelectedCourseChanged(CourseViewModel? oldValue, CourseViewModel? newValue)
        {
            LoadAssignmentsForSelectedCourse();
            LoadNotesForSelectedCourse();
            // Limpiar selecciones de asignación y nota si el curso cambia
            SelectedAssignment = null;
            SelectedNote = null;
        }

        private void LoadAssignmentsForSelectedCourse()
        {
            Assignments.Clear();
            if (SelectedCourse != null)
            {
                foreach (var assignment in AllAssignments.Where(a => a.CourseId == SelectedCourse.Id))
                {
                    Assignments.Add(new AssignmentViewModel(assignment));
                }
            }
        }

        private void LoadNotesForSelectedCourse()
        {
            Notes.Clear();
            if (SelectedCourse != null)
            {
                foreach (var note in AllNotes.Where(n => n.CourseId == SelectedCourse.Id))
                {
                    Notes.Add(new NoteViewModel(note));
                }
            }
        }

        // Opcional: Para cargar al inicio la primera vez que se accede al ViewModel.
        // Esto es más común si el ViewModel no se crea en el constructor de la Page.
        // [RelayCommand] // Puedes agregar esto si quieres un comando explícito para la carga inicial.
        // public async Task OnAppearingAsync()
        // {
        //     if (Courses.Count == 0) // Cargar solo si está vacío
        //     {
        //         await LoadDataAsync();
        //     }
        // }
    }
}
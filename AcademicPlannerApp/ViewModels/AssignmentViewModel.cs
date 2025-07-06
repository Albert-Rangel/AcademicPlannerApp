using CommunityToolkit.Mvvm.ComponentModel;
using AcademicPlannerApp.Data.Models;
using System;
using Microsoft.VisualBasic;

namespace AcademicPlannerApp.ViewModels
{
    public partial class AssignmentViewModel : ObservableObject
    {
        public Assignment Assignment { get; private set; }

        public int Id => Assignment.Id;

        [ObservableProperty]
        string? title;

        [ObservableProperty]
        string? description;

        [ObservableProperty]
        DateTime dueDate;

        [ObservableProperty]
        AssignmentStatus status;

        [ObservableProperty]
        AssignmentPriority priority;

        // No es necesario que CourseId y Course sean ObservableProperty aquí,
        // ya que se manejan a través de la selección del curso en MainViewModel.
        // public int? CourseId => Assignment.CourseId;
        // public Course? Course => Assignment.Course;

        public AssignmentViewModel(Assignment assignment)
        {
            Assignment = assignment;
            Title = assignment.Title;
            Description = assignment.Description;
            DueDate = assignment.DueDate;
            Status = assignment.Status;
            Priority = assignment.Priority;
        }

        partial void OnTitleChanged(string? value) => Assignment.Title = value;
        partial void OnDescriptionChanged(string? value) => Assignment.Description = value;
        partial void OnDueDateChanged(DateTime value) => Assignment.DueDate = value;
        partial void OnStatusChanged(AssignmentStatus value) => Assignment.Status = value;
        partial void OnPriorityChanged(AssignmentPriority value) => Assignment.Priority = value;
    }
}
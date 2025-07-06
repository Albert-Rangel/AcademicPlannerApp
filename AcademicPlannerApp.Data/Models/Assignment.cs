using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicPlannerApp.Data.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public AssignmentStatus Status { get; set; }
        public AssignmentPriority Priority { get; set; }

        // Clave foránea para el curso (puede ser nulo si la asignación no tiene curso)
        public int? CourseId { get; set; }

        // Propiedad de navegación (opcional)
        public Course? Course { get; set; }
    }
}

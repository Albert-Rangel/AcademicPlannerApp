using System.Collections.Generic; // Para las colecciones de navegación

namespace AcademicPlannerApp.Data.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; } // Puede ser nulo
        public string? Description { get; set; } // Puede ser nulo

        // Propiedades de navegación (opcional si no las vas a cargar directamente)
        // Son útiles para la lógica de la aplicación
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        public ICollection<Note> Notes { get; set; } = new List<Note>();
    }
}
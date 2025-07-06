using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace AcademicPlannerApp.Data.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public DateTime CreatedDate { get; set; }

        // Clave foránea para el curso (puede ser nulo)
        public int? CourseId { get; set; }

        // Propiedad de navegación (opcional)
        public Course? Course { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicPlannerApp.Data.Models
{
    public enum AssignmentStatus
    {
        Pending,
        InProgress,
        Completed,
        Deferred
    }

    public enum AssignmentPriority
    {
        Low,
        Medium,
        High,
        Critical
    }
}

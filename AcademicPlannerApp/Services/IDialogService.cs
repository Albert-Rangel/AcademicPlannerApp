using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicPlannerApp.Services
{
    public interface IDialogService
    {
        Task DisplayAlert(string title, string message, string cancel);
        Task<bool> DisplayConfirmation(string title, string message, string accept, string cancel);
    }
}
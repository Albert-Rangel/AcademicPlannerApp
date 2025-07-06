using AcademicPlannerApp.Services;
using Microsoft.Maui.Controls; // Necesario para Application.Current.MainPage

namespace AcademicPlannerApp.Services
{
    public class DialogService : IDialogService
    {
        public Task DisplayAlert(string title, string message, string cancel)
        {
            // Asegúrate de que estemos en el hilo de la UI
            return Application.Current.MainPage.DisplayAlert(title, message, cancel);
        }

        //public Task<bool> DisplayConfirmation(string title, string message, string accept, string cancel)
        //{
        //    // Asegúrate de que estemos en el hilo de la UI
        //    return Application.Current.MainPage.DisplayActionSheet(title, cancel, accept); // DisplayActionSheet es más flexible
        //}
        public async Task<bool> DisplayConfirmation(string title, string message, string accept, string cancel)
        {
            // DisplayActionSheet devuelve el texto del botón presionado.
            // Necesitamos comparar ese texto con el valor de 'accept'.
            string result = await Application.Current.MainPage.DisplayActionSheet(title, cancel, accept);

            // Retorna true si el resultado es el texto del botón 'accept', de lo contrario false.
            return result == accept;
        }
    }
}
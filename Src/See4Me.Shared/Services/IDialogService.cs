using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Services
{
    public interface IDialogService
    {
        Task ShowAsync(string message, string title = null, string buttonText = null, DialogIcon icon = DialogIcon.Information);

        Task<bool> AskAsync(string message, string title = null, string buttonConfirmText = null, string buttonCancelText = null, DialogIcon icon = DialogIcon.Question);
    }

    public enum DialogIcon
    {
        None,
        Information,
        Question,
        Warning,
        Error
    }
}

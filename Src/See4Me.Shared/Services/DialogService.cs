using See4Me.Localization.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Services
{
    public class DialogService : IDialogService
    {
        private GalaSoft.MvvmLight.Views.DialogService dialogService;

        public DialogService()
        {
            dialogService = new GalaSoft.MvvmLight.Views.DialogService();
        }

        public Task ShowAsync(string message, string title = null, string buttonText = null, DialogIcon icon = DialogIcon.Question)
            => dialogService.ShowMessage(message, title ?? AppResources.ApplicationTitle, buttonText ?? AppResources.OK, null);

        public Task<bool> AskAsync(string message, string title = null, string buttonConfirmText = null, string buttonCancelText = null, DialogIcon icon = DialogIcon.Information)
        {
            var tcs = new TaskCompletionSource<bool>();

            dialogService.ShowMessage(message, title ?? AppResources.ApplicationTitle,
                buttonConfirmText ?? AppResources.Yes, buttonCancelText ?? AppResources.No,
                (result) => { tcs.TrySetResult(result); });

            return tcs.Task;
        }
    }
}

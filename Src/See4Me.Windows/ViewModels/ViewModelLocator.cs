using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Microsoft.ProjectOxford.Vision;
using See4Me.Services;
using System.Globalization;
using System.Threading.Tasks;

namespace See4Me.ViewModels
{
    public partial class ViewModelLocator
    {
        public static async Task ResumeAsync()
        {
            var mainViewModel = ServiceLocator.Current.GetInstance<MainViewModel>();
            await mainViewModel.InitializeStreamingAsync();
        }
    }
}

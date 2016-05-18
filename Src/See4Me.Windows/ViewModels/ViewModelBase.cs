using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;

namespace See4Me.ViewModels
{
    public abstract partial class ViewModelBase : INavigable
    {       
        public virtual Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
            => Task.CompletedTask;

        public virtual Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            MessengerInstance.Unregister(this);
            return Task.CompletedTask;
        }

        public virtual Task OnNavigatingFromAsync(NavigatingEventArgs args)
            => Task.CompletedTask;

        [JsonIgnore]
        public Template10.Services.NavigationService.INavigationService NavigationService { get; set; }

        [JsonIgnore]
        public Template10.Common.IDispatcherWrapper Dispatcher { get; set; }

        [JsonIgnore]
        public Template10.Common.IStateItems SessionState { get; set; }
    }
}
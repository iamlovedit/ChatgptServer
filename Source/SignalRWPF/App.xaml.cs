using Microsoft.AspNetCore.SignalR.Client;
using Prism.Ioc;
using SignalRWPF.Views;
using System.Windows;

namespace SignalRWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<HubConnection>(() => new HubConnectionBuilder().WithUrl("http://localhost:5000/chat").Build());
        }
    }
}

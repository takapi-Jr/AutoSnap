using AutoSnap.Models;
using AutoSnap.ViewModels;
using AutoSnap.Views;
using Prism;
using Prism.Ioc;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace AutoSnap
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnSleep()
        {
            base.OnSleep();

            //リソース解放処理を発行
            MessagingCenter.Send<LifeCyclePayload>(new LifeCyclePayload { Status = LifeCycle.OnSleep }, "");
        }

        protected override void OnResume()
        {
            base.OnResume();

            //リソース初期化処理を発行
            MessagingCenter.Send<LifeCyclePayload>(new LifeCyclePayload { Status = LifeCycle.OnResume }, "");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<AutoSnapPage, AutoSnapPageViewModel>();
        }
    }
}

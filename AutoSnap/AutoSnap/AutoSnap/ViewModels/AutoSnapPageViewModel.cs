using AutoSnap.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoSnap.ViewModels
{
    public class AutoSnapPageViewModel : ViewModelBase
    {
        public ReactiveProperty<int> ShutterFps { get; } = new ReactiveProperty<int>(1);
        public ReactiveProperty<CameraOptions> CameraOption { get; } = new ReactiveProperty<CameraOptions>(CameraOptions.Rear);

        public AutoSnapPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Auto Snap Page";
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            ShutterFps.Value = parameters.GetValue<int>("ShutterFps");
            CameraOption.Value = parameters.GetValue<CameraOptions>("CameraOption");
        }
    }
}

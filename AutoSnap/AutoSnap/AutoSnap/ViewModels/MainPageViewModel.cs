using AutoSnap.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;

namespace AutoSnap.ViewModels
{
    public class MainPageViewModel : ViewModelBase, IDisposable
    {
        public ReactiveProperty<double> ShutterFps { get; } = new ReactiveProperty<double>(1.0f);
        public ReactiveProperty<bool> IsRearCamera { get; } = new ReactiveProperty<bool>(true);
        public CameraOptions CameraOption { get; set; } = CameraOptions.Rear;

        public AsyncReactiveCommand StartAutoSnapCommand { get; } = new AsyncReactiveCommand();

        private CompositeDisposable Disposable { get; } = new CompositeDisposable();



        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Main Page";

            IsRearCamera.Subscribe(isRearCamera =>
            {
                CameraOption = isRearCamera ? CameraOptions.Rear : CameraOptions.Front;
            }).AddTo(this.Disposable);

            StartAutoSnapCommand.Subscribe(async () =>
            {
                // パーミッションチェック
                var grantedFlag = await Common.CheckPermissions(Common.TakePhotoPermissions);
                if (!grantedFlag)
                {
                    return;
                }

                var navigationParameters = new NavigationParameters()
                {
                    //{ "キー", 値 },
                    { "ShutterFps", ShutterFps.Value },
                    { "CameraOption", CameraOption },
                };
                await this.NavigationService.NavigateAsync("AutoSnapPage", navigationParameters);
            }).AddTo(this.Disposable);
        }

        public void Dispose()
        {
            this.Disposable.Dispose();
        }
    }
}

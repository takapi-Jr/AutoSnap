using AutoSnap.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AutoSnap.Controls
{
    public class AutoSnapView : View
    {
        // シャッタースピード
        public static readonly BindableProperty ShutterFpsProperty = BindableProperty.Create(
            propertyName: "ShutterFps",
            returnType: typeof(int),
            declaringType: typeof(AutoSnapView),
            defaultValue: 1);

        public int ShutterFps
        {
            get { return (int)GetValue(ShutterFpsProperty); }
            set { SetValue(ShutterFpsProperty, value); }
        }

        // リア、フロント設定
        public static readonly BindableProperty CameraOptionProperty = BindableProperty.Create(
            propertyName: "CameraOption",
            returnType: typeof(CameraOptions),
            declaringType: typeof(AutoSnapView),
            defaultValue: CameraOptions.Rear);

        public CameraOptions CameraOption
        {
            get { return (CameraOptions)GetValue(CameraOptionProperty); }
            set { SetValue(CameraOptionProperty, value); }
        }

        //プレビュー操作用
        public static readonly BindableProperty IsPreviewingProperty = BindableProperty.Create(
            propertyName: "IsPreviewing",
            returnType: typeof(bool),
            declaringType: typeof(AutoSnapView),
            defaultValue: false);

        public bool IsPreviewing
        {
            get { return (bool)GetValue(IsPreviewingProperty); }
            set { SetValue(IsPreviewingProperty, value); }
        }
    }
}

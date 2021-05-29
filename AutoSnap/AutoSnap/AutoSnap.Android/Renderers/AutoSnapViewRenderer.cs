using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AutoSnap.Controls;
using AutoSnap.Droid.Renderers;
using AutoSnap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AutoSnapView), typeof(AutoSnapViewRenderer))]
namespace AutoSnap.Droid.Renderers
{
    public class AutoSnapViewRenderer : ViewRenderer<AutoSnapView, DroidAutoSnapView>
    {
        private DroidAutoSnapView DroidAutoSnapView { get; set; }

        public AutoSnapViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<AutoSnapView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
            }
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    this.DroidAutoSnapView = new DroidAutoSnapView(Context, e.NewElement);
                    SetNativeControl(this.DroidAutoSnapView);
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (this.Element == null || this.Control == null)
            {
                return;
            }

            // PCL側(Element)の変更をAndroid/iOSプラットフォーム(Control)に反映
            if (e.PropertyName == nameof(Element.IsPreviewing))
            {
                if (this.Control != null)
                {
                    // AutoSnapPageのビハインドコードのDisappearing/Appearingでの変更を反映
                    this.Control.FormsAutoSnapView.IsPreviewing = this.Element.IsPreviewing;
                    this.Control.IsPreviewing = this.Element.IsPreviewing;
                }
            }
            if (e.PropertyName == nameof(Element.ShutterFps))
            {
                if (this.Control != null)
                {
                    this.Control.FormsAutoSnapView.ShutterFps = this.Element.ShutterFps;
                    this.Control.ShutterFps = this.Element.ShutterFps;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Control.Release();
                MessagingCenter.Unsubscribe<LifeCyclePayload>(this.Control, "");

                // 下記実行すると、base.Dispose(disposing);で例外発生
                //Control.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}

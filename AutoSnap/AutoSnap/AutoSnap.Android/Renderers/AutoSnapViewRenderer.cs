#if false
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AutoSnap.Controls;
using AutoSnap.Droid.Renderers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.FastRenderers;

[assembly: ExportRenderer(typeof(AutoSnapView), typeof(AutoSnapViewRenderer))]
namespace AutoSnap.Droid.Renderers
{
    public class AutoSnapViewRenderer : FrameLayout, IVisualElementRenderer, IViewRenderer
    {
        public int? DefaultLabelFor { get; set; }
        public bool Disposed { get; set; }

        public VisualElementTracker VisualElementTracker { get; set; }
        public VisualElementRenderer VisualElementRenderer { get; set; }
        
        public AutoSnapFragment AutoSnapFragment { get; set; }

        private AutoSnapView element;
        public AutoSnapView Element
        {
            get { return element; }
            set
            {
                if (element == value)
                {
                    return;
                }
                var oldElement = element;
                element = value;
                OnElementChanged(new ElementChangedEventArgs<AutoSnapView>(oldElement, element));
            }
        }

        private FragmentManager fragmentManager;
        public FragmentManager FragmentManager => fragmentManager ??= Context.GetFragmentManager();

        public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
        public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;



        public AutoSnapViewRenderer(Context context)
            : base(context)
        {
             = ;
        }





#region IViewRenderer

        void IViewRenderer.MeasureExactly() => MeasureExactly(this, Element, Context);

        public void MeasureExactly(Android.Views.View control, VisualElement element, Context context)
        {
            if (control == null || element == null)
            {
                return;
            }

            var width = element.Width;
            var height = element.Height;

            if (width <= 0 || height <= 0)
            {
                return;
            }

            var realWidth = (int)context.ToPixels(width);
            var realHeight = (int)context.ToPixels(height);

            var widthMeasureSpec = MeasureSpecFactory.MakeMeasureSpec(realWidth, MeasureSpecMode.Exactly);
            var heightMeasureSpec = MeasureSpecFactory.MakeMeasureSpec(realHeight, MeasureSpecMode.Exactly);

            control.Measure(widthMeasureSpec, heightMeasureSpec);
        }

#endregion


#region IVisualElementRenderer

        VisualElement IVisualElementRenderer.Element => Element;
        VisualElementTracker IVisualElementRenderer.Tracker => VisualElementTracker;
        ViewGroup IVisualElementRenderer.ViewGroup => null;
        Android.Views.View IVisualElementRenderer.View => this;

        public SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            Measure(widthConstraint, heightConstraint);
            var result = new SizeRequest(new Size(MeasuredWidth, MeasuredHeight), new Size(Context.ToPixels(20), Context.ToPixels(20)));
            return result;
        }

        public void SetElement(VisualElement element)
        {
            if (!(element is AutoSnapView autoSnapView))
            {
                throw new ArgumentException($"{nameof(element)} must be of type {nameof(AutoSnapView)}");
            }

            if (this.VisualElementTracker == null)
            {
                this.VisualElementTracker = new VisualElementTracker(this);
            }
            this.Element = autoSnapView;
        }

        public void SetLabelFor(int? id)
        {
            if (this.DefaultLabelFor == null)
            {
                this.DefaultLabelFor = LabelFor;
            }
            LabelFor = (int)(id ?? this.DefaultLabelFor);
        }

        public void UpdateLayout()
        {
            this.VisualElementTracker?.UpdateLayout();
        }

#endregion



        static class MeasureSpecFactory
        {
            public static int GetSize(int measureSpec)
            {
                const int modeMask = 0x3 << 30;
                return measureSpec & ~modeMask;
            }

            public static int MakeMeasureSpec(int size, MeasureSpecMode mode)
            {
                return size + (int)mode;
            }
        }
    }
}
#endif
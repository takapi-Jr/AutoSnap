using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AutoSnap.Controls;
using AutoSnap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace AutoSnap.Droid.Renderers
{
    public class DroidAutoSnapView : ViewGroup, ISurfaceHolderCallback
    {
        private SurfaceView SurfaceView { get; set; }
        private ISurfaceHolder Holder { get; set; }
        private Camera.Size PreviewSize { get; set; }
        private IList<Camera.Size> SupportedPreviewSizes { get; set; }
        private byte[] Buff { get; set; }
        private bool IsSurfaceCreated { get; set; }

        public CameraPreviewCallback PreviewCallback { get; set; }
        public AutoSnapView FormsAutoSnapView { get; set; }
        public double ShutterFps { get; set; }
        public int PreviewFrameRate { get; set; }

        private bool isPreviewing;
        public bool IsPreviewing
        {
            get { return isPreviewing; }
            set
            {
                if (value)
                {
                    StartPreview();
                }
                else
                {
                    StopPreview();
                }
                isPreviewing = value;
            }
        }

        private Camera previewCamera;
        public Camera PreviewCamera
        {
            get { return previewCamera; }
            set
            {
                previewCamera = value;
                if (previewCamera != null)
                {
                    this.SupportedPreviewSizes = this.PreviewCamera.GetParameters().SupportedPreviewSizes;
                    RequestLayout();
                }
            }
        }



        public DroidAutoSnapView(Context context, AutoSnapView formsAutoSnapView)
            : base(context)
        {
            // カメラプレビュー
            this.FormsAutoSnapView = formsAutoSnapView;
            this.SurfaceView = new SurfaceView(context);
            AddView(this.SurfaceView);

            // オーバーレイビュー
            //overlayView = new QRScannerOverlayView(context);
            //AddView(overlayView);

            // Install a SurfaceHolder.Callback so we get notified when the
            // underlying surface is created and destroyed.
            this.isPreviewing = this.FormsAutoSnapView.IsPreviewing;
            this.Holder = this.SurfaceView.Holder;
            this.Holder.AddCallback(this);
            //this.Holder.SetType(SurfaceType.PushBuffers);

            //this.context = context;

            MessagingCenter.Subscribe<LifeCyclePayload>(this, "", (p) => {
                switch (p.Status)
                {
                    case LifeCycle.OnSleep:
                        if (this.IsSurfaceCreated)
                        {
                            //Sleepの時にSurfaceViewが生成されていればリソース解放
                            Release();
                        }
                        break;
                    case LifeCycle.OnResume:
                        if (this.IsSurfaceCreated)
                        {
                            //Resumeの時にSurfaceViewが生成されていればリソース初期化
                            Initialize();
                        }
                        break;
                }
            });
        }

        private void StartPreview()
        {
            if (this.PreviewCamera != null)
            {
                this.PreviewCamera.StartPreview();
                this.PreviewCamera.SetPreviewCallbackWithBuffer(this.PreviewCallback);  //画面移動するとコールバックが無効になるので再セット
                this.PreviewCamera.AddCallbackBuffer(Buff);
            }
        }

        private void StopPreview()
        {
            if (this.PreviewCamera != null)
            {
                this.PreviewCamera.AddCallbackBuffer(null);
                this.PreviewCamera.SetPreviewCallbackWithBuffer(null);
                this.PreviewCamera.StopPreview();
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            //@@@@@@@@@
            //base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            // We purposely disregard child measurements because act as a
            // wrapper to a SurfaceView that centers the camera preview instead
            // of stretching it.
            var width = ResolveSize(SuggestedMinimumWidth, widthMeasureSpec);
            var height = ResolveSize(SuggestedMinimumHeight, heightMeasureSpec);
            SetMeasuredDimension(width, height);

            //@@@@@@@@@
            // TODO: 不要なら削除
            if (this.SupportedPreviewSizes != null)
            {
                this.PreviewSize = GetOptimalPreviewSize(this.SupportedPreviewSizes, width, height);
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            if (changed && ChildCount > 0)
            {
                var child = GetChildAt(0);

                var width = r - l;
                var height = b - t;

                var previewWidth = width;
                var previewHeight = height;
                if (this.PreviewSize != null)
                {
                    previewWidth = this.PreviewSize.Width;
                    previewHeight = this.PreviewSize.Height;
                }

                // Center the child SurfaceView within the parent.
                if (width * previewHeight > height * previewWidth)
                {
                    var scaledChildWidth = previewWidth * height / previewHeight;
                    child.Layout((width - scaledChildWidth) / 2, 0,
                                 (width + scaledChildWidth) / 2, height);
                }
                else
                {
                    var scaledChildHeight = previewHeight * width / previewWidth;
                    child.Layout(0, (height - scaledChildHeight) / 2,
                                 width, (height + scaledChildHeight) / 2);
                }
            }

            //@@@@@@@@@
            //var width = r - l;
            //var height = b - t;

            //// カメラプレビューのレイアウト
            //var msw = MeasureSpec.MakeMeasureSpec((int)(width * 0.6f), MeasureSpecMode.Exactly);
            //var msh = MeasureSpec.MakeMeasureSpec((int)(height * 0.6f), MeasureSpecMode.Exactly);
            //surfaceView.Measure(msw, msh);
            //surfaceView.Layout((int)(width * 0.2f), (int)(width * 0.2f), (int)(width * 0.8f), (int)(width * 0.8f));

            //// オーバーレイビューのレイアウト
            //msw = MeasureSpec.MakeMeasureSpec(width, MeasureSpecMode.Exactly);
            //msh = MeasureSpec.MakeMeasureSpec(height, MeasureSpecMode.Exactly);
            //overlayView.Measure(msw, msh);
            //overlayView.Layout(0, 0, width, height);
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            // The Surface has been created, acquire the camera and tell it where
            // to draw.
            this.IsSurfaceCreated = true;
            Initialize();
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Android.Graphics.Format format, int width, int height)
        {
            // Now that the size is known, set up the camera parameters and begin
            // the preview.
            if (PreviewCamera == null)
            {
                Initialize();
            }
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            // Surface will be destroyed when we return, so stop the preview.
            if (this.PreviewCamera != null)
            {
                Release();
            }
            this.IsSurfaceCreated = false;
        }

        public void Initialize()
        {
            this.PreviewCamera = Camera.Open((int)FormsAutoSnapView.CameraOption);

            //Portrait固定
            this.PreviewCamera.SetDisplayOrientation(90);

            //プレビューサイズ設定
            if (this.SupportedPreviewSizes != null)
            {
                this.PreviewSize = GetOptimalPreviewSize(this.SupportedPreviewSizes, this.SurfaceView.Width, this.SurfaceView.Height);
            }

            var parameters = this.PreviewCamera.GetParameters();
            parameters.SetPreviewSize(this.PreviewSize.Width, this.PreviewSize.Height);

            //フレームレート設定
            parameters.SetPreviewFpsRange(10000, 24000);      // Androidバージョンによっては設定不可
            this.PreviewFrameRate = parameters.PreviewFrameRate;

            // フォーカスモード設定
            parameters.FocusMode = Camera.Parameters.FocusModeContinuousVideo;

            this.PreviewCamera.SetParameters(parameters);
            RequestLayout();

            //フレーム処理用バッファの作成
            var size = this.PreviewSize.Width * this.PreviewSize.Height * Android.Graphics.ImageFormat.GetBitsPerPixel(Android.Graphics.ImageFormatType.Nv21) / 8;
            this.Buff = new byte[size];
            //フレーム処理用のコールバック生成
            this.PreviewCallback = new CameraPreviewCallback
            {
                Buff = this.Buff,
                ShutterFps = this.ShutterFps,
                PreviewFrameRate = this.PreviewFrameRate,
                //NavigationService = MedCalMaker.ViewModels.MainPageViewModel.NavigationServiceForCustomRenderer,
            };

            this.PreviewCamera.SetPreviewCallbackWithBuffer(this.PreviewCallback);
            this.PreviewCamera.AddCallbackBuffer(this.Buff);

            //@@@@@@@@@
            //ピンチジェスチャー登録処理
            //pinchlistener = new PinchListener { camera = Preview, PreviewCallback = PreviewCallback, buff = Buff };
            //scaleGestureDetector = new ScaleGestureDetector(context, pinchlistener);

            this.PreviewCamera.SetPreviewDisplay(this.Holder);

            if (this.IsPreviewing)
            {
                StartPreview();
            }

            // アプリ内で明るさ調整(上げる)
            var attributesWindow = new WindowManagerLayoutParams();
            var window = Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity.Window;
            attributesWindow.CopyFrom(window.Attributes);
            if (attributesWindow.ScreenBrightness != 1.0f)
            {
                attributesWindow.ScreenBrightness = 1.0f;
                window.Attributes = attributesWindow;
            }
        }

        public void Release()
        {
            // アプリ内で明るさ調整(元に戻す)
            var attributesWindow = new WindowManagerLayoutParams();
            var window = Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity.Window;
            attributesWindow.CopyFrom(window.Attributes);
            if (attributesWindow.ScreenBrightness == 1.0f)
            {
                attributesWindow.ScreenBrightness = -1.0f;
                window.Attributes = attributesWindow;
            }

            // 解放処理
            this.PreviewCamera?.StopPreview();
            PreviewCallback.Dispose();
            this.PreviewCamera?.AddCallbackBuffer(null);
            this.PreviewCamera?.SetPreviewCallbackWithBuffer(null);
            //@@@@@@@@@
            //pinchlistener.Dispose();
            //scaleGestureDetector.Dispose();

            this.PreviewCamera?.Release();
            this.PreviewCamera = null;
        }



        private Camera.Size GetOptimalPreviewSize(IList<Camera.Size> sizes, int w, int h)
        {
            var aspectTolerance = 0.1;
            var targetRatio = (double)w / h;

            if (sizes == null)
            {
                return null;
            }

            Camera.Size optimalSize = null;
            var minDiff = Double.MaxValue;

            var targetHeight = h;

            // Try to find an size match aspect ratio and size
            foreach (var size in sizes)
            {
                var ratio = (double)size.Height / size.Width;    //Portraitは縦横逆

                if (Math.Abs(ratio - targetRatio) > aspectTolerance)
                {
                    continue;
                }

                if (Math.Abs(size.Width - targetHeight) < minDiff)
                {
                    optimalSize = size;
                    minDiff = Math.Abs(size.Width - targetHeight);
                }
            }

            // Cannot find the one match the aspect ratio, ignore the requirement
            if (optimalSize == null)
            {
                minDiff = Double.MaxValue;
                foreach (var size in sizes)
                {
                    if (Math.Abs(size.Width - targetHeight) < minDiff)
                    {
                        optimalSize = size;
                        minDiff = Math.Abs(size.Width - targetHeight);
                    }
                }
            }

            return optimalSize;
        }
    }
}
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AutoSnap.Droid.Renderers
{
    public class CameraPreviewCallback : Java.Lang.Object, Android.Hardware.Camera.IPreviewCallback
    {
        public byte[] Buff { get; set; }
        public double ShutterFps { get; set; }
        public int PreviewFrameRate { get; set; }
        private static int Count { get; set; }

        public async void OnPreviewFrame(byte[] data, Android.Hardware.Camera camera)
        {
            var shutterCount = (int)(this.PreviewFrameRate / this.ShutterFps);
            if (Count == shutterCount)
            {
                //ここでフレーム画像データを加工したり情報を取得したり
                var parameters = camera.GetParameters();
                //await YuvToJpegAsync(data, parameters);

                //次のバッファをセット
                camera.AddCallbackBuffer(Buff);
                Count = 0;
            }
            else
            {
                Count++;
            }
        }

        public async Task YuvToJpegAsync(byte[] data, Android.Hardware.Camera.Parameters parameters)
        {
            var size = parameters.PreviewSize;
            var previewFormat = parameters.PreviewFormat;

            // dataはYuv形式のバイト配列
            var yuvImage = new YuvImage(data, previewFormat, size.Width, size.Height, null);

            using (var stream = new MemoryStream())
            {
                await yuvImage.CompressToJpegAsync(new Android.Graphics.Rect(0, 0, size.Width, size.Height), 100, stream);
                var bytes = stream.ToArray();
            }
        }
    }
}
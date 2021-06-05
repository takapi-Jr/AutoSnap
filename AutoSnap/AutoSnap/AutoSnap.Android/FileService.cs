using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AutoSnap.Droid;
using AutoSnap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileService))]
namespace AutoSnap.Droid
{
    public class FileService : IFileService
    {
        public string GetPicturesFolderPath()
        {
            // /storage/emulated/0/Android/data/PACKAGE_NAME/files/Pictures
            var context = Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity as Context;
            var folder = context.GetExternalFilesDir(Android.OS.Environment.DirectoryPictures).Path;

            return folder;
        }
    }
}
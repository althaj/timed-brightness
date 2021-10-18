using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace TimedBrightness
{
    class AndroidBrightnessService
    {
        /// <summary>
        /// Set the brightness of the device.
        /// </summary>
        /// <param name="brightness">Brightness of the device.</param>
        public void SetBrightness(int brightness)
        {
            Android.Provider.Settings.System.PutInt(MainActivity.mainActivity.ContentResolver, Android.Provider.Settings.System.ScreenBrightness, brightness);
        }

        /// <summary>
        /// Get the device brightness.
        /// </summary>
        public int GetBrightness()
        {
            var brightness = Android.Provider.Settings.System.GetInt(MainActivity.mainActivity.ContentResolver, Android.Provider.Settings.System.ScreenBrightness);
            return brightness;
        }
    }
}
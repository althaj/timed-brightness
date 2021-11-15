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
        List<BrightnessSetting> settings;
        BrightnessSetting currentSetting;

        public AndroidBrightnessService()
        {
            UpdateService();
        }

        /// <summary>
        /// Update the service data.
        /// </summary>
        public void UpdateService()
        {
            settings = DataProvider.LoadData();
            BrightnessSetting currentTime = new BrightnessSetting(DateTime.Now.TimeOfDay);

            if (settings.Count > 0)
            {
                currentSetting = settings.Last();

                for (int i = 0; i < settings.Count; i++)
                {
                    if (settings[i].CompareTo(currentTime) <= 0)
                        currentSetting = settings[i];
                }
            }
            else
            {
                currentSetting = null;
            }

            SetBrightness();
        }

        /// <summary>
        /// Set the brightness of the device.
        /// </summary>
        /// <param name="brightness">Brightness of the device.</param>
        public void SetBrightness(int brightness)
        {
            Android.Provider.Settings.System.PutInt(MainActivity.mainActivity.ContentResolver, Android.Provider.Settings.System.ScreenBrightness, brightness);
        }

        /// <summary>
        /// Set the brightness of the device.
        /// </summary>
        public void SetBrightness()
        {
            if (currentSetting != null)
                SetBrightness(currentSetting.Brightness);
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
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
using System.Timers;
using Xamarin.Essentials;

namespace TimedBrightness
{
    class AndroidBrightnessService
    {
        MainActivity activity;
        List<BrightnessSetting> settings;
        BrightnessSetting currentSetting;
        Timer timer;

        /// <summary>
        /// Create a new instance of the Brightness service.
        /// </summary>
        /// <param name="activity">Main activity.</param>
        public AndroidBrightnessService(MainActivity activity)
        {
            this.activity = activity;
            timer = new Timer();
            timer.Elapsed += Timer_Elapsed;
            UpdateService();
        }

        /// <summary>
        /// Timer event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SetBrightness();
        }

        /// <summary>
        /// Update the service data.
        /// </summary>
        public void UpdateService()
        {
            settings = DataProvider.LoadData();
            SetBrightness();
        }

        /// <summary>
        /// Get the current setting from the settings.
        /// </summary>
        /// <returns>Current setting that should be active currently.</returns>
        private BrightnessSetting GetCurrentSetting()
        {
            if (settings.Count > 0)
            {
                BrightnessSetting currentTime = new BrightnessSetting(DateTime.Now.TimeOfDay);
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
            return currentSetting;
        }

        /// <summary>
        /// Set the brightness of the device.
        /// </summary>
        /// <param name="brightness">Brightness of the device.</param>
        public void SetBrightness(int brightness)
        {
            if (GetBrightness() != brightness)
            {
                Android.Provider.Settings.System.PutInt(MainActivity.mainActivity.ContentResolver, Android.Provider.Settings.System.ScreenBrightness, brightness);
                activity.SendNotification(brightness);
            }

            timer.Stop();
            timer.Interval = 600000;
            timer.Start();
        }

        /// <summary>
        /// Set the brightness of the device.
        /// </summary>
        public void SetBrightness()
        {
            currentSetting = GetCurrentSetting();

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

        /// <summary>
        /// Dispose of the class.
        /// </summary>
        public void Destroy()
        {
            timer.Dispose();
        }
    }
}
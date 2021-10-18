using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using Google.Android.Material.Snackbar;
using Android.Widget;
using System.Collections.Generic;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Button;
using AndroidX.Core.App;
using Android;
using AndroidX.CoordinatorLayout.Widget;
using Android.Content.PM;
using Android.Content;
using Android.Provider;
using Xamarin.Essentials;

namespace TimedBrightness
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public static MainActivity mainActivity;

        List<BrightnessSetting> brightnessSettings = new List<BrightnessSetting>();
        RecyclerView recyclerView;
        RecyclerView.LayoutManager layoutManager;

        private const int REQUEST_PERMISSION_WRITE_SETTINGS = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            mainActivity = this;

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            MaterialButton addNew = FindViewById<MaterialButton>(Resource.Id.buttonAddNew);
            addNew.Click += AddNewOnClick;
            MaterialButton save = FindViewById<MaterialButton>(Resource.Id.buttonSave);
            save.Click += SaveOnClick;

            brightnessSettings = DataProvider.LoadData();

            BrightnessSettingAdapter adapter = new BrightnessSettingAdapter(this, brightnessSettings);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.brightnessList);
            layoutManager = new GridLayoutManager(this, 1, GridLayoutManager.Vertical, false);
            recyclerView.SetLayoutManager(layoutManager);
            recyclerView.SetAdapter(adapter);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void AddNewOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            brightnessSettings.Add(new BrightnessSetting()
            {
                Hour = 12,
                MinuteString = "00",
                Brightness = 128
            });
            recyclerView.GetAdapter().NotifyDataSetChanged();
        }

        private void SaveOnClick(object sender, EventArgs eventArgs)
        {
            DataProvider.SaveData(brightnessSettings);

            if (!Settings.System.CanWrite(this))
            {
                Intent intent = new Intent(Settings.ActionManageWriteSettings);
                intent.SetData(Android.Net.Uri.Parse("package:" + AppInfo.PackageName));
                StartActivity(intent);
            } else
            {
                if (brightnessSettings.Count > 0)
                {
                    AndroidBrightnessService service = new AndroidBrightnessService();

                    Console.WriteLine(service.GetBrightness());

                    service.SetBrightness((int)brightnessSettings[0].Brightness);

                    Console.WriteLine(service.GetBrightness());
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            switch (requestCode)
            {
                case REQUEST_PERMISSION_WRITE_SETTINGS:
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        Toast.MakeText(this, "Permission Granted!", ToastLength.Short).Show();
                    }
                    else
                    {
                        Toast.MakeText(this, "Permission Denied!", ToastLength.Short).Show();
                    }
                    break;
            }
        }
    }
}

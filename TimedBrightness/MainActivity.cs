using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Android.Widget;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Button;

namespace TimedBrightness
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        List<BrightnessSetting> brightnessSettings = new List<BrightnessSetting>();
        RecyclerView recyclerView;
        RecyclerView.LayoutManager layoutManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            MaterialButton addNew = FindViewById<MaterialButton>(Resource.Id.buttonAddNew);
            addNew.Click += AddNewOnClick;

            // Build sample data
            brightnessSettings.Add(new BrightnessSetting()
            {
                Hour = 8,
                MinuteString = "30",
                Brightness = 0.9f
            });

            brightnessSettings.Add(new BrightnessSetting()
            {
                Hour = 20,
                MinuteString = "00",
                Brightness = 0.5f
            });

            brightnessSettings.Add(new BrightnessSetting()
            {
                Hour = 22,
                MinuteString = "30",
                Brightness = 0.2f
            });


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
            View view = (View) sender;
            brightnessSettings.Add(new BrightnessSetting()
            {
                Hour = 12,
                MinuteString = "00",
                Brightness = 0.5f
            });
            recyclerView.GetAdapter().NotifyDataSetChanged();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

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
        AndroidBrightnessService service;

        private const int REQUEST_PERMISSION_WRITE_SETTINGS = 1;

        bool startupDialogDisplayed = false;

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

            service = new AndroidBrightnessService();
        }

        /// <summary>
        /// Handle permission access.
        /// </summary>
        private void HandlePermissions()
        {
            if (!Settings.System.CanWrite(this))
            {
                Snackbar snackbar = Snackbar.Make(FindViewById<View>(Resource.Id.root_view), Resource.String.permission_write_settings_rationale, Snackbar.LengthIndefinite);
                snackbar.View.FindViewById<TextView>(Resource.Id.snackbar_text).SetMaxLines(5);
                snackbar.SetAction(Resource.String.ok, new Action<View>(delegate (View obj)
                {
                    Intent intent = new Intent(Settings.ActionManageWriteSettings);
                    intent.SetData(Android.Net.Uri.Parse("package:" + AppInfo.PackageName));
                    StartActivity(intent);
                }));
                snackbar.Show();
                return;
            }

            PowerManager pm = (PowerManager)Application.Context.GetSystemService(PowerService);
            if (!pm.IsIgnoringBatteryOptimizations(AppInfo.PackageName))
            {
                Snackbar snackbar = Snackbar.Make(FindViewById<View>(Resource.Id.root_view), Resource.String.permission_battery_optimization_rationale, Snackbar.LengthIndefinite);
                snackbar.View.FindViewById<TextView>(Resource.Id.snackbar_text).SetMaxLines(5);
                snackbar.SetAction(Resource.String.ok, new Action<View>(delegate (View obj)
                    {
                        Intent intent = new Intent(Android.Provider.Settings.ActionIgnoreBatteryOptimizationSettings);
                        StartActivity(intent);
                    }));
                snackbar.Show();
                return;
            }

            if (!startupDialogDisplayed)
            {
                startupDialogDisplayed = true;

                Android.App.AlertDialog.Builder startupDialog = new Android.App.AlertDialog.Builder(this);
                startupDialog.SetTitle(Resource.String.startup_dialog_title);
                startupDialog.SetMessage(Resource.String.startup_dialog_message);
                startupDialog.SetPositiveButton(Resource.String.ok, (senderAlert, args) =>
                {
                    Intent intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings);
                    intent.SetData(Android.Net.Uri.Parse("package:" + AppInfo.PackageName));
                    StartActivity(intent);
                });

                startupDialog.SetNegativeButton(Resource.String.dismiss, (senderAlert, args) =>
                {
                });

                Dialog dialog = startupDialog.Create();
                dialog.Show();
            }
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
                HandlePermissions();
            }
            else
            {
                if (brightnessSettings.Count > 0)
                {
                    service.UpdateService();
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

        protected override void OnResume()
        {
            base.OnResume();
            HandlePermissions();
        }
    }
}

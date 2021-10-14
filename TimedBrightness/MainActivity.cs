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
using System.IO;
using System.Xml.Serialization;
using System.Xml;

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
            MaterialButton save = FindViewById<MaterialButton>(Resource.Id.buttonSave);
            save.Click += SaveOnClick;

            LoadData();

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

        private void SaveOnClick(object sender, EventArgs eventArgs)
        {
            SaveData();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        /// <summary>
        /// Load data from an XML file.
        /// </summary>
        private void LoadData()
        {
            var filePath = GetSaveFilePath();
            XmlSerializer serializer = new XmlSerializer(typeof(List<BrightnessSetting>));

            if (!File.Exists(filePath))
            {
                brightnessSettings = new List<BrightnessSetting>();
                SaveData(serializer);
            }
            else
            {
                using (XmlReader reader = XmlReader.Create(filePath))
                {
                    brightnessSettings = (List<BrightnessSetting>)serializer.Deserialize(reader);
                }
            }
        }

        /// <summary>
        /// Save data to an XML file.
        /// </summary>
        /// <param name="serializer">XML serializer to save data.</param>
        private void SaveData(XmlSerializer serializer = null)
        {
            var filePath = GetSaveFilePath();

            if(brightnessSettings == null)
                brightnessSettings = new List<BrightnessSetting>();

            if (serializer == null)
                serializer = new XmlSerializer(typeof(List<BrightnessSetting>));

            TextWriter writer = new StreamWriter(filePath);
            serializer.Serialize(writer, brightnessSettings);
            writer.Close();

        }

        /// <summary>
        /// Get the path to the save file.
        /// </summary>
        /// <returns>Path to the save file.</returns>
        private string GetSaveFilePath()
        {
            var folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            return Path.Combine(folderPath, "save.xml");
        }
    }
}

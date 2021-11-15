using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TimedBrightness
{
    public static class DataProvider
    {
        /// <summary>
        /// Load data from an XML file.
        /// </summary>
        /// <returns>Returns the list of brightness settings.</returns>
        public static List<BrightnessSetting> LoadData()
        {
            List<BrightnessSetting> brightnessSettings = new List<BrightnessSetting>();

            string filePath = GetSaveFilePath();
            XmlSerializer serializer = new XmlSerializer(typeof(List<BrightnessSetting>));

            if (!File.Exists(filePath))
            {
                brightnessSettings = new List<BrightnessSetting>();
                SaveData(brightnessSettings, serializer);
            }
            else
            {
                using (XmlReader reader = XmlReader.Create(filePath))
                {
                    brightnessSettings = (List<BrightnessSetting>)serializer.Deserialize(reader);
                }
            }

            return brightnessSettings;
        }

        /// <summary>
        /// Save data to an XML file.
        /// </summary>
        /// <param name="brightnessSettings">Settings to save.</param>
        /// <param name="serializer">XML serializer to save data.</param>
        public static void SaveData(List<BrightnessSetting> brightnessSettings, XmlSerializer serializer = null)
        {
            var filePath = GetSaveFilePath();

            if (brightnessSettings == null)
                brightnessSettings = new List<BrightnessSetting>();

            brightnessSettings = brightnessSettings.OrderBy(x => x.Minute).OrderBy(x => x.Hour).ToList(); ;

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
        public static string GetSaveFilePath()
        {
            var folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            return Path.Combine(folderPath, "save.xml");
        }
    }
}
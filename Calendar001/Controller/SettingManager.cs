using Calendar001.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Calendar001.Controller
{
    /// <summary>
    /// Setting 값을 관리할 수 있다.
    /// </summary>
    internal class SettingManager
    {
        private static readonly string SETTING_FILE_NAME = "setting.dat";
        private static readonly string SETTING_FILE_DIRECTORY = "data";

        private SettingManager()
        {

        }
        private static Setting currentSetting;
        internal static Setting CurrentSetting {
            get
            {
                if(currentSetting == null)
                {
                    currentSetting = new Setting();
                }
                return currentSetting;
            }
        }

        internal static void SaveCurrentSetting()
        {
            string filePath = System.Windows.Forms.Application.StartupPath + $"\\{SETTING_FILE_DIRECTORY}";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            Stream writeStream = new FileStream(filePath + $"\\{SETTING_FILE_NAME}", FileMode.Create);
            BinaryFormatter serializer = new BinaryFormatter();

            serializer.Serialize(writeStream, CurrentSetting);
            writeStream.Close();
        }

        public static void LoadCurrentSetting()
        {
            string directoryPath = System.Windows.Forms.Application.StartupPath + $"\\{SETTING_FILE_DIRECTORY}";
            string filePath = directoryPath + $"\\{SETTING_FILE_NAME}";

            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            if (!File.Exists(filePath))
            {
                return;
            }
            Stream readStream = new FileStream(filePath, FileMode.Open);
            BinaryFormatter deserializer = new BinaryFormatter();

            try
            {
                currentSetting = (Setting)deserializer.Deserialize(readStream);
            }
            catch (SerializationException e)
            {
                currentSetting = CurrentSetting;
            }
            readStream.Close();
        }
    }
}

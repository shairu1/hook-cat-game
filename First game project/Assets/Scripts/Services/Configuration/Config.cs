using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Services.Configuration
{
    public static class Config
    {
        private const string pathToConfigFile = "Data\\config.json";

        private static ConfigFile _configFile;
        private static ConfigFile _config
        {
            get
            { 
                if (_configFile == null)
                    LoadConfig();

                return _configFile;
            }

            set
            {
                _configFile = value;
            }
        }

        public static bool fullScreen
        {
            get { return Screen.fullScreen; }
            set { Screen.fullScreen = value;  }
        }

        public static float masterVolume 
        {
            get { return _config.masterVolume; }
            set { _config.masterVolume = value; }
        }

        public static float musicVolume
        {
            get { return _config.musicVolume; }
            set { _config.musicVolume = value; }
        }

        public static float soundVolume
        {
            get { return _config.soundVolume; }
            set { _config.soundVolume = value; }
        }


        public static void SaveConfig()
        {
            string path = Path.Combine(Application.dataPath, pathToConfigFile);
            File.WriteAllText(path, JsonUtility.ToJson(_config));
        }

        public static void LoadConfig()
        {
            string path = Path.Combine(Application.dataPath, pathToConfigFile);

            if (File.Exists(path))
            {
                _configFile = JsonUtility.FromJson<ConfigFile>(File.ReadAllText(path));
            }

            if (_configFile == null)
            {
                _configFile = new ConfigFile();
                SaveConfig();
                fullScreen = _configFile.fullScreen;
            }
        }

        [Serializable]
        private class ConfigFile
        {
            public bool fullScreen;
            public float masterVolume;
            public float soundVolume;
            public float musicVolume;

            public ConfigFile()
            {
                this.fullScreen = true;
                this.masterVolume = 100;
                this.musicVolume = 100;
                this.soundVolume = 100;
            }
        }
    }  
}



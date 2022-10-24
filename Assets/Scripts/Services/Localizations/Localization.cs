using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;


namespace Services.Localizations
{
    public static class Localization
    {
        private const string pathToLanguagesFolder = @"Data\Localizations";
        private const string pathToLanguagesFolderInResources = "Localizations/";
        private const string pathToLanguagesInResources = "Localizations/Languages";
        private const string pathToLanguage = @"Data\Localizations\Language.json";


        private static string[] _allLanguages;
        public static string[] allLanguages
        {
            get
            {
                if (_allLanguages == null)
                {
                    Init();
                }

                return (string[]) _allLanguages.Clone();
            }
        }


        private static string _language;
        public static string language
        {
            get
            { 
                if (_language == null || _language == "")
                {
                    Init();
                }

                return _language;
            }
        }


        private static Dictionary<string, string> _words;

        public static void Init()
        {
            string folder = Path.Combine(Application.dataPath, pathToLanguagesFolder);

            if (!Directory.Exists(folder))
            {
                UpdateLanguages();
            }
            else
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folder);

                if (directoryInfo.GetDirectories().Length == 0)
                {
                    UpdateLanguages();
                }
                else
                {
                    List<string> locales = new List<string>();
                    foreach (var directory in directoryInfo.GetDirectories())
                    {
                        if (File.Exists(Path.Combine(Application.dataPath, pathToLanguagesFolder, directory.Name,
                            directory.Name + ".json")))
                        {
                            locales.Add(directory.Name);
                        }
                    }
                    _allLanguages = locales.ToArray(); 
                }           
            }

            LoadLanguage();

            bool fl = false;
            for (int i = 0; i < _allLanguages.Length; i++)
            {
                if (_allLanguages[i] == _language)
                {
                    fl = true;
                    break;
                }
            }

            if (!fl)
            {
                _language = _allLanguages[0];
                SaveLanguage();
            }
                
            string jsonWords = File.ReadAllText(
                Path.Combine(Application.dataPath, pathToLanguagesFolder, _language, _language + ".json"));

            _words = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonWords);
        }

        private static void UpdateLanguages()
        {
            string folder = Path.Combine(Application.dataPath, pathToLanguagesFolder);

            if (Directory.Exists(folder))
                Directory.Delete(folder, true);

            Directory.CreateDirectory(folder);

            TextAsset localesTextFile = Resources.Load<TextAsset>(pathToLanguagesInResources);
            _allLanguages = JsonConvert.DeserializeObject<string[]>(localesTextFile.text);

            if (_allLanguages.Length > 0)
            {
                _language = _allLanguages[0];
                SaveLanguage();
            }    

            for (int i = 0; i < _allLanguages.Length; i++)
            {
                TextAsset locale = Resources.Load<TextAsset>(
                    pathToLanguagesFolderInResources + _allLanguages[i]);

                Directory.CreateDirectory(
                    Path.Combine(Application.dataPath, pathToLanguagesFolder, _allLanguages[i]));

                File.WriteAllText(
                    Path.Combine(Application.dataPath, pathToLanguagesFolder, _allLanguages[i],
                    _allLanguages[i] + ".json"), locale.text);
            }
        }
        
        private static void SaveLanguage()
        {
            File.WriteAllText(Path.Combine(Application.dataPath, pathToLanguage), 
                JsonConvert.SerializeObject(_language));
        }

        private static void LoadLanguage()
        {
            string path = Path.Combine(Application.dataPath, pathToLanguage);

            if (!File.Exists(path))
            {
                _language = "";
            }
            else
            {
                string value = File.ReadAllText(path);
                _language = JsonConvert.DeserializeObject<string>(value);
            }
        }

        public static void SetLanguage(string language)
        {
            if (_allLanguages == null)
                Init();

            for (int i = 0; i < _allLanguages.Length; i++)
            {
                if (_allLanguages[i] == language)
                {
                    _language = language;
                    SaveLanguage();
                }
            }

            // error
        }

        public static string GetString(string key)
        {
            if (_words == null)
                Init();

            if (_words.TryGetValue(key, out string value))
                return value;
            else 
                return "";
        }
    }
}

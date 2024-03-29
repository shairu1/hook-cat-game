using System.IO;
using UnityEngine;
using System;
using System.Collections.Generic;

public static class ConfigManager
{
    private const string config_file = @"Data\config.json";

    private static ConfigFile _config;

    public static ConfigFile config
    { 
        private set 
        {
            _config = value;
        }

        get
        {
            if (_config == null)
                LoadConfig();

            return _config;
        }
    }

    private static void LoadConfig()
    {
        string path = Path.Combine(Application.dataPath, config_file);

        if (!File.Exists(path))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            config = new ConfigFile("-", "Data/Game", "Data/Upload", 
                "http://h203029328.nichost.ru/firstgame/launcher/", "info.json", "");

            File.WriteAllText(path, JsonUtility.ToJson(config));
        }
        else
        {
            config = JsonUtility.FromJson<ConfigFile>(File.ReadAllText(path));

            List<LauncherManager.GameVersion> versions = new List<LauncherManager.GameVersion>();
            for (int i = 0; i < config.downloadedVersionsGame.Length; i++)
            {
                string pathGame = Path.Combine(Application.dataPath, config.gameFolder,
                    config.downloadedVersionsGame[i].gameExe);

                if (File.Exists(pathGame))
                    versions.Add(config.downloadedVersionsGame[i]);
            }

            config.downloadedVersionsGame = versions.ToArray();

            bool check = true;
            for (int i = 0; i < config.downloadedVersionsGame.Length; i++)
            {
                if (config.downloadedVersionsGame[i].gameVersion == config.gameVersion)
                {
                    check = false;
                    break;
                }      
            }

            if (check)
                config.gameVersion = "-";
        } 
    }

    public static void AddDownloadedVersion(LauncherManager.GameVersion version)
    {
        for (int i = 0; i < config.downloadedVersionsGame.Length; i++)
        {
            if (config.downloadedVersionsGame[i].gameVersion == version.gameVersion)
                return;
        }

        LauncherManager.GameVersion[] newVersions = 
            new LauncherManager.GameVersion[config.downloadedVersionsGame.Length + 1];

        for (int i = 0; i < config.downloadedVersionsGame.Length; i++)
        {
            newVersions[i] = config.downloadedVersionsGame[i];
        }

        newVersions[newVersions.Length - 1] = version;

        config.downloadedVersionsGame = newVersions;
        SaveConfig();
    }

    public static void SaveConfig()
    {
        string path = Path.Combine(Application.dataPath, config_file);
        File.WriteAllText(path, JsonUtility.ToJson(config));
    }

    public static bool CheckLoadedVersion(string version)
    {
        for (int i = 0; i < config.downloadedVersionsGame.Length; i++)
        {
            if (config.downloadedVersionsGame[i].gameVersion == version)
                return true;
        }

        return false;
    }

    public static void SetSelectGameVersion(string version)
    {
        config.gameVersion = version;
        SaveConfig();
    }

    public static string GetPathGameExe()
    {
        for (int i = 0; i < config.downloadedVersionsGame.Length; i++)
        {
            if (config.downloadedVersionsGame[i].gameVersion == config.gameVersion)
                return config.downloadedVersionsGame[i].gameExe;
        }

        return "";
    }

    [Serializable]
    public class ConfigFile
    {
        public LauncherManager.GameVersion[] downloadedVersionsGame;
        public string gameVersion; // ��������� ������ ����
        public string gameFolder; // ����� � ������
        public string uploadFolder; // ��������
        public string serverURL;
        public string serverInfoURL;

        public ConfigFile(string version, string gameF, string uploadF, string serverURl,
            string serverInfoURL, string gameExe)
        {
            downloadedVersionsGame = new LauncherManager.GameVersion[0];
            this.gameVersion = version;
            this.gameFolder = gameF;
            this.uploadFolder = uploadF;
            this.serverURL = serverURl;
            this.serverInfoURL = serverInfoURL;
        }
    }
}



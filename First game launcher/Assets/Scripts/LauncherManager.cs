using UnityEngine;
using Networking;
using System.IO;
using System.ComponentModel;
using System.Net;
using System.IO.Compression;
using System;
using System.Collections;

public class LauncherManager : MonoBehaviour
{
    private static LauncherManager instance;
    public static bool updating;

    private ButtonType buttonType;
    private InfoFile info; // с сервера о игре
    private GameVersion downloadingVersion;

    private void Start()
    {
        instance = this;

        SetButtonType(ButtonType.None);
        UIManager.SetActivLoadingBar(false);
        UIManager.SetButtonText("Синхронизация");

        string dataPath = Path.Combine(Application.dataPath, ConfigManager.config.gameFolder);

        if (!Directory.Exists(dataPath))
            Directory.CreateDirectory(dataPath);

        UIManager.SetGameVersion(ConfigManager.config.gameVersion);

        dataPath = Path.Combine(Application.dataPath, ConfigManager.config.uploadFolder);

        if (!Directory.Exists(dataPath)) 
            Directory.CreateDirectory(dataPath);

        string pathInfo = Path.Combine(Application.dataPath, ConfigManager.config.uploadFolder, "info.json");

        if (File.Exists(pathInfo))
            File.Delete(pathInfo);

        ShaiNetwork.DownloadFileAsync
        (
            ConfigManager.config.serverURL + ConfigManager.config.serverInfoURL,
            pathInfo,
            null,
            new AsyncCompletedEventHandler[] { DownloadInfoCompleted }
        );
    }
    
    public static void DownloadGame(GameVersion version)
    {
        updating = true;
        instance.downloadingVersion = version;

        UIManager.SetActivLoadingBar(true);
        UIManager.SetLoadingBar(0);
        instance.SetButtonType(ButtonType.None);
        UIManager.SetButtonText("Обновление");
        UIManager.SetLoadingText("Загрузка обновления" + " (0%)");

        string pathGame = Path.Combine(Application.dataPath, ConfigManager.config.uploadFolder, "game.zip");

        if (File.Exists (pathGame))
            File.Delete(pathGame);

        ShaiNetwork.DownloadFileAsync
        (
            ConfigManager.config.serverURL + instance.downloadingVersion.gameURL,
            pathGame,
            new DownloadProgressChangedEventHandler[] { DownloadGameProgress },
            new AsyncCompletedEventHandler[] { DownloadGameCompleted }
        );
    }

    private static void DownloadGameProgress(object sender, DownloadProgressChangedEventArgs e)
    {
        UIManager.SetLoadingText("Загрузка обновления" + $" ({e.ProgressPercentage}%)");
        UIManager.SetLoadingBar(Mathf.Lerp(UIManager.GetLoadingBar(), e.ProgressPercentage / 100.0f, 0.01f));
    }

    private static void DownloadGameCompleted(object sender, AsyncCompletedEventArgs e)
    {
        if (e.Error == null)
        {
            string pathGameFolder = Path.Combine(Application.dataPath, ConfigManager.config.gameFolder);

            UIManager.SetLoadingBar(1);
            UIManager.SetLoadingText("Установка игры");

            instance.StartCoroutine(ExtractZipFile
            (
                Path.Combine(Application.dataPath, ConfigManager.config.uploadFolder, "game.zip"),
                Path.Combine(Application.dataPath, ConfigManager.config.gameFolder)
            ));
        }
        else
        {
            instance.SetButtonType(ButtonType.Play);
        }
    }

    private static IEnumerator ExtractZipFile(string file, string directory)
    {
        yield return new WaitForSeconds(0.5f);

        ZipFile.ExtractToDirectory(file, directory);

        UIManager.SetActivLoadingBar(false);

        instance.SetButtonType(ButtonType.Play);

        yield return new WaitForSeconds(0.5f);

        ConfigManager.AddDownloadedVersion(instance.downloadingVersion);
        VersionManager.UpdateVersions(instance.info.gameVersions);
        updating = false;

        yield break;
    }

    public static void SetGameVersion(string version)
    {
        ConfigManager.SetSelectGameVersion(version);
        VersionManager.SetSelectGameVersion(version);
        UIManager.SetGameVersion(version);
    }

    // загрузка файла с информацией о игре
    private void DownloadInfoCompleted(object sender, AsyncCompletedEventArgs e)
    {
        string pathInfo = Path.Combine(Application.dataPath, ConfigManager.config.uploadFolder, "info.json");

        if (e.Error == null)
        {
            info = JsonUtility.FromJson<InfoFile>(File.ReadAllText(pathInfo));

            VersionManager.UpdateVersions(info.gameVersions);

            if (ConfigManager.config.gameVersion != "-")
            {
                VersionManager.SetSelectGameVersion(ConfigManager.config.gameVersion);
                SetButtonType(ButtonType.Play);
            } 
        }
        else
        {
            if (ConfigManager.config.gameVersion != "-")
            {
                SetButtonType(ButtonType.Play);
            }
        }
    }

    private void SetButtonType(ButtonType type)
    {
        UIManager.SetButtonText("Играть");
        buttonType = type;
    }

    public void OnButtonClick()
    {
        if (buttonType == ButtonType.None)
        {
            return;
        }
        else if (buttonType == ButtonType.Play)
        {
            string path = Path.Combine(Application.dataPath,
                ConfigManager.config.gameFolder,
                ConfigManager.GetPathGameExe());

            if (File.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
        }
    }

    [Serializable]
    public class InfoFile
    {
        public GameVersion[] gameVersions;

        public GameVersion GetGameVersion(string version)
        {
            for (int i = 0; i < gameVersions.Length; i++)
            {
                if (gameVersions[i].gameVersion == version)
                    return gameVersions[i];
            }

            return null;
        }
    }

    [Serializable]
    public class GameVersion
    {
        public string gameData;
        public string gameVersion;
        public string gameURL;
        public string gameExe;
    }

}

public enum ButtonType // Тип кнопки при нажатии
{
    None,
    Play
}
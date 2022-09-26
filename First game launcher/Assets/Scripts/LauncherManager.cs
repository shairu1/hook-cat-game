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
    private ButtonType buttonType;

    private InfoFile info; // с сервера о игре

    private void Start()
    {
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
    
    private void UpdateGame()
    {
        UIManager.SetActivLoadingBar(true);
        UIManager.SetLoadingBar(0);
        SetButtonType(ButtonType.None);
        UIManager.SetButtonText("Обновление");
        UIManager.SetLoadingText("Загрузка обновления" + " (0%)");

        string pathGame = Path.Combine(Application.dataPath, ConfigManager.config.uploadFolder, "game.zip");

        if (File.Exists (pathGame))
            File.Delete(pathGame);

        ShaiNetwork.DownloadFileAsync
        (
            ConfigManager.config.serverURL + info.gameURL,
            pathGame,
            new DownloadProgressChangedEventHandler[] { DownloadGameProgress },
            new AsyncCompletedEventHandler[] { DownloadGameCompleted }
        );

    }

    private void DownloadGameProgress(object sender, DownloadProgressChangedEventArgs e)
    {
        UIManager.SetLoadingText("Загрузка обновления" + $" ({e.ProgressPercentage}%)");
        UIManager.SetLoadingBar(Mathf.Lerp(UIManager.GetLoadingBar(), e.ProgressPercentage / 100.0f, 0.01f));
    }

    private void DownloadGameCompleted(object sender, AsyncCompletedEventArgs e)
    {
        if (e.Error == null)
        {
            string pathGameFolder = Path.Combine(Application.dataPath, ConfigManager.config.gameFolder);

            if (Directory.Exists(pathGameFolder))
                Directory.Delete(pathGameFolder, true);

            Directory.CreateDirectory(pathGameFolder);

            UIManager.SetLoadingBar(1);
            UIManager.SetLoadingText("Установка игры");
            Camera.main.Render();

            StartCoroutine(ExtractZipFile
            (
                Path.Combine(Application.dataPath, ConfigManager.config.uploadFolder, "game.zip"),
                Path.Combine(Application.dataPath, ConfigManager.config.gameFolder)
            ));
        }
        else
        {
            SetButtonType(ButtonType.Play);
        }
    }

    IEnumerator ExtractZipFile(string file, string directory)
    {
        yield return new WaitForSeconds(0.5f);

        ZipFile.ExtractToDirectory(file, directory);

        ConfigManager.config.gameVersion = info.gameVersion;
        ConfigManager.config.gameExe = info.gameExe;
        ConfigManager.SaveConfig();

        UIManager.SetActivLoadingBar(false);

        UIManager.SetGameVersion(ConfigManager.config.gameVersion);

        SetButtonType(ButtonType.Play);

        yield break;
    }

    // загрузка файла с информацией о игре
    private void DownloadInfoCompleted(object sender, AsyncCompletedEventArgs e)
    {
        string pathInfo = Path.Combine(Application.dataPath, ConfigManager.config.uploadFolder, "info.json");

        if (e.Error == null)
        {
            info = JsonUtility.FromJson<InfoFile>(File.ReadAllText(pathInfo));

            if (info.gameVersion != ConfigManager.config.gameVersion)
                SetButtonType(ButtonType.Update);
            else
                SetButtonType(ButtonType.Play);
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
        if (type == ButtonType.None)
            UIManager.SetButtonText("Играть");
        else if (type == ButtonType.Update)
            UIManager.SetButtonText("Обновить");
        else if (type == ButtonType.Play)
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
                ConfigManager.config.gameExe);

            if (File.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
        }
        else if (buttonType == ButtonType.Update)
        {
            UpdateGame();
        }
    }

    [Serializable]
    public class InfoFile
    {
        public string gameVersion;
        public string gameURL;
        public string gameExe;

        public InfoFile()
        {
            gameVersion = "";
            gameURL = "";
            gameExe = "";
        }

        public InfoFile(string gameVersion, string gameURL, string gameExe)
        {
            this.gameVersion = gameVersion;
            this.gameURL = gameURL;
            this.gameExe = gameExe;
        }
    }
}

public enum ButtonType // Тип кнопки при нажатии
{
    None,
    Update,
    Play
}




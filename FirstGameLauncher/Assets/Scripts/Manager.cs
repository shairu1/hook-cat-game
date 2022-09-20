using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;

public class Manager : MonoBehaviour
{
    public string UrlServer;

    public EType Etype;

    public TextMeshProUGUI VersionText;
    public TextMeshProUGUI OnButtonText;

    private void Start()
    {
        Etype = EType.None;
        OnButtonText.text = "Синхронизация с сервером";

        if(!Directory.Exists(Path.Combine(Application.dataPath, "Download")))
        {
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "Download"));
        }

        if (!Directory.Exists(Path.Combine(Application.dataPath, "Game")))
        {
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "Game"));
        }

        DownloadFileAsync
        (
            UrlServer + "version.txt", 
            Path.Combine(Application.dataPath, "Download", "version.txt"),
            null,
            new AsyncCompletedEventHandler[] { DownloadVersionCompleted }
        );
    }

    public static void DownloadFileAsync(string url, string filePath,
                DownloadProgressChangedEventHandler[] progressEvent,
                AsyncCompletedEventHandler[] completedEvent)
    {
        WebClient webClient = new WebClient();

        if (progressEvent != null)
        {
            for (int i = 0; i < progressEvent.Length; i++)
                webClient.DownloadProgressChanged += progressEvent[i];
        }

        if (completedEvent != null)
        {
            for (int i = 0; i < completedEvent.Length; i++)
                webClient.DownloadFileCompleted += completedEvent[i];
        }

        webClient.DownloadFileAsync(new System.Uri(url), filePath);
    }

    private void DownloadVersionCompleted(object sender, AsyncCompletedEventArgs e)
    {
        if(e.Error == null)
        {
            string sv = File.ReadAllText(Path.Combine(Application.dataPath, "Download", "version.txt"));

            if(sv != Application.version)
            {
                OnButtonText.text = "Обновить игру";
                Etype = EType.Update;
            }
            else
            {
                OnButtonText.text = "Начать игру";
                Etype = EType.Play;
            }
        }
        else
        {
            OnButtonText.text = "Начать игру";
            Etype = EType.Play;
        }
    }

    public void OnButtonClick()
    {
        if (Etype == EType.Update)
        {
            DownloadFileAsync
            (
                UrlServer + "fgp.txt",
                Path.Combine(Application.dataPath, "Download", "fgp.zip"),
                new DownloadProgressChangedEventHandler [] { DownloadUpdateProgress },
                new AsyncCompletedEventHandler[] { DownloadUpdateCompleted }
            );
        }
        else if (Etype == EType.Play)
        {
            if (File.Exists(Path.Combine(Application.dataPath, "Game", "First game.exe")))
            {
                System.Diagnostics.Process.Start(Path.Combine(Application.dataPath, "Game", "First game.exe"));
            }
        }
    }

    private void DownloadUpdateCompleted(object sender, AsyncCompletedEventArgs e)
    {
        if (e.Error == null)
        {
            Directory.Delete(Path.Combine(Application.dataPath, "Game"));
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "Game"));
            ZipFile.ExtractToDirectory
            (
                Path.Combine(Application.dataPath, "Download", "fgp.zip"),
                Path.Combine(Application.dataPath, "Game")
            );
            Etype = EType.Play;
        }
    }

    private void DownloadUpdateProgress(object sender, DownloadProgressChangedEventArgs e)
    {
        OnButtonText.text = $"Обновление ({e.ProgressPercentage})";
    }
}

public enum EType
{
    None,
    Play,
    Update
}
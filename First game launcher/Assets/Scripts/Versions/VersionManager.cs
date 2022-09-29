using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionManager : MonoBehaviour
{
    [SerializeField] private GameObject _versionUpdatePrefab;
    [SerializeField] private GameObject _versionPrefab;
    [SerializeField] private Transform _content;

    private static VersionManager instance;

    private ControllerVersion _selectVersion;
    private ControllerVersion[] _versionControllers;

    private void Start()
    {
        instance = this;
    }

    public static void UpdateVersions(LauncherManager.GameVersion[] info)
    {
        bool CheckVersionInList(List<LauncherManager.GameVersion> versions, LauncherManager.GameVersion version)
        {
            for (int i = 0; i < versions.Count; i++)
            {
                if (versions[i].gameVersion == version.gameVersion)
                    return true;
            }

            return false;
        }

        foreach (Transform child in instance._content)
            Destroy(child.gameObject);

        List<LauncherManager.GameVersion> versions = new List<LauncherManager.GameVersion>(info);
        ConfigManager.ConfigFile config = ConfigManager.config;

        for (int i = 0; i < config.downloadedVersionsGame.Length; i++)
        {
            if (!CheckVersionInList(versions, config.downloadedVersionsGame[i]))
            {
                versions.Add(config.downloadedVersionsGame[i]);
            }
        }

        instance._versionControllers = new ControllerVersion[versions.Count];

        for (int i = versions.Count - 1; i >= 0; i--)
        {
            GameObject go;

            if (ConfigManager.CheckLoadedVersion(versions[i].gameVersion))
                go = Instantiate(instance._versionPrefab, instance._content);
            else
                go = Instantiate(instance._versionUpdatePrefab, instance._content);

            ControllerVersion controllerVersion = go.GetComponent<ControllerVersion>();     
            controllerVersion.Init(versions[i]);

            if (ConfigManager.config.gameVersion == versions[i].gameVersion)
            {
                controllerVersion.isSelect = true;
                instance._selectVersion = controllerVersion;
            }
            else
            {
                controllerVersion.isSelect = false;
            }

            instance._versionControllers[i] = controllerVersion;
        }
    }

    public static void SetSelectGameVersion(string version)
    {
        if (instance._selectVersion != null)
            instance._selectVersion.isSelect = false;

        for (int i = 0; i < instance._versionControllers.Length; i++)
        {
            if (instance._versionControllers[i].GetVersion() == version)
            {
                instance._versionControllers[i].isSelect = true;
                instance._selectVersion = instance._versionControllers[i];
            }    
        }
    }
}

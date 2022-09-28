using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionManager : MonoBehaviour
{
    [SerializeField] private GameObject _versionUpdatePrefab;
    [SerializeField] private GameObject _versionPrefab;
    [SerializeField] private Transform _content;

    private VersionManager instance;

    private void Start()
    {
        instance = this;
    }

    public static void UpdateVersions(LauncherManager.InfoFile info)
    {
        for (int i = 0; i < info.gameVersions.Length; i++)
        {

        }    
    }

    public static void SetSelectVersion(string version)
    {

    }
}

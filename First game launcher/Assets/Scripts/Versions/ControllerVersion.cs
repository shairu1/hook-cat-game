using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ControllerVersion : MonoBehaviour
{
    private static Color32 defaultColor = new Color32(36, 143, 218, 170);
    private static Color32 selectColor = new Color32(35, 218, 78, 170);

    [SerializeField] private TextMeshProUGUI _textPro;
    [SerializeField] private Image _background;

    private bool _isSelect;
    public bool isSelect
    {
        get
        { 
            return _isSelect;
        }

        set
        {
            if (value)
                _background.color = selectColor;
            else
                _background.color = defaultColor;

            _isSelect = value;
        }
    }

    private LauncherManager.GameVersion _version;

    public void Init(LauncherManager.GameVersion version)
    {
        _version = version;
        isSelect = false;
        _textPro.text = version.gameData + "\n" + version.gameVersion;
    }

    public string GetVersion()
    {
        return _version.gameVersion;
    }

    public void OnDownloadButtonDown()
    {
        if (LauncherManager.updating) return;

        if (!ConfigManager.CheckLoadedVersion(_version.gameVersion))
        {
            LauncherManager.DownloadGame(_version);
        }
    }

    public void OnButtonDown()
    {
        if (LauncherManager.updating || !ConfigManager.CheckLoadedVersion(_version.gameVersion)) 
            return;

        LauncherManager.SetGameVersion(_version.gameVersion);
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameVersionText;
    [SerializeField] private TextMeshProUGUI buttonText;

    [SerializeField] private GameObject loadingBarObj;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private Image loadingBarImage;
    
    private static UIManager instance;

    private void Awake()
    {
        instance = this;
    }

    public static void SetButtonText(string text)
    {
        instance.buttonText.text = text;
    }

    public static void SetGameVersion(string version)
    {
        instance.gameVersionText.text = "Версия: " + version;
    }

    public static void SetActivLoadingBar(bool value)
    {
        instance.loadingBarObj.SetActive(value);
    }

    public static void SetLoadingBar(float value)
    {
        instance.loadingBarImage.fillAmount = value;
    }
    
    public static float GetLoadingBar()
    {
        return instance.loadingBarImage.fillAmount;
    }

    public static void SetLoadingText(string text)
    {
        instance.loadingText.text = text;
    }
}

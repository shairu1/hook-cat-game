using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManagerLoadingGame : MonoBehaviour
{
    public Camera cam;

    [SerializeField] private TextMeshProUGUI _loadingTxt;

    private void Update()
    {
        Services.SceneTransitions.SceneTransition.SwitchToScene("Level1");
        enabled = false;
    }

    public void SetloadingText(string text)
    {
        _loadingTxt.text = text;
    }
}

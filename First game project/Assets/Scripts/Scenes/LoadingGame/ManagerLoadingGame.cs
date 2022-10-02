using UnityEngine;
using TMPro;


public class ManagerLoadingGame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _loadingTxt;

    public Camera cam;

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

using UnityEngine;
using UnityEngine.UI;


public class LoadingAnimation : MonoBehaviour
{
    [SerializeField] private Image _loadingImg;
    [SerializeField] public float SpeedAnimation;

    private void Start()
    {
        _loadingImg.fillClockwise = true;
        _loadingImg.fillAmount = 0;
    }

    private void Update()
    {
        if (_loadingImg.fillClockwise)
        {
            _loadingImg.fillAmount += SpeedAnimation * Time.deltaTime;

            if (_loadingImg.fillAmount >= 1)
                _loadingImg.fillClockwise = false;
        }
        else
        {
            _loadingImg.fillAmount -= SpeedAnimation * Time.deltaTime;

            if (_loadingImg.fillAmount <= 0)
                _loadingImg.fillClockwise = true;
        }
    }
}

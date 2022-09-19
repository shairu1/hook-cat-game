using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SceneTransitions
{
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField] private Image _loadingProgressBar;
    
        private static SceneTransition instance;
        private static bool shouldPlayOpeningAnimation = false;
    
        private Animator _componentAnimator;
        private AsyncOperation _loadingSceneOperation;
        private string _loadingSceneName;

        public static void SwitchToScene(string sceneName)
        {
            if(instance == null)
            {
                Debug.Log("instance = null");
            }

            instance.gameObject.SetActive(true);
            instance._componentAnimator.SetTrigger("hide");
            instance._loadingProgressBar.fillAmount = 0;
            instance._loadingSceneName = sceneName;
        }
    
        private void Start()
        {
            instance = this;
        
            _componentAnimator = GetComponent<Animator>();
        
            if (shouldPlayOpeningAnimation) 
            {
                _componentAnimator.SetTrigger("show");
                instance._loadingProgressBar.fillAmount = 1;
            
                // Чтобы если следующий переход будет обычным SceneManager.LoadScene, не проигрывать анимацию opening:
                shouldPlayOpeningAnimation = false; 
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (_loadingSceneOperation != null)
            {
                // Присвоить прогресс с быстрой анимацией, чтобы ощущалось плавнее:
                _loadingProgressBar.fillAmount = Mathf.Lerp(_loadingProgressBar.fillAmount, _loadingSceneOperation.progress,
                Time.deltaTime * 5);
            }
        }

        public void OnHideAnimationOver()
        {
            instance._loadingSceneOperation = SceneManager.LoadSceneAsync(_loadingSceneName);
        
            // Чтобы сцена не начала переключаться пока играет анимация closing:
            instance._loadingSceneOperation.allowSceneActivation = false;

            // Чтобы при открытии сцены, куда мы переключаемся, проигралась анимация opening:
            shouldPlayOpeningAnimation = true;
        
            _loadingSceneOperation.allowSceneActivation = true;
        }

        public void OnShowAnimationOver()
        {
            gameObject.SetActive(false);
        }
    }
}


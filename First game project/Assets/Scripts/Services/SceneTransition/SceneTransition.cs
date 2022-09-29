using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace Services.SceneTransitions
{
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _loadingText;

        private static SceneTransition instance;
        private static bool shouldPlayOpeningAnimation = false;

        private Animator _animator;
        private AsyncOperation _loadingSceneOperation;
        private string _loadingSceneName;

        public static void SwitchToScene(string sceneName)
        {
            instance.gameObject.SetActive(true);
            instance._animator.SetTrigger("hide");
            instance._loadingText.text = "Загрузка (0%)";
            instance._loadingSceneName = sceneName;
        }

        private void Start()
        {
            instance = this;

            _animator = GetComponent<Animator>();

            if (shouldPlayOpeningAnimation)
            {
                _animator.SetTrigger("show");
                instance._loadingText.text = "Загрузка (100%)";

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
                _loadingText.text = $"Загрузка ({(int)((_loadingSceneOperation.progress + 0.1f) * 100)}%)";
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
using System.Collections;
using TimeManagement;
using UnityEngine;
using UnityEngine.UI;

namespace FadeToBlack
{
    [RequireComponent(typeof(Image))]
    public class FadeToBlackPanel : MonoBehaviour
    {
        [SerializeField] private float fadeInSeconds = 0.25f, stayVisibleSeconds = 0.1f, fadeOutSeconds = 0.25f;
        private Image _image;
        private float _timer;


        #region Singleton

        // Singleton stuff

        /// <summary>
        /// The singleton instance.
        /// </summary>
        public static FadeToBlackPanel Instance;

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            _image = GetComponent<Image>();

            // Don't destroy parent canvas
            DontDestroyOnLoad(transform.parent.gameObject);
        }

        #endregion
        
        /// <summary>
        /// Manually invoke a fade to black
        /// </summary>
        public void FadeToBlack()
        {
            StopAllCoroutines();
            StartCoroutine(FadeIn());
        }

        private void OnEnable()
        {
            TimeHandler.Instance.onDayPhaseChanged.AddListener(OnDaytimePhaseChanged);
        }

        private void OnDisable()
        {
            TimeHandler.Instance.onDayPhaseChanged.RemoveListener(OnDaytimePhaseChanged);
        }

        private void OnDaytimePhaseChanged(DaytimePhase phase)
        {
            FadeToBlack();
        }

        private IEnumerator FadeIn()
        {
            _image.enabled = true;
            
            _timer = 0f;
            while (_timer < fadeInSeconds)
            {
                _timer += Time.deltaTime;
                _image.color = new Color(0, 0, 0, _timer / fadeInSeconds);
                yield return null;
            }

            StartCoroutine(StayVisible());
        }
        
        private IEnumerator StayVisible()
        {
            _timer = 0f;
            _image.color = new Color(0, 0, 0, 1);
            while (_timer < stayVisibleSeconds)
            {
                _timer += Time.deltaTime;
                yield return null;
            }

            StartCoroutine(FadeOut());
        }
        
        
        private IEnumerator FadeOut()
        {
            _timer = 0f;
            while (_timer < fadeInSeconds)
            {
                _timer += Time.deltaTime;
                _image.color = new Color(0, 0, 0,  1.0f - (_timer / fadeOutSeconds));
                yield return null;
            }

            _image.enabled = false;
        }
    }
}

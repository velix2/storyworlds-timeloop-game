using System;
using System.Collections;
using TimeManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace FadeToBlack
{
    [RequireComponent(typeof(Image))]
    public class FadeToBlackPanel : MonoBehaviour
    {
        [SerializeField] private float fadeInSeconds = 0.25f, stayVisibleSeconds = 0.1f, fadeOutSeconds = 0.25f;
        public float FadeInSeconds => fadeInSeconds;
        public float FadeOutSeconds => fadeOutSeconds;
        public float StayVisibleSeconds => stayVisibleSeconds;
        private Image _image;
        private float _timer;

        private bool _isSceneTransitioning;

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
            // If there currently is a Scene Transition Fade happening to nothing
            if (_isSceneTransitioning) return;

            StopAllCoroutines();
            Action afterStay = () => StartCoroutine(FadeOut());
            Action afterFadeIn = () => StartCoroutine(StayVisible(afterStay));
            
            StartCoroutine(FadeIn(afterFadeIn));
        }

        private void Start()
        {
            TimeHandler.Instance.onDayPhaseChanged.AddListener(OnDaytimePhaseChanged);
        }

        private void OnDaytimePhaseChanged(DaytimePhase phase)
        {
            FadeToBlack();
        }

        public void SceneTransitionIn(Action onAnimationEnd = null)
        {
            _isSceneTransitioning = true;
            
            StartCoroutine(FadeIn(onAnimationEnd));
        }

        public void SceneTransitionOut(Action onAnimationEnd = null)
        {
            StartCoroutine(FadeOut(WrapperCallback));
            return;

            // Reset the transition flag when done as well
            void WrapperCallback()
            {
                _isSceneTransitioning = false;
                onAnimationEnd?.Invoke();
            }
        }

        private IEnumerator FadeIn(Action onAnimationEnd = null)
        {
            _image.enabled = true;

            _timer = 0f;
            while (_timer < fadeInSeconds)
            {
                _timer += Time.deltaTime;
                _image.color = new Color(0, 0, 0, _timer / fadeInSeconds);
                yield return null;
            }

            onAnimationEnd?.Invoke();
        }

        private IEnumerator StayVisible(Action onAnimationEnd = null)
        {
            _timer = 0f;
            _image.color = new Color(0, 0, 0, 1);
            while (_timer < stayVisibleSeconds)
            {
                _timer += Time.deltaTime;
                yield return null;
            }
            
            onAnimationEnd?.Invoke();
        }


        private IEnumerator FadeOut(Action onAnimationEnd = null)
        {
            _timer = 0f;
            while (_timer < fadeInSeconds)
            {
                _timer += Time.deltaTime;
                _image.color = new Color(0, 0, 0, 1.0f - (_timer / fadeOutSeconds));
                yield return null;
            }

            _image.enabled = false;

            onAnimationEnd?.Invoke();
        }
    }
}
using TMPro;
using UnityEngine;

namespace DeliveryRushExam.UI
{
    public class ScorePopupView : MonoBehaviour
    {
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private float lifetime = 1.1f;
        [SerializeField] private float moveSpeed = 55f;

        private float age;
        private CanvasGroup _canvasGroup;
        private ScorePopupPool _pool;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Setup(string message, ScorePopupPool pool)
        {
            _pool = pool;
            if (_canvasGroup != null) _canvasGroup.alpha = 1f;
            age = 0f;
            messageText.text = message;
        }

        private void Update()
        {
            age += Time.deltaTime;
            transform.localPosition += Vector3.up * moveSpeed * Time.deltaTime;

            CanvasGroup canvasGroup = _canvasGroup;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f - age / lifetime;
            }

            if (age >= lifetime)
            {
                _pool.Return(this);
            }
        }
    }
}

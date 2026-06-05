using System.Collections.Generic;
using UnityEngine;

namespace DeliveryRushExam.UI
{
    public class ScorePopupPool : MonoBehaviour
    {
        [SerializeField] private ScorePopupView prefab;
        [SerializeField] private RectTransform container;
        [SerializeField] private int initialSize = 10;

        private readonly Queue<ScorePopupView> _available = new Queue<ScorePopupView>();

        private void Awake()
        {
            for (int i = 0; i < initialSize; i++)
            {
                ScorePopupView popup = Instantiate(prefab, container);
                popup.gameObject.SetActive(false);
                _available.Enqueue(popup);
            }

            Debug.Log("[ScorePopupPool] Pool inicializado con " + initialSize + " popups.");
        }

        public ScorePopupView Get()
        {
            ScorePopupView popup;

            if (_available.Count > 0)
            {
                popup = _available.Dequeue();
            }
            else
            {
                // Si el pool se quedó sin objetos, creamos uno nuevo
                popup = Instantiate(prefab, container);
                Debug.LogWarning("[ScorePopupPool] Pool expandido — considerar aumentar initialSize.");
            }

            popup.gameObject.SetActive(true);
            return popup;
        }

        public void Return(ScorePopupView popup)
        {
            popup.gameObject.SetActive(false);
            _available.Enqueue(popup);
        }
    }
}
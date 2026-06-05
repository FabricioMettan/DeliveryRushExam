using System;
using System.Threading.Tasks;
using DeliveryRushExam.Data;
using DeliveryRushExam.UGS;
using UnityEngine;

namespace DeliveryRushExam.Save
{
    public class SaveManager : MonoBehaviour
    {
        [SerializeField] private UgsInitializer ugsInitializer;

        public PlayerProgressData CurrentProgress { get; private set; } = new PlayerProgressData();
        public event Action<PlayerProgressData> ProgressLoaded;

        private ISaveService _saveService;

        private async void Awake()
        {
            if (ugsInitializer != null)
            {
                await ugsInitializer.InitializeAsync();
                Debug.Log("[SaveManager] UGS listo: " + ugsInitializer.IsReady);
            }

            _saveService = ServiceLocator.Get<ISaveService>();
            Debug.Log("[SaveManager] Servicio cargado: " + _saveService.GetType().Name);
            await LoadProgressAsync();
        }

        public async Task LoadProgressAsync()
        {
            CurrentProgress = await _saveService.LoadAsync();
            Debug.Log("[SaveManager] Progreso cargado — bestScore: " + CurrentProgress.bestScore +
                      " | totalCoins: " + CurrentProgress.totalCoins +
                      " | completedOrders: " + CurrentProgress.completedOrders);
            ProgressLoaded?.Invoke(CurrentProgress);
        }

        public async Task SaveMatchResultAsync(int score, int coins, int completedOrders)
        {
            CurrentProgress.bestScore = Mathf.Max(CurrentProgress.bestScore, score);
            CurrentProgress.totalCoins += coins;
            CurrentProgress.completedOrders += completedOrders;
            CurrentProgress.unlockedLevel = Mathf.Max(CurrentProgress.unlockedLevel,
                                            1 + CurrentProgress.completedOrders / 10);

            await _saveService.SaveAsync(CurrentProgress);
            Debug.Log("[SaveManager] Progreso guardado — bestScore: " + CurrentProgress.bestScore +
                      " | totalCoins: " + CurrentProgress.totalCoins +
                      " | completedOrders: " + CurrentProgress.completedOrders);
        }
    }
}
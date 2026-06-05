using System.Collections.Generic;
using System.Threading.Tasks;
using DeliveryRushExam.Data;
using UnityEngine;

#if DELIVERY_RUSH_UGS
using Unity.Services.CloudSave;
using Unity.Services.Authentication;
#endif

namespace DeliveryRushExam.Save
{
    public class UgsCloudSaveService : ISaveService
    {
        private const string ProgressKey = "delivery_rush_progress";

        public async Task<PlayerProgressData> LoadAsync()
        {
#if DELIVERY_RUSH_UGS
            try
            {
                var keys = new HashSet<string> { ProgressKey };
                var result = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

                if (result.ContainsKey(ProgressKey))
                {
                    string json = result[ProgressKey].Value.GetAsString();
                    PlayerProgressData data = JsonUtility.FromJson<PlayerProgressData>(json);
                    Debug.Log("[UgsCloudSaveService] Progreso cargado desde la nube — PlayerId: "
                              + AuthenticationService.Instance.PlayerId);
                    return data ?? new PlayerProgressData();
                }

                Debug.Log("[UgsCloudSaveService] No hay progreso en la nube, creando nuevo.");
                return new PlayerProgressData();
            }
            catch (System.Exception e)
            {
                Debug.LogError("[UgsCloudSaveService] Error al cargar: " + e.Message);
                return new PlayerProgressData();
            }
#else
            Debug.LogWarning("UGS Cloud Save no está habilitado.");
            await Task.Yield();
            return new PlayerProgressData();
#endif
        }

        public async Task SaveAsync(PlayerProgressData progressData)
        {
#if DELIVERY_RUSH_UGS
            try
            {
                progressData.TouchSaveDate();
                string json = JsonUtility.ToJson(progressData);

                var data = new Dictionary<string, object>
                {
                    { ProgressKey, json }
                };

                await CloudSaveService.Instance.Data.Player.SaveAsync(data);
                Debug.Log("[UgsCloudSaveService] Progreso guardado en la nube — " +
                          "bestScore: " + progressData.bestScore +
                          " | totalCoins: " + progressData.totalCoins +
                          " | completedOrders: " + progressData.completedOrders +
                          " | lastSaveDate: " + progressData.lastSaveDate);
            }
            catch (System.Exception e)
            {
                Debug.LogError("[UgsCloudSaveService] Error al guardar: " + e.Message);
            }
#else
            Debug.LogWarning("UGS Cloud Save no está habilitado.");
            await Task.Yield();
#endif
        }
    }
}
using System.Threading.Tasks;
using UnityEngine;

#if DELIVERY_RUSH_UGS
using Unity.Services.Authentication;
using Unity.Services.Core;
#endif

namespace DeliveryRushExam.UGS
{
    public class UgsInitializer : MonoBehaviour
    {
        [SerializeField] private bool verboseLogs = true;

        private static bool _initialized = false;

        // Start() eliminado — la inicialización la maneja SaveManager
        // para evitar llamadas paralelas al SignInAnonymouslyAsync

        public bool IsReady { get; private set; }

        public async Task InitializeAsync()
        {
            // Si ya inicializamos, no hacer nada
            if (_initialized)
            {
                IsReady = true;
                return;
            }

#if DELIVERY_RUSH_UGS
            try
            {
                // Paso 1 — inicializar UGS
                if (UnityServices.State == ServicesInitializationState.Uninitialized)
                {
                    await UnityServices.InitializeAsync();
                    if (verboseLogs) Debug.Log("[UgsInitializer] UnityServices inicializado.");
                }

                // Paso 2 — autenticar
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    if (verboseLogs) Debug.Log("[UgsInitializer] SignIn completado. PlayerId: "
                        + AuthenticationService.Instance.PlayerId);
                }

                _initialized = true;
                IsReady = true;

                if (verboseLogs) Debug.Log("UGS ready. PlayerId: "
                    + AuthenticationService.Instance.PlayerId);
            }
            catch (System.Exception e)
            {
                Debug.LogError("[UgsInitializer] Error al inicializar UGS: " + e.Message);
                IsReady = false;
            }
#else
            IsReady = false;
            if (verboseLogs)
                Debug.Log("UGS initializer present. Install UGS packages and define DELIVERY_RUSH_UGS to enable it.");
            await Task.Yield();
#endif
        }
    }
}
using UnityEngine;

namespace ClownMeister.UnityEssentials.DI.ServiceLocator
{
    public abstract class BaseServiceInitializer : MonoBehaviour
    {
        private static bool _servicesInitialized = false;

        private void Awake()
        {
            if (_servicesInitialized) return;
            RegisterServices();
            _servicesInitialized = true;
            OnServicesLoad();
        }

        // Child classes must implement this method to register their specific services
        protected abstract void RegisterServices();

        // Child classes must implement this method to customize what happens after services are loaded
        protected abstract void OnServicesLoad();
    }
}
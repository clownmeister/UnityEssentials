using System.Collections.Generic;
using UnityEngine;

namespace ClownMeister.UnityEssentials.DI.ServiceLocator
{
    public abstract class BaseServiceInitializer : MonoBehaviour
    {
        private static bool _servicesInitialized = false;
        [SerializeField]
        private List<GameObject> persistentObjects = new List<GameObject>();

        private void Awake()
        {
            if (_servicesInitialized) return;
            RegisterServices();

            // Make all specified persistent objects persistent.
            // Most likely the one that holds all the services and this initializer
            foreach (GameObject obj in persistentObjects)
            {
                DontDestroyOnLoad(obj);
            }

            _servicesInitialized = true;
            OnServicesLoad();
        }

        // Child classes must implement this method to register their specific services
        // Use ServiceLocator.Register<>()
        protected abstract void RegisterServices();

        // Child classes must implement this method to customize what happens after services are loaded
        protected abstract void OnServicesLoad();
    }
}
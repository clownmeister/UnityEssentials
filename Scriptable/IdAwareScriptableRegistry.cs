using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClownMeister.UnityEssentials.Scriptable
{
    [CreateAssetMenu(fileName = "NewIdAwareScriptableRegistry", menuName = "Registry/IdAwareScriptableRegistry")]
    public class IdAwareScriptableRegistry : ScriptableObject
    {
        [SerializeField]
        private List<IdAwareScriptable> scriptableObjects = new List<IdAwareScriptable>();

        private Dictionary<Guid, IdAwareScriptable> _registry = new Dictionary<Guid, IdAwareScriptable>();

        private void OnEnable()
        {
            _registry.Clear();
            foreach (var scriptable in scriptableObjects)
            {
                if (scriptable != null)
                {
                    _registry.TryAdd(scriptable.Id, scriptable);
                }
            }
        }

        public IdAwareScriptable GetById(Guid id)
        {
            _registry.TryGetValue(id, out IdAwareScriptable scriptable);
            return scriptable;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace ClownMeister.UnityEssentials.Scriptable
{
    [CreateAssetMenu(fileName = "NewIdAwareScriptableRegistry", menuName = "Registry/IdAwareScriptableRegistry")]
    public class IdAwareScriptableRegistry<T> : ScriptableObject where T : IdAwareScriptable
    {
        [SerializeField]
        public List<T> items = new List<T>();
    }
}
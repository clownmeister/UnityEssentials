using System.Collections.Generic;
using UnityEngine;

namespace ClownMeister.UnityEssentials.Scriptable
{
    public abstract class IdAwareScriptableRegistry<T> : ScriptableObject where T : IdAwareScriptable
    {
        [SerializeField]
        public List<T> items = new List<T>();
    }
}
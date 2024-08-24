using UnityEngine;

namespace ClownMeister.UnityEssentials.Scriptable
{
    //Useful for referencing scriptables across project reliably with persistence.
    public abstract class IdAwareScriptable : ScriptableObject
    {
        private System.Guid _id;

        public System.Guid Id => _id;

        private void OnValidate()
        {
            // Generate a unique ID if not already set
            if (_id == System.Guid.Empty)
            {
                _id = System.Guid.NewGuid();
            }
        }
    }
}
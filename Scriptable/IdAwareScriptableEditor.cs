using UnityEditor;

namespace ClownMeister.UnityEssentials.Scriptable
{
    [CustomEditor(typeof(IdAwareScriptable), true)]
    public class IdAwareScriptableEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            IdAwareScriptable scriptable = (IdAwareScriptable)target;

            // Display the immutable ID as read-only
            EditorGUILayout.LabelField("ID", scriptable.Id.ToString());

            // Draw the rest of the default inspector
            DrawDefaultInspector();
        }
    }
}
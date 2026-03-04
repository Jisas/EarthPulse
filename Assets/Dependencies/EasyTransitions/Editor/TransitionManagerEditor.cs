using UnityEditor;

namespace EasyTransition
{
#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TransitionManager))]
    public class TransitionManagerEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
        }
    }
#endif
}
    

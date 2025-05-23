using UnityEngine;
using UnityEditor;

public static class MissingScriptFinder
{
    [MenuItem("Tools/Find Missing Scripts")]
    public static void FindMissingScripts()
    {
        foreach (var go in Object.FindObjectsOfType<GameObject>())
        {
            var so = new SerializedObject(go);
            var comps = so.FindProperty("m_Component");
            for (int i = comps.arraySize - 1; i >= 0; i--)
            {
                var compRef = comps.GetArrayElementAtIndex(i)
                                   .FindPropertyRelative("component")
                                   .objectReferenceValue;
                if (compRef == null)
                    Debug.Log($"Missing script on GameObject: {go.name}", go);
            }
        }
    }
}

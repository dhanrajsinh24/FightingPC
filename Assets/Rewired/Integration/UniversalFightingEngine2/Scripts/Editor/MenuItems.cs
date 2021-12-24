namespace Rewired.Integration.UniversalFightingEngine.Editor {
    using UnityEngine;
    using UnityEditor;

    public static class MenuItems {

        private const string assetGuid_rewiredInputManager = "0ac6769e68424234396411600a35469d";

        [MenuItem(Rewired.Consts.menuRoot + "/Integration/Universal Fighting Engine 2/Create/Rewired Input Manager")]
        [MenuItem("GameObject/Create Other/Rewired/Integration/Universal Fighting Engine 2/Rewired Input Manager")]
        public static void CreateInputManager() {
            if(!InstantiatePrefabAtGuid(assetGuid_rewiredInputManager, "Rewired UFE2 Input Manager", true)) {
                Debug.LogError("Unable to locate prefab file. Please reinstall the Universal Fighting Engine 2 integration pack.");
            }
        }

        private static bool InstantiatePrefabAtGuid(string guid, string name, bool breakPrefabInstance) {
            GameObject prefab = LoadAssetAtGuid<GameObject>(guid);
            if(prefab == null) return false;

            GameObject instance = GameObject.Instantiate(prefab);
            if(instance == null) return false;

            if(!string.IsNullOrEmpty(name)) {
                instance.name = name;
            } else {
                // Strip (Clone) off the end of the name
                if(instance.name.EndsWith("(Clone)")) {
                    instance.name = instance.name.Substring(0, instance.name.Length - 7);
                }
            }

#if !UNITY_2018_3_OR_NEWER
            if(breakPrefabInstance) PrefabUtility.DisconnectPrefabInstance(instance);
#endif
            Undo.RegisterCreatedObjectUndo(instance, "Create " + prefab.name);
            return true;
        }

        private static T LoadAssetAtGuid<T>(string guid) where T : Object {
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
        }
    }
}
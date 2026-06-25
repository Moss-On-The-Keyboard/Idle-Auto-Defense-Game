using Moss.IdleAutoDefense;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Moss.IdleAutoDefense.Editor
{
    public static class MossIdleAutoDefenseDevMenu
    {
        private const string MenuRoot = "Tools/Deucarian/Moss Idle Auto Defense/";
        private const string SmokeScenePath = "Assets/Moss/IdleAutoDefense/Scenes/MossIdleAutoDefense.unity";

        [MenuItem(MenuRoot + "Open Smoke Scene", priority = 10)]
        public static void OpenSmokeScene()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            EditorSceneManager.OpenScene(SmokeScenePath);
        }

        [MenuItem(MenuRoot + "Reset Dev Save", priority = 20)]
        public static void ResetDevSave()
        {
            bool deleted = MossIdleAutoDefenseSave.Reset();
            EditorUtility.DisplayDialog(
                "Moss Idle Auto Defense",
                deleted ? "Deleted the Moss dev save." : "No Moss dev save was found.",
                "OK");
            Debug.Log("[Moss Idle Auto Defense] " + (deleted ? "Dev save reset." : "No dev save found.") + " Path: " + MossIdleAutoDefenseSave.SaveFilePath);
        }
    }
}

using UnityEditor;
using UnityEngine;

namespace YooTools.ProjectFolders.Editor {
    [CustomEditor(typeof(Folder))]
    public class FolderEditor : UnityEditor.Editor {
        [MenuItem("YooTools/Create Folder")]
        public static void Execute() {
            var assets = GenerateFolderStructure();
            CreateFolders(assets);

            AssetDatabase.Refresh();

            if (!AssetDatabase.IsValidFolder($"Assets/{Application.productName}")) {
                return;
            }

            if (AssetDatabase.IsValidFolder($"Assets/{Application.productName}/Settings")) {
                if (AssetDatabase.IsValidFolder($"Assets/{Application.productName}/Settings/Renderer")) {
                    MoveAsset("DefaultVolumeProfile.asset", "Settings/Renderer/DefaultVolumeProfile.asset");
                    MoveAsset("UniversalRenderPipelineGlobalSettings.asset",
                        "Settings/Renderer/UniversalRenderPipelineGlobalSettings.asset");
                    MoveAsset("Settings/Renderer2D.asset", "Settings/Renderer/Renderer2D.asset");
                    MoveAsset("Settings/UniversalRP.asset", "Settings/Renderer/UniversalRP.asset");
                    MoveAsset("Settings/Lit2DSceneTemplate.scenetemplate",
                        "Settings/Renderer/Lit2DSceneTemplate.scenetemplate");
                    MoveAsset("Settings/Scenes/URP2DSceneTemplate.unity",
                        "Settings/Renderer/Scenes/URP2DSceneTemplate.unity");

                    AssetDatabase.Refresh();

                    MoveAsset("Settings/Mobile_Renderer.asset", "Settings/Renderer/Mobile_Renderer.asset");
                    MoveAsset("Settings/Mobile_RPAsset.asset", "Settings/Renderer/Mobile_RPAsset.asset");
                    MoveAsset("Settings/PC_Renderer.asset", "Settings/Renderer/PC_Renderer.asset");
                    MoveAsset("Settings/PC_RPAsset.asset", "Settings/Renderer/PC_RPAsset.asset");
                    MoveAsset("Settings/SampleSceneProfile.asset", "Settings/Renderer/SampleSceneProfile.asset");

                    AssetDatabase.Refresh();

                    MoveAsset("Settings/HDRP Balanced.asset", "Settings/Renderer/HDRP Balanced.asset");
                    MoveAsset("Settings/HDRP High Fidelity.asset", "Settings/Renderer/HDRP High Fidelity.asset");
                    MoveAsset("Settings/HDRP Performant.asset", "Settings/Renderer/HDRP Performant.asset");
                    MoveAsset("Settings/SkyandFogSettingsProfile.asset",
                        "Settings/Renderer/SkyandFogSettingsProfile.asset");

                    AssetDatabase.Refresh();

                    AssetDatabase.DeleteAsset("Assets/Settings");
                }

                if (AssetDatabase.IsValidFolder($"Assets/{Application.productName}/Settings/Resources")) {
                    MoveAsset("InputSystem_Actions.inputactions",
                        "Settings/Resources/InputSystem_Actions.inputactions");
                    AssetDatabase.Refresh();
                }
            }

            if (AssetDatabase.IsValidFolder($"Assets/{Application.productName}/Scenes")) {
                MoveAsset("Scenes/SampleScene.unity", "Scenes/SampleScene.unity");
                MoveAsset("OutDoorsScene.unity", "Scenes/OutDoorsScene.unity");
                AssetDatabase.DeleteAsset("Assets/Scenes");
                AssetDatabase.Refresh();
            }

            MoveAsset("YooTools.dll", "Plugins/YooTools.dll");

            AssetDatabase.DeleteAsset("Assets/Readme.asset");
            AssetDatabase.DeleteAsset("Assets/TutorialInfo");

            AssetDatabase.Refresh();
        }

        [MenuItem("YooTools/Create Folder", true, 0)]
        public static bool ValidateExecute() {
            return !AssetDatabase.IsValidFolder($"Assets/{Application.productName}");
        }

        private static Folder GenerateFolderStructure() {
            var rootFolder = new Folder("Assets", "");

            var subFolder = rootFolder.Add(Application.productName);
            subFolder.Add("Effects");
            subFolder.Add("Scenes");

            var pluginFolder = subFolder.Add("Plugins");
            pluginFolder.Add("Editor");

            subFolder.Add("Prefabs");
            subFolder.Add("Scriptables");
            subFolder.Add("Tests");

            var scriptFolder = subFolder.Add("Scripts");
            scriptFolder.Add("Editor");
            scriptFolder.Add("Runtime");

            var artFolder = subFolder.Add("Art");
            artFolder.Add("Animations");
            artFolder.Add("Fonts");
            artFolder.Add("Materials");
            artFolder.Add("Meshes");
            artFolder.Add("Textures");
            artFolder.Add("Shaders");
            artFolder.Add("Sprites");
            artFolder.Add("Audio");

            var uiFolder = subFolder.Add("UIToolkit");
            uiFolder.Add("Layouts");
            uiFolder.Add("Settings");
            uiFolder.Add("Styles");
            uiFolder.Add("Theme");

            var settingsFolder = subFolder.Add("Settings");
            settingsFolder.Add("Presets");
            settingsFolder.Add("Resources");

            var rendererFolder = settingsFolder.Add("Renderer");
            rendererFolder.Add("Scenes");

            return rootFolder;
        }

        private static void CreateFolders(Folder rootFolder) {
            if (!AssetDatabase.IsValidFolder(rootFolder.CurrentFolder)) {
                Debug.Log($"Creating: <b>{rootFolder.CurrentFolder}</b>");

                AssetDatabase.CreateFolder(rootFolder.ParentFolder, rootFolder.Name);

                File.Create(
                    Directory.GetCurrentDirectory()
                    + Path.DirectorySeparatorChar
                    + rootFolder.CurrentFolder
                    + Path.DirectorySeparatorChar
                    + ".keep"
                );

                Debug.Log($"Creating '.keep' file in: <b>{rootFolder.CurrentFolder}</b>");
            } else {
                if (Directory.GetFiles(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar +
                                       rootFolder.CurrentFolder).Length < 1) {
                    File.Create(
                        Directory.GetCurrentDirectory()
                        + Path.DirectorySeparatorChar
                        + rootFolder.CurrentFolder
                        + Path.DirectorySeparatorChar
                        + ".keep"
                    );
                } else {
                    Debug.Log($"Directory <b>{rootFolder.CurrentFolder}</b> already exists");
                }
            }

            foreach (var folder in rootFolder.Folders) {
                CreateFolders(folder);
            }
        }

        private static void MoveAsset(string oldPath, string newPath) {
            var sourcePath = $"Assets/{oldPath}";

            var destinationFolder = $"Assets/{Application.productName}/{newPath}";
            var error = AssetDatabase.MoveAsset(sourcePath, destinationFolder);

            if (!string.IsNullOrEmpty(error)) {
                Debug.LogError($"Failed to move: {error}");
            }
        }
    }
}
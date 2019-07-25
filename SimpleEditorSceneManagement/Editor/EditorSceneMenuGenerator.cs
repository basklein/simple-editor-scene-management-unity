using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

/* Made by Jeroen Endenburg
 * Expanded upon by Bas Klein
 * Created: 13-04-2016
 * Last update: 18-12-2017
 */

namespace SimpleEditorSceneManagement
{
    public class EditorSceneMenuGenerator
    {
        // The actual code. Don't touch if you don't know what you're doing!
        [MenuItem("Scenes/Get All &c")]
        public static void GenerateScript()
        {
            try
            {
                if (EditorSceneMenuSettings.editorPath.Contains("Editor"))
                {
                    List<string> paths = new List<string>(AssetDatabase.GetAllAssetPaths());
                    List<string> scenePaths = new List<string>();

                    foreach (string path in paths)
                    {
                        string[] pathSplit = path.Split('.');
                        if (pathSplit.Length > 0)
                        {
                            if (pathSplit[pathSplit.Length - 1] == "unity")
                                scenePaths.Add(path);
                        }
                    }

                    using (StreamWriter writer = new StreamWriter(EditorSceneMenuSettings.editorPath + "EditorSceneMenus.cs"))
                    {
                        writer.Write("using UnityEditor;");
                        writer.Write("namespace SimpleEditorSceneManagement {");
                        writer.Write("public class EditorSceneMenus {");

                        int priority = 0;

                        foreach (string scenePath in scenePaths)
                        {
                            string subfolder = "";
                            string[] splitPath = scenePath.Split('/');
                            string sceneMenuName = splitPath[splitPath.Length - 1].Split('.')[0];
                            string sceneName = sceneMenuName.Replace("\"", "\\\"").Replace("#", "Hashtag").Replace("(", "POpen").Replace(")", "PClose").Replace("*", "Asterisk").Replace("[", "BOpen").Replace("]", "BClose").Replace("-", "Dash");

                            for (int i = 0; i < EditorSceneMenuSettings.orderSetString.Length; i++)
                            {
                                if (sceneMenuName.Contains(EditorSceneMenuSettings.orderSetString[i]))
                                {
                                    priority = i * 50;
                                    if (EditorSceneMenuSettings.orderSetFolder.Length > i)
                                    {
                                        if (EditorSceneMenuSettings.orderSetFolder[i] == true)
                                            subfolder = EditorSceneMenuSettings.orderSetString[i] + "/";
                                    }
                                    break;
                                }
                                else
                                    priority = (EditorSceneMenuSettings.orderSetString.Length + 1) * 50;
                            }

                            if (priority > (EditorSceneMenuSettings.orderSetString.Length * 50) && EditorSceneMenuSettings.hideOthers)
                                Debug.Log(sceneMenuName + " was hidden.");
                            else
                                WriteSceneData(writer, scenePath, sceneMenuName, sceneName, subfolder, priority);
                        }

                        writer.Write("}");
                    }

                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogErrorFormat("Could not generate script. Path {0} is not a valid path. Please enter a valid Editor folder path.", EditorSceneMenuSettings.editorPath);
                }
            }
            catch
            {
                Debug.LogError("Something went wrong with Simple Editor Scene Management. Please try again or reset.");
            }
        }

        public static void WriteSceneData(StreamWriter writer, string scenePath, string sceneMenuName, string sceneName, string subfolder, int priority = 0)
        {
            writer.Write("[MenuItem(\"Scenes/" + subfolder + "OPEN " + sceneMenuName + "\", false, " + priority.ToString() + ")]");
            writer.Write("public static void OpenScene_" + sceneName.Replace(' ', '_') + "() {");
            writer.Write("EditorSceneMenuGenerator.OpenScene(\"" + scenePath + "\");");
            writer.Write("}}");
        }

        public static void OpenScene(string path)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(path);
            }
        }

        [MenuItem("Scenes/Reset")]
        public static void DestroyScript()
        {
            string scriptPath = EditorSceneMenuSettings.editorPath + "EditorSceneMenus.cs";
            if (File.Exists(scriptPath))
            {
                File.Delete(scriptPath);
                File.Delete(scriptPath + ".meta");
            }
            AssetDatabase.Refresh();
        }
    }
}
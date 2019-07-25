using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

/* Made by Bas Klein
 * Created: 22-05-2017
 * Last update: 25-07-2019
 */

namespace SimpleEditorSceneManagement
{
    public class EditorSceneMenuGeneratorWizard : ScriptableWizard
    {
        static string editorPath = null;
        static OrderElement[] orderSet = null;
        static bool hideOthers = false;

        /* The folder that the menu script will be generated to. Has to be an Editor folder.
         * Default: "Assets/SimpleSceneManagement/Editor/"
         */
        public string EditorPath = null;
        /* 
         */
        public OrderElement[] OrderSet = null;
        /* Used to dictate whether or not the scenes that aren't mentioned in the array above should appear in the list of MenuItems.
         * true = hidden
         * false = shown (default)
         */
        public bool HideOthers = false;

        // The actual code. Don't touch if you don't know what you're doing!
        [MenuItem("Scenes/Settings")]
        public static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard(
                "Change Scene Management Settings", typeof(EditorSceneMenuGeneratorWizard), "Save Settings", "Cancel");
        }

        public EditorSceneMenuGeneratorWizard()
        {
            Vector2 wizardPosition = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            Vector2 wizardSize = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

            position = new Rect(wizardPosition, wizardSize);
        }

        private void OnEnable()
        {
            try
            {
                string settingsPath = "";
                if (SettingsExist(out settingsPath))
                {
                    using (StreamReader reader = new StreamReader(settingsPath))
                    {
                        string settings = reader.ReadLine().Replace("\\", "").Replace("*", "").Replace(",", "").Replace("\"", "");
                        string editorSplit = settings.Split('¹')[1];
                        editorSplit = editorSplit.Remove(0, 1);
                        editorSplit = editorSplit.Remove(editorSplit.Length - 1, 1);
                        EditorPath = editorSplit;

                        string hideSplit = settings.Split('²')[1];
                        hideSplit = hideSplit.Remove(0, 1);
                        hideSplit = hideSplit.Remove(hideSplit.Length - 1, 1);
                        HideOthers = (hideSplit == "true");

                        List<string> order = new List<string> { };
                        string[] orderSplit = settings.Split('³');
                        int orderLength = int.Parse(orderSplit[1].Remove(orderSplit[1].Length - 2, 2));
                        if (orderLength > 0)
                        {
                            for (int i = 2; i < orderLength + 2; i++)
                            {
                                orderSplit[i] = orderSplit[i].Remove(0, 1);
                                orderSplit[i] = orderSplit[i].Remove(orderSplit[i].Length - 1, 1);
                                order.Add(orderSplit[i]);
                            }
                        }

                        List<OrderElement> orderTemp = new List<OrderElement>();

                        for (int i = 0; i < order.Count; i++)
                        {
                            OrderElement orderElementTemp = new OrderElement();
                            orderElementTemp.FilterString = order[i];
                            orderTemp.Add(orderElementTemp);
                        }

                        List<bool> folder = new List<bool> { };
                        string[] folderSplit = settings.Split('⁴');
                        int folderLength = int.Parse(folderSplit[1].Remove(folderSplit[1].Length - 2, 2));
                        if (folderLength > 0)
                        {
                            for (int i = 2; i < folderLength + 2; i++)
                            {
                                folderSplit[i] = folderSplit[i].Remove(0, 1);
                                folderSplit[i] = folderSplit[i].Remove(folderSplit[i].Length - 1, 1);
                                folder.Add(bool.Parse(folderSplit[i]));
                            }
                        }

                        for (int i = 0; i < order.Count; i++)
                        {
                            orderTemp[i].SetAsSubFolder = folder[i];
                        }

                        OrderSet = orderTemp.ToArray();
                    }
                }
                else
                {
                    EditorPath = "Assets/SimpleEditorSceneManagement/Editor/";
                    OrderSet = new OrderElement[] { };
                    HideOthers = false;
                }
            }
            catch
            {
                Debug.LogError("Something went wrong with Simple Editor Scene Management. Please try again or reset.");
            }
        }

        public static void WriteSettings()
        {
            try
            {
                if (editorPath.Contains("Editor"))
                {
                    if (editorPath[editorPath.Length - 1] != '/')
                        editorPath += "/";

                    using (StreamWriter writer = new StreamWriter(editorPath + "EditorSceneMenuSettings.cs"))
                    {
                        writer.Write("namespace SimpleEditorSceneManagement {");
                        writer.Write("public class EditorSceneMenuSettings {");

                        writer.Write("public static string editorPath = /*¹*/\"" + editorPath + "\"/*¹*/;");
                        writer.Write("public static bool hideOthers = /*²*/" + hideOthers.ToString().ToLower() + "/*²*/;");

                        writer.Write("public static string[] orderSetString = new string[] {/*³" + orderSet.Length.ToString() + "*//*³*/");
                        foreach (OrderElement order in orderSet)
                        {
                            writer.Write("\"" + order.FilterString + "\"" + ",/*³*/");
                        }
                        writer.Write("};");

                        writer.Write("public static bool[] orderSetFolder = new bool[] {/*⁴" + orderSet.Length.ToString() + "*//*⁴*/");
                        foreach (OrderElement order in orderSet)
                        {
                            writer.Write(order.SetAsSubFolder.ToString().ToLower() + ",/*⁴*/");
                        }
                        writer.Write("};");

                        writer.Write("}}");
                    }

                    AssetDatabase.Refresh();

                    EditorSceneMenuGenerator.GenerateScript();
                }
                else
                {
                    Debug.LogErrorFormat("Could not generate script. Path {0} is not a valid path. Please enter a valid Editor folder path.", editorPath);
                }

                AssetDatabase.Refresh();
            }
            catch
            {
                Debug.LogError("Something went wrong with Simple Editor Scene Management. Please try again or reset.");
            }
        }

        private void OnWizardUpdate()
        {
            editorPath = EditorPath;
            orderSet = OrderSet;
            hideOthers = HideOthers;
        }

        private void OnWizardCreate()
        {
            if (AllApproved())
                WriteSettings();
        }

        private void OnWizardOtherButton()
        {
            Close();
        }

        public bool AllApproved()
        {
            if (editorPath != null)
            {
                if (!editorPath.Contains("Editor") || !editorPath.Contains("Assets"))
                {
                    Debug.LogError("Editor path is invalid. Please change it to the appropriate folder.");
                    return false;
                }
                foreach (OrderElement order in orderSet)
                {
                    if (order == null || order.FilterString == "")
                    {
                        Debug.LogError("A string in the order setter array is empty. Please remove this element or fill it in.");
                        return false;
                    }
                }
                return true;
            }
            else return false;
        }

        public bool SettingsExist(out string settingsPath)
        {
            List<string> paths = new List<string>(AssetDatabase.GetAllAssetPaths());
            settingsPath = "";

            foreach (string path in paths)
            {
                string[] pathSplit = path.Split('/');
                if (pathSplit.Length > 0)
                {
                    if (pathSplit[pathSplit.Length - 1] == "EditorSceneMenuSettings.cs")
                    {
                        settingsPath = path;
                        return true;
                    }
                }
            }
            return false;
        }
    }

    [System.Serializable]
    public class OrderElement
    {
        /* The string that will be detected in the name of the scene file. The MenuItems' priority will be set according to the order in which the strings appear.
         * Meaning that you can order the way in which the MenuItems will appear by writing certain tags in the name of the scene file.
         * eg. "[Level] Level 1".unity and "[Level] Level 2".unity will appear in the same area when you enter the string "[Level]" in this array.
         * If you for example also enter "[Menu]" into the array before "[Level]", the "[Menu]" scenes will appear before the "[Level]" scenes.
         * Note: you don't have to write it in brackets, it's just more reliable that way.
         */
        public string FilterString = "";
        /* Used to dictate whether or not scenes should be grouped in a fold-out menu within the scene selection menu.
         */
        public bool SetAsSubFolder = false;
    }
}
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
public class DataNewWindow : EditorWindow
{
    public static DataNewWindow window;
    public static string panelTitle = "创建一个XML数据文件";
    public static string defaultName = "Test";
    public static string extensionType = "xml";
    static public Rect inputTextRect = new Rect(20, 70, 248, 50);
    static public Rect PopupRect = new Rect(274, 70, 196, 50);
    static public Rect dataRect = new Rect(20, 50, 480, 350);
    static public Rect UserInputdataRect = new Rect(16, 100, 480, 260);
    static public float globalindexWidth = 20;
    static public int sizeIntboolArrayCheckboxes = 30;
    static public bool[] boolArrayCheckboxes = new bool[30];
    static public int fixedCellSize = 200;
    static public int boxTextBuffer = 40;
    public static string directory;
    public static bool isCancelSelected = false;
    public static string dataName;
    public static string dataPath;

    public static string XMLName;
    static public string currentRowNameInputLine;
    static public int selGridIntColumnDeclaration;
    static public string currentRowTypeInputLine;
    static public Vector2 scrollPosition;
    static public bool showButtonFour = false;
    static public string[] listOfRowInfo;
    static public Queue<string> queueOfUserInputs = new Queue<string>();
    public static int userSelectedContent;
    static public string[] listofGuiCheckboxes;
    static public string[] localListofGuiCheckboxes;
    static public int indexToInvertColumns = -1;
    static public string currentString;
    static public float allTextwidth;
    public static void OpenWindow()
    {
        directory = SetDefaultDirectory();
        isCancelSelected = false;
        string tempFileNameString = "";
        if (directory != ".")
        {
            tempFileNameString = EditorUtility.SaveFilePanel(panelTitle, directory, defaultName, extensionType);
        }
        else
        {
            tempFileNameString = EditorUtility.SaveFilePanel(panelTitle, "", defaultName, extensionType);
        }
        if (tempFileNameString != "")
        {
            dataName = Path.GetFileName(tempFileNameString);
            dataPath = tempFileNameString;
        }
        else
        {
            DataEditorWindow.SetDataLoaded(false);
            isCancelSelected = true;
        }
        if (!isCancelSelected)
        {
            window = (DataNewWindow)GetWindow(typeof(DataNewWindow));
            window.Show();
        }
    }
    public void OnGUI()
    {
        XMLName = EditorGUILayout.TextField("XML文件名称", XMLName);
        if (!string.IsNullOrEmpty(XMLName))
            XMLName = XMLName.Replace(" ", "");
        FieldTitleAndTypeofData();
        GUILayout.BeginArea(inputTextRect);
        currentRowNameInputLine = EditorGUILayout.TextField(currentRowNameInputLine);
        GUILayout.EndArea();
        if (!string.IsNullOrEmpty(currentRowNameInputLine))
        {
            currentRowNameInputLine = currentRowNameInputLine.Replace(" ", "");
        }
        CreateNewDropDownMenu();
        GUILayout.Space(80);
        listOfRowInfo = queueOfUserInputs.ToArray();
        GUILayout.BeginArea(UserInputdataRect);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, true, true, GUILayout.Height(Screen.height - 190));
        int intTotalDataInRow = 0;
        if (null == listOfRowInfo)
        {
            showButtonFour = false;
        }
        else
        {
            intTotalDataInRow = 2;
            SendDataToSpreadSheetGridCheckbox(listOfRowInfo, intTotalDataInRow);
        }      
    }
    public static string SetDefaultDirectory()
    {
        string path = Application.dataPath + "/Resources/Config/";
        DirectoryInfo di = new DirectoryInfo(path);
        if (di.Exists == false)
        {
            path = ".";
            Debug.Log("不存在该文件夹"+di.ToString());
        }

        if (Application.platform == RuntimePlatform.OSXEditor)
        {
            return path;
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            string forwardSlash = "/";
            String backwardSlash = "\\";
            path = path.Replace(forwardSlash, backwardSlash);
            return path;
        }
        else
        {
            Debug.LogError("You are not using a supported platform");
            Application.Quit();
            return "Garbage";
        }
    }
    public static void FieldTitleAndTypeofData()
    {
        try
        {
            GUILayout.BeginArea(dataRect);
            GUIContent[] listOfRowTitleInfo = new GUIContent[2];
            listOfRowTitleInfo[0] = new GUIContent("XML数据头部名称");
            listOfRowTitleInfo[1] = new GUIContent("XML数据头部类型");
            int intTotalDataInRow = 2 + 1;  //Add 1 for the checkbox...

            SendDataToHeaderCheckbox(listOfRowTitleInfo, intTotalDataInRow);
            GUILayout.EndArea();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    public static void SendDataToHeaderCheckbox(GUIContent[] listOfRowNames, int intTotalDesiredData)
    {
        EditorGUILayout.BeginHorizontal();
        CheckBox();
        GUILayout.Label("", new GUIStyle(GUI.skin.label), GUILayout.Width(globalindexWidth));
        for (var i = 0; i < listOfRowNames.Length; i++)
        {
            float TextWidth = 200;
            GUILayout.Label(listOfRowNames[i].text, new GUIStyle(GUI.skin.label), GUILayout.Width(TextWidth));           
        }
        EditorGUILayout.EndHorizontal();
    }
    public static void SendDataToSpreadSheetGridCheckbox(string[] listOfGuiContent, int intTotalDesiredData)
    {
        GUI.SetNextControlName("MyTextField");
        Queue<string> queueOfCheckboxes = new Queue<string>();
        int gridMonitor = 0;
        int column = 0;
        int columnCurrent = 0;
        bool firstVisit = true;
        bool secondVisit = false;

        int totalData = 0;
        int totalCheckboxes = 1;
        for (int i = 0; i < listOfGuiContent.Length; i++)
        {
            if (firstVisit)
            {
                EditorGUILayout.BeginHorizontal();
                if (Event.current.type == EventType.MouseDown)
                {
                    userSelectedContent = totalData;
                }
                int currentCheckbox = totalCheckboxes - 1;
                string currentCheckStatus = ListOfCheckbox(currentCheckbox, listofGuiCheckboxes);
                queueOfCheckboxes.Enqueue(currentCheckStatus);
                firstVisit = false;
                secondVisit = true;
                i--;
                totalData++;
                totalCheckboxes++;
            }
            else if (secondVisit)
            {
                if (firstVisit)
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        userSelectedContent = totalData;
                    }
                    if (indexToInvertColumns == columnCurrent)
                    {
                        string tempString = "" + columnCurrent;
                        int boxTextBuffer = 20;
                        float indexWidth = GUI.skin.GetStyle("Label").CalcSize(new GUIContent(tempString)).x;
                        currentString = GUILayout.TextField(tempString, GUI.skin.textField, GUILayout.Width(indexWidth + boxTextBuffer));
                    }
                    else
                    {
                        int boxTextBuffer = 20;
                        string tempString = "" + columnCurrent;
                        float indexWidth = GUI.skin.GetStyle("Label").CalcSize(new GUIContent(tempString)).x;
                        if (indexWidth > globalindexWidth)
                        {
                            globalindexWidth = indexWidth;
                        }
                        if (columnCurrent % 2 == 0)
                        {
                            currentString = GUILayout.TextField(tempString, GUI.skin.textField, GUILayout.Width(globalindexWidth));
                        }
                        else
                        {
                            currentString = GUILayout.TextField(tempString, GUI.skin.textField, GUILayout.Width(globalindexWidth));
                        }
                    }
                }
                else
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        userSelectedContent = totalData;
                    }
                    float TextWidth = CalCulateCellSize(listOfGuiContent[i], intTotalDesiredData, true);
                    SetSpreadSheetGridData(listOfGuiContent, GUI.skin.textField, GUI.skin.textField, TextWidth, i, gridMonitor);
                }
                secondVisit = false;
                i--;
                totalData++;
            }
            else if (column < intTotalDesiredData - 1)
            {

            }
        }
    }
    public static void SetSpreadSheetGridData(string[] listOfGuiContent, GUIStyle guiStyleWhite, GUIStyle guiStyleGray, float TextWidth, int index, int gridMonitor)
    {
        if (allTextwidth > TextWidth)
        {
            TextWidth = allTextwidth;
        }
        float tempMod = gridMonitor % 2;
        if (tempMod == 0)
        {
            if (listOfGuiContent[index] != null)
            {
                string tempString = listOfGuiContent[index];
                tempString = tempString.Replace(" ", "");
                currentString = GUILayout.TextArea(listOfGuiContent[index], guiStyleWhite, GUILayout.Width(allTextwidth));
            }
        }
        else
        {
            if (listOfGuiContent[index] != null)
            {
                string oddtempString = listOfGuiContent[index];
                oddtempString = oddtempString.Replace(" ", "");
                currentString = GUILayout.TextArea(listOfGuiContent[index], guiStyleGray, GUILayout.Width(allTextwidth));
            }
        }
    }
    public static float CalCulateCellSize(string inputString, int intTotalDataInRow, bool fixedSizeBool)
    {
        if (fixedSizeBool)
        {
            allTextwidth = fixedCellSize;
        }
        else
        {
            float currentInputTextWidth = GUI.skin.GetStyle("Label").CalcSize(new GUIContent(inputString)).x; ;
            float TextWidth = currentInputTextWidth + boxTextBuffer;
            if (allTextwidth < TextWidth)
            {
                if ((allTextwidth * intTotalDataInRow) > (Screen.width - 50))
                {
                    if (intTotalDataInRow < 3)
                    {
                        allTextwidth = Mathf.Clamp(TextWidth, boxTextBuffer, Screen.width / intTotalDataInRow);
                    }
                    else
                    {
                        allTextwidth = Mathf.Clamp(TextWidth, boxTextBuffer, Screen.width / 3);
                    }
                }
                else
                {
                    allTextwidth = TextWidth;
                }
            }
        }
        return allTextwidth;
    }
    public static string ListOfCheckbox(int index, string[] listOfCheckboxes)
    {
        string userCheckbox = "";
        string tempString = "null"; //Null input
        float currentInputTextWidth = GUI.skin.GetStyle("Label").CalcSize(new GUIContent(tempString)).x;
        if (index < sizeIntboolArrayCheckboxes)
        {
            boolArrayCheckboxes[index] = GUILayout.Toggle(boolArrayCheckboxes[index], "", GUILayout.Width(currentInputTextWidth));
            if (boolArrayCheckboxes[index] == true)
            {
                userCheckbox = "true";
            }
            else
            {
                userCheckbox = "false";
            }
        }
        else
        {
            sizeIntboolArrayCheckboxes = localListofGuiCheckboxes.Length + 30;
            boolArrayCheckboxes = new bool[sizeIntboolArrayCheckboxes];
            boolArrayCheckboxes[index] = GUILayout.Toggle(boolArrayCheckboxes[index], "", GUILayout.Width(currentInputTextWidth));
            if (boolArrayCheckboxes[index] == true)
            {
                userCheckbox = "true";
            }
            else
            {
                userCheckbox = "false";
            } 
        }
        return userCheckbox;
    }
    public static void CheckBox()
    {
        float currentInputTextWidth = GUI.skin.GetStyle("Label").CalcSize(new GUIContent("null")).x;
        GUILayout.Toggle(false, "", GUILayout.Width(currentInputTextWidth));
    }
    public static void CreateNewDropDownMenu()
    {
        Queue<string> tempQueue = new Queue<string>();
        tempQueue.Enqueue("请选择一个数据类型");
        tempQueue.Enqueue("string");
        tempQueue.Enqueue("int");
        tempQueue.Enqueue("float");
        tempQueue.Enqueue("bool");
        tempQueue.Enqueue("double");
        tempQueue.Enqueue("long");
        string[] listOfColumnDeclaration = tempQueue.ToArray();
        selGridIntColumnDeclaration = EditorGUI.Popup(PopupRect, selGridIntColumnDeclaration, listOfColumnDeclaration, EditorStyles.popup);
        if (selGridIntColumnDeclaration < 0)
        {
            
        }
        else
        {
            currentRowTypeInputLine = listOfColumnDeclaration[selGridIntColumnDeclaration];
        }
    }
}
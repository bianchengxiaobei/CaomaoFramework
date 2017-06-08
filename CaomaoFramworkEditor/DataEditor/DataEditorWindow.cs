using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class DataEditorWindow : EditorWindow
{
    public static DataEditorWindow window;
    public static bool dataLoaded;
    private Texture newImage;
    private Texture openImage;
    private Texture saveImage;
    [MenuItem("Window/数据编辑窗口")]
    public static void Init()
    {
        window = (DataEditorWindow)EditorWindow.GetWindow(typeof(DataEditorWindow));
        window.Show();
    }
    public void OnGUI()
    {
        TopIconToolbar();
    }

    
    private void OnEnable()
    {
        newImage = Resources.Load("EditorTextures/new") as Texture;
        openImage = Resources.Load("EditorTextures/open") as Texture;
        saveImage = Resources.Load("EditorTextures/save") as Texture;
    }

    public static void SetDataLoaded(bool value)
    {
        dataLoaded = value;
    }
    private void TopIconToolbar()
    {
        try
        {
            GUILayout.BeginArea(new Rect(10, 0, Screen.width, 80));
            if (dataLoaded)
            {

            }
            else
            {
                CreateNoLoadedDatabaseToolbar();
            }
            GUILayout.EndArea();
        }
        catch (Exception e)
        {

        }
    }
    private Texture[] LoadTextureToolbarImage()
    {
        Queue texturedQueue = new Queue();
        texturedQueue.Enqueue(newImage);
        texturedQueue.Enqueue(openImage);
        Texture[] texturedArray = TexturedQueueToTexturedArray(texturedQueue);
        return texturedArray;
    }
    private Texture[] TexturedQueueToTexturedArray(Queue texturedQueue)
    {
        int size = texturedQueue.Count;
        Texture[] arrayOfTextures = new Texture[size];

        for (int i = 0; i < size; i++)
        {
            arrayOfTextures[i] = texturedQueue.Dequeue() as Texture;
        }
        return arrayOfTextures;
    }
    private bool IsClickedGuilayoutTexture(Texture buttonTexture)
    {
        float textureWidth = GetGuiButtonTextureWidth(buttonTexture);
        if (GUILayout.Button(buttonTexture, GUILayout.Width(textureWidth)))
        {
            return true;
        }
        else return false;
    }
    private float GetGuiButtonTextureWidth(Texture inputTexture)
    {
        float textureWidth = 0;
        textureWidth = GUI.skin.GetStyle("Button").CalcSize(new GUIContent(inputTexture)).x;
        return textureWidth;
    }
    private void CreateNoLoadedDatabaseToolbar()
    {
        GUILayout.BeginArea(new Rect(10, 5, Screen.width, Screen.height));
        EditorGUILayout.BeginHorizontal();
        Texture[] noDatabaseLoadedToolbar = this.LoadTextureToolbarImage();
        if (IsClickedGuilayoutTexture(noDatabaseLoadedToolbar[0]))
        {
            this.OpenNewWindow();
        }
        else if (IsClickedGuilayoutTexture(noDatabaseLoadedToolbar[1]))
        {

        }
        EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
    private void OpenNewWindow()
    {
        DataNewWindow.OpenWindow();
    }
}

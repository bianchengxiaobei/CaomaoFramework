using System;
using System.Collections.Generic;
using CaomaoFramework;
public class DataSceneConfig : GameData<DataSceneConfig>
{
    public static readonly string fileName = "sceneConfig";
    public int ID
    {
        get;
        private set;
    }
    public string SceneName
    {
        get;
        private set;
    }
    public string ChapterName
    {
        get;
        private set;
    }
    public string StoryDSLFile
    {
        get;
        private set;
    }
    public float EnterX
    {
        get;
        private set;
    }
    public float EnterZ
    {
        get;
        private set;
    }
}

namespace CaomaoFramework.Scene
{
    public class SceneSaveData : GameData<SceneSaveData>
    {
        public const string fileName = "sceneSaveData";
        public int ID
        {
            get;set;
        }
        public int MapId
        {
            get;
            set;
        }
        public string SceneName
        {
            get;set;
        }
    }
}

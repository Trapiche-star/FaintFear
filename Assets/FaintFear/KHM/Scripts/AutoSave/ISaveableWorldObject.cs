namespace FaintFear
{
    public interface ISaveableWorldObject
    {
        string GetID();
        void Save(ref SaveData data);
        void Load(SaveData data);
    }
}
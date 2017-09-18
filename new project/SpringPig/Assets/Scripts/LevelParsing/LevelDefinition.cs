using System.Collections.Generic;

[System.Serializable]
public class Heights
{
    public int Id;
    public int StartHeight;
    public int EndHeight;
}

[System.Serializable]
public class LevelVertical
{
    public string Id;
    public List<Heights> Heights;
}

[System.Serializable]
public class LevelDefinition
{
    public string Name;
    public int Length;
    public int Width;
    public List<object> LevelBase;
    public List<LevelVertical> LevelVertical;
}
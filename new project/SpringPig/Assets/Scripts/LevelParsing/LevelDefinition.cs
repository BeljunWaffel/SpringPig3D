using System.Collections.Generic;

[System.Serializable]
public class Player
{
    public int X;
    public int Y;
    public int Z;
}

[System.Serializable]
public class Heights
{
    public object Id;
    public int StartHeight;
    public int EndHeight;
}

[System.Serializable]
public class VerticalDefinitions
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
    public Player Player { get; set; }
    public List<object> LevelBase;
    public List<VerticalDefinitions> VerticalDefinitions;
}
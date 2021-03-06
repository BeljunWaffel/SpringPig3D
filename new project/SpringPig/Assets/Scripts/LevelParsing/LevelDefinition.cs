﻿using System.Collections.Generic;

[System.Serializable]
public class Player
{
    public int X;
    public int Y;
    public int Z;
}

[System.Serializable]
public class Flag
{
    public int X;
    public int Y;
    public int Z;
}

[System.Serializable]
public class VerticalDefinition
{
    public string Id;
    public int StartHeight;
}

[System.Serializable]
public class VerticalDefinitions
{
    public string Id;
    public List<VerticalDefinition> VerticalDefinition;
}

[System.Serializable]
public class LevelDefinition
{
    public string Name;
    public int X_Scale;
    public int Z_Scale;
    public Player Player { get; set; }
    public Flag Flag { get; set; }
    public List<string> LevelBase;
    public List<VerticalDefinitions> VerticalDefinitions;
}
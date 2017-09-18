using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public static class LevelParser
{
    public static bool ParseLevelJson(string levelName, out LevelDefinition levelDefinition)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "LevelDefinitions/" + levelName);
        if (File.Exists(path))
        {
            var jsonLevel = File.ReadAllText(path);
            try
            {
                levelDefinition = JsonConvert.DeserializeObject<LevelDefinition>(jsonLevel);
            }
            catch (Exception e)
            {
                throw e;
            }

            levelDefinition = null;
            return true;
        }
        else
        {
            Debug.Log("Can't load " + levelName + " from path: " + path);
            levelDefinition = null;
            return false;
        }
    }
}
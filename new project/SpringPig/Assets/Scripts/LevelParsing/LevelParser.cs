using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

            return true;
        }
        else
        {
            Debug.Log("Can't load " + levelName + " from path: " + path);
            levelDefinition = null;
            return false;
        }
    }

    public static bool ParseLevelsList(string levelListPath, out Dictionary<int, string> levelList)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "LevelDefinitions/" + levelListPath);
        if (File.Exists(path))
        {
            var jsonLevelList = File.ReadAllText(path);
            try
            {
                var mappingsList = JsonConvert.DeserializeObject<List<LevelNameNumberMapping>>(jsonLevelList);
                levelList = new Dictionary<int, string>();
                foreach (var mapping in mappingsList)
                {
                    levelList[mapping.LevelNumber] = mapping.Name;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return true;
        }
        else
        {
            Debug.Log("Can't load " + levelListPath + " from path: " + path);
            levelList = null;
            return false;
        }
    }
}
using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public float respawnX = 0;
    public float respawnY = 0;
    public float respawnZ = 0;
    public string lastSceneName = "Stage1";
    public List<string> unlockedWarpIDs = new List<string>();
}

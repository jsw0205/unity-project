using System.Collections.Generic;
using System;

[Serializable]
public class SaveData
{
    public float respawnX;
    public float respawnY;
    public float respawnZ;
    public string lastSceneName = "Stage1";

    public List<WarpPointData> allWarpPoints = new List<WarpPointData>();
    public List<string> unlockedWarpIDs = new List<string>();
    public List<WarpPointData> unlockedWarps = new List<WarpPointData>();



}


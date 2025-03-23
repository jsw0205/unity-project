// WarpPointDatabase.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WarpPointDatabase", menuName = "Warp/WarpPointDatabase")]
public class WarpPointDatabase : ScriptableObject
{
    public List<WarpPointData> defaultWarpPoints = new List<WarpPointData>();
}

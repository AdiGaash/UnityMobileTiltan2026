using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    public PlayerSaveData playerData;
    public TowerSaveData towerData;
    public DifficultyProgressData difficultyData;
}

[System.Serializable]
public class PlayerSaveData
{
    public float positionX;
    public float positionY;
    public float positionZ;
}

[System.Serializable]
public class TowerSaveData
{
    public List<SegmentSaveData> segments;
    public float nextSpawnY;
    public bool[] lastSegmentUpperLadders;
    public bool isFirstSegment;
}

[System.Serializable]
public class SegmentSaveData
{
    public float positionX;
    public float positionY;
    public float positionZ;
    public bool[] ladderSlots;
    public float rotationX;
    public float rotationY;
    public float rotationZ;
}

[System.Serializable]
public class DifficultyProgressData
{
    public int currentLevelIndex;
    public int segmentsGenerated;
}

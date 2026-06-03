using UnityEngine;


[CreateAssetMenu(fileName = "TowerParameters", menuName = "GameSO/TowerParameters", order = 0)]
public class TowerParameters : ScriptableObject
{
    public int MinLadders = 1;
    public int MaxLadders = 3;
    public Material SegMaterial;
}

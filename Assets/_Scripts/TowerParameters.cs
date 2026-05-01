using UnityEngine;

namespace _Scripts
{
    [CreateAssetMenu(fileName = "TowerParameters", menuName = "GameSO", order = 0)]
    public class TowerParameters : ScriptableObject
    {
        public int MinLadders = 1;
        public int MaxLadders = 3;
        public Material SegMaterial;
    }
}
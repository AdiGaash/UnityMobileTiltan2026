using UnityEngine;

[CreateAssetMenu(fileName = "FloatVariable", menuName = "ScriptableObjects/FloatVariable")]
public class FloatVariable : ScriptableObject
{
    [SerializeField]
    public float Value;
}
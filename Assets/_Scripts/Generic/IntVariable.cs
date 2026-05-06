using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "IntVariable", menuName = "ScriptableObjects/IntVariable")]
public class IntVariable : ScriptableObject
{
    [SerializeField] private int value;

    public event UnityAction<int> OnValueChanged = delegate { };
    public int Value
    {
        get => value;
        set
        {
            this.value = value;
            OnValueChanged.Invoke(value);
        }
    }
}
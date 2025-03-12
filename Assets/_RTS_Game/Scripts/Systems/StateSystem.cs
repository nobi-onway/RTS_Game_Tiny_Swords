using System;
using UnityEngine;

[Serializable]
public class EnumSystem<T> where T : Enum
{
    [SerializeField] private T m_value;
    public T Value
    {
        get => m_value;
        private set
        {
            m_value = value;
        }
    }

    public event Action<T> OnValueChange;

    public void SetValue(T newValue)
    {
        OnValueChange?.Invoke(newValue);

        Value = newValue;
    }

    public void SetValue(T oldState, T newState)
    {
        if (!IsCurrentValue(oldState)) return;

        SetValue(newState);
    }

    public bool IsCurrentValue(T value) => Value.CompareTo(value) == 0;
}
using System;
using UnityEngine;

public class EnumSystem<T> where T : Enum
{
    public T Value { get; private set; }

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
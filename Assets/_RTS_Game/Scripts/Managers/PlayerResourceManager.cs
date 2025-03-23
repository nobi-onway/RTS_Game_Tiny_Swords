using System;
using UnityEngine;

public class PlayerResourceManager
{
    private static PlayerResourceManager m_instance;
    public static PlayerResourceManager Instance
    {
        get
        {
            m_instance ??= new();

            return m_instance;
        }
    }

    private int m_gold;
    private int m_wood;

    public int Gold => m_gold;
    public int Wood => m_wood;

    public Action<int, int> OnResourceChange;
    public Action<int, int> OnAddResource;
    public Action<int, int> OnReduceResource;

    public void AddResource(int gold, int wood)
    {
        m_gold += gold;
        m_wood += wood;

        OnResourceChange?.Invoke(m_gold, m_wood);
        OnAddResource?.Invoke(gold, wood);
    }
}
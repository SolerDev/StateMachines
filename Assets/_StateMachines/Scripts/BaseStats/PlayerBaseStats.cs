using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "Character Stats/Player", order = 0)]
public class PlayerBaseStats : BaseStats
{
    [Header("Player Specific")]
    [SerializeField]
    private int axeMax = 100;
    public int AxeMax { get => axeMax; private set => axeMax = value; }

    [Space(10f)]

    [Tooltip("Valor em segundos com duas casas de precisão.")]
    [SerializeField]
    private float invulnerabilityTime = 1.1f;

    [Space(10f)]
    [SerializeField]
    private float dashImpulse = 200f;
    public float DashImpulse { get => dashImpulse; private set => dashImpulse = value; }

    [SerializeField]
    private float dashImpulseUnderwater = 100f;
    public float DashImpulseUnderwater { get => dashImpulseUnderwater; private set => dashImpulseUnderwater = value; }
    
    public int InvulnerabilityFrames
    {
        get => (int)(invulnerabilityTime * 60);
        private set => invulnerabilityTime = (value / 60f);
    }
    
}

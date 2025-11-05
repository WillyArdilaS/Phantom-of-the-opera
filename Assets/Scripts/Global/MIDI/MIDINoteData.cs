using System;
using UnityEngine;

[Serializable]
public class MIDINoteData
{
    [SerializeField, HideInInspector] private string name;
    [SerializeField, HideInInspector] private int channel;
    [SerializeField] private double startTime;
    [SerializeField] private double duration;
    [SerializeField] private bool isActive = true;

    // === Properties ===
    public string Name => name;
    public int Channel => channel;
    public double StartTime => startTime;
    public double Duration => duration;
    public bool IsActive => isActive;

    // === Constructor ===
    public MIDINoteData(string name, int channel, double startTime, double duration) {
        this.name = name;
        this.channel = channel;
        this.startTime = startTime;
        this.duration = duration;
    }
}
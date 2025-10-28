using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChannelSpawnGroup
{
    [SerializeField, HideInInspector] private string channelName;
    [SerializeField] private List<GhostSpawnData> ghostSpawnDataList = new();

    // === Properties ===
    public string ChannelName => channelName;
    public List<GhostSpawnData> GhostDataList => ghostSpawnDataList;

    // === Constructor ===
    public ChannelSpawnGroup(string name)
    {
        channelName = name;
    }
}
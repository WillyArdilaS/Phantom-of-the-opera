using System;

[Serializable]
public class MIDINoteData
{
    public bool isActive = true;
    public string name;
    public int octave;
    public double startTime;
    public double duration;
    public int channel;
}
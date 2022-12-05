using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TBLine
{
    [SerializeField] private string _name;
    [SerializeField] private bool _knownName = true;
    [SerializeField] private string _char;
    [SerializeField] private string _anim;
    [Header("")][SerializeField] private string _blipSound;
    [SerializeField] private bool _skipFade;
    [SerializeField] private bool _stopMusic;
    [SerializeField] private Interjection _interjection;
    [Header("")][SerializeField][TextArea(3, 4)] private string _dialogue;

    public string Name => _name;
    public string Char => _char;
    public string Anim => _anim;
    public bool KnownName => _knownName;
    public string BlipSound => _blipSound;
    public Interjection Interjection => _interjection;
    public bool SkipFade => _skipFade;
    public bool StopMusic => _stopMusic;
    public string Dialogue => _dialogue;
}

public enum Interjection
{
    NA,
    Objection,
    HoldIt,
    TakeThat
}
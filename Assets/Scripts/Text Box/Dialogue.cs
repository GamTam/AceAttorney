using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
    [SerializeField] private bool _addToCourtRecord;
    [SerializeField] private Interjection _interjection;
    [SerializeField] private Align _align;
    [Header("")][SerializeField][TextArea(3, 4)] private string _dialogue;
    [SerializeField] private StateChange _stateChange;

    public string Name => _name;
    public string Char => _char;
    public string Anim => _anim;
    public bool KnownName => _knownName;
    public bool AddToCourtRecord => _addToCourtRecord;
    public string BlipSound => _blipSound;
    public Interjection Interjection => _interjection;
    public Align Align => _align;
    public StateChange StateChange => _stateChange;
    public bool SkipFade => _skipFade;
    public bool StopMusic => _stopMusic;
    public string Dialogue => _dialogue;
}

[Serializable]
public struct StateChange
{
    public string StoryFlag;
    public EvidenceSO EvidenceToAdd;
    public EvidenceSO EvidenceToRemove;
    public EvidenceSO PersonToAdd;
} 

public enum Interjection
{
    NA,
    Objection,
    HoldIt,
    TakeThat
}
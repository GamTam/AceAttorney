using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class TBLine
{
    [SerializeField] private string _name;
    [SerializeField] private bool _knownName = true;
    [SerializeField] private string _char;
    [SerializeField] private string _anim;
    [Header("")][SerializeField] private string _blipSound;
    [SerializeField] private bool _thinking;
    [SerializeField] private bool _autoEnd;
    [SerializeField] private bool _stopMusic;
    [SerializeField] private bool _addToCourtRecord;
    [SerializeField][Range(-1, 1)] private int _skipFade;
    [SerializeField] private Interjection _interjection;
    [SerializeField] private TextAlignOptions _align;
    [Header("")][SerializeField][TextArea(3, 4)] private string _dialogue;
    [SerializeField] private StateChange _stateChange;

    public string Name => _name;
    public string Char => _char;
    public string Anim => _anim;
    public bool KnownName => _knownName;
    public bool Thinking => _thinking;
    public bool AutoEnd => _autoEnd;
    public bool AddToCourtRecord => _addToCourtRecord;
    public string BlipSound => _blipSound;
    public Interjection Interjection => _interjection;
    public TextAlignOptions Align => _align;
    public StateChange StateChange => _stateChange;
    public bool SkipFade => _skipFade == 1;
    public bool ForceFade => _skipFade == -1;
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
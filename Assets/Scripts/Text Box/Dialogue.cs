using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

[Serializable]
public class TBLine
{
    [SerializeField] private string _nameTagText;
    [SerializeField] private string _charOnScreen;
    [SerializeField] private string _animPlaying;
    [SerializeField] private string _blipSound;
    [SerializeField] private string _background;
    [Header("")][SerializeField][TextArea(3, 4)] private string _dialogue;
    [FormerlySerializedAs("_metadata")] [SerializeField] private Metadata _extras;
    [SerializeField] private StateChange _stateChange;

    public string Name => _nameTagText;
    public string Char => _charOnScreen;
    public string Anim => _animPlaying;
    public bool KnownName => !_extras.Unknown;
    public bool Thinking => _extras.Thinking;
    public bool AutoEnd => _extras.AutoEnd;
    public bool AddToCourtRecord => _extras.AddToCourtRecord;
    public string BlipSound => _blipSound;
    public Interjection Interjection => _extras.Interjection;
    public TextAlignOptions Align => _extras.Align;
    public StateChange StateChange => _stateChange;
    public FadeTypes FadeType => _extras.SkipFade;
    public bool HideOptions => _extras.HideOptions;
    public bool StopMusic => _extras.StopMusic;
    public string Dialogue => _dialogue;
    public string Background => _background;
}

[Serializable]
public struct StateChange
{
    public string StoryFlag;
    public EvidenceSO EvidenceToAdd;
    public EvidenceSO EvidenceToRemove;
    public EvidenceSO PersonToAdd;
}

[Serializable]
public struct Metadata
{
    public bool Unknown;
    public bool Thinking;
    public bool AutoEnd;
    public bool StopMusic;
    public bool AddToCourtRecord;
    public bool HideOptions;
    public FadeTypes SkipFade;
    public Interjection Interjection;
    public TextAlignOptions Align;
}

public enum FadeTypes
{
    Default,
    SkipFade,
    ForceFade,
    FadeToBlack
}

public enum Interjection
{
    NA,
    Objection,
    HoldIt,
    TakeThat
}
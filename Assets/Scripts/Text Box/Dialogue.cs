using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class TBLine
{
    [SerializeField] private string _nameTagText;
    [SerializeField] private string _charOnScreen;
    [SerializeField] private string _animPlaying;
    [SerializeField] private string _blipSound;
    [Header("")][SerializeField][TextArea(3, 4)] private string _dialogue;
    [SerializeField] private Metadata _metadata;
    [SerializeField] private StateChange _stateChange;

    public string Name => _nameTagText;
    public string Char => _charOnScreen;
    public string Anim => _animPlaying;
    public bool KnownName => !_metadata.Unknown;
    public bool Thinking => _metadata.Thinking;
    public bool AutoEnd => _metadata.AutoEnd;
    public bool AddToCourtRecord => _metadata.AddToCourtRecord;
    public string BlipSound => _blipSound;
    public Interjection Interjection => _metadata.Interjection;
    public TextAlignOptions Align => _metadata.Align;
    public StateChange StateChange => _stateChange;
    public FadeTypes FadeType => _metadata.SkipFade;
    public bool HideOptions => _metadata.HideOptions;
    public bool StopMusic => _metadata.StopMusic;
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
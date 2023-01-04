using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

[Serializable]
public class TBLine
{
    [SerializeField] private string _charOnScreen;
    [SerializeField] private string _animPlaying;
    [SerializeField] private string _blipSound;
    [SerializeField] private string _background;
    [Header("")][SerializeField][TextArea(3, 4)] private string _dialogue;
    [FormerlySerializedAs("_metadata")] [SerializeField] private Metadata _extras;
    [SerializeField] private StateChange _stateChange;

    public string Name => _extras.CustomName == "" ? _charOnScreen : _extras.CustomName;
    public string Char => _charOnScreen;
    public string Anim => _animPlaying;
    public bool Thinking => _extras.Thinking;
    public bool AutoEnd => _extras.AutoEnd;
    public bool AddToCourtRecord => _extras.AddToCourtRecord;
    public string BlipSound => _blipSound;
    public Interjection Interjection => _extras.Interjection;
    public TextAlignOptions Align => _extras.Align;
    public StateChange StateChange => _stateChange;
    public FadeTypes FadeType => _extras.FadeOptions.CharFade;
    public bool HideOptions => _extras.HideOptions;
    public bool StopMusic => _extras.StopMusic;
    public string Dialogue => _dialogue;
    public string Background => _background;
    public bool HideNameTag => _extras.HideNameTag;
    public BackgroundFade FadeDetails => _extras.FadeOptions;
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
    public bool Thinking;
    public bool AutoEnd;
    public bool StopMusic;
    public bool AddToCourtRecord;
    public bool HideNameTag;
    public bool HideOptions;
    public string CustomName;
    public Interjection Interjection;
    public TextAlignOptions Align;
    public BackgroundFade FadeOptions;
}

[Serializable]
public struct BackgroundFade
{
    public FadeTypes CharFade;
    public BGFadePos BackgroundFadePos;
    public float LengthInSeconds;
    public Color Color;
}

public enum FadeTypes
{
    Auto,
    SkipFade,
    ForceFade
}

public enum BGFadePos
{
    None = 0,
    BackgroundGone = -2,
    SceneNoUI = -10,
    Everything = -15
}

public enum Interjection
{
    NA,
    Objection,
    HoldIt,
    TakeThat
}
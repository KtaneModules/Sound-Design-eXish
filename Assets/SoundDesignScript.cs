using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using RND = UnityEngine.Random;

public class SoundDesignScript : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable[] Buttons;
    public AudioSource ExampleClip;
    public AudioClip[] ClipArray;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    private int ClipNumber = 0;
    private int PitchNumber = 0;
    private int PitchTurns = 0;
    private int PulseTurns = 0;
    private int ReleaseTurns = 0;
    private int AttackTurns = 0;
    private int ShapePress = -1;
    private string ShapeName;
    private int Attack;
    private int Release;
    private int PulseWidth;
    private float z = 0f;
    private float[] PitchValues = new float[]

    {
        1f, 1.125f, 1.269f, 1.325f, 1.5f, 1.68f, 1.875f
    };

    void Awake()
    {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable Button in Buttons)
        {
            Button.OnInteract += delegate ()
            {
                PressedButton(Button); return false;
            };
        }
    }
    // Use this for initialization
    void Start ()
    {
        SelectClip();
        Solving();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void PressedButton(KMSelectable Button)
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        int Index = Array.IndexOf(Buttons, Button);
        if(Index.InRange(0, 5))
        {
            Button.AddInteractionPunch();
        }
        switch (Index)
        {
            case 0:
                if(moduleSolved)
                {
                    break;
                }
                bool[] Answers = CheckAnswer();
                if(Answers.Any(x => !x))
                {
                    GetComponent<KMBombModule>().HandleStrike();
                    Debug.LogFormat("[Sound Design #{0}]: Strike! The incorrect states in format of pitch, attack, release, pulse width, and wave shape: {1}", moduleId, Answers.Join(", "));
                    return;
                }
                GetComponent<KMBombModule>().HandlePass();
                Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
                moduleSolved = true;
                Debug.LogFormat("[Sound Design #{0}]: Correct! Module defused!", moduleId);
                break;
            case 1: ExampleClip.Play(); break;
            case 2:
                if (ShapePress != -1)
                {
                    Buttons[ShapePress].transform.localPosition = new Vector3(0.011f, 9.646f, z);
                }
                Button.transform.localPosition = new Vector3(0.011f, 9.641f, 0.1126f);
                ShapePress = Index;
                z = Button.transform.localPosition.z;
                break;

            case 3:
                if (ShapePress != -1)
                {
                    Buttons[ShapePress].transform.localPosition = new Vector3(0.011f, 9.646f, z);
                }
                Button.transform.localPosition = new Vector3(0.011f, 9.641f, 0.0886f);
                ShapePress = Index;
                z = Button.transform.localPosition.z;
                break;
            case 4:
                if (ShapePress != -1)
                {
                    Buttons[ShapePress].transform.localPosition = new Vector3(0.011f, 9.646f, z);
                }
                Button.transform.localPosition = new Vector3(0.011f, 9.641f, 0.0646f);
                ShapePress = Index;
                z = Button.transform.localPosition.z;
                break;
            case 5:
                if (ShapePress != -1)
                {
                    Buttons[ShapePress].transform.localPosition = new Vector3(0.011f, 9.646f, z);
                }
                Button.transform.localPosition = new Vector3(0.011f, 9.641f, 0.04f);
                ShapePress = Index;
                z = Button.transform.localPosition.z;
                break;
            case 6:
                Button.transform.Rotate(Vector3.up, (315/7f));
                PitchTurns++;
                if(PitchTurns > 6)
                {
                    Button.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
                    PitchTurns = 0; return;
                }
                break;
            case 7:
                Button.transform.Rotate(Vector3.up, (270 / 2f));
                PulseTurns++;
                if (PulseTurns > 2)
                {
                    Button.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
                    PulseTurns = 0; return;
                }
                break;
            case 8:
                Button.transform.Rotate(Vector3.up, (270 / 2f));
                AttackTurns++;
                if (AttackTurns > 2)
                {
                    Button.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
                    AttackTurns = 0; return;
                }
                break;
            default:
                Button.transform.Rotate(Vector3.up, (270 / 2f));
                ReleaseTurns++;
                if (ReleaseTurns > 2)
                {
                    Button.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
                    ReleaseTurns = 0; return;
                }
                break;


        }
        
    }

    void SelectClip()
    {
        ClipNumber = RND.Range(0, ClipArray.Length);
        PitchNumber = RND.Range(0, 7);
        ExampleClip.clip = ClipArray[ClipNumber];
        ExampleClip.pitch = PitchValues[PitchNumber];
    }

    void Solving()
    {
        string[] ClipName = ClipArray[ClipNumber].name.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
        ShapeName = ClipName[0].ToLower();
        Attack = int.Parse(ClipName[2][3].ToString());
        Release = int.Parse(ClipName[3][3].ToString());
        PulseWidth = ShapeName == "square" ? int.Parse(ClipName[4][2].ToString()) : 3;
        Debug.LogFormat("[Sound Design #{0}]: The generated attributes in format of shape name, attack, release, pitch, and pulse width: {1}, {2}, {3}, {4}, and {5}.", moduleId, ShapeName, Attack, Release, "CDEFGAB"[PitchNumber],PulseWidth == 3 ? 0 : PulseWidth);
    }

    bool[] CheckAnswer()
    {
        bool TruePitch = PitchTurns == PitchNumber;
        bool TrueAttack = Array.IndexOf(new int[] { 0, 1, 5 }, Attack) == AttackTurns;
        bool TrueRelease = Array.IndexOf(new int[] { 0, 1, 5 }, Release) == ReleaseTurns;
        string[] ShapeNames = new string[] { "sine", "tri", "saw", "square" };
        bool TrueShape = ShapeNames[ShapePress - 2] == ShapeName;
        bool TruePW = PulseWidth == 3 ? true : PulseTurns == PulseWidth;
        return new bool[] { TruePitch, TrueAttack, TrueRelease, TruePW, TrueShape };
    }


}

using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using RND = UnityEngine.Random;
using System.Text.RegularExpressions;

public class SoundDesignScript : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable[] Buttons;
    public AudioSource ExampleClip;
    public AudioClip[] ClipArray;
    public GameObject VolDot;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    string[] ShapeNames = new string[] { "sine", "tri", "saw", "square" };
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
    private int VolInd = 5;
    private float z = 0f;
    private float[] PitchValues = new float[]
    {
        1f, 1.125f, 1.269f, 1.325f, 1.5f, 1.68f, 1.875f
    };
    private float[] VolValues = new float[]
    {
        0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1f
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

    void PressedButton(KMSelectable Button)
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Button.transform);
        int Index = Array.IndexOf(Buttons, Button);
        if (Index.InRange(0, 6))
        {
            Button.AddInteractionPunch();
        }
        switch (Index)
        {
            case 0:
                if (moduleSolved)
                {
                    break;
                }
                bool[] Answers = CheckAnswer();
                if (Answers.Any(x => !x))
                {
                    GetComponent<KMBombModule>().HandleStrike();
                    Debug.LogFormat("[Sound Design #{0}] Strike! The incorrect states in format of pitch, attack, release, pulse width, and wave shape: {1}", moduleId, Answers.Join(", "));
                    return;
                }
                GetComponent<KMBombModule>().HandlePass();
                Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
                moduleSolved = true;
                Debug.LogFormat("[Sound Design #{0}] Correct! Module defused!", moduleId);
                break;
            case 1:
                ExampleClip.clip = ClipArray[ClipNumber];
                ExampleClip.pitch = PitchValues[PitchNumber];
                ExampleClip.Play();
                break;
            case 2:
                if (ShapePress == -1)
                {
                    break;
                }
                string clipName = ShapeNames[ShapePress - 3][0].ToString().ToUpper() + ShapeNames[ShapePress - 3].Substring(1) + " C att" + new int[] { 0, 1, 5 }[AttackTurns] + " rel" + new int[] { 0, 1, 5 }[ReleaseTurns];
                if (clipName.StartsWith("Square"))
                    clipName += " PW" + (PulseTurns % 3);
                int index = -1;
                for (int i = 0; i < ClipArray.Length; i++)
                {
                    if (ClipArray[i].name == clipName)
                    {
                        index = i;
                        break;
                    }
                }
                ExampleClip.clip = ClipArray[index];
                ExampleClip.pitch = PitchValues[PitchTurns];
                ExampleClip.Play();
                break;
            case 3:
                if (ShapePress != -1)
                {
                    Buttons[ShapePress].transform.localPosition = new Vector3(0.011f, 9.646f, z);
                }
                Button.transform.localPosition = new Vector3(0.011f, 9.641f, 0.1126f);
                ShapePress = Index;
                z = Button.transform.localPosition.z;
                break;
            case 4:
                if (ShapePress != -1)
                {
                    Buttons[ShapePress].transform.localPosition = new Vector3(0.011f, 9.646f, z);
                }
                Button.transform.localPosition = new Vector3(0.011f, 9.641f, 0.0886f);
                ShapePress = Index;
                z = Button.transform.localPosition.z;
                break;
            case 5:
                if (ShapePress != -1)
                {
                    Buttons[ShapePress].transform.localPosition = new Vector3(0.011f, 9.646f, z);
                }
                Button.transform.localPosition = new Vector3(0.011f, 9.641f, 0.0646f);
                ShapePress = Index;
                z = Button.transform.localPosition.z;
                break;
            case 6:
                if (ShapePress != -1)
                {
                    Buttons[ShapePress].transform.localPosition = new Vector3(0.011f, 9.646f, z);
                }
                Button.transform.localPosition = new Vector3(0.011f, 9.641f, 0.04f);
                ShapePress = Index;
                z = Button.transform.localPosition.z;
                break;
            case 7:
                Button.transform.Rotate(Vector3.up, (315/7f));
                PitchTurns++;
                if (PitchTurns > 6)
                {
                    Button.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
                    PitchTurns = 0; return;
                }
                break;
            case 8:
                Button.transform.Rotate(Vector3.up, (270 / 2f));
                PulseTurns++;
                if (PulseTurns > 2)
                {
                    Button.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
                    PulseTurns = 0; return;
                }
                break;
            case 9:
                Button.transform.Rotate(Vector3.up, (270 / 2f));
                AttackTurns++;
                if (AttackTurns > 2)
                {
                    Button.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
                    AttackTurns = 0; return;
                }
                break;
            case 10:
                Button.transform.Rotate(Vector3.up, (270 / 2f));
                ReleaseTurns++;
                if (ReleaseTurns > 2)
                {
                    Button.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
                    ReleaseTurns = 0; return;
                }
                break;
            case 11:
                if (VolInd == 0) return;
                else VolInd--;
                ExampleClip.volume = VolValues[VolInd];
                VolDot.transform.localPosition = new Vector3(VolDot.transform.localPosition.x - 0.0115f, VolDot.transform.localPosition.y, VolDot.transform.localPosition.z);
                break;
            default:
                if (VolInd == 10) return;
                else VolInd++;
                ExampleClip.volume = VolValues[VolInd];
                VolDot.transform.localPosition = new Vector3(VolDot.transform.localPosition.x + 0.0115f, VolDot.transform.localPosition.y, VolDot.transform.localPosition.z);
                break;
        } 
    }

    void SelectClip()
    {
        ClipNumber = RND.Range(0, ClipArray.Length);
        PitchNumber = RND.Range(0, 7);
    }

    void Solving()
    {
        string[] ClipName = ClipArray[ClipNumber].name.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
        ShapeName = ClipName[0].ToLower();
        Attack = int.Parse(ClipName[2][3].ToString());
        Release = int.Parse(ClipName[3][3].ToString());
        PulseWidth = ShapeName == "square" ? int.Parse(ClipName[4][2].ToString()) : 3;
        Debug.LogFormat("[Sound Design #{0}] The generated attributes in format of shape name, attack, release, pitch, and pulse width: {1}, {2}, {3}, {4}, and {5}.", moduleId, ShapeName[0].ToString().ToUpper() + ShapeName.Substring(1), Attack, Release, "CDEFGAB"[PitchNumber], PulseWidth == 3 ? 0 : PulseWidth);
    }

    bool[] CheckAnswer()
    {
        bool TruePitch = PitchTurns == PitchNumber;
        bool TrueAttack = Array.IndexOf(new int[] { 0, 1, 5 }, Attack) == AttackTurns;
        bool TrueRelease = Array.IndexOf(new int[] { 0, 1, 5 }, Release) == ReleaseTurns;
        bool TrueShape = false;
        if (ShapePress != -1)
            TrueShape = ShapeNames[ShapePress - 3] == ShapeName;
        bool TruePW = PulseWidth == 3 ? true : PulseTurns == PulseWidth;
        return new bool[] { TruePitch, TrueAttack, TrueRelease, TruePW, TrueShape };
    }

    //twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} play [Presses the play button] | !{0} preview [Presses the preview button] | !{0} submit [Presses the submit button] | !{0} wave <waveform> [Presses the button of the specified waveform] | !{0} set <knob> <dir> [Sets the specified knob in the specified direction] | !{0} volume/vol <%> [Sets the volume to the specified percentage] | Valid waveforms are Sine, Tri, Saw, and Square | Valid knobs are P, A, PW, or R | Valid directions are any abbreviated cardinal direction";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        if (Regex.IsMatch(command, @"^\s*play\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            Buttons[1].OnInteract();
        }
        if (Regex.IsMatch(command, @"^\s*preview\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            Buttons[2].OnInteract();
        }
        if (Regex.IsMatch(command, @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            Buttons[0].OnInteract();
        }
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*wave\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (parameters.Length > 2)
            {
                yield return "sendtochaterror Too many parameters!";
            }
            else if (parameters.Length == 2)
            {
                if (!ShapeNames.Contains(parameters[1].ToLower())) { yield return "sendtochaterror!f The specified waveform '" + parameters[1] + "' is invalid!"; yield break; }
                Buttons[Array.IndexOf(ShapeNames, parameters[1].ToLower()) + 3].OnInteract();
            }
            else if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify a valid waveform!";
            }
        }
        if (Regex.IsMatch(parameters[0], @"^\s*set\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            string[] knobs = new string[] { "p", "pw", "a", "r" };
            string[] cardinals = new string[] { "sw", "w", "nw", "n", "ne", "e", "se" };
            if (parameters.Length > 3)
            {
                yield return "sendtochaterror Too many parameters!";
            }
            else if (parameters.Length == 3)
            {
                if (!knobs.Contains(parameters[1].ToLower())) { yield return "sendtochaterror!f The specified knob '" + parameters[1] + "' is invalid!"; yield break; }
                if (!cardinals.Contains(parameters[2].ToLower())) { yield return "sendtochaterror!f The specified direction '" + parameters[2] + "' is invalid!"; yield break; }
                if (!parameters[1].ToLower().Equals("p") && parameters[2].ToLower().EqualsAny("w", "nw", "ne", "e")) { yield return "sendtochaterror The specified direction '" + parameters[2] + "' is not possible to set to on this knob!"; yield break; }
                if (parameters[1].ToLower().Equals("p"))
                {
                    while (Array.IndexOf(cardinals, parameters[2].ToLower()) != PitchTurns)
                    {
                        Buttons[7].OnInteract();
                        yield return new WaitForSeconds(0.1f);
                    }
                }
                else if (!parameters[1].ToLower().Equals("p"))
                {
                    string[] abbrev = new string[] { "sw", "n", "se" };
                    int[] counts = new int[] { PulseTurns, AttackTurns, ReleaseTurns };
                    while (Array.IndexOf(abbrev, parameters[2].ToLower()) != counts[Array.IndexOf(knobs, parameters[1].ToLower()) - 1])
                    {
                        Buttons[Array.IndexOf(knobs, parameters[1].ToLower()) + 7].OnInteract();
                        yield return new WaitForSeconds(0.1f);
                        counts = new int[] { PulseTurns, AttackTurns, ReleaseTurns };
                    }
                }
            }
            else if (parameters.Length == 2)
            {
                if (knobs.Contains(parameters[1].ToLower()))
                    yield return "sendtochaterror Please specify the direction to set this knob to!";
                else
                    yield return "sendtochaterror!f The specified knob '" + parameters[1] + "' is invalid!";
            }
            else if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify a knob and direction to set!";
            }
        }
        if (Regex.IsMatch(parameters[0], @"^\s*volume\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(parameters[0], @"^\s*vol\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            string[] percents = new string[] { "0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%" };
            if (parameters.Length > 2)
            {
                yield return "sendtochaterror Too many parameters!";
            }
            else if (parameters.Length == 2)
            {
                if (!percents.Contains(parameters[1])) { yield return "sendtochaterror!f The specified percentage '" + parameters[1] + "' is invalid!"; yield break; }
                if (Array.IndexOf(percents, parameters[1]) > VolInd)
                {
                    int end = Array.IndexOf(percents, parameters[1]) - VolInd;
                    for (int i = 0; i < end; i++)
                    {
                        Buttons[12].OnInteract();
                        yield return new WaitForSeconds(0.1f);
                    }
                }
                else if (Array.IndexOf(percents, parameters[1]) < VolInd)
                {
                    int end = VolInd - Array.IndexOf(percents, parameters[1]);
                    for (int i = 0; i < end; i++)
                    {
                        Buttons[11].OnInteract();
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
            else if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify a valid percentage!";
            }
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        string[] cardinals = new string[] { "sw", "w", "nw", "n", "ne", "e", "se" };
        int[] indexes = new int[] { 0, 3, 6 };
        if (ShapePress == -1)
        {
            Buttons[Array.IndexOf(ShapeNames, ShapeName) + 3].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        else if (ShapeNames[ShapePress - 3] != ShapeName)
        {
            Buttons[Array.IndexOf(ShapeNames, ShapeName) + 3].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        yield return ProcessTwitchCommand("set p " + cardinals[PitchNumber]);
        yield return ProcessTwitchCommand("set a " + cardinals[indexes[Array.IndexOf(new int[] { 0, 1, 5 }, Attack)]]);
        yield return ProcessTwitchCommand("set pw " + cardinals[indexes[PulseWidth % 3]]);
        yield return ProcessTwitchCommand("set r " + cardinals[indexes[Array.IndexOf(new int[] { 0, 1, 5 }, Release)]]);
        Buttons[0].OnInteract();
    }
}
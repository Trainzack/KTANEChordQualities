using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;
using System.Linq;

public enum Note {
    A,
    Asharp,
    B,
    C,
    Csharp,
    D,
    Dsharp,
    E,
    F,
    Fsharp,
    G,
    Gsharp
}


public class ChordQualities
    : MonoBehaviour
{
    #region Used In 

    public KMSelectable WheelButton;
    public KMSelectable SelectButton;
    public KMSelectable SubmitButton;
    public Transform lightPrefab;
    public Transform LightsSource;
    public NoteLight[] lights;

    public KMModSettings settings;
	#endregion
	[HideInInspector]
	public bool isSolved = false;//Not sure what this is used for (probably other modules), but I saw it in Chess.

	#region Settings Defined
    #endregion
    
	static int moduleNumber = 1;

	int thisModuleNumber;


    #region Problem Decision

    
    Quality[] qualities = Quality.getQualities();
    Chord givenChord;
    #endregion

    #region Solutions
    private Chord solutionChord;
    private int[] RootToQuality = new int[] { 11, 9, 1, 5, 7, 2, 4, 10, 6, 0, 3, 8 };
    #endregion

    #region Used In Module
    bool active = false;
    int position = 0;
    bool selectButtonIsPressed = false;
    int selectPressNumber = 0;
    const float selectCancelHoldTime = 0.6f;
    float timeWheelPressed;
    bool wheelIsPressed = false;
    int wheelPressNumber = 0;
    const float wheelBackHoldTime = 0.3f;
    #endregion


    void Start() { 
		thisModuleNumber = moduleNumber++;
		loadSettings ();
		GetComponent<KMBombModule>().OnActivate += OnActivate;
        position = Random.Range(0, 12);
        WheelButton.GetComponent<Transform>().Rotate(new Vector3(0, 1, 0), (position*-360.0f/12.0f));
        selectChord();
        FindSolution();
    }

    private void selectChord() {
        givenChord = new Chord(qualities[Random.Range(0, 12)], (Note)Random.Range(0, 12));
        LogMessage("Selected chord: " + givenChord);
        for (int i = 0;i < lights.Length; i++) {
            bool inChord = false;
            foreach (Note o in givenChord.Notes) {
                if (o == lights[i].note) { inChord = true; }
            }
            lights[i].setOutputLight(inChord);
        }
    }



    void FindSolution() {
        Note root = findRoot(givenChord.Quality);
        Quality quality = findQuality(givenChord.Notes[0]);
        solutionChord = new Chord(quality, root);
        LogMessage("Correct Chord: " + solutionChord);
    }

    public Note findRoot(Quality q) {
        return q.TargetRoot;
    }

    public Quality findQuality(Note r) {
        return Quality.getQualities()[RootToQuality[(int)r]];
    }

    void OnActivate() {
        WheelButton.OnInteract += delegate () { WheelButton.AddInteractionPunch(0.1f); SpinWheelOn(); return false; };
        WheelButton.OnInteractEnded += delegate () { SpinWheelOff(); };
        SelectButton.OnInteract += delegate () { SelectButton.AddInteractionPunch(0.3f); SelectOn(); return false; };
        SelectButton.OnInteractEnded += delegate () { SelectOff(); };
        SubmitButton.OnInteract += delegate () { SubmitButton.AddInteractionPunch(0.6f); Submit(); return false; };
        active = true;
        lights[position].ChangeHighlight(true);
    }

    void SpinWheelOn() {
        if (!wheelIsPressed)
        {
            timeWheelPressed = Time.time;
            wheelIsPressed = true;
            StartCoroutine(wheelHeld(++wheelPressNumber));
        }
    }

    IEnumerator wheelHeld(int p)
    {
        yield return new WaitForSecondsRealtime(wheelBackHoldTime);
        if (p == wheelPressNumber && wheelIsPressed)
        {
            GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TypewriterKey, transform);
        }
    }

    void SpinWheelOff()
    {
        float holdTime = Time.time - timeWheelPressed;
        wheelIsPressed = false;
        StartCoroutine(rotateWheel(holdTime < wheelBackHoldTime));
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
    }

    IEnumerator rotateWheel(bool forward) {
        lights[position].ChangeHighlight(false);
        if (forward)
        {
            position = (position + 1) % 12;
        } else
        {
            position = (position + 11) % 12;
        }
        lights[position].ChangeHighlight(true);
        Transform t = WheelButton.GetComponent<Transform>();
        int delay = 10;
        float rotateAmount = -(360/12.0f)/delay;
        rotateAmount *= (forward) ? 1 : -1;
        for (int i = 0; i < delay; i++) {
            t.Rotate(new Vector3(0, 1, 0), rotateAmount);
            yield return new WaitForEndOfFrame();
        }
    }

    void SelectOn()
    {
        if (!selectButtonIsPressed)
        {
            selectButtonIsPressed = true;
            StartCoroutine(selectHeld(++selectPressNumber));
            NoteLight selected = lights[position];
            selected.toggleInputLight();
        }
    }

    IEnumerator selectHeld( int p)
    {
        yield return new WaitForSecondsRealtime(selectCancelHoldTime);
        if (p == selectPressNumber && selectButtonIsPressed)
        {
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].turnInputLightOff();
            }
        }
    }

    void SelectOff()
    {
        selectButtonIsPressed = false;
    }

    void Submit() {
        StartCoroutine(endingFlourish());
    }

    IEnumerator endingFlourish() {
        lights[position].ChangeHighlight(false);
        yield return new WaitForSeconds(0.03f);
        foreach (NoteLight n in lights) {
            if (n.InputLightIsOn) {
                n.turnInputLightOn();
                n.ChangeHighlight(true);
                yield return new WaitForSeconds(0.17f);
            }
        }
        if (active)
        {
            checkCorrect();
        }
        yield return new WaitForSeconds(0.17f);
        for (int i = 0; i < lights.Length; i++) {
            if (i != position) {
                lights[i].ChangeHighlight(false);
            } else {
                lights[i].ChangeHighlight(true);
            }
        }
    }

    void checkCorrect() {
        bool correct = true;//If a light is off and in the chord or on and not in the chord, then it should strike
        string missing = " ";
        string extra = " ";
        if (solutionChord != null) {

            foreach (NoteLight nl in lights) {//This is probably better done with a dictionary, but whatevs.
                bool noteInChord = solutionChord.Notes.Contains(nl.note);
                if (nl.InputLightIsOn & !noteInChord) {
                    correct = false;
                    extra += nl.note +" ";
                } else if (!nl.InputLightIsOn & noteInChord) {
                    correct = false;
                    missing += nl.note + " ";
                }
            }
        }
        if (correct) {
            LogMessage("Answer accepted.");
            Pass();
        } else if (!isSolved) {
            LogMessage("Answer rejected, as " + solutionChord + " was not input; [" + missing + "] are missing and [" + extra + "] are extraneous.");
            Strike();
        }
    }



    //PASS/FAIL

    void Pass() {
        GetComponent<KMBombModule>().HandlePass();
        active = false;
        isSolved = true;
    }
	void Strike() {
        LogMessage("Giving strike #" + (GetComponent<KMBombInfo>().GetStrikes() + 1));
        GetComponent<KMBombModule> ().HandleStrike ();
		
	}


	void LogMessage(string message) {
		Debug.Log ("[ChordQualities #" + thisModuleNumber +"] " + message);
	}
 

	void loadSettings() {
        /*
		try {
			//RhythmsSettings modSettings = JsonConvert.DeserializeObject<RhythmsSettings> (settings.Settings);
			if (modSettings != null) {
				colorBlindMode = modSettings.GetColorBlindMode ();
				int _DebugPattern = modSettings.GetDebugModePattern () - 1;
				int _DebugColor = modSettings.GetDebugModeColor () - 1;
				if (_DebugPattern >= 0 & _DebugPattern < patterns.Length) {
					DebugPattern = _DebugPattern;
				}
				if (_DebugColor >= 0 & _DebugColor < colors.Length) {
					DebugColor = _DebugColor;
				}
			} else {
				LogMessage ("Could not read settings file!");
			}
		} catch (JsonReaderException e) {
			LogMessage ("Malformed settings file! " + e.Message);
		}
		*/
    }

    //EDITOR


    //This method is only used to build the lights in the editor.
    public void BuildLights() {
        string[] noteNames = new string[] { "A", "*", "B", "C", "*", "D", "*", "E", "F", "*", "G", "*" };
        for (int i = 0; i < 12; i++) {
            if (lights[i] != null) {
                DestroyImmediate(lights[i].gameObject);
            }
            Transform o = (Transform)Instantiate(lightPrefab, LightsSource.position, Quaternion.AngleAxis((360 * i / 12.0f), new Vector3(0, 1, 0)));
            o.Translate(new Vector3(0.00f, 0.0f, 0.12f));//For vertical alignment. This is eaiser (but not better) than modifiying the prefab.
            o.SetParent(WheelButton.GetComponent<Transform>());
            NoteLight NL = o.GetComponent<NoteLight>();
            lights[i] = NL;
            NL.setText(noteNames[i]);
            NL.name = "Note Light:" + Quality.notes[i];
            NL.note = (Note)i;
            NL.audio = GetComponent<KMAudio>();
        }
    }

    public void BuildManual() {
        string[] noteNames = new string[] { "A", "A♯", "B", "C", "C♯", "D", "D♯", "E", "F", "F♯", "G", "G♯" };
        Quality[] qs = Quality.getQualities();
        string message = "<table class=\"repeaters-table\"><tbody>\n\t\t\t\t<tr><th colspan = \"2\" class=\"whos-on-first-look-at-display\">Root to Quality</th><th class=\"repeaters-spacer\"></th><th colspan = \"2\" class=\"whos-on-first-look-at-display\">Quality to Root</th></tr>";
        for (int i = 0; i < 12; i++) {
            message += "\n\t\t\t\t<tr><th>" + noteNames[i] + "</th><th>" + findQuality((Note)i) + "</th><th class=\"repeaters-spacer\"></th>" +
                "<th>" + qs[i] + "</th><th>" + noteNames[(int)findRoot(qs[i])] + "</th></tr>";
        }
        message += "</tbody>\n\t\t\t</table>";
        Debug.Log(message);

        message = "<table class=\"quality-sheet\"><tbody>";

        for (int row = -1; row < 12; row++)
        {
            message += "\n\t\t\t\t";

            if (row != -1)
            {
                message += "<tr>< th>+" + row + "</th>";
            } else
            {
                message += "<tr class=\"header\"><th>Off</th>";
            }

            for (int i = 0; i < 12; i++)
            {
                if (row == -1)
                {
                    message += "<th>" + qs[i].ToString().Replace("-", "&#8209;") + " </th>"; //We need to replace the hyphens for the minor qualities with non-breaking hyphens
                }
                else
                {
                    if (qs[i].Offsets.Contains(row) | row == 0)
                    {
                        message += "<th  class=\"selected\">⛌</th>";
                    } else if (i == 4 & row == 7)
                    {
                        message += "<th>*</th>";
                    } else
                    {
                        message += "<th></th>";
                    }
                }
            }
            message += "</tr>";
        }
        message += "</tbody>\n\t\t\t</table>";
        Debug.Log(message);
    }

}


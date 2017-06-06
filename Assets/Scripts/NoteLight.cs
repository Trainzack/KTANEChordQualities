using UnityEngine;
using System.Collections;

public class NoteLight : MonoBehaviour {

    public TextMesh noteName;
    public SpriteRenderer noteBacking;
    public Color normal;
    public Color highlighted;

    public TextMesh outputLight;
    public MeshRenderer inputLight;

    public SpriteRenderer flash;

    public Material inLightOnMat;
    public Material outLightOnMat;
    public Material lightOffMat;
    public Note note;

    public new KMAudio audio;
    
    private bool inputLightIsOn = false;

    public bool InputLightIsOn {
        get {
            return inputLightIsOn;
        }

        set {
            inputLightIsOn = value;
        }
    }

    // Use this for initialization
    void Start () {
        inputLight.material = lightOffMat;
        InputLightIsOn = false;
        flash.enabled = false;
        noteBacking.color = normal;
    }

    public void toggleInputLight() {
        if (InputLightIsOn) {
            turnInputLightOff();
        } else {
            turnInputLightOn();
        }
    }

    public void turnInputLightOn() { 
        inputLight.material = inLightOnMat;
        InputLightIsOn = true;
        audio.PlaySoundAtTransform("note" + note, transform);
        flash.enabled = true;
    }

    public void turnInputLightOff() { 
        inputLight.material = lightOffMat;
        InputLightIsOn = false;
        audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TypewriterKey, transform);
        flash.enabled = false;
    }

    public void setOutputLight(bool on) {
        if (on) {
            outputLight.text = "▲";//▲Δ
        } else {
            outputLight.text = " ";
        }
    }

    public void setText(string s) {
        noteName.text = s;
        if (s == "*")
        {
            noteName.text = "♯♭";
            noteName.characterSize = 0.7f;
            noteBacking.transform.localScale -= new Vector3(0.09f, 0.09f, 0.09f);
            //noteName.transform.Translate(0, 0.002f, 0f);
        }
    }

    public void ChangeHighlight(bool h) {
        if (h) {
            noteBacking.color = highlighted;
        } else {
            noteBacking.color = normal;
        }
    }
	
}

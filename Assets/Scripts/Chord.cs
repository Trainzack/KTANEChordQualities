using UnityEngine;
using System.Collections;

public class Chord {

    static string[] noteNames = new string[] { "A", "A♯", "B", "C", "C♯", "D", "D♯", "E", "F", "F♯", "G", "G♯" };

    Note[] notes = new Note[4];
    Quality quality;

    public Chord(Quality q, Note r) {
        Notes[0] = r;
        Quality = q;
        for (int i = 0; i < q.Offsets.Length; i++) {
            Notes[i+1] = (Note)(((int)q.Offsets[i] + (int)r) % 12);
        }
    }

    public Note[] Notes {
        get {
            return notes;
        }

        set {
            notes = value;
        }
    }

    public string QualitySymbol {
        get {
            return Quality.Name;
        }
        
    }

    public Quality Quality {
        get {
            return quality;
        }

        set {
            quality = value;
        }
    }

    override public string ToString() {
        string o = noteNames[(int)Notes[0]] + QualitySymbol + ": "+ noteNames[(int)Notes[0]];
        for (int i = 1; i < Notes.Length; i++) {
            o += "," + getNoteName((int)Notes[i]);
        }
        return o;
    }

    private string getNoteName(int i) {
        return noteNames[(i + 12) % 12];
    }

}

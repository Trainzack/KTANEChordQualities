using UnityEngine;
using System.Collections;

public class Quality  {
    
    string name;
    Note targetRoot;
    int[] offsets;
    private static Quality[] qualities;

    public static string[] notes = new string[] { "A", "A♯", "B", "C", "C♯", "D", "D♯", "E", "F", "F♯", "G", "G♯" };

    public Quality(Note tR, int[] off, string name) {
        Offsets = off;
        this.Name = name;
        TargetRoot = tR;
    }

    public int[] Offsets {
        get {
            return offsets;
        }

        set {
            offsets = value;
        }
    }

    public string Name {
        get {
            return name;
        }

        set {
            name = value;
        }
    }

    public Note TargetRoot {
        get {
            return targetRoot;
        }

        set {
            targetRoot = value;
        }
    }

    //♯♭
    public static Quality[] getQualities() {
        if (qualities == null) {
            qualities = new Quality[12] {
                new Quality(Note.G, new int[] {4,7,10},"7"),
                new Quality(Note.Gsharp, new int[] {3,7,10},"-7"),
                new Quality(Note.Asharp, new int[] {4,7,11},"Δ7"),
                new Quality(Note.F, new int[] {3,7,11},"-Δ7"),
                new Quality(Note.A, new int[] {3,4,10},"7♯9"),
                new Quality(Note.Csharp, new int[] {3,6,10},"ø"),
                new Quality(Note.Dsharp, new int[] {2,4,7},"add9"),
                new Quality(Note.E, new int[] {2,3,7},"-add9"),
                new Quality(Note.Fsharp, new int[] {4,8,10},"7♯5"),
                new Quality(Note.C, new int[] {4,8,11},"Δ7♯5"),
                new Quality(Note.D, new int[] {5,7,10},"7sus"),
                new Quality(Note.B, new int[] {3,8,11},"-Δ7♯5")
            };
        }
        return qualities;

    }

    override public string ToString() {
        return name;
    }
}

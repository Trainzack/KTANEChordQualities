using UnityEngine;
using System.Collections;

public class RhythmsSettings {

	public bool ColorBlindMode = false;

	public int DebugModePattern = -1;

	public int DebugModeColor = -1;

	public bool GetColorBlindMode() {return ColorBlindMode;}

	public int GetDebugModePattern() {
		return DebugModePattern;
	}

	public int GetDebugModeColor() {
		return DebugModeColor;
	}
}

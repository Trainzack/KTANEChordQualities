using UnityEngine;
using System.Collections;

public class ManualGen : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	static string[] labels = new string[] { "♩", "♪", "♫", "♬" };
	public static int[][] solutionsStep1 = new int[][] //Left is pattern, right is color
	{									//Number %4 = button to press, number /4 = instruction: 0: press, 1: hold 1 beep, 2: hold 2 beep, or number = -2: press repeatedly
		new int[] {8,-2,11,10},//Pattern 0
		new int[] {4,1,2,6},
		new int[] {6,5,0,3},
		new int[] {1,7,7,6},
		new int[] {5,3,4,3},
		new int[] {4,1,2,2},
		new int[] {0,2,3,1},
	};

	//This is the second button press the player needs to make
	public static int[][] solutionsStep2 = new int[][] //Left is pattern, right is color
	{									//Number %4 = button to press, number /4 = instruction: 0: press, 1: hold 1 beep, 2: hold 2 beep, or number = -1: nothing,pass automatically
		new int[] {1,-1,1,1},//Pattern 0
		new int[] {3,0,5,7},
		new int[] {1,6,3,7},
		new int[] {2,0,3,1},
		new int[] {2,2,1,6},
		new int[] {7,5,1,4},
		new int[] {5,6,7,1},
	};

	public static string getManual() {
		string output = "<table class=\"repeaters-table\"><tbody>";
		for (int p = 0; p < solutionsStep1.Length; p++) {
			output += "<tr>";
			for (int c = 0; c < solutionsStep1 [p].Length; c++) {
				output += "<td>" + decodeInstruction(solutionsStep1[p][c]) + "/" + decodeInstruction(solutionsStep2[p][c]) + "</td>";
			}
					output += "</tr>";
		}

		output += "</tbody></table>";
		return output;
	}

	static string decodeInstruction(int i) {
		string output = "";

		if (i >= 0) {
			output += labels [i % 4];
		}
		int a = i / 4;
		if (a == -1) {
			output = "";
		} else if (a == -2) {
			output = "***";
		} else if (a == 0) {
			output += "P";
		} else {
			output += a;
		}
		return output;
	}
}

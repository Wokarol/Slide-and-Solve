using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Wokarol.VersionEditor {
	public class QuickIncrementors : Editor {

		[MenuItem("Tools/VersionEditor/QuickOptions/Major++ %&q", priority = 1)]
		static void IncrementMajor () {
			VersionDataPack tempVersionData = Functions.LoadVersionData();
			tempVersionData.major++;
			Functions.SetVersion(tempVersionData);
		}
		[MenuItem("Tools/VersionEditor/QuickOptions/Minor++ %&w", priority = 2)]
		static void IncrementMinor () {
			VersionDataPack tempVersionData = Functions.LoadVersionData();
			tempVersionData.minor++;
			Functions.SetVersion(tempVersionData);
		}
		[MenuItem("Tools/VersionEditor/QuickOptions/SubMinor++ %&e", priority = 3)]
		static void IncrementSubMinor () {
			VersionDataPack tempVersionData = Functions.LoadVersionData();
			tempVersionData.subMinor++;
			Functions.SetVersion(tempVersionData);
		}
	}
}
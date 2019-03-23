using UnityEngine;
using UnityEditor;


namespace Wokarol.VersionEditor {
	public class VersionEditor :EditorWindow {

		// Core systems
		VersionDataPack versionData;
		VersionDataPack lastVersionData;
		public void UpdateVersionInWindow () {
			lastVersionData = versionData;
		}
		[MenuItem("Tools/VersionEditor/Editor")]
		static void Init () {
			VersionEditor window = (VersionEditor)GetWindow<VersionEditor>("Version editor");
			window.Show();
		}

		public void Update () {
			if (PlayerSettings.bundleVersion != (versionData.major + "." + versionData.minor + "." + versionData.subMinor + Functions.buildPrefix + versionData.build)) {
				versionData = Functions.LoadVersionData();
				UpdateVersionInWindow();
				Repaint();
			}
		}

		public void OnGUI () {
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.HelpBox("Current version: " + Application.version, MessageType.None);

			IntSetter(ref versionData.major, "Major", ref versionData.minor, ref versionData.subMinor);
			IntSetter(ref versionData.minor, "Minor", ref versionData.subMinor);
			IntSetter(ref versionData.subMinor, "SubMinor");


			if (EditorPrefs.GetBool("VersionEditorSettings.BuildNumberEditor")) {
				IntSetter(ref versionData.build, "Build");
			} else {
				EditorGUILayout.HelpBox("Build: " + versionData.build, MessageType.None);
			}

			if (lastVersionData != versionData && GUILayout.Button("Accept")) {
				GUI.FocusControl("");
				Functions.SetVersion(versionData);
				UpdateVersionInWindow();
			}

			if (EditorGUI.EndChangeCheck()) {
				Functions.SetVersion(versionData);
				UpdateVersionInWindow();
			}
		}
		void IntSetter (ref int value, string name, ref int prevValue0) {
			int trashInt = 0;
			IntSetter(ref value, name, ref prevValue0, ref trashInt);
		}
		void IntSetter (ref int value, string name) {
			int trashInt0 = 0;
			int trashInt1 = 0;
			IntSetter(ref value, name, ref trashInt0, ref trashInt1);
		}
		void IntSetter (ref int value, string name, ref int prevValue0, ref int prevValue1) {
			EditorGUILayout.BeginHorizontal();
			value = EditorGUILayout.IntField(name, value);
			if (GUILayout.Button("+")) {
				value++;
				if (EditorPrefs.GetBool("VersionEditorSettings.ResetPrev")) {
					prevValue0 = 0;
					prevValue1 = 0;
				}
				GUI.FocusControl("");
			}
			if (GUILayout.Button("-")) {
				value--;
				GUI.FocusControl("");
			}
			if (GUILayout.Button("0")) {
				value = 0;
				GUI.FocusControl("");
			}
			value = Mathf.Clamp(value, 0, int.MaxValue);
			EditorGUILayout.EndHorizontal();
		}

		private void OnEnable () {
			versionData = Functions.LoadVersionData();
			UpdateVersionInWindow();
		}
	}
}

using UnityEngine;
using UnityEditor;
namespace Wokarol.VersionEditor {
	public class VersionEditorSettings :EditorWindow {

		bool resetPrevious;
		bool autoIncrementBuild;
		bool autoIncrementBuildByPlayMode;
		bool buildNumberEditor;

		[MenuItem("Tools/VersionEditor/Settings")]
		static void Init () {
			VersionEditorSettings window = (VersionEditorSettings)GetWindow<VersionEditorSettings>("Settings");
			window.Show();
		}

		private void OnGUI () {
			EditorGUI.BeginChangeCheck();
			resetPrevious = GUILayout.Toggle(resetPrevious, "Reset previous");
			EditorGUILayout.Space();
			autoIncrementBuild = GUILayout.Toggle(autoIncrementBuild, "Auto increment build");
			autoIncrementBuildByPlayMode = GUILayout.Toggle(autoIncrementBuildByPlayMode, "Auto increment build by editor play mode");
			EditorGUILayout.Space();
			buildNumberEditor = GUILayout.Toggle(buildNumberEditor, "Build number editor");
			EditorGUILayout.Space();
			if (GUILayout.Button("Reset everything")) {
				DeleteKeys();
				LoadEditorData();
			}
			if (GUILayout.Button("Save")) {
				SaveEditorData();
			}
		}

		private void OnEnable () {
			LoadEditorData();
		}
		void SaveEditorData () {
			EditorPrefs.SetBool("VersionEditorSettings.ResetPrev", resetPrevious);
			EditorPrefs.SetBool("VersionEditorSettings.AutoIncrBuild", autoIncrementBuild);
			EditorPrefs.SetBool("VersionEditorSettings.AutoIncrBuildByPlay", autoIncrementBuildByPlayMode);
			EditorPrefs.SetBool("VersionEditorSettings.BuildNumberEditor", buildNumberEditor);
		}
		void LoadEditorData () {
			resetPrevious = LoadBoolKey(false, "VersionEditorSettings.ResetPrev");
			autoIncrementBuild = LoadBoolKey(true, "VersionEditorSettings.AutoIncrBuild");
			autoIncrementBuildByPlayMode = LoadBoolKey(true, "VersionEditorSettings.AutoIncrBuildByPlay");
			buildNumberEditor = LoadBoolKey(false, "VersionEditorSettings.BuildNumberEditor");
		}

		bool LoadBoolKey (bool defaultValue, string name) {
			if (EditorPrefs.HasKey(name)) {
				return EditorPrefs.GetBool(name);
			} else {
				EditorPrefs.SetBool(name, defaultValue);
				return defaultValue;
			}

		}

		void DeleteKeys () {
			EditorPrefs.DeleteKey("VersionEditorSettings.ResetPrev");
			EditorPrefs.DeleteKey("VersionEditorSettings.AutoIncrBuild");
			EditorPrefs.DeleteKey("VersionEditorSettings.AutoSetVersion");
			EditorPrefs.DeleteKey("VersionEditorSettings.AutoIncrBuildByPlay");
			EditorPrefs.DeleteKey("VersionEditorSettings.BuildNumberEditor");
		}
	}
}

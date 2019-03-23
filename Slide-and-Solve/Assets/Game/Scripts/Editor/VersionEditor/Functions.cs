using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Wokarol.VersionEditor {
	public class Functions{

		public const char buildPrefix = 'b';

		public static VersionDataPack LoadVersionData () {
			string versionString = PlayerSettings.bundleVersion;
			VersionDataPack tempVersionData = new VersionDataPack();
			var numbers = versionString.Split('.');
			if (numbers.Length != 3) {
				return tempVersionData;
			}
			int value = 0;
			if (int.TryParse(numbers[0], out value)) {
				tempVersionData.major = value;
			} else {
				tempVersionData.major = 0;
			}
			if (int.TryParse(numbers[1], out value)) {
				tempVersionData.minor = value;
			} else {
				tempVersionData.minor = 0;
			}

			var subMinorAndbuild = numbers[2].Split(buildPrefix);
			if (subMinorAndbuild.Length != 2) {
				return tempVersionData;
			}

			if (int.TryParse(subMinorAndbuild[0], out value)) {
				tempVersionData.subMinor = value;
			} else {
				tempVersionData.subMinor = 0;
			}
			if (int.TryParse(subMinorAndbuild[1], out value)) {
				tempVersionData.build = value;
			} else {
				tempVersionData.build = 0;
			}
			return tempVersionData;
		}


		public static void SetVersion (VersionDataPack versionData) {
			PlayerSettings.bundleVersion = versionData.major + "." + versionData.minor + "." + versionData.subMinor + Functions.buildPrefix + versionData.build;
		}


	}

	public class AutoIncrementing :IPostprocessBuildWithReport, IPreprocessBuildWithReport {

		public int callbackOrder { get { return 0; } }

		public void OnPostprocessBuild (BuildReport report) {
			if (EditorPrefs.GetBool("VersionEditorSettings.AutoIncrBuild")) {
				VersionDataPack tempVersionData = Functions.LoadVersionData();

				tempVersionData.build++;
				Functions.SetVersion(tempVersionData);
			}
			Functions.SetVersion(Functions.LoadVersionData());
			VersionEditor window = (VersionEditor)EditorWindow.GetWindow(typeof(VersionEditor));
			window.UpdateVersionInWindow();
		}

		public void OnPreprocessBuild (BuildReport report) {
			Functions.SetVersion(Functions.LoadVersionData());
			VersionEditor window = (VersionEditor)EditorWindow.GetWindow(typeof(VersionEditor));
			window.UpdateVersionInWindow();
		}
	}



	[InitializeOnLoad]
	public class PlayModeHandler :Editor {

		static PlayModeHandler () {
			EditorApplication.playModeStateChanged += ModeChanged;
		}

		static void ModeChanged (PlayModeStateChange playModeState) {
			if (playModeState == PlayModeStateChange.EnteredEditMode) {
				Functions.SetVersion(Functions.LoadVersionData());
			}
			if (playModeState == PlayModeStateChange.ExitingEditMode) {
				if (EditorPrefs.GetBool("VersionEditorSettings.AutoIncrBuildByPlay")) {
					VersionDataPack tempVersionData = Functions.LoadVersionData();
					tempVersionData.build++;
					Functions.SetVersion(tempVersionData);
					VersionEditor[] windows = FindObjectsOfType<VersionEditor>();
					foreach (VersionEditor editor in windows) {
						editor.UpdateVersionInWindow();
					}

				}
			}
		}

	}

	public struct VersionDataPack {
		public int major;
		public int minor;
		public int subMinor;
		public int build;

		public override bool Equals (object target) {
			VersionDataPack obj = (VersionDataPack)target;
			return (major == obj.major &&
				minor == obj.minor &&
				subMinor == obj.subMinor &&
				build == obj.build);
		}
		public static bool operator == (VersionDataPack obj1, VersionDataPack obj2) {
			return (obj2.major == obj1.major &&
				obj2.minor == obj1.minor &&
				obj2.subMinor == obj1.subMinor &&
				obj2.build == obj1.build);
		}
		public static bool operator != (VersionDataPack obj1, VersionDataPack obj2) {
			return !(obj2.major == obj1.major &&
				obj2.minor == obj1.minor &&
				obj2.subMinor == obj1.subMinor &&
				obj2.build == obj1.build);
		}
		public override int GetHashCode () {
			int hash = 13;
			hash = (hash * 7) + major.GetHashCode();
			hash = (hash * 7) + minor.GetHashCode();
			hash = (hash * 7) + subMinor.GetHashCode();
			hash = (hash * 7) + build.GetHashCode();
			return hash;
		}
	}
}
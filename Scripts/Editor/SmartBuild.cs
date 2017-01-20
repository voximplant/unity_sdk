using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames.xcode;

public class ScriptBatch
{
    [UnityEditor.MenuItem("SmartBuild/Export Android project")]
    public static void AndroidBuildProjec()
    {
        // Get path to build
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built project", "", "");

        if (path == "")
        {
            return;
        }
#if UNITY_5_5
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.ADT;
#endif
        // Build player.
		BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, path, BuildTarget.Android, BuildOptions.AcceptExternalModificationsToPlayer);
    }

	[UnityEditor.MenuItem("SmartBuild/Export xCode project")]
	public static void iOSBuild()
	{
		// Get path to build
		string path = EditorUtility.SaveFolderPanel("Choose Location of Built project", "", "");

		if (path == "")
		{
			return;
		}

		PlayerSettings.SetPropertyInt( "ScriptingBackend", (int)ScriptingImplementation.IL2CPP, BuildTargetGroup.iOS );
		PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneAndiPad;
		PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
		PlayerSettings.iOS.targetOSVersion = iOSTargetOSVersion.iOS_8_1;
		PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0;

		// Build player.
		BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, path, BuildTarget.iOS, BuildOptions.AcceptExternalModificationsToPlayer);
	}

	[PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
	{
		switch (buildTarget) {
		case BuildTarget.iOS:
			{
				// Get and edit plist
				string plistPath = path + "/Info.plist";

				PlistDocument plist = new PlistDocument();
				plist.ReadFromString(File.ReadAllText(plistPath));
				PlistElementDict rootDict = plist.root;
				rootDict.SetString("NSMicrophoneUsageDescription","&");
				rootDict.SetString("NSCameraUsageDescription","&");
				File.WriteAllText(plistPath, plist.WriteToString());

				// set build settings
				string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
				PBXProject proj = new PBXProject();
				proj.ReadFromString(File.ReadAllText(projPath));

				string nativeTarget = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
				string testTarget = proj.TargetGuidByName(PBXProject.GetUnityTestTargetName());
				string[] buildTargets = new string[]{nativeTarget, testTarget};

				proj.SetBuildProperty(buildTargets, "ENABLE_BITCODE", "NO");
				proj.SetBuildProperty(buildTargets, "ARCHS", "$(ARCHS_STANDARD)");

				// save proj
				File.WriteAllText(projPath, proj.WriteToString());

				break;
			}
		case BuildTarget.Android:
			{
				if (path.Contains(".apk"))
					break;

        string classPath = "/com/voximplant/sdk";
				string[] filesList = Directory.GetFiles(path, "UnityPlayerActivity.java", SearchOption.AllDirectories);

				if (filesList.Length == 0)
				{
					EditorUtility.DisplayDialog("Error", "Can't find UnityPlayerActivity.java", "Ok");
					return;
				}

				string activityPath = filesList[0];

				string[] allLines = File.ReadAllLines( activityPath );
				List<string> linesList = new List<string>(allLines);

				bool importsTargetAdd = false;
				bool variableAdd = false;
				bool initAdd = false;
				bool permisionRequestAdd = false;
				bool existModifiy = false;

				// if we already modify activity
				for(int i = 0; i < linesList.Count; i++)
				{
					if (linesList[i].Contains("private AVoImClient mVoxClient"))
					{
						existModifiy = true;
					}
				}

				if (!existModifiy)
				{
					for(int i = 0; i < linesList.Count; i++)
					{
						if (linesList[i].Contains("{") && !variableAdd)
						{
							linesList.Insert(i+1, "\tprivate AVoImClient mVoxClient;" +
								" // don't change the name of this variable; referenced from native code\n");
							variableAdd = true;
						}

						if (linesList[i].Contains("import") && !importsTargetAdd)
						{
							linesList.Insert(i+1, "import android.annotation.TargetApi;");
							linesList.Insert(i+2, "import com.voximplant.sdk.AVoImClient;");
							importsTargetAdd = true;
						}

						if (linesList[i].Contains("setContentView") && !initAdd)
						{
							linesList.Insert(i+1, "\t  mVoxClient = new AVoImClient(this);");
							initAdd = true;
						}

						if (linesList[i].Contains("mUnityPlayer.resume();") && !permisionRequestAdd)
						{
							linesList.Insert(i+2,"\n" + "\t@TargetApi(22)\n" +
								"\t@Override\n " +
								"\tpublic void onRequestPermissionsResult(int requestCode, String permissions[], int[] grantResults) \n\t{\n" +
								"\t\tmVoxClient.Init();\n" +
								"\t}");
							permisionRequestAdd = true;
						}
					}
				}

				//write file
				if (!existModifiy)
					File.WriteAllLines(activityPath, linesList.ToArray());

				// add package additional files
				string[] dirList = Directory.GetDirectories(path,"src", SearchOption.AllDirectories);

				if (dirList.Length != 0)
				{
					if (Directory.Exists("Assets/androidPartSDK/comvoximplant/") && Directory.Exists(dirList[0]))
					{
						string newPackagePath = dirList[0] + classPath;
						if (Directory.Exists(newPackagePath))
							Directory.Delete(newPackagePath,true);

						Directory.CreateDirectory(newPackagePath);
						string[] fileList = Directory.GetFiles("Assets/androidPartSDK/comvoximplant/","*.java");

						foreach(string filePath in fileList)
						{
							FileStream fileStream = File.Create(newPackagePath + "/" + Path.GetFileName(filePath));
							byte[] chunk = File.ReadAllBytes(filePath);
							fileStream.BeginWrite(chunk, 0, chunk.Length, null, null);
						}
					}
				}
				break;
			}
		default:
			break;
		}
	}
}

using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using System;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace Core.Scripts.Editor.Postprocess
{
    public class PostprocessBuildUtility : IPostprocessBuildWithReport, IPreprocessBuildWithReport
    {
        const string PLIST_FILE_NAME = "GoogleService-Info.plist";

        int IOrderedCallback.callbackOrder => 10;

        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.result != BuildResult.Succeeded &&
                report.summary.result != BuildResult.Unknown)
                return;
#if UNITY_IOS
            if (report.summary.platform == BuildTarget.iOS)
                ManageIOSPostBuild(report);
#elif UNITY_ANDROID
            if (report.summary.platform == BuildTarget.Android)
                ManageAndroidPostBuild(report);
#endif
        }

    

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            var platform = report.summary.platform;
#if UNITY_IOS
            var date = DateTime.Now.ToString("yyyyMMdd");
            var version = Version(date);
            version.set(version.get() + 1);
            PlayerSettings.iOS.buildNumber = $"{date}.{version.get()}";
            //PlayerSettings.stripEngineCode = true;
#elif UNITY_ANDROID
            var versionCode = PlayerSettings.Android.bundleVersionCode;
            var version = Version(platform.ToString());
            version.set( Math.Max(version.get(), versionCode) + 1);
            PlayerSettings.Android.bundleVersionCode = version.get();
            //PlayerSettings.stripEngineCode = false;
#endif
        }
        
        private (Func<int> get, Action<int> set) Version(string key)
            => (() => EditorPrefs.GetInt($"{key}.version", 0), 
                value => EditorPrefs.SetInt($"{key}.version", value));

#if UNITY_ANDROID
        private void ManageAndroidPostBuild(BuildReport report)
        {
            
        }
        
#elif UNITY_IOS
        private void ManageIOSPostBuild(BuildReport report)
        {
            string projPath = PBXProject.GetPBXProjectPath(report.summary.outputPath);
            PBXProject project = new PBXProject();
            project.ReadFromFile(projPath);

            string plistPath = Path.Combine(report.summary.outputPath, "Info.plist");
            PlistDocument plist = new PlistDocument(); // Read Info.plist file into memory
            plist.ReadFromFile(plistPath);

            SetEncryption(plist.root);
            //FixBundleVersion(plist.root);
            FixGoogleServiceInfo(project, plist.root, report.summary.outputPath);
            FixSwiftLibraries(project);
            
            // Simply create a .command file which can be run in OSX to fix some permissions
            CreateFixIOSBuildScriptsCommandFile(project, report.summary.outputPath);
            
            plist.WriteToFile(plistPath);
            project.WriteToFile(projPath);
        }
       
        private void CreateFixIOSBuildScriptsCommandFile(PBXProject project, string pathToBuiltProject)
        {
            var script = "chmod u+x \"${PROJECT_DIR}/MapFileParser.sh\"\n" +
                             "chmod u+x \"${PROJECT_DIR}/process_symbols.sh\"";
            project.InsertShellScriptBuildPhase(
                0,
                project.GetUnityFrameworkTargetGuid(),
                "Make executable",
                "/bin/sh -x",
                script);
            
            var filePath = Path.Combine(pathToBuiltProject, "runme_fix_permissions.command");

            File.WriteAllText(
                filePath,
                "# This fixes errors where Xcode couldn't execute these scripts" + "\n" +

                // Needed because current directory is not set when running .command files. gearsdigital's life-saving comment on this SO answer: https://stackoverflow.com/a/9650209
                "cd \"`dirname \"$0\"`\"" + "\n" +

                "chmod a+x MapFileParser.sh" + "\n" +
                "chmod a+x process_symbols.sh"
            );
        }

        private void SetEncryption(PlistElementDict root, bool value = false)
            => root.SetBoolean("ITSAppUsesNonExemptEncryption", value);

        private void FixSwiftLibraries(PBXProject project)
        {
            var unityFrameworkTargetGuid = project.GetUnityFrameworkTargetGuid();
            var unityMainTargetGuid = project.GetUnityMainTargetGuid();

            project.AddBuildProperty(
                unityFrameworkTargetGuid,
                "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES",
                "NO");
            project.AddBuildProperty(
                unityMainTargetGuid,
                "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES",
                "YES");
            
            var script = "cd \"${CONFIGURATION_BUILD_DIR}/UnityFramework.framework/\"\n" +
                         "if [[ -d \"Frameworks\" ]]; then\n" +
                         "  rm -rf \"Frameworks\"\n" +
                         "fi";
            
            // script = "cd \"${CONFIGURATION_BUILD_DIR}/${UNLOCALIZED_RESOURCES_FOLDER_PATH}/\"\n" +
            //              "if [[ -d \"Frameworks\" ]]; then\n" +
            //              "  rm -rf \"Frameworks\"\n" +
            //              "fi";

            project.AddShellScriptBuildPhase(
                unityFrameworkTargetGuid,
                "Remove Frameworks",
                "/bin/sh -x",
                script);
            
            project.InsertShellScriptBuildPhase(
                0,
                unityMainTargetGuid,
                "Remove Frameworks",
                "/bin/sh -x",
                script);
        }

        private void FixBundleVersion(PlistElementDict root)
        {
            var date = DateTime.Now.ToString("yyyyMMdd");
            var version = Version(date);
            version.set(version.get() + 1);
            root.SetString("CFBundleVersion", $"{date}.{version.get()}");
        }

        private void FixGoogleServiceInfo(PBXProject project, PlistElementDict root, string pathToBuiltProject)
        {
            string target = project.GetUnityMainTargetGuid();

            string[] files = Directory.GetFiles("Assets", PLIST_FILE_NAME, SearchOption.AllDirectories);

            if (files.Length > 0)
            {
                // Copy plist from the project folder to the build folder
                FileUtil.ReplaceFile(files[0], Path.Combine(pathToBuiltProject, PLIST_FILE_NAME));
                project.AddFileToBuild(target, project.AddFile(PLIST_FILE_NAME, PLIST_FILE_NAME));

                // add URLType
                var plistCred = new PlistDocument();
                plistCred.ReadFromFile(files[0]);

                if (root["CFBundleURLTypes"] == null)
                    root.CreateArray("CFBundleURLTypes");

                var urlTypeArray = root["CFBundleURLTypes"].AsArray();
                var urlTypeDict = urlTypeArray.AddDict();
                urlTypeDict.SetString("CFBundleTypeRole", "Editor");

                var urlScheme = urlTypeDict.CreateArray("CFBundleURLSchemes");
                urlScheme.AddString(plistCred.root["REVERSED_CLIENT_ID"].AsString());
            }
            
            //https://firebase.google.com/docs/reference/unity/class/firebase/messaging/firebase-messaging#tokenregistrationoninitenabled
            root.SetBoolean("FirebaseMessagingAutoInitEnabled", false);
            root.SetBoolean("FIREBASE_ANALYTICS_COLLECTION_ENABLED", false);
        }
#endif
    }
}
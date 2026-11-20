#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace JackpotSlots.Editor
{
    /// <summary>Repeatable one-command WebGL build used locally and by CI.</summary>
    public static class SlotBuildTools
    {
        private const string ScenePath = "Assets/Scenes/SampleScene.unity";
        private const string OutputPath = "Build/WebGL";

        [MenuItem("Jackpot Slots/Build WebGL")]
        public static void BuildWebGL()
        {
            Directory.CreateDirectory(OutputPath);
            var options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = OutputPath,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
                throw new BuildFailedException($"WebGL build failed with {report.summary.totalErrors} error(s).");

            Debug.Log($"WebGL build completed: {Path.GetFullPath(OutputPath)}");
        }
    }
}
#endif

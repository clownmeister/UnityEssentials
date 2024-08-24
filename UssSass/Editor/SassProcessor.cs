﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;

namespace ClownMeister.UssSass.Editor
{
    internal class SassProcessor : AssetPostprocessor
    {
        //Common settings
        private const Mode Mode = Editor.Mode.Entrypoint;

        //Entrypoints mode settings
        private const string Entrypoint = "Assets/UI/src/index.scss";
        private const string DistTarget = "Assets/UI/dist/Main.uss";
        private static readonly string[] TargetExtensions = { "scss", "css", "sass" };

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            IEnumerable<string> files = importedAssets
                .Where(filePath => filePath.Split('.').Length > 1 && TargetExtensions.Contains(filePath.Split(".")[1].ToLower()));

            foreach (string filePath in files)
            {
                Process process = new()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        FileName = "sass",
                        Arguments = GetSassArguments(Mode, filePath)
                    }
                };
                process.Start();
            }
        }

        private static string GetSassArguments(Mode mode, string filePath)
        {
            string path = filePath.Split('.')[0];

            return mode == Mode.Classic
                ? string.Format(" --no-source-map {0} {1}.uss", filePath, path)
                : string.Format(" --no-source-map {0} {1}", Entrypoint, DistTarget);
        }
    }

    internal enum Mode
    {
        Classic,
        Entrypoint
    }
}
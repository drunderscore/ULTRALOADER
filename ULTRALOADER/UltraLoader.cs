using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ULTRALOADER
{
    public class UltraLoader : MonoBehaviour
    {
        public const string ModsDirectory = "Mods";
        public const string GitHubLatestReleaseUrl = "https://api.github.com/repos/drunderscore/ULTRALOADER/releases/latest";
        private static Harmony _harmony;
        private static readonly List<Mod> _mods = new List<Mod>();
        private static GameObject _loaderObject;
        private static string _subtext;
        private static GitHubRelease _latestGitHubRelease;
        public static ReadOnlyCollection<Mod> Mods => _mods.AsReadOnly();

        // ReSharper disable once UnusedMember.Global
        public static void Main()
        {
            UltraLoaderEvents.CheatsManagerRenderInfo += OnCheatsManagerRenderInfo;
            // Unity Doorstop puts us _way_ too early into the initialization... we really need to wait for Unity to load and link
            // Harmony suggests waiting for SceneManager.sceneLoaded, so that's exactly what we do!
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static GitHubRelease RequestLatestGitHubRelease()
        {
            try
            {
                var request = WebRequest.CreateHttp(GitHubLatestReleaseUrl);
                request.UserAgent =
                    $"ULTRALOADER/{typeof(UltraLoader).Assembly.GetName().Version} My sole purpose is to check the version. What a repetitive life.";
                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Debug.LogWarning(
                        $"Got non-OK status code {response.StatusCode} when requesting the latest GitHub release for ULTRALOADER: {response.StatusDescription}");
                }
                else
                {
                    using var textResponseReader = new StreamReader(response.GetResponseStream());
                    var textResponse = textResponseReader.ReadToEnd();
                    return JsonUtility.FromJson<GitHubRelease>(textResponse);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed requesting latest GitHub release for ULTRALOADER");
                Debug.LogException(e);
            }

            return null;
        }

        private static void OnCheatsManagerRenderInfo(ref string value)
        {
            // TODO: This would be much nicer as a button on the Main Menu, with a list of mod there.
            value += "Mods:\n";
            foreach (var mod in Mods)
                value += $"{mod.Assembly.GetName().Name}\n";
        }

        private static void Initialize()
        {
            _harmony = new Harmony("xyz.jame.ultraloader");
            _harmony.PatchAll();

            // TODO: Obviously anyone could now make a mod that cheats, but would count towards normal game progression and Cyber Grind,
            //        which is not okay! We should forcefully enable cheats, and in the future, show a UI screen similar to the cheat
            //        consent when the game starts.

            _loaderObject = new GameObject("ULTRALOADER");
            DontDestroyOnLoad(_loaderObject);
            _loaderObject.AddComponent<UltraLoader>();

            if (Directory.Exists(ModsDirectory))
            {
                Debug.Log($"Version {typeof(UltraLoader).Assembly.GetName().Version} loading mods at {DateTime.Now}");

                foreach (var modPath in Directory.EnumerateFiles(ModsDirectory, "*.dll"))
                {
                    try
                    {
                        var modAssembly = Assembly.LoadFile(modPath);
                        foreach (var type in modAssembly.GetTypes())
                        {
                            var entryAttributes = type.GetCustomAttributes(typeof(Entry), false);

                            if (entryAttributes.Length != 1)
                                continue;

                            // We just assume this is a valid component (inherits MonoBehaviour, not abstract)... I think that's okay
                            // Unity will let us know if something is wrong.

                            var modObject = new GameObject(modAssembly.FullName);
                            DontDestroyOnLoad(modObject);
                            var modComponent = modObject.AddComponent(type);
                            _mods.Add(new Mod
                            {
                                Object = modObject,
                                Component = modComponent,
                                Assembly = modAssembly
                            });
                            Debug.Log($"Loaded mod {modPath} ({type.FullName})");

                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to load mod file {modPath}");
                        Debug.LogException(e);
                    }
                }

                Debug.Log($"{_mods.Count} mods loaded");

                var currentVersion = typeof(UltraLoader).Assembly.GetName().Version;
                _subtext = $"ULTRALOADER {currentVersion} - {Mods.Count} mod{(Mods.Count == 1 ? string.Empty : "s")} loaded";

                if ((_latestGitHubRelease = RequestLatestGitHubRelease()) != null)
                {
                    if (Version.TryParse(_latestGitHubRelease.tag_name, out var latestGitHubVersion))
                    {
                        if (latestGitHubVersion > currentVersion)
                        {
                            _subtext += $" - UPDATE AVAILABLE to {_latestGitHubRelease.tag_name}";
                            Debug.Log($"ULTRALOADER update available from version {currentVersion} to {_latestGitHubRelease.tag_name}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Unable to parse latest GitHub release version string \"{_latestGitHubRelease.tag_name}\"");
                    }
                }
            }
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(5, Screen.height - 20, 800, 20), _subtext);
        }

        private static void OnSceneLoaded(Scene _, LoadSceneMode __)
        {
            // Remove first, in the case that Initialize fails, we don't want to get stuck in a loop each time a scene loads
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Initialize();
        }

        public class Mod
        {
            public GameObject Object { get; internal set; }
            public Component Component { get; internal set; }
            public Assembly Assembly { get; internal set; }
        }

        [AttributeUsage(AttributeTargets.Class)]
        public class Entry : Attribute
        {
        }
    }
}
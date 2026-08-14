#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace GenericWorkflow.Editor
{
        /// <summary>
    /// Editor window that imports raw text and generates WorkflowGuideAsset files.
    /// Use Tools > Workflow Raw Text Importer (Dyslexia-Friendly) to open.
    /// </summary>
    public class WorkflowTextImporterWindow : EditorWindow
    {
        private string guideTitle = "VR Tutorial Workflow Guide";
        private string creatorName = "Sunny Valley Studio";
        private string videoTitle = "VR Tutorial Essentials";
        private string mainVideoUrl = "https://www.youtube.com/watch?v=sample123";
        private string githubRepoUrl = "https://github.com/example/repo";

        private static readonly string aiPromptTemplate =
@"Please analyze the following YouTube tutorial transcript/description and format it for my Unity Workflow Assistant tool using the exact format below.

INSTRUCTIONS FOR AI:
1. Chapter Titles: Group steps into logical Section headers (e.g., Section 1: Setup, Section 2: Mechanics).
2. Timestamps & Steps: Extract chronological timestamps using format [MM:SS] with step-by-step instructions.
3. Key Insights & Gotchas: Highlight critical gotchas, missed steps, hardware caveats, and component warnings.
4. Tools & Packages: List any Unity packages or tools referenced (e.g. XRIT 3.0, OpenXR).
5. GitHub Repositories: Include any GitHub code repository links mentioned.

--- PROMPT FORMAT TEMPLATE ---

Guide Title: [Tutorial Topic Name]
Creator Name: [Creator Name]
Video Title: [Video Title]
Video URL: [YouTube Link]
GitHub URL: [GitHub Repo Link]

Section 1: [Chapter Title]
[MM:SS] [Step Title & Step-by-Step Description]
[MM:SS] [Step Title & Step-by-Step Description]
Insights: [Key Insights, Gotchas & Component Warnings]

Section 2: [Chapter Title]
[MM:SS] [Step Title & Step-by-Step Description]
Insights: [Key Insights, Gotchas & Component Warnings]

--- MY RAW TRANSCRIPT / NOTES BELOW ---
[Paste your raw YouTube transcript or notes here]
";

        [TextArea(12, 25)]
        private string rawText = @"Section 1: Blank Project Setup & Package Installation
00:00 Introduction & Overview
00:45 Installing XR Interaction Toolkit 3.0+
03:22 Project Settings & Validation Fixes
06:44 Configuring Input Action Manager Asset

Section 2: Core Mechanics & Hand Interaction
08:30 Setting up Grabbable Objects & Layer Masks
10:15 Rigidbody Kinematics & Pull Constraints
15:40 Writing C# Interaction Controller Script

Section 3: Audio & Haptic Vibrations
18:10 Adding 3D Audio Source for Sound Effects
22:05 Triggering Controller Haptic Impulses";

        private Vector2 scrollPos;
        private bool showAiPromptFoldout = true;

        [MenuItem("Tools/Workflow Raw Text Importer (Dyslexia-Friendly)")]
        public static void ShowWindow()
        {
            WorkflowTextImporterWindow window = GetWindow<WorkflowTextImporterWindow>("Workflow Raw Text Importer");
            window.minSize = new Vector2(650, 750);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Raw Text & Timestamp Auto-Importer (Dyslexia-Friendly)", EditorStyles.boldLabel);
            
            EditorGUILayout.HelpBox(
                "HOW IT WORKS:\n" +
                "1. Copy the AI Prompt template below to ask AI to analyze any tutorial (chapter titles, timestamps, insights, tools, and GitHub links).\n" +
                "2. Paste the AI output or raw video description into the text area below.\n" +
                "3. Click 'Generate Workflow Guide Asset' to build your ready-to-use Unity guide with zero manual entry!",
                MessageType.Info
            );

            EditorGUILayout.Space(10);
            
            showAiPromptFoldout = EditorGUILayout.Foldout(showAiPromptFoldout, "🤖 AI Formatting Prompt Template (Click to Expand)", true, EditorStyles.foldoutHeader);
            if (showAiPromptFoldout)
            {
                EditorGUILayout.TextArea(aiPromptTemplate, GUILayout.Height(150));
            }

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Guide Metadata", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical(GUI.skin.box);
            guideTitle = EditorGUILayout.TextField("Guide Title:", guideTitle);
            creatorName = EditorGUILayout.TextField("Creator Name:", creatorName);
            videoTitle = EditorGUILayout.TextField("Video Title:", videoTitle);
            mainVideoUrl = EditorGUILayout.TextField("Video URL:", mainVideoUrl);
            githubRepoUrl = EditorGUILayout.TextField("GitHub Repo URL:", githubRepoUrl);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Paste Raw Text Here (Timestamps, Markdown, AI Output):", EditorStyles.boldLabel);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(250));
            rawText = EditorGUILayout.TextArea(rawText, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(20);
            
            GUI.backgroundColor = new Color(0.2f, 0.8f, 0.2f);
            if (GUILayout.Button("⚡ Generate Workflow Guide Asset", GUILayout.Height(42)))
            {
                GenerateAssetFromRawText();
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.Space(10);
            
            if (GUILayout.Button("Load Demo Format", GUILayout.Height(30)))
            {
                LoadDemoFormat();
            }
        }

        private void LoadDemoFormat()
        {
            guideTitle = "VR Interaction & Physics Masterclass";
            creatorName = "Sunny Valley Studio";
            videoTitle = "How to Build VR Mechanics in Unity 6";
            mainVideoUrl = "https://www.youtube.com/watch?v=sample123";
            githubRepoUrl = "https://github.com/example/vr-tutorial";

            rawText = @"Section 1: Blank Project Setup & Package Installation
00:00 Introduction & Overview
00:45 Installing XR Interaction Toolkit 3.0+
03:22 Project Settings & Validation Fixes
06:44 Configuring Input Action Manager Asset

Section 2: Core Mechanics & Hand Interaction
08:30 Setting up Grabbable Objects & Layer Masks
10:15 Rigidbody Kinematics & Pull Constraints
15:40 Writing C# Interaction Controller Script

Section 3: Audio & Haptic Vibrations
18:10 Adding 3D Audio Source for Sound Effects
22:05 Triggering Controller Haptic Impulses";

            GUI.FocusControl(null);
            Repaint();
        }

        private void GenerateAssetFromRawText()
        {
            if (string.IsNullOrEmpty(rawText))
            {
                EditorUtility.DisplayDialog("Error", "Please paste some raw text or timestamps first!", "OK");
                return;
            }

            WorkflowGuideAsset asset = ScriptableObject.CreateInstance<WorkflowGuideAsset>();
            asset.guideTitle = string.IsNullOrEmpty(guideTitle) ? "Generated Workflow Guide" : guideTitle;
            asset.guideSubtitle = $"Interactive tutorial workflow assistant for {videoTitle} by {creatorName}";
            asset.uniqueGuideId = "Guide_" + Guid.NewGuid().ToString().Substring(0, 8);
            asset.rawTranscript = rawText;

            string cleanMainVideoUrl = CleanYouTubeUrl(mainVideoUrl);

            if (!string.IsNullOrEmpty(cleanMainVideoUrl) && cleanMainVideoUrl.StartsWith("http"))
            {
                asset.resourceLinks.Add(new ResourceLinkData { label = $"Video: {creatorName}", url = cleanMainVideoUrl });
            }
            if (!string.IsNullOrEmpty(githubRepoUrl) && githubRepoUrl.StartsWith("http"))
            {
                asset.resourceLinks.Add(new ResourceLinkData { label = "GitHub Repository", url = githubRepoUrl });
            }

            Regex timestampRegex = new Regex(@"(?:\[|\()?(?:(\d{1,2}):)?(\d{1,2}):(\d{2})(?:\]|\))?");
            Regex phaseRegex = new Regex(@"^(?:Phase|Section|Chapter)\s*(\d+)[:\s-]*(.*)$", RegexOptions.IgnoreCase);

            string[] lines = rawText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            int currentPhaseNum = 0;
            WorkflowPhaseData currentPhase = null;
            WorkflowStepData currentStep = null;
            CreatorBreakdownData creatorBreakdown = null;
            int fallbackPhaseNum = 1;

            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                Match phaseMatch = phaseRegex.Match(trimmed);
                if (phaseMatch.Success)
                {
                    if (currentStep != null && creatorBreakdown != null && creatorBreakdown.timestamps.Count > 0)
                    {
                        currentStep.creatorBreakdowns.Add(creatorBreakdown);
                        asset.steps.Add(currentStep);
                    }

                    int parsedPhase = fallbackPhaseNum;
                    if (int.TryParse(phaseMatch.Groups[1].Value, out int regexPhase))
                    {
                        parsedPhase = regexPhase;
                        fallbackPhaseNum = parsedPhase + 1;
                    }
                    else
                    {
                        parsedPhase = fallbackPhaseNum++;
                    }
                    
                    // Don't add a duplicate phase if one with the same number already exists.
                    // Instead, we just reuse the phase number for the new step if it was a duplicate "Section X".
                    currentPhaseNum = parsedPhase;
                    bool phaseExists = asset.phases.Exists(p => p.phaseNumber == currentPhaseNum);
                    
                    string phaseTitleStr = phaseMatch.Groups[2].Value.Trim();
                    if (string.IsNullOrEmpty(phaseTitleStr)) phaseTitleStr = trimmed;

                    if (!phaseExists)
                    {
                        currentPhase = new WorkflowPhaseData
                        {
                            phaseNumber = currentPhaseNum,
                            phaseTitle = $"Section {currentPhaseNum}: {phaseTitleStr}"
                        };
                        asset.phases.Add(currentPhase);
                    }

                    // Count existing steps in this phase to generate a unique step id
                    int stepsInPhase = asset.steps.FindAll(s => s.phaseNumber == currentPhaseNum).Count;

                    currentStep = new WorkflowStepData
                    {
                        id = $"Step_{currentPhaseNum}_{stepsInPhase + 1}",
                        phaseNumber = currentPhaseNum,
                        stepTitle = $"Step {currentPhaseNum}.{stepsInPhase + 1}: {phaseTitleStr}",
                        description = $"Tutorial section {currentPhaseNum} steps and timestamps."
                    };

                    creatorBreakdown = new CreatorBreakdownData
                    {
                        creatorName = creatorName,
                        videoTitle = videoTitle,
                        githubUrl = githubRepoUrl
                    };
                    continue;
                }

                Match tsMatch = timestampRegex.Match(trimmed);
                if (tsMatch.Success)
                {
                    if (creatorBreakdown == null)
                    {
                        // Fallback if timestamps appear before any phase
                        currentPhaseNum = fallbackPhaseNum++;
                        currentPhase = new WorkflowPhaseData { phaseNumber = currentPhaseNum, phaseTitle = $"Section {currentPhaseNum}: General Setup & Overview" };
                        asset.phases.Add(currentPhase);
                        currentStep = new WorkflowStepData
                        {
                            id = $"Step_{currentPhaseNum}_1",
                            phaseNumber = currentPhaseNum,
                            stepTitle = $"Step {currentPhaseNum}.1: Core Tutorial Steps & Timestamps",
                            description = "Follow along with the timestamped steps below."
                        };
                        creatorBreakdown = new CreatorBreakdownData
                        {
                            creatorName = creatorName,
                            videoTitle = videoTitle,
                            githubUrl = githubRepoUrl
                        };
                    }

                    int totalSeconds = ConvertTimestampToSeconds(tsMatch);

                    int hours = 0, minutes = 0, seconds = 0;
                    if (!string.IsNullOrEmpty(tsMatch.Groups[1].Value)) int.TryParse(tsMatch.Groups[1].Value, out hours);
                    if (!string.IsNullOrEmpty(tsMatch.Groups[2].Value)) int.TryParse(tsMatch.Groups[2].Value, out minutes);
                    if (!string.IsNullOrEmpty(tsMatch.Groups[3].Value)) int.TryParse(tsMatch.Groups[3].Value, out seconds);

                    string timeLabel = hours > 0 ? $"{hours}:{minutes:D2}:{seconds:D2}" : $"{minutes:D2}:{seconds:D2}";
                    string desc = trimmed.Replace(tsMatch.Value, "").Trim('-', ':', ' ', '[', ']', '(', ')');
                    if (string.IsNullOrEmpty(desc)) desc = "Tutorial timestamp step";

                    string timeUrl = BuildYouTubeTimestampUrl(mainVideoUrl, totalSeconds);

                    creatorBreakdown.timestamps.Add(new TimestampData
                    {
                        timeLabel = timeLabel,
                        url = timeUrl,
                        description = desc
                    });
                }
            }

            if (currentStep != null && creatorBreakdown != null && creatorBreakdown.timestamps.Count > 0)
            {
                currentStep.creatorBreakdowns.Add(creatorBreakdown);
                asset.steps.Add(currentStep);
            }

            string path = EditorUtility.SaveFilePanelInProject(
                "Save Workflow Guide Asset",
                SanitizeFilename(asset.guideTitle) + ".asset",
                "asset",
                "Select where to save the generated Workflow Guide Asset"
            );

            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = asset;

                EditorUtility.DisplayDialog("Success!", $"Workflow Guide Asset created successfully at:\n{path}", "OK");
            }
        }

        private int ConvertTimestampToSeconds(Match match)
        {
            int hours = 0;
            int minutes = 0;
            int seconds = 0;

            if (!string.IsNullOrEmpty(match.Groups[1].Value))
            {
                int.TryParse(match.Groups[1].Value, out hours);
            }
            if (!string.IsNullOrEmpty(match.Groups[2].Value))
            {
                int.TryParse(match.Groups[2].Value, out minutes);
            }
            if (!string.IsNullOrEmpty(match.Groups[3].Value))
            {
                int.TryParse(match.Groups[3].Value, out seconds);
            }

            return (hours * 3600) + (minutes * 60) + seconds;
        }

        public static string CleanYouTubeUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return "";

            string clean = Regex.Replace(url, @"([?&])(?:t|start)=[^&]*", "");
            clean = clean.Replace("?&", "?");
            clean = Regex.Replace(clean, @"&{2,}", "&");
            return clean.TrimEnd('?', '&');
        }

        public static string BuildYouTubeTimestampUrl(string baseUrl, int totalSeconds)
        {
            if (string.IsNullOrEmpty(baseUrl)) return "";
            string cleanBase = CleanYouTubeUrl(baseUrl);
            string separator = cleanBase.Contains("?") ? "&" : "?";
            return $"{cleanBase}{separator}t={totalSeconds}s";
        }

        public static string SanitizeYouTubeUrl(string rawUrl)
        {
            if (string.IsNullOrEmpty(rawUrl)) return "";

            var matches = Regex.Matches(rawUrl, @"[?&](?:t|start)=([^&]+)");
            if (matches.Count <= 1) return rawUrl;

            string targetTime = matches[matches.Count - 1].Groups[1].Value;
            string cleanBase = CleanYouTubeUrl(rawUrl);
            string separator = cleanBase.Contains("?") ? "&" : "?";
            return $"{cleanBase}{separator}t={targetTime}";
        }

        private string SanitizeFilename(string name)
        {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
            return name.Replace(" ", "");
        }
    }
}
#endif

using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace DyslexicScholarsWorkflow
{
    public class WorkflowImporterWindow : EditorWindow
    {
        private string guideTitle = "My New Workflow";
        private string creatorName = "Tutorial Creator";
        private string videoUrl = "https://youtube.com/...";
        private string rawText = "";
        private Vector2 scrollPos;

        private readonly string promptTemplate =
@"Please analyze the following YouTube tutorial transcript/description and format it for my Dyslexic Scholars Workflow tool.

1. Chapter Titles: Group steps into sections like 'Section 1: Setup'.
2. Timestamps & Steps: Extract timestamps as [MM:SS] followed by the step description.
3. Gotchas: Include any critical warnings or gotchas starting with 'Insights:'.

--- PASTE TRANSCRIPT BELOW ---";

        [MenuItem("Tools/Dyslexic Scholars/Workflow Importer")]
        public static void ShowWindow()
        {
            var window = GetWindow<WorkflowImporterWindow>("Workflow Importer");
            window.minSize = new Vector2(600, 700);
        }

        private void OnGUI()
        {
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 18, alignment = TextAnchor.MiddleCenter };
            GUILayout.Space(10);
            GUILayout.Label("📚 Dyslexic Scholars Workflow Importer", titleStyle);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox("1. Copy the AI Prompt below.\n2. Paste it into an AI with your transcript.\n3. Paste the AI's response into the Raw Text box below to generate your Guide Asset.", MessageType.Info);

            GUILayout.Label("AI Prompt Template:", EditorStyles.boldLabel);
            EditorGUILayout.TextArea(promptTemplate, GUILayout.Height(120));

            GUILayout.Space(15);
            GUILayout.Label("Guide Metadata", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            guideTitle = EditorGUILayout.TextField("Guide Title:", guideTitle ?? "");
            creatorName = EditorGUILayout.TextField("Creator Name:", creatorName ?? "");
            videoUrl = EditorGUILayout.TextField("Video URL:", videoUrl ?? "");
            EditorGUILayout.EndVertical();

            GUILayout.Space(15);
            GUILayout.Label("Paste AI Formatted Text Here:", EditorStyles.boldLabel);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(250));
            rawText = EditorGUILayout.TextArea(rawText ?? "", GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();

            GUILayout.Space(15);
            if (GUILayout.Button("⚡ Generate Workflow Guide Asset", GUILayout.Height(40)))
            {
                GenerateAsset();
            }
        }

        private void GenerateAsset()
        {
            if (string.IsNullOrEmpty(rawText.Trim()))
            {
                EditorUtility.DisplayDialog("Error", "Please paste the formatted text first!", "OK");
                return;
            }

            WorkflowGuide asset = ScriptableObject.CreateInstance<WorkflowGuide>();
            asset.guideTitle = guideTitle;
            asset.creatorName = creatorName;
            asset.videoUrl = videoUrl;
            asset.rawTranscript = rawText;

            Regex phaseRegex = new Regex(@"^(?:Phase|Section|Chapter)\s*(\d+)[:\s-]*(.*)$", RegexOptions.IgnoreCase);
            Regex timestampRegex = new Regex(@"(?:\[|\()?(?:(\d{1,2}):)?(\d{1,2}):(\d{2})(?:\]|\))?");
            Regex gotchaRegex = new Regex(@"^(?:Insights|Gotchas|Warning)[:\s]*(.*)$", RegexOptions.IgnoreCase);

            string[] lines = rawText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            int currentSectionNum = 0;
            WorkflowSection currentSection = null;
            WorkflowStep currentStep = null;
            int fallbackSectionNum = 1;

            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                Match phaseMatch = phaseRegex.Match(trimmed);
                if (phaseMatch.Success)
                {
                    if (currentStep != null) asset.steps.Add(currentStep);

                    int parsedPhase = fallbackSectionNum;
                    if (int.TryParse(phaseMatch.Groups[1].Value, out int regexPhase))
                    {
                        parsedPhase = regexPhase;
                        fallbackSectionNum = parsedPhase + 1;
                    }
                    else
                    {
                        parsedPhase = fallbackSectionNum++;
                    }

                    currentSectionNum = parsedPhase;
                    string phaseTitleStr = phaseMatch.Groups[2].Value.Trim();

                    currentSection = new WorkflowSection
                    {
                        sectionNumber = currentSectionNum,
                        sectionTitle = $"Section {currentSectionNum}: {phaseTitleStr}"
                    };
                    asset.sections.Add(currentSection);

                    currentStep = null; // Start a new block of steps
                    continue;
                }

                Match gotchaMatch = gotchaRegex.Match(trimmed);
                if (gotchaMatch.Success && currentStep != null)
                {
                    currentStep.gotchas += gotchaMatch.Groups[1].Value.Trim() + "\n";
                    continue;
                }

                Match tsMatch = timestampRegex.Match(trimmed);
                if (tsMatch.Success)
                {
                    if (currentSection == null)
                    {
                        currentSectionNum = fallbackSectionNum++;
                        currentSection = new WorkflowSection { sectionNumber = currentSectionNum, sectionTitle = $"Section {currentSectionNum}: General Overview" };
                        asset.sections.Add(currentSection);
                    }

                    if (currentStep == null)
                    {
                        int stepsInPhase = asset.steps.FindAll(s => s.sectionNumber == currentSectionNum).Count;
                        currentStep = new WorkflowStep
                        {
                            sectionNumber = currentSectionNum,
                            title = $"Step {currentSectionNum}.{stepsInPhase + 1}"
                        };
                    }

                    int hours = 0, minutes = 0, seconds = 0;
                    if (!string.IsNullOrEmpty(tsMatch.Groups[1].Value)) int.TryParse(tsMatch.Groups[1].Value, out hours);
                    if (!string.IsNullOrEmpty(tsMatch.Groups[2].Value)) int.TryParse(tsMatch.Groups[2].Value, out minutes);
                    if (!string.IsNullOrEmpty(tsMatch.Groups[3].Value)) int.TryParse(tsMatch.Groups[3].Value, out seconds);

                    string timeLabel = hours > 0 ? $"{hours}:{minutes:D2}:{seconds:D2}" : $"{minutes:D2}:{seconds:D2}";
                    string desc = trimmed.Replace(tsMatch.Value, "").Trim('-', ':', ' ', '[', ']', '(', ')');

                    int totalSeconds = (hours * 3600) + (minutes * 60) + seconds;
                    string timeUrl = BuildYouTubeTimestampUrl(videoUrl, totalSeconds);

                    currentStep.timestamps.Add(new WorkflowTimestamp
                    {
                        timeLabel = timeLabel,
                        url = timeUrl,
                        description = desc
                    });
                }
                else if (currentStep != null)
                {
                    // If it's just regular text under a step, add it to description
                    currentStep.description += trimmed + "\n";
                }
            }

            if (currentStep != null) asset.steps.Add(currentStep);

            string path = EditorUtility.SaveFilePanelInProject("Save Workflow Guide", "NewWorkflowGuide.asset", "asset", "Save Guide");
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = asset;
                EditorUtility.DisplayDialog("Success", $"Workflow created at {path}", "OK");
            }
        }

        private string BuildYouTubeTimestampUrl(string baseUrl, int totalSeconds)
        {
            if (string.IsNullOrEmpty(baseUrl)) return "";
            string cleanBase = Regex.Replace(baseUrl, @"([?&])(?:t|start)=[^&]*", "").Replace("?&", "?").TrimEnd('?', '&');
            string separator = cleanBase.Contains("?") ? "&" : "?";
            return $"{cleanBase}{separator}t={totalSeconds}s";
        }
    }
}

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GenericWorkflow.Editor
{
    public class GenericWorkflowWindow : EditorWindow
    {
        public WorkflowGuideAsset activeGuide;

        private Vector2 scrollPosition;
        private string searchFilter = "";

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

        private Dictionary<int, bool> phaseFoldouts = new Dictionary<int, bool>();
        private Dictionary<string, bool> creatorFoldouts = new Dictionary<string, bool>();
        private Dictionary<string, bool> gotchaFoldouts = new Dictionary<string, bool>();
        private Dictionary<string, bool> userNoteFoldouts = new Dictionary<string, bool>();
        private bool foldoutGlobalNotes = true;
        private bool foldoutTranscript = false;

        // Custom GUI Styles
        private GUIStyle headerStyle;
        private GUIStyle subHeaderStyle;
        private GUIStyle categoryHeaderStyle;
        private GUIStyle stepTitleStyle;
        private GUIStyle creatorTitleStyle;
        private GUIStyle selectableBodyStyle;
        private GUIStyle selectableGotchaStyle;
        private GUIStyle linkButtonStyle;
        private GUIStyle timeStampButtonStyle;
        private GUIStyle doneButtonStyle;
        private GUIStyle markButtonStyle;

        [MenuItem("Tools/Generic Workflow Assistant Window")]
        [MenuItem("Window/Generic Workflow Assistant Window")]
        public static void ShowWindow()
        {
            GenericWorkflowWindow window = GetWindow<GenericWorkflowWindow>("Generic Workflow Assistant");
            window.minSize = new Vector2(650, 750);
            window.Show();
        }

        private void InitStyles()
        {
            if (headerStyle == null)
            {
                headerStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 18,
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = new Color(0.2f, 0.75f, 1.0f) }
                };
            }

            if (subHeaderStyle == null)
            {
                subHeaderStyle = new GUIStyle(EditorStyles.textField)
                {
                    fontSize = 11,
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true,
                    normal = { textColor = Color.gray }
                };
            }

            if (categoryHeaderStyle == null)
            {
                categoryHeaderStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 13
                };
            }

            if (stepTitleStyle == null)
            {
                stepTitleStyle = new GUIStyle(EditorStyles.textField)
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 12,
                    normal = { textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black }
                };
            }

            if (creatorTitleStyle == null)
            {
                creatorTitleStyle = new GUIStyle(EditorStyles.textField)
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 11,
                    normal = { textColor = new Color(1.0f, 0.65f, 0.2f) }
                };
            }

            if (selectableBodyStyle == null)
            {
                selectableBodyStyle = new GUIStyle(EditorStyles.textField)
                {
                    fontSize = 11,
                    wordWrap = true,
                    normal = { textColor = EditorGUIUtility.isProSkin ? new Color(0.9f, 0.9f, 0.9f) : Color.black }
                };
            }

            if (selectableGotchaStyle == null)
            {
                selectableGotchaStyle = new GUIStyle(EditorStyles.textField)
                {
                    fontSize = 11,
                    wordWrap = true,
                    normal = { textColor = new Color(0.3f, 0.85f, 1.0f) }
                };
            }

            if (linkButtonStyle == null)
            {
                linkButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 10,
                    fontStyle = FontStyle.Bold,
                    fixedHeight = 22,
                    margin = new RectOffset(2, 2, 2, 2)
                };
            }

            if (timeStampButtonStyle == null)
            {
                timeStampButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 10,
                    fontStyle = FontStyle.Bold,
                    fixedHeight = 20,
                    fixedWidth = 65,
                    normal = { textColor = new Color(0.3f, 0.9f, 0.4f) }
                };
            }

            if (doneButtonStyle == null)
            {
                doneButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 10,
                    fontStyle = FontStyle.Bold,
                    fixedHeight = 20,
                    fixedWidth = 60,
                    normal = { textColor = new Color(0.2f, 0.9f, 0.3f) }
                };
            }

            if (markButtonStyle == null)
            {
                markButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 10,
                    fixedHeight = 20,
                    fixedWidth = 60,
                    normal = { textColor = Color.gray }
                };
            }
        }

        private void OnGUI()
        {
            InitStyles();

            EditorGUILayout.Space(10);
            DrawAssetSelector();
            EditorGUILayout.Space(10);

            if (activeGuide == null)
            {
                DrawEmptyStateAndFormatTemplate();
                return;
            }

            DrawHeader();
            EditorGUILayout.Space(10);

            DrawResourceToolbar();
            EditorGUILayout.Space(10);

            DrawProgressBar();
            EditorGUILayout.Space(10);

            DrawSearchAndFilter();
            EditorGUILayout.Space(10);

            DrawGlobalNotesSection();
            EditorGUILayout.Space(10);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            foreach (var phase in activeGuide.phases)
            {
                if (!phaseFoldouts.ContainsKey(phase.phaseNumber)) phaseFoldouts[phase.phaseNumber] = true;
                bool foldoutState = phaseFoldouts[phase.phaseNumber];

                DrawPhaseSection(phase, ref foldoutState);
                phaseFoldouts[phase.phaseNumber] = foldoutState;
                EditorGUILayout.Space(10);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space(10);
        }

        private void DrawAssetSelector()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Active Workflow Guide:", GUILayout.Width(150));
            activeGuide = (WorkflowGuideAsset)EditorGUILayout.ObjectField(activeGuide, typeof(WorkflowGuideAsset), false);

            if (GUILayout.Button("Import New (Raw Text)", GUILayout.Width(150)))
            {
                WorkflowTextImporterWindow.ShowWindow();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawEmptyStateAndFormatTemplate()
        {
            EditorGUILayout.HelpBox(
                "Please assign a Workflow Guide Asset above or create a new one using Tools -> Workflow Raw Text Importer!",
                MessageType.Info
            );

            EditorGUILayout.Space(10);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("AI Prompt & Tutorial Analysis Template (Select & Ctrl+C to Copy for AI):", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Highlight and copy (Ctrl+C) the prompt template below to paste into AI chat when analyzing YouTube tutorials or notes:",
                MessageType.None
            );

            EditorGUILayout.TextArea(aiPromptTemplate, EditorStyles.textField, GUILayout.Height(280), GUILayout.ExpandWidth(true));

            EditorGUILayout.Space(10);
            if (GUILayout.Button("⚡ Open Workflow Raw Text Importer Window", GUILayout.Height(35)))
            {
                WorkflowTextImporterWindow.ShowWindow();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawHeader()
        {
            EditorGUILayout.LabelField(activeGuide.guideTitle, headerStyle);
            DrawSelectableField(activeGuide.guideSubtitle, subHeaderStyle, false);
        }

        private void DrawResourceToolbar()
        {
            if (activeGuide.resourceLinks == null || activeGuide.resourceLinks.Count == 0) return;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Primary Resources & Video Links", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            foreach (var res in activeGuide.resourceLinks)
            {
                if (GUILayout.Button(res.label, linkButtonStyle))
                {
                    Application.OpenURL(res.url);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawProgressBar()
        {
            int totalSteps = activeGuide.steps.Count;
            int completedSteps = 0;
            int totalTimestamps = 0;
            int completedTimestamps = 0;

            foreach (var step in activeGuide.steps)
            {
                string key = activeGuide.uniqueGuideId + "_" + step.id;
                if (EditorPrefs.GetBool(key, false)) completedSteps++;

                if (step.creatorBreakdowns != null)
                {
                    foreach (var creator in step.creatorBreakdowns)
                    {
                        if (creator.timestamps != null)
                        {
                            foreach (var ts in creator.timestamps)
                            {
                                totalTimestamps++;
                                string tsKey = $"{activeGuide.uniqueGuideId}_{step.id}_{creator.creatorName}_{ts.timeLabel}";
                                if (EditorPrefs.GetBool(tsKey, false)) completedTimestamps++;
                            }
                        }
                    }
                }
            }

            float stepProgress = totalSteps > 0 ? (float)completedSteps / totalSteps : 0f;
            float tsProgress = totalTimestamps > 0 ? (float)completedTimestamps / totalTimestamps : 0f;
            float overallProgress = totalTimestamps > 0 ? (stepProgress + tsProgress) / 2f : stepProgress;

            string progressText = $"{completedSteps}/{totalSteps} Steps | {completedTimestamps}/{totalTimestamps} Timestamps ({Mathf.RoundToInt(overallProgress * 100)}%)";

            EditorGUILayout.BeginHorizontal();
            Rect rect = EditorGUILayout.GetControlRect(false, 20);
            EditorGUI.ProgressBar(rect, overallProgress, progressText);

            if (GUILayout.Button("Reset Progress", GUILayout.Width(100), GUILayout.Height(20)))
            {
                if (EditorUtility.DisplayDialog("Reset Progress", "Uncheck all completed steps and timestamps for this guide?", "Yes", "Cancel"))
                {
                    foreach (var step in activeGuide.steps)
                    {
                        string key = activeGuide.uniqueGuideId + "_" + step.id;
                        EditorPrefs.DeleteKey(key);

                        if (step.creatorBreakdowns != null)
                        {
                            foreach (var creator in step.creatorBreakdowns)
                            {
                                if (creator.timestamps != null)
                                {
                                    foreach (var ts in creator.timestamps)
                                    {
                                        string tsKey = $"{activeGuide.uniqueGuideId}_{step.id}_{creator.creatorName}_{ts.timeLabel}";
                                        EditorPrefs.DeleteKey(tsKey);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSearchAndFilter()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Search:", GUILayout.Width(60));
            searchFilter = EditorGUILayout.TextField(searchFilter);

            if (GUILayout.Button("Clear", GUILayout.Width(50))) searchFilter = "";
            EditorGUILayout.EndHorizontal();
        }

        private void DrawGlobalNotesSection()
        {
            if (activeGuide == null) return;
            string globalNotesKey = $"{activeGuide.uniqueGuideId}_GlobalNotes";
            string currentNotes = EditorPrefs.GetString(globalNotesKey, "");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            foldoutGlobalNotes = EditorGUILayout.Foldout(
                foldoutGlobalNotes,
                string.IsNullOrEmpty(currentNotes) ? "📝 Global Guide Notes & Troubleshooting Log (Click to Add Notes)" : "📝 Global Guide Notes & Troubleshooting Log (Active Notes Saved)",
                true,
                EditorStyles.foldoutHeader
            );

            if (foldoutGlobalNotes)
            {
                EditorGUILayout.HelpBox("Type your custom project notes or troubleshooting fixes below. Notes are saved automatically!", MessageType.None);
                
                string newNotes = EditorGUILayout.TextArea(currentNotes, GUILayout.Height(70), GUILayout.ExpandWidth(true));
                if (newNotes != currentNotes)
                {
                    EditorPrefs.SetString(globalNotesKey, newNotes);
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            foldoutTranscript = EditorGUILayout.Foldout(
                foldoutTranscript,
                "📜 Original AI Transcript / Source Text",
                true,
                EditorStyles.foldoutHeader
            );

            if (foldoutTranscript)
            {
                EditorGUI.BeginChangeCheck();
                string newTranscript = EditorGUILayout.TextArea(activeGuide.rawTranscript ?? "", GUILayout.MinHeight(100), GUILayout.MaxHeight(300), GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(activeGuide, "Modify Transcript");
                    activeGuide.rawTranscript = newTranscript;
                    EditorUtility.SetDirty(activeGuide);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawPhaseSection(WorkflowPhaseData phase, ref bool foldoutState)
        {
            List<WorkflowStepData> steps = activeGuide.GetStepsByPhase(phase.phaseNumber);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            foldoutState = EditorGUILayout.Foldout(foldoutState, phase.phaseTitle, true, categoryHeaderStyle);

            if (foldoutState)
            {
                EditorGUI.indentLevel++;
                foreach (var step in steps)
                {
                    if (ShouldDisplayStep(step)) DrawStepItem(step);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        private bool ShouldDisplayStep(WorkflowStepData step)
        {
            if (string.IsNullOrEmpty(searchFilter)) return true;
            string query = searchFilter.ToLower();

            // Search in global notes
            if (activeGuide != null)
            {
                string globalNotes = EditorPrefs.GetString($"{activeGuide.uniqueGuideId}_GlobalNotes", "").ToLower();
                if (globalNotes.Contains(query)) return true;

                // Search in step user notes
                string userNote = EditorPrefs.GetString($"{activeGuide.uniqueGuideId}_{step.id}_UserNotes", "").ToLower();
                if (userNote.Contains(query)) return true;
            }

            if (step.stepTitle.ToLower().Contains(query) ||
                step.description.ToLower().Contains(query) ||
                step.gotchas.ToLower().Contains(query)) return true;

            foreach (var creator in step.creatorBreakdowns)
            {
                if (creator.creatorName.ToLower().Contains(query)) return true;
                foreach (var ts in creator.timestamps)
                {
                    if (ts.description.ToLower().Contains(query)) return true;
                }
            }

            return false;
        }

        private void DrawStepItem(WorkflowStepData step)
        {
            string key = activeGuide.uniqueGuideId + "_" + step.id;
            bool currentStatus = EditorPrefs.GetBool(key, false);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.BeginHorizontal();

            // Main Step Toggle
            EditorGUI.BeginChangeCheck();
            bool newStatus = EditorGUILayout.Toggle(currentStatus, GUILayout.Width(20));
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(key, newStatus);
                GUI.FocusControl(null); // Clear focus to prevent state caching
                Repaint();
            }

            DrawSelectableField(step.stepTitle, stepTitleStyle, false);
            EditorGUILayout.EndHorizontal();

            DrawSelectableField(step.description, selectableBodyStyle, true);

            // Creator Breakdown
            if (step.creatorBreakdowns != null && step.creatorBreakdowns.Count > 0)
            {
                if (!creatorFoldouts.ContainsKey(step.id)) creatorFoldouts[step.id] = true;
                bool creatorExpanded = creatorFoldouts[step.id];

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                creatorExpanded = EditorGUILayout.Foldout(creatorExpanded, $"Creator Coverage ({step.creatorBreakdowns.Count} Reference Links)", true);
                creatorFoldouts[step.id] = creatorExpanded;

                if (creatorExpanded)
                {
                    EditorGUI.indentLevel++;
                    foreach (var creator in step.creatorBreakdowns)
                    {
                        EditorGUILayout.BeginVertical(GUI.skin.box);
                        EditorGUILayout.BeginHorizontal();
                        
                        DrawSelectableField($"{creator.creatorName} -- {creator.videoTitle}", creatorTitleStyle, false);

                        if (!string.IsNullOrEmpty(creator.githubUrl))
                        {
                            if (GUILayout.Button("📁 Repo", linkButtonStyle, GUILayout.Width(80))) Application.OpenURL(creator.githubUrl);
                        }
                        EditorGUILayout.EndHorizontal();

                        foreach (var ts in creator.timestamps)
                        {
                            EditorGUILayout.BeginHorizontal();

                            // 1. PLAY TIMESTAMP BUTTON FIRST
                            if (GUILayout.Button($"▶ {ts.timeLabel}", timeStampButtonStyle))
                            {
                                Application.OpenURL(WorkflowTextImporterWindow.SanitizeYouTubeUrl(ts.url));
                            }

                            // 2. BULLETPROOF TOGGLE BUTTON SECOND (AFTER PLAY BUTTON TO MARK AS VIEWED)
                            string tsKey = $"{activeGuide.uniqueGuideId}_{step.id}_{creator.creatorName}_{ts.timeLabel}";
                            bool tsStatus = EditorPrefs.GetBool(tsKey, false);

                            GUIContent btnContent = tsStatus ? new GUIContent("✓ Done", "Click to mark as incomplete") : new GUIContent("◯ Mark", "Click to mark as viewed/done");
                            GUIStyle btnStyle = tsStatus ? doneButtonStyle : markButtonStyle;

                            if (GUILayout.Button(btnContent, btnStyle))
                            {
                                EditorPrefs.SetBool(tsKey, !tsStatus);
                                GUI.FocusControl(null); // Clear focus so toggle state updates immediately
                                Repaint();
                            }

                            DrawSelectableField(ts.description, selectableBodyStyle, true);
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
            }

            // Gotchas
            if (!string.IsNullOrEmpty(step.gotchas))
            {
                if (!gotchaFoldouts.ContainsKey(step.id)) gotchaFoldouts[step.id] = false;
                bool gotchaExpanded = gotchaFoldouts[step.id];

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                gotchaExpanded = EditorGUILayout.Foldout(gotchaExpanded, "Gotchas & Important Warnings", true);
                gotchaFoldouts[step.id] = gotchaExpanded;

                if (gotchaExpanded) DrawSelectableField(step.gotchas, selectableGotchaStyle, true);
                EditorGUILayout.EndVertical();
            }

            // STEP-LEVEL USER CUSTOM NOTES SECTION
            string userNoteKey = $"{activeGuide.uniqueGuideId}_{step.id}_UserNotes";
            string currentStepNote = EditorPrefs.GetString(userNoteKey, "");

            if (!userNoteFoldouts.ContainsKey(step.id)) userNoteFoldouts[step.id] = false;
            bool userNoteExpanded = userNoteFoldouts[step.id];

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            userNoteExpanded = EditorGUILayout.Foldout(
                userNoteExpanded,
                string.IsNullOrEmpty(currentStepNote) ? "✏️ My Notes for This Step (Click to Add Notes / Troubleshooting)" : "✏️ My Notes for This Step (Saved Notes Active)",
                true
            );
            userNoteFoldouts[step.id] = userNoteExpanded;

            if (userNoteExpanded)
            {
                string newStepNote = EditorGUILayout.TextArea(currentStepNote, GUILayout.Height(50), GUILayout.ExpandWidth(true));
                if (newStepNote != currentStepNote)
                {
                    EditorPrefs.SetString(userNoteKey, newStepNote);
                }

                EditorGUILayout.Space(5);
                string linkKey = $"{activeGuide.uniqueGuideId}_{step.id}_CustomLink";
                string currentLink = EditorPrefs.GetString(linkKey, "");

                EditorGUILayout.BeginHorizontal();
                string newLink = EditorGUILayout.TextField("🔗 Link (Google Doc/URL):", currentLink);
                if (newLink != currentLink) EditorPrefs.SetString(linkKey, newLink);

                if (!string.IsNullOrEmpty(newLink))
                {
                    if (GUILayout.Button("Open Link", GUILayout.Width(80)))
                    {
                        Application.OpenURL(newLink);
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(5);
                string imgKey = $"{activeGuide.uniqueGuideId}_{step.id}_ImagePath";
                string currentImgPath = EditorPrefs.GetString(imgKey, "");
                Texture2D loadedImg = null;

                if (!string.IsNullOrEmpty(currentImgPath))
                {
                    loadedImg = AssetDatabase.LoadAssetAtPath<Texture2D>(currentImgPath);
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("🖼️ Reference Image:", GUILayout.Width(130));
                Texture2D newImg = (Texture2D)EditorGUILayout.ObjectField(loadedImg, typeof(Texture2D), false);
                EditorGUILayout.EndHorizontal();

                if (newImg != loadedImg)
                {
                    if (newImg == null)
                    {
                        EditorPrefs.SetString(imgKey, "");
                    }
                    else
                    {
                        string newPath = AssetDatabase.GetAssetPath(newImg);
                        EditorPrefs.SetString(imgKey, newPath);
                    }
                }

                if (newImg != null)
                {
                    EditorGUILayout.Space(5);
                    float safeHeight = Mathf.Max(1f, newImg.height);
                    float aspect = (float)newImg.width / safeHeight;
                    float displayWidth = Mathf.Max(1f, position.width - 150); // Account for sidebar and padding
                    float displayHeight = displayWidth / aspect;

                    if (displayHeight > 250) // Limit max height for thumbnails
                    {
                        displayHeight = 250;
                        displayWidth = displayHeight * aspect;
                    }

                    Rect rect = GUILayoutUtility.GetRect(displayWidth, displayHeight);
                    GUI.DrawTexture(rect, newImg, ScaleMode.ScaleToFit);
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);
        }

        private void DrawSelectableField(string text, GUIStyle style, bool multiline = false)
        {
            if (string.IsNullOrEmpty(text)) return;
            float width = EditorGUIUtility.currentViewWidth - 160;
            if (width < 200) width = 200;
            float height = style.CalcHeight(new GUIContent(text), width) + 6;

            if (multiline)
                EditorGUILayout.TextArea(text, style, GUILayout.Height(Mathf.Max(24, height)), GUILayout.ExpandWidth(true));
            else
                EditorGUILayout.TextField(text, style, GUILayout.Height(22), GUILayout.ExpandWidth(true));
        }
    }
}
#endif

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRArcheryWorkflow.Editor
{
    /// <summary>
    /// VR Archery & Hands Master Workflow Assistant
    /// Structured strictly according to tutorial flow:
    /// - Section 1: Blank Project Setup, Packages, Hands & Grabbables (GameDev Blueprint - ofjPCrh0ZIk)
    /// - Section 2: Bow Mechanics & String Interaction (Sunny Valley Studio Parts 1 & 2)
    /// - Section 3: Arrow Visualization & Bow Strength Math (Sunny Valley Studio Part 3)
    /// - Section 4: Arrow Projectile Physics & Target Sticking (Sunny Valley Studio Parts 4 & 5)
    /// - Section 5: Quiver Sockets & Arrow Auto-Spawning (Miniieee - sxyspcd6zO8)
    /// - Section 6: Meta Quest Controller Face Buttons & 3D Audio Systems (SpatialXR - 43ZZfKAOPzk & Sunny Valley Part 5)
    /// </summary>
    public class VRArcheryWorkflowWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private string searchFilter = "";

        // Category Foldout States
        private bool foldoutSec1 = true;
        private bool foldoutSec2 = true;
        private bool foldoutSec3 = true;
        private bool foldoutSec4 = true;
        private bool foldoutSec5 = true;
        private bool foldoutSec6 = true;

        // Global Notes Foldout State
        private bool foldoutGlobalNotes = true;

        // Expandable Foldout States
        private Dictionary<string, bool> creatorFoldouts = new Dictionary<string, bool>();
        private Dictionary<string, bool> gotchaFoldouts = new Dictionary<string, bool>();
        private Dictionary<string, bool> userNoteFoldouts = new Dictionary<string, bool>();

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

        [MenuItem("Tools/VR Archery Master Workflow")]
        [MenuItem("Window/VR Archery Master Workflow")]
        public static void ShowWindow()
        {
            VRArcheryWorkflowWindow window = GetWindow<VRArcheryWorkflowWindow>("VR Archery Master Workflow");
            window.minSize = new Vector2(650, 750);
            window.Show();
        }

        private void OnEnable()
        {
            foreach (var step in masterWorkflowSteps)
            {
                if (!creatorFoldouts.ContainsKey(step.id)) creatorFoldouts[step.id] = true;
                if (!gotchaFoldouts.ContainsKey(step.id)) gotchaFoldouts[step.id] = false;
                if (!userNoteFoldouts.ContainsKey(step.id)) userNoteFoldouts[step.id] = false;
            }
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
                    normal = { textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black },
                    focused = { textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black }
                };
            }

            if (creatorTitleStyle == null)
            {
                creatorTitleStyle = new GUIStyle(EditorStyles.textField)
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 11,
                    normal = { textColor = new Color(1.0f, 0.65f, 0.2f) },
                    focused = { textColor = new Color(1.0f, 0.65f, 0.2f) }
                };
            }

            if (selectableBodyStyle == null)
            {
                selectableBodyStyle = new GUIStyle(EditorStyles.textField)
                {
                    fontSize = 11,
                    wordWrap = true,
                    normal = { textColor = EditorGUIUtility.isProSkin ? new Color(0.9f, 0.9f, 0.9f) : Color.black },
                    focused = { textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black }
                };
            }

            if (selectableGotchaStyle == null)
            {
                selectableGotchaStyle = new GUIStyle(EditorStyles.textField)
                {
                    fontSize = 11,
                    wordWrap = true,
                    normal = { textColor = new Color(0.3f, 0.85f, 1.0f) },
                    focused = { textColor = Color.cyan }
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

        private void DrawSelectableField(string text, GUIStyle style, bool multiline = false)
        {
            if (string.IsNullOrEmpty(text)) return;
            
            float width = EditorGUIUtility.currentViewWidth - 160;
            if (width < 200) width = 200;
            float height = style.CalcHeight(new GUIContent(text), width) + 6;

            if (multiline)
            {
                EditorGUILayout.TextArea(text, style, GUILayout.Height(Mathf.Max(24, height)), GUILayout.ExpandWidth(true));
            }
            else
            {
                EditorGUILayout.TextField(text, style, GUILayout.Height(22), GUILayout.ExpandWidth(true));
            }
        }

        private void OnGUI()
        {
            InitStyles();

            EditorGUILayout.Space(10);
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

            // Section 1
            DrawPhaseFoldout(
                "Section 1: Blank Project Setup, XRIT 3.0 Packages & Hand Animations (GameDev Blueprint)",
                ref foldoutSec1,
                GetStepsByPhase(1)
            );

            EditorGUILayout.Space(10);

            // Section 2
            DrawPhaseFoldout(
                "Section 2: VR Bow Mechanics, Handle Setup & String Constraints (Sunny Valley Studio P1 & P2)",
                ref foldoutSec2,
                GetStepsByPhase(2)
            );

            EditorGUILayout.Space(10);

            // Section 3
            DrawPhaseFoldout(
                "Section 3: Bow Strength Remapping Math & Nocked Arrow Visualization (Sunny Valley Studio P3)",
                ref foldoutSec3,
                GetStepsByPhase(3)
            );

            EditorGUILayout.Space(10);

            // Section 4
            DrawPhaseFoldout(
                "Section 4: Arrow Projectile Physics, Flight Curves & Target Sticking (Sunny Valley Studio P4 & P5)",
                ref foldoutSec4,
                GetStepsByPhase(4)
            );

            EditorGUILayout.Space(10);

            // Section 5
            DrawPhaseFoldout(
                "Section 5: VR Sockets, Back Quiver & Auto-Spawning Arrows (Miniieee Primary Sockets)",
                ref foldoutSec5,
                GetStepsByPhase(5)
            );

            EditorGUILayout.Space(10);

            // Section 6
            DrawPhaseFoldout(
                "Section 6: Meta Quest Hardware Buttons (SpatialXR) & 3D Pitch Creak Audio (Sunny Valley P5)",
                ref foldoutSec6,
                GetStepsByPhase(6)
            );

            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space(10);
        }

        #region Header & Toolbar

        private void DrawHeader()
        {
            EditorGUILayout.LabelField("VR Archery Master Workflow (Zero to Final)", headerStyle);
            DrawSelectableField(
                "STRICT TUTORIAL FLOW: Click '[Done]' to mark timestamps! Add your own custom notes & troubleshooting fixes below.",
                subHeaderStyle,
                false
            );
        }

        private void DrawResourceToolbar()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Primary Project Repositories & Creator Video Links", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("🎥 GameDev Blueprint Video (ofjPCrh0ZIk)", linkButtonStyle))
            {
                Application.OpenURL("https://www.youtube.com/watch?v=ofjPCrh0ZIk");
            }
            if (GUILayout.Button("🎬 Sunny Valley Studio Full Playlist", linkButtonStyle))
            {
                Application.OpenURL("https://www.youtube.com/watch?v=j1jLkra5DRU&list=PLcRSafycjWFf8ayYlaVYRFbVnoIcgVY3N");
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("⭐ Miniieee VR Sockets Video (sxyspcd6zO8)", linkButtonStyle))
            {
                Application.OpenURL("https://www.youtube.com/watch?v=sxyspcd6zO8");
            }
            if (GUILayout.Button("⭐ SpatialXR Controller Inputs Video (43ZZfKAOPzk)", linkButtonStyle))
            {
                Application.OpenURL("https://www.youtube.com/watch?v=43ZZfKAOPzk");
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("📁 My Starter Fork (NatalieC001)", linkButtonStyle))
            {
                Application.OpenURL("https://github.com/NatalieC001/VRTutorialXRInteractionToolkit3x");
            }
            if (GUILayout.Button("📁 My Archery Fork (NatalieC001)", linkButtonStyle))
            {
                Application.OpenURL("https://github.com/NatalieC001/VR-Archery-in-Unity-2022");
            }
            if (GUILayout.Button("🌐 Original Miniieee Repo", linkButtonStyle))
            {
                Application.OpenURL("https://github.com/Miniieee/VRTutorialXRInteractionToolkit3x");
            }
            if (GUILayout.Button("🌐 Original Sunny Valley Repo", linkButtonStyle))
            {
                Application.OpenURL("https://github.com/SunnyValleyStudio/VR-Archery-in-Unity-2022");
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawProgressBar()
        {
            int totalSteps = masterWorkflowSteps.Count;
            int completedSteps = 0;
            int totalTimestamps = 0;
            int completedTimestamps = 0;

            foreach (var step in masterWorkflowSteps)
            {
                if (EditorPrefs.GetBool(step.id, false)) completedSteps++;

                if (step.creatorBreakdowns != null)
                {
                    foreach (var creator in step.creatorBreakdowns)
                    {
                        if (creator.timestamps != null)
                        {
                            foreach (var ts in creator.timestamps)
                            {
                                totalTimestamps++;
                                string tsKey = $"{step.id}_{creator.creatorName}_{ts.timeLabel}";
                                if (EditorPrefs.GetBool(tsKey, false)) completedTimestamps++;
                            }
                        }
                    }
                }
            }

            float stepProgress = totalSteps > 0 ? (float)completedSteps / totalSteps : 0f;
            float tsProgress = totalTimestamps > 0 ? (float)completedTimestamps / totalTimestamps : 0f;
            float overallProgress = (stepProgress + tsProgress) / 2f;

            string progressText = $"{completedSteps}/{totalSteps} Steps | {completedTimestamps}/{totalTimestamps} Timestamps Checked ({Mathf.RoundToInt(overallProgress * 100)}%)";

            EditorGUILayout.BeginHorizontal();
            Rect rect = EditorGUILayout.GetControlRect(false, 20);
            EditorGUI.ProgressBar(rect, overallProgress, progressText);

            if (GUILayout.Button("Reset Progress", GUILayout.Width(100), GUILayout.Height(20)))
            {
                if (EditorUtility.DisplayDialog(
                    "Reset Archery Workflow Progress",
                    "Are you sure you want to uncheck all completed steps and timestamps?",
                    "Yes, Reset All",
                    "Cancel"))
                {
                    foreach (var step in masterWorkflowSteps)
                    {
                        EditorPrefs.DeleteKey(step.id);
                        if (step.creatorBreakdowns != null)
                        {
                            foreach (var creator in step.creatorBreakdowns)
                            {
                                if (creator.timestamps != null)
                                {
                                    foreach (var ts in creator.timestamps)
                                    {
                                        string tsKey = $"{step.id}_{creator.creatorName}_{ts.timeLabel}";
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

            if (GUILayout.Button("Clear", GUILayout.Width(50)))
            {
                searchFilter = "";
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawGlobalNotesSection()
        {
            string globalNotesKey = "VRArchery_GlobalNotes";
            string currentNotes = EditorPrefs.GetString(globalNotesKey, "");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            foldoutGlobalNotes = EditorGUILayout.Foldout(
                foldoutGlobalNotes,
                string.IsNullOrEmpty(currentNotes) ? "📝 Global Project Notes & Troubleshooting Log (Click to Add Notes)" : "📝 Global Project Notes & Troubleshooting Log (Active Notes Saved)",
                true,
                EditorStyles.foldoutHeader
            );

            if (foldoutGlobalNotes)
            {
                EditorGUILayout.HelpBox("Type your personal project notes or troubleshooting fixes below (e.g., Oculus Link Room OpenXR settings fix). Notes are saved automatically!", MessageType.None);
                
                string newNotes = EditorGUILayout.TextArea(currentNotes, GUILayout.Height(70), GUILayout.ExpandWidth(true));
                if (newNotes != currentNotes)
                {
                    EditorPrefs.SetString(globalNotesKey, newNotes);
                }
            }
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region Phase Foldouts & Step Items

        private void DrawPhaseFoldout(string title, ref bool foldoutState, List<WorkflowStep> steps)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            foldoutState = EditorGUILayout.Foldout(foldoutState, title, true, categoryHeaderStyle);

            if (foldoutState)
            {
                EditorGUI.indentLevel++;
                foreach (var step in steps)
                {
                    if (ShouldDisplayStep(step))
                    {
                        DrawStepItem(step);
                    }
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }

        private bool ShouldDisplayStep(WorkflowStep step)
        {
            if (string.IsNullOrEmpty(searchFilter)) return true;

            string query = searchFilter.ToLower();
            
            // Search in global notes
            string globalNotes = EditorPrefs.GetString("VRArchery_GlobalNotes", "").ToLower();
            if (globalNotes.Contains(query)) return true;

            // Search in step user notes
            string userNote = EditorPrefs.GetString($"{step.id}_UserNotes", "").ToLower();
            if (userNote.Contains(query)) return true;

            if (step.title.ToLower().Contains(query) ||
                step.description.ToLower().Contains(query) ||
                step.gotchas.ToLower().Contains(query)) return true;

            foreach (var creator in step.creatorBreakdowns)
            {
                if (creator.creatorName.ToLower().Contains(query)) return true;
                foreach (var m in creator.timestamps)
                {
                    if (m.description.ToLower().Contains(query)) return true;
                }
            }

            return false;
        }

        private void DrawStepItem(WorkflowStep step)
        {
            bool currentStatus = EditorPrefs.GetBool(step.id, false);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.BeginHorizontal();

            // Main Step Checkbox
            bool newStatus = EditorGUILayout.Toggle(currentStatus, GUILayout.Width(20));
            if (newStatus != currentStatus)
            {
                EditorPrefs.SetBool(step.id, newStatus);
            }

            // Step Title (100% SELECTABLE PLAIN TEXT)
            DrawSelectableField(step.title, stepTitleStyle, false);

            EditorGUILayout.EndHorizontal();

            // Description / Goal (100% SELECTABLE MULTILINE PLAIN TEXT)
            DrawSelectableField(step.description, selectableBodyStyle, true);

            // Per-Creator Breakdown Section
            if (step.creatorBreakdowns != null && step.creatorBreakdowns.Count > 0)
            {
                bool creatorExpanded = creatorFoldouts.ContainsKey(step.id) && creatorFoldouts[step.id];
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                creatorExpanded = EditorGUILayout.Foldout(
                    creatorExpanded,
                    $"Coverage Per Creator ({step.creatorBreakdowns.Count} Creators Reference This Step)",
                    true
                );
                creatorFoldouts[step.id] = creatorExpanded;

                if (creatorExpanded)
                {
                    EditorGUI.indentLevel++;
                    foreach (var creator in step.creatorBreakdowns)
                    {
                        EditorGUILayout.BeginVertical(GUI.skin.box);
                        EditorGUILayout.BeginHorizontal();
                        
                        // Creator Name & Title (100% SELECTABLE PLAIN TEXT)
                        DrawSelectableField($"{creator.creatorName} -- {creator.videoTitle}", creatorTitleStyle, false);

                        if (!string.IsNullOrEmpty(creator.githubUrl))
                        {
                            if (GUILayout.Button("📁 Code (My Fork)", linkButtonStyle, GUILayout.Width(110)))
                            {
                                Application.OpenURL(creator.githubUrl);
                            }
                        }

                        if (!string.IsNullOrEmpty(creator.backupGithubUrl))
                        {
                            if (GUILayout.Button("🌐 Original Repo", linkButtonStyle, GUILayout.Width(100)))
                            {
                                Application.OpenURL(creator.backupGithubUrl);
                            }
                        }

                        EditorGUILayout.EndHorizontal();

                        foreach (var ts in creator.timestamps)
                        {
                            EditorGUILayout.BeginHorizontal();
                            
                            // 1. PLAY TIMESTAMP BUTTON FIRST
                            if (GUILayout.Button($"▶ {ts.timeLabel}", timeStampButtonStyle))
                            {
                                Application.OpenURL(SanitizeYouTubeUrl(ts.url));
                            }

                            // 2. BULLETPROOF TOGGLE BUTTON SECOND (AFTER PLAY BUTTON TO MARK AS VIEWED)
                            string tsKey = $"{step.id}_{creator.creatorName}_{ts.timeLabel}";
                            bool tsStatus = EditorPrefs.GetBool(tsKey, false);
                            
                            GUIContent btnContent = tsStatus ? new GUIContent("✓ Done", "Click to mark as incomplete") : new GUIContent("◯ Mark", "Click to mark as viewed/done");
                            GUIStyle btnStyle = tsStatus ? doneButtonStyle : markButtonStyle;

                            if (GUILayout.Button(btnContent, btnStyle))
                            {
                                EditorPrefs.SetBool(tsKey, !tsStatus);
                                GUI.FocusControl(null); // Clear focus so toggle state updates immediately
                                Repaint();
                            }

                            // 3. TIMESTAMP DESCRIPTION THIRD
                            DrawSelectableField(ts.description, selectableBodyStyle, true);

                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.Space(1);
                        }

                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space(2);
                    }
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndVertical();
            }

            // Gotchas & Missed Steps
            if (!string.IsNullOrEmpty(step.gotchas))
            {
                bool gotchaExpanded = gotchaFoldouts.ContainsKey(step.id) && gotchaFoldouts[step.id];
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                gotchaExpanded = EditorGUILayout.Foldout(
                    gotchaExpanded,
                    "Missed Steps & Hardware Gotchas (Click to Expand)",
                    true
                );
                gotchaFoldouts[step.id] = gotchaExpanded;

                if (gotchaExpanded)
                {
                    // Gotcha Explanation (100% SELECTABLE MULTILINE PLAIN TEXT)
                    DrawSelectableField(step.gotchas, selectableGotchaStyle, true);
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(2);
            }

            // STEP-LEVEL USER CUSTOM NOTES & TROUBLESHOOTING SECTION
            string userNoteKey = $"{step.id}_UserNotes";
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
                string linkKey = $"VRArchery_{step.id}_CustomLink";
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
                string imgKey = $"VRArchery_{step.id}_ImagePath";
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

        private List<WorkflowStep> GetStepsByPhase(int phaseNumber)
        {
            return masterWorkflowSteps.FindAll(s => s.phase == phaseNumber);
        }

        #endregion

        #region Master Workflow Data Structures

        public class TimestampItem
        {
            public string timeLabel;   // e.g. "02:15"
            public string url;         // YouTube URL with exact timestamp parameter
            public string description; // What happens at this second
        }

        public class CreatorBreakdown
        {
            public string creatorName;  // e.g. "Sunny Valley Studio (Part 5)", "GameDev Blueprint", "Miniieee"
            public string videoTitle;   // e.g. "Arrow Sticking to objects - VR Archery in Unity P5"
            public string githubUrl;    // Creator matching GitHub link
            public string backupGithubUrl; // Backup forked repository link
            public List<TimestampItem> timestamps;
        }

        public class WorkflowStep
        {
            public string id;
            public int phase;
            public string title;
            public string description;
            public string gotchas;
            public List<CreatorBreakdown> creatorBreakdowns;
        }

        private readonly List<WorkflowStep> masterWorkflowSteps = new List<WorkflowStep>()
        {
            // SECTION 1: Blank Project Setup, Packages, Hands & Grabbables (GameDev Blueprint)
            new WorkflowStep()
            {
                id = "Sec_1_1",
                phase = 1,
                title = "Step 1.1: Blank Project Setup, Package Manager & XRIT 3.0 Starter Assets",
                description = "Start from a blank project in Unity 6 / 2022. Open Package Manager, install XR Interaction Toolkit 3.0+, import Starter Assets & Fix Project Validation.",
                gotchas = "GameDev Blueprint Package Setup: 1) Open Package Manager (0:26). 2) Import Starter Assets from Samples tab (0:45). 3) Go to Project Settings > XR Plug-in Management & click Fix All in Validation tab (3:22). 4) Set Input Action Manager asset on XR Origin (6:44).",
                creatorBreakdowns = new List<CreatorBreakdown>()
                {
                    new CreatorBreakdown
                    {
                        creatorName = "GameDev Blueprint (Verified Timestamps)",
                        videoTitle = "How to make a VR Game in Unity 6 Under 60m",
                        githubUrl = "https://github.com/NatalieC001/VRTutorialXRInteractionToolkit3x",
                        backupGithubUrl = "https://github.com/Miniieee/VRTutorialXRInteractionToolkit3x",
                        timestamps = new List<TimestampItem>
                        {
                            new TimestampItem { timeLabel = "00:26", url = "https://www.youtube.com/watch?v=ofjPCrh0ZIk&t=26s", description = "(0:26 - 2:50) Opening Package Manager (Window > Package Manager) & installing XR Interaction Toolkit 3.0+" },
                            new TimestampItem { timeLabel = "00:45", url = "https://www.youtube.com/watch?v=ofjPCrh0ZIk&t=45s", description = "(0:45 - 2:50) Package Manager -> XR Interaction Toolkit -> Samples tab -> Import Starter Assets" },
                            new TimestampItem { timeLabel = "03:22", url = "https://www.youtube.com/watch?v=ofjPCrh0ZIk&t=202s", description = "(3:22 - 4:08) Project Settings & Validation: Edit > Project Settings > XR Plug-in Management -> Click Fix All in Project Validation tab" },
                            new TimestampItem { timeLabel = "06:44", url = "https://www.youtube.com/watch?v=ofjPCrh0ZIk&t=404s", description = "(6:44 - 6:57) Input Action Manager: Assigning default Input Action Asset to XR Origin" }
                        }
                    }
                }
            },
            new WorkflowStep()
            {
                id = "Sec_1_2",
                phase = 1,
                title = "Step 1.2: XR Origin Setup & Hand Animation System (Blend Trees & C# Scripting)",
                description = "Spawn XR Origin (VR) into scene. Create Animator Controller with 2D/1D Blend Trees for flex/pinch poses, write C# input controller script, and connect to Hand transforms.",
                gotchas = "GameDev Blueprint Hand Animation Setup: 1) Animator & Blend Tree (13:00-15:31): Create Animator Controller & Blend Tree for Flex/Pinch hand poses. 2) Scripting (15:39-17:51): Write C# script reading grip/trigger floats & setting Animator parameters. 3) Connect Controllers (18:08-19:24): Assign Hand Animator scripts to Left & Right Hand objects under Camera Offset.",
                creatorBreakdowns = new List<CreatorBreakdown>()
                {
                    new CreatorBreakdown
                    {
                        creatorName = "GameDev Blueprint (Verified Timestamps)",
                        videoTitle = "How to make a VR Game in Unity 6 Under 60m",
                        githubUrl = "https://github.com/NatalieC001/VRTutorialXRInteractionToolkit3x",
                        backupGithubUrl = "https://github.com/Miniieee/VRTutorialXRInteractionToolkit3x",
                        timestamps = new List<TimestampItem>
                        {
                            new TimestampItem { timeLabel = "13:00", url = "https://www.youtube.com/watch?v=ofjPCrh0ZIk&t=780s", description = "(13:00 - 15:31) Hand Animation Setup: Creating Animator Controller & Blend Tree for Flex/Pinch hand poses" },
                            new TimestampItem { timeLabel = "15:39", url = "https://www.youtube.com/watch?v=ofjPCrh0ZIk&t=939s", description = "(15:39 - 17:51) Scripting Hand Animations: Writing C# script to read controller input values & drive animator parameters" },
                            new TimestampItem { timeLabel = "18:08", url = "https://www.youtube.com/watch?v=ofjPCrh0ZIk&t=1088s", description = "(18:08 - 19:24) Refining & Connecting Controllers: Assigning Hand Animator scripts to Left & Right Hand objects under Camera Offset" }
                        }
                    }
                }
            },
            new WorkflowStep()
            {
                id = "Sec_1_3",
                phase = 1,
                title = "Step 1.3: Grabbables & Custom Interaction Layer Mask Setup",
                description = "Define custom Interaction Layers (Bow, Arrow, Socket). Apply interaction layer masks to Left/Right hand controllers and grabbable objects.",
                gotchas = "GameDev Blueprint Custom Layers Rule: 1) Edit > Project Settings > XR Interaction Toolkit -> Interaction Layer Mask list. Add names (Bow, Arrow, Socket) (32:55-33:06). 2) Apply to Left/Right Hand Controllers (33:06-33:43). 3) Apply to XR Grab Interactable on Bow/Arrow models so interactors only grab matching layers (34:34-35:31).",
                creatorBreakdowns = new List<CreatorBreakdown>()
                {
                    new CreatorBreakdown
                    {
                        creatorName = "GameDev Blueprint (Verified Timestamps)",
                        videoTitle = "How to make a VR Game in Unity 6 Under 60m",
                        githubUrl = "https://github.com/NatalieC001/VRTutorialXRInteractionToolkit3x",
                        backupGithubUrl = "https://github.com/Miniieee/VRTutorialXRInteractionToolkit3x",
                        timestamps = new List<TimestampItem>
                        {
                            new TimestampItem { timeLabel = "32:55", url = "https://www.youtube.com/watch?v=ofjPCrh0ZIk&t=1975s", description = "(32:55 - 33:06) Creating Interaction Layers: Edit > Project Settings > XR Interaction Toolkit -> Add custom names (Bow, Arrow, Socket)" },
                            new TimestampItem { timeLabel = "33:06", url = "https://www.youtube.com/watch?v=ofjPCrh0ZIk&t=1986s", description = "(33:06 - 33:43) Applying to Interactors: Update Left/Right Hand Controller Interaction Layer Mask component" },
                            new TimestampItem { timeLabel = "34:34", url = "https://www.youtube.com/watch?v=ofjPCrh0ZIk&t=2074s", description = "(34:34 - 35:31) Setting up Interactables: Adding XR Grab Interactable component to 3D objects" }
                        }
                    }
                }
            },

            // SECTION 2: VR Bow Mechanics, Handle Setup & String Constraints (Sunny Valley Studio P1 & P2)
            new WorkflowStep()
            {
                id = "Sec_2_1",
                phase = 2,
                title = "Step 2.1: Bow Model Import, Grab Handle & Colliders Array Gotcha",
                description = "Import Bow 3D model. Add XR Grab Interactable, assign specific main collider to Colliders array, and create 'HandleAttachPoint' child object.",
                gotchas = "Sunny Valley Studio Part 1 Gotchas: 1) COLLIDERS ARRAY (4:00 - 6:42): You MUST manually assign your specific Bow collider into the 'Colliders' array field on XR Grab Interactable! If left empty, it will pick up collision data from child objects. 2) ATTACH TRANSFORM (6:43 - 8:31): Create a child GameObject named 'HandleAttachPoint', adjust its rotation/position, and assign it to the 'Attach Transform' field.",
                creatorBreakdowns = new List<CreatorBreakdown>()
                {
                    new CreatorBreakdown
                    {
                        creatorName = "Sunny Valley Studio (Part 1 -- Primary Mechanics)",
                        videoTitle = "How to add VR Bow in Unity: Archery game essentials - Part 1",
                        githubUrl = "https://github.com/NatalieC001/VR-Archery-in-Unity-2022",
                        backupGithubUrl = "https://github.com/SunnyValleyStudio/VR-Archery-in-Unity-2022",
                        timestamps = new List<TimestampItem>
                        {
                            new TimestampItem { timeLabel = "00:35", url = "https://www.youtube.com/watch?v=j1jLkra5DRU&t=35s", description = "(0:35 - 3:59) Initial project configuration: OpenXR setup & connecting Oculus Quest 2 for controller input" },
                            new TimestampItem { timeLabel = "04:00", url = "https://www.youtube.com/watch?v=j1jLkra5DRU&t=240s", description = "(4:00 - 6:42) Adding XR Grab Interactable & manually assigning main collider to Colliders array (Prevents child object grab bugs)" },
                            new TimestampItem { timeLabel = "06:43", url = "https://www.youtube.com/watch?v=j1jLkra5DRU&t=403s", description = "(6:43 - 8:31) Creating child GameObject 'HandleAttachPoint' and assigning to Attach Transform for perfect hand-to-bow rotation" },
                            new TimestampItem { timeLabel = "08:32", url = "https://www.youtube.com/watch?v=j1jLkra5DRU&t=512s", description = "(08:32 - 13:29) Fine tuning bow model & grabbing behavior" },
                            new TimestampItem { timeLabel = "13:30", url = "https://www.youtube.com/watch?v=j1jLkra5DRU&t=810s", description = "(13:30 - 22:09) Testing & verifying bow handling in VR" }
                        }
                    }
                }
            },
            new WorkflowStep()
            {
                id = "Sec_2_2",
                phase = 2,
                title = "Step 2.2: Bow String Interaction, Rigidbody Kinematics & Pull Constraints",
                description = "Setup interactive String Cube with XR Grab Interactable. Write BowStringController to manage string movement, handle LineRenderer, and restrict pull along local Z-axis.",
                gotchas = "Sunny Valley Studio Part 2 Technical Gotchas: 1) KINEMATIC RIGIDBODY (1:54): String Rigidbody MUST be set to isKinematic = true to prevent gravity drop. 2) UNCHECK THROW ON DETACH (2:11): MUST UNCHECK 'Throw on Detach' in XR Grab Interactable. 3) IS TRIGGER COLLIDER (2:36): Set String Cube BoxCollider to Is Trigger = true. 4) LOCAL CONSTRAINTS (10:26): Restrict movement strictly along local Z-axis.",
                creatorBreakdowns = new List<CreatorBreakdown>()
                {
                    new CreatorBreakdown
                    {
                        creatorName = "Sunny Valley Studio (Part 2 -- Bow String)",
                        videoTitle = "Pulling VR Bow String - VR Archery in Unity P2",
                        githubUrl = "https://github.com/NatalieC001/VR-Archery-in-Unity-2022",
                        backupGithubUrl = "https://github.com/SunnyValleyStudio/VR-Archery-in-Unity-2022",
                        timestamps = new List<TimestampItem>
                        {
                            new TimestampItem { timeLabel = "00:00", url = "https://www.youtube.com/watch?v=_d4e4Rq_T84&t=0s", description = "(0:00) Introduction: Project overview for making the bow string interactive" },
                            new TimestampItem { timeLabel = "00:30", url = "https://www.youtube.com/watch?v=_d4e4Rq_T84&t=30s", description = "(0:30 - 3:36) Grabbing the Bow String: Setting up Cube object on string with XR Grab Interactable" },
                            new TimestampItem { timeLabel = "01:54", url = "https://www.youtube.com/watch?v=_d4e4Rq_T84&t=114s", description = "(1:54) Rigidbody Kinematics Gotcha: Setting Rigidbody to isKinematic = true (prevents string falling due to gravity)" },
                            new TimestampItem { timeLabel = "02:11", url = "https://www.youtube.com/watch?v=_d4e4Rq_T84&t=131s", description = "(2:11) Throwing Settings Gotcha: Unchecking 'Throw on Detach' in XR Grab Interactable component" },
                            new TimestampItem { timeLabel = "02:36", url = "https://www.youtube.com/watch?v=_d4e4Rq_T84&t=156s", description = "(2:36) Collider Triggers Gotcha: Set Box Collider to Is Trigger = true to ensure interaction without physics collision" },
                            new TimestampItem { timeLabel = "03:37", url = "https://www.youtube.com/watch?v=_d4e4Rq_T84&t=217s", description = "(3:37 - 10:25) Scripting the Controller: Creating BowStringController to manage string movement & drive LineRenderer" },
                            new TimestampItem { timeLabel = "10:26", url = "https://www.youtube.com/watch?v=_d4e4Rq_T84&t=626s", description = "(10:26 - 15:29) Implementing Constraints: Restricting string movement along local Z-axis & setting stretch limit" },
                            new TimestampItem { timeLabel = "15:30", url = "https://www.youtube.com/watch?v=_d4e4Rq_T84&t=930s", description = "(15:30) Arrow Nocking: Snapping arrow notch transform to string pull position" }
                        }
                    }
                }
            },

            // SECTION 3: Bow Strength Remapping Math & Nocked Arrow Visualization (Sunny Valley Studio P3)
            new WorkflowStep()
            {
                id = "Sec_3_1",
                phase = 3,
                title = "Step 3.1: Nocked Arrow Visualization on String & Hierarchy Cleanup",
                description = "Add Arrow model as child transform to bow string midpoint visual notch. Toggle visibility on string pull, and disable visualization colliders.",
                gotchas = "Sunny Valley Studio Part 3 Arrow Viz Gotchas: 1) AVOID MOVING PARENTS (2:06): When adjusting arrow alignment, move Arrow mesh child itself, NOT the midpoint visual parent! 2) DISABLE COLLIDERS (2:20): Arrow visualization mesh attached while pulling MUST have its colliders disabled. 3) HIERARCHY CLEANUP (10:15): Midpoint visual MUST be a pure visual transform. Do NOT put XR Grab Interactable on the visual object!",
                creatorBreakdowns = new List<CreatorBreakdown>()
                {
                    new CreatorBreakdown
                    {
                        creatorName = "Sunny Valley Studio (Part 3 -- Arrow Visualization)",
                        videoTitle = "Arrow Visualization - VR Archery in Unity P3",
                        githubUrl = "https://github.com/NatalieC001/VR-Archery-in-Unity-2022",
                        backupGithubUrl = "https://github.com/SunnyValleyStudio/VR-Archery-in-Unity-2022",
                        timestamps = new List<TimestampItem>
                        {
                            new TimestampItem { timeLabel = "00:00", url = "https://www.youtube.com/watch?v=Rg_B-rAQraQ&t=0s", description = "(0:00 - 3:07) Arrow Visualization Setup: Adding arrow model as child to bow string's midpoint visual transform" },
                            new TimestampItem { timeLabel = "02:06", url = "https://www.youtube.com/watch?v=Rg_B-rAQraQ&t=126s", description = "(2:06) Avoid Moving Parents Gotcha: Move Arrow child transform itself, NOT the midpoint parent" },
                            new TimestampItem { timeLabel = "02:20", url = "https://www.youtube.com/watch?v=Rg_B-rAQraQ&t=140s", description = "(2:20) Disable Colliders Gotcha: Disable Colliders on Arrow visualization mesh (colliders reserved for spawned projectile)" },
                            new TimestampItem { timeLabel = "08:05", url = "https://www.youtube.com/watch?v=Rg_B-rAQraQ&t=485s", description = "(8:05 - 9:29) Arrow Controller Logic: Script to toggle visualization mesh on string pull/release events" },
                            new TimestampItem { timeLabel = "10:15", url = "https://www.youtube.com/watch?v=Rg_B-rAQraQ&t=615s", description = "(10:15 - 11:00) Hierarchy Cleanup Gotcha: Midpoint visual must be pure visual. XR Grab Interactable on visual object gets stuck in hand" }
                        }
                    }
                }
            },
            new WorkflowStep()
            {
                id = "Sec_3_2",
                phase = 3,
                title = "Step 3.2: Bow.cs Scripting, Remap() Strength Math & Pull/Release UnityEvents",
                description = "Write Bow.cs script to remap string draw distance into normalized strength ratio (0.0 to 1.0) and invoke onPull and onRelease UnityEvents.",
                gotchas = "Sunny Valley Studio Part 3 Strength Math: Use Remap(float value, float from1, float to1, float from2, float to2) method to convert pull distance along local Z-axis into launch velocity float. Bind UnityEvents in Inspector to trigger Arrow visualization toggle.",
                creatorBreakdowns = new List<CreatorBreakdown>()
                {
                    new CreatorBreakdown
                    {
                        creatorName = "Sunny Valley Studio (Part 3 -- Arrow Visualization)",
                        videoTitle = "Arrow Visualization - VR Archery in Unity P3",
                        githubUrl = "https://github.com/NatalieC001/VR-Archery-in-Unity-2022",
                        backupGithubUrl = "https://github.com/SunnyValleyStudio/VR-Archery-in-Unity-2022",
                        timestamps = new List<TimestampItem>
                        {
                            new TimestampItem { timeLabel = "03:08", url = "https://www.youtube.com/watch?v=Rg_B-rAQraQ&t=188s", description = "(3:08 - 5:48) Scripting Bow Logic: Variables for bow strength & setting up UnityEvents (onPull, onRelease)" },
                            new TimestampItem { timeLabel = "05:49", url = "https://www.youtube.com/watch?v=Rg_B-rAQraQ&t=349s", description = "(5:49 - 8:04) Remapping Strength: Implementing math-based Remap() method to calculate draw strength float (0.0 to 1.0)" },
                            new TimestampItem { timeLabel = "09:30", url = "https://www.youtube.com/watch?v=Rg_B-rAQraQ&t=570s", description = "(9:30 - 10:14) Finalizing Components: Assigning scripts and events in Unity Inspector" }
                        }
                    }
                }
            },

            // SECTION 4: Arrow Projectile Physics, Flight Curves & Target Sticking (Sunny Valley Studio P4 & P5)
            new WorkflowStep()
            {
                id = "Sec_4_1",
                phase = 4,
                title = "Step 4.1: Arrow Prefab Setup, Continuous Dynamic & Layer Collision Matrix",
                description = "Build Arrow Prefab with Rigidbody (Continuous Dynamic). Set pivot at tip, and configure Physics Layer Collision Matrix so Arrow never collides with Bow.",
                gotchas = "Sunny Valley Studio Part 4 Gotchas: 1) PIVOT AT TIP (0:13): Arrow Parent object MUST act as container with pivot at arrow tip for accurate spawning & flight trajectory. 2) CONTINUOUS DYNAMIC (1:02): Set Rigidbody Collision Detection to Continuous Dynamic to prevent fast arrows clipping through thin VR targets. 3) LAYER COLLISION MATRIX (8:10): Put Arrow & Bow on separate physics layers. In Edit > Project Settings > Physics, UNCHECK collision between Arrow and Bow layers.",
                creatorBreakdowns = new List<CreatorBreakdown>()
                {
                    new CreatorBreakdown
                    {
                        creatorName = "Sunny Valley Studio (Part 4 -- Arrow Projectile)",
                        videoTitle = "Arrow projectile - VR Archery in Unity P4",
                        githubUrl = "https://github.com/NatalieC001/VR-Archery-in-Unity-2022",
                        backupGithubUrl = "https://github.com/SunnyValleyStudio/VR-Archery-in-Unity-2022",
                        timestamps = new List<TimestampItem>
                        {
                            new TimestampItem { timeLabel = "00:00", url = "https://www.youtube.com/watch?v=QVd0DE3qVSM&t=0s", description = "(0:00 - 2:39) Arrow Prefab Setup: Configuring Arrow Parent, Rigidbody & Collision settings" },
                            new TimestampItem { timeLabel = "00:13", url = "https://www.youtube.com/watch?v=QVd0DE3qVSM&t=13s", description = "(0:13) Technical Gotcha: Pivot Point Importance! Arrow Parent pivot must be at arrow tip" },
                            new TimestampItem { timeLabel = "01:02", url = "https://www.youtube.com/watch?v=QVd0DE3qVSM&t=62s", description = "(1:02) Technical Gotcha: Setting Rigidbody Collision Detection to Continuous Dynamic (prevents passing through targets)" },
                            new TimestampItem { timeLabel = "02:39", url = "https://www.youtube.com/watch?v=QVd0DE3qVSM&t=159s", description = "(2:39 - 4:10) Arrow Rotation Logic: Implementing script to make arrow tail follow flight trajectory" },
                            new TimestampItem { timeLabel = "03:05", url = "https://www.youtube.com/watch?v=QVd0DE3qVSM&t=185s", description = "(3:05) Forcing Smooth Rotation Gotcha: Using Vector3.Slerp in FixedUpdate to align arrow tail with velocity vector" },
                            new TimestampItem { timeLabel = "04:10", url = "https://www.youtube.com/watch?v=QVd0DE3qVSM&t=250s", description = "(4:10 - 6:30) Arrow Controller Script: Coding instantiation & force propulsion logic for shooting arrow" },
                            new TimestampItem { timeLabel = "06:30", url = "https://www.youtube.com/watch?v=QVd0DE3qVSM&t=390s", description = "(6:30 - 8:40) Hierarchy & Layer Setup: Setting up spawn points & Layer Collision Matrix" },
                            new TimestampItem { timeLabel = "08:10", url = "https://www.youtube.com/watch?v=QVd0DE3qVSM&t=490s", description = "(8:10) Technical Gotcha: Collision Matrix Strategy! Uncheck collision between Arrow and Bow layers in Physics settings" },
                            new TimestampItem { timeLabel = "08:40", url = "https://www.youtube.com/watch?v=QVd0DE3qVSM&t=520s", description = "(8:40 - 10:32) Final Adjustments & Testing: Debugging colliders, disabling placeholders & testing flight" }
                        }
                    }
                }
            },
            new WorkflowStep()
            {
                id = "Sec_4_2",
                phase = 4,
                title = "Step 4.2: Target Sticking (OnCollisionEnter) & Non-Uniform Target Scale Gotcha",
                description = "On collision with target object, instantiate stationary Sticking Arrow prefab, freeze physics, and parent transform to hit target.",
                gotchas = "Sunny Valley Studio Part 5 Target Sticking Insights: 1) STICKING ARROW PREFAB (0:00 - 6:49): Disable tip sphere collider & enable box collider for reliable surface hit detection. OnCollisionEnter: Instantiate stationary arrow prefab at hit position, destroy flying original. 2) NON-UNIFORM SCALE GOTCHA (0:00 - 6:49): If target object has non-uniform scaling (e.g. 1, 2, 0.5), parenting arrow to it directly may cause ugly rotation & scale distortion. 3) GAMEPLAY TESTING (15:16 - 16:48): Verify complete end-to-end bow pickup, string pull, release audio, and target sticking.",
                creatorBreakdowns = new List<CreatorBreakdown>()
                {
                    new CreatorBreakdown
                    {
                        creatorName = "Sunny Valley Studio (Part 5 -- Sticking Arrows)",
                        videoTitle = "Arrow Sticking to objects - VR Archery in Unity P5",
                        githubUrl = "https://github.com/NatalieC001/VR-Archery-in-Unity-2022",
                        backupGithubUrl = "https://github.com/SunnyValleyStudio/VR-Archery-in-Unity-2022",
                        timestamps = new List<TimestampItem>
                        {
                            new TimestampItem { timeLabel = "00:00", url = "https://www.youtube.com/watch?v=xcEbaCs7SeQ&t=0s", description = "(0:00 - 6:49) Creating Sticking Arrows: Arrow Parent setup, BoxCollider tuning & OnCollisionEnter replacement logic" },
                            new TimestampItem { timeLabel = "15:16", url = "https://www.youtube.com/watch?v=xcEbaCs7SeQ&t=916s", description = "(15:16 - 16:48) Gameplay Integration: Testing bow pickup, string pulling with audio & shooting arrows at targets" }
                        }
                    },
                    new CreatorBreakdown
                    {
                        creatorName = "NatalieC001",
                        videoTitle = "ArrowController.cs (Vid 5)",
                        githubUrl = "https://github.com/NatalieC001/VR-Archery-in-Unity-2022/blob/main/Vid%205/ArrowController.cs",
                        timestamps = new List<TimestampItem>
                        {
                            new TimestampItem { timeLabel = "00:00", url = "https://github.com/NatalieC001/VR-Archery-in-Unity-2022/blob/main/Vid%205/ArrowController.cs", description = "Viewing NatalieC001 ArrowController.cs source code file on GitHub" }
                        }
                    }
                }
            },

            // SECTION 5: VR Sockets, Back Quiver & Auto-Spawning Arrows (Miniieee Sockets)
            new WorkflowStep()
            {
                id = "Sec_5_1",
                phase = 5,
                title = "Step 5.1: Back/Shoulder Quiver Socket & Auto-Spawning Arrows (XRSocketInteractor)",
                description = "Add XRSocketInteractor to Quiver attached to XR Main Camera / Shoulder. Restrict interaction layer mask to Arrow only, and auto-spawn arrows on selectExited.",
                gotchas = "Miniieee Socket Physics Rule: Attach Quiver parent transform to XR Main Camera / Head. Set Interaction Layer Mask strictly to 'Arrow' so only arrows snap into the quiver socket. Configure Hover Mesh Material (08:15) and subscribe to selectExited event to auto-spawn new Arrow when pulled (12:45).",
                creatorBreakdowns = new List<CreatorBreakdown>()
                {
                    new CreatorBreakdown
                    {
                        creatorName = "Miniieee (Unity 6 / XRIT 3.x -- Primary Recommended for Sockets)",
                        videoTitle = "VR Sockets and Grabbable Items in Unity 6",
                        githubUrl = "https://github.com/NatalieC001/VRTutorialXRInteractionToolkit3x",
                        backupGithubUrl = "https://github.com/Miniieee/VRTutorialXRInteractionToolkit3x",
                        timestamps = new List<TimestampItem>
                        {
                            new TimestampItem { timeLabel = "01:45", url = "https://www.youtube.com/watch?v=sxyspcd6zO8&t=105s", description = "(1:45) Setting up XR Grab Interactable component on 3D grabbable mesh" },
                            new TimestampItem { timeLabel = "03:20", url = "https://www.youtube.com/watch?v=sxyspcd6zO8&t=200s", description = "(3:20) Configuring Rigidbody physics settings: Continuous Dynamic & Interpolate for smooth VR tracking" },
                            new TimestampItem { timeLabel = "04:50", url = "https://www.youtube.com/watch?v=sxyspcd6zO8&t=290s", description = "(4:50) Adding XRSocketInteractor component & configuring socket physics bounds" },
                            new TimestampItem { timeLabel = "06:40", url = "https://www.youtube.com/watch?v=sxyspcd6zO8&t=400s", description = "(6:40) Setting Interaction Layer Mask to 'Arrow' only" },
                            new TimestampItem { timeLabel = "08:15", url = "https://www.youtube.com/watch?v=sxyspcd6zO8&t=495s", description = "(8:15) Configuring Socket Hover Mesh Material for visual feedback when storing arrows" },
                            new TimestampItem { timeLabel = "12:45", url = "https://www.youtube.com/watch?v=sxyspcd6zO8&t=765s", description = "(12:45) Subscribing to socket selectExited event to auto-spawn new Arrow when pulled" }
                        }
                    }
                }
            },

            // SECTION 6: Meta Quest Hardware Buttons (SpatialXR) & 3D Pitch Creak Audio (Sunny Valley P5)
            new WorkflowStep()
            {
                id = "Sec_6_1",
                phase = 6,
                title = "Step 6.1: Meta Quest Controller Face Buttons (A, B, X, Y) & Haptic Vibration",
                description = "Bind Meta Quest face buttons (A, B, X, Y) using exact hardware paths (10:16-13:35) and send haptic vibration impulses on string tension.",
                gotchas = "SpatialXR Rule (10:16-13:35): To get Meta Quest face button values (A, B, X, Y), AVOID the generic 'Usages' category! 1) Open Input Action binding window -> Find XR Controller section. 2) Select Meta Quest Touch Controller (or Oculus Touch). 3) Expand section to locate specific hardware paths like Primary Button (A/X) or Secondary Button (B/Y).",
                creatorBreakdowns = new List<CreatorBreakdown>()
                {
                    new CreatorBreakdown
                    {
                        creatorName = "SpatialXR (XRIT 3.0 Controller Button Inputs -- Primary Hardware Recommended)",
                        videoTitle = "Part 6.1 - How To Get Controller Button Input",
                        githubUrl = "https://github.com/NatalieC001/VRTutorialXRInteractionToolkit3x",
                        backupGithubUrl = "https://github.com/Miniieee/VRTutorialXRInteractionToolkit3x",
                        timestamps = new List<TimestampItem>
                        {
                            new TimestampItem { timeLabel = "01:15", url = "https://www.youtube.com/watch?v=43ZZfKAOPzk&t=75s", description = "(1:15) Creating InputActionProperty serialized fields in C# script" },
                            new TimestampItem { timeLabel = "03:30", url = "https://www.youtube.com/watch?v=43ZZfKAOPzk&t=210s", description = "(3:30) Binding Trigger (Activate) and Grip (Select) action references" },
                            new TimestampItem { timeLabel = "05:40", url = "https://www.youtube.com/watch?v=43ZZfKAOPzk&t=340s", description = "(5:40) Reading button float values (action.action.ReadValue<float>()) for hand flex pose" },
                            new TimestampItem { timeLabel = "07:15", url = "https://www.youtube.com/watch?v=43ZZfKAOPzk&t=435s", description = "(7:15) Calling SendHapticImpulse(amplitude, duration) on controller reference while pulling string" },
                            new TimestampItem { timeLabel = "10:16", url = "https://www.youtube.com/watch?v=43ZZfKAOPzk&t=616s", description = "(10:16 - 13:35) Meta Quest Face Buttons: Open Input Action binding window -> XR Controller -> Meta Quest Touch Controller" },
                            new TimestampItem { timeLabel = "11:45", url = "https://www.youtube.com/watch?v=43ZZfKAOPzk&t=705s", description = "(11:45) Avoiding 'Usages' category and selecting specific hardware path for Primary Button (A/X) or Secondary Button (B/Y)" },
                            new TimestampItem { timeLabel = "13:35", url = "https://www.youtube.com/watch?v=43ZZfKAOPzk&t=815s", description = "(13:35) Verifying Meta Quest controller hardware button registration in Unity Editor VR Simulator" }
                        }
                    }
                }
            },
            new WorkflowStep()
            {
                id = "Sec_6_2",
                phase = 6,
                title = "Step 6.2: 3D Spatial Audio, Bow Release Sound & Dynamic Pitch Creak System",
                description = "Add 3D AudioSource (Spatial Blend 1.0). Play release sound on shoot, and modulate string creak pitch (1 forward, -1 reverse) on pull.",
                gotchas = "Sunny Valley Studio Part 5 Audio Gotchas: 1) SPATIAL BLEND 3D (6:49): Ensure Spatial Blend is set to 1.0 (3D) for directional VR audio. 2) DYNAMIC PITCH CREAK (9:36): Modulate audioSource.pitch (1.0 for pulling forward, -1.0 for pushing back). If pulled too slowly, pause/unpause to prevent clip audio limit breaking.",
                creatorBreakdowns = new List<CreatorBreakdown>()
                {
                    new CreatorBreakdown
                    {
                        creatorName = "Sunny Valley Studio (Part 5 -- Sticking Arrows & Audio)",
                        videoTitle = "Arrow Sticking to objects - VR Archery in Unity P5",
                        githubUrl = "https://github.com/NatalieC001/VR-Archery-in-Unity-2022",
                        backupGithubUrl = "https://github.com/SunnyValleyStudio/VR-Archery-in-Unity-2022",
                        timestamps = new List<TimestampItem>
                        {
                            new TimestampItem { timeLabel = "06:49", url = "https://www.youtube.com/watch?v=xcEbaCs7SeQ&t=409s", description = "(6:49 - 8:36) Adding Impact & Bow Sounds: Integrating 3D audio sources (Spatial Blend 1.0) for impact & release" },
                            new TimestampItem { timeLabel = "08:36", url = "https://www.youtube.com/watch?v=xcEbaCs7SeQ&t=516s", description = "(08:36 - 09:36) Bow Release Audio: Triggering release sound effect precisely when arrow is released" },
                            new TimestampItem { timeLabel = "09:36", url = "https://www.youtube.com/watch?v=xcEbaCs7SeQ&t=576s", description = "(09:36 - 15:16) Dynamic Bow String Sound: Playing sound incrementally with audioSource.pitch (1 forward, -1 reverse)" }
                        }
                    }
                }
            }
        };

        private static string SanitizeYouTubeUrl(string rawUrl)
        {
            if (string.IsNullOrEmpty(rawUrl)) return "";

            var matches = System.Text.RegularExpressions.Regex.Matches(rawUrl, @"[?&](?:t|start)=([^&]+)");
            if (matches.Count <= 1) return rawUrl;

            string targetTime = matches[matches.Count - 1].Groups[1].Value;
            string cleanBase = System.Text.RegularExpressions.Regex.Replace(rawUrl, @"([?&])(?:t|start)=[^&]*", "");
            cleanBase = cleanBase.Replace("?&", "?");
            cleanBase = System.Text.RegularExpressions.Regex.Replace(cleanBase, @"&{2,}", "&");
            cleanBase = cleanBase.TrimEnd('?', '&');

            string separator = cleanBase.Contains("?") ? "&" : "?";
            return $"{cleanBase}{separator}t={targetTime}";
        }

        #endregion
    }
}
#endif

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DyslexicScholarsWorkflow
{
    public class WorkflowAssistantWindow : EditorWindow
    {
        private WorkflowGuide activeGuide;
        private Vector2 mainScrollPos;
        private bool foldoutTranscript = false;
        private Dictionary<int, bool> sectionFoldouts = new Dictionary<int, bool>();
        private Dictionary<string, bool> userAdditionsFoldouts = new Dictionary<string, bool>();

        // Styling
        private GUIStyle sectionHeaderStyle;
        private GUIStyle stepTitleStyle;
        private GUIStyle bodyStyle;

        [MenuItem("Tools/Dyslexic Scholars/Workflow Assistant")]
        public static void ShowWindow()
        {
            var window = GetWindow<WorkflowAssistantWindow>("Workflow Assistant");
            window.minSize = new Vector2(500, 600);
        }

        private void InitializeStyles()
        {
            if (sectionHeaderStyle == null)
            {
                sectionHeaderStyle = new GUIStyle(EditorStyles.foldoutHeader)
                {
                    fontSize = 14,
                    fontStyle = FontStyle.Bold,
                    margin = new RectOffset(5, 5, 10, 5)
                };

                stepTitleStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 13,
                    wordWrap = true,
                    richText = true
                };

                bodyStyle = new GUIStyle(EditorStyles.label)
                {
                    fontSize = 12,
                    wordWrap = true,
                    richText = true
                };
            }
        }

        private void OnGUI()
        {
            InitializeStyles();

            GUILayout.Space(10);
            GUILayout.Label("🎓 Dyslexic Scholars Workflow Assistant", new GUIStyle(EditorStyles.boldLabel) { fontSize = 16, alignment = TextAnchor.MiddleCenter });
            GUILayout.Space(10);

            EditorGUI.BeginChangeCheck();
            activeGuide = (WorkflowGuide)EditorGUILayout.ObjectField("Active Guide:", activeGuide, typeof(WorkflowGuide), false);
            if (EditorGUI.EndChangeCheck())
            {
                sectionFoldouts.Clear();
                userAdditionsFoldouts.Clear();
            }

            if (activeGuide == null)
            {
                EditorGUILayout.HelpBox("Please assign a Workflow Guide asset above to begin.", MessageType.Info);
                return;
            }

            mainScrollPos = EditorGUILayout.BeginScrollView(mainScrollPos);

            DrawHeader();
            DrawTranscriptViewer();

            GUILayout.Space(15);

            foreach (var section in activeGuide.sections)
            {
                DrawSection(section);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label(activeGuide.guideTitle, new GUIStyle(EditorStyles.boldLabel) { fontSize = 14 });
            if (!string.IsNullOrEmpty(activeGuide.creatorName)) GUILayout.Label($"Created by: {activeGuide.creatorName}");

            if (!string.IsNullOrEmpty(activeGuide.videoUrl))
            {
                if (GUILayout.Button("▶ Watch Full Video", GUILayout.Width(150), GUILayout.Height(25)))
                {
                    Application.OpenURL(activeGuide.videoUrl);
                }
            }

            GUILayout.Space(10);
            GUILayout.Label("Global Notes", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            string newNotes = EditorGUILayout.TextArea(activeGuide.globalNotes ?? "", GUILayout.MinHeight(60));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(activeGuide, "Modify Global Notes");
                activeGuide.globalNotes = newNotes;
                EditorUtility.SetDirty(activeGuide);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawTranscriptViewer()
        {
            if (string.IsNullOrEmpty(activeGuide.rawTranscript)) return;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            foldoutTranscript = EditorGUILayout.Foldout(foldoutTranscript, "📜 Original AI Transcript / Source Text", true, sectionHeaderStyle);

            if (foldoutTranscript)
            {
                EditorGUI.BeginChangeCheck();
                string newTranscript = EditorGUILayout.TextArea(activeGuide.rawTranscript, GUILayout.MinHeight(100), GUILayout.MaxHeight(300));
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(activeGuide, "Modify Transcript");
                    activeGuide.rawTranscript = newTranscript;
                    EditorUtility.SetDirty(activeGuide);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawSection(WorkflowSection section)
        {
            if (!sectionFoldouts.ContainsKey(section.sectionNumber)) sectionFoldouts[section.sectionNumber] = true;

            EditorGUILayout.BeginVertical(GUI.skin.box);
            sectionFoldouts[section.sectionNumber] = EditorGUILayout.Foldout(sectionFoldouts[section.sectionNumber], section.sectionTitle, true, sectionHeaderStyle);

            if (sectionFoldouts[section.sectionNumber])
            {
                EditorGUI.indentLevel++;
                var steps = activeGuide.GetStepsBySection(section.sectionNumber);
                foreach (var step in steps)
                {
                    DrawStep(step);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawStep(WorkflowStep step)
        {
            string statusKey = $"{activeGuide.name}_{step.id}_Status";
            bool isDone = EditorPrefs.GetBool(statusKey, false);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();

            // Checkbox with GUI.FocusControl fix
            EditorGUI.BeginChangeCheck();
            bool newDone = EditorGUILayout.Toggle(isDone, GUILayout.Width(20));
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(statusKey, newDone);
                GUI.FocusControl(null);
                Repaint();
            }

            // Step Title
            EditorGUILayout.LabelField(step.title, stepTitleStyle);
            EditorGUILayout.EndHorizontal();

            // Description & Gotchas
            if (!string.IsNullOrEmpty(step.description)) EditorGUILayout.LabelField(step.description, bodyStyle);
            if (!string.IsNullOrEmpty(step.gotchas))
            {
                GUIStyle gotchaStyle = new GUIStyle(bodyStyle) { normal = { textColor = new Color(0.8f, 0.4f, 0.4f) } };
                EditorGUILayout.LabelField($"<b>Insights:</b> {step.gotchas}", gotchaStyle);
            }

            // Timestamps
            if (step.timestamps.Count > 0)
            {
                GUILayout.Space(5);
                foreach (var ts in step.timestamps)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button($"▶ {ts.timeLabel}", GUILayout.Width(80)))
                    {
                        Application.OpenURL(ts.url);
                    }
                    EditorGUILayout.LabelField(ts.description, bodyStyle);
                    EditorGUILayout.EndHorizontal();
                }
            }

            // User Additions (Notes, Links, Images) baked into the asset
            DrawUserAdditions(step);

            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
        }

        private void DrawUserAdditions(WorkflowStep step)
        {
            if (!userAdditionsFoldouts.ContainsKey(step.id)) userAdditionsFoldouts[step.id] = false;

            GUILayout.Space(10);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            userAdditionsFoldouts[step.id] = EditorGUILayout.Foldout(userAdditionsFoldouts[step.id], "✏️ Personal Notes, Links & Images", true);

            if (userAdditionsFoldouts[step.id])
            {
                EditorGUI.BeginChangeCheck();

                GUILayout.Label("My Notes:", EditorStyles.boldLabel);
                string newNotes = EditorGUILayout.TextArea(step.userNotes ?? "", GUILayout.Height(50));

                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                string newLink = EditorGUILayout.TextField("🔗 Link:", step.customLinkUrl ?? "");
                if (!string.IsNullOrEmpty(newLink) && GUILayout.Button("Open", GUILayout.Width(60)))
                {
                    Application.OpenURL(newLink);
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("🖼️ Image:", GUILayout.Width(130));
                Texture2D newImg = (Texture2D)EditorGUILayout.ObjectField(step.referenceImage, typeof(Texture2D), false);
                EditorGUILayout.EndHorizontal();

                if (newImg != null)
                {
                    float safeHeight = Mathf.Max(1f, newImg.height);
                    float aspect = (float)newImg.width / safeHeight;
                    float displayWidth = Mathf.Max(1f, position.width - 120);
                    float displayHeight = displayWidth / aspect;

                    if (displayHeight > 250)
                    {
                        displayHeight = 250;
                        displayWidth = displayHeight * aspect;
                    }

                    Rect rect = GUILayoutUtility.GetRect(displayWidth, displayHeight);
                    GUI.DrawTexture(rect, newImg, ScaleMode.ScaleToFit);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(activeGuide, "Modify Step Additions");
                    step.userNotes = newNotes;
                    step.customLinkUrl = newLink;
                    step.referenceImage = newImg;
                    EditorUtility.SetDirty(activeGuide);
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}

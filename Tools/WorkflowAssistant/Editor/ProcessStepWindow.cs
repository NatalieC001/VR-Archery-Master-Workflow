using UnityEditor;
using UnityEngine;

public class ProcessStepWindow : EditorWindow
{
    private ProcessStepData data;
    private int selectedStepIndex = -1;
    private Vector2 scrollPosition;
    private Vector2 contentScrollPosition;

    [MenuItem("Window/Custom Tools/Process Step Viewer")]
    public static void ShowWindow()
    {
        GetWindow<ProcessStepWindow>("Process Steps");
    }

    private void OnGUI()
    {
        GUILayout.Label("Process Step Viewer", EditorStyles.boldLabel);

        data = (ProcessStepData)EditorGUILayout.ObjectField("Step Data", data, typeof(ProcessStepData), false);

        if (data == null)
        {
            EditorGUILayout.HelpBox("Please assign a ProcessStepData object to view the steps.", MessageType.Info);
            return;
        }

        if (data.steps == null)
        {
            data.steps = new System.Collections.Generic.List<ProcessStep>();
        }
        
        if (data.steps.Count == 0)
        {
            EditorGUILayout.HelpBox("No steps found. Add a step using the button below.", MessageType.Info);
        }

        EditorGUILayout.BeginHorizontal();

        // Left Sidebar: List of steps
        DrawSidebar();

        // Right Content Area: Details of the selected step
        DrawContentArea();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawSidebar()
    {
        EditorGUILayout.BeginVertical("box", GUILayout.Width(200), GUILayout.ExpandHeight(true));
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < data.steps.Count; i++)
        {
            string stepName = string.IsNullOrEmpty(data.steps[i].stepName) ? $"Step {i + 1}" : data.steps[i].stepName;
            
            // Highlight the selected step
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            if (i == selectedStepIndex)
            {
                buttonStyle.normal.textColor = Color.green;
            }

            if (GUILayout.Button(stepName, buttonStyle))
            {
                selectedStepIndex = i;
            }
        }

        EditorGUILayout.EndScrollView();
        
        if (GUILayout.Button("Add New Step"))
        {
            Undo.RecordObject(data, "Add Process Step");
            data.steps.Add(new ProcessStep { stepName = $"Step {data.steps.Count + 1}" });
            EditorUtility.SetDirty(data);
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawContentArea()
    {
        EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        
        if (selectedStepIndex >= 0 && selectedStepIndex < data.steps.Count)
        {
            ProcessStep selectedStep = data.steps[selectedStepIndex];
            
            contentScrollPosition = EditorGUILayout.BeginScrollView(contentScrollPosition);

            EditorGUI.BeginChangeCheck();

            // Store values in temporaries so we can record Undo properly
            string newStepName = EditorGUILayout.TextField("Step Name", selectedStep.stepName);

            // Notes
            GUILayout.Label("Notes", EditorStyles.boldLabel);
            string newNotes = EditorGUILayout.TextArea(selectedStep.notes, GUILayout.MinHeight(60));

            // Screenshot
            GUILayout.Label("Screenshot", EditorStyles.boldLabel);
            Texture2D newScreenshot = (Texture2D)EditorGUILayout.ObjectField(selectedStep.screenshot, typeof(Texture2D), false, GUILayout.Height(EditorGUIUtility.singleLineHeight));

            // Display Image if it exists
            if (selectedStep.screenshot != null)
            {
                GUILayout.Space(10);
                
                // Calculate aspect ratio to fit the image properly in the window
                float aspect = (float)selectedStep.screenshot.width / selectedStep.screenshot.height;
                float displayWidth = position.width - 240; // Approx window width minus sidebar and padding
                float displayHeight = displayWidth / aspect;

                // Optional limit max height
                if (displayHeight > 400)
                {
                    displayHeight = 400;
                    displayWidth = displayHeight * aspect;
                }

                Rect rect = GUILayoutUtility.GetRect(displayWidth, displayHeight);
                GUI.DrawTexture(rect, selectedStep.screenshot, ScaleMode.ScaleToFit);
            }
            else
            {
                EditorGUILayout.HelpBox("No screenshot assigned for this step.", MessageType.Info);
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(data, "Modify Process Step");
                selectedStep.stepName = newStepName;
                selectedStep.notes = newNotes;
                selectedStep.screenshot = newScreenshot;
                EditorUtility.SetDirty(data);
            }
            
            GUILayout.Space(20);
            if (GUILayout.Button("Delete Step", GUILayout.Width(100)))
            {
                Undo.RecordObject(data, "Delete Process Step");
                data.steps.RemoveAt(selectedStepIndex);
                selectedStepIndex = -1; // Reset selection
                EditorUtility.SetDirty(data);
            }

            EditorGUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Label("Select a step to view details.", EditorStyles.centeredGreyMiniLabel);
        }

        EditorGUILayout.EndVertical();
    }
}

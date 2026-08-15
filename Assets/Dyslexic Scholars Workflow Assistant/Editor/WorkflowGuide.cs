using System;
using System.Collections.Generic;
using UnityEngine;

namespace DyslexicScholarsWorkflow
{
    [Serializable]
    public class WorkflowTimestamp
    {
        public string timeLabel = "00:00";
        public string description = "";
        public string url = "";
    }

    [Serializable]
    public class WorkflowStep
    {
        public string id = Guid.NewGuid().ToString();
        public int sectionNumber = 1;
        public string title = "";

        [TextArea(3, 10)]
        public string description = "";

        [TextArea(2, 5)]
        public string gotchas = "";

        // The exact timestamp breakdown from the video
        public List<WorkflowTimestamp> timestamps = new List<WorkflowTimestamp>();

        // NEW: Permanent User Notes, Links, and Images baked into the asset!
        [Header("User Additions")]
        [TextArea(3, 10)]
        public string userNotes = "";
        public string customLinkUrl = "";
        public Texture2D referenceImage;
    }

    [Serializable]
    public class WorkflowSection
    {
        public int sectionNumber = 1;
        public string sectionTitle = "Section 1: Setup";
    }

    [CreateAssetMenu(fileName = "NewWorkflowGuide", menuName = "Dyslexic Scholars/Workflow Guide", order = 1)]
    public class WorkflowGuide : ScriptableObject
    {
        [Header("Guide Information")]
        public string guideTitle = "New Workflow Guide";
        public string creatorName = "Tutorial Creator";
        public string videoUrl = "";

        [Header("Global Permanent Notes")]
        [TextArea(4, 15)]
        public string globalNotes = "";

        [Header("Original AI Transcript")]
        [TextArea(10, 30)]
        public string rawTranscript = "";

        [Header("Workflow Content")]
        public List<WorkflowSection> sections = new List<WorkflowSection>();
        public List<WorkflowStep> steps = new List<WorkflowStep>();

        public List<WorkflowStep> GetStepsBySection(int sectionNum)
        {
            return steps.FindAll(s => s.sectionNumber == sectionNum);
        }
    }
}

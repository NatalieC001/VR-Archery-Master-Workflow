#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GenericWorkflow.Editor
{
    [Serializable]
    public class TimestampData
    {
        public string timeLabel = "00:00";   // e.g. "02:15"
        public string url = "";             // YouTube URL with exact &t= timestamp
        public string description = "";     // Description of what happens at this timestamp
    }

    [Serializable]
    public class CreatorBreakdownData
    {
        public string creatorName = "";     // e.g. "Sunny Valley Studio", "GameDev Blueprint"
        public string videoTitle = "";      // e.g. "VR Archery Essentials - Part 1"
        public string githubUrl = "";       // Primary repository link
        public string backupGithubUrl = ""; // Secondary/Original repository link
        public List<TimestampData> timestamps = new List<TimestampData>();
    }

    [Serializable]
    public class WorkflowStepData
    {
        public string id = Guid.NewGuid().ToString();
        public int phaseNumber = 1;
        public string stepTitle = "";
        public string description = "";
        public string gotchas = "";
        public List<CreatorBreakdownData> creatorBreakdowns = new List<CreatorBreakdownData>();
    }

    [Serializable]
    public class WorkflowPhaseData
    {
        public int phaseNumber = 1;
        public string phaseTitle = "Phase 1: Setup";
    }

    [Serializable]
    public class ResourceLinkData
    {
        public string label = "Link";
        public string url = "https://";
    }

    /// <summary>
    /// ScriptableObject container storing modular tutorial guide data.
    /// Can be created via Assets -> Create -> Tools -> Workflow Guide Asset.
    /// </summary>
    [CreateAssetMenu(fileName = "NewWorkflowGuideAsset", menuName = "Tools/Workflow Guide Asset", order = 1)]
    public class WorkflowGuideAsset : ScriptableObject
    {
        [Header("Guide Header Info")]
        public string guideTitle = "🎯 VR Tutorial Workflow Guide";
        [TextArea(2, 4)]
        public string guideSubtitle = "Interactive tutorial workflow assistant with timestamps, code snippets, and gotchas.";
        public string uniqueGuideId = "VR_Workflow_Guide_01";

        [Header("Top Toolbar Resources")]
        public List<ResourceLinkData> resourceLinks = new List<ResourceLinkData>();

        [Header("Workflow Phases & Steps")]
        public List<WorkflowPhaseData> phases = new List<WorkflowPhaseData>();
        public List<WorkflowStepData> steps = new List<WorkflowStepData>();

        [Header("Raw Transcript")]
        [TextArea(5, 10)]
        public string rawTranscript = "";

        public List<WorkflowStepData> GetStepsByPhase(int phaseNum)
        {
            return steps.FindAll(s => s.phaseNumber == phaseNum);
        }
    }
}
#endif

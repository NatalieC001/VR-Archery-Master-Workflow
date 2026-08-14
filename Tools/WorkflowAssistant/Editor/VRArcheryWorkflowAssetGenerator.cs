#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GenericWorkflow.Editor
{
    public static class VRArcheryWorkflowAssetGenerator
    {
        [MenuItem("Tools/Generate Sample VR Archery Guide Asset")]
        public static void GenerateSampleAsset()
        {
            WorkflowGuideAsset asset = ScriptableObject.CreateInstance<WorkflowGuideAsset>();
            asset.guideTitle = "🎯 VR Archery Master Workflow Guide";
            asset.guideSubtitle = "Interactive tutorial workflow assistant built with the Generic Workflow Framework.";
            asset.uniqueGuideId = "VR_Archery_Master_Workflow";

            // Add Resource Links
            asset.resourceLinks.Add(new ResourceLinkData { label = "🎥 GameDev Blueprint Video", url = "https://www.youtube.com/watch?v=ofjPCrh0ZIk" });
            asset.resourceLinks.Add(new ResourceLinkData { label = "🎬 Sunny Valley Playlist", url = "https://www.youtube.com/watch?v=j1jLkra5DRU&list=PLcRSafycjWFf8ayYlaVYRFbVnoIcgVY3N" });
            asset.resourceLinks.Add(new ResourceLinkData { label = "⭐ Miniieee VR Sockets Video", url = "https://www.youtube.com/watch?v=sxyspcd6zO8" });
            asset.resourceLinks.Add(new ResourceLinkData { label = "⭐ SpatialXR Controller Inputs Video", url = "https://www.youtube.com/watch?v=43ZZfKAOPzk" });

            // Add Phases
            asset.phases.Add(new WorkflowPhaseData { phaseNumber = 1, phaseTitle = "Section 1: Blank Project Setup, Packages & Hands (🎥 GameDev Blueprint)" });
            asset.phases.Add(new WorkflowPhaseData { phaseNumber = 2, phaseTitle = "Section 2: VR Bow Mechanics & String Interaction (🎬 Sunny Valley P1 & P2)" });
            asset.phases.Add(new WorkflowPhaseData { phaseNumber = 3, phaseTitle = "Section 3: Bow Strength Math & Nock Visualization (🎬 Sunny Valley P3)" });
            asset.phases.Add(new WorkflowPhaseData { phaseNumber = 4, phaseTitle = "Section 4: Arrow Physics & Target Sticking (🎬 Sunny Valley P4 & P5)" });
            asset.phases.Add(new WorkflowPhaseData { phaseNumber = 5, phaseTitle = "Section 5: VR Sockets & Back Quiver (⭐ Miniieee Sockets)" });
            asset.phases.Add(new WorkflowPhaseData { phaseNumber = 6, phaseTitle = "Section 6: Meta Quest Controller Buttons & 3D Audio (⭐ SpatialXR & Sunny Valley)" });

            // Add Step 1.1
            WorkflowStepData step1_1 = new WorkflowStepData
            {
                id = "Sec_1_1",
                phaseNumber = 1,
                stepTitle = "Step 1.1: Blank Project Setup, Package Manager & XRIT 3.0",
                description = "Start from a blank project in Unity 6 / 2022. Open Package Manager, install XR Interaction Toolkit 3.0+, import Starter Assets & Fix Project Validation.",
                gotchas = "1) Package Manager (0:26). 2) Import Starter Assets (0:45). 3) XR Plug-in Management -> Fix All (3:22)."
            };
            step1_1.creatorBreakdowns.Add(new CreatorBreakdownData
            {
                creatorName = "GameDev Blueprint",
                videoTitle = "How to make a VR Game in Unity 6 Under 60m",
                githubUrl = "https://github.com/NatalieC001/VRTutorialXRInteractionToolkit3x",
                timestamps = new List<TimestampData>
                {
                    new TimestampData { timeLabel = "00:26", url = "https://www.youtube.com/watch?v=ofjPCrh0ZIk&t=26s", description = "(0:26) Installing XR Interaction Toolkit 3.0+" },
                    new TimestampData { timeLabel = "00:45", url = "https://www.youtube.com/watch?v=ofjPCrh0ZIk&t=45s", description = "(0:45) Importing Starter Assets" },
                    new TimestampData { timeLabel = "03:22", url = "https://www.youtube.com/watch?v=ofjPCrh0ZIk&t=202s", description = "(3:22) Fixing Project Validation" }
                }
            });
            asset.steps.Add(step1_1);

            // Add Step 2.1
            WorkflowStepData step2_1 = new WorkflowStepData
            {
                id = "Sec_2_1",
                phaseNumber = 2,
                stepTitle = "Step 2.1: Bow Model Import & Colliders Array Gotcha",
                description = "Import Bow 3D model. Add XR Grab Interactable, assign specific main collider to Colliders array, and create 'HandleAttachPoint' child object.",
                gotchas = "COLLIDERS ARRAY (4:00): Manually assign main collider into Colliders array. ATTACH TRANSFORM (6:43): Create HandleAttachPoint child object."
            };
            step2_1.creatorBreakdowns.Add(new CreatorBreakdownData
            {
                creatorName = "Sunny Valley Studio (Part 1)",
                videoTitle = "How to add VR Bow in Unity",
                githubUrl = "https://github.com/NatalieC001/VR-Archery-in-Unity-2022",
                timestamps = new List<TimestampData>
                {
                    new TimestampData { timeLabel = "04:00", url = "https://www.youtube.com/watch?v=j1jLkra5DRU&t=240s", description = "(4:00) Manually assigning main collider to Colliders array" },
                    new TimestampData { timeLabel = "06:43", url = "https://www.youtube.com/watch?v=j1jLkra5DRU&t=403s", description = "(6:43) Setting up HandleAttachPoint for hand rotation" }
                }
            });
            asset.steps.Add(step2_1);

            // Save asset
            string path = "Assets/VRArcheryMasterWorkflow.asset";
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            EditorUtility.DisplayDialog("Generated!", $"Sample VR Archery Asset created at {path}.\nOpen Tools -> Generic Workflow Assistant Window to view!", "OK");
        }
    }
}
#endif

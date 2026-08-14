using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProcessStep
{
    public string stepName = "";
    [TextArea(3, 10)]
    public string notes = "";
    public Texture2D screenshot;
}

[CreateAssetMenu(fileName = "NewProcessStepData", menuName = "Custom Tools/Process Step Data")]
public class ProcessStepData : ScriptableObject
{
    public List<ProcessStep> steps = new List<ProcessStep>();
}

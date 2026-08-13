# 🎯 VR Archery Master Workflow & Universal Generic Workflow Framework for Unity

An interactive, dyslexia-friendly, timestamped **Unity Editor Assistant** built for VR archery mechanics, hand interactions, socket inventories, and XR development. Combined with a **Universal Generic Workflow Engine** that lets you convert any YouTube tutorial transcript or raw notes into a fully interactive Unity workflow window with timestamped links and progress tracking.

---

## 📁 Repository Directory Structure

```
VR-Archery-Master-Workflow/
├── Editor/
│   ├── VRArcheryWorkflowWindow.cs         # Part 1: Dedicated VR Archery Master Workflow Window
│   ├── GenericWorkflowWindow.cs           # Part 2: Universal Generic Workflow Assistant Engine
│   ├── WorkflowGuideAsset.cs              # Data Schema for Custom ScriptableObject Guides
│   ├── WorkflowTextImporterWindow.cs      # Raw Text & Timestamp Auto-Importer Tool (Dyslexia-Friendly)
│   └── VRArcheryWorkflowAssetGenerator.cs # Sample Guide Generator Utility
└── README.md
```

---

## ✨ Features

### 🏹 Part 1: VR Archery Master Workflow (`VRArcheryWorkflowWindow.cs`)
- **6 Comprehensive Sections** covering complete VR archery development from blank project setup to final audio creaks and Quest controller button bindings:
  1. **Blank Project Setup, XRIT 3.0 Packages & Hand Animations** (GameDev Blueprint)
  2. **VR Bow Mechanics, Handle Setup & String Constraints** (Sunny Valley Studio Parts 1 & 2)
  3. **Bow Strength Remapping Math & Nocked Arrow Visualization** (Sunny Valley Studio Part 3)
  4. **Arrow Projectile Physics, Flight Curves & Target Sticking** (Sunny Valley Studio Parts 4 & 5)
  5. **VR Sockets, Back Quiver & Auto-Spawning Arrows** (Miniieee Primary Sockets)
  6. **Meta Quest Hardware Buttons & 3D Pitch Creak Audio** (SpatialXR & Sunny Valley Part 5)
- **100% Selectable Text Fields** for easy copying into AI assistants or custom documentation.
- **Direct YouTube Timestamp Integration** (▶ buttons open exact video seconds with sanitized URLs).
- **Step & Timestamp Checklists** (`✓ Done` / `◯ Mark`) with automatic progress tracking saved in `EditorPrefs`.
- **Missed Steps & Technical Gotchas** foldouts highlighting common unity/VR hardware bugs.
- **Global & Step-Level Personal Notes** for saving custom troubleshooting solutions.

### 🌐 Part 2: Universal Generic Workflow Framework (`GenericWorkflowWindow.cs` & `WorkflowGuideAsset.cs`)
- Data-driven workflow engine powered by Unity `ScriptableObject` assets (`WorkflowGuideAsset`).
- Import and swap custom tutorial guides for any topic (VR, Multiplayer, Shaders, UI).
- Real-time search filter searching step titles, descriptions, gotchas, timestamps, and personal notes.
- Included sample generator (`VRArcheryWorkflowAssetGenerator.cs`) for instant testing.

### 🤖 Part 3: Dyslexia-Friendly AI Text & Timestamp Importer (`WorkflowTextImporterWindow.cs`)
- Built-in **AI Prompt Template** to paste into ChatGPT/Claude to automatically structure YouTube video descriptions or raw notes.
- Auto-parses timestamps (`[MM:SS]`, `HH:MM:SS`, or `MM:SS`) and automatically generates YouTube URLs with `&t=...s` parameters.
- Automatic YouTube URL cleaning and sanitization preventing double timestamp parameter corruption.
- One-click asset generation (`.asset`) saved directly into your project.

---

## 🚀 Installation into your Unity Project

1. Copy the `VR-Archery-Master-Workflow` folder (or just the `Editor/` folder) into your Unity project under `Assets/` (e.g., `Assets/Tools/VR-Archery-Master-Workflow/`).
2. Allow Unity to compile the C# scripts.
3. Access all tools from the top menu bar in Unity:
   - **`Tools > VR Archery Master Workflow`**
   - **`Tools > Generic Workflow Assistant Window`**
   - **`Tools > Workflow Raw Text Importer (Dyslexia-Friendly)`**
   - **`Tools > Generate Sample VR Archery Guide Asset`**

---

## 🎥 Primary Tutorial & Code References

- **GameDev Blueprint**: [How to make a VR Game in Unity 6 Under 60m](https://www.youtube.com/watch?v=ofjPCrh0ZIk)
- **Sunny Valley Studio**: [VR Archery in Unity 2022 Full Playlist](https://www.youtube.com/watch?v=j1jLkra5DRU&list=PLcRSafycjWFf8ayYlaVYRFbVnoIcgVY3N)
- **Miniieee**: [VR Sockets and Grabbable Items in Unity 6](https://www.youtube.com/watch?v=sxyspcd6zO8)
- **SpatialXR**: [How To Get Controller Button Input](https://www.youtube.com/watch?v=43ZZfKAOPzk)
- **NatalieC001 Repositories**:
  - Starter Fork: [VRTutorialXRInteractionToolkit3x](https://github.com/NatalieC001/VRTutorialXRInteractionToolkit3x)
  - Archery Fork: [VR-Archery-in-Unity-2022](https://github.com/NatalieC001/VR-Archery-in-Unity-2022)

---

## 📄 License

MIT License. Free for personal and commercial Unity VR projects!

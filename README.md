# 🎯 VR Archery Master Workflow & Universal Unity Learning Tool Suite

This repository delivers two distinct tools for Unity developers:
1. **The VR Archery & Hands Master Workflow**: A complete, pre-populated interactive workflow for building VR Archery.
2. **The Generic Workflow Engine**: A universal, distraction-free engine to learn *anything* in Unity step-by-step from YouTube.

---

# 🎯 Core Productivity Features & Benefits

- **Personal Notes & Troubleshooting Log**: Dedicated text boxes under every step AND at the top of the window (`📝 Global Project Notes & Troubleshooting Log`) where you can type your own fixes (e.g. Oculus Link Room OpenXR settings fix). Notes save automatically to `EditorPrefs`!
- **Search Integration**: Your custom notes and troubleshooting fixes are fully indexed by the real-time search bar.
- **Granular Timestamp Completion**: Status buttons next to every timestamp (`[ ▶ MM:SS ] [ ◯ Mark ] / [ ✓ Done ]`) so you can watch a video segment and mark it as viewed.
- **Dual-Level Progress Tracking**: Visual progress bar tracking both overall step completion and individual timestamp completion (`% complete`).
- **Stop Wasting Hours on YouTube**: Never spend hours scrubbing through long YouTube videos trying to remember where you left off or getting lost in video recommendations.
- **Persistent Progress**: Your completed steps, timestamp ticks, and custom notes are automatically saved across Unity Editor sessions (`EditorPrefs`).
- **100% Selectable Plain Text**: Highlight and copy (`Ctrl+C`) any step title, note, or gotcha for your own documentation or AI assistance.

---

# 🏹 PART 1: VR Archery & Hands Master Workflow

This tool is a dedicated Unity Editor window built to guide you step-by-step through creating a complete VR Archery game in Unity. It synthesizes and organizes video tutorials from top creators into a single interactive checklist directly inside Unity.

### 🌟 Key Features
- **6 Structured Sections**:
  - **Section 1**: Project Setup, XRIT 3.0 Packages & Hand Animations (*GameDev Blueprint*)
  - **Section 2**: VR Bow Mechanics, Handle Setup & String Pull Constraints (*Sunny Valley Studio P1 & P2*)
  - **Section 3**: Bow Strength Remapping Math & Nocked Arrow Visualization (*Sunny Valley Studio P3*)
  - **Section 4**: Arrow Projectile Physics, Flight Trajectory & Target Sticking (*Sunny Valley Studio P4 & P5*)
  - **Section 5**: Quiver Sockets, Shoulder Attachment & Auto-Spawning Arrows (*Miniieee*)
  - **Section 6**: Meta Quest Hardware Controller Face Buttons & 3D Spatial Audio (*SpatialXR & Sunny Valley*)
- **Clickable Video Timestamps with Status Buttons**: Click `[ ▶ MM:SS ]` to watch and `[ ✓ Done ]` to mark viewed.
- **Personal Notes & Troubleshooting Foldouts**: Add your own notes to any step.
- **Matching GitHub Repos**: Quick access buttons to matching source code repositories for each tutorial.

### 🕹️ How to Open
In Unity, go to: **`Tools ➔ VR Archery Master Workflow`**

---

# ⚙️ PART 2: The Generic Workflow Engine (Learn Anything in Unity!)

Beyond the VR Archery implementation, this repository includes an abstract, reusable tool suite created to save time, eliminate browser tab-switching distractions, and provide a dyslexia-friendly learning environment.

You can extend this tool to **any topic you want to achieve in Unity**—whether it's shaders, multiplayer, procedural generation, UI, or animation!

### 🚀 Key Components

1. **Generic Workflow Assistant Window (`Tools ➔ Generic Workflow Assistant Window`)**:
   - Renders modular `WorkflowGuideAsset` files for any subject.
   - Tracks your learning progress with visual progress bars saved across Unity sessions.
   - Includes real-time keyword search to filter your tutorial notes & personal troubleshooting logs instantly.

2. **Dyslexia-Friendly Raw Text Importer (`Tools ➔ Workflow Raw Text Importer`)**:
   - Automatically converts raw video transcripts or notes into interactive Unity workflow assets with zero manual data entry.

3. **Selectable AI Prompt Template**:
   - Built-in prompt template with generic `[MM:SS]` timestamp placeholders. Highlight, copy (`Ctrl+C`), and paste it into AI chat (ChatGPT, Gemini, Claude) along with any YouTube transcript to format your custom tutorial guide instantly.

---

## 📖 How to Create a Custom Workflow for Any YouTube Tutorial

```
[ YouTube Video / Transcript ] 
            │
            ▼
[ Copy Built-in AI Prompt Template ] ➔ [ Paste into AI Chat ]
            │
            ▼
[ Paste AI Output into Importer Window ] ➔ [ Click "Generate Workflow Guide Asset" ]
            │
            ▼
[ Open in Unity & Learn Step-by-Step! ]
```

1. Open **`Tools ➔ Workflow Raw Text Importer (Dyslexia-Friendly)`**.
2. Select & copy (`Ctrl+C`) the **AI Prompt Template** displayed in the window.
3. Paste the prompt and your YouTube video transcript/description into AI chat.
4. Paste the AI response into the text importer box and click **⚡ Generate Workflow Guide Asset**.
5. Assign your newly created `.asset` file into **`Tools ➔ Generic Workflow Assistant Window`**!

---

# Saving Images and Notes in Unity ScriptableObjects

This repository contains a proof-of-concept (PoC) for an Editor tool in Unity that allows users to save screenshots (images) and text notes for various steps of a process, storing them inside a `ScriptableObject`.

## Approach

In Unity Editor, you can store references to assets (like textures or sprites) inside a `ScriptableObject`. 

### The ScriptableObject (`ProcessStepData.cs`)

We define a `ScriptableObject` that holds a list of "Steps". Each step has:
- A `string` for notes.
- A `Texture2D` for the image/screenshot.

### The Editor Window (`ProcessStepWindow.cs`)

An Editor Window can be created to view, add, edit, and click through these steps.
It will read from the `ScriptableObject` and display the images using `GUILayout.Label(texture)`.

## Limitations & Best Practices

1. **Asset References vs. Raw Image Data:** In Editor, you typically store a reference to an existing `Texture2D` asset (saved in your Project). If you are capturing screenshots on the fly inside the Editor, you must save them to disk (e.g., as `.png` files via `File.WriteAllBytes`), import them into Unity using `AssetDatabase.Refresh()`, and then assign the imported `Texture2D` to your `ScriptableObject`.
2. **Runtime vs Editor:** `ScriptableObject` data changes made at runtime in a built game will *not* be saved. If you intend this to be a tool used by players in a built game, you must save the images to the `Application.persistentDataPath` as `.png` files and save a JSON file referencing the paths and notes. 

This repository focuses on the **Editor** tool approach as requested.

---

## 📜 License
MIT License - Free to use and extend in personal and commercial Unity projects.

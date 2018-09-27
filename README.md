# RuGo
USC CSCI538 Augmented, Virtual, and Mixed Reality course team project.

# Team Members
- [Michael Root](https://github.com/nemosx)
- [Kishore Venkateshan](https://github.com/kv3n)
- [Hsuan-Hau (Howard) Liu](https://github.com/hsuanhauliu)
- [Darwin Mendyke](https://github.com/NiwradMendyke)
- [Abhishek Bhatt](https://github.com/abhatt95)
- [Devashree Shirude](https://github.com/DevaShirude)
- [Sarah Riaz](https://github.com/sriaz08)

--------------------

# Project Overview

## Key Abstractions

### Gadget

Gadgets are the objects from which our Rube Goldberg machine is built with. Each concrete Gadget such as **BoxGadget, DominoGadget, BallGadget** derive from the abstract Gadget class.

The Gadget class manages shared concerns such as transparency, solidification, and toggling between physics modes.

### GameManager

The **GameManager** manages the current mode of the game. It listens for user input to open the **GadgetSelectorMenu**.
The GameManager also uses the GadgetManipulator to interact with objects in the 3D scene.

Currently there are three modes.

* Build Mode - The default mode in which the player can pick gadgets in the world for manipulation

* Select Mode - The mode that is set when the GadgetSelectorMenu is active

* Draw Mode - The mode that is set when Draw/Path tooling is enabled

### GadgetManipulator

The **GadgetManipulator** interacts with gadgets in the world. It has a notion of a selected gadget.
When it has a selected gadget, it will move the selected gadget to the position of the mouse.

The GadgetManipulator also has the ability to insert objects into the **World**


## World

A collection of Gadgets that have been inserted into the scene.


## Adding New Gadgets

**This guide is subject to change and is intended to only give a rough overview**


1. Create a prefab in the resources folder with the name of your gadget.
2. Attach a YourGadget.cs file to your prefab. YourGadget.cs must extend the **Gadget** abstract class.
3. Add the name of your gadget, which matches the name in the resources directory, to the GadgetInventory enum inside Gadget.cs.
4. If your Gadget is composed of multiple GameObjects, implement GetRenderers to return the renderers you want to make transparent when your gadget is selected
--------------------

# Development Guidelines

## Scenes

Each developer has a scene with their usc email id as the name of the scene. Please do all your testing / feature work on this scene and please **do not interfere with the Master** scene. The person doing the integrations will be responsible for collecting all the necessary working bits from each scene and merging onto the Master. This way we can keep a consistent, compilable and working build at the end of each week.

## Scripts

Each developer also should **create a folder inside the global Scripts folder** with their usc email id as the name of their scripts folder. Please insert all your scripts here and work locally on them as much as possible. If there is a need to reuse a script and you feel like you need to edit that script, make sure you communicate that need with the author of that script. This way we can avoid a lot of unnecessary merge conflict issues.

Every week when the integrations are performed, the necessary scripts will be brought into a **Master folder inside the Scripts folder**. This again will help us keep a compilable and working build at the end of each week.

## Prefabs

Similar to Scripts, before creating your prefab ensure that you create a folder with your usc email id as the name of the folder inside the **global Prefabs folder**. Drag and drop your prefabs into this folder and try to keep it as localized as possible. If a change is need in the prefab, please let the author know that you will be making changes to the respective prefab.

Every week when the integrations are performed, the necessary prefabs will be brought into a **Master folder inside the Prefabs folder**. All references will be carefully looked at to make sure there are no broken references. This again will help us keep a compilable and working build at the end of each week.

## Materials and Textures

Every time you create a new material for your scene, please place the material in the **Materials** folder. Please be thoughtful when it comes to naming your materials so that it can be reused by others when building their prototypes.

We will follow the same principle for **Textures** folder as well. Good descriptive names go a long way into helping others search for the material and/or textures they want to use with their prototypes.

# Demystifying Unity

## Awake vs Start vs OnEnabled

At the outset these functions appear to do exactly the same thing but this is not true. The general ordering is, Awake runs first, then OnEnabled and then Start.

But the difference is as follows
- Awake and Start run only once when the object is first initialized to the scene. Where as OnEnabled runs every time you SetActive a deactivated object or every time you SetActive the script component.
- If an object and the script is active, both Awake and Start are called. But if the object is active and script is not active when we first start the game, only Awake is called. Start is called at a later time when the script is first activated.

# Game Input Controls

## Keyboard Keys

### Master Scene

##### GameManager.cs

M - open/close menu  
R - reset gadgets (in Build mode only)  
Left-Click - select a gadget in world (in Build mode only)

##### GadgetManipulator.cs
D - Removes the gadget from the world  
C - keeps on rotating gadget clock-wise around y-axis until released  
Z - keeps on rotating gadget counter clock-wise around y-axis until released  
Enter - place a gadget down  
Mouse - moving gadget templates

##### CannonGadget.cs
F - fire cannon

# Current Issues
- Add physics materials to the gadgets.
- Disable select mode in Draw mode.
- Disable gadget functions for the gadget templates until solidified.
- "F" (fire) will trigger multiple cannon gadgets in the world.
- Transformation changed immediately after selecting gadget.
- Change transparency of some gadget templates (airplane, cannon, etc.).

## Needed Features
- Snapping capability for ramps and tracks.
- Snapping feature of all gadgets (to fix the shaky remote situation).
- Saving and loading function.
- Simulation mode.

## Future Gadget Ideas
- Tracks
- Conveyor belt
- Support beams
- Bridge (sort of like a track)

## Assigned Tasks (9/27)
Kishore:
- VIVE and ZED M bug fix.

Mike:
- Work on save and load game function.
- Create 2-D blueprint of a game level.

Howard:
- Update documentation and clean up Unity folder.
- VIVE and ZED M bug fix.

Abhi:
- Fix airplane transparency issue.
- Design UI features.

Sarah:
- Work on UI features and design.

Devashree:
- Finish fan gadget.
- Work on UI features and design.

Darwin:
- Optimization of the overall game system.

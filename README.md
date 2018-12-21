# RuGo
This was a team project for the USC CSCI538 Augmented, Virtual, and Mixed Reality course. Read more about the game on our website [here](https://hsuanhauliu.github.io/RuGo/).

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

The **GameManager** manages the current mode of the game. It listens for user input to open the GadgetShelf.


Currently there are three modes.

* Build Mode - The default mode in which the player can pick gadgets in the world for manipulation

* Select Mode - The mode that is set when the GadgetShelf is visible

* Draw Mode - The mode that is set when Draw/Path tooling is enabled

* Delete Mode - Gadgets touched in delete mode are deleted from the scene.


## World

A collection of Gadgets that have been inserted into the scene. The world also manages auto saving to a current save slot.


## Adding New Gadgets

**This guide is subject to change and is intended to only give a rough overview**


1. Create a prefab in the resources folder with the name of your gadget.
2. Attach a YourGadget.cs file to your prefab. YourGadget.cs must extend the **Gadget** abstract class.
3. Add the name of your gadget, which matches the name in the resources directory, to the GadgetInventory enum inside Gadget.cs.

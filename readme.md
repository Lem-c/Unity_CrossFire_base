# Unity First-Person Shooter: CrossFire

*An experimental project to learn game development using Unity.*

## Features

- Direct import of FBX models as player arms.
- Weapon switching between rifle, pistol, and knife.
- Weapon drop and pick up
- Physics-based bullets + ray detection with customizable damage settings.
- Separate audio player for each weapon (implemented using animation events to call the corresponding methods).
- Footstep sound effects.
- Player state UI (health, weapon, ammo and weapon slot switch)
- Hitbox that requires manual adjustment for damage differentiation
- Recoil pattern system

## How to use

*For Experience Features*: Download the provided released ZIP file, extract it, and run the __.exe__ file (Windows only).

*For Further Development*: Clone the repository to your local directory, open the project, and switch to the __Ship__ scene, which contains all necessary components.

## Resources

*All resources are sourced from the [GAMEBANANA](https://gamebanana.com/mods/459371) CS 1.6 mod and other online collections.*

The __.mdl__ files were decompiled using [Crowbar](https://valvedev.info/tools/crowbar/). The __.qc__ files were re-imported into Blender for __.fbx__ model export to be used in Unity.

Inspired by [FabianoE](https://github.com/FabianoE/CrossFire---UNITY)'s project, thanks.

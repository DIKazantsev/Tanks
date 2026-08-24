# Third-party asset licenses

This file tracks third-party assets used or evaluated in the Tanks prototype.

## Abrams M1

- Source: https://sketchfab.com/3d-models/abrams-m1-34fcc7e772724a1c810652171f1c506d
- Author: Fri13Day
- License: Creative Commons Attribution 4.0 International (CC BY 4.0)
- Attribution: Required in the shipped game's credits. Include the author, asset title, source URL, CC BY 4.0 license URL, and note any modifications.
- Planned changes: None yet. Any Unity import setup, material conversion, prefab creation, or mesh separation must be recorded here.

## Challenger Tank – Realistic 3D Military Vehicle

- Source: https://sketchfab.com/3d-models/challenger-tank-realistic-3d-military-vehicle-8011c2a901da4407a9bffc408f9516e1
- Author: erenzek
- License: Creative Commons Attribution 4.0 International (CC BY 4.0)
- Attribution: Required in the shipped game's credits. Include the author, asset title, source URL, CC BY 4.0 license URL, and note any modifications.
- Planned changes: None yet. Any Unity import setup, material conversion, prefab creation, or mesh separation must be recorded here.

## WWII Tank Pack — Panther and IS-2

- Source: https://metaworldos.itch.io/wwii-tank-pack-glb-models
- Author: metaworldos
- License: CC0 1.0 Universal / Public Domain Dedication
- Attribution: Not required. Preserve this record for provenance.
- Planned changes: None yet. The pack's published format is GLB/glTF 2.0 with embedded PBR materials; any conversion or prefab setup must be recorded here.

## Kenney Nature Kit 2.1

- Source: https://kenney.nl/assets/nature-kit
- Author: Kenney
- License: Creative Commons Zero (CC0 1.0)
- Commercial use: Allowed; attribution is optional.
- Downloaded from: https://kenney.nl/media/pages/assets/nature-kit/37ac38a37b-1677698939/kenney_nature-kit.zip
- Used files: selected rock, grass and pine FBX models under `Assets/_Project/Art/Environment/KenneyNature/`.

## Kenney Impact Sounds 1.0

- Source: https://kenney.nl/assets/impact-sounds
- Author: Kenney
- License: Creative Commons Zero (CC0 1.0)
- Commercial use: Allowed; attribution is optional.
- Downloaded from: https://kenney.nl/media/pages/assets/impact-sounds/87b4ddecda-1677589768/kenney_impact-sounds.zip
- Used files: selected OGG metal and generic impact clips under `Assets/_Project/Audio/Impacts/`.

## Tanks-Unity AI reference

- Source: https://github.com/agneay/Tanks-Unity
- Author: Agneay B Nair
- License: MIT
- Unity version checked: 6000.3.7f1; project target: 6000.5.9f1.
- Used code: the seek/path-refresh/cooldown approach was adapted into the project's `EnemyTankAI` without importing the donor project, packages or settings.
- Required notice: retain this MIT notice with redistributed substantial portions of the adapted code.

## Evaluated but not imported

- Unity Terrain Sample Asset Pack: https://assetstore.unity.com/packages/3d/environments/landscapes/terrain-sample-asset-pack-145808. Free under the Unity Asset Store EULA and URP-compatible, but 1.6 GB and unavailable for unattended download in this environment.
- Unity Particle Pack: https://assetstore.unity.com/packages/vfx/particles/particle-pack-127325. Free under the Unity Companion License and URP-compatible in recent releases, but not downloaded because Asset Store authentication is required.
- Unity-URP-SmokeLighting: https://github.com/peeweek/Unity-URP-SmokeLighting. MIT, but it is a Unity 2021.2 custom smoke shader reference rather than a drop-in VFX pack; not imported to avoid custom shader dependencies and magenta risk.

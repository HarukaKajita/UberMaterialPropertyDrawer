# UberMaterialPropertyDrawer

This document is the English README. A Japanese version is also available: `README_JP.md`.

## Overview
UberMaterialPropertyDrawer is a Unity Editor extension that adds grouped and custom material property drawers for ShaderLab. It improves inspector readability by organizing properties into foldout groups, providing toggle-driven sections, and adding specialized fields such as vectors, enums, and generated textures (curve/gradient).

## Requirements
- Unity 2022.3 LTS (project currently uses 2022.3.22f1).
- Editor-only package: all scripts live under `Assets/UberMaterialPropertyDrawer/Editor/`.

## Installation (UPM via Git)
Add the repository as a UPM package with a Git URL in the Package Manager.
```
git@github.com:HarukaKajita/UberMaterialPropertyDrawer.git?path=Assets/UberMaterialPropertyDrawer
```
If the repository is private, make sure your Git access is configured (SSH key or credential helper).

## Usage (ShaderLab)
Use the `Uber` material property drawer attribute in the shader `Properties` block.
The drawer syntax is:
```
[Uber(GroupName, DrawerName, optionalArgs...)]
```
`GroupName` controls grouping, `DrawerName` selects behavior, and `optionalArgs` configure the drawer.

Example:
```
[Uber(MyGroup, BeginGroup)] _GroupHeader("My Group", Float) = 0
[Uber(MyGroup, ToggleUI)] _Enable("Enable", Float) = 0
[Uber(MyGroup, Vector2)] _Scale("Scale", Vector) = (1, 1, 0, 0)
[Uber(MyGroup, CurveTexture, ch4, res256, bit16)] _CurveTex("Curve", 2D) = "white" {}
[Uber(MyGroup, GradientTexture, res256)] _GradientTex("Gradient", 2D) = "white" {}
[Uber(MyGroup, EndGroup)] _GroupEnd("End", Float) = 0
```

### Supported drawer types
- `BeginGroup` / `EndGroup`: Foldout group start/end.
- `BeginToggleGroup`: Foldout group controlled by a Float/Int toggle property (the property itself is the toggle).
- `ToggleUI`: Boolean toggle UI for Float/Int properties.
- `Vector2` / `Vector3`: Compact vector fields with custom label layout.
- `Enum`: Enum popup. Use `Enum` + type name, or explicit `name, value` pairs.
- `CurveTexture`: Generates a curve-based texture and assigns it to the property.
- `GradientTexture`: Generates a gradient texture and assigns it to the property.

Example for `Enum` by type name:
```
[Uber(MyGroup, Enum, MyEnumType)] _Mode("Mode", Float) = 0
```

Example for `Enum` with explicit pairs:
```
[Uber(MyGroup, Enum, Low, 0, High, 1)] _Mode("Mode", Float) = 0
```

### Curve/Gradient arguments
Arguments are optional and can be combined:
- `resN` (e.g., `res256`): texture resolution.
- `bit16`: use 16-bit texture formats.
- `chN` (e.g., `ch1`, `ch4`): channel count (used by curve textures).
- `accum`: accumulate curve values over the X axis (curve textures).

## How it works
- Drawer state (foldout open/close) is tracked per group name.
- Curve/gradient data is stored as sub-assets inside the material.
- Generated textures are also stored as material sub-assets and assigned to the property.

## Tips
- Use Float or Int properties for toggle-driven drawers (`BeginToggleGroup`, `ToggleUI`).
- Place `BeginGroup`/`EndGroup` around related properties to keep inspectors readable.
- If a group does not respond, confirm the group name matches exactly (case-sensitive).

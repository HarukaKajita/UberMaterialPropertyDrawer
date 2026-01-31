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
Use drawer class names directly in the shader `Properties` block.
The drawer syntax is:
```
[DrawerName(GroupName, optionalArgs...)]
```
`GroupName` controls grouping, and `optionalArgs` configure the drawer.

Example:
```
[BeginGroup(MyGroup)] _GroupHeader("My Group", Float) = 0
[UberToggle(MyGroup)] _Enable("Enable", Float) = 0
[Vector2(MyGroup)] _Scale("Scale", Vector) = (1, 1, 0, 0)
[CurveTexture(MyGroup, ch4, res256, bit16)] _CurveTex("Curve", 2D) = "white" {}
[GradientTexture(MyGroup, res256)] _GradientTex("Gradient", 2D) = "white" {}
[EndGroup(MyGroup)] _GroupEnd("End", Float) = 0
```

### InitGroup decorator
`[InitGroupDecorator]` resets the group state. It can be stacked with other drawers:
```
[InitGroupDecorator][BeginGroup(MyGroup)] _GroupHeader("My Group", Float) = 0
```

### Supported drawer types
- `BeginGroup` / `EndGroup`: Foldout group start/end.
- `BeginToggleGroup`: Foldout group controlled by a Float/Int toggle property (the property itself is the toggle).
- `UberToggle`: Boolean toggle UI for Float/Int properties.
- `Vector2` / `Vector3`: Compact vector fields with custom label layout.
- `UberEnum`: Enum popup. Use enum type name or explicit `name, value` pairs.
- `CurveTexture`: Generates a curve-based texture and assigns it to the property.
- `GradientTexture`: Generates a gradient texture and assigns it to the property.

Example for `UberEnum` by type name:
```
[UberEnum(MyGroup, MyEnumType)] _Mode("Mode", Float) = 0
```

Example for `UberEnum` with explicit pairs:
```
[UberEnum(MyGroup, Low, 0, High, 1)] _Mode("Mode", Float) = 0
```

### Curve/Gradient arguments
Arguments are optional and can be combined:
- `resN` (e.g., `res256`): texture resolution.
- `bit16`: use 16-bit texture formats.
- `chN` (e.g., `ch1`, `ch4`): channel count (used by curve textures).
- `accum`: accumulate curve values over the X axis (curve textures).

### Group name optional (group-only drawers)
For `Vector2`, `Vector3`, and `UberToggle`, the group name can be omitted:
- If used outside a group: always visible (acts like a normal drawer).
- If used inside a group: visibility follows the parent foldout.

### Legacy `Uber` drawer
`[Uber(GroupName)]` is kept for simple grouping with default drawing. It ignores extra args and always uses standard shader property rendering.

## How it works
- Drawer state (foldout open/close) is tracked per group name.
- Curve/gradient data is stored as sub-assets inside the material.
- Generated textures are also stored as material sub-assets and assigned to the property.

## Tips
- Use Float or Int properties for toggle-driven drawers (`BeginToggleGroup`, `UberToggle`).
- Place `BeginGroup`/`EndGroup` around related properties to keep inspectors readable.
- If a group does not respond, confirm the group name matches exactly (case-sensitive).

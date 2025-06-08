# Uber Material Property Drawer

## Overview

The Uber Material Property Drawer is a comprehensive system for creating highly customized and organized Material Inspectors in Unity. Instead of the default flat list of properties, this tool allows shader authors to group properties into collapsible sections, create toggleable feature groups, use custom drawers for enums and vectors, and manage nested hierarchies of properties.

This system is implemented as a collection of `MaterialPropertyDrawer` scripts that are invoked from shader files using specific syntax in the property attributes.

## Key Features & Components

The system is primarily driven by the `UberDrawer` class, which acts as a dispatcher for various drawing behaviors based on arguments provided in a material property's `[Uber(...)]` attribute in the shader.

-   **Collapsible Groups:**
    -   `[Uber(GroupName, BeginGroup)]`: Starts a foldable section in the Inspector titled `GroupName`.
    -   `[Uber(GroupName, EndGroup)]`: Marks the end of the section.
    -   Properties within this group (also tagged with `[Uber(GroupName)]`) will only be visible if the group is expanded.
-   **Toggleable Groups:**
    -   `[Uber(GroupName, BeginToggleGroup)]`: Similar to `BeginGroup`, but the group header includes a toggle checkbox. This checkbox controls a float/integer property in the shader (typically 0 or 1), allowing you to enable/disable the feature controlled by the properties within this group. The group is also foldable.
-   **Enum Dropdowns:**
    -   `[Uber(GroupName, Enum, EnumTypeName)]`: Draws a float or integer shader property as a dropdown menu.
    -   `EnumTypeName` must be the string name of a C# enum type defined in your project (e.g., `"UnityEngine.Rendering.BlendMode"` or a custom enum). The drawer uses reflection to populate the dropdown.
-   **Custom Vector Drawers:**
    -   `[Uber(GroupName, Vector2)]`: Provides a compact drawer for `Vector2` properties.
    -   `[Uber(GroupName, Vector3)]`: Provides a compact drawer for `Vector3` properties.
    -   These ensure consistent layout within groups.
-   **Curve Texture Drawer:**
    -   `[Uber(GroupName, CurveTexture, size,mode,precision)]`: Shows up to four `AnimationCurve` fields and bakes them into a texture stored as a sub-asset of the material. `size` is the texture width, `mode` is `value` or `cumulative`, and `precision` is `8bit` or `half`.
-   **Property Visibility:**
    -   Standard material properties tagged with `[Uber(GroupName)]` are drawn using the default property drawer but are only visible if `GroupName` (and all its parent groups, if nested) are expanded.
-   **Nesting:** Groups can be nested within other groups to create deeper hierarchies.
-   **State Initialization:**
    -   `[Uber(_, Init)]`: A special command, usually placed on a dummy property at the beginning of the shader's `Properties` block. It resets the expanded/collapsed state of all groups. The group name `_` is a convention for this utility.

## How to Use in Shaders

To use the Uber Material Property Drawer system, you'll decorate your shader properties with `[Uber(...)]` attributes.

**1. Initialization (Recommended):**
Place this on a dummy property at the very beginning of your `Properties` block to ensure foldout states are reset when the shader is selected or recompiled.
```shaderlab
Properties
{
    [Uber(_, Init)] _Init ("Editor State Initializer", Int) = 0 // Group name '_' is conventional
    // ... rest of your properties
}
```

**2. Creating a Basic Foldable Group:**
```shaderlab
    [Uber(MySection, BeginGroup)] _MySectionFoldout ("My Section Settings", Int) = 0 // Dummy property for the group header
    [Uber(MySection)] _MyColor ("My Color", Color) = (1,1,1,1)
    [Uber(MySection)] _MyFloat ("My Float", Float) = 1.0
    [Uber(MySection, EndGroup)] _MySectionEnd ("End My Section", Int) = 0 // Dummy property
```
-   `MySection` is the unique name for this group.
-   Properties `_MyColor` and `_MyFloat` will only be visible when "My Section Settings" is expanded.

**3. Creating a Toggleable Group:**
The property associated with `BeginToggleGroup` will act as the toggle (0 for off, 1 for on).
```shaderlab
    [Uber(MyFeature, BeginToggleGroup)] _EnableMyFeature ("Enable My Feature", Float) = 1.0 // This property is the toggle
    [Uber(MyFeature)] _FeatureTexture ("Feature Texture", 2D) = "white" {}
    [Uber(MyFeature)] _FeatureIntensity ("Feature Intensity", Range(0, 5)) = 1.0
    [Uber(MyFeature, EndGroup)] _MyFeatureEnd ("End My Feature", Int) = 0
```
-   The "Enable My Feature" header will have a checkbox tied to `_EnableMyFeature`.
-   `_FeatureTexture` and `_FeatureIntensity` are only shown if "Enable My Feature" is expanded. Their usability in the shader would typically depend on the value of `_EnableMyFeature`.

**4. Using Enum Dropdowns:**
Ensure you have a C# enum defined, for example:
```csharp
// In a C# script (not necessarily in an Editor folder)
public enum MyBlendMode { Alpha, Additive, Multiply }
```
Then in the shader:
```shaderlab
    [Uber(Effects, BeginGroup)] _EffectsFoldout ("Effects Settings", Int) = 0
    [Uber(Effects, Enum, MyBlendMode)] _CurrentBlendMode ("Blend Mode", Float) = 0 // Default to Alpha (0)
    [Uber(Effects, EndGroup)] _EffectsEnd ("End Effects", Int) = 0
```
-   The `_CurrentBlendMode` property will appear as a dropdown with "Alpha", "Additive", and "Multiply" options.

**5. Using Custom Vector Drawers:**
```shaderlab
    [Uber(Transform, BeginGroup)] _TransformFoldout ("Transform Settings", Int) = 0
    [Uber(Transform, Vector2)] _Offset ("UV Offset", Vector) = (0,0,0,0) // Vector2 uses x,y
    [Uber(Transform, Vector3)] _Scale ("Local Scale", Vector) = (1,1,1,0) // Vector3 uses x,y,z
    [Uber(Transform, EndGroup)] _TransformEnd ("End Transform", Int) = 0
```

**6. Baking Curves to a Texture:**
```shaderlab
    [Uber(Curves, CurveTexture, 256,cumulative,half)] _CurveTex ("Curve Texture", 2D) = "white" {}
```

**7. Nesting Groups:**
Simply define a `BeginGroup` within another active group.
```shaderlab
    [Uber(Parent, BeginGroup)] _ParentFoldout ("Parent Group", Int) = 0
    [Uber(Parent)] _ParentProp ("Parent Property", Float) = 0.0

        [Uber(Child, BeginGroup)] _ChildFoldout ("Child Group (inside Parent)", Int) = 0
        [Uber(Child)] _ChildProp ("Child Property", Color) = (1,0,0,1)
        [Uber(Child, EndGroup)] _ChildEnd ("End Child", Int) = 0

    [Uber(Parent, EndGroup)] _ParentEnd ("End Parent", Int) = 0
```
-   The "Child Group" will only be visible if "Parent Group" is expanded. `_ChildProp` is only visible if both are expanded.

**Important Notes:**
-   **Group Names:** Group names are significant and are used to track the expanded/collapsed state. Choose unique and descriptive names.
-   **Dummy Properties:** `BeginGroup`, `EndGroup`, and `Init` drawers are typically attached to dummy integer or float properties in the shader (e.g., `_MySectionFoldout`, `_Init`). The actual values of these dummy properties are not usually used by the drawer logic, but the properties themselves act as anchors for the drawer attributes. For `BeginToggleGroup`, the property it's attached to *is* used for the toggle state.
-   **Order Matters:** The `EndGroup` call should correspond to the correct `BeginGroup` or `BeginToggleGroup`. The system uses a stack internally, so mismatches can lead to errors or unexpected UI behavior.
-   **Shader Compilation:** Changes to these attributes require the shader to recompile for the Material Inspector to update.

This system provides a robust toolkit for creating clean, user-friendly, and highly organized interfaces for complex materials.

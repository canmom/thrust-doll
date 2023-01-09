using Unity.Entities;
using Unity.Rendering;

[MaterialProperty("_Ring_thickness")]
struct UIToggleRingThickness : IComponentData
{
    public float Thickness;
}

[MaterialProperty("_Centre_opacity")]
struct UIToggleCentreOpacity : IComponentData
{
    public float Opacity;
}
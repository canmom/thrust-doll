using System;

[Flags]
public enum CollisionLayers
{
    ClickEvent = 1 << 0,
    Toggleable = 1 << 1
}
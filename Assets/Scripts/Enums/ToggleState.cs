using System;

[Flags]
public enum ToggleState
{
    Enabled = 1 << 0,
    Hovering = 1 << 1
}
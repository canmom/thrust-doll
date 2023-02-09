using Unity.Entities;
using UnityEngine.InputSystem;
using Unity.Mathematics;

partial class InputReadingSystem : SystemBase
{
    protected override void OnCreate()
    {
        EntityManager
            .AddComponent<Intent>(SystemHandle);
        UnityEngine.Cursor.lockState = UnityEngine.CursorLockMode.Locked;
    }

    protected override void OnUpdate()
    {
        var mouse = Mouse.current;

        float2 delta = mouse.delta.ReadValue() * 0.001f;

        bool thrust = mouse.leftButton.wasPressedThisFrame;

        Intent intent = new Intent { Rotate = delta, Thrust = thrust };

        SystemAPI
            .SetComponent<Intent>(SystemHandle, intent);
    }
}
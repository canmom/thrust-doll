using Unity.Entities;
using UnityEngine.InputSystem;
using Unity.Mathematics;

partial class InputReadingSystem : SystemBase
{
    protected override void OnCreate()
    {
        EntityManager.AddComponent<Intent>(SystemHandle);
    }

    protected override void OnUpdate()
    {
        var mouse = Mouse.current;

        float2 delta = mouse.delta.ReadValue() * SystemAPI.Time.DeltaTime;

        Intent intent = new Intent { Rotate = delta };

        SystemAPI.SetComponent<Intent>(SystemHandle, intent);
    }
}
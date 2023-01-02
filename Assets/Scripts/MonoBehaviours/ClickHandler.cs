using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Entities;
using Unity.Physics;

public class ClickHandler : MonoBehaviour
{
    public InputAction ClickAction;
    public Camera Camera;

    private Entity Entity;
    private World World;

    private void OnEnable()
    {
        // set the callback for the click action
        ClickAction.performed += OnClick;
        ClickAction.Enable();

        Camera = Camera == null ? Camera.main : Camera;

        World = World.DefaultGameObjectInjectionWorld;
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        Vector2 screenPosition = context.ReadValue<Vector2>();
        UnityEngine.Ray ray = Camera.ScreenPointToRay(screenPosition);

        Debug.Log(ray.GetPoint(Camera.farClipPlane));

        if(World.IsCreated && !World.EntityManager.Exists(Entity))
        {
            Entity = World.EntityManager.CreateEntity();
            World.EntityManager.AddBuffer<UIClick>(Entity);
        }

        RaycastInput input = new RaycastInput() {
            Start = ray.origin,
            Filter = new CollisionFilter
            {
                BelongsTo = (uint) CollisionLayers.ClickEvent,
                CollidesWith = (uint) CollisionLayers.Toggleable
            },
            End = ray.GetPoint(Camera.farClipPlane)
        };

        World.EntityManager.GetBuffer<UIClick>(Entity).Add(new UIClick() {Value = input});
    }

    private void OnDisable()
    {
        ClickAction.started -= OnClick;
        ClickAction.Disable();
    }
}

public struct UIClick : IBufferElementData
{
    public RaycastInput Value;
}
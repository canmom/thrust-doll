using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Entities;
//using Unity.Physics;

public class ClickHandler : MonoBehaviour
{
    public InputAction ClickAction;
    public InputAction PointerMoveAction;
    public Camera Camera;

    private Entity Entity;
    private World World;

    private void OnEnable()
    {
        // set the callback for the click action
        ClickAction.performed += OnClick;
        ClickAction.Enable();

        // PointerMoveAction.performed += OnMove;
        // PointerMoveAction.Enable();

        Camera = Camera == null ? Camera.main : Camera;

        World = World.DefaultGameObjectInjectionWorld;
    }

    // private void OnMove(InputAction.CallbackContext context)
    // {
    //     CheckEntityExists();

    //     World.EntityManager.GetBuffer<MouseMove>(Entity).Add(new MouseMove() { Ray = Ray(context)});
    // }

    private void OnClick(InputAction.CallbackContext context)
    {
        CheckEntityExists();

        World.EntityManager.GetBuffer<MouseClick>(Entity).Add(new MouseClick() { Clicked = true });
    }

    private void OnDisable()
    {
        ClickAction.performed -= OnClick;
        ClickAction.Disable();

        // PointerMoveAction.performed -= OnMove;
        // PointerMoveAction.Disable();
    }

    // private RaycastInput Ray(InputAction.CallbackContext context) {
    //     Vector2 screenPosition = context.ReadValue<Vector2>();
    //     UnityEngine.Ray ray = Camera.ScreenPointToRay(screenPosition);

    //     return new RaycastInput() {
    //         Start = ray.origin,
    //         Filter = new CollisionFilter
    //         {
    //             BelongsTo = (uint) CollisionLayers.ClickEvent,
    //             CollidesWith = (uint) CollisionLayers.Toggleable
    //         },
    //         End = ray.GetPoint(Camera.farClipPlane)
    //     };
    // }

    private void CheckEntityExists() {
        if(World.IsCreated && !World.EntityManager.Exists(Entity))
        {
            Entity = World.EntityManager.CreateEntity();
            // World.EntityManager.AddBuffer<MouseMove>(Entity);
            World.EntityManager.AddBuffer<MouseClick>(Entity);
        }
    }
}

// public struct MouseMove : IBufferElementData
// {
//     public RaycastInput Ray;
// }

public struct MouseClick : IBufferElementData
{
    public bool Clicked;
}
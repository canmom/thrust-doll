using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AddCollidersMenuItem : MonoBehaviour
{
    [MenuItem( "Utils/Add convex colliders and remove renderers" )]
    static void AddConvexCollidersAndRemoveRenderers()
    {
      foreach( GameObject go in Selection.gameObjects )
      {
        MeshCollider m = go.AddComponent<MeshCollider>();
        m.convex = true;
        Object.DestroyImmediate( go.GetComponent<MeshRenderer>());
      }
    }

    [MenuItem( "Utils/Add wall collider tag component" )]
    static void AddWallColliders()
    {
        foreach( GameObject go in Selection.gameObjects )
        {
            go.AddComponent<WallColliderAuthoring>();
        }
    }
}

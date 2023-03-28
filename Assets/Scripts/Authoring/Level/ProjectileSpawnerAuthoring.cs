using Unity.Entities;
using Unity.Mathematics;

public class ProjectileSpawnerAuthoring : UnityEngine.MonoBehaviour
{
    public float Interval;
    public float RotationPeriod;
    public float ShotSpeed;
}

public class ProjectileSpawnerBaker : Baker<ProjectileSpawnerAuthoring>
{
    public override void Bake(ProjectileSpawnerAuthoring authoring)
    {
        float3 rotationAxis = (float3) GetComponent<UnityEngine.Transform>().up;

        AddComponent
            ( new AngularVelocity
                { Value = rotationAxis * 2 * math.PI / authoring.RotationPeriod
                }
            );

        AddComponent
            ( new ProjectileSpawner
                { Interval = authoring.Interval
                , ShotSpeed = authoring.ShotSpeed
                }
            );
    }
}
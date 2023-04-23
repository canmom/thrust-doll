using Unity.Entities;
using Unity.Mathematics;

public class CharacterAuthoring : UnityEngine.MonoBehaviour
{
    public float DragCoefficient;
    public float AngularDamping;
}

class CharacterBaker : Baker<CharacterAuthoring>
{
    public override void Bake(CharacterAuthoring authoring)
    {
        AddComponent<Character>();
        AddComponent
            ( new Velocity 
                { Value = new float3(0f)
                }
            );
        AddComponent
            ( new AngularVelocity
                { Value = new float3(0f)
                }
            );
        AddComponent
            ( new Drag
                { Coefficient = authoring.DragCoefficient
                }
            );
        AddComponent
            ( new AngularDamping
                { Coefficient = authoring.AngularDamping
                }
            );
        AddComponent
            ( new DynamicAnimationClip
                { Index = AnimationClipIndex.LevelFlight
                , SampleTime = 0
                , Weight = 0
                }
            );
        // AddComponent
        //     (new DampedRotationSpring
        //         { Stiffness = authoring.RotationSpringStiffness
        //         , Damping = authoring.RotationSpringDamping
        //         }
        //     );
        // AddComponent
        //     (new RotationTarget
        //         { Target = new float3(0f, 0f, 1f)
        //         }
        //     );
    }
}

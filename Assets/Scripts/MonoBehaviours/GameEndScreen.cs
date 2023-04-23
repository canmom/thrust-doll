using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameEndScreen : MonoBehaviour
{
    private VisualElement _root;
    private Label _expendedElement;
    private Label _deathCounterElement;
    private Label _runTimeElement;

    [SerializeField]
    private string _deathCounterTemplate =
        "dolls killed in this deployment: <b>{0}</b>";
    [SerializeField]
    private string _runTimeTemplate =
        "this doll lived for: <b>{0:0.0} seconds</b>";

    public static GameEndScreen Instance;

    [SerializeField]
    private float _animationDelay = 0.5f;
    [SerializeField]
    private float _slideDuration = 0.5f;
    [SerializeField]
    private float _turnRedDuration = 0.8f;
    [SerializeField]
    private float _fadeInDuration = 1f;
    [SerializeField]
    private float _animationOverlap = 0.2f;

    void Awake()
    {
        Instance = this;

        _root =
            GetComponent<UIDocument>()
                .rootVisualElement
                .Q("DeathScreenRoot") as VisualElement;
        _expendedElement =
            _root
                .Q("Expended") as Label;
        _deathCounterElement =
            _root
                .Q("DeathCounter") as Label;
        _runTimeElement =
            _root
                .Q("RunTime") as Label;

        _root.style.transitionDelay =
            new List<TimeValue> { _animationDelay };
        _root.style.transitionDuration =
            new List<TimeValue> { _slideDuration };

        _expendedElement.style.transitionDelay =
            new List<TimeValue>
                { _animationDelay
                + _slideDuration 
                - _animationOverlap * 2f
                };
        _expendedElement.style.transitionDuration =
            new List<TimeValue> { _turnRedDuration };

        _deathCounterElement.style.transitionDelay =
            new List<TimeValue>
                { _animationDelay
                + _slideDuration
                + _turnRedDuration
                - _animationOverlap * 2f
                };
        _deathCounterElement.style.transitionDuration =
            new List<TimeValue>
                { _fadeInDuration
                };

        _runTimeElement.style.transitionDelay =
            new List<TimeValue>
                { _animationDelay
                + _slideDuration
                + _turnRedDuration
                + _fadeInDuration
                - _animationOverlap * 3f
                };
        _runTimeElement.style.transitionDuration =
            new List<TimeValue>
                { _fadeInDuration
                };
    }

    public void DisplayDeathScreen(double runTime, uint deaths)
    {
        _root.style.translate = new Translate(0, 0);
        _deathCounterElement.text = string.Format(_deathCounterTemplate, deaths);
        _runTimeElement.text = string.Format(_runTimeTemplate, runTime);

        _expendedElement.style.color = Color.red;
        _deathCounterElement.style.opacity = 1f;
        _runTimeElement.style.opacity = 1f;
    }

    public void HideDeathScreen()
    {
        var oldTransitionDelay = _root.style.transitionDelay;
        _root.style.transitionDelay = new List<TimeValue> {0};
        _root.style.translate = new Translate(new Length(120,LengthUnit.Percent), 0);
        _root.style.transitionDelay = oldTransitionDelay;
        _expendedElement.style.color = Color.white;
        _deathCounterElement.style.opacity = 0f;
        _runTimeElement.style.opacity = 0f;
    }
}
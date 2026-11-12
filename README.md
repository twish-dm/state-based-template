# State-based template

Unity 2022.3 template built around a small state machine and an observable model.
Gameplay and UI logic live in plain C# states; MonoBehaviours only bind scene objects to the model.

## Structure

```
Assets/Engine
├── Scripts      Engine: scene entry point, owns the root model and state machine
├── Stater       IState / State / Stater
├── Models       DynamicModel, DynamicList, ServiceRegistry
├── Events       Eventer (string-keyed event bus), ListEvent
├── Behaviours   DynamicBehaviour, BindingBehaviour, StaterBehaviour
├── Viewer       Stack-based screen navigation with Resources-loaded views
├── Components   UI bindings, navigation, spawner, layout helpers, REST client, timers
└── Example      Sample scene
```

## How it fits together

- `Engine` registers itself in `ServiceRegistry` and creates the root `DynamicModel` and `Stater`.
- A `State` subscribes to its own `Events` once in `OnInitialize`. The stater routes incoming events only to the active state, so states never unsubscribe on exit.
- `DynamicBehaviour` binds to the nearest `IStateContext` up the hierarchy (a `StaterBehaviour` or the engine).
  `ButtonBind` sends events to the active state, `TextBind` and `ActivatorBind` react to model changes.
- `StaterBehaviour` creates a nested scope with its own model and state machine, so a self-contained piece of UI can run its own states.

```csharp
public sealed class MainState : State
{
    protected override void OnInitialize()
    {
        Events.Add("Play", () => Viewer.Push("Gameplay"));
    }

    public override void Enter() => Model.Set("title", "Main menu");
}
```

## Notes

- `Assets/Engine/NavMeshComponents` is Unity's NavMeshComponents package and is kept as is.
- DOTween in `Assets/Plugins` is not used by the engine assembly.

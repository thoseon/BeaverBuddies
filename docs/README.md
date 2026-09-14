# BeaverBuddies developer documentation

Start at the repo-root [`CLAUDE.md`](../CLAUDE.md) for the quick map and checklists. These pages hold the detail.

## Pages

| Page | Read when |
|---|---|
| [architecture.md](architecture.md) | You need to understand lockstep, the tick loop, `EventIO`, speed/pause, serialization, or DI setup. |
| [debugging-desyncs.md](debugging-desyncs.md) | **A desync.** Detection mechanics, trace format, decision tree, adding traces, bisect switches, worked example, offline replay. |
| [events-and-patches.md](events-and-patches.md) | A player action does not sync; adding a `ReplayEvent`; full catalog of events and every Harmony patch; `[ManualMethodOverwrite]` list. |
| [determinism.md](determinism.md) | RNG, GUIDs, `Time.time`, movement/animation, parallel water; how to add a determinism fix. |
| [networking-and-connection.md](networking-and-connection.md) | Connection failures, map transfer, wire format, Steam vs TCP, user setup for triaging reports. |
| [logging-reference.md](logging-reference.md) | What a log line means; `Player.log` location; how to compare host and client logs. |
| [timberborn-modding.md](timberborn-modding.md) | Game-side concepts: `IModStarter`, Bindito, entities/components, dev/debug mode, UI Toolkit, decompiling. |
| [known-issues-and-history.md](known-issues-and-history.md) | Has this been seen before? Maintainer's issue log, open TODOs, eight reference fixes, recurring lessons. |
| [build-and-test.md](build-and-test.md) | Building, `env.props`, deploy path, versioning, manual two-instance verification, post-game-update checklist. |

## Existing developer notes (`BeaverBuddies/Doc/`)

| File | Content |
|---|---|
| `Movement.md` | How `Walker → PathFollower → MovementAnimator → AnimatedPathFollower → CharacterModel` interact and where float drift enters. Essential for movement desyncs. |
| `ClassesWithRandom.txt` | Audit of every Timberborn class using RNG. Legend: `=` addressed, `#` verified game logic/irrelevant, `>` partially addressed, `~` needs investigation, `+` unaddressed. |
| `ClassesWithHashSets.txt` | Classes holding HashSets (HashSet order was later ruled out as a desync cause). |
| `ClassesWithTimeTriggers.txt` | Classes using `TimeTrigger`s (noisy to trace). |
| `ParallelSingletons.txt` | The four parallel-ticked singletons. |
| `DerivedClasses.txt` | Large derived-class listing from ILSpy scans. |
| `UIFragments.txt` | All `IEntityPanelFragment`s, where most syncable UI actions live. |
| `AllEvents.txt` | Timberborn's own EventBus events (not `ReplayEvent`s). |
| `ToTestV6.md` | Open test checklist. |
| `Changelog.txt`, `Consent.txt`, `TranslationPrompt.txt`, `WorkshopDescription.txt` | Release and store text. |

## External

- BeaverBuddies wiki: https://github.com/thomaswp/BeaverBuddies/wiki (Home, Installation and Running, Port Forwarding Guide, How does it work?, Contributing, Debugging a desync). A local clone may exist at `BeaverBuddies.wiki/`; read pages with `git -C BeaverBuddies.wiki show HEAD:<Page>.md`.
- Timberborn modding wiki: https://github.com/mechanistry/timberborn-modding/wiki
- Desync trace viewer: https://thomaswp.github.io/BBDesyncViewer/
- Issues: https://github.com/thomaswp/BeaverBuddies/issues (templates in `.github/ISSUE_TEMPLATE/`)
- Live-coding video on adding an event: https://www.youtube.com/watch?v=xD7x8R580N0
- Steam Workshop item `3293380223`; mod.io: https://mod.io/g/timberborn/m/beaverbuddies; Discord thread linked from `README.md`.
- Agent definition for bug work: `.claude/agents/bugfixer.md`.

using Content.Server.Administration;
using Content.Shared._ArcheCrawl.StatEffects;
using Content.Shared._ArcheCrawl.StatEffects.Effects;
using Content.Shared.Administration;
using Content.Shared.Prototypes;
using Robust.Shared.Console;
using Robust.Shared.Containers;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server._ArcheCrawl.StatEffects
{
    public sealed class StatEffectsSystem : EntitySystem
    {
        [Dependency] private readonly IConsoleHost _consoleHost = default!;
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
        [Dependency] private readonly SharedStatEffectsSystem _sharedSystem = default!;

        /// <inheritdoc/>
        public override void Initialize()
        {
            SubscribeLocalEvent<StatEffectsComponent, ComponentShutdown>(OnShutdown);

            _consoleHost.RegisterCommand("addeffect", Loc.GetString("add-effect-command"), "addeffect <uid> <effect ID> <strength> <timer>", AddEffectCommand, StatusCommandCompletion);
        }

        private void OnShutdown(EntityUid uid, StatEffectsComponent component, ComponentShutdown args)
        {
            foreach (var effectUid in component.StatusContainer.ContainedEntities)
            {
                QueueDel(effectUid);
            }
        }

        [AdminCommand(AdminFlags.Fun)]
        private void AddEffectCommand(IConsoleShell shell, string argStr, string[] args)
        {
            if (args.Length < 2)
            {
                shell.WriteError("Too few arguments, arguments that can be used (in order) are: Entity Uid, Effect Prototype, optionally the strength of the effect and the length in seconds.");
                return;
            }

            if (!EntityUid.TryParse(args[0], out var uid) || !HasComp<StatEffectsComponent>(uid))
            {
                shell.WriteError("Entity either doesn't exist or cannot have effects.");
                return;
            }

            if (!_prototypeManager.TryIndex<EntityPrototype>(args[1], out var effectPrototype) || !effectPrototype.HasComponent<StatEffectComponent>())
            {
                shell.WriteError("Prototype either isn't real or doesn't have StatEffectComponent.");
                return;
            }

            var strength = 1;
            TimeSpan? length = null;

            if (args.TryGetValue(2, out var strStrength) && int.TryParse(strStrength, out var newStrength))
                strength = newStrength;

            if (args.TryGetValue(3, out var strLength) && float.TryParse(strLength, out var newLength))
                length = TimeSpan.FromSeconds(newLength);

            _sharedSystem.ApplyEffect(uid, effectPrototype.ID, strength, length, false, true);
        }

        private CompletionResult StatusCommandCompletion(IConsoleShell shell, string[] args)
        {
            if (args.Length == 1)
                return CompletionResult.FromHint("<uid>");

            if (args.Length == 2)
                return CompletionResult.FromHint("<effect proto ID>");

            if (args.Length == 3)
                return CompletionResult.FromHint("<strength>");

            if (args.Length == 4)
                return CompletionResult.FromHint("<length>");

            return CompletionResult.Empty;
        }
    }
}

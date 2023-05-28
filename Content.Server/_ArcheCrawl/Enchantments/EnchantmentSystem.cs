using System.Linq;
using Content.Server.Administration;
using Content.Shared._ArcheCrawl.Enchantments;
using Content.Shared._ArcheCrawl.Enchantments.Components;
using Content.Shared.Administration;
using Content.Shared.Prototypes;
using Robust.Shared.Console;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Server._ArcheCrawl.Enchantments;

/// <inheritdoc/>
public sealed class EnchantmentSystem : SharedEnchantmentSystem
{
    [Dependency] private readonly IConsoleHost _consoleHost = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<EnchantableComponent, ComponentGetState>(OnGetState);
        SubscribeLocalEvent<EnchantableComponent, ComponentShutdown>(OnShutdown);

        _consoleHost.RegisterCommand("addenchantment", Loc.GetString("add-enchantment-command"), "addenchantment <uid> <enchantment ID>",
            AddEnchantmentCommand,
            AddEnchantmentCommandCompletions);
    }

    private void OnGetState(EntityUid uid, EnchantableComponent component, ref ComponentGetState args)
    {
        args.State = new EnchantableComponentState();
    }

    private void OnShutdown(EntityUid uid, EnchantableComponent component, ComponentShutdown args)
    {
        foreach (var enchantmentUid in component.EnchantmentContainer.ContainedEntities)
        {
            QueueDel(enchantmentUid);
        }
    }

    [AdminCommand(AdminFlags.Fun)]
    private void AddEnchantmentCommand(IConsoleShell shell, string argstr, string[] args)
    {
        if (args.Length != 2)
            shell.WriteError("Argument length must be 2");

        if (!EntityUid.TryParse(args[0], out var uid) || !TryComp<EnchantableComponent>(uid, out var enchantable))
            return;

        if (!PrototypeManager.TryIndex<EntityPrototype>(args[1], out var enchantmentPrototype) || !enchantmentPrototype.HasComponent<EnchantmentComponent>())
            return;

        ApplyEnchantment(uid, enchantmentPrototype.ID, enchantable);
    }

    private CompletionResult AddEnchantmentCommandCompletions(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
        {
            var options = new List<CompletionOption>();
            var query = EntityQueryEnumerator<EnchantableComponent>();
            while (query.MoveNext(out var ent, out _))
            {
                options.Add(new CompletionOption(ent.ToString(), ToPrettyString(ent)));
            }
            return CompletionResult.FromHintOptions(options, "<uid>");
        }

        if (args.Length == 2)
        {
            var options = PrototypeManager.EnumeratePrototypes<EntityPrototype>()
                .Where(p => p.HasComponent<EnchantmentComponent>()).Select(p => p.ID).ToList();
            return CompletionResult.FromHintOptions(options, "<enchantment ID>");
        }

        return CompletionResult.Empty;
    }
}

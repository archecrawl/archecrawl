using System.Linq;
using Content.Server.Administration;
using Content.Shared._ArcheCrawl.Stats;
using Content.Shared._ArcheCrawl.Stats.Components;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server._ArcheCrawl.Stats;

/// <inheritdoc/>
public sealed class StatsSystem : SharedStatsSystem
{
    [Dependency] private readonly IConsoleHost _consoleHost = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        _consoleHost.RegisterCommand("setstat", Loc.GetString("stat-command-set-stat"), "setstat <uid> <stat ID> <value>",
            SetStatCommand,
            SetStatCommandCompletions);
    }

    [AdminCommand(AdminFlags.Fun)]
    private void SetStatCommand(IConsoleShell shell, string argstr, string[] args)
    {
        if (args.Length != 3)
            shell.WriteError("Argument length must be 3");

        if (!EntityUid.TryParse(args[0], out var uid) || !TryComp<StatsComponent>(uid, out var stat))
            return;

        if (!PrototypeManager.TryIndex<StatPrototype>(args[1], out var statPrototype))
            return;

        if (!int.TryParse(args[2], out var value))
            return;

        SetStatValue(uid, statPrototype, value, stat);
    }

    private CompletionResult SetStatCommandCompletions(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
        {
            var options = new List<CompletionOption>();
            var query = EntityQueryEnumerator<StatsComponent>();
            while (query.MoveNext(out var ent, out _))
            {
                options.Add(new CompletionOption(ent.ToString(), ToPrettyString(ent)));
            }
            return CompletionResult.FromHintOptions(options, "<uid>");
        }

        if (args.Length == 2 && EntityUid.TryParse(args[0], out var uid) && TryComp<StatsComponent>(uid, out var stats))
        {
            return CompletionResult.FromHintOptions(stats.Stats.Keys, "<stat ID>");
        }

        return CompletionResult.Empty;
    }
}

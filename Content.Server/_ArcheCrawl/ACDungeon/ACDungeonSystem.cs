using System.Threading.Tasks;
using Content.Server._ArcheCrawl.ACDungeon.Components;
using Content.Server.Atmos;
using Content.Server.Atmos.Components;
using Content.Server.Parallax;
using Content.Server.Procedural;
using Content.Shared.Atmos;
using Content.Shared.Gravity;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Content.Shared.Parallax.Biomes;
using Content.Shared.Procedural;
using Content.Shared.Tag;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._ArcheCrawl.ACDungeon;

public sealed class ACDungeonSystem : EntitySystem
{
    [Dependency] private readonly IMapManager _map = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly BiomeSystem _biome = default!;
    [Dependency] private readonly DungeonSystem _dungeon = default!;
    [Dependency] private readonly TagSystem _tag = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ACStaircaseComponent, StartCollideEvent>(OnCollideInit);
        SubscribeLocalEvent<ACStaircaseComponent, InteractHandEvent>(OnInteract);
    }

    private void OnCollideInit(EntityUid uid, ACStaircaseComponent component, ref StartCollideEvent args)
    {
        if (component.Generating)
            return;

        if (!HasComp<ActorComponent>(args.OtherEntity) || !HasComp<MobStateComponent>(args.OtherEntity))
            return;

        if (component.LinkedStair != null)
            return;

        component.Generating = true;
        GenerateFloor(uid, component);
    }

    private void OnInteract(EntityUid uid, ACStaircaseComponent component, InteractHandEvent args)
    {
        if (component.LinkedStair is not { } stair)
            return;

        var xform = Transform(stair);
        _transform.SetCoordinates(args.User, xform.Coordinates);
    }

    private async Task GenerateFloor(EntityUid uid, ACStaircaseComponent component)
    {
        var map = _map.CreateMap();
        var mapUid = _map.GetMapEntityId(map);

        var biome = EnsureComp<BiomeComponent>(mapUid);
        _biome.SetSeed(biome, _random.Next());
        _biome.SetTemplate(biome, _prototype.Index<BiomeTemplatePrototype>("ACFullCaves"));
        Dirty(mapUid, biome);

        var gravity = EnsureComp<GravityComponent>(mapUid);
        gravity.Enabled = true;
        Dirty(mapUid, gravity);

        var light = EnsureComp<MapLightComponent>(mapUid);
        light.AmbientLightColor = Color.FromHex("#181624");
        Dirty(mapUid, light);

        // Atmos
        var atmos = EnsureComp<MapAtmosphereComponent>(mapUid);

        atmos.Space = false;
        var moles = new float[Atmospherics.AdjustedNumberOfGases];
        moles[(int) Gas.Oxygen] = 21.824779f;
        moles[(int) Gas.Nitrogen] = 82.10312f;

        atmos.Mixture = new GasMixture(2500)
        {
            Temperature = 293.15f,
            Moles = moles,
        };

        var gridComp = EnsureComp<MapGridComponent>(mapUid);
        await _dungeon.GenerateDungeonAsync(_prototype.Index<DungeonConfigPrototype>("ACStone"), mapUid, gridComp, Vector2i.Zero, _random.Next());

        if (FindValidStairOnFloor(map) is not { } pair)
        {
            Log.Error("Failed to generate valid staircase to link to.");
            return;
        }

        var secondStairEnt = pair.Item1;
        var secondStairComp = pair.Item2;
        component.LinkedStair = secondStairEnt;
        secondStairComp.LinkedStair = uid;
    }

    private (EntityUid, ACStaircaseComponent)? FindValidStairOnFloor(MapId mapId)
    {
        var query = EntityQueryEnumerator<ACStaircaseComponent, TagComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var stair, out var tag, out var xform))
        {
            if (stair.LinkedStair != null)
                continue;

            if (xform.MapID != mapId)
                continue;

            if (!_tag.HasTag(tag, "ACUpStairs"))
                continue;

            return (uid, stair);
        }

        return null;
    }
}

namespace Content.Shared.NPC;

public abstract class SharedPathfindingSystem : EntitySystem
{
    /// <summary>
    /// This is equivalent to agent radii for navmeshes. In our case it's preferable that things are cleanly
    /// divisible per tile so we'll make sure it works as a discrete number.
    /// </summary>
    public const byte SubStep = 4;

    public const byte ChunkSize = 8;

    /// <summary>
    /// We won't do points on edges so we'll offset them slightly.
    /// </summary>
    protected const float StepOffset = 1f / SubStep / 2f;

    public Vector2 GetCoordinate(Vector2i chunk, Vector2i index)
    {
        return new Vector2(index.X, index.Y) / SubStep+ (chunk) * ChunkSize + StepOffset;
    }

    public static float ManhattanDistance(Vector2i start, Vector2i end)
    {
        var distance = end - start;
        return Math.Abs(distance.X) + Math.Abs(distance.Y);
    }

    public static float OctileDistance(Vector2i start, Vector2i end)
    {
        var diff = start - end;
        var ab = Vector2.Abs(diff);
        return ab.X + ab.Y + (1.41f - 2) * Math.Min(ab.X, ab.Y);
    }
}

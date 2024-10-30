using System;

public static class RhythmHelpers {

    // TODO test these values
    private const long INPUT_NORMAL_BAD_THRESHOLD = 150 * TimeSpan.TicksPerMillisecond;
    private const long INPUT_NORMAL_GOOD_THRESHOLD = 100 * TimeSpan.TicksPerMillisecond;
    private const long INPUT_NORMAL_GREAT_THRESHOLD = 50 * TimeSpan.TicksPerMillisecond;
    private const long INPUT_NORMAL_PERFECT_THRESHOLD = 25 * TimeSpan.TicksPerMillisecond;

    public const long INPUT_MAX_THRESHOLD = 200 * TimeSpan.TicksPerMillisecond;

    /// <summary>
    /// Returns whether there is a value associated with a given RhythmEventType
    /// </summary>
    public static bool HasValue(this RhythmEventType type) => type == RhythmEventType.BpmChange;

    public static InputType[] GetValidInputTypes(this RhythmEventType type) => type switch {
        RhythmEventType.Normal => new InputType[] { InputType.Normal },
        _ => Array.Empty<InputType>(),
    };

    public static BeatmapHitResult GetHitResultFromOffset(this InputType type, long signedOffset) {
        switch (type) {
            case InputType.Normal:
                long unsignedOffset = Math.Abs(signedOffset);
                if (unsignedOffset <= INPUT_NORMAL_PERFECT_THRESHOLD) {
                    return BeatmapHitResult.Perfect;
                } else if (unsignedOffset <= INPUT_NORMAL_GREAT_THRESHOLD) {
                    return BeatmapHitResult.Great;
                } else if (unsignedOffset <= INPUT_NORMAL_GOOD_THRESHOLD) {
                    return BeatmapHitResult.Good;
                } else if (unsignedOffset <= INPUT_NORMAL_BAD_THRESHOLD) {
                    return BeatmapHitResult.Bad;
                } else {
                    return BeatmapHitResult.NoHit;
                }
            default:
                // something went wrong
                return BeatmapHitResult.NoHit;
        }
    }
}

public static class RhythmHelpers {

	/// <summary>
	/// Returns whether there is a value associated with a given RhythmEventType
	/// </summary>
	public static bool HasValue(this RhythmEventType type) => type == RhythmEventType.BpmChange;
}

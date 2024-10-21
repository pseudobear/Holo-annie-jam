using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;

// TODO: replace MediaPlayer with audio player that has more control over play position

/// <summary>
/// Creates a new beatmap player with the given visible timespan, beatmap, and song. 
/// </summary>
/// <remarks>
/// Both the beatmap and the song should have a few seconds of delay at the start, greater than or equal to the visible
/// timespan.
/// </remarks>
public class BeatmapPlayer(long visibleTimespanTicks, Beatmap beatmap, Song song) {

    // TODO: make this a more reasonable number
    /// <summary>
    /// The default timespan that is visible on the screen when playing a beatmap, in ticks (100 nanoseconds).
    /// </summary>
    public const long DEFAULT_VISIBLE_TIMESPAN_TICKS = 2 * TimeSpan.TicksPerSecond;

    /// <summary>
    /// The timespan that is visible on the screen when playing a beatmap, in ticks (100 nanoseconds).
    /// </summary>
    public long VisibleTimespanTicks { get; } = visibleTimespanTicks;

    /// <summary>
    /// Whether the beatmap is currently in play
    /// </summary>
	public bool Playing { get; private set; } = false;

    /// <summary>
    /// Whether the beatmap is currently paused
    /// </summary>
    public bool Paused { get; private set; } = false;

    private readonly Beatmap _beatmap = beatmap;
    private readonly Song _song = song;

    private int firstVisibleEventIdx;
    private int nextVisibleEventIdx;

    /// <summary>
    /// Creates a new beatmap player with the default visible timespan and given beatmap and song. 
    /// </summary>
    /// <remarks>
    /// Both the beatmap and the song should have a few seconds of delay at the start, greater than or equal to the
    /// visible timespan.
    /// </remarks>
    public BeatmapPlayer(Beatmap beatmap, Song song) : this(DEFAULT_VISIBLE_TIMESPAN_TICKS, beatmap, song) { }

    /// <summary>
    /// Starts playing the beatmap
    /// </summary>
    public void Start() {
        if (!this.Playing) {
            this.Reset();
            MediaPlayer.Play(_song);
        }
    }

    /// <summary>
    /// Pauses the beatmap
    /// </summary>
    public void Pause() {
        if (!this.Paused) {
            this.Paused = true;
            MediaPlayer.Pause();
        }
    }

    /// <summary>
    /// Resumes the beatmap and calls <see cref="Update"/>
    /// </summary>
    public IEnumerable<RhythmEvent>? Resume() {
        if (this.Paused) {
            this.Paused = false;
            MediaPlayer.Resume();
        }
        return this.Update();
    }

    /// <summary>
    /// Gets an enumerable of currently visible rhythm events, or null if the beatmap is either already finished or not
    /// currently playing
    /// </summary>
    public IEnumerable<RhythmEvent>? Update() {
        while (this._beatmap.RhythmEvents[this.firstVisibleEventIdx].Tick < MediaPlayer.PlayPosition.Ticks) {
            this.firstVisibleEventIdx++;
            if (this.firstVisibleEventIdx >= this._beatmap.RhythmEvents.Length) {
                return null;
            }
        }
        while (this.nextVisibleEventIdx < this._beatmap.RhythmEvents.Length &&
               this._beatmap.RhythmEvents[this.nextVisibleEventIdx].Tick < MediaPlayer.PlayPosition.Ticks + this.VisibleTimespanTicks) {
            this.nextVisibleEventIdx++;
        }
        return this._beatmap.RhythmEvents
            .Skip(this.firstVisibleEventIdx)
            .Take(this.nextVisibleEventIdx - this.firstVisibleEventIdx);
    }

    /// <summary>
    /// Stops playing the beatmap, and resets the beatmap to a non-started state
    /// </summary>
    public void Reset() {
        this.Playing = false;
        this.Paused = false;
        this.firstVisibleEventIdx = 0;
        this.nextVisibleEventIdx = 0;
        MediaPlayer.Stop();
    }
}

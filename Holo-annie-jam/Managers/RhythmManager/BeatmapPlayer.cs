using ManagedBass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// Class representing beatmap player. Note that Bass.Init() MUST be called prior to using any of the play functions.
/// </summary>
public class BeatmapPlayer {

    // TODO: make this a more reasonable number
    /// <summary>
    /// The default timespan that is visible on the screen when playing a beatmap, in ticks (100 nanoseconds).
    /// </summary>
    public const long DEFAULT_VISIBLE_TIMESPAN_TICKS = (long)(1.4 * TimeSpan.TicksPerSecond);

    /// <summary>
    /// The default timespan that is still visible on the screen after the rhythm event has passed
    /// </summary>
    public const long DEFAULT_TRAILING_TIMESPAN_TICKS = RhythmHelpers.INPUT_MAX_THRESHOLD;

    /// <summary>
    /// The timespan that is visible on the screen when playing a beatmap, in ticks (100 nanoseconds).
    /// </summary>
    public long VisibleTimespanTicks { get; }

    public long TrailingTimespanTicks { get; }

    /// <summary>
    /// Whether the beatmap is currently in play
    /// </summary>
    public bool Playing { get; private set; } = false;

    /// <summary>
    /// Whether the beatmap is currently paused
    /// </summary>
    public bool Paused { get; private set; } = false;

    private readonly Beatmap _beatmap;
    private readonly int _songId;

    private int firstVisibleEventIdx;
    private int nextVisibleEventIdx;
    private int nextConsumableEventIdx;

    /// <summary>
    /// Creates a new beatmap player with the default visible timespan and given beatmap and song. 
    /// </summary>
    /// <remarks>
    /// Both the beatmap and the song should have a few seconds of delay at the start, greater than or equal to the
    /// visible timespan.
    /// </remarks>
    public BeatmapPlayer(Beatmap beatmap) : this(beatmap, DEFAULT_VISIBLE_TIMESPAN_TICKS, DEFAULT_TRAILING_TIMESPAN_TICKS) { }

    public BeatmapPlayer(Beatmap beatmap, long visibleTimespanTicks, long trailingTimespanTicks) {
        this._beatmap = beatmap;
        var a = File.Exists(beatmap.AudioLocation);
        this._songId = Bass.CreateStream(beatmap.AudioLocation);

        this.VisibleTimespanTicks = visibleTimespanTicks;
        this.TrailingTimespanTicks = trailingTimespanTicks;
    }

    ~BeatmapPlayer() {
        Bass.StreamFree(_songId);
    }

    /// <summary>
    /// Starts playing the beatmap
    /// </summary>
    public void Start() {
        if (!this.Playing) {
            this.Reset();
            Bass.ChannelPlay(this._songId);
        }
    }

    /// <summary>
    /// Pauses the beatmap
    /// </summary>
    public void Pause() {
        if (!this.Paused) {
            this.Paused = true;
            Bass.ChannelPause(this._songId);
        }
    }

    /// <summary>
    /// Resumes the beatmap and calls <see cref="GetVisibleEvents"/>
    /// </summary>
    public VisibleBeatmapEvents Resume() {
        if (this.Paused) {
            this.Paused = false;
            Bass.ChannelPlay(this._songId);
        }
        return this.GetVisibleEvents();
    }

    /// <summary>
    /// Jump to a specified position in the beatmap and calls <see cref="GetVisibleEvents"/>. Note that the position
    /// that it jumps to may not be exact to the given tick.
    /// </summary>
    public VisibleBeatmapEvents JumpTo(long tick) {
        this.Reset(tick);
        Bass.ChannelSetPosition(this._songId, Bass.ChannelSeconds2Bytes(this._songId, tick / (double) TimeSpan.TicksPerSecond));
        Bass.ChannelPlay(this._songId);
        return this.GetVisibleEvents();
    }

    /// <summary>
    /// Gets an enumerable of currently visible rhythm events, or null if the beatmap is either already finished
    /// currently playing
    /// </summary>
    public VisibleBeatmapEvents GetVisibleEvents() {
        long tick = this.GetCurrentPosition();
        if (this.firstVisibleEventIdx >= this._beatmap.RhythmEvents.Length) {
            return new() {
                Tick = tick
            };
        }
        while (this._beatmap.RhythmEvents[this.firstVisibleEventIdx].Tick < tick - this.TrailingTimespanTicks) {
            this.firstVisibleEventIdx++;
            if (this.firstVisibleEventIdx >= this._beatmap.RhythmEvents.Length) {
                return new() {
                    Tick = tick
                };
            }
        }
        while (this.nextVisibleEventIdx < this._beatmap.RhythmEvents.Length &&
                this._beatmap.RhythmEvents[this.nextVisibleEventIdx].Tick < tick + this.VisibleTimespanTicks) {
            this.nextVisibleEventIdx++;
        }
        return new VisibleBeatmapEvents() {
            Tick = tick,
            RhythmEvents = this._beatmap.RhythmEvents
                .Skip(this.firstVisibleEventIdx)
                .Take(this.nextVisibleEventIdx - this.firstVisibleEventIdx)
        };
    }

    /// <summary>
    /// Takes player input, assuming input was at current time, and matches it to a rhythm event in the beatmap to
    /// consume it. If no match was found, one of NoHit or Miss will be returned.
    /// </summary>
    public BeatmapHitResult ConsumePlayerInput(InputType inputType, uint lane) {
        long tick = this.GetCurrentPosition();
        BeatmapHitResult result = BeatmapHitResult.Miss;
        int eventIdx = this.nextConsumableEventIdx;
        while (eventIdx < this._beatmap.RhythmEvents.Length &&
                (result == BeatmapHitResult.NoHit || result == BeatmapHitResult.Miss) &&
                this._beatmap.RhythmEvents[eventIdx].Tick < tick + RhythmHelpers.INPUT_MAX_THRESHOLD) {
            // require lane to be strictly equal for now
            // later, it may be possible that we can introduce more lanes and input lenience (e.g. BanG Dream allows 1 lane difference)
            if (this._beatmap.RhythmEvents[eventIdx].Lane == lane) {
                result = this._beatmap.RhythmEvents[eventIdx].PeekInputResult(tick, inputType);
            }
            eventIdx++;
        }
        if (result != BeatmapHitResult.NoHit && result != BeatmapHitResult.Miss) {
            this._beatmap.RhythmEvents[eventIdx - 1].HitResult = result;
            this.nextConsumableEventIdx = eventIdx;
        } else {
            while (this.nextConsumableEventIdx < this._beatmap.RhythmEvents.Length &&
                    this._beatmap.RhythmEvents[this.nextConsumableEventIdx].Tick < tick - RhythmHelpers.INPUT_MAX_THRESHOLD) {
                this.nextConsumableEventIdx++;
            }
        }
        return result;
    }

    /// <summary>
    /// Stops playing the beatmap, and resets the beatmap to a non-started state
    /// </summary>
    public void Reset() => this.Reset(0);

    public long GetCurrentPosition() => (long) Math.Round(Bass.ChannelBytes2Seconds(this._songId, Bass.ChannelGetPosition(this._songId)) * TimeSpan.TicksPerSecond);

    /// <summary>
    /// Soft resets the beatmap and moves it to a specific tick
    /// </summary>
    private void Reset(long tick) {
        this.Playing = false;
        this.Paused = false;
        this.firstVisibleEventIdx = 0;
        this.nextVisibleEventIdx = 0;
        this.nextConsumableEventIdx = 0;
        Bass.ChannelStop(_songId);
        Bass.ChannelSetPosition(_songId, 0);
        foreach (RhythmEvent rhythmEvent in this._beatmap.RhythmEvents) {
            if (rhythmEvent.Tick > tick) {
                rhythmEvent.HitResult = BeatmapHitResult.NoHit;
            }
        }
    }
}

public class VisibleBeatmapEvents {
    public long Tick { get; set; }
    public IEnumerable<RhythmEvent>? RhythmEvents { get; set; }
}

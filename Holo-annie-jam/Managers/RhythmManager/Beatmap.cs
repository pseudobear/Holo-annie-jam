using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Represents a single beatmap, not including audio
/// </summary>
public class Beatmap {

    /// <summary>
    /// Immutable array of rhythm events representing the beatmap. This array is sorted.
    /// </summary>
    public ImmutableArray<RhythmEvent> RhythmEvents { get; }

    /// <summary>
    /// Song name of the song associated with the beatmap which can be loaded by passing the value to Content.Load
    /// </summary>
    public string SongName { get; }

    private Beatmap(List<RhythmEvent> rhythmEvents, string songName) {
        this.RhythmEvents = [.. rhythmEvents];
        this.SongName = songName;
    }

    private Beatmap(IEnumerable<RhythmEvent> rhythmEvents, string songName) {
        this.RhythmEvents = rhythmEvents.ToImmutableArray();
        this.SongName = songName;
    }

    /// <summary>
    /// Loads a beatmap from a binary file with the given filename
    /// </summary>
    public static Beatmap LoadFromFile(string filename) {
        using FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read);
        using BinaryReader reader = new(stream, Encoding.UTF8, false);

        int len = reader.ReadInt32();
        string songName = new(reader.ReadChars(len));

        List<RhythmEvent> rhythmEvents = [];

        try {
            while (true) {
                long tick = reader.ReadInt64();
                uint lane = reader.ReadUInt32();
                RhythmEventType type = (RhythmEventType) reader.ReadUInt32();
                uint value = type.HasValue() ? reader.ReadUInt32() : 0;
                rhythmEvents.Add(new RhythmEvent {
                    Tick = tick,
                    Lane = lane,
                    Type = type,
                    Value = value
                });
            }
        } catch (EndOfStreamException) { }

        return new(rhythmEvents, songName);
    }

    /// <summary>
    /// Writes a beatmap to a given file, overwriting its contents if it already exists
    /// </summary>
    public void WriteToFile(string filename) {
        using FileStream stream = File.Open(filename, FileMode.Create, FileAccess.Write);
        using BinaryWriter writer = new(stream, Encoding.UTF8, false);

        writer.Write(this.SongName.Length);
        writer.Write(this.SongName.ToCharArray());

        foreach (RhythmEvent rhythmEvent in this.RhythmEvents) {
            writer.Write(rhythmEvent.Tick);
            writer.Write(rhythmEvent.Lane);
            writer.Write((uint) rhythmEvent.Type);
            if (rhythmEvent.Type.HasValue()) {
                writer.Write(rhythmEvent.Value);
            }
        }
    }

    public class Builder {

        /// <summary>
        /// NOTE that the ticks unit here are DIFFERENT than the ticks used to represent time in a beatmap
        /// </summary>
        public const int TICKS_PER_BEAT = 960;

        /// <summary>
        /// List of rhythm events representing the beatmap being currently built. NOTE that the ticks unit used in the
        /// RhythmEvent objects here are DIFFERENT than the ticks used to represent time in a beatmap: here, a tick is
        /// 1/960th of a quarter note.
        /// </summary>
        public List<RhythmEvent> RhythmEvents { get; }

        public string SongName { get; }

        private readonly JsonSerializerOptions DefaultJsonSerializerOptions = new() {
            WriteIndented = true
        };

        private Builder(List<RhythmEvent> rhythmEvents, string songName) {
            this.RhythmEvents = rhythmEvents;
            this.SongName = songName;
        }

        public Builder(string songName) : this([], songName) { }

        /// <summary>
        /// Builds the beatmap
        /// </summary>
        public Beatmap Build() {
            if (this.RhythmEvents[0].Type != RhythmEventType.BpmChange) {
                throw new InvalidOperationException("first event must be a BPM event");
            }
            List<RhythmEvent> finalEvents = [];
            long currentTickTime = 0;
            long currentTickRhythm = 0;
            double minutesPerTick = 1.0 / this.RhythmEvents[0].Value / TICKS_PER_BEAT;
            foreach (RhythmEvent rhythmEvent in this.RhythmEvents.Skip(1)) {
                currentTickTime += (long) ((rhythmEvent.Tick - currentTickRhythm) * minutesPerTick * TimeSpan.TicksPerMinute);
                if (rhythmEvent.Type == RhythmEventType.BpmChange) {
                    minutesPerTick = 1.0 / rhythmEvent.Value / TICKS_PER_BEAT;
                } else {
                    finalEvents.Add(new RhythmEvent() {
                        Tick = currentTickTime,
                        Lane = rhythmEvent.Lane,
                        Type = rhythmEvent.Type,
                        Value = rhythmEvent.Value
                    });
                }
                currentTickRhythm = rhythmEvent.Tick;
            }
            return new Beatmap(finalEvents.OrderBy(e => e.Tick), this.SongName);
        }

        /// <summary>
        /// Loads a beatmap from a json file with the given filename
        /// </summary>
        public static Builder? LoadFromFile(string filename) => JsonSerializer.Deserialize<Builder>(File.ReadAllText(filename));

        /// <summary>
        /// Writes a beatmap to a given file, overwriting its contents if it already exists
        /// </summary>
        public void WriteToFile(string filename) => File.WriteAllText(filename, JsonSerializer.Serialize(this, DefaultJsonSerializerOptions));
    }
}

/// <summary>
/// Represents a single event in a beatmap
/// </summary>
public class RhythmEvent {

    /// <summary>
    /// The number of ticks (100 nanoseconds) after a specified point in time that the rhythm event occurs: this is
    /// usually the beginning of the beatmap, but can be any point in time depending on the context in which this
    /// object is used
    /// </summary>
    public long Tick { get; set; }

    /// <summary>
    /// The lane which the event is located
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public uint Lane { get; set; }

    /// <summary>
    /// The type of the event
    /// </summary>
	public RhythmEventType Type { get; set; }

    /// <summary>
    /// The value associated with the event, if the type requires it
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public uint Value { get; set; }

    [JsonIgnore]
    /// <summary>
    /// True if an input has already consumed this event, false otherwise
    /// </summary>
    public bool IsConsumed { get; set; } = false;
}

public enum RhythmEventType {
    Normal = 0,
    BpmChange = 1
}
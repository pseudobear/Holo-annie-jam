using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;

namespace Holo_annie_jam {

	/// <summary>
	/// Represents a single beatmap, not including audio
	/// </summary>
	public class Beatmap {

		/// <summary>
		/// The BPM of the beatmap. Not used in actual timing, but useful metadata.
		/// </summary>
		public uint BPM { get; }

		/// <summary>
		/// Immutable array of rhythm events representing the beatmap
		/// </summary>
        public ImmutableArray<RhythmEvent> RhythmEvents { get; }

        private Beatmap(uint bpm, List<RhythmEvent> rhythmEvents) {
			BPM = bpm;
            RhythmEvents = [.. rhythmEvents];
		}

		/// <summary>
		/// Loads a beatmap from a binary file with the given filename
		/// </summary>
		public static Beatmap LoadFromFile(string filename) {
			using FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read);
			using BinaryReader reader = new(stream, Encoding.UTF8, false);

            uint bpm = reader.ReadUInt32();
			List<RhythmEvent> rhythmEvents = [];

            try {
				while (true) {
					long tick = reader.ReadInt64();
					uint lane = reader.ReadUInt32();
					uint type = reader.ReadUInt32();
					rhythmEvents.Add(new RhythmEvent {
						Tick = tick,
						Lane = lane,
						Type = (RhythmEventType)type
					});
				}
			} catch (EndOfStreamException) { }

            Beatmap result = new(bpm, rhythmEvents);

            return result;
		}

        /// <summary>
        /// Writes a beatmap to a given file, overwriting its contents if it already exists
        /// </summary>
        public void WriteToFile(string filename) {
			using FileStream stream = File.Open(filename, FileMode.Create, FileAccess.Write);
			using BinaryWriter writer = new(stream, Encoding.UTF8, false);

			writer.Write(BPM);

            foreach (RhythmEvent rhythmEvent in RhythmEvents) {
                writer.Write(rhythmEvent.Tick);
				writer.Write(rhythmEvent.Lane);
				writer.Write((uint)rhythmEvent.Type);
            }
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
        public uint Lane { get; set; }

        /// <summary>
        /// The type of the event
        /// </summary>
		public RhythmEventType Type { get; set; }
	}

	// TODO add actual events
	public enum RhythmEventType {
		Event1, Event2
	}
}

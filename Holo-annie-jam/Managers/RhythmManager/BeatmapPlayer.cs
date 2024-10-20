using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Holo_annie_jam {
    public class BeatmapPlayer(Beatmap beatmap, Song song) {

        // TODO: make this a more reasonable number
        /// <summary>
        /// The timespan that is visible on the screen when playing a beatmap, in ticks (100 nanoseconds).
        /// For reference: 10,000,000 ticks is 1 second
        /// </summary>
        public const long EVENT_TICKS_VISIBLE = 20_000_000;

        private readonly Beatmap _beatmap = beatmap;
        private readonly Song _song = song;

        private bool playing = false;
        private long startTick;
        private bool paused = false;
        private long pauseStartTick = 0;
        private int firstVisibleEventIdx;
        private int nextVisibleEventIdx;

        /// <summary>
        /// Starts playing the beatmap
        /// </summary>
        public void Start() {
            if (!this.playing) {
				this.Reset();
				// delay start by the visible timespan, so that the beatmap always starts blank
				this.startTick = DateTime.UtcNow.Ticks + EVENT_TICKS_VISIBLE;
            }
        }

        /// <summary>
        /// Pauses the beatmap
        /// </summary>
        public void Pause() {
            if (!this.paused) {
				this.paused = true;
				this.pauseStartTick = DateTime.UtcNow.Ticks;
            }
        }

        /// <summary>
        /// Resumes the beatmap and gets an enumerable of currently visible rhythm events
        /// </summary>
        public IEnumerable<RhythmEvent> Resume() {
            if (this.paused) {
				this.paused = false;
				// hacky way to ensure beatmap starts where it left off
				this.startTick += DateTime.UtcNow.Ticks - this.pauseStartTick;
            }
            return this.Update();
        }

        /// <summary>
        /// Gets an enumerable of currently visible rhythm events
        /// </summary>
        public IEnumerable<RhythmEvent> Update() {
            long ticksElapsed = DateTime.UtcNow.Ticks - this.startTick;
            while (this._beatmap.RhythmEvents[this.firstVisibleEventIdx].Tick < ticksElapsed) {
				this.firstVisibleEventIdx++;
            }
            while (this._beatmap.RhythmEvents[this.nextVisibleEventIdx].Tick < ticksElapsed + EVENT_TICKS_VISIBLE) {
				this.nextVisibleEventIdx++;
            }
            return this._beatmap.RhythmEvents
                .Skip(this.firstVisibleEventIdx)
                .Take(this.nextVisibleEventIdx - this.firstVisibleEventIdx);
        }

        /// <summary>
        /// Resets the beatmap to a non-started state
        /// </summary>
        public void Reset() {
			this.playing = false;
			this.startTick = 0;
			this.firstVisibleEventIdx = 0;
			this.nextVisibleEventIdx = 0;
        }
	}
}

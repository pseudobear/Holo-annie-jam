import mido

from mido import MidiFile
from mido import MidiTrack

midi = MidiFile("song.mid")
json = open("song_beatmap.json", "w")
track = midi.tracks[0]
prev_note_on = 0
_32nd_note = midi.ticks_per_beat / 8
total_time = 0
for msg in track:
    total_time += msg.time
    tick = round(total_time / midi.ticks_per_beat * 960)
    if msg.type == "set_tempo":
        bpm = round(mido.tempo2bpm(msg.tempo))
        json.write(f'{{"Tick":{tick},"Type":1,"Value":{bpm}}},')
    elif msg.type == "note_on" and msg.velocity > 0 and prev_note_on < total_time - _32nd_note:
        lane = (msg.note % 3) + 1
        json.write(f'{{"Tick":{tick},"Lane":{lane},"Type":0}},')
        prev_note_on = total_time

json.close()
print("complete")

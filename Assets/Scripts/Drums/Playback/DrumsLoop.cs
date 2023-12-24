namespace LooperPooper.Drums.Playback
{
    public class DrumsLoop
    {
        public readonly DrumsLoopBar[] Bars;
        public readonly float BeatsPerMinute;
        public readonly int BeatsPerBar;

        public DrumsLoop(DrumsLoopBar[] bars, float beatsPerMinute)
        {
            Bars = bars;
            BeatsPerBar = bars[0].BeatsKick.Length;
            BeatsPerMinute = beatsPerMinute;
        }
    }
}
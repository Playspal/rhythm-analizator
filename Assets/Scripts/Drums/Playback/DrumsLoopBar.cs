namespace LooperPooper.Drums.Playback
{
    public class DrumsLoopBar
    {
        public readonly DrumsLoopBeat[] BeatsKick;
        public readonly DrumsLoopBeat[] BeatsSnare;

        public DrumsLoopBar(DrumsLoopBeat[] beatsKick, DrumsLoopBeat[] beatsSnare)
        {
            BeatsKick = beatsKick;
            BeatsSnare = beatsSnare;
        }
    }
}
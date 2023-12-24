namespace LooperPooper.Drums.Input
{
    public readonly struct DrumsInputEntry
    {
        public readonly DrumsBeatType Type;
        public readonly float Time;
        public readonly float Duration;
        
        public DrumsInputEntry(DrumsBeatType type, float time, float duration)
        {
            Type = type;
            Time = time;
            Duration = duration;
        }
    }
}
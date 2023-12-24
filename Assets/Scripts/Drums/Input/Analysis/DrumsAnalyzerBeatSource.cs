namespace LooperPooper.Drums.Input.Analysis
{
    public class DrumsAnalyzerBeatSource
    {
        public DrumsBeatType Type { get; set; }
        public float Time { get; set; }
        public float Duration { get; set; }
        public int Size { get; set; } = 1;
    }
}
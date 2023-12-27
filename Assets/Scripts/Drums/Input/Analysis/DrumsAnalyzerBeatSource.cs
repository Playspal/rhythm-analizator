namespace LooperPooper.Drums.Input.Analysis
{
    public class DrumsAnalyzerBeatSource
    {
        public readonly int Index;
        public DrumsBeatType Type { get; set; }
        public int Bar { get; set; } = 0;
        public float Time { get; set; }
        public float Duration { get; set; }
        public int Size { get; set; } = 1;

        public DrumsAnalyzerBeatSource(int index)
        {
            Index = index;
        }
    }
}
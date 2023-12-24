using LooperPooper.Drums.Input.Analysis.Comparison;

namespace LooperPooper.Drums.Input.Analysis
{
    public class DrumsAnalyzerBeat
    {
        public DrumsAnalyzerBeatSource Source;
        public DrumsAnalyzerBar Bar;

        public DrumsBeatType Type => Source.Type;
        public float Time => Source.Time - Bar.BeginTime;
        public float Duration => Source.Duration;
        public int Size => Source.Size;

        public DrumsAnalyzerBeat()
        {
        }

        public DrumsAnalyzerBeat(DrumsAnalyzerBeat source)
        {
            Source = source.Source;
            Bar = source.Bar;
        }
    }
}
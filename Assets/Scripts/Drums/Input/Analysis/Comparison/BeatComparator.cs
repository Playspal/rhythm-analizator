namespace LooperPooper.Drums.Input.Analysis.Comparison
{
    public class BeatComparator
    {
        public readonly bool IsEquals;

        public BeatComparator(DrumsAnalyzerBeat a, DrumsAnalyzerBeat b)
        {
            IsEquals = a.Type == b.Type &&
                       new TimeComparator(a.Time, b.Time).IsEquals &&
                       new TimeComparator(a.Duration, b.Duration).IsEquals;
        }
    }
}
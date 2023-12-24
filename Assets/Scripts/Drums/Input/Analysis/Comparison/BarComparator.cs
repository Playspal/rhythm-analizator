namespace LooperPooper.Drums.Input.Analysis.Comparison
{
    public class BarComparator
    {
        public readonly bool IsEquals;

        public BarComparator(DrumsAnalyzerBar a, DrumsAnalyzerBar b)
        {
            if (a.Beats.Count != b.Beats.Count)
            {
                IsEquals = false;
                return;
            }

            var equalBeatsCount = 0;

            for (var i = 0; i < a.Beats.Count; i++)
            {
                var beatA = a.Beats[i];
                var beatB = b.Beats[i];

                if (new BeatComparator(beatA, beatB).IsEquals)
                {
                    equalBeatsCount++;
                }
            }

            IsEquals = equalBeatsCount == a.Beats.Count;
        }
    }
}
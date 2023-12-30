namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public class ProcessCalculateBPM : IProcess<float>
    {
        private readonly DrumsInput _drumsInput;
        private readonly int _barsCount;
        
        public ProcessCalculateBPM(DrumsInput drumsInput, int barsCount)
        {
            _drumsInput = drumsInput;
            _barsCount = barsCount;
        }
        
        public float Process()
        {
            var quarter = _drumsInput.Duration / 4;

            return 60f / (quarter / _barsCount);
        }
    }
}
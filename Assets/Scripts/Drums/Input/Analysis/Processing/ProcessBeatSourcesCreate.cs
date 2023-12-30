using System.Collections.Generic;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public class ProcessBeatSourcesCreate : IProcess<List<DrumsAnalyzerBeatSource>>
    {
        private readonly DrumsInput _drumsInput;
        
        public ProcessBeatSourcesCreate(DrumsInput drumsInput)
        {
            _drumsInput = drumsInput;
        }
        
        public List<DrumsAnalyzerBeatSource> Process()
        {
            var output = new List<DrumsAnalyzerBeatSource>();
            
            var index = 0;
            
            foreach (var drumsInputEntry in _drumsInput.Entries)
            {
                output.Add(CreateBeatSource(drumsInputEntry, index));
                index++;
            }
            
            return output;
        }

        private static DrumsAnalyzerBeatSource CreateBeatSource(DrumsInputEntry drumsInputEntry, int index)
            => new(index, drumsInputEntry.Time, drumsInputEntry.Duration)
            {
                Type = drumsInputEntry.Type,
                Time = drumsInputEntry.Time,
                Duration = drumsInputEntry.Duration
            };
    }
}
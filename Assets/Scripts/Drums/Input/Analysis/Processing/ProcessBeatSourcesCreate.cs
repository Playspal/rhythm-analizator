using System.Collections.Generic;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    /// <summary>
    /// Convert DrumsInput entries in to DrumsAnalyzerBeatSource items
    /// </summary>
    public class ProcessBeatSourcesCreate : IProcess
    {
        private readonly DrumsInput _drumsInput;
        private readonly List<DrumsAnalyzerBeatSource> _beatSources;
        
        public ProcessBeatSourcesCreate(DrumsInput drumsInput, List<DrumsAnalyzerBeatSource> beatSources)
        {
            _drumsInput = drumsInput;
            _beatSources = beatSources;
        }
        
        public void Process()
        {
            var index = 0;
            foreach (var drumsInputEntry in _drumsInput.Entries)
            {
                _beatSources.Add(CreateBeatSource(drumsInputEntry, index));

                index++;
            }
        }

        private static DrumsAnalyzerBeatSource CreateBeatSource(DrumsInputEntry drumsInputEntry, int index)
            => new(index)
            {
                Type = drumsInputEntry.Type,
                Time = drumsInputEntry.Time,
                Duration = drumsInputEntry.Duration
            };
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public class ProcessValidate : IProcess<bool>
    {
        private readonly DrumsInput _drumsInput;
        private readonly List<DrumsAnalyzerBeatSource> _beatSources;
        
        public ProcessValidate(DrumsInput drumsInput, List<DrumsAnalyzerBeatSource> beatSources)
        {
            _drumsInput = drumsInput;
            _beatSources = beatSources;
        }
        
        public bool Process()
        {
            var error = 0f;
            
            for (var i = 0; i < _beatSources.Count; i++)
            {
                var a = _drumsInput.Entries[i];
                var b = _beatSources[i];

                error += Mathf.Abs(a.Time - b.Time);
            }

            Debug.LogError("Error: " + error + " / " + (error / _drumsInput.Duration));

            return error / _drumsInput.Duration < 0.175f;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public class ProcessBarsBalance : IProcess<bool>
    {
        private readonly List<DrumsAnalyzerBar> _bars;
        private readonly float _barDuration;
        private readonly int _totalBeatsCount;
        
        public ProcessBarsBalance(List<DrumsAnalyzerBar> bars, float barDuration,
            int totalBeatsCount)
        {
            _bars = bars;
            _barDuration = barDuration;
            _totalBeatsCount = totalBeatsCount;
        }
        
        public bool Process()
        {
            var operationTypes = Enum
                .GetValues(typeof(DrumsAnalyzerBarOperationType))
                .Cast<DrumsAnalyzerBarOperationType>();

            for (var j = 0; j < _totalBeatsCount; j++)
            {
                for (var i = 0; i < _bars.Count - 1; i++)
                {
                    var barA = _bars[i];
                    var barB = _bars[i + 1];
                    var barC = i < _bars.Count - 2 ? _bars[i + 2] : null;

                    foreach (var operationType in operationTypes)
                    {
                        var cloneA = new DrumsAnalyzerBar(barA);
                        var cloneB = new DrumsAnalyzerBar(barB);
                        var cloneC = barC == null ? null : new DrumsAnalyzerBar(barC);

                        var operationOnClones = new DrumsAnalyzerBarOperation(cloneA, cloneB, cloneC);

                        if (!operationOnClones.TryDo(operationType))
                        {
                            continue;
                        }

                        var isFirstBeatEqualsBefore = barA.Beats.Count > 0 &&
                                                      barB.Beats.Count > 0 &&
                                                      barA.Beats[0].Type == barB.Beats[0].Type;

                        var isFirstBeatEqualsAfter = cloneA.Beats.Count > 0 &&
                                                     cloneB.Beats.Count > 0 &&
                                                     cloneA.Beats[0].Type == cloneB.Beats[0].Type;

                        var deltaA = Mathf.Abs(_barDuration - barA.Duration);
                        var deltaCloneA = Mathf.Abs(_barDuration - cloneA.Duration);

                        if (isFirstBeatEqualsBefore == true && isFirstBeatEqualsAfter == true)
                        {
                            if (deltaCloneA < deltaA)
                            {
                                new DrumsAnalyzerBarOperation(barA, barB, barC).TryDo(operationType);
                            }
                        }

                        else if (isFirstBeatEqualsBefore == false && isFirstBeatEqualsAfter == true)
                        {
                            new DrumsAnalyzerBarOperation(barA, barB, barC).TryDo(operationType);
                        }

                        else if (isFirstBeatEqualsBefore == true && isFirstBeatEqualsAfter == false)
                        {

                        }
                        else
                        {
                            if (deltaCloneA < deltaA)
                            {
                                new DrumsAnalyzerBarOperation(barA, barB, barC).TryDo(operationType);
                            }
                        }
                    }
                }
            }

            return true;
        }
    }
}
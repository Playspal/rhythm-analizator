using System.Collections.Generic;
using UnityEngine;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public class ProcessBarsDebugPattern : IProcess
    {
        private readonly List<DrumsAnalyzerBar> _bars;
        private readonly int _timeSignature;
        private readonly string _title;
        
        public ProcessBarsDebugPattern(List<DrumsAnalyzerBar> bars, int timeSignature, string title)
        {
            _bars = bars;
            _timeSignature = timeSignature;
            _title = title;
        }
        
        public void Process()
        {
            Debug.Log("----------------");
            Debug.Log(_title);
            Debug.Log("BPM: " + Mathf.Round(_timeSignature / _bars[0].Duration * 60));
            
            _bars.ForEach(BarDebug);
        }
        
        private static void BarDebug(DrumsAnalyzerBar bar)
        {
            var debug = string.Empty;

            var s = 0;
            foreach (var beat in bar.Beats)
            {
                s += beat.Size;
            }

            debug += "[" + bar.Size + "] [" + s + "] ";
            
            foreach (var beat in bar.Beats)
            {
                debug += beat.Type == DrumsBeatType.Kick ? "K " : "S ";
                for (var i = 0; i < beat.Size - 1; i++)
                {
                    debug += "x ";
                }
            }
            
            Debug.Log(debug);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public class ProcessBarsDebug : IProcess
    {
        private readonly List<DrumsAnalyzerBar> _bars;
        private readonly string _title;
        
        public ProcessBarsDebug(List<DrumsAnalyzerBar> bars, string title)
        {
            _bars = bars;
            _title = title;
        }
        
        public void Process()
        {
            Debug.Log("----------------");
            Debug.Log(_title);
            
            _bars.ForEach(BarDebug);
        }
        
        private static void BarDebug(DrumsAnalyzerBar bar)
        {
            var debug = string.Empty;
        
            foreach (var beat in bar.Beats)
            {
                debug += "[ ";
                debug += FormatNote(beat.Type);
                debug += FormatTime(beat.Time);
                debug += " ] ";
            }
        
            Debug.Log(debug + " / " + bar.Size + " / " + bar.Duration);
        }

        private static string FormatNote(DrumsBeatType beat)
        {
            return beat == DrumsBeatType.Kick ? "K : " : "S : ";
        }
        
        private static string FormatTime(float time)
        {
            if (time == 0)
            {
                return "0.000";
            }
            
            var output = (Mathf.Round(time * 1000) / 1000).ToString();
            
            for (var i = output.Length; i < 5; i++)
            {
                output += "0";
            }

            return output;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using LooperPooper.Drums.Input.Analysis.Comparison;
using LooperPooper.Drums.Input.Analysis.Processing;
using LooperPooper.Drums.Playback;
using UnityEngine;

namespace LooperPooper.Drums.Input.Analysis
{
    public class DrumsAnalyzer
    {
        public DrumsLoop OutputDrumsLoop => _processGenerateOutput.OutputDrumsLoop;
        public float Error => _processCalculateError.Error;

        private readonly ProcessCalculateError _processCalculateError;
        private readonly ProcessGenerateOutput _processGenerateOutput;
        
        public DrumsAnalyzer(DrumsInput drumsInput, int timeSignature, int barSize, int barsCount, int attempt)
        {
            var beatSources = new List<DrumsAnalyzerBeatSource>();
            new ProcessBeatSourcesCreate(drumsInput, beatSources).Process();
            new ProcessBeatSourcesQuantize(beatSources, timeSignature, barSize, barsCount).Process();
            return;
            new ProcessBeatSourcesGroup(beatSources, timeSignature, barSize, barsCount).Process();
            
            
            _processGenerateOutput = new ProcessGenerateOutput(beatSources, timeSignature, barsCount);
            _processGenerateOutput.Process();
            
            _processCalculateError = new ProcessCalculateError(OutputDrumsLoop, drumsInput, beatSources, timeSignature, barSize, barsCount);
            _processCalculateError.Process();
            
            //

            for (var i = 0; i < barSize * barsCount; i++)
            {
                var t = i * (drumsInput.Duration / (barSize * barsCount));
                var x = 10 + Mathf.RoundToInt(t * 200);
                var y = 0;

                for (var yy = 0; yy < 5; yy++)
                {
                    Index.Texture.SetPixel(x, y + yy, Color.green);
                }
            }
            
            foreach (var entry in drumsInput.Entries)
            {
                var x = 10 + Mathf.RoundToInt(entry.Time * 200);
                var y = 10;

                for (var yy = 0; yy < (entry.Type == DrumsBeatType.Kick ? 10 : 20); yy++)
                {
                    Index.Texture.SetPixel(x, y + yy, Color.black);
                }
            }
            
            Index.Texture.SetPixel(Mathf.RoundToInt(drumsInput.Duration * 200), 10, Color.black);
            Index.Texture.SetPixel(Mathf.RoundToInt(drumsInput.Duration * 200) + 1, 10, Color.black);

            foreach (var beatSource in beatSources)
            {
                var x = 10 + Mathf.RoundToInt(beatSource.Time * 200);
                var y = 10 + 30 + attempt * 30;
                
                
                for (var yy = 0; yy < (beatSource.Type == DrumsBeatType.Kick ? 10 : 20); yy++)
                {
                    Index.Texture.SetPixel(x, y + yy, Color.blue);    
                }
            }
            
            Index.Texture.SetPixel(Mathf.RoundToInt(beatSources.Sum(beatSource => beatSource.Duration) * 200), 10 + 30 + attempt * 30, Color.blue);
            Index.Texture.SetPixel(Mathf.RoundToInt(beatSources.Sum(beatSource => beatSource.Duration) * 200) + 1, 10 + 30 + attempt * 30, Color.blue);
            
            

            
            var beatDuration = 60f / OutputDrumsLoop.BeatsPerMinute / (OutputDrumsLoop.BeatsPerBar / timeSignature);
            for (var i = 0; i < OutputDrumsLoop.Bars.Length; i++)
            {
                for (var j = 0; j < OutputDrumsLoop.Bars[i].BeatsKick.Length; j++)
                {
                    if(OutputDrumsLoop.Bars[i].BeatsKick[j].Type == DrumsBeatType.None && OutputDrumsLoop.Bars[i].BeatsSnare[j].Type == DrumsBeatType.None)
                    {
                        continue;
                    }
                    
                    var index = i * OutputDrumsLoop.Bars[i].BeatsKick.Length + j;
                    var time = beatDuration * index;
                    
                    var x = 10 + Mathf.RoundToInt(time * 200);
                    var y = 10 + 15 + attempt * 30;

                    for (var yy = 0; yy < 10; yy++)
                    {
                        Index.Texture.SetPixel(x, y + yy, Color.red);    
                    }
                }
            }
            
            Index.Texture.SetPixel(10 + Mathf.RoundToInt(beatDuration * barSize * barsCount * 200), 10 + 15 + attempt * 30, Color.red);
            Index.Texture.SetPixel(10 + Mathf.RoundToInt(beatDuration * barSize * barsCount * 200) + 1, 10 + 15 + attempt * 30, Color.red);
            
            Index.Texture.Apply();
        }

    }
}
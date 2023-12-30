using System.Linq;
using LooperPooper.Drums.Input.Analysis.Processing;
using LooperPooper.Drums.Playback;
using UnityEngine;

namespace LooperPooper.Drums.Input.Analysis
{
    public class DrumsAnalyzer
    {
        public DrumsLoop OutputDrumsLoop => _processGenerateOutput.OutputDrumsLoop;
        
        private readonly ProcessCalculateError _processCalculateError;
        private readonly ProcessGenerateOutput _processGenerateOutput;
        
        public DrumsAnalyzer(DrumsInput drumsInput, int timeSignature)
        {
            //new ProcessCalculateTest(drumsInput).Process();
            //Debug.LogError("----");
            //OutputTimeSignature = new ProcessCalculateTimeSignature(drumsInput).Process();
            
            var barsCount = new ProcessCalculateBarsCount(drumsInput, timeSignature).Process();
            var barSize = timeSignature * 4;
            var bpm = new ProcessCalculateBPM(drumsInput, barsCount).Process();
            
            Debug.Log("Time signature: " + timeSignature);
            Debug.Log("Bars count: " + barsCount);
            Debug.Log("Bar size: " + barSize);
            Debug.Log("BPM: " + bpm);

            var beatSources = new ProcessBeatSourcesCreate(drumsInput).Process();
            new ProcessBeatSourcesQuantize(beatSources, barSize, barsCount).Process();
            
            _processGenerateOutput = new ProcessGenerateOutput(beatSources, bpm, timeSignature, barSize, barsCount);
            _processGenerateOutput.Process();
            
            return;

            for (var i = 0; i < barSize * barsCount; i++)
            {
                var t = i * (drumsInput.Duration / (barSize * barsCount));
                var x = 10 + Mathf.RoundToInt(t * 200);
                var y = 0;

                var yyy = i % barSize == 0 ? 100 : 10;
                
                for (var yy = 0; yy < yyy; yy++)
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
                var y = 10 + 30 + 1 * 30;
                
                
                for (var yy = 0; yy < (beatSource.Type == DrumsBeatType.Kick ? 10 : 20); yy++)
                {
                    Index.Texture.SetPixel(x, y + yy, Color.blue);    
                }
            }
            
            Index.Texture.SetPixel(Mathf.RoundToInt(beatSources.Sum(beatSource => beatSource.Duration) * 200), 10 + 30 + 1 * 30, Color.blue);
            Index.Texture.SetPixel(Mathf.RoundToInt(beatSources.Sum(beatSource => beatSource.Duration) * 200) + 1, 10 + 30 + 1 * 30, Color.blue);
            
            Index.Texture.Apply();
        }
    }
}
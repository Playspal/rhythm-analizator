using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public class ProcessBarsQuantize : IProcess
    {
        private class Quantum
        {
            public Dictionary<DrumsBeatType, float> Magnets = new Dictionary<DrumsBeatType, float>()
            {
                {DrumsBeatType.Kick, 0},
                {DrumsBeatType.Snare, 0},
                {DrumsBeatType.None, 0},
            };
            
            public DrumsBeatType Beat = DrumsBeatType.None;

            public Quantum Left;
            public Quantum Right;

            public int Row;
            public int Column;
            
            public readonly int Index;
            public readonly float BoundMin;
            public readonly float BoundMax;

            public Quantum(int index, float boundMin, float boundMax)
            {
                Index = index;
                BoundMin = boundMin;
                BoundMax = boundMax;
            }

            public bool HitTest(float time)
            {
                return time >= BoundMin && time <= BoundMax;
            }
        }
        
        public ProcessBarsQuantize(List<DrumsAnalyzerBeatSource> beatSources, int timeSignature, int barsCount)
        {
            var accuracy = 8;
            var quantumsPerBar = timeSignature * 4;
            var quantumsTotal = quantumsPerBar * barsCount * accuracy;
            var quantumDuration = beatSources.Sum(beatSource => beatSource.Duration) / quantumsTotal;

            // Create quantums
            var quantums = new List<Quantum>();

            for (var i = 0; i < quantumsTotal; i++)
            {
                var quantum = new Quantum(i, quantumDuration * i, quantumDuration * (i + 1));
                quantums.Add(quantum);

                if (i > 0)
                {
                    quantum.Left = quantums[i - 1];
                    quantums[i - 1].Right = quantum;
                }
            }
            
            // Create quantums table
            var quantumsTable = new List<List<Quantum>>();

            for (var i = 0; i < barsCount; i++)
            {
                quantumsTable.Add(new List<Quantum>());
            }
            
            for (var i = 0; i < quantums.Count / barsCount; i++)
            {
                for (var j = 0; j < barsCount; j++)
                {
                    var index = i + j * quantums.Count / barsCount;
                    var quantum = quantums[index];

                    quantum.Row = j;
                    quantum.Column = i;
                    
                    quantumsTable[j].Add(quantum);
                }
            }
            
            // Set magnets
            foreach (var beatSource in beatSources)
            {
                foreach (var quantum in quantums)
                {
                    if (quantum.HitTest(beatSource.Time))
                    {
                        for (var i = 0; i < barsCount; i++)
                        {
                            var m = quantum.Index % (quantumsTotal / barsCount) == 0 ? barsCount : 1;
                            var weight = quantum.Column == 0 ? barsCount : m;

                            quantumsTable[i][quantum.Column].Magnets[beatSource.Type] += weight;
                        }
                        break;
                    }
                }
            }
            
            //
            foreach (var beatSource in beatSources)
            {
                foreach (var quantum in quantums)
                {
                    if (quantum.HitTest(beatSource.Time))
                    {
                        var indexFrom = Mathf.Max(0, quantum.Index - accuracy * 4);
                        var indexTo = Mathf.Min(quantumsTotal, quantum.Index + accuracy * 4);

                        var bestQuantum = quantum;
                        var bestQuantumWeight = quantum.Magnets[beatSource.Type];
                        
                        for (var i = indexFrom; i < indexTo; i++)
                        {
                            var distanceFromOriginalQuantum = Mathf.Abs(quantum.Index - i);
                            
                            var testQuantum = quantums[i];
                            var testQuantumWeight = testQuantum.Magnets[beatSource.Type];

                            if (testQuantumWeight <= 0 && distanceFromOriginalQuantum < accuracy)
                            {
                                var testQuantumWeightFallback = testQuantum.Magnets[
                                    beatSource.Type == DrumsBeatType.Kick
                                        ? DrumsBeatType.Snare
                                        : DrumsBeatType.Kick];

                                if (testQuantumWeightFallback > 0)
                                {
                                    testQuantumWeight = testQuantumWeightFallback / 2;
                                }
                            }

                            if (testQuantum.Beat != DrumsBeatType.None)
                            {
                                continue;
                            }

                            if (testQuantumWeight > bestQuantumWeight)
                            {
                                bestQuantum = testQuantum;
                                bestQuantumWeight = testQuantumWeight;
                            }
                        }

                        bestQuantum.Beat = beatSource.Type;
                        
                        break;
                    }
                }
            }

            var ds = new List<string>();

            for (var j = 0; j < barsCount; j++)
            {
                ds.Add(string.Empty);
            }

            for(var i = 0; i < quantums.Count / barsCount; i++)
            {
                var temp = new List<Quantum>();

                for (var j = 0; j < barsCount; j++)
                {
                    temp.Add(quantums[i + j * quantums.Count / barsCount]);
                }

                for (var j = 0; j < barsCount; j++)
                {
                    var quantum = temp[j];

                    //ds[j] += "[" + quantum.Magnets[LoopDrumsNoteType.Kick] + " / " +
//                             quantum.Magnets[LoopDrumsNoteType.Snare] +  "] ";

                    // ds[j] += quantum.Index + " ";

                    
                    switch (quantum.Beat)
                    {
                        case DrumsBeatType.None:
                            ds[j] += "x ";
                            break;
                    
                        case DrumsBeatType.Kick:
                            ds[j] += "K ";
                            break;
                    
                        case DrumsBeatType.Snare:
                            ds[j] += "S ";
                            break;
                    }
                    
                }

            }
            
            ds.ForEach(d => Debug.Log(d));
            
        }
        
        public void Process()
        {
            
        }
    }
}
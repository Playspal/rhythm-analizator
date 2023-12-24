using System.Collections;
using System.Collections.Generic;
using LooperPooper.Drums;
using LooperPooper.Drums.Playback;
using UnityEngine;

public class UiLoop : MonoBehaviour
{
    [SerializeField] private UiLoopBeat _beatPrefabA;
    [SerializeField] private UiLoopBeat _beatPrefabB;
    [SerializeField] private GameObject _separatorPrefab;
    [SerializeField] private RectTransform _container;

    private readonly List<UiLoopBeat> _beats = new List<UiLoopBeat>();
    private readonly List<GameObject> _separators = new List<GameObject>();
    private DrumsPlayer _drumsPlayer;
    
    public void SetDrumsPlayer(DrumsPlayer player)
    {
        _drumsPlayer = player;
        
        _beats.ForEach(beat => GameObject.Destroy(beat.gameObject));
        _beats.Clear();
        
        _separators.ForEach(separator => GameObject.Destroy(separator));
        _separators.Clear();
        
        //foreach (var bar in player.DrumsLoop.Bars)
        for(var i = 0; i < player.DrumsLoop.Bars.Length; i++)
        {
            var bar = player.DrumsLoop.Bars[i];

            var isA = true;
            var count = 0;
            //foreach (var beat in bar.Beats)
            for(var j = 0; j < bar.BeatsKick.Length; j++)
            {
                var beatKick = bar.BeatsKick[j];
                var beatSnare = bar.BeatsSnare[j];
                var beatInstance = Instantiate(isA ? _beatPrefabA : _beatPrefabB, _container);

                beatInstance.SetKickActive(beatKick.Type != DrumsBeatType.None);
                beatInstance.SetSnareActive(beatSnare.Type != DrumsBeatType.None);
                
                _beats.Add(beatInstance);
                count++;
                if (count == bar.BeatsKick.Length / 4)
                {
                    isA = !isA;
                    count = 0;
                }
            }

            if (i < player.DrumsLoop.Bars.Length - 1)
            {
                var separator = Instantiate(_separatorPrefab, _container);
                _separators.Add(separator);    
            }
        }
    }

    private void Update()
    {
        for (var i = 0; i < _beats.Count; i++)
        {
            _beats[i].SetSelectionActive(i == _drumsPlayer.Pointer);
        }
    }
}

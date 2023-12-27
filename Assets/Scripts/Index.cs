// https://github.com/Blueteak/Unity-Neural-Network/tree/master

using System;
using Cysharp.Threading.Tasks;
using LooperPooper.Drums;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Index : MonoBehaviour
{
    public static Texture2D Texture;
    
    [SerializeField] private AudioSource _kick;
    [SerializeField] private AudioSource _snare;
    [SerializeField] private AudioSource _hat;
    [SerializeField] private AudioSource _crash;
    [SerializeField] private UiLine _lineKick;
    [SerializeField] private UiLine _lineSnare;

    [SerializeField] private GameObject _blockInput;
    [SerializeField] private GameObject _blockPlayBack;
    
    [SerializeField] private UiButton _buttonKick;
    [SerializeField] private UiButton _buttonSnare;
    [SerializeField] private UiButton _buttonStop;
    [SerializeField] private UiButton _buttonReset;

    [SerializeField] private UiLoop _uiLoop;

    [SerializeField] private RawImage _debugTexture;
    
    private Drums _drums;

    private void Awake()
    {
        Texture = new Texture2D(1920, 600);
        _debugTexture.texture = Texture;
        
        AndroidNativeAudio.makePool();
        
        var kickId = AndroidNativeAudio.load("Sfx/kick.mp3");
        var snareId = AndroidNativeAudio.load("Sfx/snare.wav");
        var hatId = AndroidNativeAudio.load("Sfx/hat.wav");
        var crashId = AndroidNativeAudio.load("Sfx/crash.wav");
        
        Application.targetFrameRate = 60;
        
        _drums = new Drums(kickId, snareId, hatId, crashId);
        _drums.InputBegin();

        _buttonKick.OnPress += () => _drums.InsertKick();
        _buttonSnare.OnPress += () => _drums.InsertSnare();
        _buttonStop.OnPress += () =>
        {
            try
            {
                _drums.InputEnd();
                _uiLoop.SetDrumsPlayer(_drums.Player);
            }
            catch (Exception e)
            {
                _drums.InputBegin();
            }
        };

        _buttonReset.OnPress += () => _drums.InputBegin();
        
        //Record().Forget();
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _drums.InsertKick();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            _drums.InsertSnare();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            _drums.InputEnd();
            _uiLoop.SetDrumsPlayer(_drums.Player);
        }

        _drums.Update();
        
        _blockInput.SetActive(_drums.IsInput);
        _blockPlayBack.SetActive(!_drums.IsInput);
    }

    private async UniTask Record()
    {
        const float delay = 0.1f;
        await UniTask.Delay(TimeSpan.FromSeconds(1));

        _drums.InputBegin();
        
        for (var i = 0; i < 1; i++)
        {
            _drums.InsertKick(); 
            await DelayRandom(delay, 4);
            
            _drums.InsertKick();
            await DelayRandom(delay, 4);
            
            _drums.InsertKick();
            await DelayRandom(delay, 4);
            
            _drums.InsertSnare();
            await DelayRandom(delay, 2);
            
            _drums.InsertSnare();
            await DelayRandom(delay, 2);
            
            //
            
            
            _drums.InsertKick(); 
            await DelayRandom(delay, 4);
            
            _drums.InsertKick();
            await DelayRandom(delay, 4);
            
            await DelayRandom(delay, 4);
            
            _drums.InsertSnare();
            await DelayRandom(delay, 2);
            
            _drums.InsertSnare();
            await DelayRandom(delay, 2);
            
            
            //
            
            _drums.InsertKick(); 
            await DelayRandom(delay, 4);
            

            await DelayRandom(delay, 4);
            

            await DelayRandom(delay, 4);
            
            _drums.InsertSnare();
            await DelayRandom(delay, 2);
            
            _drums.InsertSnare();
            await DelayRandom(delay, 2);
            
            //
            
            
            _drums.InsertKick(); 
            await DelayRandom(delay, 4);
            
            _drums.InsertKick();
            await DelayRandom(delay, 4);
            
            _drums.InsertSnare();
            await DelayRandom(delay, 4);
            
            _drums.InsertSnare();
            await DelayRandom(delay, 4);
            
        }

        _drums.InputEnd();
        _uiLoop.SetDrumsPlayer(_drums.Player);
    }

    private async UniTask DelayRandom(float baseValue, int repeats)
    {
        var randomRangeMax = 0.2f;
        var randomRangeMin = 0.05f;

        //randomRangeMax = randomRangeMin = 0;
        
        var random = Random.Range(randomRangeMin, randomRangeMax) * (Random.Range(0f, 1f) > 0.5f ? -1f : 1f);
        await UniTask.Delay(TimeSpan.FromSeconds(baseValue * repeats + random));
    }


}


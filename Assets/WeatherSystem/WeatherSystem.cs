/*
Dhimant Vyas : 
Weather System :
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class WeatherSystem : MonoBehaviour
{
    [Header("Weather Effects : ")]
    public GameObject RainEffect;
    public GameObject SnowEffect;
    public GameObject LightningEffect;

    public int Particles;
    public enum weatherStates { SUNNY,                 //TO add a new Weather First add Your Weather State here.  Add Particle Effect Game Object if you have one and Lights and Sounds.
                                OVERCAST,
                                RAINY,
                                THUNDERSTORM,
                                SNOWING }


    [Header("Audio : ")]
    public GameObject audioSource;
    private AudioClip nextclip;

    [Header("Sound Clips : ")]
    public AudioClip Sunny ;
    public AudioClip overcast ;
    public AudioClip rainy ;
    public AudioClip thunderstorm ;


    [Header("Lighting : ")]
    public float overallGameLighttIntensity;
    public GameObject globalLigting2D;

    [Header("Weather Color and Light Intensity : ")]
    public Color sunny;
    [Range(0,1)]
    public float sunnyLight;
    public Color overcastlight;
    [Range(0, 1)]
    public float overcastLight;
    public Color rain;
    [Range(0, 1)]
    public float rainLight;
    public Color thunderstormcolor;
    [Range(0, 1)]
    public float thunderstormLight;
    public Color snow;
    [Range(0, 1)]
    public float snowLight;

    [Header("Weather Controller : ")]
    [Range(1, 1000)]
    public int changeRainIntensity;
    private int normalRainIntensity;
    [Range(1, 1000)]
    public int changeSnowIntensity;
    private int normalSnowIntensity;
    // Start is called before the first frame update

    [Header("Weather order : ")]
    public weatherStates[] weatherOrder;
    private int weatherOrderState;


    public float TransitionTimeforWeather;
    public float Transitionfromweatherduration;
    public bool changingWeatherbool;

    private float timePerWeather;
    public float changeTonewWeather;


    private int prevState;
    public weatherStates state = weatherStates.SUNNY;
    public bool changingWeather;
    public bool raining, snowing, thunderstorming;

   

    void Start()
    {
        Particles = 0;
        audioSource.GetComponent<AudioSource>().clip = Sunny;
        audioSource.GetComponent<AudioSource>().Play();
        globalLigting2D.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().color = sunny;
        prevState = 0;
        overallGameLighttIntensity = sunnyLight;  //Starting with Sunny Weather Here.
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!changingWeatherbool)  // Are we changing the Weather? if Not Lets wait for the time to complete Current Weather and then lets Change it.
        {
            //Debug.Log("NOT Changing Weather");

            timePerWeather += Time.deltaTime;
            if (timePerWeather > changeTonewWeather)
            {
                timePerWeather = 0;
                changeWeather();   //We have waited for the Time and now its Time to change the Weather.
                changingWeather = true;   
            }
            if (audioSource.GetComponent<AudioSource>().volume < 1)  // For not sudden Audio we increase the audio slowly.
            {
                audioSource.GetComponent<AudioSource>().volume += 0.03f;
                
            }
            if (globalLigting2D.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity < overallGameLighttIntensity)
            {
                globalLigting2D.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity += 0.001f;  // For not sudden Light we increase the Light slowly.
            }
        }

       
        if (changingWeatherbool) // SO we changing the Weather. DO this in that Time.
        {
            //Debug.Log("Changing Weather");
            TransitionTimeforWeather += Time.deltaTime;  // This is the time it will take to change the weather.
            if(TransitionTimeforWeather > Transitionfromweatherduration)
            {
                TransitionTimeforWeather = 0;
                changingWeatherbool = false;
                changingWeather = false;
            }

            if(!raining && !snowing  && !thunderstorming)  // We need to change weather slowly from one to another so lets do it 5 particles at a Time. 
            {
                if (Particles > 0) Particles -= 5;
                ParticleSystem particleSys = RainEffect.GetComponent<ParticleSystem>();
                var main = particleSys.main;
                main.maxParticles = Particles;

                particleSys = SnowEffect.GetComponent<ParticleSystem>();
                main = particleSys.main;
                main.maxParticles = Particles;
            }
            if (thunderstorming)  // Lets increase particles slowly too.
            {
                Particles += 5;
                ParticleSystem particlesys = LightningEffect.GetComponent<ParticleSystem>();
                var main = particlesys.main;
                main.maxParticles = Particles;
                overallGameLighttIntensity = Mathf.Lerp(1.0f, 0.25f, 1.0f);

            }
            if (raining)
            {
                if(Particles < normalRainIntensity)
                {
                    Particles += 5;
                    ParticleSystem particlesys = RainEffect.GetComponent<ParticleSystem>();
                    var main = particlesys.main;
                    main.maxParticles = Particles;
                }
            }
            if (snowing)
            {
                if (Particles < normalSnowIntensity)
                {
                    Particles += 5;
                    ParticleSystem particlesys = SnowEffect.GetComponent<ParticleSystem>();
                    var main = particlesys.main;
                    main.maxParticles = Particles;
                }
            }
            if (audioSource.GetComponent<AudioSource>().volume > 0)  // Before changing the volume decrease it and then Increase it.
            {
                audioSource.GetComponent<AudioSource>().volume -= 0.03f;
               
            }
            if(globalLigting2D.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity > 0.5f) // Same for Lightining.
            {
                globalLigting2D.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity -= 0.001f;
            }
            else
            {
                audioSource.GetComponent<AudioSource>().clip = nextclip;
                audioSource.GetComponent<AudioSource>().Play();
            }
        }
    }



    private void changeWeather()
    {
        changingWeatherbool = true;  // We are making an Array for the Weather States and whatever weather is in that state willl come next.
        if(weatherOrderState == weatherOrder.Length - 1)
        {
            weatherOrderState = 0;
        }
        else
        {
            weatherOrderState += 1;
        }

        var random = UnityEngine.Random.Range(0,4);  // Just to make it bit Dynamic if we have Thunderstorm or Rainy weather, Anything Can happen.
        if (prevState == 2)
        {
            state = weatherOrder[random];
        }
        if (prevState == 3)
        {
            state = weatherOrder[random];
        }
        else {
            state = weatherOrder[weatherOrderState];
        }

        switch (state) // We stet things for Our weathers here. 
        {
            case weatherStates.SUNNY:
                raining = false;
                snowing = false;
                thunderstorming = false;
                normalRainIntensity = 0;
                nextclip = Sunny;
                globalLigting2D.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().color = sunny;
                overallGameLighttIntensity = sunnyLight;
                prevState = 0;
                RainEffect.SetActive(false);
                SnowEffect.SetActive(false);
                LightningEffect.SetActive(false);
                break;
            case weatherStates.OVERCAST:
                raining = false;
                snowing = false;
                thunderstorming = false;
                normalRainIntensity = 0;
                nextclip = overcast;
                globalLigting2D.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().color = overcastlight;
                prevState = 1;
                RainEffect.SetActive(false);
                SnowEffect.SetActive(false);
                LightningEffect.SetActive(false);
                overallGameLighttIntensity = overcastLight;

                break;
            case weatherStates.RAINY:
                raining = true;
                snowing = false;
                thunderstorming = false;
                Particles = 0;
                RainEffect.SetActive(true);
                SnowEffect.SetActive(false);
                LightningEffect.SetActive(false);
                nextclip = rainy;
                normalRainIntensity = 300;
                globalLigting2D.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().color = rain;
                prevState = 2;
                overallGameLighttIntensity = rainLight;

                break;
            case weatherStates.THUNDERSTORM:
                raining = true;
                snowing = false;
                thunderstorming = true;
                Particles = 0;
                RainEffect.SetActive(true);
                SnowEffect.SetActive(false);
                LightningEffect.SetActive(true);
                nextclip = thunderstorm;
                normalRainIntensity = 1000;
                globalLigting2D.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().color = thunderstormcolor;
                prevState = 3;
                overallGameLighttIntensity = thunderstormLight;

                break;
            case weatherStates.SNOWING:
                raining = false;
                snowing = true;
                thunderstorming = false;
                Particles = 0;
                normalSnowIntensity = 600;
                SnowEffect.SetActive(true);
                RainEffect.SetActive(false);
                LightningEffect.SetActive(false);
                nextclip = overcast;
                globalLigting2D.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().color = snow;
                prevState = 4;
                overallGameLighttIntensity = snowLight;

                break;

            default: 
                break;
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

sealed public class AudioManager : MonoBehaviour
{
    sealed public class AudioFader
    {
        private const float min = 0.0001f;
        private const float max = 1f;

        private Coroutine       fadeInCoroutine,
                                fadeOutCoroutine;
        private float           volume = 1f;

        private MonoBehaviour Mono { get; }
        private AudioMixer Mixer { get; }
        public float Volume
        {
            get => volume;

            set
            {
                volume = Mathf.Clamp(value, min, max); ;
                Mixer.SetFloat("Master Volume", Mathf.Log10(volume) * 20f);
            }
        } 

        public bool IsFadingIn { get => fadeInCoroutine != null; }
        public bool IsFadingOut { get => fadeOutCoroutine != null; }

        public AudioFader(MonoBehaviour mono, AudioMixer mixer)
        {
            Mono = mono;
            Mixer = mixer;
        }

        public void FadeIn (float speed = 1f)
        {
            if (fadeOutCoroutine != null)
            {
                Mono.StopCoroutine(fadeOutCoroutine);
                fadeOutCoroutine = null;
            }

            if (fadeInCoroutine != null)
                Mono.StopCoroutine(fadeInCoroutine);

            fadeInCoroutine = Mono.StartCoroutine(FadeInVolume(speed));
        }

        public void FadeOut(float speed = 1f)
        {
            if (fadeInCoroutine != null)
            {
                Mono.StopCoroutine(fadeInCoroutine);
                fadeInCoroutine = null;
            }

            if (fadeOutCoroutine != null)
                Mono.StopCoroutine(fadeOutCoroutine);

            fadeOutCoroutine = Mono.StartCoroutine(FadeOutVolume(speed));
        }

        private IEnumerator FadeInVolume (float speed)
        {
            speed = Mathf.Max(speed, 0.001f);
            Mixer.GetFloat("Master Volume", out float tempVolume);
            tempVolume = Mathf.Pow(10, tempVolume / 20f);

            while (tempVolume < volume)
            {
                tempVolume += (max - min) * Time.unscaledDeltaTime * speed;
                Mixer.SetFloat("Master Volume",
                                     Mathf.Log10(tempVolume) * 20f);

                yield return null;
            }
            Mixer.SetFloat("Master Volume", Mathf.Log10(volume) * 20f);

            fadeInCoroutine = null;
        }

        private IEnumerator FadeOutVolume (float speed)
        {
            speed = Mathf.Max(speed, 0.001f);
            Mixer.GetFloat("Master Volume", out float volume);
            volume = Mathf.Pow(10, volume / 20f);

            while (volume > min)
            {
                volume -= (max - min) * Time.unscaledDeltaTime * speed;
                Mixer.SetFloat("Master Volume",
                                     Mathf.Log10(volume) * 20f);

                yield return null;
            }
            Mixer.SetFloat("Master Volume", Mathf.Log10(min) * 20f);

            fadeOutCoroutine = null;
        }
    }

    public enum Channel : byte
    {
        Master  = 0,
        FX      = 1,
        Voice   = 2,
        UI      = 3,
    }

    [SerializeField] private AudioMixer masterMixer = null;
    [SerializeField] private AudioMixer uiMixer = null;
    [SerializeField] private AudioMixer fxMixer = null;
    [SerializeField] private AudioMixer voiceMixer = null;

    private AudioFader MasterFader { get; set; }
    private AudioFader FxFader { get; set; }
    private AudioFader UiFader { get; set; }
    private AudioFader VoiceFader { get; set; }

    private void Awake()
    {
        MasterFader     = new AudioFader(this, masterMixer);
        FxFader         = new AudioFader(this, fxMixer);
        UiFader         = new AudioFader(this, uiMixer);
        VoiceFader      = new AudioFader(this, voiceMixer);
    }

    public void SetVolume(Channel channel, float volume)
    {
        switch (channel)
        {
            case Channel.Master:
                MasterFader.Volume = volume;
                break;
            case Channel.FX:
                FxFader.Volume = volume;
                break;
            case Channel.Voice:
                VoiceFader.Volume = volume;
                break;
            case Channel.UI:
                UiFader.Volume = volume;
                break;
        }
    }

    public float GetVolume(Channel channel)
    {
        switch (channel)
        {
            case Channel.Master:
                return MasterFader.Volume;
            case Channel.FX:
                return FxFader.Volume;
            case Channel.Voice:
                return VoiceFader.Volume;
            case Channel.UI:
                return UiFader.Volume;
        }

        return 0f;
    }

    public void FadeOut(Channel channel, float speed = 1f)
    {
        switch (channel)
        {
            case Channel.Master:
                MasterFader.FadeOut(speed);
                break;
            case Channel.FX:
                FxFader.FadeOut(speed);
                break;
            case Channel.Voice:
                VoiceFader.FadeOut(speed);
                break;
            case Channel.UI:
                UiFader.FadeOut(speed);
                break;
        }
    }

    public void FadeIn(Channel channel, float speed = 1f)
    {
        switch (channel)
        {
            case Channel.Master:
                MasterFader.FadeIn(speed);
                break;
            case Channel.FX:
                FxFader.FadeIn(speed);
                break;
            case Channel.Voice:
                VoiceFader.FadeIn(speed);
                break;
            case Channel.UI:
                UiFader.FadeIn(speed);
                break;
        }
    }

    public bool IsFadingIn(Channel channel)
    {
        switch (channel)
        {
            case Channel.Master:
                return MasterFader.IsFadingIn;
            case Channel.FX:
                return FxFader.IsFadingIn;
            case Channel.Voice:
                return VoiceFader.IsFadingIn;
            case Channel.UI:
                return UiFader.IsFadingIn;
        }

        return false;
    }

    public bool IsFadingOut(Channel channel)
    {
        switch (channel)
        {
            case Channel.Master:
                return MasterFader.IsFadingOut;
            case Channel.FX:
                return FxFader.IsFadingOut;
            case Channel.Voice:
                return VoiceFader.IsFadingOut;
            case Channel.UI:
                return UiFader.IsFadingOut;
        }

        return false;
    }
}
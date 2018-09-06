using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [System.Serializable]
    public class Sound
    {
        public AudioClip clip;
        [HideInInspector]
        public int simultaneousPlayCount = 0;
    }

    [Header("允许同时播放相同音频的最大数量")]
    public int maxSimultaneousSounds = 7;


    public Sound button;
    public Sound gems;
    public Sound score;
    public Sound jump;
    public Sound gameOver;
    public Sound tick;
    public Sound rewarded;

    public delegate void OnMuteStatusChanged(bool isMuted);

    public static event OnMuteStatusChanged MuteStatusChanged;

    public delegate void OnMusicStatusChanged(bool isOn);

    public static event OnMusicStatusChanged MusicStatusChanged;

    enum PlayingState
    {
        Playing,
        Paused,
        Stopped
    }

    private AudioSource audioSource;
    private PlayingState musicState = PlayingState.Stopped;
    private const string MUTE_PREF_KEY = "MutePreference";
    private const int MUTED = 0;
    private const int UN_MUTED = 1;
    private const string MUSIC_PREF_KEY = "MutePreference";
    private const int MUSIC_OFF = 0;
    private const int MUSIC_ON = 1;


    void Awake()
    {
        if (Instance)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // 根据存储在PlayerPrefs中的值设置是否静音
        SetMute(IsMuted());
    }

    /// <summary>
    /// 根据设置播放指定的声音同时降低正在播放的相同音频的音量，以防叠加产生爆音
    /// </summary>
    /// <param name="sound">音频.</param>
    /// <param name="autoScaleVolume">如果设置为<c>true</c>自动降低正在播放的相同音频的音量.</param>
    /// <param name="maxVolumeScale">降低音量前的最高音量.</param>
    public void PlaySound(Sound sound, bool autoScaleVolume = true, float maxVolumeScale = 1f)
    {
        StartCoroutine(CRPlaySound(sound, autoScaleVolume, maxVolumeScale));
    }

    IEnumerator CRPlaySound(Sound sound, bool autoScaleVolume = true, float maxVolumeScale = 1f)
    {
        if (sound.simultaneousPlayCount >= maxSimultaneousSounds)
        {
            yield break;
        }

        sound.simultaneousPlayCount++;

        float vol = maxVolumeScale;

        if (autoScaleVolume && sound.simultaneousPlayCount > 0)
        {
            vol = vol / (float)(sound.simultaneousPlayCount);
        }

        audioSource.PlayOneShot(sound.clip, vol);

        // 音频播放解锁减小同时播放计数
        float delay = sound.clip.length * 0.7f;

        yield return new WaitForSeconds(delay);

        sound.simultaneousPlayCount--;
    }

    /// <summary>
    /// 播放指定音乐
    /// </summary>
    /// <param name="music">Music.</param>
    /// <param name="loop">如果设置为<c>true</c>则循环播放.</param>
    public void PlayMusic(Sound music, bool loop = true)
    {
        if (IsMusicOff())
        {
            return;
        }

        audioSource.clip = music.clip;
        audioSource.loop = loop;
        audioSource.Play();
        musicState = PlayingState.Playing;
    }

    /// <summary>
    /// 暂停音乐
    /// </summary>
    public void PauseMusic()
    {
        if (musicState == PlayingState.Playing)
        {
            audioSource.Pause();
            musicState = PlayingState.Paused;
        }
    }

    /// <summary>
    /// 恢复播放音乐
    /// </summary>
    public void ResumeMusic()
    {
        if (musicState == PlayingState.Paused)
        {
            audioSource.UnPause();
            musicState = PlayingState.Playing;
        }
    }

    /// <summary>
    /// 停止播放音乐
    /// </summary>
    public void StopMusic()
    {
        audioSource.Stop();
        musicState = PlayingState.Stopped;
    }

    /// <summary>
    /// 判断是否静音
    /// </summary>
    /// <returns><c>true</c> 如果静音; 否则, <c>false</c>.</returns>
    public bool IsMuted()
    {
        return (PlayerPrefs.GetInt(MUTE_PREF_KEY, UN_MUTED) == MUTED);
    }

    public bool IsMusicOff()
    {
        return (PlayerPrefs.GetInt(MUSIC_PREF_KEY, MUSIC_ON) == MUSIC_OFF);
    }

    /// <summary>
    /// 切换音效静音状态
    /// </summary>
    public void ToggleMute()
    {
        bool mute = !IsMuted();

        if (mute)
        {
            // 静音了
            PlayerPrefs.SetInt(MUTE_PREF_KEY, MUTED);

            if (MuteStatusChanged != null)
            {
                MuteStatusChanged(true);
            }
        }
        else
        {
            // 没有静音
            PlayerPrefs.SetInt(MUTE_PREF_KEY, UN_MUTED);

            if (MuteStatusChanged != null)
            {
                MuteStatusChanged(false);
            }
        }

        SetMute(mute);
    }

    /// <summary>
    /// 切换音乐静音状态
    /// </summary>
    public void ToggleMusic()
    {
        if (IsMusicOff())
        {
            // 打开音乐
            PlayerPrefs.SetInt(MUSIC_PREF_KEY, MUSIC_ON);
            if (musicState == PlayingState.Paused)
            {
                ResumeMusic();
            }

            if (MusicStatusChanged != null)
            {
                MusicStatusChanged(true);
            }
        }
        else
        {
            // 关闭音乐
            PlayerPrefs.SetInt(MUSIC_PREF_KEY, MUSIC_OFF);
            if (musicState == PlayingState.Playing)
            {
                PauseMusic();
            }

            if (MusicStatusChanged != null)
            {
                MusicStatusChanged(false);
            }
        }
    }

    void SetMute(bool isMuted)
    {
        audioSource.mute = isMuted;
    }
}
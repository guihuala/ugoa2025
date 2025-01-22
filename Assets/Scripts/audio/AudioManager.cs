using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AudioInfo
{
    public string audioName;

    public AudioSource audioSource;
}
public class AudioManager : SingletonPersistent<AudioManager>
{
    // ��������BGM����Ƶ��Ϣ
    public List<AudioInfo> bgmAudioInfoList;

    // ��������SFX����Ƶ��Ϣ
    public List<AudioInfo> sfxAudioInfoList;

    // ��Ƶ�����ȫ������
    public float mainVolume;

    // BGM���������ӣ��������� = mainVolume * bgmVolumeFactor
    public float bgmVolumeFactor;

    // SFX���������ӣ��������� = mainVolume * sfxVolumeFactor
    public float sfxVolumeFactor;

    // ������Ƶ���ݵ���Դ
    public AudioDatas audioDatas;

    private GameObject _bgmSourcesRootGO;

    private GameObject _sfxSourcesRootGO;

    protected override void Awake()
    {
        base.Awake();

        // ����BGM��SFX��AudioSource��������
        _bgmSourcesRootGO = new GameObject("BGM_ROOT");

        _sfxSourcesRootGO = new GameObject("SFX_ROOT");

        _bgmSourcesRootGO.transform.SetParent(transform);

        _sfxSourcesRootGO.transform.SetParent(transform);
    }

    /// <summary>
    /// ����BGM
    /// </summary>
    /// <param name="fadeInMusicName">��Ҫ�������������</param>
    /// <param name="fadeOutMusicName">��Ҫ�������������ƣ�Ĭ��ֵΪ���ַ���</param>
    /// <param name="fadeInDuration">�������ʱ��</param>
    /// <param name="fadeOutDuration">��������ʱ��</param>
    /// <param name="loop">�Ƿ�ѭ������</param>
    public void PlayBgm(string fadeInMusicName, string fadeOutMusicName = "", float fadeInDuration = 0.5f, float fadeOutDuration = 0.5f, bool loop = true)
    {
        // ����DoTween���У����ڲ��Ź��ɶ���
        Sequence s = DOTween.Sequence();

        // �����Ҫ����ĳ��BGM
        if (fadeOutMusicName != "")
        {
            AudioInfo fadeOutInfo = bgmAudioInfoList.Find(x => x.audioName == fadeOutMusicName);

            if (fadeOutInfo == null)
            {
                Debug.LogWarning("��ǰδ����" + fadeOutMusicName + ",�޷����е�������");
                return;
            }

            s.Append(fadeOutInfo.audioSource.DOFade(0, fadeOutDuration).OnComplete(() =>
            {
                fadeOutInfo.audioSource.Pause();
            }));
        }

        // �����Ҫ���ŵ������Ѿ����ڣ���ֱ�ӻָ�����
        AudioInfo audioInfo = bgmAudioInfoList.Find(x => x.audioName == fadeInMusicName);

        if (audioInfo != null)
        {
            s.Append(audioInfo.audioSource.DOFade(mainVolume * bgmVolumeFactor, fadeInDuration).OnComplete(() =>
            {
                audioInfo.audioSource.Play();
            }));

            return;
        }

        // ����Դ�в�����Ҫ���ŵ���������
        AudioData fadeInData = audioDatas.audioDataList.Find(x => x.audioName == fadeInMusicName);

        if (fadeInData == null)
        {
            Debug.LogWarning("��Ƶ��ԴSO��δ�ҵ�����Ϊ" + fadeInMusicName + "����Ƶ����");
            return;
        }

        // ��̬����һ��GameObject�����AudioSource��������Ƶ��Դ
        GameObject fadeInAudioGO = new GameObject(fadeInMusicName);

        fadeInAudioGO.transform.SetParent(_bgmSourcesRootGO.transform);

        AudioSource fadeInAudioSource = fadeInAudioGO.AddComponent<AudioSource>();

        fadeInAudioSource.clip = Resources.Load<AudioClip>(fadeInData.audioPath);

        fadeInAudioSource.loop = loop;

        // ������Ч������
        fadeInAudioSource.volume = mainVolume * bgmVolumeFactor;

        fadeInAudioSource.Play();
        
        if (fadeInDuration > 0)
        {
            fadeInAudioSource.volume = 0;
            s.Append(fadeInAudioSource.DOFade(mainVolume * bgmVolumeFactor, fadeInDuration));
        }
        AudioInfo info = new AudioInfo();

        // ���´�����BGM��Ϣ��ӵ��б�
        info.audioName = fadeInMusicName;

        info.audioSource = fadeInAudioSource;

        bgmAudioInfoList.Add(info);

        StartCoroutine(DetectingAudioPlayState(info, true));
    }


    /// <summary>
    /// ��ͣBGM
    /// </summary>
    /// <param name="pauseBgmName">��Ҫ��ͣ��BGM����</param>
    /// <param name="fadeOutDuration">��������ʱ��</param>
    public void PauseBgm(string pauseBgmName, float fadeOutDuration = 0.5f)
    {
        AudioInfo audioInfo = bgmAudioInfoList.Find(x => x.audioName == pauseBgmName);

        if (audioInfo == null)
        {
            Debug.LogWarning("��ǰδ����" + pauseBgmName + ",�޷�����ֹͣ����");
            return;
        }

        Sequence s = DOTween.Sequence();

        s.Append(audioInfo.audioSource.DOFade(0, fadeOutDuration).OnComplete(() =>
        {
            audioInfo.audioSource.Pause();
        }));
    }


    /// <summary>
    /// ֹͣBGM
    /// </summary>
    /// <param name="stopBgmName">��Ҫֹͣ��BGM����</param>
    /// <param name="fadeOutDuration">��������ʱ��</param>
    public void StopBgm(string stopBgmName, float fadeOutDuration = 0.5f)
    {
        AudioInfo audioInfo = bgmAudioInfoList.Find(x => x.audioName == stopBgmName);

        if (audioInfo == null)
        {
            Debug.LogWarning("��ǰδ����" + stopBgmName + ",�޷�����ֹͣ����");
            return;
        }

        Sequence s = DOTween.Sequence();

        s.Append(audioInfo.audioSource.DOFade(0, fadeOutDuration).OnComplete(() =>
        {
            audioInfo.audioSource.Stop();

            Destroy(audioInfo.audioSource.gameObject);
        }));

        bgmAudioInfoList.Remove(audioInfo);

    }
    
    /// <summary>
    /// ֹͣ��������
    /// </summary>
    /// <param name="fadeOutDuration">��������ʱ��</param>
    public void StopAllBGM(float fadeOutDuration = 0.5f)
    {
        // ֹͣ���� BGM
        foreach (var bgmInfo in bgmAudioInfoList.ToArray())
        {
            StopBgm(bgmInfo.audioName, fadeOutDuration);
        }
        StopAllCoroutines();
    }



    /// <summary>
    /// ������Ч
    /// </summary>
    /// <param name="sfxName">��Ҫ���ŵ���Ч����</param>
    /// <param name="fadeInDuration">��Ч����ʱ��</param>
    /// <param name="loop">�Ƿ�ѭ������</param>
    public void PlaySfx(string sfxName, float fadeInDuration = 0, bool loop = false)
    {
        Sequence s = DOTween.Sequence();

        // ����Դ�в�����Ҫ���ŵ���Ч����
        AudioData sfxData = audioDatas.audioDataList.Find(x => x.audioName == sfxName);

        if (sfxData == null)
        {
            Debug.LogWarning("��Ƶ��ԴSO��δ�ҵ�����Ϊ" + sfxName + "����Ƶ����");
            return;
        }


        // ��̬����һ��GameObject�����AudioSource��������Ч��Դ
        GameObject sfxAudioGO = new GameObject(sfxName);

        sfxAudioGO.transform.SetParent(_sfxSourcesRootGO.transform);

        AudioSource sfxAudioSource = sfxAudioGO.AddComponent<AudioSource>();

        sfxAudioSource.clip = Resources.Load<AudioClip>(sfxData.audioPath);

        sfxAudioSource.loop = loop;

        // ������Ч������
        sfxAudioSource.volume = mainVolume * sfxVolumeFactor;

        sfxAudioSource.Play();
        
        if (fadeInDuration > 0)
        {
            sfxAudioSource.volume = 0;
            s.Append(sfxAudioSource.DOFade(mainVolume * sfxVolumeFactor, fadeInDuration));
        }
        
        AudioInfo info = new AudioInfo();

        // ����Ч��Ϣ��ӵ��б�
        info.audioName = sfxName;

        info.audioSource = sfxAudioSource;

        sfxAudioInfoList.Add(info);

        StartCoroutine(DetectingAudioPlayState(info, false));
        //ThreadPool.QueueUserWorkItem(new WaitCallback(DetectingAudioPlayState), sfxAudioGO);//?????????????????????????
    }

    /// <summary>
    /// ?????��
    /// </summary>
    /// <param name="pauseSfxName">???????��??</param>
    public void PauseSfx(string pauseSfxName)
    {
        AudioInfo audioInfo = sfxAudioInfoList.Find(x => x.audioName == pauseSfxName);

        if (audioInfo == null)
        {
            Debug.LogWarning("?????��????" + pauseSfxName + ",??????????��");
            return;
        }

        audioInfo.audioSource.Pause();
    }


    /// <summary>
    /// ????��
    /// </summary>
    /// <param name="stopSfxName">??????��??</param>
    public void StopSfx(string stopSfxName)
    {
        AudioInfo audioInfo = bgmAudioInfoList.Find(x => x.audioName == stopSfxName);

        if (audioInfo == null)
        {
            Debug.LogWarning("?????��????" + stopSfxName + ",???????????��");
            return;
        }

        audioInfo.audioSource.Stop();

        bgmAudioInfoList.Remove(audioInfo);

        Destroy(audioInfo.audioSource.gameObject);
    }


    /// <summary>
    /// ?????????
    /// </summary>
    /// <param name="volume"></param>
    public void ChangeMainVolume(float volume)
    {
        mainVolume = volume;

        foreach (var info in bgmAudioInfoList)
        {
            info.audioSource.volume = mainVolume * bgmVolumeFactor;

        }
        foreach (var info in sfxAudioInfoList)
        {
            info.audioSource.volume = mainVolume * sfxVolumeFactor;
        }
    }

    /// <summary>
    /// ???Bgm????????
    /// </summary>
    /// <param name="factor"></param>
    public void ChangeBgmVolume(float factor)
    {
        bgmVolumeFactor = factor;

        foreach (var info in bgmAudioInfoList)
        {
            info.audioSource.volume = mainVolume * bgmVolumeFactor;
        }
    }


    /// <summary>
    /// ???Sfx????????
    /// </summary>
    /// <param name="factor"></param>
    public void ChangeSfxVolume(float factor)
    {
        sfxVolumeFactor = factor;

        foreach (var info in sfxAudioInfoList)
        {
            info.audioSource.volume = mainVolume * sfxVolumeFactor;
        }
    }
    
    /// <summary>
    /// �����Ƶ����״̬�����Ž���������
    /// </summary>
    /// <param name="info">��Ƶ��Ϣ</param>
    /// <param name="isBgm">�Ƿ���BGM</param>
    IEnumerator DetectingAudioPlayState(AudioInfo info, bool isBgm)
    {
        AudioSource audioSource = info.audioSource;
        while (audioSource.isPlaying)
        {
            //if (audioSource == null)
            //{
            //    if (isBgm)
            //    {
            //        bgmAudioInfoList.Remove(info);
            //    }
            //    else
            //    {
            //        sfxAudioInfoList.Remove(info);
            //    }
            //    Destroy(info.audioSource.gameObject);
            //    yield return null;

            //}
            yield return null;
        }
        if (isBgm)
        {
            bgmAudioInfoList.Remove(info);
        }
        else
        {
            sfxAudioInfoList.Remove(info);
        }

    }
}

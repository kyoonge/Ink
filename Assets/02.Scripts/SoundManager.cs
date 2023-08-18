using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Sound  // MonoBehaviour ����� ���� ���� Ŭ������ ������Ʈ�� �߰� �Ұ���
{
    public string name;     // ���� �̸�
    public AudioClip clip;  // ����
}

public class SoundManager : MonoBehaviour
{
    // ���� �Ŵ��� �̱��� ���
    public static SoundManager instance;

    void Awake()
    {
        instance = this;
    }

    public Sound[] effectSounds;    // ȿ���� ����� Ŭ����
    public Sound[] bgmSounds;       // BGM ����� Ŭ����

    public AudioSource audioSourceBGM;  // BGM �����, BGM ����� �� �������� �̷����Ƿ� �迭 X
    public AudioSource[] audioSourceEffects;    // ȿ�������� ���ÿ� �������� ����� �� �����Ƿ� �迭 O

    public string[] playSoundName;  // ��� ���� ȿ���� ���� �̸� �迭

    void Start()
    {
        // ȿ���� �迭�� ũ�⸸ŭ �ʱ�ȭ
        playSoundName = new string[audioSourceEffects.Length];

        // ����� ���
        // PlayBGM("BGM_1");
        // audioSourceBGM.loop = true;
    }

    // ȿ���� ��� �Լ�
    public void PlaySE(string _name)
    {
        for (int i = 0; i < effectSounds.Length; i++)
        {
            // �Ű������� ���� �̸��� �����Ŭ���� �̸��� ������
            if (_name == effectSounds[i].name)
            {
                for (int j = 0; j < audioSourceEffects.Length; j++)
                {
                    // ȿ������ ��������� �ʴٸ�
                    if (!audioSourceEffects[j].isPlaying)
                    {
                        // ���� ��� �ִ� �����(�����Ŭ��)�� �Ű������� ���� ȿ������ �Ҵ�, ���
                        audioSourceEffects[j].clip = effectSounds[i].clip;
                        audioSourceEffects[j].Play();
                        // ��� ���� ȿ������ �Ű������� ���� ȿ������ �̸��� ��
                        playSoundName[j] = effectSounds[i].name;
                        // ����
                        return;
                    }
                }
                Debug.Log("��� ���� AudioSource�� ��� ���Դϴ�.");
                return;
            }
        }
        Debug.Log(_name + " ���尡 SoundManager�� ��ϵ��� �ʾҽ��ϴ�.");
    }

    // BGM ��� �Լ�
    public void PlayBGM(string _name)
    {
        for (int i = 0; i < bgmSounds.Length; i++)
        {
            // �Ű������� ���� �̸��� bgm�� �̸��� ������
            if (_name == bgmSounds[i].name)
            {
                // bgm����⿡ �Է¹��� bgm�� �Ҵ�, ���
                audioSourceBGM.clip = bgmSounds[i].clip;
                audioSourceBGM.Play();
                return;
            }
        }
        Debug.Log(_name + " ���尡 SoundManager�� ��ϵ��� �ʾҽ��ϴ�.");
    }

    // ��� ȿ���� ����� ���ߴ� �Լ�
    public void StopAllSE()
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            audioSourceEffects[i].Stop();
        }
    }

    // Ư�� ȿ���� ����� ���ߴ� �Լ�
    public void StopSE(string _name)
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            // ������� ȿ������ �̸��� �Ű������� ���� �̸��� ���ٸ�
            if (playSoundName[i] == _name)
            {
                audioSourceEffects[i].Stop();
                break;
            }
        }
        Debug.Log("��� ���� " + _name + " ���尡 �����ϴ�.");
    }
}
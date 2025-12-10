using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour
{
    AudioSource musicAudio; // AudioMixer의 Music부분 SoundManager로부터 가져오기
    List<string> playList = new List<string>(); // 실제로 재생되는 플레이리스트가 여기에 담김
    int musicIndex = 0; // 현재 재생 위치 파악하기 위해

    public void StartInit()
    {
        musicAudio = SoundManager.soundInstance.GetMusicAudioSource();

        UpdateMusic();

        if (playList.Count > 0)
        {
            string firstMusicName = playList[0];
    
            if (GlobalMusicData.musicDataInstance.playListDic.ContainsKey(firstMusicName))
            {
                SoundManager.soundInstance.PlaySound(SoundType.Music, GlobalMusicData.musicDataInstance.playListDic[firstMusicName]);
            }
    
            SoundManager.soundInstance.PlayMusicPlayEvent(firstMusicName);
        }
    }


    /** lobby에서 Music Editor에서 업데이트 눌렀을 때 */
    public void UpdateMusic()
    {
        MusicData mData = JsonDataManager.jsonInstance.LoadMusicData();
        playList = mData.musicNames;
    }

    int FindMusicIndex(string musicName)
    {
        int ret = 0;
        // 인덱스 업데이트 
        for(int i=0; i < playList.Count; i++)
        {
            if(playList[i].CompareTo(musicName) == 0)
            {
                ret = i;
            }   
        }

        return ret;
    }

    public void PlayMusic(string musicName)
    {
        if(GlobalMusicData.musicDataInstance.playListDic.ContainsKey(musicName) == false)
            return;

        if(musicAudio.clip != null)
            musicAudio.Stop();

        musicAudio.clip = GlobalMusicData.musicDataInstance.playListDic[musicName];
        musicAudio.Play();

        musicIndex = FindMusicIndex(musicName);
    }

    public string PlayNextMusic()
    {
        if (playList.Count == 0)
            return "";

        musicIndex++;
        if (musicIndex >= playList.Count)
        {
            musicIndex = 0;
        }

        string nextMusicName = playList[musicIndex];

        PlayMusic(nextMusicName);
        return nextMusicName;
    }
}

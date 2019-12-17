﻿using System;
using System.Collections.Generic;
using System.Text;
using WMPLib;
using System.Collections;

namespace MusicBox
{
    public class Player
    {

        private AxWMPLib.AxWindowsMediaPlayer myPlayer;
        private ArrayList playList;
        private int currentPlay;


        public int NumOfMusic
        {
            get
            {
                return playList.Count;
            }
        }

        public WMPLib.WMPPlayState playstate
        {
            get
            {
                return myPlayer.playState;
            }
        }

        public int CurrentPlay
        {
            get
            {
                return currentPlay;
            }
        }

        //public string PlayList(int num)
        //{
        //    return playList[num].ToString();
        //}




        public Player(AxWMPLib.AxWindowsMediaPlayer mediaPlayer)
        {
            myPlayer = mediaPlayer;
            playList = new ArrayList();
        }


        public void Clear()
        {
            playList.Clear();
        }

        public void AddFile(string path)
        {
            if (playList.Count < 1000)
            {
                playList.Add(path);
            }

        }

        public void DelFile(int index)
        {
            if (playList.Count > 0)
            {
                playList.RemoveAt(index);
            }
        }

        public void play(int index)
        {
            if (index >= 0 && index < playList.Count)
            {
                myPlayer.URL = playList[index].ToString();
                currentPlay = index;
                myPlayer.Ctlcontrols.play();
            }
            else
                myPlayer.Ctlcontrols.stop();
        }


        public int NextPlay(int type)
        {

            /* type = 0 顺序

            type = 1 重复播放全部

            type = 2 重复播放一首

            type = 3 随机播放
             
            type = 4 单曲播放一次

            */

            switch (type)
            {

                case 0:
                    currentPlay++;
                    if (currentPlay > NumOfMusic - 1) return -1;
                    else return currentPlay;
                case 1:
                    currentPlay++;
                    if (currentPlay > NumOfMusic - 1) return 0;
                    else return currentPlay;
                case 2:
                    return currentPlay;
                case 3:
                    Random rdm = new Random();
                    currentPlay = rdm.Next(0, NumOfMusic);
                    return currentPlay;
                case 4:
                    return -1;
                default:
                    return 0;
            }
        }
    }

}


using System;
using System.Collections.Generic;

namespace PNUnity.Share
{
    public class SPlayer
    {
        public long playerid;                           // 플레이어ID
        public long accountid;                          // 어카운트ID
        public DateTime create;                         // 생성시간
        public int gamech;                              // 인게임채널ID
        public string nickname;                         // 닉네임
        public Dictionary<CurrencyType, long> currency; // 보유재화
    }
}
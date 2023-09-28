using System;
using System.Collections.Generic;
using PNUnity.Share;

namespace PNShare.DB;

/// <summary>
/// 계정
/// 계정 1 : 플레이어 N
/// </summary>
public class MongoGameAccount : IMongoDocument<long>
{
    public long id { get; set; }           // 어카운트ID 
    public DateTime create { get; set; }   // 생성시간
    public string idkey { get; set; }      // 아이디키
    public long lastplayerid { get; set; } // 마지막접속 플레이어ID
}

/// <summary>
/// 플레이어 기본
/// </summary>
public class MongoGamePlayer1 : IMongoDocument<long>
{
    public long id { get; set; }                                 // 플레이어ID
    public long accountid { get; set; }                          // 어카운트ID
    public DateTime create { get; set; }                         // 생성시간
    public string token;                                         // 토큰
    public DateTime tokenexpire;                                 // 토큰만료시간
    public int gamech { get; set; }                              // 인게임채널ID
    public string nickname { get; set; }                         // 닉네임
    public Dictionary<CurrencyType, long> currency { get; set; } // 보유재화
    public DateTime logintime { get; set; }                      // 로그인시간
}
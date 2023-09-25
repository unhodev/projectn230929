namespace PNUnity.Share
{
    #region __BASE
    public abstract class ResponseCommon
    {
        public ErrorCode code { get; set; }
    }
    public class ResError : ResponseCommon
    {
        public string debug { get; set; }
    }
    public class RequestCommon
    {
        public string token { get; set; } // 엑세스키
    }
    public class RequestCommon2 : RequestCommon
    {
        public string sync { get; set; } // 동기화데이터
    }
    #endregion

    #region Account
    #region Account/Login
    // 계정 로그인시 호출
    public class ReqAccountLogin
    {
        public string idtoken { get; set; } // 식별자
    }
    public class ResAccountLogin : ResponseCommon
    {
        public string token { get; set; } // 엑세스키
    }
    #endregion
    #endregion

    #region Player
    #region Player/Enter
    // 최초 게임 진입시 호출
    public class ReqPlayerEnter : RequestCommon
    {
    }
    public class ResPlayerEnter : ResponseCommon
    {
    }
    #endregion
    #region Player/Sync
    // 동기화 시 호출
    public class ReqPlayerSync : RequestCommon2
    {
    }
    public class ResPlayerSync : ResponseCommon
    {
    }
    #endregion
    #endregion
}
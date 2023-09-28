namespace PNUnity.Share
{
    #region __BASE
    public abstract class ResponseCommon
    {
        public int result; // (int)ErrorCode
    }

    public class ResponseError : ResponseCommon
    {
        public string debug; // 디버깅메시지
    }

    public abstract class RequestCommon
    {
        public string token; // 토큰
    }

    public abstract class RequestWithSync : RequestCommon
    {
        public string sync; // 동기화데이터
    }
    #endregion

    #region Account
    #region Account/Login
    // 계정 로그인시 호출
    public class ReqAccountLogin
    {
        public string idtoken; // 식별자
    }

    public class ResAccountLogin : ResponseCommon
    {
        public string token; // 토큰
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
        public SPlayer player;         // 플레이어 정보
        public SStageDatas stagedatas; // 스테이지 정보
    }
    #endregion
    #region Player/Sync
    // 동기화 시 호출
    public class ReqPlayerSync : RequestWithSync
    {
    }

    public class ResPlayerSync : ResponseCommon
    {
    }
    #endregion
    #endregion
}
using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ServerSystem
{
    /// <summary>
    /// SQL Query 결과 값이 없는 경우 발생되는 오류 클래스
    /// </summary>
    public class NODATAEXCEPTION : Exception
    {
        public NODATAEXCEPTION() : base() 
        {
        }

        public NODATAEXCEPTION(string message) : base(message)
        { 
        }

        public NODATAEXCEPTION(string message, Exception innerException) : base(message, innerException)
        {
        }

        public NODATAEXCEPTION(SerializationInfo info, StreamingContext context)
        { 
        }

    }

    /// <summary>
    /// SQL connection 객체가 종료되지 못한 경우 발생되는 오류 클래스
    /// </summary>
    public class NOTCLOSEEXCEPTION : Exception
    {
        public NOTCLOSEEXCEPTION() : base()
        {
        }

        public NOTCLOSEEXCEPTION(string message) : base(message)
        {
        }

        public NOTCLOSEEXCEPTION(string message, Exception innerException) : base(message, innerException)
        {
        }

        public NOTCLOSEEXCEPTION(SerializationInfo info, StreamingContext context)
        {
        }

    }
        
    // 인터페이스에서 사용되는 델리게이터 선언
    public delegate bool NoResultInOneParamDelegate(string value, MySqlConnection conn);
    public delegate bool NoResultInTwoParamDelegate(string value1, string value2, MySqlConnection conn);
    public delegate bool NoResultInThreeParamDelegate(string value1, int value2, bool value3, MySqlConnection conn);
    public delegate DataTable ResultInOneParamDelegate(string value, MySqlConnection conn);

    // DB에 접근하기 위한 클래스
    public class SQL
    {
        /// <summary>
        /// DB에 접속하기 위해서 db connection 객체를 생성하는 메서드
        /// connection생성에 필요한 공통 부분을 처리하는 메서드
        /// </summary>
        /// <param name="user">DB 계정</param>
        /// <param name="database">DB 이름</param>
        /// <param name="password">DB 비밀번호</param>
        /// <param name="sslmode">DB sslmode</param>
        /// <returns></returns>
        private MySqlConnection Connect(
            string user,
            string database,
            string password,
            string sslmode
        )
        {
            // DB 서버와 포트번호
            string server = "localhost";
            int port = 3306;

            // DB connection 객체를 생성하기 위한 문자열
            string conn = $"SERVER = {server};port = {port};user = {user}; DATABASE = {database}; password = {password}; SSLMODE = {sslmode}";

            // DB connection 객체 생성
            MySqlConnection Conn = new MySqlConnection(conn);

            // conn 객체를 open 상태로 만들어줌
            Conn.Open();

            return Conn;
        }

        /// <summary>
        /// DB를 admin 계정으로 접속하기 위한 db connection 객체를 생성하는 메서드
        /// Connect메서드를 이용하여 관리자 권한으로 connection 객체를 생성하는 메서드
        /// </summary>
        /// <returns></returns>
        private MySqlConnection AdminConnect()
        {
            // DB admin 계정으로 connection 객체 만들기
            MySqlConnection adminConn = Connect("root", "db_server", "yuhan1234", "none");

            return adminConn;
        }

        /// <summary>
        /// DB를 user 계정으로 접속하기 위한 db connection 객체를 생성하는 메서드
        /// Connect메서드를 이용하여 사용자 권한으로 connection 객체를 생성하는 메서드
        /// </summary>
        /// <returns></returns>
        private MySqlConnection UserConnect()
        {
            // DB user 계정으로 connection 객체 만들기
            MySqlConnection adminConn = Connect("root", "db_server", "yuhan1234", "none");

            return adminConn;
        }

        /// <summary>
        /// DB connection 객체가 close 상태인지 확인하는 메서드
        /// </summary>
        /// <param name="conn">확인하려는 DB connection 객체</param>
        /// <returns></returns>
        private bool ConnClose(MySqlConnection conn)
        {
            // conn의 상태를 확인
            if (conn != null)
            {
                conn.Close();
            }

            conn = null;

            return true;
        }

        /// <summary>
        /// DB에서 회원가입에 필요한 프로시저를 실행시키는 메서드
        /// </summary>
        /// <param name="value">사용자의 입력정보, SQL insert문 values절에 들어갈 값들</param>
        /// <param name="conn">DB connection 객체</param>
        /// <returns></returns>
        internal bool Signin(
            string value,
            MySqlConnection conn
         )
        {
            try
            {
                // SQL 회원가입 Procedure를 수행 쿼리 
                string query = $"call Pro_user_in({value});";

                // command : 쿼리를 수행하는 객체
                MySqlCommand msc = new MySqlCommand(query, conn);

                // ExecuteNonQuery() 메서드는 쿼리의 영향을 받은 행의 수를 반환 하는 메서드
                if (msc.ExecuteNonQuery() == 0)
                {
                    throw new NODATAEXCEPTION();
                }
            }
            catch (NODATAEXCEPTION noDataException)
            {
                // Procedure가 수행되지 않았을 경우
                Console.WriteLine(noDataException.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// DB에서 로그인에 필요한 함수를 실행시키는 메서드
        /// </summary>
        /// <param name="id">사용자가 입력하는 id값</param>
        /// <param name="pw">사용자가 입력하는 pw값</param>
        /// <param name="conn">DB connection 객체</param>
        /// <returns></returns>
        internal bool Login(
            string id,
            string pw,
            MySqlConnection conn
        )
        {
            try
            {
                // SQL 로그인 Fuction 수행 쿼리
                string query = $"SELECT F_Login('{id}','{pw}')";

                // command : 쿼리를 수행하는 객체
                MySqlCommand msc = new MySqlCommand(query, conn);

                if (msc.ExecuteNonQuery() == 0)
                {
                    throw new NODATAEXCEPTION();
                }
            }
            catch (NODATAEXCEPTION noDataException)
            {
                // Function이 수행되지 않았을 경우
                Console.WriteLine(noDataException.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// DB에서 회원정보를 가져오는 프로시저를 실행시키는 메서드
        /// </summary>
        /// <param name="id">사용자 아이디</param>
        /// <param name="conn">DB connection 객체</param>
        /// <returns></returns>
        internal DataTable GetUserinfo(
            string id,
            MySqlConnection conn
        )
        {
            // SQL Procedure 결과를 저장할 데이터 테이블 객체
            var datatable = new DataTable();

            try
            {
                // SQL 회원정보를 가져오는 Procedure 수행 쿼리
                string query = $"call p_set_sel('{id}');";

                // MySqlDataAdapter : 쿼리 수행 결과를 가져오는 객체
                using (var mda = new MySqlDataAdapter(query, conn))
                {
                    if (mda != null)
                    {
                        mda.Fill(datatable);
                    }
                    // null일경우 예외처리 필요함
                }

            }
            catch (NODATAEXCEPTION noDataException)
            {
                // Select문이 수행되지 않았을 경우
                Console.WriteLine(noDataException.Message);
                return null;
            }

            return datatable;
        }

        /// <summary>
        /// DB에서 프로필정보를 가져오는 프로시저를 실행시키는 메서드
        /// </summary>
        /// <param name="id">사용자 아이디</param>
        /// <param name="conn">DB connectnion 객체</param>
        /// <returns></returns>
        internal DataTable GetProfileinfo(
            string id,
            MySqlConnection conn
        )
        {
            // SQL Procedure 결과를 저장하기위한 데이터 테이블 객체
            var datatable = new DataTable();

            try
            {
                // SQL 회원 프로필 정보를 가져오는 Procedure 수행 쿼리
                string query = $"call p_pr_sel('{id}');";

                //// command : 쿼리를 수행하는 객체
                //// datareader : 쿼리 수행 결과를 가져오는 객체
                //MySqlCommand msc = new MySqlCommand(query, conn);

                using (var mda = new MySqlDataAdapter(query, conn))
                {
                    if (mda != null)
                    {
                        mda.Fill(datatable);
                    }
                }
            }
            catch (NODATAEXCEPTION noDataException)
            {
                // Select문이 수행되지 않았을 경우
                Console.WriteLine(noDataException.Message);
                return null;
            }

            return datatable;
        }

        /// <summary>
        /// 수정된 회원정보를 DB에 저장하는 프로시저를 실행시키는 메서드
        /// </summary>
        /// <param name="value">회원정보</param>
        /// <param name="conn">DB connection 객체</param>
        /// <returns></returns>
        internal bool SetUserinfo(
            string value,
            MySqlConnection conn
        )
        {
            try
            {
                // SQL 회원정보 수정 Procedure를 수행 쿼리 
                string query = $"call p_set_up({value});";

                // command : 쿼리를 수행하는 객체
                MySqlCommand msc = new MySqlCommand(query, conn);

                // ExecuteNonQuery() 메서드는 쿼리의 영향을 받은 행의 수를 반환 하는 메서드
                if (msc.ExecuteNonQuery() == 0)
                {
                    throw new NODATAEXCEPTION();
                }
            }
            catch (NODATAEXCEPTION noDataException)
            {
                // Procedure가 수행되지 않았을 경우
                Console.WriteLine(noDataException.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// DB에 회원 별명을 저장하는 프로시저를 실행시키는 메서드
        /// </summary>
        /// <param name="id">사용자 아이디</param>
        /// <param name="nickname">사용자 별명</param>
        /// <param name="conn">DB connection 객체</param>
        /// <returns></returns>
        internal bool SetUserNick(
            string id,
            string nickname,
            MySqlConnection conn
        )
        {
            try
            {
                // SQL 회원정보 수정 Procedure를 수행 쿼리 
                string query = $"call p_set_up_Nick({id},{nickname});";

                // command : 쿼리를 수행하는 객체
                MySqlCommand msc = new MySqlCommand(query, conn);

                // ExecuteNonQuery() 메서드는 쿼리의 영향을 받은 행의 수를 반환 하는 메서드
                if (msc.ExecuteNonQuery() == 0)
                {
                    throw new NODATAEXCEPTION();
                }
            }
            catch (NODATAEXCEPTION noDataException)
            {
                // Procedure가 수행되지 않았을 경우
                Console.WriteLine(noDataException.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">사용자 아이디</param>
        /// <param name="name">사용자 이름</param>
        /// <param name="conn">DB connection 객체</param>
        /// <returns></returns>
        internal bool SetUserName(
            string id,
            string name,
            MySqlConnection conn
        )
        {
            try
            {
                // SQL 회원정보 수정 Procedure를 수행 쿼리 
                string query = $"call p_set_up_name({id},{name});";

                // command : 쿼리를 수행하는 객체
                MySqlCommand msc = new MySqlCommand(query, conn);

                // ExecuteNonQuery() 메서드는 쿼리의 영향을 받은 행의 수를 반환 하는 메서드
                if (msc.ExecuteNonQuery() == 0)
                {
                    throw new NODATAEXCEPTION();
                }
            }
            catch (NODATAEXCEPTION noDataException)
            {
                // Procedure가 수행되지 않았을 경우
                Console.WriteLine(noDataException.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">사용자 아이디</param>
        /// <param name="phone">사용자 핸드폰 번호</param>
        /// <param name="conn">DB connection 객체</param>
        /// <returns></returns>
        internal bool SetUserPhone(
            string id,
            string phone,
            MySqlConnection conn
        )
        {
            try
            {
                // SQL 회원정보 수정 Procedure를 수행 쿼리 
                string query = $"call p_set_up_phone ({id},{phone});";

                // command : 쿼리를 수행하는 객체
                MySqlCommand msc = new MySqlCommand(query, conn);

                // ExecuteNonQuery() 메서드는 쿼리의 영향을 받은 행의 수를 반환 하는 메서드
                if (msc.ExecuteNonQuery() == 0)
                {
                    throw new NODATAEXCEPTION();
                }
            }
            catch (NODATAEXCEPTION noDataException)
            {
                // Procedure가 수행되지 않았을 경우
                Console.WriteLine(noDataException.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// DB에서 친구등록을 Id를 통해 실행하는 프로시저를 실행시키는 메서드
        /// </summary>
        /// <param name="id">사용자 아이디</param>
        /// <param name="friendId">친구 아이디</param>
        /// <param name="conn">DB connection 객체</param>
        /// <returns></returns>
        internal bool AddFriendById(
            string id,
            string friendId,
            MySqlConnection conn
        )
        {
            try
            {
                // SQL 친구 아이디를 통한 친구추가 Procedure를 수행 쿼리 
                string query = $"call P_Fr_id_in({id},{friendId});";

                // command : 쿼리를 수행하는 객체
                MySqlCommand msc = new MySqlCommand(query, conn);

                // ExecuteNonQuery() 메서드는 쿼리의 영향을 받은 행의 수를 반환 하는 메서드
                if (msc.ExecuteNonQuery() == 0)
                {
                    throw new NODATAEXCEPTION();
                }
            }
            catch (NODATAEXCEPTION noDataException)
            {
                // Procedure가 수행되지 않았을 경우
                Console.WriteLine(noDataException.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// DB에서 친구등록을 phone를 통해 실행하는 프로시저를 실행시키는 메서드
        /// </summary>
        /// <param name="id">사용자 아이디</param>
        /// <param name="friendPhone">친구 핸드폰 번호</param>
        /// <param name="conn">DB connection 객체</param>
        /// <returns></returns>
        internal bool AddFriendByPhone(
            string id,
            string friendPhone,
            MySqlConnection conn
        )
        {
            try
            {
                // SQL 핸드폰번호를 통한 친구추가 Procedure를 수행 쿼리 
                string query = $"call P_Fr_id_in({id},{friendPhone});";

                // command : 쿼리를 수행하는 객체
                MySqlCommand msc = new MySqlCommand(query, conn);

                // ExecuteNonQuery() 메서드는 쿼리의 영향을 받은 행의 수를 반환 하는 메서드
                if (msc.ExecuteNonQuery() == 0)
                {
                    throw new NODATAEXCEPTION();
                }
            }
            catch (NODATAEXCEPTION noDataException)
            {
                // Procedure가 수행되지 않았을 경우
                Console.WriteLine(noDataException.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// DB에서 친구 별명을 설정하는 프로시저를 실행시키는 메서드
        /// </summary>
        /// <param name="id">사용자 아이디</param>
        /// <param name="friendCode">친구 코드</param>
        /// <param name="friendNickName">친구 별명</param>
        /// <param name="conn">DB connection 객체</param>
        /// <returns></returns>
        internal bool SetFriendNick(
            string id,
            int friendCode,
            string friendNickName,
            MySqlConnection conn
        )
        {
            try
            {
                // SQL 친구 별명 수정 Procedure를 수행 쿼리 
                string query = $"call P_Fr_set_nick({id},{friendCode},{friendNickName});";

                // command : 쿼리를 수행하는 객체
                MySqlCommand msc = new MySqlCommand(query, conn);

                // ExecuteNonQuery() 메서드는 쿼리의 영향을 받은 행의 수를 반환 하는 메서드
                if (msc.ExecuteNonQuery() == 0)
                {
                    throw new NODATAEXCEPTION();
                }
            }
            catch (NODATAEXCEPTION noDataException)
            {
                // Procedure가 수행되지 않았을 경우
                Console.WriteLine(noDataException.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// DB에서 친구숨김을 설정하는 프로시저를 실행시키는 메서드
        /// </summary>
        /// <param name="id">사용자 아이디</param>
        /// <param name="friendCode">친구 코드</param>
        /// <param name="isCheck">숨김 여부를 표현하는 bool변수</param>
        /// <param name="conn">DB connection 객체</param>
        /// <returns></returns>
        internal bool SetFriendHide(
            string id,
            int friendCode,
            bool isCheck,
            MySqlConnection conn
        )
        {
            try
            {
                // SQL 친구 숨김 Procedure를 수행 쿼리 
                string query = $"call P_Fr_set_hide ({id},{friendCode},{isCheck});";

                // command : 쿼리를 수행하는 객체
                MySqlCommand msc = new MySqlCommand(query, conn);

                // ExecuteNonQuery() 메서드는 쿼리의 영향을 받은 행의 수를 반환 하는 메서드
                if (msc.ExecuteNonQuery() == 0)
                {
                    throw new NODATAEXCEPTION();
                }
            }
            catch (NODATAEXCEPTION noDataException)
            {
                // Procedure가 수행되지 않았을 경우
                Console.WriteLine(noDataException.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// DB에서 친구차단을 설정하는 프로시저를 실행시키는 메서드
        /// </summary>
        /// <param name="id">사용자 아이디</param>
        /// <param name="friendCode">친구 코드</param>
        /// <param name="isCheck">차단 여부를 표현하는 bool변수</param>
        /// <param name="conn">DB connection 객체</param>
        /// <returns></returns>
        internal bool SetFriendBlock(
            string id,
            int friendCode,
            bool isCheck,
            MySqlConnection conn
        )
        {
            try
            {
                // SQL 친구 차단 Procedure를 수행 쿼리 
                string query = $"call P_Fr_set_block ({id},{friendCode},{isCheck});";

                // command : 쿼리를 수행하는 객체
                MySqlCommand msc = new MySqlCommand(query, conn);

                // ExecuteNonQuery() 메서드는 쿼리의 영향을 받은 행의 수를 반환 하는 메서드
                if (msc.ExecuteNonQuery() == 0)
                {
                    throw new NODATAEXCEPTION();
                }
            }
            catch (NODATAEXCEPTION noDataException)
            {
                // Procedure가 수행되지 않았을 경우
                Console.WriteLine(noDataException.Message);
                return false;
            }

            return true;
        }

        // ==============================================인터페이스==============================================

        /// <summary>
        /// 클라이언트에서 넘어온 회원정보의 유효성을 확인하는 메서드
        /// </summary>
        /// <param name="id">클라이언트에서 보낸 아이디</param>
        /// <param name="pw">클라이언트에서 보낸 비밀번호</param>
        /// <param name="name">클라이언트에서 보낸 이름</param>
        /// <param name="nick">클라이언트에서 보낸 별칭</param>
        /// <param name="phone">클라이언트에서 보낸 전화번호</param>
        /// <param name="checkInMethod">회원정보의 유효성 확인 후 실행할 메서드</param>
        /// <returns></returns>
        public bool beforeConnectDBCheck(
            string id,
            string pw,
            string name,
            string nick,
            string phone,
            NoResultInOneParamDelegate inMethod
        )
        {
            // 예외 경우일 경우 do-while 문을 탈출하여 return 하므로 디폴트를 false로 설정
            bool okSignIn = false;
            string userInfo = "";

            do
            {
                // 매개변수 값들의 null 체크
                if (id == null)
                {
                    break;
                }
                if (pw == null)
                {
                    break;
                }
                if (name == null)
                {
                    break;
                }
                if (nick == null)
                {
                    break;
                }
                if (phone == null)
                {
                    break;
                }

                // 매개변수 값들의 공백을 체크
                if (id == "")
                {
                    break;
                }
                if (pw == "")
                {
                    break;
                }
                if (name == "")
                {
                    break;
                }
                if (nick == "")
                {
                    break;
                }
                if (phone == "")
                {
                    break;
                }

                // 매개변수 값들의 이상이 없다면 수행 되는 과정
                userInfo = $"'{id}','{pw}','{nick}','{name}','{phone}',null";
                okSignIn = noResultConnectDB(userInfo, inMethod);

            } while (false);

            return okSignIn;
        }

        // 오버로딩
        // Signin, Modify
        /// <summary>
        /// 결과 값이 없는 DB Query를 실행시키는 인터페이스
        /// </summary>
        /// <param name="value"></param>
        /// <param name="noResultInOneParamDelegate"></param>
        /// <returns></returns>
        internal bool noResultConnectDB(
            string value,
            NoResultInOneParamDelegate noResultInOneParamDelegate
        )
        {

            try
            {
                // DB 연결
                MySqlConnection conn = UserConnect();

                // SQL 모듈 수행
                if (noResultInOneParamDelegate(value, conn) != true)
                {
                    // SQL모듈이 정상 작동 하지 못했을 때
                    return false;
                }

                // DB 닫기
                if (!ConnClose(conn))
                {
                    throw new NOTCLOSEEXCEPTION();
                }
            }
            catch (NOTCLOSEEXCEPTION notCloseException)
            {
                // conn close를 실패했을 때
                Console.WriteLine(notCloseException.Message);
                return false;
            }

            return true;
        }

        //오버로딩
        //Login, AddfriendByUd, AddfriendByPhone
        internal bool noResultConnectDB(
            string value1,
            string value2,
            NoResultInTwoParamDelegate noResultInTwoParamDelegate
        )
        {
            try
            {
                // DB 연결
                MySqlConnection conn = UserConnect();

                // SQL 모듈 수행
                if (noResultInTwoParamDelegate(value1, value2, conn) != true)
                {
                    // SQL모듈이 정상 작동 하지 못했을 때
                    return false;
                }

                // DB 닫기
                if (!ConnClose(conn))
                {
                    throw new NOTCLOSEEXCEPTION();
                }

            }
            catch (NOTCLOSEEXCEPTION notCloseException)
            {
                // conn close를 실패했을 때
                Console.WriteLine(notCloseException.Message);
                return false;
            }

            return true;
        }

        // 오버로딩
        // SetFriendNick, SetFriendHide, SetFriendBlock
        internal bool noResultConnectDB(
            string id,
            int friendCode,
            bool isCheck,
            NoResultInThreeParamDelegate noResultInThreeParamDelegate
        )
        {

            try
            {
                // DB 연결
                MySqlConnection conn = UserConnect();

                // Insert문 수행
                if (noResultInThreeParamDelegate(id, friendCode, isCheck, conn) != true)
                {
                    return false;
                }

                // DB 닫기
                if (!ConnClose(conn))
                {
                    throw new NOTCLOSEEXCEPTION();
                }
            }
            catch (NOTCLOSEEXCEPTION notCloseException)
            {
                // conn close를 실패했을 때
                Console.WriteLine(notCloseException.Message);
                return false;
            }

            return true;
        }

        // getUserInfo, getProfileInfo
        internal DataTable resultConnectDB(
            string value,
            ResultInOneParamDelegate resultInOneParamDelegate 
        )
        {
            var dt = new DataTable();

            try
            {
                // DB 연결
                MySqlConnection conn = UserConnect();

                dt = resultInOneParamDelegate(value, conn);

                //Console.WriteLine(dt.Rows[0]["U_password"]);

                // SQL Procedure 수행
                if (dt == null)
                {
                    // SQL함수가 정상 작동 하지 못했을 때
                    return null;
                }

                // DB 닫기
                if (!ConnClose(conn))
                {
                    throw new NOTCLOSEEXCEPTION();
                }

            }
            catch (NOTCLOSEEXCEPTION notCloseException)
            {
                // conn close를 실패했을 때
                Console.WriteLine(notCloseException.Message);
                return null;
            }

            return dt;
        }
    }    
}

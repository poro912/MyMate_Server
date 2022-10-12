﻿using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ServerSystem
{
    class NODATAEXCEPTION : Exception
    {
    }

    class NOTCLOSEEXCEPTION : Exception
    {
    }

    class NOIDEXCEPTION : Exception
    {
    }
    public delegate bool NoResultInOneParamDelegate(string value, MySqlConnection conn);
    public delegate bool NoResultInTwoParamDelegate(string value1, string value2, MySqlConnection conn);
    public delegate DataTable ResultInOneParamDelegate(string value, MySqlConnection conn);

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
            MySqlConnection adminConn = Connect("admin", "db_server", "12345", "none");

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
                // Select 문을 수행 쿼리
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
        public bool Check(
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
        // signin, modify
        internal bool noResultConnectDB(
            string value,
            NoResultInOneParamDelegate noResultInOneParamDelegate
        )
        {
            // 결과값을 반환하는 변수
            bool okInsert = true;

            try
            {
                // DB 연결
                MySqlConnection conn = UserConnect();

                // Insert문 수행
                okInsert = noResultInOneParamDelegate(value, conn);

                // DB 닫기
                if (!ConnClose(conn))
                {
                    throw new NOTCLOSEEXCEPTION();
                }
            }
            catch (NOTCLOSEEXCEPTION noDataException)
            {
                // conn close를 실패했을 때

                return false;
            }

            return okInsert;
        }

        //오버로딩
        //login
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

                // SQL Login Functio 수행
                if (noResultInTwoParamDelegate(value1, value2, conn) != true)
                {
                    // SQL함수가 정상 작동 하지 못했을 때
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

                return null;
            }

            return dt;
        }

        /*

        /// <summary>
        /// 회원가입을 위해서 DB에 Insert 문을 통해서 사용자 정보 등록하는 메서드
        /// </summary>
        ///  <param name="id">회원 id</param>
        /// <param name="pw">회원 password</param>
        /// <param name="name">회원 이름</param>
        /// <param name="nick">회원 별명</param>
        /// <param name="phone">회원 전화번호</param>
        /// <returns></returns>
        internal bool SigninInsert(
            string userInfo
        )
        {
            // 결과값을 반환하는 변수
            bool okInsert = true;
                                   
            try
            {
                // DB 연결
                MySqlConnection conn = UserConnect();

                // Insert문 수행
                okInsert = CallSigninSP(userInfo, conn);

                // DB 닫기
                if (!ConnClose(conn))
                {
                    throw new NOTCLOSEEXCEPTION();
                }
            }
            catch (NOTCLOSEEXCEPTION noDataException)
            {
                // conn close를 실패했을 때

                return false;
            }

            return okInsert;
        }

        /// <summary>
        /// 회원정보 수정을 저장하기 위해 DB에 Insert 문을 통하여 사용자 정보를 등록하는 메서드
        /// </summary>
        ///  <param name="id">회원 id</param>
        /// <param name="pw">회원 password</param>
        /// <param name="name">회원 이름</param>
        /// <param name="nick">회원 별명</param>
        /// <param name="phone">회원 전화번호</param>
        /// <returns></returns>
        internal bool ModifyInsert(
            string userInfo
        )
        {
            // 결과값을 반환하는 변수
            bool okUpdate = true;
                    
            try
            {
                // DB 연결
                MySqlConnection conn = UserConnect();

                // Insert문 수행
                okUpdate = CallSetUserinfoSP(userInfo, conn);

                // DB 닫기
                if (!ConnClose(conn))
                {
                    throw new NOTCLOSEEXCEPTION();
                }
            }
            catch (NOTCLOSEEXCEPTION noDataException)
            {
                // conn close를 실패했을 때

                return false;
            }

            return okUpdate;
        }

        /// <summary>
        /// 입력된 id, pw로 로그인 시켜주는 메서드
        /// </summary>
        /// <param name="id">회원이 입력한 id</param>
        /// <param name="pw">회원이 입력한 pw</param>
        /// <returns></returns>
        public bool Login(
            string id, 
            string pw
        )
        {
            try
            {
                // DB 연결
                MySqlConnection conn = UserConnect();

                // SQL Login Functio 수행
                if (CallLoginSF(id, pw, conn) != true)
                {
                    // SQL함수가 정상 작동 하지 못했을 때
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

                return false;
            }

            return true;
        }

        /// <summary>
        /// id값 입력을 통해서 사용자의 회원정보를 가져오는 메서드
        /// </summary>
        /// <param name="id">사용자 아이디</param>
        /// <returns></returns>
        public DataTable GetUserInfo(
            string id
        )
        {
            var dt = new DataTable();

            try
            {
                // DB 연결
                MySqlConnection conn = UserConnect();

                dt = CallGetUserinfoSP(id, conn);

                Console.WriteLine(dt.Rows[0]["U_password"]);

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

                return null;
            }

            return dt;
        }

        /// <summary>
        /// id값을 입력을 통해서 사용자의 프로필정보를 가져오는 메서드
        /// </summary>
        /// <param name="id">사용자 아이디</param>
        /// <returns></returns>
        public DataTable GetProfileInfo(
            string id
        )
        {
            var dt = new DataTable();

            try
            {
                // DB 연결
                MySqlConnection conn = UserConnect();

                dt = CallGetProfileinfoSP(id, conn);

                //Console.WriteLine(dt.Rows[0]["U_name"]);

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

                return null;
            }

            return dt;
        }
        */


        /*
        /// <summary>
        /// DB에 Select 구문을 수행하는 메서드
        /// </summary>
        /// <param name="table">Select하려는 테이블</param>
        /// <param name="condition">Select할때 조건절(select문에서 where키워드 이후 전부 작성)</param>
        /// <param name="conn">DB connection 객체</param>
        /// <returns></returns>
        private DataTable SqlSelect(
            string table,
            string condition,
            MySqlConnection conn
        )
        {
            // Select문을 반환하기 위한 데이터 테이블
            var datatable = new DataTable();

            try
            {
                // Select 문을 수행 쿼리
                string query = $"SELECT * FROM {table} WHERE {condition}";

                // command : 쿼리를 수행하는 객체
                // datareader : 쿼리 수행 결과를 가져오는 객체
                MySqlCommand msc = new MySqlCommand(query, conn);

                if (msc.ExecuteNonQuery() == 0)
                {
                    throw new NODATAEXCEPTION();
                }
                else
                {
                    // ExecuteReader() 메서드는 DataReader를 만들어줌
                    MySqlDataReader msdr = msc.ExecuteReader();

                    // Load() 메서드는 DataReader를 통해 DataTable을 채움
                    datatable.Load(msdr);
                }
            }
            catch (NODATAEXCEPTION noDataException)
            {
                // Select문이 수행되지 않았을 경우

                return null;
            }

            return datatable;
        }

        /// <summary>
        /// DB에 Insert 구문을 수행하는 메서드
        /// </summary>
        /// <param name="table">Insert하려는 테이블</param>
        /// <param name="value">Insert할때 조건절(insert문에서 values 키워드 이후 전부 작성)</param>
        /// <param name="conn">DB connection 객체</param>
        /// <returns></returns>
        private bool SqlInsert(
            string table,
            string value,
            MySqlConnection conn
         )
        {
            try
            {
                // Insert 문을 수행 쿼리 
                string query = $"INSERT INTO {table} VALUES ({value})";

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
                // Insert문이 수행되지 않았을 경우

                return false;
            }

            return true;
        }*/
    }
        
}
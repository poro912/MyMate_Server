using MySql.Data.MySqlClient;
using MySql.Data.Types;
using ServerSystem;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace MyMate_Server
{
    public class UserParm
    {
        public int userCode;
        public string id;
        public string pwd;
        public string nick;
        public string name;
        public string phone;
        public string email;
        public string content;
        public int isDeleted;
        public string dataFormat;
    }

    public class FriendParm
    {
        public int userCode;
        public int friendCode;
        public string id;
        public string phone;
        public string nick;
        public int hide;
        public int block;
        public int isDeleted;
    }

    public class UserChannelParm
    {
        public int userCode;
        public int channelCode;
        public string title;
        public int state;
        public int isPrivate;
        public int isDeleted;
    }

    public class UserChecklistParm
    {
        public int userCode;
        public int channelCode;
        public int checklistCode;
        public string cotent;
        public long startTime;
        public long endTime;
        public int ischecked;
        public int isPrivate;
        public int isDeleted;

    }

    public class UserCalendarParm
    {
        public int userCode;
        public int channelCode;
        public int calendarCode;
        public string content;
        public long startTime;
        public long endTime;
        public int isPrivate;
        public int isDeleted;
    }

    public class ServerParm
    {
        public int serverCode;
        public string title;
        public int adminCode;
        public int isSingle;
        public int isDeleted;
    }

    public class ChannelParm
    {
        public int serverCode;
        public int channelCode;
        public string title;
        public int state;
        public int isDeleted;
    }

    public class MessageParm
    {
        public int serverCode;
        public int channelCode;
        public int messageCode;
        public string content;
        public int creater;
        public long startTime;
        public int isPrivate;
        public int isDeleted;
    }

    public class ChecklistParm
    {
        public int serverCode;
        public int channelCode;
        public int checklistCode;
        public string content;
        public int creater;
        public long startTime;
        public long endTime;
        public int ischecked;
        public int isPrivate;
        public int isDeleted;
    }

    public class CalendarParm
    {
        public int serverCode;
        public int channelCode;
        public int calendarCode;
        public string content;
        public int creater;
        public long startTime;
        public long endTime;
        public int isPrivate;
        public int isDeleted;
    }

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

    public class DBModule
    {
        // user 기능
        const string Login = "login_user";
        const string Signin = "create_user";
        const string GetUser = "select_user";
        const string GetUserCode = "select_u_code";
        const string ChangeUser = "update_user";

        // friend 기능
        const string AddFriend = "create_friend";
        const string GetFriend = "select_friend";
        const string SetFriend = "update_friend";

        // user channel 기능
        const string AddUserChannel = "create_channel";
        const string GetUserChannel = "select_channel";
        const string SetUserChannel = "update_channel";

        // user checklist 기능
        const string AddUserChecklist = "create_u_checklist";
        const string GetUserChecklist = "select_u_checklist";
        const string SetUserChecklist = "update_u_checklist";

        // user calendar 기능
        const string AddUserCalendar = "create_u_calendar";
        const string GetUserCalendar = "select_u_calendar";
        const string SetUserCalendar = "update_u_calendar";

        // server 기능
        const string AddServer = "create_server";
        const string GetServer = "select_server";
        const string SetServer = "update_server";

        // channel 기능
        const string AddChannel = "create_channel";
        const string GetChannel = "select_channel";
        const string SetChannel = "update_channel";

        // message 기능
        const string AddMessage = "create_message";
        const string GetMessage = "select_message";
        const string SetMessage = "update_message";

        // checklist 기능
        const string AddChecklist = "create_checklist";
        const string GetChecklist = "select_checklist";
        const string SetChecklist = "update_checklist";

        // calendar 기능
        const string AddCalendar = "create_calendar";
        const string GetCalendar = "select_calendar";
        const string SetCalendar = "update_calendar";


        public string query;

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

        // ==============================================기초기능==============================================

        public bool noResultConnectDB(
            object obj,
            string queryName
        )
        {
            try
            {
                // DB 연결
                MySqlConnection conn = UserConnect();

                // Query 생성
                query = makeQuery(obj, queryName);

                // Query 실행
                if (!processNoResultQuery(query, conn))
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

        public DataTable resultConnectDB(
            object obj,
            string queryName
        )
        {
            DataTable dataTable = new DataTable();

            try
            {
                // DB 연결
                MySqlConnection conn = UserConnect();

                // Query 생성
                query = makeQuery(obj, queryName);

                // Query 실행
                dataTable = processResultQuery(query, conn);

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

            return dataTable;
        }

        public string makeQuery(object parmObj,string queryName)
        {
            string query="";

            if (queryName == "Login")
            {
                UserParm userParm = (UserParm)parmObj;
                query = $"select {Login}('{userParm.id}','{userParm.pwd}')";
            }
            else if (queryName == "Signin")
            {
                UserParm userParm = (UserParm)parmObj;
                query = $"call {Signin}('{userParm.id}','{userParm.pwd}','{userParm.nick}','{userParm.name}','{userParm.phone}','{userParm.email}',null)";
            }
            else if (queryName == "GetUser")
            {
                UserParm userParm = (UserParm)parmObj;
                query = $"call {GetUser}('{userParm.userCode}','{userParm.dataFormat}')";
            }
            else if (queryName == "GetUserCode")
            {
                UserParm userParm = (UserParm)parmObj;
                query = $"call {GetUserCode}('{userParm.id}')";
            }
            else if (queryName == "ChangeUser")
            {
                UserParm userParm = (UserParm)parmObj;
                query = $"call {ChangeUser}('{userParm.userCode}','{userParm.pwd}','{userParm.nick}','{userParm.name}','{userParm.phone}','{userParm.email}','{userParm.content}','{userParm.isDeleted}')";
            }

            else if (queryName == "AddFriend")
            {
                FriendParm friendParm = (FriendParm)parmObj;
                query = $"call {AddFriend}('{friendParm.userCode}','{friendParm.id}','{friendParm.phone}')";
            }
            else if (queryName == "GetFriend")
            {
                FriendParm friendParm = (FriendParm)parmObj;
                query = $"call {GetFriend}('{friendParm.userCode}','{friendParm.friendCode}')";
            }
            else if (queryName == "SetFriend")
            {
                FriendParm friendParm = (FriendParm)parmObj;
                query = $"call {SetFriend}('{friendParm.userCode}','{friendParm.friendCode}','{friendParm.nick}','{friendParm.hide}','{friendParm.block}','{friendParm.isDeleted}')";
            }

            else if (queryName == "AddUserChannel")
            {
                UserChannelParm userChannelParm = (UserChannelParm)parmObj;
                query = $"call {AddUserChannel}('{userChannelParm.userCode}','{userChannelParm.title}','{userChannelParm.state}','{userChannelParm.isPrivate}')";
            }
            else if (queryName == "GetUserChannel")
            {
                UserChannelParm userChannelParm = (UserChannelParm)parmObj;
                query = $"call {GetUserChannel}('{userChannelParm.userCode}','{userChannelParm.channelCode}')";
            }
            else if (queryName == "SetUserChannel")
            {
                UserChannelParm userChannelParm = (UserChannelParm)parmObj;
                query = $"call {SetUserChannel}('{userChannelParm.userCode}','{userChannelParm.channelCode}','{userChannelParm.title}','{userChannelParm.state}','{userChannelParm.isPrivate}','{userChannelParm.isDeleted}')";
            }

            else if (queryName == "AddUserChecklist")
            {
                UserChecklistParm userChecklistParm = (UserChecklistParm)parmObj;
                query = $"call {AddUserChecklist}('{userChecklistParm.userCode}','{userChecklistParm.channelCode}','{userChecklistParm.cotent}','{userChecklistParm.startTime}','{userChecklistParm.endTime}','{userChecklistParm.isPrivate}')";
            }
            else if (queryName == "GetUserChecklist")
            {
                UserChecklistParm userChecklistParm = (UserChecklistParm)parmObj;
                query = $"call {GetUserChecklist}('{userChecklistParm.userCode}','{userChecklistParm.channelCode}','{userChecklistParm.checklistCode}')";
            }
            else if (queryName == "SetUserChecklist")
            {
                UserChecklistParm userChecklistParm = (UserChecklistParm)parmObj;
                query = $"call {SetUserChecklist}('{userChecklistParm.userCode}','{userChecklistParm.channelCode}','{userChecklistParm.checklistCode}','{userChecklistParm.cotent}','{userChecklistParm.startTime}','{userChecklistParm.endTime}','{userChecklistParm.ischecked}','{userChecklistParm.isPrivate}','{userChecklistParm.isDeleted}')";
            }

            else if (queryName == "AddUserCalendar")
            {
                UserCalendarParm userCalendarParm = (UserCalendarParm)parmObj;
                query = $"call {AddUserCalendar}('{userCalendarParm.userCode}','{userCalendarParm.channelCode}','{userCalendarParm.content}','{userCalendarParm.startTime}','{userCalendarParm.endTime}','{userCalendarParm.isPrivate}')";
            }
            else if (queryName == "GetUserCalendar")
            {
                UserCalendarParm userCalendarParm = (UserCalendarParm)parmObj;
                query = $"call {GetUserCalendar}('{userCalendarParm.userCode}','{userCalendarParm.channelCode}','{userCalendarParm.calendarCode}')";
            }
            else if (queryName == "SetUserCalendar")
            {
                UserCalendarParm userCalendarParm = (UserCalendarParm)parmObj;
                query = $"call {SetUserCalendar}('{userCalendarParm.userCode}','{userCalendarParm.channelCode}','{userCalendarParm.calendarCode}','{userCalendarParm.content}','{userCalendarParm.startTime}','{userCalendarParm.endTime}','{userCalendarParm.isPrivate}','{userCalendarParm.isDeleted}')";
            }

            else if (queryName == "AddServer")
            {
                ServerParm serverParm = (ServerParm)parmObj;
                query = $"call {AddServer}('{serverParm.title}','{serverParm.adminCode}','{serverParm.isSingle}')";
            }
            else if (queryName == "GetServer")
            {
                ServerParm serverParm = (ServerParm)parmObj;
                query = $"call {GetServer}('{serverParm.serverCode}')";
            }
            else if (queryName == "SetServer")
            {
                ServerParm serverParm = (ServerParm)parmObj;
                query = $"call {SetServer}('{serverParm.serverCode}','{serverParm.title}','{serverParm.adminCode}', '{serverParm.isSingle}', '{serverParm.isDeleted}')";
            }

            else if (queryName == "AddChannel")
            {
                ChannelParm channelParm = (ChannelParm)parmObj;
                query = $"call {AddChannel}('{channelParm.serverCode}','{channelParm.title}','{channelParm.state}')";
            }
            else if (queryName == "GetChannel")
            {
                ChannelParm channelParm = (ChannelParm)parmObj;
                query = $"call {GetChannel}('{channelParm.serverCode}','{channelParm.channelCode}')";
            }
            else if (queryName == "SetChannel")
            {
                ChannelParm channelParm = (ChannelParm)parmObj;
                query = $"call {SetChannel}('{channelParm.serverCode}','{channelParm.channelCode}','{channelParm.title}','{channelParm.state}','{channelParm.isDeleted}')";
            }

            else if (queryName == "AddMessage")
            {
                MessageParm messageParm = (MessageParm)parmObj;
                query = $"call {AddMessage}('{messageParm.serverCode}','{messageParm.channelCode}','{messageParm.content}','{messageParm.creater}','{messageParm.startTime}','{messageParm.isPrivate}')";
            }
            else if (queryName == "GetMessage")
            {
                MessageParm messageParm = (MessageParm)parmObj;
                query = $"call {GetMessage}('{messageParm.serverCode}','{messageParm.channelCode}','{messageParm.serverCode}')";
            }
            else if (queryName == "SetMessage")
            {
                MessageParm messageParm = (MessageParm)parmObj;
                query = $"call {SetMessage}('{messageParm.serverCode}','{messageParm.channelCode}','{messageParm.messageCode}','{messageParm.content}','{messageParm.creater}','{messageParm.startTime}','{messageParm.isPrivate}','{messageParm.isDeleted}')";
            }

            else if (queryName == "AddChecklist")
            {
                ChecklistParm checklistParm = (ChecklistParm)parmObj;
                query = $"call {AddChecklist}('{checklistParm.serverCode}','{checklistParm.channelCode}','{checklistParm.content}','{checklistParm.startTime}','{checklistParm.endTime}','{checklistParm.isPrivate}')";
            }
            else if (queryName == "GetChecklist")
            {
                ChecklistParm checklistParm = (ChecklistParm)parmObj;
                query = $"call {GetChecklist}('{checklistParm.serverCode}','{checklistParm.channelCode}','{checklistParm.checklistCode}')";
            }
            else if (queryName == "SetChecklist")
            {
                ChecklistParm checklistParm = (ChecklistParm)parmObj;
                query = $"call {SetChecklist}('{checklistParm.serverCode}','{checklistParm.channelCode}','{checklistParm.checklistCode}','{checklistParm.content}','{checklistParm.creater}','{checklistParm.startTime}','{checklistParm.endTime}','{checklistParm.ischecked}','{checklistParm.isPrivate}','{checklistParm.isDeleted}')";
            }

            else if (queryName == "AddCalendar")
            {
                CalendarParm calendarParm = (CalendarParm)parmObj;
                query = $"call {AddCalendar}('{calendarParm.serverCode}','{calendarParm.channelCode}','{calendarParm.content}','{calendarParm.creater}','{calendarParm.startTime}','{calendarParm.endTime}','{calendarParm.isPrivate}')";
            }
            else if (queryName == "GetCalendar")
            {
                CalendarParm calendarParm = (CalendarParm)parmObj;
                query = $"call {GetCalendar}('{calendarParm.serverCode}','{calendarParm.channelCode}','{calendarParm.calendarCode}')";
            }
            else if (queryName == "SetCalendar")
            {
                CalendarParm calendarParm = (CalendarParm)parmObj;
                query = $"call {SetCalendar}('{calendarParm.serverCode}','{calendarParm.channelCode}','{calendarParm.calendarCode}','{calendarParm.content}','{calendarParm.creater}','{calendarParm.startTime}','{calendarParm.endTime}','{calendarParm.isPrivate}','{calendarParm.isDeleted}')";
            }
            

            return query;
        }

        public bool processNoResultQuery(string query, MySqlConnection conn)
        {
            try
            {
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

        public DataTable processResultQuery(string query, MySqlConnection conn)
        {
            // SQL Procedure 결과를 저장할 데이터 테이블 객체
            var datatable = new DataTable();

            try
            {
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
    }
}

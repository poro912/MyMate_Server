using System.Data;
using ServerNetwork.Module;
using System.Net.Sockets;
using Protocol;
using Protocol.Protocols;
using ReceiveResult = System.Collections.Generic.KeyValuePair<byte, object?>;

namespace ServerSystem
{
    public class UserClient :
        Client
    {
        private int? userCode;
        private bool isLogin;
        private readonly Semaphore semaphore = new(1, 1);
        private List<byte>? bytes = new();
        private ReceiveResult result;

        public UserClient(TcpClient tcpClient) :
            base(tcpClient)
        {
            receive.ReceiveEvent += WakeUp;

            start();

            BeforeLoginEvent.ConnectCheckEvent += CheckConnection;
        }

        private void WakeUp()
        {
            Console.WriteLine(userCode + ": Receive event is occurred.");

            if (!tcpClient.Connected)
            {
                Disconnection();
                return;
            }

            if (!semaphore.WaitOne(10))
            {
                return;
            }

            Console.WriteLine(userCode + ": Semaphore is assigned.");

            if (!receive.isEmpty())
            {
                if (isLogin)
                {
                    AfterLogin();
                }
                else
                {
                    BeforeLogin();
                }
            }

            Console.WriteLine(userCode + ": Semaphore is returned.");

            semaphore.Release();
        }

        private void BeforeLogin()
        {
            while (receive.isEmpty())
            {
                receive.Pop(out bytes);
                result = Converter.Convert(bytes);

                if (result.Value == null)
                {
                    continue;
                }

                switch (result.Key)
                {
                    case DataType.LOGIN:
                        Login(result);
                        break;
                }
            }
        }

        private void AfterLogin()
        {
            while (receive.isEmpty())
            {
                receive.Pop(out bytes);
                result = Converter.Convert(bytes);

                if (result.Value == null)
                {
                    continue;
                }

                switch (result.Key)
                {
                    case DataType.LOGOUT:
                        Logout(result);
                        break;
                }
            }
        }

        private void Login(ReceiveResult result)
        {
            var login = result.Value as LoginProtocol.Login;

            if (login == null)
            {
                return;
            }
            
            Console.WriteLine(userCode + ": Logging in...");
            Console.WriteLine(userCode + ": ID " + login.id + " has been attempted.");

            SQL sql = new();

            if (sql.noResultConnectDB(login.id, login.pw, sql.Login))
            {
                DataTable dataTable = new();

                dataTable = sql.resultConnectDB(login.id, sql.GetUserinfo);

                if (dataTable == null)
                {
                    bytes = new();
                    Generater.Generate("ID does not exist.", ref bytes);
                    SendData(bytes);
                }
                else
                {
                    isLogin = true;
                    userCode = (int)dataTable.Rows[0]["U_code"];

                    var container = LoginContainer.Instance;
                    
                    container.registUser((int)userCode, this);

                    var id = dataTable.Rows[0]["U_id"].ToString();
                    var name = dataTable.Rows[0]["U_name"].ToString();
                    var nickname = dataTable.Rows[0]["U_Nickname"].ToString();
                    var phone = dataTable.Rows[0]["U_phone"].ToString();

                    UserInfoProtocol.User user = new((int)userCode, id, name, nickname, phone);
                    bytes = new();
                    Generater.Generate(user, ref bytes);
                    SendData(bytes);

                    BeforeLoginEvent.ConnectCheckEvent -= CheckConnection;
                    
                    Console.WriteLine(userCode + ": Login succeeded.");
                }
            }
        }

        private void Logout(ReceiveResult result)
        {
            var logout = result.Value as LogoutProtocol.Logout;

            if (logout == null)
            {
                return;
            }

            var loginContainer = LoginContainer.Instance;

            if (loginContainer.isLogin(logout.usercode))
            {
                isLogin = false;
                userCode = null;
                
                // 로그인 컨테이너에서 유저 삭제 함수 추가 예정

                bytes = new();
                Generater.Generate("You have been logged out.", ref bytes);
                SendData(bytes);

                BeforeLoginEvent.ConnectCheckEvent += CheckConnection;
            }
        }

        private void SendData(List<byte> bytes)
        {
            try
            {
                send.Data(bytes);
                if (!tcpClient.Connected)
                {
                    Console.WriteLine(userCode + ": User Client is Disconnected.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Disconnection();
                throw;
            }
        }

        private void CheckConnection()
        {
            List<byte> dummy = new();

            isConnectProtocol.IsConnect isConnect = new(1);
            Generater.Generate(isConnect, ref dummy);

            try
            {
                SendData(dummy);
                if (!tcpClient.Connected)
                {
                    Console.WriteLine(userCode + ": User Client is Disconnected.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Disconnection();
                throw;
            }
        }

        private void Disconnection()
        {
            Console.WriteLine(userCode + ": User Client is Disconnected.");

            isLogin = false;
            userCode = null;

            BeforeLoginEvent.ConnectCheckEvent -= CheckConnection;
            receive.ReceiveEvent -= WakeUp;

            receive.Stop();
        }
    }
}
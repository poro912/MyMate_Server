using MyMate_Server;
using MyMate_Server.ServerModule;
using ServerNetwork;
using ServerSystem;

Server server = Server.Instance;
server.clientAccept = AcceptProcess.AccpetRun;

LoginContainer login = LoginContainer.Instance;

while (true)
{
    Thread.Sleep(5000);

    //BeforeLoginEvent.ConnectCheck();


    //beforeClient=before.Check();

    // 로그인 된 클라이언트가 있다면
    //if(null!=beforeClient)
    //{
    // beforeLogin 종료
    //	beforeClient.Delete();
    //	login.registUser(beforeClient.usercode, new(beforeClient.client));
    //}
}



// 클래스 만들어서 스레드

// 통신, db

do
{

}
while (true);
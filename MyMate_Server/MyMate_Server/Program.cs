using MyMate_Server.ServerModule;

using ServerNetwork.Module;

Server server = Server.Instance;

SQL sql = new();

//Console.WriteLine(sql.Check("sky","1234","haneul","sky","010-5566-7788",sql.Signin));

//Console.WriteLine(sql.noResultConnectDB("sky","1234",sql.Login);

Console.WriteLine(sql.resultConnectDB("sky",sql.GetUserinfo));

do
{
    // 스레드로 처리
}
while (true);
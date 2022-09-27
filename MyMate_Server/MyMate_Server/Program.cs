using MyMate_Server.ServerModule;

using MyMate_Network;

Server server = Server.Instance;

SQL sql = new();

//Console.WriteLine(sql.Check("sky","1234","haneul","sky","010-5566-7788",sql.CallSigninSP));

//Console.WriteLine(sql.noResultConnectDB("sky","1234",sql.CallLoginSF));

Console.WriteLine(sql.resultConnectDB("sky",sql.CallGetUserinfoSP));

do
{
    // Server.Start();
}
while (true);
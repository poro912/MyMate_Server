using MyMate_Server.ServerModule;

using MyMate_Network;

Server server = Server.Instance;


SQL sql = new();

Console.WriteLine(sql.SignIn("mod", "1234", "rara", "roro", "010-3333-4444"));

Console.WriteLine(sql.Login("test", "1234"));

do
{
    //Server.Start();
}
while (true);
using MyMate_Server.ServerModule;

using MyMate_Network;

Server server = Server.Instance;

SQL sql = new();

//Console.WriteLine(sql.Signin("ing", "1234", "gaga", "gogo", "010-0258-8520"));

//Console.WriteLine(sql.Login("test", "1234"));

Console.WriteLine(sql.GetUserInfo("mod"));

//Console.WriteLine(sql.GetProfileInfo("mod"));

//Console.WriteLine(sql.Modify("new","1234", "yaya", "yoyo", "010-9999-0000"));

//Console.WriteLine(sql.Check("up","1234","wawa","wowo","010-2468-3579",sql.SigninInsert));

do
{
    // Server.Start();
}
while (true);
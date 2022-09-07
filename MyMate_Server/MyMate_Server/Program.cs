using MyMate_Server.ServerModule;

SQL sql = new();

Console.WriteLine(sql.SignIn("mod", "1234", "rara", "roro", "010-3333-4444"));

Console.WriteLine(sql.Login("test", "1234"));

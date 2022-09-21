using System.Data.SQLite;
using System.Net.NetworkInformation;

SQLiteConnection sqlite_conn = CreateConnection();

if (sqlite_conn != null)
{
    if (CheckData(sqlite_conn))
    {
        CreateTable(sqlite_conn);
        InsertData(sqlite_conn, "VLADI_HOME", "46.35.184.210");
    }
}

//string line = Console.ReadLine();

//while (line != "END")
//{
//    string name = line;
//    string ip = Console.ReadLine();

//    InsertData(sqlite_conn, name, ip);

//    line = Console.ReadLine();
//}


SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand();
sqlite_cmd.CommandText = "SELECT * FROM PingTable";

SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();


while (true)
{
    while (sqlite_datareader.Read())
    {

        string ip = sqlite_datareader.GetString(2);

        if (await PingAsync(ip))
        {
            Console.WriteLine(true);
        }
        else
        {
            Console.WriteLine(false);
        }

        Console.WriteLine(sqlite_datareader.GetString(1) + " " + sqlite_datareader.GetString(2));
        Console.WriteLine(Environment.NewLine);
    }

    sqlite_datareader.Close();

    Console.Write("Waith: ");
    for (int i = 49; i < 58; i++)
    {
        Console.Write((char)i);
        await Task.Delay(1000);
        Console.Write("\b");
    }

    Console.WriteLine(Environment.NewLine);

    sqlite_datareader = sqlite_cmd.ExecuteReader();
}

static async Task<bool> PingAsync(string ipAddress)
{
    //var hostUrl = "www.code4it.dev";

    Ping ping = new();

    PingReply result = await ping.SendPingAsync(ipAddress, 128);
    return result.Status == IPStatus.Success;
}

static SQLiteConnection CreateConnection()
{
    // Create a new database connection:
    SQLiteConnection sqlite_conn = new("Data Source=database.db; Version = 3; New = True; Compress = True; ");
    // Open the connection:
    try
    {
        sqlite_conn.Open();
    }
    catch (Exception)
    {
        return null;
    }

    return sqlite_conn;
}

static bool CheckData(SQLiteConnection conn)
{
    try
    {
        SQLiteCommand sqlite_cmd = conn.CreateCommand();
        sqlite_cmd.CommandText = "SELECT * FROM PingTable";

        SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();
        return false;
    }
    catch (Exception)
    {
        return true;
    }
}

static void CreateTable(SQLiteConnection conn)
{
    string Createsql = "CREATE TABLE PingTable (ID INTEGER PRIMARY KEY AUTOINCREMENT, NAME VARCHAR(20), IpAddress VARCHAR(20)); ";
    SQLiteCommand sqlite_cmd = conn.CreateCommand();

    sqlite_cmd.CommandText = Createsql;
    sqlite_cmd.ExecuteNonQuery();
}

static void InsertData(SQLiteConnection conn, string name, string ip)
{
    SQLiteCommand sqlite_cmd = conn.CreateCommand();
    sqlite_cmd.CommandText = $"INSERT INTO PingTable (NAME, IpAddress) VALUES('{name}', '{ip}'); ";
    sqlite_cmd.ExecuteNonQuery();
}
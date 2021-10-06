using System;
using System.Threading;
using System.Text.Json;

namespace Server
{
    class Program
    {
    static ServerObject server; // сервер
    static Thread listenThread; // потока для прослушивания
    static void Main(string[] args)
    {
        try
        {
            server = new ServerObject();
            listenThread = new Thread(new ThreadStart(server.Listen));
            listenThread.Start(); //старт потока
            Console.Read();
        }
        catch (Exception ex)
        {
            server.Disconnect();
            Console.WriteLine(ex.Message);
        }
    }
}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class ServerObject
    {
        static TcpListener tcpListener; // сервер для прослушивания
        internal ServerInfo serverInfo = new ServerInfo();
        protected internal void AddConnection(ClientObject clientObject)
        {
            serverInfo.clients.Add(clientObject);
        }
        protected internal void RemoveConnection(string id)
        {
            // получаем по id закрытое подключение
            ClientObject client = serverInfo.clients.FirstOrDefault(c => c.Id == id);
            // и удаляем его из списка подключений
            if (client != null)
                serverInfo.clients.Remove(client);
        }
        // прослушивание входящих подключений
        async protected internal void Listen()
        {  
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
                    processClient(tcpClient);
                    Console.WriteLine("Сервер подключает!");
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }
        async Task processClient(TcpClient c)
        {
            using (ClientObject clientObject = new ClientObject(c, this))
                await clientObject.ProcessAsync();


        }

        // трансляция сообщения подключенным клиентам
        protected internal void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < serverInfo.clients.Count; i++)
            {
                if (serverInfo.clients[i].Id != id) // если id клиента не равно id отправляющего
                {
                    serverInfo.clients[i].Stream.Write(data, 0, data.Length); //передача данных
                }
            }
        }
        // отключение всех клиентов
        protected internal void Disconnect()
        {
            tcpListener.Stop(); //остановка сервера

            for (int i = 0; i < serverInfo.clients.Count; i++)
            {
                serverInfo.clients[i].Close(); //отключение клиента
            }
            Environment.Exit(0); //завершение процесса
        }
    }
}

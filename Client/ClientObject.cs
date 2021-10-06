using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;
namespace Server
{
    public class ClientObject: IDisposable
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        ClientInfo clientInfo;
        TcpClient client;
        ServerObject server; // объект сервера

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();//Перенести в ClientInfo
            clientInfo = new ClientInfo();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        public async Task ProcessAsync()
        {
            try
            {
                Stream = client.GetStream();
                string message;
                while (true)
                {
                    try
                    {
                        message = await GetMessage();
     
                        if (message == "")
                            throw new ArgumentException();
                        ReadMessageFromClient(message);
                    }
                    catch
                    {
                        message = String.Format("{0}: покинул чат", clientInfo.name);
                        Console.WriteLine(message);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        // чтение входящего сообщения и преобразование в строку
         async Task<String> GetMessage()
        {
            byte[] data = new byte[64]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes =  await Stream.ReadAsync(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        void SendMessageToClient(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            Stream.Write(data, 0, data.Length);
            Console.WriteLine("Отправлен:" + message);
                
        }


        void ReadMessageFromClient(string json_string)
        {
            ServerMessage message = JsonSerializer.Deserialize<ServerMessage>(json_string);
            defineRequest(message);
        }

        void defineRequest(ServerMessage message)
        {
            string state = message.status;
            switch(state)
            { 
                case "inMenu":
                    var menuRequest = new MenuRequest();
                    break;
                case "inGame":
                    var gamePlayRequest = new GamePlayRequst();
                    break;
                case "Аuthorization":
                    var autorizeRequest = new AutorizeRequest(message.command,message.data, clientInfo);
                    string answerToClient = autorizeRequest.callHandler();
                    SendMessageToClient(answerToClient);
                    break;

            }

        }

        //void Authorization(ServerMessage message)
        //{
        //    clientInfo.name = message.data;
        //    clientInfo.statusClient = ClientInfo.StatusClient.InMenu;
        //    Console.WriteLine("Подключился клиент с именем:{0}", clientInfo.name);

        //}

        //void InMenuFunc(ServerMessage message)
        //{
        //    switch(message.command)
        //    {
        //        case "CreateRoom":
        //            server.serverInfo.createRoom(message.data);
        //            SendMessageToClient("Комната создана!");
        //            break;
        //    }

        //}


        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

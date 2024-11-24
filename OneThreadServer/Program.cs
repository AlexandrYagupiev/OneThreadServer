using System.Net;
using System.Net.Sockets;
using System.Text;

namespace OneThreadServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Однопоточный сервер запущен");
            //подготавливаем конечную точку для сокета
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress iPAddr = ipHost.AddressList[0];
            IPEndPoint iPEndPoint = new IPEndPoint(iPAddr, 8888);

            //создаем потоковый сокет, протокол TCP/IP
            Socket sock = new Socket(iPAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                //связываем сокет с конечной точкой
                sock.Bind(iPEndPoint);
                //начинаем прослушку сокета
                sock.Listen(10);

                //начинаем слушать соединения
                while(true)
                {
                    Console.WriteLine("Слушаем, порт {0}", iPEndPoint);

                    //программа приостанавливается входящее соединение
                    //сокет для обмена данными с клиентом
                    Socket s = sock.Accept();

                    //данные от клиента
                    string data = null;

                    //считываем данные от клиента
                    byte[] bytes = new byte[1024];
                    //длинна полученых данных
                    int bytesCount= s.Receive(bytes);

                    //декодируем строку
                    data += Encoding.UTF8.GetString(bytes, 0, bytesCount);

                    //ооказываем данные в консоли
                    Console.Write("Данные от клиента: "+data+"\n\n");

                    //отправка ответа клиенту
                    string replay = "Quary size: " + data.Length.ToString() + " chars";

                    //кодируем ответ сервера
                    byte[] msg = Encoding.UTF8.GetBytes(replay);

                    //отправка ответа сервера
                    s.Send(msg);

                    if(data.IndexOf("<TheEnd>")>-1)
                    {
                        Console.WriteLine("Содинение завершено");
                        break;
                    }
                    s.Shutdown(SocketShutdown.Both);
                    s.Close();
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }

        }
    }
}

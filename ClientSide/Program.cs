﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Channels;
using MyClasses;

class Program
{
    static void Main()
    {

        // TcpClient ::
        var remote_ip = IPAddress.Parse("127.0.0.1");
        var remote_ip_port = 27001;

        var client = new TcpClient();
        client.Connect(remote_ip, remote_ip_port);

        var stream = client.GetStream();



        // BinaryFormatter class object to convert class object to byte and vise versa
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        Task.Run(() =>
        {
            while (true)
            {
                byte[] data = new byte[client.ReceiveBufferSize];
                int bytesRead = stream.Read(data, 0, data.Length);

                Car received_car = ByteToObject(data, bytesRead);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n\nResponse:");
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine($"{received_car}\n");

                Console.Write("[1] [Get]\n");
                Console.Write("[2] [Post]\n");
                Console.Write("[3] [Put]\n");
                Console.Write("[4] [Delete]\n");
                Console.Write("Your Choice: ");
            }
        });


        while (true)
        {
                byte[] data = null;
                var request = new MyCommand();
            while (true)
            {
                Console.Write("[1] [Get]\n");
                Console.Write("[2] [Post]\n");
                Console.Write("[3] [Put]\n");
                Console.Write("[4] [Delete]\n");
                Console.Write("Your Choice: ");
                var method = Console.ReadLine();
                Console.Write("Car Id: ");
                int id = Convert.ToInt32(Console.ReadLine());
                Console.Clear();


                if (method == "1")
                {
                    request.HttpMethod = HttpMethods.Get;
                    request.Car = new Car() { Id = id };
                    data = ObjectToByte(request);
                    break;
                }
                else if (method == "2")
                {
                    request.HttpMethod = HttpMethods.Post;
                    request.Car = new Car() { Id = id };
                    data = ObjectToByte(request);
                    break;
                }
                else if (method == "3")
                {
                    request.HttpMethod = HttpMethods.Put;
                    request.Car = new Car() { Id = id };
                    data = ObjectToByte(request);
                    break;
                }
                else if (method == "4")
                {
                    request.HttpMethod = HttpMethods.Delete;
                    request.Car = new Car() { Id = id };
                    data = ObjectToByte(request);
                    break;
                }

            }

            stream.Write(data, 0, data.Length);
        }



        byte[] ObjectToByte(MyCommand request)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, request);

                byte[] data = memoryStream.ToArray();

                return data;
            }
        }

        Car ByteToObject(byte[] data, int bytesread)
        {
            // Deserialize the byte array into an object
            using (MemoryStream memoryStream = new MemoryStream(data, 0, bytesread))
            {
                memoryStream.Position = 0;

                Car receivedObject = (Car)binaryFormatter.Deserialize(memoryStream);

                return receivedObject;
            }
        }
    }
}

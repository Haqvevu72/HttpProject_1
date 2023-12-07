using Bogus;
using Bogus.DataSets;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using MyClasses;
#pragma warning disable


class Program
{
    static void Main()
    {
        // List of 'Car' s :

        List<Car> MyCars = new List<Car>
        {
            new Car { Id = 1, Make = "Toyota", Model = "Camry", Year = 2020, VIN = "ABC123", Color = "Blue" },
            new Car { Id = 2, Make = "Honda", Model = "Accord", Year = 2021, VIN = "DEF456", Color = "Red" },
            new Car { Id = 3, Make = "Ford", Model = "Mustang", Year = 2022, VIN = "GHI789", Color = "Yellow" },
            new Car { Id = 4, Make = "Chevrolet", Model = "Malibu", Year = 2020, VIN = "JKL012", Color = "Silver" },
            new Car { Id = 5, Make = "Nissan", Model = "Sentra", Year = 2021, VIN = "MNO345", Color = "Green" },
            new Car { Id = 6, Make = "Jeep", Model = "Cherokee", Year = 2022, VIN= "PQR678", Color = "Orange" },
            new Car { Id = 7, Make = "Hyundai", Model = "Elantra", Year = 2019, VIN = "STU901", Color = "Black" },
            new Car { Id = 8, Make = "Kia", Model = "Soul", Year = 2020, VIN = "VWX234", Color = "White" },
            new Car { Id = 9, Make = "Subaru", Model = "Outback", Year = 2022, VIN = "YZA567", Color = "Purple" },
            new Car { Id = 10, Make = "Mazda", Model = "CX-5", Year = 2021, VIN = "BCD890", Color = "Gray" },
            new Car { Id = 11, Make = "Tesla", Model = "Model 3", Year = 2022, VIN = "EFG123", Color = "Silver" },
            new Car { Id = 12, Make = "Audi", Model = "Q5", Year = 2020, VIN = "HIJ456", Color = "Blue" },
            new Car { Id = 13, Make = "BMW", Model = "X3", Year = 2021, VIN= "KLM789", Color = "Red" },
            new Car { Id = 14, Make = "Mercedes-Benz", Model = "C-Class", Year = 2022, VIN = "NOP012", Color = "Yellow" },
            new Car { Id = 15, Make = "Lexus", Model = "RX", Year = 2020, VIN = "QRS345", Color = "Silver" },
            new Car { Id = 16, Make = "Volvo", Model = "XC60", Year = 2021, VIN = "TUV678", Color = "Green" },
            new Car { Id = 17, Make = "Porsche", Model = "911", Year = 2022, VIN = "WXY901", Color = "Orange" },
            new Car { Id = 18, Make = "Jaguar", Model = "F-PACE", Year = 2019, VIN = "ZAB234", Color = "Black" },
            new Car { Id = 19, Make = "Land Rover", Model = "Discovery", Year = 2020, VIN = "BCD567", Color = "White" },
            new Car { Id = 20, Make = "Ferrari", Model = "488 GTB", Year = 2022, VIN = "EFG890", Color = "Red" }
        };




        // Methods :

        Car? GetbyId(int Id) { return null; }

        List<Car>? GetAll() { return null; }

        bool Add(Car car) { return false; }

        bool Delete(Car car) { return false; }

        bool Update(Car car) { return false; }





        // TcpListener ::
        var listener_ip = IPAddress.Parse("127.0.0.1");
        var listener_ip_port = 27001;


        var listener = new TcpListener(listener_ip, listener_ip_port);

        listener.Start(1);

        // BinaryFormatter class object to convert class object to byte and vise versa
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        while (true)
        {
            var client = listener.AcceptTcpClient();
            Console.WriteLine($"{client.Client.RemoteEndPoint} Connected.... ");
            var stream = client.GetStream();


            Task.Run(() =>
            {
                while (true)
                {
                    // Receive the byte array from the network stream
                    byte[] data = new byte[client.ReceiveBufferSize];
                    int bytesRead = stream.Read(data, 0, data.Length);
                    

                    MyCommand myCommand = ByteToObject(data, bytesRead);

                    bool IsCarFound = false;
                    if (myCommand.HttpMethod == HttpMethods.Get)
                    {
                        // Checking whether car exist or not 

                        for (int i = 0; i < MyCars.Count; i++)
                        {
                            if (MyCars[i].Id == myCommand.Car.Id)
                            {
                                MyCars[i].status = Car.Status.None;
                                byte[] car_bytes = ObjectToByte(MyCars[i]);
                                stream.Write(car_bytes, 0, car_bytes.Length);
                                Console.WriteLine("Car Has Sent !");

                                IsCarFound = true;
                                break;
                            }
                        }

                        if (IsCarFound == false)
                        {
                            Console.WriteLine("Car not found !");
                        }
                    }
                    else if (myCommand.HttpMethod == HttpMethods.Post)
                    {
                        // Post method does not check updates , just add 

                        MyCars.Add(myCommand.Car);
                        myCommand.Car.status= Car.Status.Added;
                        byte[] car_bytes = ObjectToByte(myCommand.Car);
                        stream.Write(car_bytes, 0, car_bytes.Length);
                        Console.WriteLine("Car Has Added !");
                    }
                    else if (myCommand.HttpMethod == HttpMethods.Put)
                    {
                        // Checking whether car exist or not 

                        for (int i = 0; i < MyCars.Count; i++)
                        {
                            // If exist 

                            if (MyCars[i].Id == myCommand.Car.Id)
                            {
                                // Checking any changes

                                if (MyCars[i].Make != myCommand.Car.Make ||
                                    MyCars[i].Model != myCommand.Car.Model ||
                                    MyCars[i].Year != myCommand.Car.Year ||
                                    MyCars[i].VIN != myCommand.Car.VIN ||
                                    MyCars[i].Color != myCommand.Car.Color)
                                {
                                    // Add modified car
                                    
                                    MyCars[i] = myCommand.Car;
                                    myCommand.Car.status = Car.Status.Added;
                                    byte[] car_bytes = ObjectToByte(myCommand.Car);
                                    stream.Write(car_bytes, 0, car_bytes.Length);
                                    Console.WriteLine("Car Has Added !");
                                }
                                IsCarFound = true;
                                break;
                            }
                        }
                        if (IsCarFound == false)
                        {
                            MyCars.Add(myCommand.Car);
                            myCommand.Car.status = Car.Status.Added;
                            byte[] car_bytes = ObjectToByte(myCommand.Car);
                            stream.Write(car_bytes, 0, car_bytes.Length);
                            Console.WriteLine("Car Has Added !");
                        }

                    }
                    else if (myCommand.HttpMethod == HttpMethods.Delete)
                    {
                        // Checking whether car exist or not 
                        
                        for (int i = 0; i < MyCars.Count; i++)
                        {
                            if (MyCars[i].Id == myCommand.Car.Id)
                            {
                                myCommand.Car.status = Car.Status.Deleted;
                                byte[] car_bytes = ObjectToByte(myCommand.Car);
                                stream.Write(car_bytes, 0, car_bytes.Length);
                                MyCars.Remove(MyCars[i]);
                                IsCarFound = true;
                                Console.WriteLine("Car Has Deleted !");
                                break;
                            }
                        }

                        if (IsCarFound == false)
                        {
                            Console.WriteLine("Car not found !");
                        }
                    }

                }
            });

        }

        MyCommand ByteToObject(byte[] data, int bytesread)
        {
            // Deserialize the byte array into an object
            using (MemoryStream memoryStream = new MemoryStream(data, 0, bytesread))
            {
                memoryStream.Position = 0;

                MyCommand receivedObject = (MyCommand)binaryFormatter.Deserialize(memoryStream);

                return receivedObject;
            }
        }

        byte[] ObjectToByte(Car car)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, car);

                byte[] data = memoryStream.ToArray();

                return data;
            }
        }
    }
}


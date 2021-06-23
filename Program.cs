using FluentFTP;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PTTParser
{
    class Program
    {
 

        static void Main(string[] args)
        {
           


            Facotry pool = new Facotry();


            for (int i = 0; i < 10; i++)
            {
                var conn = pool.GetFtpConnection();
 

                var client = conn.ftpClie;
                client.Connect();
                if (client.IsConnected)
                {
                    Console.WriteLine($"client is connect = {conn.Name} ");
                    if (client.FileExists("/test/test.txt")) 
                    {
                        Console.WriteLine(" FileExists ");
                        pool.RecycleObject(conn);
                    }
                }
                else
                {
                    Console.WriteLine(0);
                }

            }



            Console.Read();

        }
 
    }


    public class Facotry
    {
        private static int _PoolMaxSize = 3;

        private static readonly Queue objPool = new Queue(_PoolMaxSize);

        public FtpFactory GetFtpConnection()
        {
            FtpFactory obj;
            if (FtpFactory.ObjectCounter >= _PoolMaxSize &&  objPool.Count > 0)
            {
                Console.WriteLine("old object");
                obj = RetrieveFromPool();
            }
            else
            {
                Console.WriteLine("new object");
                obj = GetNewFtpObj();
            }
            return obj;
        }

        private FtpFactory GetNewFtpObj()
        {
            FtpFactory fa = new FtpFactory();
            objPool.Enqueue(fa);
            return fa;
        }

        public void RecycleObject(FtpFactory fa)
        {
            objPool.Enqueue(fa);
        }

        protected FtpFactory RetrieveFromPool()
        {
            FtpFactory stu;
            if (objPool.Count > 0)
            {
                stu = (FtpFactory)objPool.Dequeue();
                FtpFactory.ObjectCounter--;
            }
            else
            {
                stu = new FtpFactory();
            }
            return stu;
        }
    }

    public class FtpFactory
    {

         public static int ObjectCounter = 0;
        FtpClient client = null;
        public FtpFactory()
        {
             ++ObjectCounter;
            client = new FtpClient("127.0.0.1");
            client.Credentials = new NetworkCredential("ftpuser", "ftpuser");
            
        }

        public FtpClient ftpClie
        {
            get { return client; }
        }

        public string Name
        {
            get { return ObjectCounter.ToString(); }

        }
    }
 
}

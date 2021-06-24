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
        private static FtpClientPool p = new FtpClientPool();

        static void Main(string[] args)
        {

            // Practice 2

            FtpClient client =  p.borrowObject();


            Parallel.For(0, 20, i => {
                if (client.IsConnected)
                {
                    if (client.FileExists("/test/test.txt"))
                    {
                        Console.WriteLine("FileExists ");

                    }
                }
                else
                {
                    Console.WriteLine("client not connect");
                }


            });
            
             

           var poosize = p.PoolSize();

            if (p.PoolSize() > 0)
            {
                p.disposePool();
            }


            Console.WriteLine("Finish");
            //Console.Read();

        }
 
    }



    public class FtpClientPool
    {
        private static int defaultPoolSize = 2;
        private static readonly ConcurrentBag<FtpClient> objPool = new ConcurrentBag<FtpClient>();

        private static FtpClientFactory factory = new FtpClientFactory();

        public FtpClientPool()
        {
            initPool();
        }

        public int PoolSize()
        {
            return objPool.Count;
        }

        public void initPool()
        {
            for(int i=0; i< defaultPoolSize; i++)
            {
                FtpClient client = new FtpClient("127.0.0.1", "ftpuser", "ftpuser");
                client.Connect();
                objPool.Add(client);
            }
            Console.WriteLine("Initial pool size = "+objPool.Count);
        }

        public void disposePool()
        {
            while (objPool.Count > 0)
            {
                FtpClient client;
                if(objPool.TryTake(out client))
                {
                    client.Disconnect();
                    client.Dispose();
                }
            }
        }

        public FtpClient borrowObject()
        {
            FtpClient client;

            if(objPool.TryPeek(out client))
            {
                if (!client.IsConnected)
                {
                    factory.destoryObject(client);
                    client = factory.makeObject();
                    objPool.Add(client);
                }
            }
            return client;
        }

    }

    public class FtpClientFactory
    {
        public FtpClient makeObject()
        {
            FtpClient ftpClient = new FtpClient("127.0.0.1", "ftpuser", "ftpuser");
            //ftpClient.Connect();
            ftpClient.Connect();
            return ftpClient;
        }

        public void destoryObject(FtpClient ftpClient)
        {
            try
            {
                if (ftpClient.IsConnected)
                {
                    ftpClient.Disconnect();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                ftpClient.Dispose();
            }
        }
    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace SynapseTrack_Resident
{
    class Socket : IDisposable
    {
        const string HOST = "127.0.0.1";
        const int PORT = 8111;
        TcpClient client;
        NetworkStream stream;

        public Socket()
        {
            client = new TcpClient();
            client.Connect(HOST, PORT);
            stream = client.GetStream();
        }

        public JointInfo RequestJoint()
        {
            JointInfo ret;
            byte[] buf1 = Encoding.UTF8.GetBytes("102");
            Console.WriteLine("send id: {0}", string.Join(",", buf1));
            stream.Write(buf1, 0, buf1.Length);
            byte[] buf2 = new byte[2048];
            stream.Read(buf2, 0, buf2.Length);
            ret = JointInfo.FromBytes(buf2);
            Console.WriteLine(ret.ToString());
            return ret;
        }

        public void Dispose()
        {
            stream.Close();
            client.Close();
        }
    }
}

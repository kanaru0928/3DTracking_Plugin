using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;

namespace SynapseTrack_Resident
{
    class GotJointsEventArgs : EventArgs {
        public JointInfo JointInfo;
    }

    /// <summary>
    /// TCP通信を行うクラス
    /// </summary>
    class Socket : IDisposable
    {
        const string HOST = "127.0.0.1";
        const int PORT = 8111;
        const int RETURN_SIZE = 2048;

        TcpClient client;
        NetworkStream stream;
        bool watching = false;

        public delegate void GotJointsEventHandler(object sender, GotJointsEventArgs e);
        public event GotJointsEventHandler GotJoints;

        /// <summary>
        /// 関節を受け取ったときに発火するイベント(未使用)
        /// </summary>
        /// <param name="e">イベント引数</param>
        protected virtual void OnGotJoints(GotJointsEventArgs e)
        {
            GotJointsEventHandler handler = GotJoints;
            handler?.Invoke(this, e);
        }

        public Socket()
        {
            client = new TcpClient();
            client.Connect(HOST, PORT);
            stream = client.GetStream();
        }

        /// <summary>
        /// 関節の更新を監視(未使用)
        /// </summary>
        public void WatchJoint()
        {
            watching = true;
            byte[] startId = Encoding.UTF8.GetBytes("104");
            stream.Write(startId, 0, startId.Length);
            Thread task = new Thread(() =>
            {
                while (stream.CanRead && watching)
                {
                    byte[] streamReturn = new byte[RETURN_SIZE];
                    stream.Read(streamReturn, 0, streamReturn.Length);
                    GotJointsEventArgs args = new GotJointsEventArgs();
                    args.JointInfo = JointInfo.FromBytes(streamReturn);
                    OnGotJoints(args);
                }
                MessageBox.Show("Stream closed");
                stream.Close();
                client.Close();
            });
            task.Start();
        }

        /// <summary>
        /// 関節の推定を開始
        /// </summary>
        /// <returns>成功なら <c>true</c> 失敗なら <c>false</c></returns>
        public bool StartEstimation()
        {
            byte[] buf = Encoding.UTF8.GetBytes("106");
            stream.Write(buf, 0, buf.Length);
            byte[] ret = new byte[16];
            stream.Read(ret, 0, ret.Length);
            string str = Encoding.UTF8.GetString(ret);
            str = str.TrimEnd('\0');
            return str == "106OK";
        }

        /// <summary>
        /// 関節の推定を停止
        /// </summary>
        public void StopEstimation()
        {
            byte[] buf = Encoding.UTF8.GetBytes("105");
            stream?.Write(buf, 0, buf.Length);
        }

        /// <summary>
        /// 関節を要求
        /// </summary>
        /// <returns>サーバーから返された関節情報</returns>
        public JointInfo RequestJoint()
        {
            JointInfo ret;
            byte[] buf1 = Encoding.UTF8.GetBytes("102");
            Console.WriteLine("send id: {0}", string.Join(",", buf1));
            stream.Write(buf1, 0, buf1.Length);
            byte[] buf2 = new byte[RETURN_SIZE];
            stream.Read(buf2, 0, buf2.Length);
            ret = JointInfo.FromBytes(buf2);
            Console.WriteLine(ret.ToString());
            return ret;
        }

        public void Dispose()
        {
            watching = false;
            stream?.Close();
            client?.Close();
        }
    }
}

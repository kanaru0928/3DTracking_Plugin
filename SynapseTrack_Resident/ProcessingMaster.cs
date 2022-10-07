using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynapseTrack_Resident
{
    /// <summary>
    /// 関節処理を行うクラス
    /// </summary>
    class ProcessingMaster : IDisposable
    {
        Socket socket;
        PreProcessing preProcessing;
        DrawProcessing drawProcessing;

        public bool showed = false;

        public ProcessingMaster()
        {
            preProcessing = new PreProcessing();
            drawProcessing = new DrawProcessing();
        }

        /// <summary>
        /// サーバーとの通信を確立
        /// </summary>
        public void ConnectSocket()
        {
            socket = new Socket();
            socket.GotJoints += Socket_GotJoints;
            //socket.WatchJoint();
            if (!socket.StartEstimation()) throw new Exception("サーバーが不正な値を返しました。");
        }

        /// <summary>
        /// サーバーから関節を受け取った際のイベントを取得(未使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Socket_GotJoints(object sender, GotJointsEventArgs e)
        {
            preProcessing.SetJoints(e.JointInfo);
        }

        /// <summary>
        /// 3Dモデルに現時点の関節情報を適用
        /// </summary>
        /// <param name="sender"></param>
        public void SetJoints(Class1 sender)
        {
            //JointInfo joints = preProcessing.GetJoints(sender);
            JointInfo joints = socket.RequestJoint();
            joints = PreProcessing.CalcRootRot(joints);
            joints = PreProcessing.RotateJoints(joints);
            drawProcessing.SetJoints1(sender, joints);
        }

        /// <summary>
        /// 接続解除
        /// </summary>
        public void DisconnectSocket()
        {
            socket?.StopEstimation();
            socket?.Dispose();
        }

        public void Dispose()
        {
            DisconnectSocket();
        }
    }
}

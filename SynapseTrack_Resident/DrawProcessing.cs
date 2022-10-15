using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using MikuMikuPlugin;
using DxMath;
using System.Collections;

namespace SynapseTrack_Resident
{
    /// <summary>
    /// 描画部のクラス
    /// </summary>
    class DrawProcessing : IDisposable
    {
        bool showed;
        DateTime prevDate = DateTime.Now;
        //Quaternion[,] prevQuaternion = new Quaternion[JointInfo.DEFAULT_NUM_PERSON, JointInfo.NUM_JOINT];

        public DrawProcessing()
        {
        }

        /// <summary>
        /// 2ベクトル間の回転四元数を取得
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        private Quaternion GetQuaternionBetweenVector(Vector3 v1, Vector3 v2)
        {
            Vector3 axis = Vector3.Cross(v1, v2);
            float angle = (float)Math.Acos(Vector3.Dot(v1, v2));
            Quaternion ret = Quaternion.RotationAxis(axis, angle);
            return ret;
        }

        /// <summary>
        /// 指定したボーンの回転四元数を取得
        /// </summary>
        /// <param name="boneName">ボーン名</param>
        /// <param name="bone">ボーンの向きを表すベクトル</param>
        /// <param name="root">親ボーンの向きを表すベクトル</param>
        /// <returns></returns>
        private Quaternion GetQuaternion(string boneName, Vector3 bone, Vector3 root)
        {
            Quaternion quaternion;
            bool hasRoot = Config.ROOT.ContainsKey(boneName);
            Vector3 bone0 = Config.defaultVector[boneName];

            if (hasRoot)
            {
                string rootName = Config.ROOT[boneName];
                Vector3 root0 = Config.defaultVector[rootName];

                Quaternion rootQ = GetQuaternionBetweenVector(root, root0);
                Quaternion boneQ = GetQuaternionBetweenVector(root0, bone);
                Quaternion preQ;
                if (Config.defaultVector[boneName] == Config.defaultVector[rootName])
                {
                    preQ = Quaternion.Identity;
                }
                else
                {
                    preQ = GetQuaternionBetweenVector(bone0, root0);
                }

                //quaternion = preQ * boneQ;
                quaternion = rootQ * boneQ * preQ;
            }
            else
            {
                quaternion = GetQuaternionBetweenVector(bone0, bone);
            }

            if (boneName.Contains("肩P"))
            {
                quaternion = Quaternion.Conjugate(quaternion);
            }
            return quaternion;
        }

        /// <summary>
        /// 関節情報をボーンに適用
        /// </summary>
        /// <param name="sender">呼び出し元のオブジェクト(ログ表示用)</param>
        /// <param name="jointInfo">関節情報</param>
        public void SetJoints1(Class1 sender, JointInfo jointInfo)
        {
            TimeSpan time = DateTime.Now - prevDate;
            sender.userControl.setFps(1000 / time.TotalMilliseconds);
            prevDate = DateTime.Now;

            float[][][] vectors = jointInfo.joints;
            float[][] root_pos = jointInfo.root_pos;
            float[] root_rot = jointInfo.root_rot;
            int num_person = jointInfo.num_person;

            if (jointInfo == JointInfo.GetIdentity())
            {
                return;
            }

            Quaternion[][] memo = new Quaternion[num_person][];
            for (int i = 0; i < memo.Length; i++)
            {
                memo[i] = new Quaternion[JointInfo.NUM_JOINT];
            }

            int pidx = 0;

            //MessageBox.Show(jointInfo.ToString());
            List<UserControl1.Bind> modelInfo = sender.userControl.bindingModels;

            foreach (var model in sender.Scene.Models)
            {
                if (pidx >= modelInfo.Count) continue;
                int motion = sender.userControl.bindingModels[pidx].MotionId;

                if(motion >= 0 && motion < num_person && jointInfo.joints[motion].SequenceEqual(JointInfo.Identity.joints[0]))
                {
                    continue;
                }

                Bone rootBone = model.Bones["全ての親"];

                // ボーン回転
                foreach (DictionaryEntry entry in Config.JOINT_PAIR)
                {
                    string boneName = (string)entry.Key;
                    int boneIndex = (int)entry.Value;

                    Bone currentBone = model.Bones[boneName];

                    if (currentBone != null)
                    {
                        MotionData data = currentBone.CurrentLocalMotion;

                        if (motion < 0 || motion >= num_person)
                        {
                            //data.Rotation = Quaternion.Identity;
                            //currentBone.Layers[0].CurrentLocalMotion = data;
                            continue;
                        }

                        bool hasRoot = Config.ROOT.ContainsKey(boneName);

                        Vector3 bone = Vector3.Normalize(Utils.ArrayToVector(vectors[motion][boneIndex]));
                        Vector3 root = Vector3.Zero;
                        if (hasRoot)
                        {
                            string rootName = Config.ROOT[boneName];
                            root = Vector3.Normalize(Utils.ArrayToVector(vectors[motion][(int)Config.JOINT_PAIR[rootName]]));
                        }
                        // hasRootの時だけ定義する

                        Quaternion quaternion;

                        if (bone == Vector3.Zero)
                        {
                            quaternion = Quaternion.Identity;
                        }
                        else
                        {
                            if (memo[motion][boneIndex] != new Quaternion())
                            {
                                quaternion = memo[motion][boneIndex];
                            }
                            else
                            {
                                quaternion = GetQuaternion(boneName, bone, root);
                                memo[motion][boneIndex] = quaternion;
                            }
                        }

                        if(quaternion != Quaternion.Identity)
                        {
                            data.Rotation = quaternion;
                        }
                        currentBone.Layers[0].CurrentLocalMotion = data;
                    }
                }

                float[] move_f;

                // ルートボーン移動・回転
                if (rootBone != null)
                {
                    MotionData data = rootBone.CurrentLocalMotion;

                    if (motion < 0 || motion >= num_person)
                    {
                        //data.Rotation = Quaternion.Identity;
                        //data.Move = Vector3.Zero;
                        //rootBone.Layers[0].CurrentLocalMotion = data;
                        continue;
                    }
                    move_f = root_pos[motion];

                    //sender.userControl.PrintLine($"[{motion}]RootPos:{Utils.ArrayToVector(root_pos[motion])}");

                    Vector3 move = new Vector3(move_f[0] / 126, 0, move_f[2] / 126 + sender.userControl.offsetZ);
                    Quaternion rot = Quaternion.RotationAxis(Vector3.UnitY, -root_rot[motion]);
                    if (true)
                    {
                        data.Rotation = rot;
                    }
                    else
                    {
                        data.Rotation = Quaternion.Identity;
                    }
                    if (move_f[2] != 0f)
                    {
                        data.Move = move;
                    }
                    rootBone.Layers[0].CurrentLocalMotion = data;
                    if (!showed)
                    {
                        //MessageBox.Show($"{motion}: {root_rot[motion]}rad");
                    }

                }
                else
                {
                    throw new Exception("");
                }

                Bone center = model.Bones["センター"];
                if (center != null)
                {
                    float leg_length = center.InitialPosition.Y;

                    MotionData data = center.CurrentLocalMotion;
                    Vector3 move = new Vector3(0, move_f[1] / 126 - leg_length + 8.0f, 0);
                    data.Move = move;
                    center.Layers[0].CurrentLocalMotion = data;
                }

                pidx++;
            }
            showed = true;
        }

        public void Dispose()
        {
        }
    }
}

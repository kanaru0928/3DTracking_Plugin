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
    class JointProcessing : IDisposable
    {
        Socket socket;
        public bool showed = false;

        public JointProcessing()
        {
        }

        public void ConnectSocket()
        {
            socket = new Socket();
        }

        public void DisconnectSocket()
        {
            try
            {
                socket.Dispose();
            }
            catch { }
        }

        public void SetJoints(Class1 sender)
        {
            JointInfo jointInfo = socket.RequestJoint();
            float[][][] vectors = jointInfo.joints;
            float[][] root_pos = jointInfo.root_pos;
            float[] root_rot = jointInfo.root_rot;
            int num_person = jointInfo.num_person;

            int p = 0;


            foreach (var model in sender.Scene.Models)
            {
                Dictionary<string, Quaternion> quaternions = new Dictionary<string, Quaternion>();
                Bone root = model.Bones["全ての親"];
                if (root != null)
                {
                    MotionData data = root.CurrentLocalMotion;
                    Vector3 move = Vector3.Zero;
                    Quaternion rot = Quaternion.Identity;
                    data.Rotation = rot;
                    data.Move = move;
                    root.Layers[0].CurrentLocalMotion = data;
                }

                if (p >= num_person) break;

                foreach (DictionaryEntry pair in Config.JOINT_PAIR)
                {
                    string key = (string)pair.Key;
                    int val = (int)pair.Value;
                    Bone bone = model.Bones[key];
                    if (bone != null)
                    {
                        MotionData data = bone.CurrentLocalMotion;
                        //if (localvx.X == 0.0f) localvx.X = 1e-2f;
                        //if (localvy.Y == 0.0f) localvy.Y = 1e-2f;
                        //if (localvz.Z == 0.0f) localvz.Z = 1e-2f;

                        float vecx = vectors[p][val][0];
                        float vecy = vectors[p][val][1];
                        float vecz = vectors[p][val][2];

                        Vector3 vec = new Vector3(vecx, vecy, vecz);
                        //MessageBox.Show(vec.ToString());
                        //MessageBox.Show(localm.ToString());
                        //vec = Vector3.TransformNormal(vec, -localm);
                        //string r = key;
                        //while (!Config.ROOT.ContainsKey(r))
                        //{
                        //    r = Config.ROOT[r];
                        //}

                        Vector3 root_vec;
                        bool hasroot = false;

                        if (Config.root_abs_angle.ContainsKey(key))
                        {
                            root_vec = Config.root_abs_angle[key];
                            //localx = Config.root_abs_angle[key].X;
                            //localy = Config.root_abs_angle[key].Y;
                            //localz = Config.root_abs_angle[key].Z;
                            //rotx -= localx;
                            //roty -= localy;

                            //rotz -= localz;
                            //if (val == 2 || val == 5)
                            //{
                            //    roty += root_rot[p];
                            //}
                        }
                        else //if (Config.ROOT.ContainsKey(key))
                        {
                            float[] v = vectors[p][(int)Config.JOINT_PAIR[Config.ROOT[key]]];
                            root_vec = new Vector3(v[0], v[1], v[2]);
                            hasroot = true;
                        }
                        vec.Normalize(); root_vec.Normalize();

                        Vector4 localvx = new Vector4(bone.LocalAxisX, 0);
                        Vector4 localvy = new Vector4(bone.LocalAxisY, 0);
                        Vector4 localvz = new Vector4(bone.LocalAxisZ, 0);
                        if (hasroot)
                        {
                            localvx = Vector4.Transform(localvx, Quaternion.Conjugate(quaternions[Config.ROOT[key]]));
                            localvy = Vector4.Transform(localvx, Quaternion.Conjugate(quaternions[Config.ROOT[key]]));
                            localvz = Vector4.Transform(localvx, Quaternion.Conjugate(quaternions[Config.ROOT[key]]));
                        }
                        Matrix localm = new Matrix();
                        localm.set_Rows(0, localvx);
                        localm.set_Rows(1, localvy);
                        localm.set_Rows(2, localvz);
                        localm.M44 = 1;
                        //localm = Matrix.Invert(localm);

                        Vector3 axis = Vector3.Cross(root_vec, vec);
                        axis.Normalize();
                        axis = Vector3.TransformCoordinate(axis, localm);
                        //if (hasroot)
                        //{
                        //    Vector4 axis4 = Vector3.Transform(axis, Quaternion.Conjugate(quaternions[Config.ROOT[key]]));
                        //    axis = new Vector3(axis4.X, axis4.Y, axis4.Z);
                        //}
                        float angle = (float)Math.Acos(Vector3.Dot(root_vec, vec));
                        Quaternion rot = Quaternion.RotationAxis(axis, angle);
                        //if (Config.negative_joints2.Contains(key))
                        //{
                        //    rot.Z *= -1;
                        //}

                        if(!showed && key == "右ひじ")
                        {
                            MessageBox.Show("vec:" + vec.ToString() + '\n' +
                                "root:" + root_vec.ToString() + '\n' +
                                "rot:" + rot.ToString() + '\n' + 
                                "axis:" + axis.ToString() + '\n' +
                                "angle:" + angle + '\n' +
                                "matrix:" + localm.ToString());
                        }

                        //Quaternion rot = Quaternion.RotationAxis(Vector3.UnitZ, rotz);
                        //rot *= Quaternion.RotationAxis(Vector3.UnitY, roty);
                        //if (Config.ROOT.ContainsKey(key))
                        //{
                        //    float[] a = angles[p][(int)Config.JOINT_PAIR[Config.ROOT[key]]];
                        //    Quaternion q = Quaternion.RotationYawPitchRoll(a[2], a[1], a[0]);
                        //    rot -= q;
                        //    rot.Normalize();
                        //    if(key == "上半身2" && !showed)
                        //    {
                        //        MessageBox.Show("ROOT:" + string.Join(",", a) + "\n" +
                        //            "rot:" + rot);
                        //    }
                        //}
                        if (hasroot)
                        {
                            rot *= quaternions[Config.ROOT[key]];
                        }
                        quaternions.Add(key, rot);

                        if (key.Contains("肩P")/* || Config.negative_joints2.Contains(key)*/)
                        {
                            rot = Quaternion.Conjugate(rot);
                        }
                        data.Rotation = rot;
                        bone.Layers[0].CurrentLocalMotion = data;
                    }
                }

                float[] move_f = root_pos[p];
                if (root != null)
                {
                    MotionData data = root.CurrentLocalMotion;
                    Vector3 move = new Vector3(move_f[0] / 126, 0, -move_f[2] / 126 - 50);
                    Quaternion rot = Quaternion.RotationAxis(Vector3.UnitY, -root_rot[p]);
                    data.Rotation = rot;
                    data.Move = move;
                    root.Layers[0].CurrentLocalMotion = data;
                }

                //Bone center = model.Bones["センター"];
                //if (center != null)
                //{
                //    MotionData data = center.CurrentLocalMotion;
                //    Vector3 move = new Vector3(0, move_f[1] / 126 - leg_length[p] + 8.0f, 0);
                //    data.Move = move;
                //    center.Layers[0].CurrentLocalMotion = data;
                //}

                p++;
            }

            if (!showed)
            {
                showed = true;
                MessageBox.Show(sender.ApplicationForm, jointInfo.ToString());
            }
        }

        public void Dispose()
        {
            DisconnectSocket();
        }
    }
}

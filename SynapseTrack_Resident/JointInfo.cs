using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SynapseTrack_Resident
{
    /// <summary>
    /// 関節情報を持つクラス
    /// </summary>
    /// <remarks>
    /// サーバーのPython実装とほぼ同様
    /// </remarks>
    public class JointInfo
    {
        public static bool showed = false;

        const int FORMAT_VERSION = 0x01;
        public const int NUM_JOINT = 21;
        public const int DEFAULT_NUM_PERSON = 5;

        public float[][][] joints;
        public float[][] root_pos;
        public float[] root_rot;
        public int num_person;

        public JointInfo(JointInfo jointInfo)
        {
            joints = (float[][][])jointInfo.joints.Clone();
            root_pos = (float[][])jointInfo.root_pos.Clone();
            root_rot = (float[])jointInfo.root_rot.Clone();
            num_person = jointInfo.num_person;
        }
        
        /// <summary>
        /// 要素が全て0のインスタンスを作成
        /// </summary>
        /// <param name="num_person">人数</param>
        /// <returns>要素が全て0にインスタンス</returns>
        public static JointInfo GetIdentity(int num_person = DEFAULT_NUM_PERSON)
        {
            float[][][] joints = new float[num_person][][];
            float[][] root_pos = new float[num_person][];
            float[] root_rot = new float[num_person];
            for (int i = 0; i < num_person; i++)
            {
                joints[i] = new float[NUM_JOINT][];
                root_pos[i] = new float[3];
                for (int j = 0; j < NUM_JOINT; j++)
                {
                    joints[i][j] = new float[3];
                }
            }
            return new JointInfo(joints, root_pos, root_rot);
        }

        public static JointInfo Identity { get; } = new JointInfo(GetIdentity());

        public JointInfo(float[][][] joints, float[][] root_pos, float[] root_rot)
        {
            this.joints = (float[][][])joints.Clone();
            this.root_pos = (float[][])root_pos.Clone();
            this.root_rot = (float[])root_rot.Clone();
            this.num_person = joints.Length;
        }

        // public byte[] ToBytes(){

        // }

        /// <summary>
        /// 文字列からインスタンスを生成
        /// </summary>
        /// <param name="str">関節情報を表す文字列</param>
        /// <returns>生成されたインスタンス</returns>
        public static JointInfo FromString(string str)
        {
            JointInfo jointInfo = Identity;
            string[] data = str.Split('/');

            string[] rootRotStr = data[2].Split(';');
            int numPerson = rootRotStr.Length;
            float[] rootRot = new float[numPerson];
            for(int i = 0; i < numPerson; i++)
            {
                rootRot[i] = float.Parse(rootRotStr[i]);
            }

            float[][] rootPos = new float[numPerson][];
            string[] rootPosStr = data[1].Split(';');
            for (int i = 0; i < numPerson; i++)
            {
                string[] tmpStr = rootPosStr[i].Split(' ');
                rootPos[i] = new float[3];
                for(int j = 0; j < 3; j++)
                {
                    rootPos[i][j] = float.Parse(tmpStr[j]);
                }
            }

            float[][][] joints = new float[numPerson][][];
            string[] jointsStr = data[0].Split(';');
            for(int i = 0; i < numPerson; i++)
            {
                string[] tmpStr1 = jointsStr[i].Split(',');
                joints[i] = new float[tmpStr1.Length][];
                for(int j = 0; j < tmpStr1.Length; j++)
                {
                    string[] tmpStr2 = tmpStr1[j].Split(' ');
                    joints[i][j] = new float[3];
                    for(int k = 0; k < 3; k++)
                    {
                        joints[i][j][k] = float.Parse(tmpStr2[k]);
                    }
                }
            }

            Array.Copy(jointInfo.root_rot, rootRot, numPerson);
            Array.Copy(jointInfo.root_pos, rootPos, numPerson);
            Array.Copy(jointInfo.joints, joints, numPerson);
            return jointInfo;
        }

        /// <summary>
        /// 指定した人物の関節情報をループするEnumrator
        /// </summary>
        /// <param name="person">人物のインデックス</param>
        /// <returns>関節情報を表す数字</returns>
        public IEnumerable<float> GetJointEnumerator(int person)
        {
            for (int j = 0; j < NUM_JOINT; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    yield return joints[person][j][k];
                }
            }
        }

        /// <summary>
        /// 全員の関節情報をループするEnumrator
        /// </summary>
        /// <returns>関節情報を表す数字</returns>
        public IEnumerable<float> GetJointEnumerator()
        {
            for(int i = 0; i < num_person; i++)
            {
                foreach(float v in GetJointEnumerator(i))
                {
                    yield return v;
                }
            }
        }

        /// <summary>
        /// 指定した人物の中心位置をループするEnumrator
        /// </summary>
        /// <param name="person">人物のインデックス</param>
        /// <returns>中心位置を表す数字</returns>
        public IEnumerable<float> GetRootPosEnumerator(int person)
        {
            for (int j = 0; j < 3; j++)
            {
                yield return root_pos[person][j];
            }
        }

        /// <summary>
        /// 全員の中心位置をループするEnumrator
        /// </summary>
        /// <returns>中心位置を表す数字</returns>
        public IEnumerable<float> GetRootPosEnumerator()
        {
            for (int i = 0; i < num_person; i++)
            {
                foreach (float v in GetRootPosEnumerator(i))
                {
                    yield return v;
                }
            }
        }

        /// <summary>
        /// 指定した人物の向きをループするEnumrator
        /// </summary>
        /// <param name="person">人物のインデックス</param>
        /// <returns>向きを表す数字</returns>
        public IEnumerable<float> GetRootRotEnumerator(int person)
        {
            yield return root_rot[person];
        }

        /// <summary>
        /// 全員の向きをループするEnumrator
        /// </summary>
        /// <returns>向きを表す数字</returns>
        public IEnumerable<float> GetRootRotEnumerator()
        {
            for (int i = 0; i < num_person; i++)
            {
                foreach (float v in GetRootRotEnumerator(i))
                {
                    yield return v;
                }
            }
        }

        /// <summary>
        /// 全員の関節情報をループするEnumrator
        /// </summary>
        /// <returns>関節情報を表す数字</returns>
        public IEnumerable<float> GetEnumrator()
        {
            return GetJointEnumerator().Concat(GetRootPosEnumerator()).Concat(GetRootRotEnumerator());
        }

        /// <summary>
        /// バイト列からインスタンスを作成
        /// </summary>
        /// <param name="bytes">関節情報を表すバイト列</param>
        /// <returns>生成されたインスタンス</returns>
        private static JointInfo FromBytes1(byte[] bytes)
        {
            const int UNIT_BYTE = (NUM_JOINT * 3 + 3 + 1) * 4;

            if (!showed)
            {
                //MessageBox.Show(BitConverter.ToString(bytes));
            }

            int num_person = bytes[1];
            float[][][] joints = new float[num_person][][];
            float[][] root_pos = new float[num_person][];
            float[] root_rot = new float[num_person];
            for (int i = 0; i < num_person; i++)
            {
                joints[i] = new float[NUM_JOINT][];
                root_pos[i] = new float[3];
                for (int j = 0; j < NUM_JOINT; j++)
                {
                    joints[i][j] = new float[3];
                }
            }

            for (int i = 0; i < num_person; i++)
            {
                byte[] jinfo = new byte[UNIT_BYTE];
                Buffer.BlockCopy(bytes, 2 + i * UNIT_BYTE, jinfo, 0, UNIT_BYTE);
                int p = 0;
                for (int j = 0; j < NUM_JOINT; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        joints[i][j][k] = BitConverter.ToSingle(jinfo, p);
                        p += 4;
                    }
                }
                for (int j = 0; j < 3; j++)
                {
                    root_pos[i][j] = BitConverter.ToSingle(jinfo, p);
                    p += 4;
                }
                root_rot[i] = BitConverter.ToSingle(jinfo, p);
            }
            showed = true;
            JointInfo ret = new JointInfo(joints, root_pos, root_rot);
            return ret;
        }

        /// <summary>
        /// バイト列からインスタンスを作成
        /// </summary>
        /// <param name="bytes">関節情報を表すバイト列</param>
        /// <remarks>
        /// フォーマットバージョンが0x01の場合、<seealso cref="FromBytes1(byte[])"/>で処理される。
        /// </remarks>
        /// <returns>生成されたインスタンス</returns>
        public static JointInfo FromBytes(byte[] bytes)
        {
            int format_version = bytes[0];
            JointInfo ret = Identity;
            switch (format_version)
            {
                case 1:
                    ret = FromBytes1(bytes);
                    break;
            }
            return ret;
        }

        public override string ToString()
        {
            string[] tmp = new string[num_person];
            for (int i = 0; i < num_person; i++)
            {
                string[] tmp2 = new string[NUM_JOINT];
                for (int j = 0; j < NUM_JOINT; j++)
                {
                    tmp2[j] = string.Join(" ", joints[i][j]);
                }
                tmp[i] = string.Join(",", tmp2);
            }
            string joints_str = string.Join(";", tmp);

            tmp = new string[num_person];
            for (int i = 0; i < num_person; i++)
            {
                tmp[i] = string.Join(" ", root_pos[i]);
            }
            string pos_str = string.Join(";", tmp);

            string rot_str = string.Join(";", root_rot);

            return string.Format("{0}/{1}/{2}", joints_str, pos_str, rot_str);
        }

        /// <summary>
        /// joint, root_rot, root_posのベクトルを足し合わせます。O(NJ)
        /// </summary>
        /// <param name="joint1"></param>
        /// <param name="joint2"></param>
        /// <returns></returns>
        public static JointInfo operator+ (JointInfo joint1, JointInfo joint2)
        {
            JointInfo retJoint = Identity;
            int numPerson = Math.Min(joint1.num_person, joint2.num_person);

            for (int i = 0; i < numPerson; i++)
            {
                for (int j = 0; j < NUM_JOINT; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        retJoint.joints[i][j][k] = joint1.joints[i][j][k] + joint2.joints[i][j][k];
                    }
                }
                for (int j = 0; j < 3; j++)
                {
                    retJoint.root_pos[i][j] = joint1.root_pos[i][j] + joint2.root_pos[i][j];
                }
            }

            for (int i = 0; i < numPerson; i++)
            {
                retJoint.root_rot[i] = joint1.root_rot[i] + joint2.root_rot[i];
            }

            return retJoint;
        }

        public static JointInfo operator- (JointInfo joint1, JointInfo joint2)
        {
            return joint1 + -joint2;
        }

        /// <summary>
        /// joint, root_rot, root_posのベクトルにスカラー値を掛け合わせます。O(NJ)
        /// </summary>
        /// <param name="scalar"></param>
        /// <param name="joint"></param>
        /// <returns></returns>
        public static JointInfo operator* (float scalar, JointInfo joint)
        {
            JointInfo retJoint = Identity;
            int numPerson = joint.num_person;

            for (int i = 0; i < numPerson; i++)
            {
                for (int j = 0; j < NUM_JOINT; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        retJoint.joints[i][j][k] = joint.joints[i][j][k] * scalar;
                    }
                }
                for (int j = 0; j < 3; j++)
                {
                    retJoint.root_pos[i][j] = joint.root_pos[i][j] * scalar;
                }
            }

            for (int i = 0; i < numPerson; i++)
            {
                retJoint.root_rot[i] = joint.root_rot[i] * scalar;
            }

            return retJoint;
        }

        public static JointInfo operator* (JointInfo joint, float scalar)
        {
            return scalar * joint;
        }

        public static JointInfo operator- (JointInfo joint)
        {
            return -1 * joint;
        }

        public static JointInfo operator/ (JointInfo joint, float scalar)
        {
            return 1 / scalar * joint;
        }

        public override bool Equals(object o)
        {
            JointInfo jointInfo = (JointInfo)o;
            //bool ret = joints.SequenceEqual(jointInfo.joints) && 
            //    root_pos.SequenceEqual(jointInfo.root_pos) &&
            //    root_rot.SequenceEqual(jointInfo.root_rot);
            bool ret = ToString() == jointInfo.ToString();
            return ret;
        }

        public static bool operator == (JointInfo joint1, JointInfo joint2)
        {
            return joint1.Equals(joint2);
        }

        public static bool operator != (JointInfo joint1, JointInfo joint2)
        {
            return !(joint1 == joint2);
        }

        public float AngleMax(int person)
        {
            float angleMax = 0;
            IEnumerable<float> it = GetJointEnumerator(person).Concat(GetRootRotEnumerator(person));

            foreach(float val in it)
            {
                angleMax = Math.Max(val, Math.Abs(angleMax));
            }

            return angleMax;
        }

        /// <summary>
        /// 特定の人物の中心位置の最大値を取得
        /// </summary>
        /// <param name="person">人物のインデックス</param>
        /// <returns>中心位置の最大値</returns>
        public float DisplacementMax(int person)
        {
            float displacementMax = 0;

            foreach(float val in GetRootPosEnumerator(person))
            {
                displacementMax = Math.Max(displacementMax, Math.Abs(val));
            }

            return displacementMax;
        }
    }
}

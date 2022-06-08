using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynapseTrack_Resident
{
    class JointInfo
    {
        const int FORMAT_VERSION = 0x01;
        const int NUM_JOINT = 21;

        public float[][][] joints;
        public float[][] root_pos;
        public float[] root_rot;
        public int num_person;

        public JointInfo()
        {
            this.joints = new float[0][][];
            this.root_pos = new float[0][];
            this.root_rot = new float[0];
            this.num_person = 0;
        }

        public JointInfo(float[][][] joints, float[][] root_pos, float[] root_rot)
        {
            this.joints = joints;
            this.root_pos = root_pos;
            this.root_rot = root_rot;
            this.num_person = joints.Length;
        }

        // public byte[] ToBytes(){

        // }

        private static JointInfo FromBytes1(byte[] bytes)
        {
            const int NUM_JOINT = 21;
            const int UNIT_BYTE = (NUM_JOINT * 3 + 3 + 1) * 4;

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
            JointInfo ret = new JointInfo(joints: joints, root_pos: root_pos, root_rot: root_rot);
            return ret;
        }

        public static JointInfo FromBytes(byte[] bytes)
        {
            int format_version = bytes[0];
            JointInfo ret = new JointInfo();
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
    }
}

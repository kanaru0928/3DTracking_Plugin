using DxMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SynapseTrack_Resident
{
    public class Velocities
    {
        public DateTime addedDate;
        public JointInfo InitialVelocity { get; } = JointInfo.Identity;
        public JointInfo Acceleration { get; } = JointInfo.Identity;

        public Velocities(JointInfo velocities, JointInfo acceleration)
        {
            InitialVelocity = velocities;
            addedDate = DateTime.Now;
            Acceleration = acceleration;
        }

        public Velocities(Velocities velocities)
        {
            InitialVelocity = new JointInfo(velocities.InitialVelocity);
            Acceleration = new JointInfo(velocities.Acceleration);
            addedDate = velocities.addedDate;
        }

        public static Velocities Identity => new Velocities(GetIdentity());

        public bool IsIdentity()
        {
            return InitialVelocity == JointInfo.Identity && Acceleration == JointInfo.Identity;
        }

        private static Velocities GetIdentity()
        {
            return new Velocities(JointInfo.Identity, JointInfo.Identity);
        }

        public JointInfo GetVelocityAt(DateTime date)
        {
            return GetVelocityAt(date - addedDate);
        }

        public JointInfo GetVelocityAt(TimeSpan span)
        {
            JointInfo ret = InitialVelocity + (float)span.TotalSeconds * Acceleration;
            return ret;
        }
        public JointInfo GetDisplacementAt(DateTime date)
        {
            return GetDisplacementAt(date - addedDate);
        }

        public JointInfo GetDisplacementAt(TimeSpan span)
        {
            float duration = (float)span.TotalSeconds;
            JointInfo ret = duration * InitialVelocity + duration * duration / 2 * Acceleration;
            return ret;
        }
    }

    /// <summary>
    /// 後処理部のC#実装(未使用)
    /// </summary>
    class PreProcessing
    {
        const float DISPLACEMENT_SKIP_THRESHOLD = 100;

        LimitedQueue<JointInfo> localJoints = new LimitedQueue<JointInfo>(Config.AverageCount);
        LimitedQueue<JointInfo> worldJoints = new LimitedQueue<JointInfo>(Config.AverageCount);

        Velocities lastVelocities = Velocities.Identity;
        JointInfo lastAngles = JointInfo.Identity;

        List<int>[] tree;

        public PreProcessing()
        {
            tree = new List<int>[Config.ROOT_INDEX.Length];
            for(int i = 0; i < tree.Length; i++)
            {
                tree[i] = new List<int>();
            }
            for(int i = 0; i < JointInfo.NUM_JOINT; i++)
            {
                if (Config.ROOT_INDEX[i] == -1) continue;
                tree[Config.ROOT_INDEX[i]].Add(i);
            }
        }

        static float GetHipAngle(Vector3 vec)
        {
            return (float)Math.Atan2(vec.Z, vec.X);
        }

        public static JointInfo CalcRootRot(JointInfo joint)
        {
            for(int i = 0; i < JointInfo.DEFAULT_NUM_PERSON; i++)
            {
                joint.root_rot[i] = GetHipAngle(Utils.ArrayToVector(joint.joints[i][11]));
            }
            return joint;
        }

        public static JointInfo RotateJoints(JointInfo joints)
        {
            for(int i = 0; i < JointInfo.DEFAULT_NUM_PERSON; i++)
            {
                for(int j = 0; j < JointInfo.NUM_JOINT; j++)
                {
                    Quaternion q = Quaternion.RotationAxis(Vector3.UnitY, joints.root_rot[i]);
                    Vector4 angle = Vector4.Transform(new Vector4(Utils.ArrayToVector(joints.joints[i][j]), 0), q);
                    joints.joints[i][j] = Utils.VectorToArray(Utils.DownDimention(angle));
                }
            }
            return joints;
        }

        public JointInfo GetJoints(Class1 sender)
        {
            JointInfo ret = lastVelocities.GetDisplacementAt(DateTime.Now) + lastAngles;

            //sender.userControl.PrintLine("-----------");
            //sender.userControl.PrintLine("pos-z:" + ret.root_pos[0][2]);
            //sender.userControl.PrintLine("v:" + Utils.ArrayToVector(LastVelocities.InitialVelocity.root_pos[3]));
            //sender.userControl.PrintLine("a:" + Utils.ArrayToVector(LastVelocities.InitialVelocity.root_pos[3]));
            //sender.userControl.PrintLine("duration:" + (DateTime.Now - LastVelocities.addedDate));

            //return localJoints.Back();
            return lastAngles;
        }

        private JointInfo SwapJoint(JointInfo jointInfo)
        {
            if (worldJoints.Empty())
            {
                return jointInfo;
            }
            float[][] rootPos = jointInfo.root_pos;
            //JointInfo oldJointInfo = lastVelocities.GetDisplacementAt(DateTime.Now);
            JointInfo oldJointInfo = worldJoints.Back();
            float[][] oldPos = oldJointInfo.root_pos;

            int numPerson = jointInfo.num_person;
            int oldPerson = oldJointInfo.num_person;
            int numVertex = numPerson + oldPerson + 2;

            List<List<Edge>> graph = new List<Edge>[numVertex].ToList();
            for(int i = 0; i < numVertex; i++)
            {
                graph[i] = new List<Edge>();
            }
            for(int i = 1; i < oldPerson + 1; i++)
            {
                int from = 0;
                int to = i;
                graph[from].Add(new Edge(to, 1, 1, graph[to].Count()));
                graph[to].Add(new Edge(from, 0, -1, graph[from].Count() - 1));
            }
            for(int i = 0; i < oldPerson; i++)
            {
                for(int j = 0; j < numPerson; j++)
                {
                    int from = i + 1;
                    int to = j + oldPerson + 1;
                    Vector3 posVec = Utils.ArrayToVector(rootPos[j]);
                    Vector3 oldVec = Utils.ArrayToVector(oldPos[i]);
                    int add = (int)Vector3.Distance(posVec, oldVec);
                    int cost = add;
                    //if (rootPos[j] == new JointInfo(JointInfo.Identity).root_pos[0] || oldPos[i] == new JointInfo(JointInfo.Identity).root_pos[0])
                    //{
                    //    cost += 100000000;
                    //}
                    if (cost < 0) throw new ArgumentOutOfRangeException("costが負です。");
                    graph[from].Add(new Edge(to, 1, cost, graph[to].Count()));
                    graph[to].Add(new Edge(from, 0, -cost, graph[from].Count() - 1));
                }
            }
            for(int i = oldPerson + 1; i < oldPerson + numPerson + 1; i++)
            {
                int from = i;
                int to = numVertex - 1;
                graph[from].Add(new Edge(to, 1, 1, graph[to].Count()));
                graph[to].Add(new Edge(from, 0, -1, graph[from].Count() - 1));
            }

            int flow = Math.Min(oldPerson, numPerson);
            Utils.MinCostFlow(ref graph, flow);
            
            int[] oldNewPair = Enumerable.Repeat(-1, oldPerson).ToArray();

            for (int i = 0; i < oldPerson; i++)
            {
                int gindex = i + 1;
                for (int j = 0; j < graph[gindex].Count(); j++)
                {
                    if(graph[gindex][j].cap == 0)
                    {
                        oldNewPair[i] = graph[gindex][j].to - oldPerson - 1;
                        break;
                    }
                }
            }

            float[][][] retJoint = new float[numPerson][][];
            float[][] retRootPos = new float[numPerson][];
            float[] retRootRot = new float[numPerson];

            for(int i = 0; i < numPerson; i++)
            {
                int toIndex = oldNewPair[i];
                retJoint[i] = jointInfo.joints[toIndex];
                retRootPos[i] = jointInfo.root_pos[toIndex];
                retRootRot[i] = jointInfo.root_rot[toIndex];
            }

            return new JointInfo(retJoint, retRootPos, retRootRot);
        }

        private JointInfo ConvertToRelative(JointInfo _joint)
        {
            int rootJoint = 14;
            int numJoint = JointInfo.NUM_JOINT;
            int numPerson = _joint.num_person;
            Vector3[][] jointsArray = new Vector3[numPerson][];

            for (int i = 0; i < numPerson; i++)
            {
                jointsArray[i] = new Vector3[numJoint];
                //Quaternion rootRotQuaternion = Quaternion.RotationAxis(Vector3.UnitY, -_joint.root_rot[i]);
                for (int j = 0; j < numJoint; j++)
                {
                    Vector3 vector = Utils.ArrayToVector(_joint.joints[i][j]);
                    //vector = Utils.DownDimention(Vector3.Transform(vector, rootRotQuaternion));
                    jointsArray[i][j] = vector;
                }
            }

            float[][][] relativeArray = new float[numPerson][][];
            for(int i = 0; i < numPerson; i++)
            {
                relativeArray[i] = new float[numJoint][];
                for(int j = 0; j < numJoint; j++)
                {
                    relativeArray[i][j] = new float[3];
                }
            }

            Queue<int> que = new Queue<int>();
            que.Enqueue(rootJoint);
            while(que.Count != 0)
            {
                int index = que.Dequeue();
                if(index != rootJoint)
                {
                    for(int i = 0; i < numPerson; i++)
                    {
                        Quaternion rootRotQuaternion = Quaternion.RotationAxis(Vector3.UnitY, -_joint.root_rot[i]);
                        Vector3 relPos;
                        Vector3 child = jointsArray[i][index];
                        Vector3 root = jointsArray[i][Config.ROOT_INDEX[index]];
                        if (child == Vector3.Zero || root == Vector3.Zero)
                        {
                            relPos = Vector3.Zero;
                        }
                        else
                        {
                            relPos = child - root;
                            relPos = Utils.DownDimention(Vector3.Transform(relPos, rootRotQuaternion));
                            if (index == 3 && i == 0)
                            {
                                _ = 0;
                            }
                        }
                        relativeArray[i][index] = Utils.VectorToArray(relPos);
                        //MessageBox.Show(relPos.ToString() + "\n" + relativeArray[i][index]);
                    }
                }
                for(int i = 0; i < tree[index].Count; i++)
                {
                    que.Enqueue(tree[index][i]);
                }
            }


            //StringBuilder sb = new StringBuilder();
            //for(int i = 0; i < numPerson; i++)
            //{
            //    for(int j = 0; j < numJoint; j++)
            //    {
            //        sb.Append(string.Join(",", jointsArray[i][j]) + ";");
            //    }
            //    sb.Append("/");
            //}
            //MessageBox.Show(sb.ToString());

            JointInfo joint = new JointInfo(relativeArray, _joint.root_pos, _joint.root_rot);

            return joint;
        }

        public JointInfo ConvertToAngle(JointInfo joint)
        {
            JointInfo ret = joint;

            for(int i = 0; i < joint.num_person; i++)
            {
                for(int j = 0; j < JointInfo.NUM_JOINT; j++)
                {
                    float x = joint.joints[i][j][0];
                    float y = joint.joints[i][j][1];
                    float z = joint.joints[i][j][2];
                    float length = (float)Math.Sqrt(x * x + y * y + z * z);
                    if(length == 0)
                    {
                        ret.joints[i][j][0] = 0;
                        ret.joints[i][j][1] = float.NaN;
                        ret.joints[i][j][2] = float.NaN;
                        continue;
                    }
                    x /= length; y /= length; z /= length;
                    float theta = (float)Math.Asin(z);
                    float phi = (float)Math.Atan2(y, x);
                    if(j == 3 && i == 0)
                    {
                        _ = 0;
                    }
                    ret.joints[i][j][0] = 0;
                    ret.joints[i][j][1] = theta;
                    ret.joints[i][j][2] = phi;
                }
            }

            return ret;
        }

        public void AddVelocityQueue(JointInfo joint)
        {

            if (joint != JointInfo.Identity)
            {
                TimeSpan duration = DateTime.Now - lastVelocities.addedDate;

                JointInfo prevJoint = lastAngles;
                Velocities prevVelocities = lastVelocities;

                JointInfo initialVelocity = prevVelocities.GetVelocityAt(duration);
                JointInfo delta = joint - prevJoint;
                float fDuration = (float)duration.TotalSeconds;
                JointInfo acceleration;
                if (fDuration * fDuration == 0.0f)
                {
                    acceleration = JointInfo.Identity;
                }
                else
                {
                    acceleration = 2 * (delta - fDuration * initialVelocity) / fDuration * fDuration;
                }

                if (/*LastVelocities.IsIdentity()*/true)
                {
                    lastAngles = joint;
                    //Test.SetAngles(joint);
                }
                else
                {
                    lastAngles = lastVelocities.GetDisplacementAt(duration);
                    //Test.SetAngles(LastVelocities.GetDisplacementAt(duration));
                }

                for (int i = 0; i < acceleration.num_person; i++)
                {
                    //float angleMax = delta.AngleMax(i);
                    float displacementMax = delta.DisplacementMax(i);

                    if(displacementMax > DISPLACEMENT_SKIP_THRESHOLD)
                    {
                        acceleration = JointInfo.Identity;
                        initialVelocity = JointInfo.Identity;
                        lastAngles = joint;
                        //Test.SetAngles(joint);
                    }
                }

                //Test.SetVelocities(new Velocities(initialVelocity, acceleration));
                lastVelocities = new Velocities(initialVelocity, acceleration);
            }
        }

        public void SetJoints(JointInfo jointInfo)
        {
            jointInfo = SwapJoint(jointInfo);
            worldJoints.Push(jointInfo);

            JointInfo relativeJoint = ConvertToRelative(jointInfo);
            localJoints.Push(relativeJoint);

            AddVelocityQueue(relativeJoint);
        }
    }
}

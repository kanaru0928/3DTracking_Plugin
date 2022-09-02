using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxMath;

namespace SynapseTrack_Resident
{
    class Config
    {
        public static readonly int AverageCount = 3;

        public readonly static int[] ROOT_INDEX = new int[] { 16, 15, 1, 2, 3, 1, 5, 6, 14, 8, 9, 14, 11, 12, -1, 14, 1, 4, 7, 10, 13 };

        public static readonly Dictionary<string, string> ROOT = new Dictionary<string, string>
        {
            {"上半身2", "上半身"},
            {"首", "上半身2"},
            {"頭", "首"},

            {"左ひざD", "左足D"},
            {"左足首D", "左ひざD"},
            {"右ひざD", "右足D"},
            {"右足首D", "右ひざD"},

            {"左肩", "上半身2"},
            {"左腕", "左肩"},
            {"左ひじ", "左腕"},
            {"左手首", "左ひじ"},
            {"右肩", "上半身2"},
            {"右腕", "右肩"},
            {"右ひじ", "右腕"},
            {"右手首", "右ひじ"},
        };

        public static readonly OrderedDictionary JOINT_PAIR = new OrderedDictionary
        {
            {"上半身", 15 },
            {"上半身2", 1 },
            {"首", 16 },
            {"頭", 0 },

            {"左足D", 12 },
            {"左ひざD", 13 },
            {"左足首D", 20 },
            {"右足D", 9 },
            {"右ひざD", 10 },
            {"右足首D", 19 },

            //{"左足D", 12 },
            //{"左ひざD", 13 },
            //{"左足首D", 20 },
            //{"右足D", 9 },
            //{"右ひざD", 10 },
            //{"右足首D", 19 },

            {"左肩", 5 },
            {"左肩P", 5 },
            {"左腕", 6 },
            {"左ひじ", 7 },
            {"左手首", 18 },
            {"右肩", 2 },
            {"右肩P", 2 },
            {"右腕", 3 },
            {"右ひじ", 4 },
            {"右手首", 17 },

            //{"右肩", 5 },
            //{"右肩P", 5 },
            //{"右腕", 6 },
            //{"右ひじ", 7 },
            //{"右手首", 18 },
            //{"左肩", 2 },
            //{"左肩P", 2 },
            //{"左腕", 3 },
            //{"左ひじ", 4 },
            //{"左手首", 17 },
        };

        public static Dictionary<string, Quaternion> root_abs_angle = new Dictionary<string, Quaternion>
        {
            {"右足D", Quaternion.RotationAxis(Vector3.UnitZ, (float)Math.PI / 2) /*new Vector3(0, -1, 0)*/},
            {"左足D", Quaternion.RotationAxis(Vector3.UnitZ, (float)Math.PI / 2) /*new Vector3(0, -1, 0)*/ },
            //{"左肩", Quaternion.Identity /*new Vector3(1, 0, 0)*/},
            //{"右肩", Quaternion.RotationAxis(Vector3.UnitZ, (float)Math.PI) /*new Vector3(-1, 0, 0)*/},
            //{"左肩P", Quaternion.Identity /*new Vector3(1, 0, 0)*/},
            //{"右肩P", Quaternion.RotationAxis(Vector3.UnitZ, (float)Math.PI) /*new Vector3(-1, 0, 0)*/},
            //{"左肩P", new Vector3(0, (float)Math.PI, 0)},
            //{"右肩P", new Vector3(0, (float)Math.PI, -(float)Math.PI)},
            {"上半身", Quaternion.RotationAxis(Vector3.UnitZ, -(float)Math.PI / 2) /*new Vector3(0, 1, 0)*/},
        };

        public static Dictionary<string, Vector3> defaultVector = new Dictionary<string, Vector3>
        {
            {"上半身", new Vector3(0, 0.9966629f, -0.08162775f)},
            {"上半身2", new Vector3(0, 1, 0)},
            {"首", new Vector3(0, 1, 0)},
            {"頭", new Vector3(0, 1, 0)},

            {"左足D", new Vector3(0, -1, 0)},
            {"左ひざD", new Vector3(0, -1, 0)},
            {"左足首D", new Vector3(0, -0.47781744f, -0.87845916f)},
            {"右足D", new Vector3(0, -1, 0)},
            {"右ひざD", new Vector3(0, -1, 0)},
            {"右足首D", new Vector3(0, -0.47781744f, -0.87845916f)},

            {"左肩", new Vector3(1, 0, 0)},
            {"左肩P", new Vector3(1, 0, 0)},
            {"左腕", new Vector3(1, 0, 0)},
            {"左ひじ", new Vector3(1, 0, 0)},
            {"左手首", new Vector3(1, 0, 0)},
            {"右肩", new Vector3(-1, 0, 0)},
            {"右肩P", new Vector3(-1, 0, 0)},
            {"右腕", new Vector3(-1, 0, 0)},
            {"右ひじ", new Vector3(-1, 0, 0)},
            {"右手首", new Vector3(-1, 0, 0)},
        };

        public static HashSet<string> negative_joints = new HashSet<string>
        {
            "左ひざD", "左足首D", "右ひざD", "右足首D"
        };

        public static HashSet<string> negative_joints2 = new HashSet<string>
        {
            "右腕", "右ひじ", "右手首"
        };
    }
}

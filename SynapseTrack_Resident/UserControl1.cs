using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MikuMikuPlugin;
using System.Collections;
using DxMath;

namespace SynapseTrack_Resident
{
    public partial class UserControl1 : UserControl
    {
        public event EventHandler UpdateHundler;
        public List<Bind> bindingModels;

        private bool isPrintAccept = true;

        public UserControl1()
        {
            InitializeComponent();
            bindingModels = new List<Bind>();
            bindingModels.Add(new Bind(5, "初音ミク(仮)", 1));
            if(UpdateHundler != null)
            {
                UpdateHundler(this, new EventArgs());
            }
            bindingSource1.DataSource = bindingModels;
        }

        private int offsetZ_;

        public int offsetZ
        {
            get { return offsetZ_; }
            set
            {
                offsetZ_ = value;
                offsetZNumeric.Value = value;
                offsetZTrack.Value = value;
            }
        }

        public void UpdateModel(Scene scene)
        {
            List<Bind> binds = bindingModels;
            Dictionary<string, int> motionIds = new Dictionary<string, int>();
            for(int i = 0; i < binds.Count; i++)
            {
                motionIds[binds[i].ModelName] = binds[i].MotionId;
            }
            bindingModels.Clear();
            ModelCollection models = scene.Models;
            for (int i = 0; i < models.Count; i++)
            {
                int motionid = -1;
                if (motionIds.ContainsKey(models[i].Name))
                {
                    motionid = motionIds[models[i].Name];
                }
                bindingModels.Add(new Bind(i, models[i].Name, motionid));
            }
            bindingSource1.ResetBindings(true);
        }

        private void offsetZTrack_Scroll(object sender, EventArgs e)
        {
            offsetZ = offsetZTrack.Value;
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            UpdateHundler(this, new EventArgs());
        }

        public class Bind
        {
            public Bind(int id, string name, int motion) {
                ModelId = id;
                ModelName = name;
                MotionId = motion;
            }
            public int ModelId { get; set; }
            public string ModelName { get; set; }
            public int MotionId { get; set; }
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(dataGridView1.Columns[e.ColumnIndex].Name == "Increment")
            {
                bindingModels[e.RowIndex].MotionId++;
                bindingSource1.ResetBindings(false);
            }
            else if(dataGridView1.Columns[e.ColumnIndex].Name == "Decrement")
            {
                bindingModels[e.RowIndex].MotionId--;
                bindingSource1.ResetBindings(false);
            }
        }

        public void setFps(double fps)
        {
            fpsText.Text = $"FPS:{fps}";
        }

        public void PrintLine(object obj)
        {
            Print(obj.ToString() + Environment.NewLine);
        }

        public void Print(object obj)
        {
            if (isPrintAccept)
            {
                logBox.AppendText(obj.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            isPrintAccept = !isPrintAccept;
        }
    }
}


namespace SynapseTrack_Resident
{
    partial class UserControl1
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.logBox = new System.Windows.Forms.TextBox();
            this.fpsText = new System.Windows.Forms.Label();
            this.updateButton = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.modelIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.modelName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.motionIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Decrement = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Increment = new System.Windows.Forms.DataGridViewButtonColumn();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.offsetZNumeric = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.offsetZTrack = new System.Windows.Forms.TrackBar();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.offsetZNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.offsetZTrack)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.logBox);
            this.groupBox1.Controls.Add(this.fpsText);
            this.groupBox1.Controls.Add(this.updateButton);
            this.groupBox1.Controls.Add(this.dataGridView1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.offsetZNumeric);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.offsetZTrack);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(273, 387);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SynapseTrack";
            // 
            // logBox
            // 
            this.logBox.Font = new System.Drawing.Font("PlemolJP35 Console", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.logBox.Location = new System.Drawing.Point(8, 200);
            this.logBox.MaxLength = 5000;
            this.logBox.Multiline = true;
            this.logBox.Name = "logBox";
            this.logBox.ReadOnly = true;
            this.logBox.Size = new System.Drawing.Size(259, 117);
            this.logBox.TabIndex = 1;
            // 
            // fpsText
            // 
            this.fpsText.AutoSize = true;
            this.fpsText.Location = new System.Drawing.Point(6, 185);
            this.fpsText.Name = "fpsText";
            this.fpsText.Size = new System.Drawing.Size(28, 12);
            this.fpsText.TabIndex = 6;
            this.fpsText.Text = "FPS:";
            // 
            // updateButton
            // 
            this.updateButton.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.updateButton.Location = new System.Drawing.Point(41, 55);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(49, 17);
            this.updateButton.TabIndex = 5;
            this.updateButton.Text = "更新";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.modelIndex,
            this.modelName,
            this.motionIndex,
            this.Decrement,
            this.Increment});
            this.dataGridView1.DataSource = this.bindingSource1;
            this.dataGridView1.Location = new System.Drawing.Point(6, 78);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.Size = new System.Drawing.Size(261, 104);
            this.dataGridView1.TabIndex = 4;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // modelIndex
            // 
            this.modelIndex.DataPropertyName = "ModelId";
            this.modelIndex.HeaderText = "Id";
            this.modelIndex.Name = "modelIndex";
            this.modelIndex.ReadOnly = true;
            this.modelIndex.Width = 30;
            // 
            // modelName
            // 
            this.modelName.DataPropertyName = "ModelName";
            this.modelName.HeaderText = "モデル名";
            this.modelName.Name = "modelName";
            this.modelName.ReadOnly = true;
            this.modelName.Width = 130;
            // 
            // motionIndex
            // 
            this.motionIndex.DataPropertyName = "MotionId";
            this.motionIndex.HeaderText = "モーション";
            this.motionIndex.Name = "motionIndex";
            this.motionIndex.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.motionIndex.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.motionIndex.Width = 58;
            // 
            // Decrement
            // 
            this.Decrement.HeaderText = "-";
            this.Decrement.Name = "Decrement";
            this.Decrement.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Decrement.Text = "-";
            this.Decrement.Width = 20;
            // 
            // Increment
            // 
            this.Increment.HeaderText = "+";
            this.Increment.Name = "Increment";
            this.Increment.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Increment.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Increment.Text = "+";
            this.Increment.Width = 20;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "対応";
            // 
            // offsetZNumeric
            // 
            this.offsetZNumeric.Location = new System.Drawing.Point(8, 33);
            this.offsetZNumeric.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.offsetZNumeric.Minimum = new decimal(new int[] {
            300,
            0,
            0,
            -2147483648});
            this.offsetZNumeric.Name = "offsetZNumeric";
            this.offsetZNumeric.Size = new System.Drawing.Size(47, 19);
            this.offsetZNumeric.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Z補正";
            // 
            // offsetZTrack
            // 
            this.offsetZTrack.LargeChange = 1;
            this.offsetZTrack.Location = new System.Drawing.Point(61, 18);
            this.offsetZTrack.Maximum = 300;
            this.offsetZTrack.Minimum = -300;
            this.offsetZTrack.Name = "offsetZTrack";
            this.offsetZTrack.Size = new System.Drawing.Size(206, 45);
            this.offsetZTrack.TabIndex = 0;
            this.offsetZTrack.TickFrequency = 30;
            this.offsetZTrack.Scroll += new System.EventHandler(this.offsetZTrack_Scroll);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button1.Location = new System.Drawing.Point(187, 183);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(80, 16);
            this.button1.TabIndex = 7;
            this.button1.Text = "Stop";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // UserControl1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "UserControl1";
            this.Size = new System.Drawing.Size(273, 387);
            this.Load += new System.EventHandler(this.UserControl1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.offsetZNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.offsetZTrack)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown offsetZNumeric;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar offsetZTrack;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.DataGridViewTextBoxColumn modelIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn modelName;
        private System.Windows.Forms.DataGridViewTextBoxColumn motionIndex;
        private System.Windows.Forms.DataGridViewButtonColumn Decrement;
        private System.Windows.Forms.DataGridViewButtonColumn Increment;
        private System.Windows.Forms.Label fpsText;
        private System.Windows.Forms.TextBox logBox;
        private System.Windows.Forms.Button button1;
    }
}

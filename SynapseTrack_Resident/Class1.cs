using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using MikuMikuPlugin;
using DxMath;

namespace SynapseTrack_Resident
{
    /// <summary>
    /// プラグインのエントリーポイントとなるクラス
    /// </summary>
    /// <remarks>
    /// Interfaceの実装は<see href="https://sites.google.com/site/mikumikumoving/%E3%83%9E%E3%83%8B%E3%83%A5%E3%82%A2%E3%83%AB/17-%E3%83%97%E3%83%A9%E3%82%B0%E3%82%A4%E3%83%B3">MMM公式ドキュメント</see>を参照
    /// </remarks>
    public class Class1 : IResidentPlugin, IHaveUserControl
    {
        private const int IMAGE_SIZE = 32;
        private const int SMALL_SIZE = 20;

        public UserControl1 userControl;
        private Image _image;
        private Image _small_image;
        ProcessingMaster processing;

        public Class1(){
        }

        public void Initialize()
        {
            CreateIcon();
        }

        public void CreateIcon()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            Bitmap bmp = new Bitmap(assembly.GetManifestResourceStream("SynapseTrack_Resident.Icon.png"));
            Bitmap resize_bmp = new Bitmap(IMAGE_SIZE, IMAGE_SIZE);
            Graphics g = Graphics.FromImage(resize_bmp);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(bmp, 0, 0, IMAGE_SIZE, IMAGE_SIZE);
            g.Dispose();

            Bitmap small_bmp = new Bitmap(SMALL_SIZE, SMALL_SIZE);
            g = Graphics.FromImage(small_bmp);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(bmp, 0, 0, SMALL_SIZE, SMALL_SIZE);
            g.Dispose();
            _image = resize_bmp;
            _small_image = small_bmp;
        }

        public Guid GUID
        {
            get { return new Guid("18216F19-171C-43A4-8AA0-540F6BC4E787"); }
        }

        public IWin32Window ApplicationForm { get; set; }

        public Scene Scene { get; set; }

        public string Text { get { return "Track"; } }

        public string EnglishText { get { return "Track"; } }

        public string Description { get { return "This tracks poses from an image."; } }

        public Image Image { get { return _image; } }

        public Image SmallImage { get { return _small_image; } }

        public UserControl CreateControl()
        {
            userControl = new UserControl1();
            userControl.UpdateHundler += UserControl_UpdateHundler;
            return userControl;
        }

        private void UserControl_UpdateHundler(object sender, EventArgs e)
        {
            userControl.UpdateModel(Scene);
        }

        public void Update(float frame, float diff)
        {
            processing.SetJoints(this);
        }

        public void Enabled()
        {
            //foreach (var models in Scene.Models)
            //{
            //    Bone b = models.Bones["上半身"];
            //    MessageBox.Show($"上半身\n" +
            //        $"x: {b.LocalAxisX}\n" +
            //        $"y: {b.LocalAxisY}\n" +
            //        $"z: {b.LocalAxisZ}");
            //}

            processing = new ProcessingMaster();

            userControl.UpdateModel(Scene);
            processing.ConnectSocket();
        }

        public void Disabled()
        {
            processing.Dispose();
            processing.showed = false;
        }

        public void Dispose()
        {
            processing?.Dispose();
        }
    }
}

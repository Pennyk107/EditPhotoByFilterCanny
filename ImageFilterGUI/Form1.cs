using ImageFilterBack;

namespace ImageFilterGUI
{
    public partial class Form1 : Form
    {
        private ImageFilter filter = new();

        public Form1()
        {
            InitializeComponent();
            Text = filter.HandlerName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Title = "�������� ����������� ��� ��������",
                Filter = "����� �����������|*.jpg;*.png",
                CheckFileExists = true
            };
            if (dialog.ShowDialog() != DialogResult.OK)
                return;
            Image pic;
            try
            {
                pic = Image.FromFile(dialog.FileName);
            }
            catch (Exception)
            {
                MessageBox.Show(
                    caption: "������ ������ ��������",
                    text: "��� ������ �������� ��������� ������! ��������, ���� ��������.",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
                return;
            }
            filter.Source = (Bitmap)pic;
            pictureBox1.Image = pic;
            progressBar1.Maximum = pic.Width - 100;
            button1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            ProgressDelegate progress = (double _) => progressBar1.PerformStep();
            filter.startHandle(progress);
            pictureBox2.Image = filter.Result;
            button3.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new()
            {
                Title = "�������, ��� ��������� ��������",
                Filter = "����� �����������|*.jpg;*.png",
                CheckPathExists = true
            };
            if (dialog.ShowDialog() != DialogResult.OK)
                return;
            try
            {
                filter.Result.Save(dialog.FileName);
            }
            catch (Exception)
            {
                MessageBox.Show(
                    caption: "������ ���������� ��������",
                    text: "��� ���������� �������� ��������� ������! ���������� ������� ������ ����� ��� ������ ��� �����.",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
                return;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
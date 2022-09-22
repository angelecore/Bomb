using System.Windows.Forms;

namespace bomberman
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Initializemap(1);
        }

        private void Initializemap(int map)
        {
            if (map <= 0)
            {
                MessageBox.Show(String.Format("no map"));
                return;
            }

            string maplayout = string.Empty;
            switch (map)
            {
                case 1:
                    maplayout = Properties.Resources.Level1;
                    break;

            }

            using (System.IO.StringReader reader = new System.IO.StringReader(maplayout))
            {
                int blocksize = 40;
                int currentx = 0;
                int currenty = 0;
                string line = String.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] chararray = line.Split(' ');
                    foreach (string charele in chararray)
                    { Button block = new Button();
                        block.Size = new Size(blocksize, blocksize);
                        switch(charele)
                        {
                            case "i":
                                block.BackColor = Color.Black;
                                break;
                            case "c":
                                block.BackColor = Color.DarkGray;
                                break;
                            case "v":
                                block.BackColor = Color.LightGray;
                                break;
                        }
                        block.Location = new Point(currentx, currenty);
                        this.Controls.Add(block);
                        currentx += blocksize + 1;
                    }
                    currentx = 0;
                    currenty += blocksize;
                }
                reader.Close();
            }

        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace WindowsFormsApp5
{
    public partial class Form1 : Form
    {
        private Random random = new Random();
        private int score = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            player.Location = GetRandomLocation();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Движение игрока
            int moveX = random.Next(-5, 6);
            int moveY = random.Next(-5, 6);

            player.Left += moveX;
            player.Top += moveY;

            // Проверка границ
            if (player.Left < 0) player.Left = 0;
            if (player.Top < 0) player.Top = 0;
            if (player.Right > ClientSize.Width) player.Left = ClientSize.Width - player.Width;
            if (player.Bottom > ClientSize.Height) player.Top = ClientSize.Height - player.Height;

            // Проверка столкновения
            if (player.Bounds.Contains(PointToClient(Cursor.Position)))
            {
                player.Location = GetRandomLocation();
                score = 0;
                scoreLabel.Text = "Счет: 0";
                SystemSounds.Beep.Play(); // Звук при столкновении
            }
            else
            {
                score++;
                scoreLabel.Text = $"Счет: {score}";

                // Меняем цвет при высоком счете
                player.BackColor = score > 50 ? Color.Red : Color.Lime;
            }
        }

        private Point GetRandomLocation()
        {
            return new Point(
                random.Next(0, ClientSize.Width - player.Width),
                random.Next(0, ClientSize.Height - player.Height));
        }
    }
}


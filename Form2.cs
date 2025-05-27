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
        private int roundTime = 10; // 10 секунд на раунд по умолчанию
        private int roundTimer = 0;
        private int difficulty = 1; // 1-легко, 2-средне, 3-сложно
        private bool gameRunning = false;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Text = "Игра на выживание";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            player.Location = GetRandomLocation();
            menuStrip1.Renderer = new ToolStripProfessionalRenderer(new CustomProfessionalColors());
            UpdateDifficultySettings();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!gameRunning) return;

            // Движение игрока с клавиатурой
            int moveSpeed = 5 * difficulty;
            if (Control.ModifierKeys == Keys.Shift) moveSpeed *= 2; // Бег с Shift

            if (IsKeyPressed(Keys.W)) player.Top -= moveSpeed;
            if (IsKeyPressed(Keys.S)) player.Top += moveSpeed;
            if (IsKeyPressed(Keys.A)) player.Left -= moveSpeed;
            if (IsKeyPressed(Keys.D)) player.Left += moveSpeed;

            // Проверка границ
            if (player.Left < 0) player.Left = 0;
            if (player.Top < 0) player.Top = 0;
            if (player.Right > ClientSize.Width) player.Left = ClientSize.Width - player.Width;
            if (player.Bottom > ClientSize.Height) player.Top = ClientSize.Height - player.Height;

            // Проверка столкновения с курсором
            if (player.Bounds.Contains(PointToClient(Cursor.Position)))
            {
                player.Location = GetRandomLocation();
                score = 0;
                scoreLabel.Text = "Счет: 0";
                SystemSounds.Beep.Play();
            }
            else
            {
                score += difficulty; // Чем сложнее, тем больше очков
                scoreLabel.Text = $"Счет: {score}";

                // Меняем цвет при высоком счете
                player.BackColor = score > 50 ? Color.Red :
                                 score > 30 ? Color.Orange : Color.Lime;
            }

            // Таймер раунда
            roundTimer++;
            roundTimeLabel.Text = $"Время: {roundTime - roundTimer / 10}";

            if (roundTimer >= roundTime * 10)
            {
                EndRound();
            }
        }
        private void StartNewGame()
        {
            score = 0;
            roundTimer = 0;
            gameRunning = true;
            player.Location = GetRandomLocation();
            timer1.Start();
            scoreLabel.Text = "Счет: 0";
            roundTimeLabel.Text = $"Время: {roundTime}";
        }
        private void EndRound()
        {
            gameRunning = false;
            timer1.Stop();
            MessageBox.Show($"Раунд завершен!\nВаш счет: {score}", "Результат");
        }
       

        private bool IsKeyPressed(Keys key)
        {
            return (GetAsyncKeyState((int)key) & 0x8000) != 0;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);


        private void UpdateDifficultySettings()
        {
            switch (difficulty)
            {
                case 1:
                    timer1.Interval = 100;
                    player.Size = new Size(40, 40);
                    break;
                case 2:
                    timer1.Interval = 70;
                    player.Size = new Size(30, 30);
                    break;
                case 3:
                    timer1.Interval = 40;
                    player.Size = new Size(20, 20);
                    break;
            }
        }


        private Point GetRandomLocation()
        {
            return new Point(
                random.Next(0, ClientSize.Width - player.Width),
                random.Next(0, ClientSize.Height - player.Height));
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var settingsForm = new Form()
            {
                Text = "Настройки игры",
                Size = new Size(300, 200),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent
            };

            var difficultyLabel = new Label()
            {
                Text = "Сложность:",
                Location = new Point(20, 20),
                AutoSize = true
            };

            var difficultyCombo = new ComboBox()
            {
                Location = new Point(120, 20),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            difficultyCombo.Items.AddRange(new object[] { "Легкая", "Средняя", "Сложная" });
            difficultyCombo.SelectedIndex = difficulty - 1;

            var timeLabel = new Label()
            {
                Text = "Длительность раунда (сек):",
                Location = new Point(20, 60),
                AutoSize = true
            };

            var timeNumeric = new NumericUpDown()
            {
                Location = new Point(180, 60),
                Width = 90,
                Minimum = 5,
                Maximum = 60,
                Value = roundTime
            };

            var saveButton = new Button()
            {
                Text = "Сохранить",
                Location = new Point(100, 120),
                DialogResult = DialogResult.OK
            };

            saveButton.Click += (s, args) =>
            {
                difficulty = difficultyCombo.SelectedIndex + 1;
                roundTime = (int)timeNumeric.Value;
                UpdateDifficultySettings();
                settingsForm.Close();
            };

            settingsForm.Controls.AddRange(new Control[] { difficultyLabel, difficultyCombo, timeLabel, timeNumeric, saveButton });
            settingsForm.ShowDialog();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Игра на выживание\nВерсия 1.0\n\nИзбегайте курсора мыши как можно дольше!", "О программе");
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите выйти?", "Выход", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void новаяИграToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartNewGame();
        }

        private void паузаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gameRunning)
            {
                timer1.Stop();
                gameRunning = false;
                паузаToolStripMenuItem.Text = "Продолжить";
            }
            else
            {
                timer1.Start();
                gameRunning = true;
                паузаToolStripMenuItem.Text = "Пауза";
            }
        }
    }
}


    public class CustomProfessionalColors : ProfessionalColorTable
    {
        public override Color MenuItemSelected => Color.FromArgb(65, 105, 225);
        public override Color MenuItemBorder => Color.FromArgb(0, 0, 139);
        public override Color MenuItemSelectedGradientBegin => Color.FromArgb(100, 149, 237);
        public override Color MenuItemSelectedGradientEnd => Color.FromArgb(65, 105, 225);
    }




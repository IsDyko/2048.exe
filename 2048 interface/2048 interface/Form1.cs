using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace _2048WindowsForms
{
    public partial class Form1 : Form
    {
        // Déclaration des éléments graphiques
        Label[,] lbl = new Label[4, 4];
        int[,] tableau = new int[4, 4];
        Label scoreLabel = new Label();
        int score = 0;
        bool isGameOver = false;
        Label gameMessageLabel = new Label();
        Button quitButton = new Button();
        Button restartButton = new Button();
        Color[] color = { Color.Gray, Color.LightBlue, Color.LightCyan, Color.LightGreen, Color.Magenta, Color.Red, Color.Yellow, Color.DarkBlue, Color.DarkCyan, Color.DarkGreen, Color.DarkMagenta, Color.DarkRed, Color.White };

        public Form1()
        {
            InitializeComponent();
            InitializeJeu();
            this.KeyPreview = true;
        }

        // Initialisation du jeu
        private void InitializeJeu()
        {
            // Configuration du label affichant le score
            scoreLabel.Bounds = new Rectangle(20, 20 + 100 * 4, 180, 30); // Position en dessous de la grille de jeu
            scoreLabel.Font = new Font("Arial", 16);
            scoreLabel.Text = "Score: 0";
            Controls.Add(scoreLabel);

            gameMessageLabel.Bounds = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
            gameMessageLabel.BackColor = Color.FromArgb(128, 255, 255, 255); // Fond semi-transparent
            gameMessageLabel.TextAlign = ContentAlignment.MiddleCenter;
            gameMessageLabel.Font = new Font("Arial", 24, FontStyle.Bold);
            gameMessageLabel.Visible = false; // Caché par défaut
            Controls.Add(gameMessageLabel);

            // Configuration du bouton Quitter
            quitButton.Text = "Quitter";
            quitButton.Size = new Size(100, 30);
            quitButton.Location = new Point(ClientSize.Width - 220, ClientSize.Height - 60);
            quitButton.Click += QuitButtonClick;
            Controls.Add(quitButton);

            // Configuration du bouton Recommencer
            restartButton.Text = "Recommencer";
            restartButton.Size = new Size(100, 30);
            restartButton.Location = new Point(ClientSize.Width - 110, ClientSize.Height - 60);
            restartButton.Click += RestartButtonClick;
            Controls.Add(restartButton);

            // Cachez les boutons initialement
            quitButton.Visible = false;
            restartButton.Visible = false;

            // Initialisation de la grille de jeu (labels)
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    lbl[i, j] = new Label();
                    lbl[i, j].Bounds = new Rectangle(20 + 100 * j, 20 + 100 * i, 90, 90);
                    lbl[i, j].TextAlign = ContentAlignment.MiddleCenter;
                    lbl[i, j].Font = new Font("Arial", 20);
                    Controls.Add(lbl[i, j]);
                }
            }

            // Ajout des deux premiers chiffres dans la grille
            AddNewNumber();
            AddNewNumber();
            UpdateBoard();
        }

        private void QuitButtonClick(object sender, EventArgs e)
        {
            this.Close(); // Ferme l'application
        }

        private void RestartButtonClick(object sender, EventArgs e)
        {
            ResetGame();
        }

        private void ResetGame()
        {
            score = 0;
            isGameOver = false;
            tableau = new int[4, 4];

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    lbl[i, j].Text = string.Empty;
                    lbl[i, j].BackColor = color[0]; // Réinitialise la couleur des cases
                }
            }

            scoreLabel.Text = "Score: 0";
            gameMessageLabel.Visible = false;
            quitButton.Visible = false;
            restartButton.Visible = false;

            AddNewNumber();
            AddNewNumber();
            UpdateBoard();

            this.Focus();
        }


        // Ajoute un nouveau chiffre (2 ou 4) aléatoirement sur la grille
        private void AddNewNumber()
        {
            Random random = new Random();
            List<int> empty = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (tableau[i, j] == 0)
                    {
                        empty.Add(i * 4 + j);
                    }
                }
            }

            if (empty.Count > 0)
            {
                int index = empty[random.Next(empty.Count)];
                tableau[index / 4, index % 4] = random.NextDouble() < 0.9 ? 2 : 4;
            }
        }

        // Gestion de l'événement de pression d'une touche
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // Si le jeu est terminé, ne faites rien (ou ajoutez une logique pour redémarrer le jeu)
            if (isGameOver)
            {
                // Vous pouvez ajouter ici une condition pour redémarrer le jeu si nécessaire
                // Par exemple, en appuyant sur une touche spécifique
                return;
            }

            int[,] originalTableau = (int[,])tableau.Clone();
            bool moved = false;

            switch (e.KeyCode)
            {
                case Keys.Left:
                    moved = MoveLeft();
                    break;
                case Keys.Right:
                    moved = MoveRight();
                    break;
                case Keys.Up:
                    moved = MoveUp();
                    break;
                case Keys.Down:
                    moved = MoveDown();
                    break;
            }

            if (moved)
            {
                AddNewNumber();
                UpdateBoard();

                if (CheckWin())
                {
                    ShowGameMessage("You Win!");
                    isGameOver = true;
                }
                else if (!CanMakeMove())
                {
                    ShowGameMessage("Game Over");
                    isGameOver = true;
                }
            }
        }

        private void HideGameMessage()
        {
            gameMessageLabel.Visible = false;
            quitButton.Visible = false;
            restartButton.Visible = false;
            isGameOver = false;
            score = 0;
            tableau = new int[4, 4];
            InitializeJeu(); // Réinitialise le jeu
        }



        private void ShowGameMessage(string message)
        {
            gameMessageLabel.Text = message;
            gameMessageLabel.Visible = true;
            quitButton.Visible = true;
            restartButton.Visible = true;
            quitButton.BringToFront();
            restartButton.BringToFront();
        }



        // Mettre à jour l'affichage de la grille
        private void UpdateBoard()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    lbl[i, j].Text = tableau[i, j] == 0 ? string.Empty : tableau[i, j].ToString();
                    lbl[i, j].BackColor = GetColor(tableau[i, j]);
                }
            }
            UpdateScoreDisplay();
        }

        // Obtenir la couleur d'arrière-plan en fonction de la valeur du chiffre
        private Color GetColor(int number)
        {
            int index = (int)Math.Log(number, 2);
            return index >= 0 && index < color.Length ? color[index] : Color.Black;
        }

        // Vérifier si la grille a changé depuis le dernier mouvement
        private bool HasBoardChanged(int[,] original, int[,] current)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (original[i, j] != current[i, j])
                        return true;
                }
            }
            return false;
        }

        // Déplacement des chiffres vers la gauche
        private bool MoveLeft()
        {
            bool moved = false;
            for (int row = 0; row < 4; row++)
            {
                for (int col = 1; col < 4; col++)
                {
                    if (tableau[row, col] > 0)
                    {
                        int colIndex = col;
                        while (colIndex > 0 && tableau[row, colIndex - 1] == 0)
                        {
                            tableau[row, colIndex - 1] = tableau[row, colIndex];
                            tableau[row, colIndex] = 0;
                            colIndex--;
                            moved = true;
                        }

                        if (colIndex > 0 && tableau[row, colIndex - 1] == tableau[row, colIndex])
                        {
                            tableau[row, colIndex - 1] *= 2;
                            score += tableau[row, colIndex - 1];
                            tableau[row, colIndex] = 0;
                            moved = true;
                        }
                    }
                }
            }
            return moved;
        }


        // Déplacement des chiffres vers la droite
        private bool MoveRight()
        {
            bool moved = false;
            for (int row = 0; row < 4; row++)
            {
                for (int col = 2; col >= 0; col--)
                {
                    if (tableau[row, col] > 0)
                    {
                        int colIndex = col;
                        while (colIndex < 3 && tableau[row, colIndex + 1] == 0)
                        {
                            tableau[row, colIndex + 1] = tableau[row, colIndex];
                            tableau[row, colIndex] = 0;
                            colIndex++;
                            moved = true;
                        }

                        if (colIndex < 3 && tableau[row, colIndex + 1] == tableau[row, colIndex])
                        {
                            tableau[row, colIndex + 1] *= 2;
                            score += tableau[row, colIndex + 1];
                            tableau[row, colIndex] = 0;
                            moved = true;
                        }
                    }
                }
            }
            return moved;
        }


        // Déplacement des chiffres vers le haut
        private bool MoveUp()
        {
            bool moved = false;
            for (int col = 0; col < 4; col++)
            {
                for (int row = 1; row < 4; row++)
                {
                    if (tableau[row, col] > 0)
                    {
                        int rowIndex = row;
                        while (rowIndex > 0 && tableau[rowIndex - 1, col] == 0)
                        {
                            tableau[rowIndex - 1, col] = tableau[rowIndex, col];
                            tableau[rowIndex, col] = 0;
                            rowIndex--;
                            moved = true;
                        }

                        if (rowIndex > 0 && tableau[rowIndex - 1, col] == tableau[rowIndex, col])
                        {
                            tableau[rowIndex - 1, col] *= 2;
                            score += tableau[rowIndex - 1, col];
                            tableau[rowIndex, col] = 0;
                            moved = true;
                        }
                    }
                }
            }
            return moved;
        }


        // Déplacement des chiffres vers le bas
        private bool MoveDown()
        {
            bool moved = false;
            for (int col = 0; col < 4; col++)
            {
                for (int row = 2; row >= 0; row--)
                {
                    if (tableau[row, col] > 0)
                    {
                        int rowIndex = row;
                        while (rowIndex < 3 && tableau[rowIndex + 1, col] == 0)
                        {
                            tableau[rowIndex + 1, col] = tableau[rowIndex, col];
                            tableau[rowIndex, col] = 0;
                            rowIndex++;
                            moved = true;
                        }

                        if (rowIndex < 3 && tableau[rowIndex + 1, col] == tableau[rowIndex, col])
                        {
                            tableau[rowIndex + 1, col] *= 2;
                            score += tableau[rowIndex + 1, col];
                            tableau[rowIndex, col] = 0;
                            moved = true;
                        }
                    }
                }
            }
            return moved;
        }


        // Vérifier s'il est possible de faire un mouvement
        private bool CanMakeMove()
        {
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    if (tableau[row, col] == 0)
                        return true;

                    if (row < 3 && tableau[row, col] == tableau[row + 1, col])
                        return true;

                    if (col < 3 && tableau[row, col] == tableau[row, col + 1])
                        return true;
                }
            }

            return false;
        }
        private void ShowMlgGif()
        {
            PictureBox gifBox = new PictureBox();
            gifBox.Image = Image.FromFile(Path.Combine(Application.StartupPath, "6os.gif"));
            gifBox.SizeMode = PictureBoxSizeMode.StretchImage;
            gifBox.Dock = DockStyle.Fill; // Pour remplir tout le formulaire
            Controls.Add(gifBox);
            gifBox.BringToFront();

            // Pour arrêter et cacher le GIF après un certain temps, utilisez un Timer
            Timer timer = new Timer();
            timer.Interval = 5000; // Durée d'affichage du GIF en millisecondes
            timer.Tick += (sender, e) =>
            {
                gifBox.Hide();
                timer.Stop();
            };
            timer.Start();
        }



        // Vérifier si le joueur a gagné
        private bool CheckWin()
        {
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    if (tableau[row, col] == 2048)
                    {
                        ShowGameMessage("You Win!");
                        ShowMlgGif();
                        return true;
                    }
                }
            }
            return false;
        }

        // Mettre à jour l'affichage du score
        private void UpdateScoreDisplay()
        {
            scoreLabel.Text = "Score: " + score.ToString();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;
using System.Runtime.Remoting.Messaging;

namespace WpfApp1
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region variable
        OpenFileDialog dlg;
        BitmapImage bitmap;
        MyImage image;
        int compteurDeModification;
        bool flag = false;
        string name;
        bool flag2 = false;
        string texte;
        bool flag3 = false;
        string texte2;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.bmp)|*.bmp";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string filename = null;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filename = dlg.FileName;

            }
            this.bitmap = new BitmapImage();
            this.bitmap.BeginInit();
            this.bitmap.UriSource = new Uri(filename);
            this.bitmap.EndInit();
            ImageViewer.Source = this.bitmap;
            this.image = new MyImage(filename);
            this.flag = true;
            this.name = Directory.GetCurrentDirectory(); ;
        }
        private void Button_Rotation(object sender, RoutedEventArgs e)
        {
            RotateTransform rotateTransform = new RotateTransform(45);

        }

        #region Traitemement d'image (TD3)

        public void NuanceDeGris(object sender, RoutedEventArgs e)
        {
            if (this.flag == true)
            {
                Pixel[,] matriceBGR = image.MatriceBGR;
                int moyenne = 0;
                for (int i = 0; i < matriceBGR.GetLength(0); i++)
                {
                    for (int j = 0; j < matriceBGR.GetLength(1); j++)
                    {
                        moyenne = (matriceBGR[i, j].R + matriceBGR[i, j].V + matriceBGR[i, j].B) / 3;
                        matriceBGR[i, j].R = moyenne;
                        matriceBGR[i, j].V = moyenne;
                        matriceBGR[i, j].B = moyenne;
                        moyenne = 0;
                    }
                }
                image.MatriceBGR = matriceBGR;
                image.From_Image_To_File(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap = new BitmapImage();
                this.bitmap.BeginInit();
                this.bitmap.UriSource = new Uri(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap.EndInit();
                ImageViewer.Source = this.bitmap;
                compteurDeModification++;
            }
        }
        public void NoirEtBlanc(object sender, RoutedEventArgs e)
        {
            if (this.flag == true)
            {
                int valeur = 128;
                Pixel[,] matriceBGR = image.MatriceBGR;
                int moyenne = 0;
                for (int i = 0; i < matriceBGR.GetLength(0); i++)
                {
                    for (int j = 0; j < matriceBGR.GetLength(1); j++)
                    {
                        moyenne = (matriceBGR[i, j].R + matriceBGR[i, j].V + matriceBGR[i, j].B) / 3;
                        if (moyenne < valeur) moyenne = 0;
                        else moyenne = 255;
                        matriceBGR[i, j].R = moyenne;
                        matriceBGR[i, j].V = moyenne;
                        matriceBGR[i, j].B = moyenne;
                    }
                }
                image.MatriceBGR = matriceBGR;

                image.From_Image_To_File(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap = new BitmapImage();
                this.bitmap.BeginInit();
                this.bitmap.UriSource = new Uri(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap.EndInit();
                ImageViewer.Source = this.bitmap;
                compteurDeModification++;
            }
        }

        public void Miroir(object sender, RoutedEventArgs e)
        {
            if (this.flag == true)
            {
                Pixel[,] matriceBGR = image.MatriceBGR;
                Pixel[,] matriceBGRMiroir = new Pixel[matriceBGR.GetLength(0), matriceBGR.GetLength(1)];
                for (int i = 0; i < matriceBGR.GetLength(0); i++)
                {
                    for (int j = 0; j < matriceBGR.GetLength(1); j++)
                    {
                        matriceBGRMiroir[i, j] = matriceBGR[i, matriceBGR.GetLength(1) - 1 - j];
                    }
                }
                image.MatriceBGR = matriceBGRMiroir;
                image.From_Image_To_File(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap = new BitmapImage();
                this.bitmap.BeginInit();
                this.bitmap.UriSource = new Uri(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap.EndInit();
                ImageViewer.Source = this.bitmap;
                compteurDeModification++;
            }
        }
        #region Fonction agrandir
        private void ___TextBox2__TextChanged(object sender, TextChangedEventArgs e)
        {
            this.texte2 = ___TextBox2__.Text;
            flag3 = true;
        }

        private void CheckBox_Checked2(object sender, RoutedEventArgs e)
        {
            if (this.flag == true && flag3 == true)
            {

                int a = 0;
                decimal agrandis = Convert.ToDecimal(texte2);
                int longueur = Convert.ToInt32(Math.Truncate(image.MatriceBGR.GetLength(0) * agrandis));
                int largeur = Convert.ToInt32(Math.Truncate(image.MatriceBGR.GetLength(1) * agrandis));
                Pixel[,] MatriceBGRnew = new Pixel[longueur, largeur];
                Random rnd = new Random();

                // initialisation de la nouvelle matrice
                for (int i = 0; i < longueur; i++)
                {
                    for (int j = 0; j < largeur; j++)
                    {
                        MatriceBGRnew[i, j] = new Pixel(0, 0, 0);
                    }
                }

                //facteur d'aggrandissement =1
                if (largeur - image.MatriceBGR.GetLength(1) == 0)
                {
                    for (int i = 0; i < image.MatriceBGR.GetLength(0); i++)
                    {
                        for (int j = 0; j < image.MatriceBGR.GetLength(1); j++)
                        {
                            MatriceBGRnew[i, j] = image.MatriceBGR[i, j];
                        }
                    }
                }

                //facteur d'aggrandissement !=1
                else
                {
                    //facteur d'aggrandissement <1
                    if (largeur - image.MatriceBGR.GetLength(1) < 0)
                    {
                        decimal b = Math.Round(agrandis, 1);
                        for (int i = 0; i < longueur; i++)
                        {
                            int cptLongeur = 0;
                            for (int j = 0; j < largeur; j++)
                            {
                                int p = rnd.Next(0, 10);
                                if (p <= b * 10)
                                {
                                    cptLongeur++;
                                }
                                if (j + cptLongeur < image.MatriceBGR.GetLength(1)) MatriceBGRnew[i, j] = image.MatriceBGR[i, j + cptLongeur];
                            }
                        }
                        for (int j = 0; j < largeur; j++)
                        {
                            int cptLongueur = 0;
                            for (int i = 0; i < longueur; i++)
                            {
                                int p = rnd.Next(0, 10);
                                if (p <= b * 10)
                                {
                                    cptLongueur++;
                                }
                                if (i + cptLongueur < image.MatriceBGR.GetLength(0)) MatriceBGRnew[i, j] = image.MatriceBGR[i + cptLongueur, j];
                            }
                        }
                    }
                    //facteur d'aggrandissement >1
                    else
                    {
                        //aggrandissement en largeur
                        a = Convert.ToInt32(Math.Floor(agrandis));
                        double b = Convert.ToDouble(Convert.ToSingle(agrandis - a) % Convert.ToSingle(1));
                        b = Math.Round(b, 1);
                        int cptLargeur = 0;
                        for (int i = 0; i < image.MatriceBGR.GetLength(0); i++)
                        {
                            cptLargeur = 0;
                            for (int j = 0; j < image.MatriceBGR.GetLength(1); j++)
                            {
                                for (int index = 0; index < a; index++)
                                {
                                    MatriceBGRnew[i, j + index + cptLargeur] = image.MatriceBGR[i, j];
                                }
                                cptLargeur += a - 1;
                            }
                        }
                        cptLargeur = 0;
                        bool[] changementl = new bool[largeur];
                        for (int j = 0; j < largeur; j++)
                        {
                            changementl[j] = false;
                        }
                        for (int j = 0; j < image.MatriceBGR.GetLength(1); j++)
                        {

                            int p = rnd.Next(0, 10);
                            if (p <= b * 10)
                            {
                                if (j + cptLargeur < largeur)
                                {
                                    if (j + 1 < image.MatriceBGR.GetLength(1))
                                    {
                                        MatriceBGRnew[0, j + cptLargeur].R = (MatriceBGRnew[0, j].R + MatriceBGRnew[0, j + 1].R) / 2;
                                        MatriceBGRnew[0, j + cptLargeur].V = (MatriceBGRnew[0, j].V + MatriceBGRnew[0, j + 1].V) / 2;
                                        MatriceBGRnew[0, j + cptLargeur].B = (MatriceBGRnew[0, j].B + MatriceBGRnew[0, j + 1].B) / 2;
                                    }
                                    else
                                    {
                                        MatriceBGRnew[0, j + cptLargeur].R = (MatriceBGRnew[0, j].R + 0) / 2;
                                        MatriceBGRnew[0, j + cptLargeur].V = (MatriceBGRnew[0, j].V + 0) / 2;
                                        MatriceBGRnew[0, j + cptLargeur].B = (MatriceBGRnew[0, j].B + 0) / 2;
                                    }
                                    changementl[j + cptLargeur] = true;
                                    cptLargeur++;
                                }
                            }
                        }
                        for (int i = 1; i < image.MatriceBGR.GetLength(0); i++)
                        {
                            cptLargeur = 0;
                            for (int j = 0; j < image.MatriceBGR.GetLength(1); j++)
                            {
                                if (j + cptLargeur < largeur)
                                {
                                    if (changementl[j + cptLargeur] == true)
                                    {
                                        if (j + 1 < image.MatriceBGR.GetLength(1))
                                        {
                                            MatriceBGRnew[i, j + cptLargeur].R = (MatriceBGRnew[i, j].R + MatriceBGRnew[i, j + 1].R) / 2;
                                            MatriceBGRnew[i, j + cptLargeur].V = (MatriceBGRnew[i, j].V + MatriceBGRnew[i, j + 1].V) / 2;
                                            MatriceBGRnew[i, j + cptLargeur].B = (MatriceBGRnew[i, j].B + MatriceBGRnew[i, j + 1].B) / 2;
                                        }
                                        else
                                        {
                                            MatriceBGRnew[i, j + cptLargeur].R = (MatriceBGRnew[i, j].R + 0) / 2;
                                            MatriceBGRnew[i, j + cptLargeur].V = (MatriceBGRnew[i, j].V + 0) / 2;
                                            MatriceBGRnew[i, j + cptLargeur].B = (MatriceBGRnew[i, j].B + 0) / 2;
                                        }
                                        cptLargeur++;
                                    }
                                }
                            }
                        }

                        int cptLongueur = 0;
                        //aggrandissement en hauteur
                        bool[] changementL = new bool[longueur];
                        for (int j = 0; j < longueur; j++)
                        {
                            changementl[j] = false;
                        }
                        for (int i = 0; i < image.MatriceBGR.GetLength(1); i++)
                        {
                            cptLongueur = 0;
                            for (int j = 0; j < image.MatriceBGR.GetLength(0); j++)
                            {
                                for (int index = 0; index < a; index++)
                                {
                                    MatriceBGRnew[j + index + cptLongueur, i] = image.MatriceBGR[j, i];
                                }
                                cptLongueur += a - 1;
                            }
                        }

                        cptLongueur = 0;
                        for (int i = 0; i < image.MatriceBGR.GetLength(0); i++)
                        {
                            int p = rnd.Next(0, 10);
                            if (p <= b * 10)
                            {
                                if (i + cptLongueur < longueur)
                                {
                                    if (i + 1 < image.MatriceBGR.GetLength(0))
                                    {
                                        MatriceBGRnew[i + cptLongueur, 0].R = (image.MatriceBGR[i, 0].R + image.MatriceBGR[i + 1, 0].R) / 2;
                                        MatriceBGRnew[i + cptLongueur, 0].V = (image.MatriceBGR[i, 0].V + image.MatriceBGR[i + 1, 0].V) / 2;
                                        MatriceBGRnew[i + cptLongueur, 0].B = (image.MatriceBGR[i, 0].B + image.MatriceBGR[i + 1, 0].B) / 2;
                                    }
                                    else
                                    {
                                        MatriceBGRnew[i + cptLongueur, 0].R = (image.MatriceBGR[i, 0].R + 0) / 2;
                                        MatriceBGRnew[i + cptLongueur, 0].V = (image.MatriceBGR[i, 0].V + 0) / 2;
                                        MatriceBGRnew[i + cptLongueur, 0].B = (image.MatriceBGR[i, 0].B + 0) / 2;
                                    }
                                    changementl[i + cptLongueur] = true;
                                    cptLongueur++;
                                }
                            }
                        }
                        for (int j = 1; j < image.MatriceBGR.GetLength(1); j++)
                        {
                            cptLargeur = 0;
                            for (int i = 0; i < image.MatriceBGR.GetLength(0); i++)
                            {
                                if (i + cptLongueur < longueur)
                                {
                                    if (changementl[i + cptLongueur] == true)
                                    {
                                        if (i + 1 < image.MatriceBGR.GetLength(0))
                                        {
                                            MatriceBGRnew[i + cptLongueur, j].R = (MatriceBGRnew[i, j].R + MatriceBGRnew[i + 1, j].R) / 2;
                                            MatriceBGRnew[i + cptLongueur, j].V = (MatriceBGRnew[i, j].V + MatriceBGRnew[i + 1, j].V) / 2;
                                            MatriceBGRnew[i + cptLongueur, j].B = (MatriceBGRnew[i, j].B + MatriceBGRnew[i + 1, j].B) / 2;
                                        }
                                        else
                                        {
                                            MatriceBGRnew[i + cptLongueur, j].R = (MatriceBGRnew[i, j].R + 0) / 2;
                                            MatriceBGRnew[i + cptLongueur, j].V = (MatriceBGRnew[i, j].V + 0) / 2;
                                            MatriceBGRnew[i + cptLongueur, j].B = (MatriceBGRnew[i, j].B + 0) / 2;
                                        }
                                        cptLargeur++;
                                    }
                                }
                            }
                        }

                    }
                }
                image.MatriceBGR = MatriceBGRnew;
                image.Taille = image.Offset + longueur * largeur * 3;
                image.Largeur = largeur;
                image.Hauteur = longueur;
                byte[] largeur2 = image.Convertir_Int_To_Endian(largeur, 4);
                byte[] d = image.Convertir_Int_To_Endian(image.Taille, 4);
                byte[] header = image.Header;
                header[2] = d[0];
                header[3] = d[1];
                header[4] = d[2];
                header[5] = d[3];
                header[17] = largeur2[0];
                header[18] = largeur2[1];
                header[19] = largeur2[2];
                header[20] = largeur2[3];
                byte[] hauteur = image.Convertir_Int_To_Endian(largeur, 4);
                header[21] = hauteur[0];
                header[22] = hauteur[1];
                header[23] = hauteur[2];
                header[24] = hauteur[3];
                image.Header = header;

            }
            CheckBox2.IsChecked = false;
        }
        #endregion
        #region filtre
        public void fileExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Close this window
            this.Close();
            for (int i = compteurDeModification - 3; i >= 0; i--)
            {
                File.Delete(name + "\\temp" + i + ".bmp");
            }
        }
        private void Flou(object sender, RoutedEventArgs e)
        {
            if (flag == true)
            {
                //flou
                image = Filtre.Convolution(image, 3);
                image.From_Image_To_File(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap = new BitmapImage();
                this.bitmap.BeginInit();
                this.bitmap.UriSource = new Uri(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap.EndInit();
                ImageViewer.Source = this.bitmap;
                compteurDeModification++;
            }
        }

        private void RenfortBord(object sender, RoutedEventArgs e)
        {
            if (flag == true)
            {
                //renfort des bord
                image = Filtre.Convolution(image, 2);
                image.From_Image_To_File(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap = new BitmapImage();
                this.bitmap.BeginInit();
                this.bitmap.UriSource = new Uri(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap.EndInit();
                ImageViewer.Source = this.bitmap;
                compteurDeModification++;
            }
        }
        private void RessortirContour(object sender, RoutedEventArgs e)
        {
            if (flag == true)
            {
                //ressortir les contours
                image = Filtre.Convolution(image, 1);
                image.From_Image_To_File(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap = new BitmapImage();
                this.bitmap.BeginInit();
                this.bitmap.UriSource = new Uri(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap.EndInit();
                ImageViewer.Source = this.bitmap;
                compteurDeModification++;
            }
        }
        private void Repoussage(object sender, RoutedEventArgs e)
        {
            if (flag == true)
            {
                //Repoussage
                image = Filtre.Convolution(image, 4);
                image.From_Image_To_File(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap = new BitmapImage();
                this.bitmap.BeginInit();
                this.bitmap.UriSource = new Uri(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap.EndInit();
                ImageViewer.Source = this.bitmap;
                compteurDeModification++;
            }
        }
        public static void Rotation(MyImage image, int angle)
        {
            Pixel[,] matriceBGR = image.MatriceBGR;
            double longueur = Math.Ceiling(Math.Cos(angle) * matriceBGR.GetLength(0) + Math.Cos(angle) * matriceBGR.GetLength(1));
            double largeur = Math.Ceiling(Math.Sin(angle) * matriceBGR.GetLength(0) + Math.Sin(angle) * matriceBGR.GetLength(1));
            Pixel[,] matriceBGRRotation = new Pixel[Convert.ToInt32(Math.Abs(longueur)), Convert.ToInt32(Math.Abs(largeur))];
            for (int i = 0; i < matriceBGRRotation.GetLength(0); i++)
            {
                for (int j = 0; j < matriceBGRRotation.GetLength(1); j++)
                {
                    matriceBGRRotation[i, j] = new Pixel(0, 0, 0, true);
                }
            }
            for (int i = 0; i < matriceBGR.GetLength(0); i++)
            {
                for (int j = 0; j < matriceBGR.GetLength(1); j++)
                {
                    if (matriceBGR[i, j].PixelNoir != true)
                    {
                        int rayon = Convert.ToInt32(Math.Ceiling(Math.Sqrt(Math.Pow(i, 2) + Math.Pow(j, 2))));
                        matriceBGRRotation[Convert.ToInt32(Math.Abs(Math.Floor(Math.Cos(angle) * i + Math.Cos(angle) * j))), Convert.ToInt32(Math.Abs(Math.Floor(Math.Sin(angle) * i + Math.Sin(angle) * j)))] = matriceBGR[i, j];
                    }
                }
            }
            image.MatriceBGR = matriceBGRRotation;
        }
        #endregion
        #endregion
        #region Enregistration de l'image
        private void ___TextBox1__TextChanged(object sender, TextChangedEventArgs e)
        {
            this.texte = ___TextBox1_.Text;
            flag2 = true;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.flag == true && flag2 == true)
            {
                image.From_Image_To_File(name + "\\" + texte + ".bmp");
            }
            Thread.Sleep(100);
            CheckBox1.IsChecked = false;
        }
        #endregion

        #region Creation d'image 
        private void Histo(object sender, RoutedEventArgs e)
        {
            if (this.flag == true)
            {
                image = Creation.Histogramme(image);
                image.From_Image_To_File(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap = new BitmapImage();
                this.bitmap.BeginInit();
                this.bitmap.UriSource = new Uri(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap.EndInit();
                ImageViewer.Source = this.bitmap;
                compteurDeModification++;
            }
        }
        #endregion

    }
}
    


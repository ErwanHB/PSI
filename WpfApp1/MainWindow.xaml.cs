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
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.bmp)|*.bmp";
        }

        private void Ouvrir(object sender, RoutedEventArgs e)
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
            this.name = Directory.GetCurrentDirectory();
        }
        public void Sortie(object sender, RoutedEventArgs e)
        {
            // Close this window
            this.Close();
            for (int i = compteurDeModification - 3; i >= 0; i--)
            {
                File.Delete(name + "\\temp" + i + ".bmp");
            }
        }
        private void Enregistrer(object sender, RoutedEventArgs e)
        {
            if (this.flag == true)
            {
                image.From_Image_To_File(name + "\\" + TextBoxNouveau.Text + ".bmp");
            }
            Thread.Sleep(250);
            CheckBoxNouveau.IsChecked = false;
        }

        #region Traitemement d'image (TD3)

        #region modification
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
                int valeur = Convert.ToInt32(seuil.Text);
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
            Thread.Sleep(250);
            CheckBoxNoir.IsChecked = false;
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
        private void Agrandir(object sender, RoutedEventArgs e)
        {
            if (this.flag == true)
            {

                int a = 0;
                decimal agrandis = Convert.ToDecimal(coefficient.Text);
                int longueur = Convert.ToInt32(Math.Truncate(image.MatriceBGR.GetLength(0) * agrandis));
                int largeur = Convert.ToInt32(Math.Truncate(image.MatriceBGR.GetLength(1) * agrandis));
                Pixel[,] MatriceBGRnew = new Pixel[longueur, largeur];
                Random rnd = new Random();
                Pixel[,] MatriceBGRtemp = new Pixel[longueur, largeur];
                for(int i=0;i<longueur;i++)
                {
                    for(int j=0;j<largeur;j++)
                    {
                        MatriceBGRtemp[i, j] = new Pixel(0, 0, 0);
                        MatriceBGRnew[i, j] = new Pixel(0, 0, 0);
                    }
                }

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
                        double b = Convert.ToDouble(agrandis);

                        #region retrecissement en largeur
                        bool[] tab = new bool[image.MatriceBGR.GetLength(1)];
                        for (int i = 0; i < image.MatriceBGR.GetLength(1); i++)
                        {
                            tab[i] = false;
                        }
                        int cptLongueur = 0;
                        int cptLargeur = 0;
                        for (int j = 0; j < image.MatriceBGR.GetLength(1); j++)
                        {
                            double p = rnd.NextDouble();
                            if (p <= b)
                            {
                                cptLargeur++;
                            }
                            else
                            {
                                if (j - cptLargeur < largeur)
                                {
                                    tab[j] = true;
                                }
                            }
                        }
                        #endregion

                        #region retrecissement en hauteur
                        bool[] tab2 = new bool[image.MatriceBGR.GetLength(0)];
                        for (int i = 0; i < image.MatriceBGR.GetLength(0); i++)
                        {
                            tab2[i] = false;
                        }
                        for (int j = 0; j < image.MatriceBGR.GetLength(0); j++)
                        {

                            double p = rnd.NextDouble();
                            if (p <= b)
                            {
                                cptLongueur++;
                            }
                            else
                            {
                                if (j - cptLongueur < longueur)
                                {
                                    tab2[j] = true;
                                }
                            }
                        }
                        #endregion

                        #region matrice
                        int cptTemporaire1 = 1;
                        int cptTemporaire2 = 1;
                        cptLongueur = 0;
                        for (int i = 0; i < image.MatriceBGR.GetLength(0); i++)
                        {
                            cptLargeur = 0;
                            if (tab2[i] != true)
                            {
                                cptLongueur++;
                                cptTemporaire1++;
                            }
                            else
                            {
                                for (int j = 0; j < image.MatriceBGR.GetLength(1); j++)
                                {
                                    if (tab[j] != true)
                                    {
                                        cptLargeur++;
                                        cptTemporaire2++;
                                    }
                                    else
                                    {
                                        Pixel moyenne = new Pixel(0, 0, 0);
                                        for (int cpt1 = 0; cpt1 < cptTemporaire1; cpt1++)
                                        {
                                            for (int cpt2 = 0; cpt2 < cptTemporaire2; cpt2++)
                                            {
                                                moyenne.R += image.MatriceBGR[i - cpt1, j - cpt2].R;
                                                moyenne.V += image.MatriceBGR[i - cpt1, j - cpt2].V;
                                                moyenne.B += image.MatriceBGR[i - cpt1, j - cpt2].B;
                                            }
                                        }
                                        MatriceBGRnew[i - cptLongueur, j - cptLargeur].R = moyenne.R / (cptTemporaire1 * cptTemporaire2);
                                        MatriceBGRnew[i - cptLongueur, j - cptLargeur].V = moyenne.V / (cptTemporaire1 * cptTemporaire2);
                                        MatriceBGRnew[i - cptLongueur, j - cptLargeur].B = moyenne.B / (cptTemporaire1 * cptTemporaire2);
                                        cptTemporaire2 = 1;
                                    }
                                    if (j - cptLargeur == largeur - 1)
                                    {
                                        cptTemporaire2 = 1;
                                    }
                                }
                                cptTemporaire1 = 1;
                            }
                        }
                        #endregion
                    }
                    //facteur d'aggrandissement >1
                    else
                    {
                        a = Convert.ToInt32(Math.Floor(agrandis));
                        double b = Convert.ToDouble(Convert.ToSingle(agrandis - a) % Convert.ToSingle(1));
                        int cptLargeur = 0;
                        int cptLongueur = 0;

                        #region aggrandissmenet coef partie complete
                        for (int i = 0; i < image.MatriceBGR.GetLength(0); i++)
                        {

                            for (int cpt1 = 0; cpt1 < a; cpt1++)
                            {
                                for (int j = 0; j < image.MatriceBGR.GetLength(1); j++)
                                {
                                    for (int cpt2 = 0; cpt2 < a; cpt2++)
                                    {
                                        MatriceBGRnew[i + cptLongueur, j + cptLargeur] = image.MatriceBGR[i, j];
                                        cptLargeur++;
                                    }
                                    cptLargeur--;
                                }
                                cptLargeur = 0;
                                cptLongueur++;
                            }
                            cptLongueur--;
                        }
                        #endregion

                        #region  aggrandissmenet coef partie decimal
                        //configuration tableau de bool 1
                        cptLargeur = 0;
                        for (int j = 0; j < image.MatriceBGR.GetLength(1); j++)
                        {

                            Double p = rnd.NextDouble();
                            if (p <= b)
                            {
                                for (int i = 0; i < image.MatriceBGR.GetLength(0) * a; i++)
                                { if (j + cptLargeur < largeur)
                                    {
                                        MatriceBGRtemp[i, j + cptLargeur] = MatriceBGRnew[i, j];
                                        if (j + 1 + cptLargeur < largeur)
                                        {
                                            MatriceBGRtemp[i, j + 1 + cptLargeur] = MatriceBGRnew[i, j];
                                        }
                                    }

                                }
                                cptLargeur++;
                            }
                            else
                            {
                                for(int i=0;i< image.MatriceBGR.GetLength(0)*a;i++)
                                {
                                    if (j + cptLargeur < largeur)
                                    {
                                        MatriceBGRtemp[i, j + cptLargeur] = MatriceBGRnew[i, j];
                                    }
                                }
                            }
                        }

                            
                        

                        #endregion
                    }
                }
                image.MatriceBGR = MatriceBGRtemp;
                image.Taille = image.Offset + longueur * largeur * 3;
                image.Largeur = largeur;
                image.Hauteur = longueur;
                byte[] largeur2 = image.Convertir_Int_To_Endian(largeur, 4);
                byte[] taille2 = image.Convertir_Int_To_Endian(image.Taille, 4);
                byte[] hauteur2 = image.Convertir_Int_To_Endian(longueur, 4);
                byte[] taille_image2 = image.Convertir_Int_To_Endian((largeur*longueur * 3), 4);
                byte[] header = image.Header;
                header[2] = taille2[0];
                header[3] = taille2[1];
                header[4] = taille2[2];
                header[5] = taille2[3];
                header[18] = largeur2[0];
                header[19] = largeur2[1];
                header[20] = largeur2[2];
                header[21] = largeur2[3];
                header[22] = hauteur2[0];
                header[23] = hauteur2[1];
                header[24] = hauteur2[2];
                header[25] = hauteur2[3];
                header[34] = taille_image2[0];
                header[35] = taille_image2[1];
                header[36] = taille_image2[2];
                header[37] = taille_image2[3];
                image.Header = header;

                image.From_Image_To_File(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap = new BitmapImage();
                this.bitmap.BeginInit();
                this.bitmap.UriSource = new Uri(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap.EndInit();
                ImageViewer.Source = this.bitmap;
                compteurDeModification++;

            }
            Thread.Sleep(250);
            CheckBoxAgrandir.IsChecked = false;
        }
        #endregion

        #region filtre
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

        private void Fractale(object sender, RoutedEventArgs e)
        {
            this.name = Directory.GetCurrentDirectory();
            image = Creation.Fractale();
            image.From_Image_To_File(name + "\\temp" + compteurDeModification + ".bmp");
            this.bitmap = new BitmapImage();
            this.bitmap.BeginInit();
            this.bitmap.UriSource = new Uri(name + "\\temp" + compteurDeModification + ".bmp");
            this.bitmap.EndInit();
            ImageViewer.Source = this.bitmap;
            compteurDeModification++;
            flag = true;

        }
        #endregion

    }
}
    


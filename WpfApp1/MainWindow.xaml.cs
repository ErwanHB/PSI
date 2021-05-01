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
        //variable necessaire pour l'ouverture des images avec wpf
        OpenFileDialog dlg; 
        BitmapImage bitmap;

        //variable qui stocke notre variable
        MyImage image;

        //compteur qui stocke le nombre de modification que l'on a effectuée sur l'image 
        int compteurDeModification;

        //variable qui garde en memoire si on a ouvert une image ou non
        bool flag = false;

        //nom de l'endroit d'ou vient notre image
        string name;

        //variable necessaire pour l'ouverture des deux images en stenographie et pour le stockage de la methode
        BitmapImage steno1;
        MyImage imageSteno1;
        BitmapImage steno2;
        MyImage imageSteno2;

        //variable cardant en memoire si on a bien ouvert deux images
        bool flagStenographie = false;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.bmp)|*.bmp";
        }

        #region elements de base : ouverture, enregistrement, sortie
        /// <summary>
        /// Methode pour ouvrir une nouvelle image a modifier
        /// </summary>
        private void Ouvrir(object sender, RoutedEventArgs e)
        {
            
            //ImageViewer.Visibility = Visibility.Visible;
            ImageStenographie1.Visibility = Visibility.Hidden;
            ImageStenographie2.Visibility = Visibility.Hidden;
            string filename = null;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filename = dlg.FileName;

            }
            this.bitmap = new BitmapImage();
            this.bitmap.BeginInit();
            this.bitmap.UriSource = new Uri(filename);
            this.bitmap.EndInit();
            if (ImageQrcode.Visibility == Visibility.Visible)
            {
                ImageQrcode.Source = this.bitmap;
                FondBlanc.Visibility = Visibility.Visible;
            }
            else
            {
                ImageViewer.Source = this.bitmap;
            }
           
            this.image = new MyImage(filename);
            this.flag = true;
            this.name = Directory.GetCurrentDirectory();
        }

        /// <summary>
        /// Methode pour sortir du logiciel et detruire la majoeité des fichiers temporaires que l'on a crée
        /// </summary>
        public void Sortie(object sender, RoutedEventArgs e)
        {
            // Close this window
            this.Close();
            for (int i = compteurDeModification - 3; i >= 0; i--)
            {
                File.Delete(name + "\\temp" + i + ".bmp");
            }
        }

        /// <summary>
        /// Methode pour enregistrer notre nouvelle image
        /// </summary>
        private void Enregistrer(object sender, RoutedEventArgs e)
        {
            if (this.flag == true)
            {
                image.From_Image_To_File(name + "\\" + TextBoxNouveau.Text + ".bmp");
            }
            Thread.Sleep(250);
            CheckBoxNouveau.IsChecked = false;
        }
        #endregion

        #region gestion de la fenetre
        /// <summary>
        /// methode pour passer au mode QR code du logiciel
        /// </summary>
        /// on passe en visible les elements necessaire pour l'utilisation du mode QR code du logiciel et on cache les autres
        private void ModeQRCode(object sender, RoutedEventArgs e)
        {
            TextIntro.Visibility = Visibility.Hidden;
            BoutonImage.Visibility = Visibility.Hidden;
            BoutonQRcode.Visibility = Visibility.Hidden;

            BoutonOuvrir.Visibility = Visibility.Visible;
            ImageQrcode.Visibility = Visibility.Visible;
            passage21.Visibility = Visibility.Visible;
            QRcodeEspace1.Visibility = Visibility.Visible;
            ExplicationGeneration.Visibility = Visibility.Visible;
            TextBoxGenerateur.Visibility = Visibility.Visible;
            ExplicationGeneration2.Visibility = Visibility.Visible;
            TextBoxGenerateur2.Visibility = Visibility.Visible;
            checkboxGenerateur.Visibility = Visibility.Visible;
            QRcodeEspace2.Visibility = Visibility.Visible;
            TexteOuvrir.Visibility = Visibility.Visible;
            checkboxLecteur.Visibility = Visibility.Visible;
            ExplicationLecteur2.Visibility = Visibility.Visible;
            TextNouveau.Visibility = Visibility.Visible;
            TextBoxNouveau.Visibility = Visibility.Visible;
            CheckBoxNouveau.Visibility = Visibility.Visible;//a changer car pas de registre ou enregistrer
        }

        /// <summary>
        /// methode pour passer au mode creation et transformation d'image du logiciel
        /// </summary>
        /// on passe en visible les elements necessaire pour l'utilisation du mode creation et transformation d'image du logiciel et on cache les autres
        private void ModeImage(object sender, RoutedEventArgs e)
        {
            ImageViewer.Visibility = Visibility.Visible;
            BoutonOuvrir.Visibility = Visibility.Visible;
            ComboModif.Visibility = Visibility.Visible;
            TextModif.Visibility = Visibility.Visible;
            TextCreation.Visibility = Visibility.Visible;
            ComboCreation.Visibility = Visibility.Visible;
            ComboFiltre.Visibility = Visibility.Visible;
            TextFiltre.Visibility = Visibility.Visible;
            TextNouveau.Visibility = Visibility.Visible;
            TextBoxNouveau.Visibility = Visibility.Visible;
            CheckBoxNouveau.Visibility = Visibility.Visible;
            passage12.Visibility = Visibility.Visible;
            ImageViewer.Visibility = Visibility.Visible;

            ImageQrcode.Visibility = Visibility.Hidden;
            TextIntro.Visibility = Visibility.Hidden;
            BoutonImage.Visibility = Visibility.Hidden;
            BoutonQRcode.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Methode pour passer du traitement d'image à la stenographie
        /// </summary>
        private void StenographieOuvrir(object sender, RoutedEventArgs e)
        {
            this.flag = false;
            ImageViewer.Visibility = Visibility.Hidden;
            ImageStenographie1.Visibility = Visibility.Visible;
            ImageStenographie2.Visibility = Visibility.Visible;
            string filename = null;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filename = dlg.FileName;

            }
            this.steno1 = new BitmapImage();
            this.steno1.BeginInit();
            this.steno1.UriSource = new Uri(filename);
            this.steno1.EndInit();
            ImageStenographie1.Source = this.steno1;
            this.imageSteno1 = new MyImage(filename);
            this.name = Directory.GetCurrentDirectory();

            filename = null;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filename = dlg.FileName;

            }
            this.steno2 = new BitmapImage();
            this.steno2.BeginInit();
            this.steno2.UriSource = new Uri(filename);
            this.steno2.EndInit();
            ImageStenographie2.Source = this.steno2;
            this.imageSteno2 = new MyImage(filename);
            this.name = Directory.GetCurrentDirectory();
            this.flagStenographie = true;
        }

        /// <summary>
        /// Methode pourr passer du mode image au mode QR code
        /// </summary>
        private void PassageModeQRCode(object sender, RoutedEventArgs e)
        {
            ComboModif.Visibility = Visibility.Hidden;
            TextModif.Visibility = Visibility.Hidden;
            TextCreation.Visibility = Visibility.Hidden;
            ComboCreation.Visibility = Visibility.Hidden;
            ComboFiltre.Visibility = Visibility.Hidden;
            TextFiltre.Visibility = Visibility.Hidden;
            passage12.Visibility= Visibility.Hidden;
            ImageViewer.Visibility = Visibility.Hidden;

            ImageQrcode.Visibility = Visibility.Visible;
            passage21.Visibility = Visibility.Visible;
            QRcodeEspace1.Visibility = Visibility.Visible;
            ExplicationGeneration.Visibility = Visibility.Visible;
            TextBoxGenerateur.Visibility = Visibility.Visible;
            ExplicationGeneration2.Visibility = Visibility.Visible;
            TextBoxGenerateur2.Visibility = Visibility.Visible;
            checkboxGenerateur.Visibility = Visibility.Visible;
            QRcodeEspace2.Visibility = Visibility.Visible;
            TexteOuvrir.Visibility = Visibility.Visible;
            checkboxLecteur.Visibility = Visibility.Visible;
            ExplicationLecteur2.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Methode pourr passer du mode QR code au mode image
        /// </summary>
        private void PassageImage(object sender, RoutedEventArgs e)
        {
            ComboModif.Visibility = Visibility.Visible;
            TextModif.Visibility = Visibility.Visible;
            TextCreation.Visibility = Visibility.Visible;
            ComboCreation.Visibility = Visibility.Visible;
            ComboFiltre.Visibility = Visibility.Visible;
            TextFiltre.Visibility = Visibility.Visible;
            TextNouveau.Visibility = Visibility.Visible;
            TextBoxNouveau.Visibility = Visibility.Visible;
            CheckBoxNouveau.Visibility = Visibility.Visible;
            passage12.Visibility = Visibility.Visible;
            ImageViewer.Visibility = Visibility.Visible;

            FondBlanc.Visibility = Visibility.Hidden;
            ImageQrcode.Visibility = Visibility.Hidden;
            passage21.Visibility = Visibility.Hidden;
            QRcodeEspace1.Visibility = Visibility.Hidden;
            ExplicationGeneration.Visibility = Visibility.Hidden;
            TextBoxGenerateur.Visibility = Visibility.Hidden;
            ExplicationGeneration2.Visibility = Visibility.Hidden;
            TextBoxGenerateur2.Visibility = Visibility.Hidden;
            checkboxGenerateur.Visibility = Visibility.Hidden;
            QRcodeEspace2.Visibility = Visibility.Hidden;
            TexteOuvrir.Visibility = Visibility.Hidden;
            checkboxLecteur.Visibility = Visibility.Hidden;
            ExplicationLecteur2.Visibility = Visibility.Hidden;

        }

        /// <summary>
        /// Methode pour enlever le message d'erreur du QR code
        /// </summary>
        private void ErreurQR(object sender, RoutedEventArgs e)
        {
            Erreur.Visibility = Visibility.Hidden;
        }
        #endregion

        #region Traitemement d'image

        #region modification
        /// <summary>
        /// Fonction qui fait la moyenne des couleurs des pixels d'une image pour passer l'image en nuance de gris
        /// </summary>
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
                #region enregistrement, apparition de la nouvelle image 
                image.MatriceBGR = matriceBGR;
                image.From_Image_To_File(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap = new BitmapImage();
                this.bitmap.BeginInit();
                this.bitmap.UriSource = new Uri(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap.EndInit();
                ImageViewer.Source = this.bitmap;
                compteurDeModification++;
                #endregion
            }
        }

        /// <summary>
        /// Methode pour passer l'image en Noir et Blanc
        /// </summary>
        /// La fonction fait la moyenne des valeurs RGB des pixels puis la compare à la valeur seuil, 128 par defaut, pour voir si c'est noir ou blanc
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
                #region enregistrement, apparition de la nouvelle image 
                image.MatriceBGR = matriceBGR;

                image.From_Image_To_File(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap = new BitmapImage();
                this.bitmap.BeginInit();
                this.bitmap.UriSource = new Uri(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap.EndInit();
                ImageViewer.Source = this.bitmap;
                compteurDeModification++;
                #endregion
            }
            Thread.Sleep(250);
            CheckBoxNoir.IsChecked = false;
        }

        /// <summary>
        /// Fonction qui permet d'appliquer un effet mirroir à notre image
        /// </summary>
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
                #region enregistrement, apparition de la nouvelle image 
                image.MatriceBGR = matriceBGRMiroir;
                image.From_Image_To_File(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap = new BitmapImage();
                this.bitmap.BeginInit();
                this.bitmap.UriSource = new Uri(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap.EndInit();
                ImageViewer.Source = this.bitmap;
                compteurDeModification++;
                #endregion
            }
        }

        /// <summary>
        /// Fonction qui permet d'agrancir ou retrecir notre image
        /// </summary>
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
                for (int i = 0; i < longueur; i++)
                {
                    for (int j = 0; j < largeur; j++)
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
                        //aggrandissement largeur
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
                                for (int i = 0; i < image.MatriceBGR.GetLength(0) * a; i++)
                                {
                                    if (j + cptLargeur < largeur)
                                    {
                                        MatriceBGRtemp[i, j + cptLargeur] = MatriceBGRnew[i, j];
                                    }
                                }
                            }
                        }
                        //aggrandissement hauteur
                        cptLongueur = 0;
                        for (int i = 0; i < image.MatriceBGR.GetLength(0); i++)
                        {

                            Double p = rnd.NextDouble();
                            if (p <= b)
                            {
                                for (int j = 0; j < largeur; j++)
                                {
                                    if (i + cptLongueur < longueur)
                                    {
                                        MatriceBGRtemp[i + cptLongueur, j] = MatriceBGRnew[i, j];
                                        if (i + 1 + cptLongueur < longueur)
                                        {
                                            MatriceBGRtemp[i + 1 + cptLongueur, j] = MatriceBGRnew[i, j];
                                        }
                                    }
                                }
                                cptLongueur++;
                            }
                            else
                            {
                                for (int j = 0; j < largeur; j++)
                                {
                                    if (i + cptLongueur < longueur)
                                    {
                                        MatriceBGRtemp[i + cptLongueur, j] = MatriceBGRnew[i, j];
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }

                #region enregistrement, apparition de la nouvelle image 
                image.MatriceBGR = MatriceBGRtemp;
                image.Taille = image.Offset + longueur * largeur * 3;
                image.Largeur = largeur;
                image.Hauteur = longueur;
                byte[] largeur2 = image.Convertir_Int_To_Endian(largeur, 4);
                byte[] taille2 = image.Convertir_Int_To_Endian(image.Taille, 4);
                byte[] hauteur2 = image.Convertir_Int_To_Endian(longueur, 4);
                byte[] taille_image2 = image.Convertir_Int_To_Endian((largeur * longueur * 3), 4);
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
                #endregion

            }
            Thread.Sleep(250);
            CheckBoxAgrandir.IsChecked = false;
        }

        /// <summary>
        /// Fonction qui permet d'appliquer une rotation à notre image de 90 en 90 premiere version "minimum"
        /// </summary>
        private void Rotation(object sender, RoutedEventArgs e)
        {
            if (flag == true)
            {
                int largeur1 = 0;
                int longueur1 = 0;
                int angle = Convert.ToInt32(coeffRotation.Text);

                Pixel[,] matriceBGR = image.MatriceBGR;
                Pixel[,] matriceBGRRotation = new Pixel[0, 0];

                //methode pour un angle de 90°
                if (angle == 90)
                {
                    int longueur = matriceBGR.GetLength(1);
                    longueur1 = longueur;
                    int largeur = matriceBGR.GetLength(0);
                    largeur1 = largeur;
                    matriceBGRRotation = new Pixel[longueur, largeur];
                    for (int j = largeur - 1; j >= 0; j--)
                    {
                        for (int i = 0; i < longueur; i++)
                        {
                            matriceBGRRotation[i, j] = matriceBGR[Math.Abs(j - largeur + 1), i];
                        }
                    }
                }

                //methode pour un angle de 270°
                if (angle == 270)
                {
                    int longueur = matriceBGR.GetLength(1);
                    longueur1 = longueur;
                    int largeur = matriceBGR.GetLength(0);
                    largeur1 = largeur;
                    matriceBGRRotation = new Pixel[longueur, largeur];
                    for (int j = 0; j < largeur; j++)
                    {
                        for (int i = longueur - 1; i >= 0; i--)
                        {
                            matriceBGRRotation[i, j] = matriceBGR[j, Math.Abs(i - longueur + 1)];
                        }
                    }
                }

                //methode pour un angle de 180°
                if (angle == 180)
                {
                    int largeur = matriceBGR.GetLength(1);
                    largeur1 = largeur;
                    int longueur = matriceBGR.GetLength(0);
                    longueur1 = longueur;
                    matriceBGRRotation = new Pixel[longueur, largeur];
                    for (int j = largeur - 1; j >= 0; j--)
                    {
                        for (int i = longueur - 1; i >= 0; i--)
                        {
                            matriceBGRRotation[i, j] = matriceBGR[Math.Abs(i - longueur + 1), Math.Abs(j - largeur + 1)];
                        }
                    }
                }

                //methode pour l'angle 0 et 360
                if (angle == 0 || angle == 360)
                {
                    matriceBGRRotation = matriceBGR;
                    largeur1 = matriceBGR.GetLength(0);
                    longueur1 = matriceBGR.GetLength(1);
                }

                //methode pour les angles compris entre 0° et 90° (non inclus)
                if (angle > 0 && angle < 90)
                {
                    #region calculs des dimensions maximalles de la matrice et initialisation de la matrice
                    //globalement ce que l'on fait c'est un rectangle qui contient toutes les matrices possibles apres rotation. 
                    //on enlevera les lignes et colonnes inutiles à la fin.
                    int max = matriceBGR.GetLength(1); //maximum des "negatifs" vers la gauche apres rotation
                    if (matriceBGR.GetLength(0) > matriceBGR.GetLength(1))
                    {
                        largeur1 = matriceBGR.GetLength(0)+ max;
                        longueur1 = matriceBGR.GetLength(0);
                    }
                    else
                    {
                        largeur1 = matriceBGR.GetLength(1)+ max;
                        longueur1 = matriceBGR.GetLength(1);
                    }
                    matriceBGRRotation = new Pixel[longueur1, largeur1];
                    for (int j = 0; j < largeur1; j++)
                    {
                        for (int i = 0; i < longueur1; i++)
                        {
                            matriceBGRRotation[i, j] = new Pixel(0, 0, 0, true);
                        }
                    }
                    #endregion

                    double radian = Math.PI / 180;
                    int index1 = 0;
                    int index2 = 0;
                    for (int j = 0; j < matriceBGR.GetLength(1); j++)
                    {
                        for (int i = 0; i < matriceBGR.GetLength(0); i++)
                        {
                            if (j == 0)
                            {
                                index1 = Convert.ToInt32(Math.Truncate(Math.Cos(angle * radian + 90 * radian) * Math.Sqrt(Math.Pow(i, 2))));
                                index2 = max + Convert.ToInt32(Math.Truncate(Math.Sin(angle * radian + 90 * radian) * Math.Sqrt(Math.Pow(i, 2))));
                                matriceBGRRotation[Convert.ToInt32(Math.Truncate(Math.Cos(angle * radian + 90 * radian) * Math.Sqrt(Math.Pow(i, 2)))), max+ Convert.ToInt32(Math.Truncate(Math.Sin(angle * radian + 90 * radian) * Math.Sqrt(Math.Pow(i, 2))))] = matriceBGR[i, j];
                            }
                            else
                            {
                                index1 = Convert.ToInt32(Math.Truncate(Math.Cos(angle * radian + Math.Atan(i / j)) * Math.Sqrt(Math.Pow(i, 2) + Math.Pow(j, 2)))) - 2;
                                index2 = max+Convert.ToInt32(Math.Truncate(Math.Sin(angle * radian + Math.Atan(i / j)) * Math.Sqrt(Math.Pow(i, 2) + Math.Pow(j, 2))));
                                matriceBGRRotation[max + Convert.ToInt32(Math.Truncate(Math.Cos(angle * radian + Math.Atan(i / j)) * Math.Sqrt(Math.Pow(i, 2) + Math.Pow(j, 2)))) - 2, Convert.ToInt32(Math.Truncate(Math.Sin(angle * radian + Math.Atan(i / j)) * Math.Sqrt(Math.Pow(i, 2) + Math.Pow(j, 2))))] = matriceBGR[i, j];
                            }
                        }
                    }
                }

                #region enregistrement, apparition de la nouvelle image 
                image.Taille = image.Offset + longueur1 * largeur1 * 3;
                image.Largeur = largeur1;
                image.Hauteur = longueur1;
                byte[] largeur2 = image.Convertir_Int_To_Endian(largeur1, 4);
                byte[] taille2 = image.Convertir_Int_To_Endian(image.Taille, 4);
                byte[] hauteur2 = image.Convertir_Int_To_Endian(longueur1, 4);
                byte[] taille_image2 = image.Convertir_Int_To_Endian((largeur1 * longueur1 * 3), 4);
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


                image.MatriceBGR = matriceBGRRotation;
                image.From_Image_To_File(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap = new BitmapImage();
                this.bitmap.BeginInit();
                this.bitmap.UriSource = new Uri(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap.EndInit();
                ImageViewer.Source = this.bitmap;
                compteurDeModification++;
                #endregion

            }
            Thread.Sleep(250);
            CheckBoxRotation.IsChecked = false;
        }

        /// <summary>
        /// Fonction v2 qui permet d'appliquer une rotation quelconque à notre image
        /// </summary>
        private void Rotation2(object sender, RoutedEventArgs e)
        {
            double angle = Convert.ToDouble(coeffRotation.Text);
            int hauteur_ = this.image.Hauteur;
            int largeur_ = this.image.Largeur;
            Pixel[,] matriceBGRRotation = new Pixel[hauteur_, largeur_];
            Pixel[,] matriceBGR = image.MatriceBGR;

            #region Reduction de l'angle à un angle inferieur à 90°
            while (angle % 360 >= 90)
            {
                matriceBGRRotation = new Pixel[matriceBGR.GetLength(1), matriceBGR.GetLength(0)];
                for (int j = matriceBGRRotation.GetLength(1) - 1; j >= 0; j--)
                {
                    for (int i = 0; i < matriceBGRRotation.GetLength(0); i++)
                    {
                        matriceBGRRotation[i, j] = matriceBGR[Math.Abs(j - matriceBGRRotation.GetLength(1) + 1), i];
                    }
                }
                angle -= 90;
                matriceBGR = matriceBGRRotation;
                this.image.Hauteur = matriceBGR.GetLength(0);
                this.image.Largeur = matriceBGR.GetLength(1);
                hauteur_ = this.image.Hauteur;
                largeur_ = this.image.Largeur;
            }
            angle %= 90;
            #endregion

            #region appliquation de l'angle inferieur a 90°
            if (angle > 0)
            {
                angle *= Math.PI / 180;
                this.image.Hauteur= Convert.ToInt32(largeur_ * Math.Sin(angle) + hauteur_ * Math.Cos(angle));
                this.image.Largeur= Convert.ToInt32(hauteur_ * Math.Sin(angle) + largeur_ * Math.Cos(angle));
                int hauteur = this.image.Hauteur;
                int largeur = this.image.Largeur;
                matriceBGRRotation = new Pixel[hauteur, largeur];
                for (int i = 0; i < hauteur; i++)
                {
                    for (int j = 0; j < largeur; j++)
                    {
                        int I = (int)(Math.Cos(angle) * (i - hauteur / 2) - Math.Sin(angle) * (j - largeur / 2) + hauteur_ / 2);
                        int J = (int)(Math.Sin(angle) * (i - hauteur / 2) + Math.Cos(angle) * (j - largeur / 2) + largeur_ / 2);

                        if (I >= 0 && J >= 0 && I < hauteur_ && J < largeur_)
                        {
                            matriceBGRRotation[i, j] = matriceBGR[I, J];
                        }
                        else
                        {
                            matriceBGRRotation[i, j] = new Pixel(255, 255, 255);
                        }
                    }
                }
                matriceBGR = matriceBGRRotation;
            }
            #endregion

            #region enregistrement, apparition de la nouvelle image 
            image.Taille = image.Offset + this.image.Hauteur * this.image.Largeur * 3;
            byte[] largeur2 = image.Convertir_Int_To_Endian(this.image.Largeur, 4);
            byte[] taille2 = image.Convertir_Int_To_Endian(image.Taille, 4);
            byte[] hauteur2 = image.Convertir_Int_To_Endian(this.image.Hauteur, 4);
            byte[] taille_image2 = image.Convertir_Int_To_Endian((this.image.Largeur * this.image.Hauteur * 3), 4);
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


            image.MatriceBGR = matriceBGRRotation;
            image.From_Image_To_File(name + "\\temp" + compteurDeModification + ".bmp");
            this.bitmap = new BitmapImage();
            this.bitmap.BeginInit();
            this.bitmap.UriSource = new Uri(name + "\\temp" + compteurDeModification + ".bmp");
            this.bitmap.EndInit();
            ImageViewer.Source = this.bitmap;
            compteurDeModification++;
            #endregion

        
        Thread.Sleep(250);
            CheckBoxRotation.IsChecked = false;
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
            ImageViewer.Visibility = Visibility.Visible;
            ImageStenographie1.Visibility = Visibility.Hidden;
            ImageStenographie2.Visibility = Visibility.Hidden;

            this.name = Directory.GetCurrentDirectory();
            image = Creation.Fractale();
            image.From_Image_To_File(name + "\\temp" + compteurDeModification + ".bmp");
            this.bitmap = new BitmapImage();
            this.bitmap.BeginInit();
            this.bitmap.UriSource = new Uri(name + "\\temp" + compteurDeModification + ".bmp");
            this.bitmap.EndInit();
            ImageViewer.Source = this.bitmap;
            compteurDeModification++;
            this.flag = true;

        }

        private void Stenographie(object sender, RoutedEventArgs e)
        {
            if (this.flagStenographie == true)
            {

                //***********
                ImageViewer.Visibility = Visibility.Visible;
                ImageStenographie1.Visibility = Visibility.Hidden;
                ImageStenographie2.Visibility = Visibility.Hidden;

                /*image.MatriceBGR = Nouvelle matrice;
                image.From_Image_To_File(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap = new BitmapImage();
                this.bitmap.BeginInit();
                this.bitmap.UriSource = new Uri(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap.EndInit();
                ImageViewer.Source = this.bitmap;
                */
            }
            Thread.Sleep(250);
            CheckBoxRotation.IsChecked = false;
        }
        #endregion

        #region QRCode
        private void CreationQRCode(object sender, RoutedEventArgs e)
        {
            string texte = TextBoxGenerateur.Text;
            int pixelParBloc = Convert.ToInt32(TextBoxGenerateur2.Text);
            QRCode q = new QRCode(texte,pixelParBloc);
            if (q.Erreur)
            {
                Erreur.Visibility = Visibility.Visible;
            }
            else
            {
                this.name = Directory.GetCurrentDirectory();
                image = q.creationQRCode();
                image.From_Image_To_File(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap = new BitmapImage();
                this.bitmap.BeginInit();
                this.bitmap.UriSource = new Uri(name + "\\temp" + compteurDeModification + ".bmp");
                this.bitmap.EndInit();
                FondBlanc.Visibility = Visibility.Visible;
                ImageQrcode.Visibility = Visibility.Visible;
                ImageQrcode.Source = this.bitmap;
                compteurDeModification++;
            }
            Thread.Sleep(250);
            checkboxGenerateur.IsChecked = false;
        }

        private void LectureQRCode(object sender, RoutedEventArgs e)
        {
            if (this.flag)
            {
                QRCode q = new QRCode(image);
                if (q.Erreur)
                {
                    Erreur.Visibility = Visibility.Visible;
                }
                else
                {
                    ExplicationLecteur2.Text = q.Message;
                }
            }
            Thread.Sleep(250);
            checkboxLecteur.IsChecked = false;
        }
        #endregion
    }
}
    


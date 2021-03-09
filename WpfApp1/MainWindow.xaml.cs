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

namespace WpfApp1
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        OpenFileDialog dlg;
        BitmapImage bitmap;
        MyImage image;
        int compteurDeModification;
        bool flag=false;
        string name;
        bool flag2 = false;
        string texte;

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
            this.name = Directory.GetCurrentDirectory();;
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
                image.From_Image_To_File(name+"\\temp"+compteurDeModification+".bmp");
                this.bitmap = new BitmapImage();
                this.bitmap.BeginInit();
                this.bitmap.UriSource = new Uri(name+"\\temp"+compteurDeModification+".bmp");
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

                image.From_Image_To_File(name+"\\temp"+compteurDeModification+".bmp");
                this.bitmap = new BitmapImage();
                this.bitmap.BeginInit();
                this.bitmap.UriSource = new Uri(name+"\\temp"+compteurDeModification+".bmp");
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
                image.From_Image_To_File(name+"\\temp"+compteurDeModification+".bmp");
                this.bitmap = new BitmapImage();
                this.bitmap.BeginInit();
                this.bitmap.UriSource = new Uri(name+"\\temp"+compteurDeModification+".bmp");
                this.bitmap.EndInit();
                ImageViewer.Source = this.bitmap;
                compteurDeModification++;
            }
        }
        #region filtre
        public void fileExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Close this window
            this.Close();
            for(int i=compteurDeModification-3;i>=0;i--)
            {
                File.Delete(name+"\\temp"+i+".bmp");
            }
        }
        private void Flou (object sender, RoutedEventArgs e)
        {
            if (flag == true)
            {
                //flou
                image = Filtre.Convolution(image, 3);
                image.From_Image_To_File(name+"\\temp"+compteurDeModification+".bmp");
                this.bitmap = new BitmapImage();
                this.bitmap.BeginInit();
                this.bitmap.UriSource = new Uri(name+"\\temp"+compteurDeModification+".bmp");
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
                image.From_Image_To_File(name+"\\temp"+compteurDeModification+".bmp");
                this.bitmap = new BitmapImage();
                this.bitmap.BeginInit();
                this.bitmap.UriSource = new Uri(name+"\\temp"+compteurDeModification+".bmp");
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
                image.From_Image_To_File(name+"\\temp"+compteurDeModification+".bmp");
                this.bitmap = new BitmapImage();
                this.bitmap.BeginInit();
                this.bitmap.UriSource = new Uri(name+"\\temp"+compteurDeModification+".bmp");
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
                image.From_Image_To_File(name+"\\temp"+compteurDeModification+".bmp");
                this.bitmap = new BitmapImage();
                this.bitmap.BeginInit();
                this.bitmap.UriSource = new Uri(name+"\\temp"+compteurDeModification+".bmp");
                this.bitmap.EndInit();
                ImageViewer.Source = this.bitmap;
                compteurDeModification++;
            }
        }
        #endregion
        #endregion
        private void ___TextBox1__TextChanged(object sender, TextChangedEventArgs e)
        {
            this.texte =___TextBox1_.Text;
            flag2 = true;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if(this.flag == true&&flag2==true)
            {
                image.From_Image_To_File(name+"\\"+texte+".bmp");
            }
        }
    }
}
    


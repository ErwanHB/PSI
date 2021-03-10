using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace WpfApp1
{
    public class Creation
    {
        public static void Fractale(MyImage image)
        {
            
        }
        public static MyImage Histogramme(MyImage image)
        {
            int[] hauteur_couleur = new int[256 * 3];

            for (int i = 0; i < image.Hauteur; i++)
            {
                for (int j = 0; j < image.Largeur; j++)
                {
                    hauteur_couleur[image.MatriceBGR[i, j].R]++;
                    hauteur_couleur[image.MatriceBGR[i, j].V + 256]++;
                    hauteur_couleur[image.MatriceBGR[i, j].B + 256 * 2]++;
                }
            }
            int max = 0;
            for(int i = 0; i < hauteur_couleur.Length;i++)
            {
                if (hauteur_couleur[i] > max) max = hauteur_couleur[i];
            }

            Pixel[,] histo = new Pixel[max,256*3];

            byte[] header = image.Header;
            //Modification du header
            byte[] taille_octet = image.Convertir_Int_To_Endian(54 + (max*256*3*3), 4);
            header[2] = taille_octet[0];
            header[3] = taille_octet[1];
            header[4] = taille_octet[2];
            header[5] = taille_octet[3];

            byte[] largeur = image.Convertir_Int_To_Endian(256*3, 4);
            header[18] = largeur[0];
            header[19] = largeur[1];
            header[20] = largeur[2];
            header[21] = largeur[3];

            byte[] hauteur = image.Convertir_Int_To_Endian(max, 4);
            header[22] = hauteur[0];
            header[23] = hauteur[1];
            header[24] = hauteur[2];
            header[25] = hauteur[3];

            byte[] taille_image = image.Convertir_Int_To_Endian((histo.Length * 3), 4); //taille de la partie contenant les pixels
            header[34] = taille_image[0];
            header[35] = taille_image[1];
            header[36] = taille_image[2];
            header[37] = taille_image[3];

            int[] couleur = { 0xFF, 0xFF, 0xFF };

            for (int i = 0; i < histo.GetLength(0); i++)
            {
                for (int j = 0; j < histo.GetLength(1); j++)
                {
                    histo[i, j] = new Pixel(couleur);
                }
            }

            for (int i = 0; i < hauteur_couleur.Length; i++)
            {
                for (int a = 0; a < hauteur_couleur[i]; a++)
                {
                    if (i < 256)
                    {
                       couleur = new int[] { i, 0, 0 };
                    }
                    else if (i < 256*2)
                    {
                        couleur = new int[] { 0, i-256, 0 };
                    }
                    else
                    {
                        couleur = new int[] { 0, 0, i-256*2 };
                    }
                    histo[a, i] = new Pixel(couleur);
                }
            }

            MyImage newImage = new MyImage(header, histo);
            return newImage;
        }
        
    }
}

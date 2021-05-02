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
        public static MyImage Fractale()
        {
            byte[] header = { 0x42, 0x4D, 0x66, 0, 0, 0, 0, 0, 0, 0, 0x36, 0, 0, 0, 0x28, 0, 0, 0, 0x04, 0, 0, 0, 0x04, 0, 0, 0, 0x01, 0, 0x18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0xC4, 0x0E, 0, 0, 0xC4, 0x0E, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            MyImage image = new MyImage(header);

            int hauteur = 1024;
            int largeur = 1280;

            #region Modification du header
            byte[] taille_octet = image.Convertir_Int_To_Endian(54 + largeur*hauteur*3, 4);
            header[2] = taille_octet[0];
            header[3] = taille_octet[1];
            header[4] = taille_octet[2];
            header[5] = taille_octet[3];

            byte[] largeur1 = image.Convertir_Int_To_Endian(largeur, 4);
            header[18] = largeur1[0];
            header[19] = largeur1[1];
            header[20] = largeur1[2];
            header[21] = largeur1[3];

            byte[] hauteur1 = image.Convertir_Int_To_Endian(hauteur, 4);
            header[22] = hauteur1[0];
            header[23] = hauteur1[1];
            header[24] = hauteur1[2];
            header[25] = hauteur1[3];

            byte[] taille_image = image.Convertir_Int_To_Endian(largeur*hauteur*3, 4); //taille de la partie contenant les pixels
            header[34] = taille_image[0];
            header[35] = taille_image[1];
            header[36] = taille_image[2];
            header[37] = taille_image[3];
            #endregion

            Complex C;
            Complex Z;

            Pixel[,] fractale = new Pixel[hauteur, largeur];
            int[] couleur = new int[3];

            double x = -2;
            double y = -1;

            double dx = 3f/1280f;
            double dy = 2f/1024f;

            int cpt = 0; //compte le nombre d'itération

            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    couleur = new int[3] { 255, 255, 255 };
                    C = new Complex(x, y);
                    Z = new Complex(0, 0);
                    
                    cpt = 0;
                    do
                    {
                        if (couleur[1] > 0)
                        {
                            couleur[1]-=15;
                        }
                        else if (couleur[2] > 0)
                        {
                            couleur[2]-=15;
                        }
                        else if (couleur[0] > 0)
                        {
                            couleur[0]-=15;
                        }
                        
                        Z = Complex.Pow(Z, 2) + C;
                        cpt++;

                    } while (Z.Magnitude <= 2 && cpt <= 50);

                    if (couleur[0] < 0)
                    {
                        couleur[0] = 0;
                    }
                    else if (couleur[1] < 0)
                    {
                        couleur[1] = 0;
                    }
                    else if (couleur[2] < 0)
                    {
                        couleur[2] = 0;
                    }

                    fractale[i, j] = new Pixel(couleur);
                    x += dx;
                }
                x = -2;
                y += dy;
            }
            image = new MyImage(header, fractale);
            return image;  
        }

        public static MyImage Histogramme(MyImage image)
        {
            int[] hauteur_couleur = new int[256 * 3];
            int hauteurMax = 5000;

            for (int i = 0; i < image.Hauteur; i++)
            {
                for (int j = 0; j < image.Largeur; j++)
                {
                    hauteur_couleur[image.MatriceBGR[i, j].R]++;            //On compte ocuurence d'une couleur
                    hauteur_couleur[image.MatriceBGR[i, j].V + 256]++;
                    hauteur_couleur[image.MatriceBGR[i, j].B + 256 * 2]++;
                }
            }

            for (int i = 0; i < hauteur_couleur.Length; i++)
            {
                hauteur_couleur[i] = (hauteur_couleur[i]* hauteurMax) / (image.Hauteur*image.Largeur);  //On passe en pourcentage la valeur pour ne pas avoir un image trop grande
            }

            hauteurMax = hauteur_couleur.Max(); //on reduit la taille de l'image à la valeur max (en pourcentage) pour avoir une image lisible
            Pixel[,] histo = new Pixel[hauteurMax,256*3];

            byte[] header = image.Header;
            //Modification du header
            byte[] taille_octet = image.Convertir_Int_To_Endian(54 + (hauteurMax * 256*3*3), 4);
            header[2] = taille_octet[0];
            header[3] = taille_octet[1];
            header[4] = taille_octet[2];
            header[5] = taille_octet[3];

            byte[] largeur = image.Convertir_Int_To_Endian(256*3, 4);
            header[18] = largeur[0];
            header[19] = largeur[1];
            header[20] = largeur[2];
            header[21] = largeur[3];

            byte[] hauteur = image.Convertir_Int_To_Endian(hauteurMax, 4);
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

            //On rempli la matrice de blanc pour avoir un joli fond
            for (int i = 0; i < histo.GetLength(0); i++)
            {
                for (int j = 0; j < histo.GetLength(1); j++)
                {
                    histo[i, j] = new Pixel(couleur);
                }
            }

            //On ajoute ensuite les barres représantant les couleurs
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

        public static MyImage EncodageStegano(MyImage basique, MyImage secret)
        {
            MyImage stegano = basique;
            char[] steg = { '5', '3', '5', '4', '4', '5', '4', '7' }; //se retranscrit en 'STEG' en ASCII
            char[] info = new char[steg.Length + secret.Taille + 54 + 8]; //représente la chaine de bytes à encoder dans basique donc de lettre en hexa

            if (basique.Taille - 54 < info.Length) //secret.Taille à pour valeur le nombre de pixel et la taille du header mais on utilise 2 valeur pour chaque valeur du header
            {                                      // donc on ajoute 54 et le + 8 pour la taille de steg et +8 pour stocker le nombre ed bits à stocker
                return stegano;
            }
            else
            {
                string taille = Convert.ToString(secret.Taille + 54, 16); //on convertit chaque valeur en hexadécimal pour récupéré facilement chaque bits
                string bourrageDe0 = new string('0', 8 - taille.Length); //On veut avoir taille sous 4 bits donc 8 lettre en hexa au total
                taille = bourrageDe0 + taille;

                int indexInfo = 0;

                for (int i = 0; i < 8; i++)
                {
                    info[indexInfo] = steg[i];
                    indexInfo++;
                }
                for (int i = 0; i < 8; i++)
                {
                    info[indexInfo] = taille[i];
                    indexInfo++;
                }
                for(int i = 0; i < 54; i++)
                {
                    string bits = Convert.ToString(secret.RawBytes[i],16);
                    string pourTaille2 = new string('0', 2 - bits.Length); //On veut avoir 2 lettre en hexa pour chaque valeur du header
                    bits = pourTaille2 + bits;

                    info[indexInfo] = bits[0];
                    indexInfo++;
                    info[indexInfo] = bits[1];
                    indexInfo++;
                }
                for (int i = 54; i < secret.Taille; i++)
                {
                    string bits = Convert.ToString(secret.RawBytes[i], 16); //on convertit chaque valeur en hexadécimal pour récupéré facilement le bit de poids fort
                    char poidsFort;                                         //Va contenir le bit de poids fort de chaque bytes
                    if(bits.Length == 1)                                    //Pour une valeur de rawBytes < 16 le bit de poids fort est 0
                    {
                        poidsFort = '0';
                    }
                    else
                    {
                        poidsFort = bits[0];
                    }
                    info[indexInfo] = poidsFort;
                    indexInfo++;
                }

                int bourrage = (4 - ((secret.MatriceBGR.GetLength(0) * 3) % 4)) % 4;
                indexInfo = 0;
                for (int i = 0; i < stegano.MatriceBGR.GetLength(0); i++)
                {
                    for (int j = 0; j < stegano.MatriceBGR.GetLength(1); j++)
                    {
                        if (indexInfo < info.Length)
                        {
                            string bitsB = Convert.ToString(stegano.MatriceBGR[i, j].B, 16);
                            if (bitsB.Length == 1)                                    //Pour une valeur de rawBytes < 16 le bit de poids fort est 0
                            {
                                bitsB = Convert.ToString(info[indexInfo]);
                            }
                            else
                            {
                                bitsB = bitsB[0] + Convert.ToString(info[indexInfo]);
                            }
                            indexInfo++;

                            stegano.MatriceBGR[i, j].B = Convert.ToByte(bitsB, 16);
                        }

                        if (indexInfo < info.Length)
                        {
                            string bitsV = Convert.ToString(stegano.MatriceBGR[i, j].V, 16);
                            if (bitsV.Length == 1)                                    //Pour une valeur de rawBytes < 16 le bit de poids fort est 0
                            {
                                bitsV = Convert.ToString(info[indexInfo]);
                            }
                            else
                            {
                                bitsV = bitsV[0] + Convert.ToString(info[indexInfo]);
                            }
                            indexInfo++;

                            stegano.MatriceBGR[i, j].V = Convert.ToByte(bitsV, 16);
                        }

                        if (indexInfo < info.Length)
                        {
                            string bitsR = Convert.ToString(stegano.MatriceBGR[i, j].R, 16);
                            if (bitsR.Length == 1)                                    //Pour une valeur de rawBytes < 16 le bit de poids fort est 0
                            {
                                bitsR = Convert.ToString(info[indexInfo]);
                            }
                            else
                            {
                                bitsR = bitsR[0] + Convert.ToString(info[indexInfo]);
                            }
                            indexInfo++;

                            stegano.MatriceBGR[i, j].R = Convert.ToByte(bitsR, 16);
                        }   
                    }
                    if (bourrage != 0)
                    {
                        indexInfo += bourrage;
                    }
                }
            }
            return stegano;
        }

        public static MyImage DecodageStegano(MyImage image)
        {
            byte[] donnees = image.RawBytes;
            string[] infos;
            char[] steg = { '5', '3', '5', '4', '4', '5', '4', '7' };
            char[] charTaille = new char[8];
            int taille;
            bool verif = true;
            MyImage decoder = image;

            for (int i = 0; i < 8; i++)
            {
                string hexadecimal = Convert.ToString(donnees[i + 54], 16);  //On transforme chaque bit en hexadecimal (base 16)
                char poidsFaible = hexadecimal[hexadecimal.Length - 1];      //on récupère le bit de poids faible donc la derniere lettre
                if(poidsFaible != steg[i])
                {
                    verif = false;
                }
            }
            if (verif)
            {
                //Si on a bien le mot STEG caché dans l'image
                for(int i = 0; i < 8; i++)
                {
                    string hexadecimal = Convert.ToString(donnees[i + 54 + 8], 16);  //On récup l'info sur la taille
                    charTaille[i] = hexadecimal[hexadecimal.Length - 1];             //
                }
                string temp = new string(charTaille);
                taille = Convert.ToInt32(temp,16);
                infos = new string[taille];
                for(int i = 0; i < taille; i++)
                {
                    string hexadecimal = Convert.ToString(donnees[i + 54 + 8 + 8], 16);  //On transforme chaque bit en hexadecimal (base 16)
                    string poidsFaible = Convert.ToString(hexadecimal[hexadecimal.Length - 1]);      //on récupère le bit de poids faible donc la derniere lettre
                    infos[i] = poidsFaible;
                }

                byte[] header = new byte[54];
                int indexInfo = 0;
                for(int i = 0; i < 54; i++)
                {
                    header[i] = Convert.ToByte(infos[indexInfo] + infos[indexInfo + 1], 16);
                    indexInfo += 2;
                }

                decoder = new MyImage(header);

                int bourrage = (4 - ((decoder.MatriceBGR.GetLength(0) * 3) % 4)) % 4;

                for (int i = 0; i < decoder.MatriceBGR.GetLength(0); i++)
                {
                    for (int j = 0; j < decoder.MatriceBGR.GetLength(1); j++)
                    {
                        int B = Convert.ToInt32(infos[indexInfo] + "0", 16);     //la valeur de chaque couleur est de la forme "bit de poids fort" "0" en hexa decimal
                        int V = Convert.ToInt32(infos[indexInfo + 1] + "0", 16);
                        int R = Convert.ToInt32(infos[indexInfo + 2] + "0", 16);
                        decoder.MatriceBGR[i, j] = new Pixel(R, V, B);
                        indexInfo += 3;
                    }
                    if (bourrage != 0)
                    {
                        indexInfo += bourrage;
                    }
                }
            }
            return decoder;
        }
    }
}

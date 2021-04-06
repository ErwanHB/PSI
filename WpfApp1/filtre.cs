using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Filtre
    {
        #region Filtre (TD4)
        public static MyImage Convolution(MyImage image, int effet)
        {
            int[,] matriceConvolution = new int[3, 3];
            switch (effet)
            {
                case 1: //detecction des contours
                    matriceConvolution = new int[,] { { -1, -1, -1 }, { -1, 8, -1 }, { -1, -1, -1 } };
                    break;
                case 2: //renforcement des bords
                    matriceConvolution = new int[,] { { 0, 0, 0 }, { -1, 1, 0 }, { 0, 0, 0 } };
                    break;
                /* 
				*****
				*****
				Essayer de faire une matrice qui fait la moyenne des adjacents
				*****
				*****
				*/
				case 3: //flou
                    matriceConvolution = new int[,] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
                    break;
                case 4: //repoussage
                    matriceConvolution = new int[,] { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 0, 0 } };
                    break;
            }

            Pixel[,] matrice = image.MatriceBGR;

            Pixel[,] nouvelleMatrice = new Pixel[matrice.GetLongLength(0), matrice.GetLongLength(1)];
            int ligne2 = -matriceConvolution.GetLength(0) / 2;
            int colonne2 = -matriceConvolution.GetLength(1) / 2;

            for (int i = 0; i < matrice.GetLength(0); i++)
            {
                for (int j = 0; j < matrice.GetLength(1); j++)
                {
                    nouvelleMatrice[i, j] = new Pixel(0, 0, 0);
                    for (int ligneConvolution = 0; ligneConvolution < matriceConvolution.GetLength(0); ligneConvolution++)
                    {
                        for (int colonneConvolution = 0; colonneConvolution < matriceConvolution.GetLength(1); colonneConvolution++)
                        {
                            int ligneMatriceInitial = i + ligne2;
                            int colonneMatriceInitial = j + colonne2;

                            if (ligneMatriceInitial >= 0 && ligneMatriceInitial < matrice.GetLength(0) && colonneMatriceInitial >= 0 && colonneMatriceInitial < matrice.GetLength(1))
                            {
                                nouvelleMatrice[i, j].B += matrice[ligneMatriceInitial, colonneMatriceInitial].B * matriceConvolution[ligneConvolution, colonneConvolution];
                                nouvelleMatrice[i, j].V += matrice[ligneMatriceInitial, colonneMatriceInitial].V * matriceConvolution[ligneConvolution, colonneConvolution];
                                nouvelleMatrice[i, j].R += matrice[ligneMatriceInitial, colonneMatriceInitial].R * matriceConvolution[ligneConvolution, colonneConvolution];

                                if (effet == 3)
                                {
                                    nouvelleMatrice[i, j].B /= 9;
                                    nouvelleMatrice[i, j].V /= 9;
                                    nouvelleMatrice[i, j].R /= 9;
                                }
                            }
                            colonne2++;
                        }
                        colonne2 = -matriceConvolution.GetLength(1) / 2;
                        ligne2++;
                    }

                    if (nouvelleMatrice[i, j].B < 0)
                    {
                        nouvelleMatrice[i, j].B = 0;
                    }
                    else if (nouvelleMatrice[i, j].B > 255)
                    {
                        nouvelleMatrice[i, j].B = 255;
                    }

                    if (nouvelleMatrice[i, j].V < 0)
                    {
                        nouvelleMatrice[i, j].V = 0;
                    }
                    else if (nouvelleMatrice[i, j].V > 255)
                    {
                        nouvelleMatrice[i, j].V = 255;
                    }

                    if (nouvelleMatrice[i, j].R < 0)
                    {
                        nouvelleMatrice[i, j].R = 0;
                    }
                    else if (nouvelleMatrice[i, j].R > 255)
                    {
                        nouvelleMatrice[i, j].R = 255;
                    }

                    ligne2 = -matriceConvolution.GetLength(0) / 2;
                    colonne2 = -matriceConvolution.GetLength(1) / 2;
                }
            }
            MyImage newImage = new MyImage(image.Header, nouvelleMatrice);
            return newImage;
        }
        #endregion
    }
}

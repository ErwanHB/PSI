using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace WpfApp1
{
    class QRCode
    {
        bool erreur = false;
        public string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:";
        string message;
        int type;       //vaut 1 ou 2 en fonction du type de QRcode utilisé
        int pixelParBloc;
        int taille;     //taille du qrcode en nombre de blocs
       
        QRBloc[] mode = QRBloc.getTableau("0010");  //Indicateur de l'alphanumérique
        QRBloc[] nombreCaractere;      //codé sur 9 bits
        QRBloc[] donnees;
        QRBloc[] bourrage;     //Contient les zéros du bourrage
        QRBloc[] EC;
        QRBloc[] masque = QRBloc.getTableau("111011111000100");  //chaine de bits representant le niveau d'EC et le type de masque

        QRBloc[] chaine;

        QRBloc[,] qrcode; //matrice representant tout le qrcode

        public string Message { 
            get { return this.message; } 
        }
        public int Type { 
            get { return this.type; } 
        }
        public bool Erreur { get { return this.erreur; } }

        /// <summary>
        /// Cree un QRcode à partir d'un message de l'utilisateur
        /// </summary>
        /// <param name="texte">message à encoder dans le QRcode</param>
        /// <param name="pixelParBloc">lors de la creation de l'image, représente le taille d'un bloc (un bit de données) en pixel</param>
        public QRCode(string texte,int pixelParBloc = 10)
        {
            this.message = texte.ToUpper();
            this.pixelParBloc = pixelParBloc;
            this.chaine = mode;

            if (this.message.Length < 47) //taille max du message
            {
                bool verif = true;
                foreach(char c in this.message) //verif que le message contient seulement les lettre de l'alphabet
                {
                    if(!alphabet.Contains(c)) verif = false;
                }
                if (verif)
                {
                    if (this.message.Length <= 25) //pour 25 et 47 cf. CDC
                    {
                        this.type = 1;
                        this.taille = 21;
                        this.EC = new QRBloc[7*8]; //EC fait 7 octet donc 7*8 bits
                    }
                    else if (this.message.Length <= 47) 
                    {
                        this.type = 2;
                        this.taille = 25;
                        this.EC = new QRBloc[10*8]; //EC fait 10 octet donc 10*8 bits
                    }
                    string taille = Convert.ToString(this.message.Length, 2); //transforme en binaire la taille
                    this.nombreCaractere = QRBloc.getTableau(taille, 9);

                    this.chaine = QRBloc.somme(this.chaine, this.nombreCaractere);

                    Encodage();    //traitement message

                    this.chaine = QRBloc.somme(this.chaine, this.EC);

                }
                else
                {
                    this.erreur = true;
                }
            }
            else
            {
                this.erreur = true;
            }
        }
        
        /// <summary>
        /// Crée un instance QRCode à partir d'une image,
        /// </summary>
        /// <param name="image">image contenant uniquement le qrcode qui sera décodé pour récupere le qrcode</param>
        public QRCode(MyImage image)
        {
            #region verif de la taille de l'image
            if (image.Hauteur != image.Largeur || image.Hauteur < 21)
            {
                this.erreur = true;
                return;
            }
            if(image.Hauteur % 21 != 0 && image.Hauteur % 25 != 0)
            {
                this.erreur = true;
                return;
            }
            else if (image.Hauteur % 21 == 0 && image.Hauteur % 25 == 0)
            {
                //Determination de la largeur d'un finder patern, en pixel
                //pour trouver le nombre de PixelParBloc
                int index = 0;
                int largeurFinderPattern = 0;
                while (image.MatriceBGR[0, index].R == 0)
                {
                    largeurFinderPattern++;
                    index++;
                }

                this.pixelParBloc = largeurFinderPattern / 7;
                if(image.Largeur/this.pixelParBloc == 21)
                {
                    this.type = 1;
                }
                else
                {
                    this.type = 2;
                }
            }
            else
            {
                if (image.Hauteur % 21 == 0)
                {
                    this.type = 1;
                }
                else if (image.Hauteur % 25 == 0)
                {
                    this.type = 2;
                }
            }
            if (this.type == 1)
            {
                this.taille = 21;
                this.pixelParBloc = image.Hauteur / 21;
                this.chaine = new QRBloc[208];
                this.nombreCaractere = new QRBloc[9];
                this.donnees = new QRBloc[152];
                this.EC = new QRBloc[7 * 8];
            }
            else if (this.type == 2)
            {
                this.taille = 25;
                this.pixelParBloc = image.Hauteur / 25;
                this.chaine = new QRBloc[359];
                this.nombreCaractere = new QRBloc[9];
                this.donnees = new QRBloc[272];
                this.EC = new QRBloc[10 * 8];
            }
            #endregion
            # region lecture des bits à partir de l'image
            this.qrcode = new QRBloc[this.taille, this.taille];
            int ligne = image.Hauteur - 1;
            int colonne = 0;
            for (int i = 0; i < this.taille; i++)
            {
                for (int j = 0; j < this.taille; j++)
                {
                    this.qrcode[i, j] = new QRBloc(image.MatriceBGR[ligne, colonne]);
                    colonne += this.pixelParBloc;
                }
                ligne -= this.pixelParBloc;
                colonne = 0;
            }
            #endregion
            #region verification qu'on a un qr code
            bool verif;
            int len = this.taille;

            //Coin haut gauche
            verif = iscarree(-1, -1, 9, 0, true);
            verif = iscarree(0, 0, 7, 1);
            verif = iscarree(1, 1, 5, 0);
            verif = iscarree(2, 2, 3, 1);
            verif = this.qrcode[3, 3].Noir;

            //coin haut droite
            verif = iscarree(-1, len - 8, 9, 0, true);
            verif = iscarree(0, len - 7, 7, 1);
            verif = iscarree(1, len - 6, 5, 0);
            verif = iscarree(2, len - 5, 3, 1);
            verif = this.qrcode[3, len - 4].Noir;

            //coin bas gauche
            verif = iscarree(len - 8, -1, 9, 0, true);
            verif = iscarree(len - 7, 0, 7, 1);
            verif = iscarree(len - 6, 1, 5, 0);
            verif = iscarree(len - 5, 2, 3, 1);
            verif = this.qrcode[len - 4, 3].Noir;

            if (!verif)
            {
                this.erreur = true;
                return;
            }
            #endregion
            decodage();
            Console.WriteLine(this.message);
        }

        /// <summary>
        /// Méthode pour encoder le message en bianire, faire le bourrage et crée les bits d'erreurs corrections
        /// Complète le tableau QRBloc[] chaine
        /// </summary>
        public void Encodage()
        {
            #region Decoupage et changement base du message
            string[] decoupe = new string[(this.message.Length+1)/2];
            int index = -1; //pour prendre le cas i=0 en compte dans le index++;

            for (int i = 0; i < this.message.Length; i++)
            {
                if (i % 2 == 0) //2 caractere par index de decoupe
                {
                    index++;
                }
                decoupe[index] += this.message[i];
            }
            
            for (int i = 0; i < decoupe.Length; i++)
            {
                int poids = 0;
                int puissance = 0;
                for (int j = decoupe[i].Length-1; j >= 0; j--) //Chaque couple de 2 lettres
                {
                    int code = alphabet.IndexOf(decoupe[i][j]);  //recup le code du caractere (entre 0 et 47)
                    poids += Convert.ToInt32(code * Math.Pow(45, puissance)); //recup le poids correspondant des 2 lettres
                    puissance++;
                }

                QRBloc[] codeBinaire;

                if (decoupe[i].Length == 1)
                {
                    codeBinaire = QRBloc.getTableau(Convert.ToString(poids, 2), 6);
                }
                else
                {
                    codeBinaire = QRBloc.getTableau(Convert.ToString(poids, 2), 11);
                }
                
                if (i == 0)
                {
                    this.donnees = codeBinaire;
                }
                else
                {
                    this.donnees = QRBloc.somme(this.donnees, codeBinaire); //concatenne les blocs du message
                }
                
            }
            #endregion

            this.chaine = QRBloc.somme(this.chaine, this.donnees);

            #region Terminaison 
            //ajoute 4 zeros max pour atteindre la limite de caractère
            int tailleTotale = this.donnees.Length + this.mode.Length + this.nombreCaractere.Length;
            int zeros = 0;
            if (this.type == 1 && tailleTotale < 152) //taille max est 152 ou 272 cf.CDC
            {
                zeros = 152 - tailleTotale;
                if (zeros > 4) zeros = 4;  
            }
            else if (this.type == 2 && tailleTotale < 272)
            {
                zeros = 272 - tailleTotale;
                if (zeros > 4) zeros = 4;
            }
            tailleTotale += zeros;
            #endregion
            #region bourrage
            //jusqu'a que la taille soit multiple de 8
            while (tailleTotale % 8 != 0)
            {
                tailleTotale++;
                zeros++;
            }

            //Creation de bourrage avec tous les zeros d'un coup
            this.bourrage = new QRBloc[zeros];
            while (zeros > 0)
            {
                this.bourrage[zeros - 1] = new QRBloc(0);
                zeros--;
            }

            //Si toujours trop court, on ajoute 236 et 17 en binaire à la fin jusqu'a taille max (cf. CDC page20)
            QRBloc[] _236 = QRBloc.getTableau("11101100");
            QRBloc[] _17 = QRBloc.getTableau("00010001");
            int alterner = 0;
            while (tailleTotale < 272 * (this.type - 1) + 152 * (this.type % 2))  //calcul pour ne pas utilisé de if
            {
                tailleTotale += 8;
                if(alterner%2 == 0)
                {
                    this.bourrage = QRBloc.somme(this.bourrage, _236);
                }
                else
                {
                    this.bourrage = QRBloc.somme(this.bourrage, _17);
                }
                alterner++;
            }
            #endregion

            this.chaine = QRBloc.somme(this.chaine, this.bourrage);

            #region ReedSolomon
            string stringChaine = QRBloc.tabToString(this.chaine);
            string[] csub = new string[stringChaine.Length / 8];
            byte[] octet = new byte[stringChaine.Length / 8];
            for (int i = 0; i < csub.Length; i++)
            {
                csub[i] = stringChaine.Substring(0 + i * 8, 8);
                octet[i] = Convert.ToByte(csub[i], 2);
            }

            int nombreEC = 0;
            if (this.type == 1)
            {
                nombreEC = 7;
            }
            else if (this.type == 2)
            {
                nombreEC = 10;
            }
            byte[] result = ReedSolomon.ReedSolomonAlgorithm.Encode(octet, nombreEC, ReedSolomon.ErrorCorrectionCodeType.QRCode);

            for (int i = 0; i < result.Length; i++)
            {
                string binaire = Convert.ToString(result[i], 2);
                QRBloc[] chaine = QRBloc.getTableau(binaire, 8);
                if (i == 0)
                {
                    this.EC = chaine;
                }
                else
                {
                    this.EC = QRBloc.somme(this.EC, chaine);
                } 
            }
            #endregion
        }

        /// <summary>
        /// Crée une matrice de QRBloc repésentant le QR Code au complet
        /// ATTENTION ne prend pas en compte le format BMP
        /// crée la matrice avec le 0,0 en haut à gauche
        /// </summary>
        public MyImage creationQRCode()
        {
            this.qrcode = new QRBloc[this.taille, this.taille];

            #region Finder patterns dans les coins
            int len = this.taille;

            //Coin haut gauche
            carree(-1, -1, 9, 0,true);
            carree(0, 0, 7, 1);
            carree(1, 1, 5, 0);
            carree(2, 2, 3, 1);
            this.qrcode[3, 3] = new QRBloc(1);

            //coin haut droite
            carree(-1, len-8, 9, 0, true);
            carree(0, len-7, 7, 1);
            carree(1, len-6, 5, 0);
            carree(2, len-5, 3, 1);
            this.qrcode[3, len-4] = new QRBloc(1);

            //coin bas gauche
            carree(len-8, -1, 9, 0, true);
            carree(len-7, 0, 7, 1);
            carree(len-6, 1, 5, 0);
            carree(len-5, 2, 3, 1);
            this.qrcode[len-4, 3] = new QRBloc(1);

            #endregion

            //allignement pattern
            if (this.type == 2)
            {
                //alignement parten tjr à la meme place pour le type 2
                carree(16, 16, 5, 1);
                carree(17, 17, 3, 0);
                this.qrcode[18, 18] = new QRBloc(1);
            }

            //timing patterns
            int l1 = 8;
            int c1 = 6;
            while(this.qrcode[l1,c1] == null)
            {
                this.qrcode[l1, c1] = new QRBloc((l1+1) % 2);
                this.qrcode[c1, l1] = new QRBloc((l1+1) % 2);
                l1++;
            }

            //Dark module
            this.qrcode[this.taille-8, 8] = new QRBloc(1);

            #region ajout du masque
            //les 2 lignes à coté des paterns bas gauche et haut droit
            for (int i = 0; i < 7; i++){
                this.qrcode[this.taille - 1 - i, 8] = this.masque[i];
            }
            for (int i = 0; i < 8; i++)
            {
                this.qrcode[8, this.taille - 8 + i] = this.masque[i + 7];
            }
            //le coin haut gauche
            for (int i = 0; i < 6; i++)
            {
                this.qrcode[8, i] = this.masque[i];
                this.qrcode[5-i, 8] = this.masque[i + 9];
            }
            //3 bit dans le coin
            this.qrcode[7, 8] = this.masque[6];
            this.qrcode[8, 8] = this.masque[7];
            this.qrcode[8, 7] = this.masque[8];

            #endregion

            #region ajout des données
            int y = this.taille-1;
            int x = this.taille-1;
            int index = 0;
            int sens = -1; //vaut -1 ou 1 pour indiquer le sens d'écriture (haut vers bas ou inverse)
            while (index < this.chaine.Length+7*(this.type-1))
            {
                for (int i = 0; i < 2; i++)
                {
                    if (this.qrcode[y,x-i] == null)
                    {
                        //aplication du masque 0, qui correspond à une matrice cadrillé
                        //on applique un XOR entre la valeur de la matrice masque et notre matrice
                        if (index >= this.chaine.Length) //Une qrcode de version à 359 case de données disponibles
                                                         // Mais on a 352 bits de données à rentrer donc on rempli la fin de zéros (ca s'appelle redondance)
                                                         // auquel on applique le masque
                        {
                            bool xor = false ^ ((y + x - i) % 2 == 0);
                            this.qrcode[y, x - i] = new QRBloc(xor);
                        }
                        else
                        {
                            bool xor = this.chaine[index].Noir ^ ((y + x - i) % 2 == 0);
                            this.qrcode[y, x - i] = new QRBloc(xor);
                        }
                        index++;
                    }
                }
                
                y+=sens;
                if(y < 0)
                {
                    x -= 2;
                    y = 0;
                    sens = 1;
                }
                if (y >= this.taille)
                {
                    x -= 2;
                    y = this.taille-1;
                    sens = -1;
                }
                //Pour eviter le timing pattern vertical, on saute 1 plus loin
                if (x == 6)
                {
                    x--;
                }
                
                
            }
            #endregion

            #region creation de la matrice RGB et du header
            byte[] header = { 0x42, 0x4D, 0x66, 0, 0, 0, 0, 0, 0, 0, 0x36, 0, 0, 0, 0x28, 0, 0, 0, 0x04, 0, 0, 0, 0x04, 0, 0, 0, 0x01, 0, 0x18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0xC4, 0x0E, 0, 0, 0xC4, 0x0E, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            MyImage image = new MyImage(header);
            int tailleImage = this.taille * pixelParBloc;
                #region Modification du header
                byte[] taille_octet = image.Convertir_Int_To_Endian(54 + tailleImage * tailleImage * 3, 4);
                header[2] = taille_octet[0];
                header[3] = taille_octet[1];
                header[4] = taille_octet[2];
                header[5] = taille_octet[3];

                byte[] largeur1 = image.Convertir_Int_To_Endian(tailleImage, 4);
                header[18] = largeur1[0];
                header[19] = largeur1[1];
                header[20] = largeur1[2];
                header[21] = largeur1[3];

                byte[] hauteur1 = image.Convertir_Int_To_Endian(tailleImage, 4);
                header[22] = hauteur1[0];
                header[23] = hauteur1[1];
                header[24] = hauteur1[2];
                header[25] = hauteur1[3];

                byte[] taille_image = image.Convertir_Int_To_Endian(tailleImage * tailleImage * 3, 4); //taille de la partie contenant les pixels
                header[34] = taille_image[0];
                header[35] = taille_image[1];
                header[36] = taille_image[2];
                header[37] = taille_image[3];
                #endregion

            
            Pixel[,] matriceBGR = new Pixel[tailleImage, tailleImage];

            int ligne = tailleImage - 1;
            int colonne = 0;
            for (int i = 0; i < this.taille; i++)
            {
                for (int j = 0; j < this.taille; j++)
                {
                    //Pour chaque bloc, crée x pixel
                    //x etant le nombre de pixel par bloc
                    for (int a = 0; a < this.pixelParBloc; a++)
                    {
                        for (int b = 0; b < this.pixelParBloc; b++)
                        {
                            matriceBGR[ligne - b, colonne + a] = this.qrcode[i,j].P;
                        }
                    }
                    colonne += this.pixelParBloc;
                }
                ligne -= this.pixelParBloc;
                colonne = 0;
            }
            #endregion

            image = new MyImage(header,matriceBGR);
            return image;
        }

        /// <summary>
        /// Cree un carré vide, utilisé que dans creationQRCode
        /// </summary>
        /// <param name="c0">coordonnées du coin haut gauche du carrée, colonne initiale 0</param>
        /// <param name="l0">coordonnées du coin haut gauche du carrée, ligne initialle 0</param>
        /// <param name="taille">taille en QRBloc du carrée</param>
        /// <param name="valeur">valeur du bloc, 1 = noir; 0 = blanc </param>
        /// <param name="bande">pour faire les separateur blancs autour des coins </param>
        /// <param name="remove">utiliser seulement dans decodage et utiliser pour supprimer les coins</param>
        public void carree(int l0, int c0, int taille, int valeur, bool bande = false, bool remove = false)
        {
            if(taille == 1)
            {
                if (remove) this.qrcode[l0, c0] = null;
                else this.qrcode[l0, c0] = new QRBloc(valeur);
            }
            else
            {
                for (int ligne = 0; ligne < taille; ligne++)
                {
                    for (int colonne = 0; colonne < taille; colonne++)
                    {
                        if(ligne == 0 || colonne == 0 || ligne == taille-1 || colonne == taille-1)
                        {
                            if (bande)
                            {
                                try
                                {
                                    if (remove) this.qrcode[l0 + ligne, c0 + colonne] = null;
                                    else this.qrcode[l0 + ligne, c0 + colonne] = new QRBloc(valeur);
                                }
                                catch (IndexOutOfRangeException e)
                                {
                                    Console.WriteLine("Erreur par design", e);
                                }
                            }
                            else
                            {
                                if (remove) this.qrcode[l0 + ligne, c0 + colonne] = null;
                                else this.qrcode[l0 + ligne, c0 + colonne] = new QRBloc(valeur);
                            }
                        }
                    }
                }
            }
        }

        public void decodage()
        {
            #region Suppression des finder parterns
            int len = this.taille;

            //Coin haut gauche
            carree(-1, -1, 9, 0, true,remove:true);
            carree(0, 0, 7, 1,remove:true);
            carree(1, 1, 5, 0,remove: true);
            carree(2, 2, 3, 1,remove: true);
            this.qrcode[3, 3] = null;

            //coin haut droite
            carree(-1, len - 8, 9, 0, true, remove: true);
            carree(0, len - 7, 7, 1, remove: true);
            carree(1, len - 6, 5, 0, remove: true);
            carree(2, len - 5, 3, 1, remove: true);
            this.qrcode[3, len - 4] = null;

            //coin bas gauche
            carree(len - 8, -1, 9, 0, true, remove: true);
            carree(len - 7, 0, 7, 1, remove: true);
            carree(len - 6, 1, 5, 0, remove: true);
            carree(len - 5, 2, 3, 1, remove: true);
            this.qrcode[len - 4, 3] = null;

            //Pour le allignement Pattern
            if (this.type == 2)
            {
                carree(16, 16, 5, 1, remove: true);
                carree(17, 17, 3, 0, remove: true);
                this.qrcode[18, 18] = null;
            }
            #endregion
            #region Suppression info masque et EC
            //les 2 lignes à coté des paterns bas gauche et haut droit
            for (int i = 0; i < 7; i++)
            {
                this.qrcode[this.taille - 1 - i, 8] = null;
            }
            for (int i = 0; i < 8; i++)
            {
                this.qrcode[8, this.taille - 8 + i] = null;
            }
            //le coin haut gauche
            for (int i = 0; i < 6; i++)
            {
                this.qrcode[8, i] = null;
                this.qrcode[5 - i, 8] = null;
            }
            //3 bit dans le coin
            this.qrcode[7, 8] = null;
            this.qrcode[8, 8] = null;
            this.qrcode[8, 7] = null;
            #endregion
            #region Suppresion des motifs de synchro et dark module
            int l1 = 8;
            int c1 = 6;
            while (this.qrcode[l1, c1] != null)
            {
                this.qrcode[l1, c1] = null;
                this.qrcode[c1, l1] = null;
                l1++;
            }

            //Dark module
            this.qrcode[this.taille - 8, 8] = null;
            #endregion
            #region Lecture et demasquage des données
            int y = this.taille - 1;
            int x = this.taille - 1;
            int index = 0;
            int sens = -1; //vaut -1 ou 1 pour indiquer le sens d'écriture (haut vers bas ou inverse)
            while (x>0)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (this.qrcode[y, x - i] != null)
                    {
                        //Pour demasquer un masque 0,
                        //on reaplique un XOR entre la valeur de la matrice masque et notre matrice
                        bool xor =  this.qrcode[y, x - i].Noir ^ ((y + x - i) % 2 == 0);
                        this.chaine[index] = new QRBloc(xor);
                        index++;
                    }
                }

                y += sens;
                if (y < 0)
                {
                    x -= 2;
                    y = 0;
                    sens = 1;
                }
                if (y >= this.taille)
                {
                    x -= 2;
                    y = this.taille - 1;
                    sens = -1;
                }
                //Pour eviter le timing pattern vertical, on saute 1 plus loin
                if (x == 6)
                {
                    x--;
                }
            }
            #endregion
            #region Decoupage de la chaine
            int cpt = 0;
            int indexEC = 0;
            while (cpt < this.chaine.Length)
            {
                if(cpt >= 4 && cpt < 13)
                {
                    this.nombreCaractere[cpt - 4] = this.chaine[cpt];
                }
                if (cpt < 272 * (this.type - 1) + 152 * (this.type % 2)) //On lit toute la chaine de données car besoin pour reedsolomon
                {
                    this.donnees[cpt] = this.chaine[cpt];
                }
                else if (cpt < 272 * (this.type - 1) + 152 * (this.type % 2) + this.EC.Length)
                {
                    this.EC[indexEC] = this.chaine[cpt];
                    indexEC++;
                }
                else
                {
                    //On ne fait rien, 
                    //ce cas arrive seulement dans le cas QRcode type 2 car bourrage à la fin
                }
                cpt++;
            }
            #endregion
            #region Verification ReedSolomon
            //on transforme en string binaire les donnees
            string stringDonnee = QRBloc.tabToString(this.donnees);
            stringDonnee = stringDonnee.Substring(0, stringDonnee.Length); //on ne prend pas les 7 bits de redondance si de type 2
            string[] csub = new string[stringDonnee.Length / 8];
            byte[] byteDonnees = new byte[stringDonnee.Length / 8];
            //puis le string en tableau de byte
            for (int i = 0; i < csub.Length; i++)
            {
                csub[i] = stringDonnee.Substring(0 + i * 8, 8);
                byteDonnees[i] = Convert.ToByte(csub[i], 2);
            }

            //de meme pour l'erreur correction
            string stringEC = QRBloc.tabToString(this.EC);
            string[] sub = new string[stringEC.Length / 8];
            byte[] byteEC = new byte[stringEC.Length / 8];
            for (int i = 0; i < sub.Length; i++)
            {
                sub[i] = stringEC.Substring(0 + i * 8, 8);
                byteEC[i] = Convert.ToByte(sub[i], 2);
            }

            byteDonnees = ReedSolomon.ReedSolomonAlgorithm.Decode(byteDonnees, byteEC, ReedSolomon.ErrorCorrectionCodeType.QRCode);
            if (byteDonnees == null)
            {
                this.erreur = true;
                return;    //On sort de la fonction si ReedSolomon retournz null (cad qu'il n'a pas réussi à décoder)
            }
            #endregion
            #region nettoyage de la chaine ReedSolomon
            //Decodage du nombre de caractere du message
            int nombreDeCaractere = Convert.ToInt32(QRBloc.tabToString(this.nombreCaractere), 2);

            //Extraction de données correspondant au message de ReedSolomon
            //On remet la chaine de byte de ReedSolomon sous un seule string
            string sDonnees = "";
            string bits;
            for (int i = 0; i < byteDonnees.Length; i++)
            {
                bits = Convert.ToString(byteDonnees[i], 2);  //Convert.ToString ne donne pas forcement un octet,
                if (bits.Length < 8)                         //Donc on ajoute des zéros à gauche
                {
                    string zeros = new string('0', 8 - bits.Length);
                    bits = zeros + bits;
                }
                sDonnees += bits;
            }

            int longueur = 0; //nombre de bits dans lequel sont encodé le message
            longueur = ((nombreDeCaractere + 1 * (nombreDeCaractere%2)) * 11) / 2;

            sDonnees = sDonnees.Substring(13, longueur);  //on récupere toutes les donnees sans prendre en compte le mode et le nombre de caractere
            #endregion
            #region Decodage des donnes en message
            //Decoupage en string de 11 bits
            string[] s = new string[sDonnees.Length / 11]; ;
            for (int i = 0; i < s.Length; i++)
            {
                if(nombreDeCaractere%2 == 1 && i == s.Length - 1) //si on  un nombre impaire de caractere, la derniere chaine de bits fait seulement 6 bits
                {
                    s[i] = sDonnees.Substring(0 + i * 11, 6);
                }
                else
                {
                    s[i] = sDonnees.Substring(0 + i * 11, 11);
                }
            }

            char[] message = new char[nombreDeCaractere];
            int indexMessage = 0;
            for(int i = 0; i < s.Length; i++)
            {
                int poids = Convert.ToInt32(s[i], 2);
                if(poids < 45)
                {
                    message[indexMessage] = alphabet[poids];
                    indexMessage++;
                }
                else
                {
                    int lettre2 = poids % 45;  //index de l'alphabet de la 2eme lettre du mot de 2 lettre
                    poids -= lettre2;
                    int lettre1 = poids / 45; //index de la 1er lettre
                    message[indexMessage] = alphabet[lettre1];
                    indexMessage++;
                    message[indexMessage] = alphabet[lettre2];
                    indexMessage++;
                }
            }
            #endregion
            this.message = new string(message); //message final recrée à partir du tableau de caractère
        }

        /// <summary>
        /// verifie si il y a une carré de blocs à l'emplacement précisé
        /// </summary>
        /// Pour les paramètres, cf la fonction carree
        /// <returns></returns>
        public bool iscarree(int l0, int c0, int taille, int valeur, bool bande = false)
        {
            int verif = 0;
            if (taille == 1)
            {
                if(this.qrcode[l0, c0].Valeur == valeur)
                {
                    verif ++;
                }
            }
            else
            {
                for (int ligne = 0; ligne < taille; ligne++)
                {
                    for (int colonne = 0; colonne < taille; colonne++)
                    {
                        if(ligne == 0 || colonne == 0)
                        {
                            if (bande)
                            {
                                try
                                {
                                    bool test = (this.qrcode[l0, c0].Valeur == valeur);
                                    if (test)
                                    {
                                        verif++;
                                    }
                                }
                                catch (IndexOutOfRangeException e)
                                {
                                    Console.WriteLine("Erreur par design", e);
                                }
                                catch (NullReferenceException e)
                                {
                                    Console.WriteLine("Erreur par design", e);
                                }
                            }
                            else
                            {
                                if (this.qrcode[l0, c0].Valeur == valeur)
                                {
                                    verif++;
                                }
                            }
                        }
                        
                    }
                }
            }
            bool resultat;
            if(!bande && verif == taille*4 - 4)
            {
                resultat = true;
            }
            else if(bande && verif == taille*2 -1)
            {
                resultat = true;
            }
            else{
                resultat = false;
            }
            return resultat;
        }

        public void afficheMatrice()
        {
            for (int a = 0; a < this.taille; a++)
            {
                for (int b = 0; b < this.taille; b++)
                {
                    if(this.qrcode[a,b] == null)
                    {
                        Console.Write(". ");
                    }
                    else
                    {
                        Console.Write(this.qrcode[a, b].ToString() + " ");
                    } 
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}

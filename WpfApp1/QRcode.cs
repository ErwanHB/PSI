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
        int taille; //taille du qrcode en nombre de blocs
       
        QRBloc[] mode = QRBloc.getTableau("0010");  //Indicateur de l'alphanumérique
        QRBloc[] nombreCaractere;      //codé sur 9 bits
        QRBloc[] donnees;
        QRBloc[] bourrage;     //Contient les zéros du bourrage
        QRBloc[] EC;
        QRBloc[] masque = QRBloc.getTableau("111011111000100");  //chaine de bits representant le niveau d'EC et le type de masque

        QRBloc[] chaine;

        QRBloc[,] qrcode; //matrice representant tout le qrcode

        string Message { 
            get { return this.message; } 
        }
        int Type { 
            get { return this.type; } 
        }

        public QRCode(string texte,int pixelParBloc = 2)
        {
            this.pixelParBloc = pixelParBloc;
            this.message = "HELLO WORLD"; //message = texte.ToUpper;
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
                        this.taille = 24;
                        this.EC = new QRBloc[10*8]; //EC fait 10 octet donc 10*8 bits
                    }
                    string taille = Convert.ToString(this.message.Length, 2); //transforme en binaire la taille
                    this.nombreCaractere = QRBloc.getTableau(taille, 9);

                    this.chaine = QRBloc.somme(this.chaine, this.nombreCaractere);

                    Encodage();    //traitement message

                    this.chaine = QRBloc.somme(this.chaine, this.EC);

                    creationQRCode();
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
        public void creationQRCode()
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
                this.qrcode[18, 18] = new QRBloc(0);
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
            this.qrcode[(4 * this.type) + 9, 8] = new QRBloc(1);

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
            while (index < this.chaine.Length)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (this.qrcode[y,x-i] == null)
                    {
                        //aplication du masque 0, qui correspond à une matrice cadrillé
                        //on applique un XOR entre la valeur de la matrice masque et notre matrice
                        bool xor = this.chaine[index].Noir ^ ((y + x - i)%2 == 0);
                        this.qrcode[y, x - i] = new QRBloc(xor);

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
            image.From_Image_To_File("qrcode.bmp");
        }

        /// <summary>
        /// Cree un carré vide, utilisé que dans creationQRCode
        /// </summary>
        /// <param name="c0">coordonnées du coin haut gauche du carrée, colonne initiale 0</param>
        /// <param name="l0">coordonnées du coin haut gauche du carrée, ligne initialle 0</param>
        /// <param name="taille">taille en QRBloc du carrée</param>
        /// <param name="valeur">valeur du bloc, 1 = noir; 0 = blanc </param>
        /// <param name="bande">pour faire les separateur blancs autour des coins </param>
        public void carree(int l0, int c0, int taille, int valeur, bool bande = false)
        {
            if(taille == 1)
            {
                this.qrcode[l0, c0] = new QRBloc(valeur);
            }
            else
            {
                for (int ligne = 0; ligne < taille; ligne++)
                {
                    for (int colonne = 0; colonne < taille; colonne++)
                    {
                        if (bande)
                        {
                            try
                            {
                                this.qrcode[l0 + ligne, c0 + colonne] = new QRBloc(valeur);
                            }
                            catch(IndexOutOfRangeException e){
                                Console.WriteLine("Erreur par design", e);
                            }
                        }
                        else
                        {
                            this.qrcode[l0 + ligne, c0 + colonne] = new QRBloc(valeur);
                        }
                    }
                }
            }
        }

    }
}

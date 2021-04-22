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
        public string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:";
        string message;
        int type;       //vaut 1 ou 2 en fonction du type de QRcode utilisé
       
        QRBloc[] mode = QRBloc.getTableau("0010");  //Indicateur de l'alphanumérique
        QRBloc[] nombreCaractere;      //codé sur 9 bits
        QRBloc[] donnees;
        QRBloc[] bourrage;     //Contient les zéros du bourrage
        QRBloc[] EC;
        QRBloc[] masque = QRBloc.getTableau("111011111000100");  //chaine de bits representant le niveau d'EC et le type de masque

        QRBloc[] chaine;

        string Message { 
            get { return this.message; } 
        }
        int Type { 
            get { return this.type; } 
        }

        public QRCode(string texte)
        {
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
                        this.EC = new QRBloc[7*8]; //EC fait 7 octet donc 7*8 bits
                    }
                    else if (this.message.Length <= 47) 
                    {
                        this.type = 2;
                        this.EC = new QRBloc[10*8]; //EC fait 10 octet donc 10*8 bits
                    }
                    string taille = Convert.ToString(this.message.Length, 2); //transforme en binaire la taille
                    this.nombreCaractere = QRBloc.getTableau(taille, 9);

                    this.chaine = QRBloc.somme(this.chaine, this.nombreCaractere);

                    Encodage();    //traitement message
                }
                
            }
        }

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
            tailleTotale += this.EC.Length;
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
            Encoding u8 = Encoding.UTF8;
            byte[] bytesa = u8.GetBytes(this.message);
            int nombreEC = 0;
            if (this.type == 1)
            {
                nombreEC = 7;
            }
            else if (this.type == 2)
            {
                nombreEC = 10;
            }
            byte[] result = ReedSolomon.ReedSolomonAlgorithm.Encode(bytesa, nombreEC, ReedSolomon.ErrorCorrectionCodeType.QRCode);

            for (int i = 0; i < result.Length; i++)
            {
                Console.WriteLine(result[i]);

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

            this.chaine = QRBloc.somme(this.chaine, this.EC);
        }
    }
}

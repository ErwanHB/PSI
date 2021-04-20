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
        public string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVXYZ $%*+-./:";
        string message;
        int type;       //vaut 1 ou 2 en fonction du type de QRcode utilisé
       
        QRBloc[] mode = QRBloc.getTableau("0010");  //Indicateur de l'alphanumérique
        QRBloc[] nombreCaractere;      //codé sur 9 bits
        QRBloc[] donnees;
        QRBloc[] correctionErreur;
        QRBloc[] masque = QRBloc.getTableau("111011111000100");  //chaine de bits définit dans les cahiers des charges

        string Message { 
            get { return this.message; } 
        }
        int Type { 
            get { return this.type; } 
        }

        public QRCode(string texte)
        {
            this.message = "HELLO WORLD"; //message = texte.ToUpper;
            if (this.message.Length < 47)
            {
                if (this.message.Length <= 25)
                {
                    this.type = 1;
                }
                else if (this.message.Length <= 47)
                {
                    this.type = 2;
                }
                string taille = Convert.ToString(this.message.Length, 2);
                this.nombreCaractere = QRBloc.getTableau(taille,9);
                Encodage();
            }
        }

        public void Encodage()
        {
            string[] decoupe = new string[(this.message.Length+1)/2];
            int index = -1; //pour prendre le cas i=0 en compte

            for (int i = 0; i < this.message.Length; i++)
            {
                if (i % 2 == 0) //découpe en string de 2 caracteres
                {
                    index++;
                }
                decoupe[index] += this.message[i];
            }
            
            for (int i = 0; i < decoupe.Length; i++)
            {
                int poids = 0;
                for (int j = 0; j < 2; j++)
                {
                    int code = alphabet.IndexOf(decoupe[i][j])+1;
                    poids += Convert.ToInt32(code * Math.Pow(45, 1 - i));
                }
                QRBloc[] chaine = QRBloc.getTableau(Convert.ToString(poids,2),11);
                if (i == 0)
                {
                    this.donnees = chaine;
                }
                this.donnees = QRBloc.somme(this.donnees, chaine);
            }

            //ReedSolomon
            Encoding u8 = Encoding.UTF8;
            byte[] bytesa = u8.GetBytes(this.message);
            byte[] result = ReedSolomonAlgorithm.Encode(bytesa, 7, ErrorCorrectionCodeType.QRCode);
        }
    }
}

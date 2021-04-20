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
        string[] alphabet = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "X", "Y", "Z", " ", "$", "%", "*", "+", "-", ".", "/", ":" };
        string message;
        int type;       //vaut 1 ou 2 en fonction du type de QRcode utilisé
       
        QRBloc[] mode = QRBloc.getTableau("0010");  //Indicateur de l'alphanumérique
        QRBloc[] nombreCaractere;      //codé sur 9 bits
        QRBloc[] données;
        QRBloc[] correctionErreur;
        QRBloc[] masque = QRBloc.getTableau("111011111000100");  //chaine de bits définit dans les cahiers des charges

        string Message
        {
            get { return this.message; }
        }

        public QRCode(string texte)
        {
            string sa = "0123456789ABCDEFGHIJKLMNOPQRSTUVXYZ $% *+-./:";
            sa.ToUpper();
            Console.WriteLine(sa);

            message = "Hello world"; //message = texte;
            if (message.Length < 47)
            {
                if (message.Length <= 25)
                {
                    type = 1;
                }
                else if (message.Length <= 47)
                {
                    type = 2;
                }
                string s = Convert.ToString(message.Length, 2);
                nombreCaractere = QRBloc.getTableau(s);
            }
        }

        public void Encodage()
        {
           
        }
    }
}

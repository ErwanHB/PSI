using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class QRBloc
    {
        bool noir; //false = blanc, true = noir
        Pixel p;
        
        bool Noir { get { return this.noir; } }
        Pixel P { get { return this.p; } }

        public QRBloc(char c)
        {
            if(c == '1') 
            {
                noir = true;
                p = new Pixel(0, 0, 0);
            }
            else if(c == '0')
            {
                noir = false;
                p = new Pixel(255, 255, 255);
            }
        }
        public QRBloc(int a)
        {
            if (a == 1)
            {
                noir = true;
                p = new Pixel(0, 0, 0);
            }
            else if (a == 0)
            {
                noir = false;
                p = new Pixel(255, 255, 255);
            }
        }
        public QRBloc(bool b)
        {
            noir = b;
            if (noir) p = new Pixel(0, 0, 0);
            else p = new Pixel(255, 255, 255);
        }

        public static QRBloc[] getTableau(string s, int length = 0)
        {
            if(length == 0)
            {
                length = s.Length;
            }
            QRBloc[] tab = new QRBloc[length];

            if (s.Length != length) //si le string à rempllir est plus petit que la taille du mot voulu, on complète de 0 à gauche
            {
                for (int i = 0; i < length - s.Length; i++)
                {
                    tab[i] = new QRBloc(false);
                }
                for (int i = 0; i < s.Length; i++)
                {
                    tab[i+length-s.Length] = new QRBloc(s[i]);
                }
            }
            else
            {
                for (int i = 0; i < length; i++) 
                {
                    tab[i] = new QRBloc(s[i]);
                }
            }
            return tab;
        }

        public static QRBloc[] somme(QRBloc[] a, QRBloc[] b) //concatenne a et b
        {
            QRBloc[] somme = new QRBloc[a.Length + b.Length];
            for(int i = 0; i < a.Length; i++)
            {
                somme[i] = a[i];
            }
            for (int i = a.Length; i < b.Length; i++)
            {
                somme[i] = b[i - a.Length];
            }
            return somme;
        }
        public static QRBloc[] somme(QRBloc a, QRBloc[] b) //concatenne a et b
        {
            QRBloc[] somme = new QRBloc[b.Length+1];
            somme[0] = a;
            for (int i = 1; i < b.Length; i++)
            {
                somme[i] = b[i-1];
            }
            return somme;
        }
        public static QRBloc[] somme(QRBloc[] a, QRBloc b) //concatenne a et b
        {
            QRBloc[] somme = new QRBloc[a.Length + 1];
            somme[a.Length] = b;
            for (int i = 0; i < a.Length; i++)
            {
                somme[i] = a[i];
            }
            return somme;
        }
    }
}

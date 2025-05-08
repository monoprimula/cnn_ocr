using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ocr
{
    class Noron
    {
        // Her bir noron'nun değerini tutar
        public double value;

        // Her bir noron'nun ağırlığını tutar
        public double[] agirliklar;

        // Noronun ağırlık ve value'nun çarpımı sonucunu tutar
        public double cikisValue;

        // Geri hesaplamada kullanılmak için vardır
        public double cikisValueOncesi;

        // Beklenen ile çıkan sonuç arasındaki fark
        public double hata;

        // Ağırlığın ne kadar arttırılacağını veya azaltılacağını tutar
        public double dagitilacakHata;

        // Bias değerini tutar
        public double bias;
    }
}

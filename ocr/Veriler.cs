using System;

namespace ocr
{
    // Bu sınıf, eğitim veri setini temsil etmektedir
    public class Veriler
    {
        // Eğitim giriş verileri
        public int[][] veriler;

        // Beklenen çıkış verileri
        public int[][] beklenenler;

        // Sınıf örneklendiğinde verileri hazırla
        public Veriler()
        {
            HazirlaVeriler();
        }

        private void HazirlaVeriler()
        {
            // Harflerin 7x5 matris temsilleri
            int[,,] egitim = new int[5, 7, 5] {
                {/* A */
                  {0,0,1,0,0},
                  {0,1,0,1,0},
                  {1,0,0,0,1},
                  {1,0,0,0,1},
                  {1,1,1,1,1},
                  {1,0,0,0,1},
                  {1,0,0,0,1} },

                {/* B */ 
                  {1,1,1,1,0},
                  {1,0,0,0,1},
                  {1,0,0,0,1},
                  {1,1,1,1,0},
                  {1,0,0,0,1},
                  {1,0,0,0,1},
                  {1,1,1,1,0} },

                { /* C */ 
                  {0,0,1,1,1},
                  {0,1,0,0,0},
                  {1,0,0,0,0},
                  {1,0,0,0,0},
                  {1,0,0,0,0},
                  {0,1,0,0,0},
                  {0,0,1,1,1} },

                { /* D */ 
                  {1,1,1,0,0},
                  {1,0,0,1,0},
                  {1,0,0,0,1},
                  {1,0,0,0,1},
                  {1,0,0,0,1},
                  {1,0,0,1,0},
                  {1,1,1,0,0} },

                { /* E */ 
                  {1,1,1,1,1},
                  {1,0,0,0,0},
                  {1,0,0,0,0},
                  {1,1,1,1,1},
                  {1,0,0,0,0},
                  {1,0,0,0,0},
                  {1,1,1,1,1} }
            };

            // Her harf için beklenen çıkış değerleri (one-hot encoding)
            int[,] istenenCikti = new int[5, 5] {
                {1,0,0,0,0}, // A
                {0,1,0,0,0}, // B
                {0,0,1,0,0}, // C
                {0,0,0,1,0}, // D
                {0,0,0,0,1}  // E
            };

            // Verileri düz dizi formatına dönüştürür
            veriler = new int[5][];
            beklenenler = new int[5][];

            for (int i = 0; i < 5; i++)
            {
                // Her harf için 35 boyutlu giriş vektörü
                veriler[i] = new int[35];
                int index = 0;

                for (int satir = 0; satir < 7; satir++)
                {
                    for (int sutun = 0; sutun < 5; sutun++)
                    {
                        veriler[i][index++] = egitim[i, satir, sutun];
                    }
                }

                // Her harf için 5 boyutlu hedef vektörü
                beklenenler[i] = new int[5];
                for (int j = 0; j < 5; j++)
                {
                    beklenenler[i][j] = istenenCikti[i, j];
                }
            }
        }
    }
}
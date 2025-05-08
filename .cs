using System;

namespace ocr
{
    // Bu sınıf, yapay sinir ağının eğitim ve tahmin işlemlerini içerir.
    public partial class NeuralNetwork
    {
        // Eğitim işlemi: Girdi ve hedef verileriyle ağı eğitir, belirlenen hata eşiğine ulaşana kadar devam eder
        // Momentum kullanarak gradient descent optimizasyonu ile ağırlıkları günceller
        public bool Training(int[][] inputs, int[][] targets, double errorThreshold = 0.01)
        {
            int epoch = 10000;

            // Momentum terimlerini sıfırla
            for (int i = 0; i < inputSize; i++)
                for (int j = 0; j < hiddenSize; j++)
                    deltaWeightsInputHidden[i, j] = 0;
            for (int i = 0; i < hiddenSize; i++)
                for (int j = 0; j < outputSize; j++)
                    deltaWeightsHiddenOutput[i, j] = 0;

            double finalError = 1.0;

            for (int e = 0; e < epoch; e++)
            {
                double totalError = 0;

                for (int k = 0; k < inputs.Length; k++)
                {
                    ileriHesaplama(inputs[k]);
                    totalError += hataHesaplama(targets[k]);
                    geriHesaplama(inputs[k], targets[k]);
                }

                finalError = totalError / inputs.Length;
                hataOrani = finalError;

                if (e % 1000 == 0)
                    Console.WriteLine($"Epoch: {e}, Total Error: {finalError}");

                if (finalError < errorThreshold)
                    return true;
            }

            return false;
        }

        // Tahmin işlemi: Bir giriş verisi ile ağın tahmini çıktısını hesaplar
        // Eğer beklenen çıktı verildiyse, tahmin hatası da hesaplanır
        public void tahmin(int[] input, int[] expectedOutput = null)
        {
            ileriHesaplama(input);
            for (int k = 0; k < outputSize; k++)
                sonuc[k] = outputLayer[k];

            if (expectedOutput != null)
            {
                hataOrani = hataHesaplama(expectedOutput);
            }
        }

        // En yüksek değere sahip çıkış nöronunun indeksini döndürür
        // OCR uygulamasında bu, tanınan harfin indeksidir (0: A, 1: B, 2: C, 3: D, 4: E)
        public int GetPredictedLetter()
        {
            int maxIndex = 0;
            for (int i = 1; i < outputSize; i++)
                if (sonuc[i] > sonuc[maxIndex])
                    maxIndex = i;
            return maxIndex; // 0: A, 1: B, 2: C, 3: D, 4: E
        }
    }
}
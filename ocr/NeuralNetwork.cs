/*
 * Bu dosya basit bir yapay sinir ağı uygulaması içermektedir. 
 * Kod, tek gizli katmanlı bir sinir ağı modeli oluşturur ve eğitir.
 * OCR (Optik Karakter Tanıma) için tasarlanmış olup, A-E harflerini tanımak üzere kullanılabilir.
 */

using System;

namespace ocr
{
    // Bu sınıf, tek gizli katmanlı bir yapay sinir ağı modelini temsil eder.
    class NeuralNetwork
    {
        // Ağın yapısı ve parametreleri
        int inputSize, hiddenSize, outputSize;
        double[,] weightsInputHidden;
        double[,] weightsHiddenOutput;
        double[] hiddenBias;
        double[] outputBias;

        // Katmanlar
        double[] hiddenLayer;
        double[] outputLayer;

        // Öğrenme oranı ve momentum
        double learningRate = 0.1;
        double momentum = 0.9;

        // Momentum terimleri (önceki ağırlık değişiklikleri)
        double[,] deltaWeightsInputHidden;
        double[,] deltaWeightsHiddenOutput;

        public double[] sonuc;     // Son tahmin çıktısı
        public double hataOrani;  // Ortalama hata değeri

        // Yapıcı metot: Ağırlıkları rastgele başlatır
        // Varsayılan olarak 35 girişli (5x7 piksel karakter), 15 gizli nöronlu ve 5 çıkışlı (A-E harfleri) bir ağ oluşturur
        public NeuralNetwork(int inputSize = 35, int hiddenSize = 15, int outputSize = 5)
        {
            this.inputSize = inputSize;
            this.hiddenSize = hiddenSize;
            this.outputSize = outputSize;

            weightsInputHidden = new double[inputSize, hiddenSize];
            weightsHiddenOutput = new double[hiddenSize, outputSize];

            hiddenBias = new double[hiddenSize];
            outputBias = new double[outputSize];

            hiddenLayer = new double[hiddenSize];
            outputLayer = new double[outputSize];

            deltaWeightsInputHidden = new double[inputSize, hiddenSize];
            deltaWeightsHiddenOutput = new double[hiddenSize, outputSize];

            sonuc = new double[outputSize];

            Random rnd = new Random();
            for (int i = 0; i < inputSize; i++)
                for (int j = 0; j < hiddenSize; j++)
                    weightsInputHidden[i, j] = rnd.NextDouble() - 0.5;

            for (int i = 0; i < hiddenSize; i++)
                for (int j = 0; j < outputSize; j++)
                    weightsHiddenOutput[i, j] = rnd.NextDouble() - 0.5;

            for (int i = 0; i < hiddenSize; i++)
                hiddenBias[i] = rnd.NextDouble() - 0.5;

            for (int i = 0; i < outputSize; i++)
                outputBias[i] = rnd.NextDouble() - 0.5;

            hataOrani = 1.0;
        }

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

        // İleri hesaplama: Girdi verisini ağ üzerinden geçirerek çıktı üretir
        // Sigmoid aktivasyon fonksiyonu kullanarak her nöronun çıktısını hesaplar
        public void ileriHesaplama(int[] input)
        {
            for (int j = 0; j < hiddenSize; j++)
            {
                double sum = 0;
                for (int i = 0; i < inputSize; i++)
                    sum += input[i] * weightsInputHidden[i, j];
                hiddenLayer[j] = Sigmoid(sum + hiddenBias[j]);
            }

            for (int k = 0; k < outputSize; k++)
            {
                double sum = 0;
                for (int j = 0; j < hiddenSize; j++)
                    sum += hiddenLayer[j] * weightsHiddenOutput[j, k];
                outputLayer[k] = Sigmoid(sum + outputBias[k]);
            }
        }

        // Hata hesaplama: Kare hata fonksiyonuyla hedef ve tahmin arasındaki farkı hesaplar
        // Toplam kare hatayı döndürür, eğitim sırasında minimize edilmeye çalışılır
        public double hataHesaplama(int[] target)
        {
            double sum = 0;
            for (int k = 0; k < outputSize; k++)
                sum += 0.5 * Math.Pow(target[k] - outputLayer[k], 2);
            return sum;
        }

        // Geri yayılım: Çıktıdaki hatayı geriye doğru yayarak ağırlıkları günceller
        // Gradyan iniş ve momentum tekniklerini kullanarak öğrenmeyi hızlandırır
        public void geriHesaplama(int[] input, int[] target)
        {
            double[] outputErrors = new double[outputSize];
            double[] hiddenErrors = new double[hiddenSize];

            for (int k = 0; k < outputSize; k++)
            {
                double error = target[k] - outputLayer[k];
                outputErrors[k] = error * outputLayer[k] * (1 - outputLayer[k]);
            }

            for (int j = 0; j < hiddenSize; j++)
            {
                double error = 0;
                for (int k = 0; k < outputSize; k++)
                    error += outputErrors[k] * weightsHiddenOutput[j, k];
                hiddenErrors[j] = error * hiddenLayer[j] * (1 - hiddenLayer[j]);
            }

            for (int j = 0; j < hiddenSize; j++)
            {
                for (int k = 0; k < outputSize; k++)
                {
                    double delta = learningRate * outputErrors[k] * hiddenLayer[j] + momentum * deltaWeightsHiddenOutput[j, k];
                    weightsHiddenOutput[j, k] += delta;
                    deltaWeightsHiddenOutput[j, k] = delta;
                }
            }

            for (int i = 0; i < inputSize; i++)
            {
                for (int j = 0; j < hiddenSize; j++)
                {
                    double delta = learningRate * hiddenErrors[j] * input[i] + momentum * deltaWeightsInputHidden[i, j];
                    weightsInputHidden[i, j] += delta;
                    deltaWeightsInputHidden[i, j] = delta;
                }
            }

            for (int k = 0; k < outputSize; k++)
                outputBias[k] += learningRate * outputErrors[k];

            for (int j = 0; j < hiddenSize; j++)
                hiddenBias[j] += learningRate * hiddenErrors[j];
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

        // Sigmoid aktivasyon fonksiyonu: Nöron çıktılarını [0,1] aralığına sıkıştırır
        // Doğrusal olmayan bu fonksiyon, ağın karmaşık ilişkileri öğrenmesini sağlar
        public static double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }
    }
}
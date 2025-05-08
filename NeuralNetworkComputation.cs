using System;

namespace ocr
{
    // Bu sınıf, yapay sinir ağının hesaplama işlemlerini içerir.
    public partial class NeuralNetwork
    {
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

        // Sigmoid aktivasyon fonksiyonu: Nöron çıktılarını [0,1] aralığına sıkıştırır
        // Doğrusal olmayan bu fonksiyon, ağın karmaşık ilişkileri öğrenmesini sağlar
        public static double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }
    }
}
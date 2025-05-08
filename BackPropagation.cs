using System;

namespace ocr
{
    // NeuralNetwork sınıfının geri yayılım ve eğitim kısmını içeren parçası
    partial class NeuralNetwork
    {
        // Eğitim metodu
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

                // Ortalama hata hesapla
                finalError = totalError / inputs.Length;
                hataOrani = finalError; // Hata oranını güncelle

                if (e % 1000 == 0)
                    Console.WriteLine($"Epoch: {e}, Total Error: {finalError}");

                if (finalError < errorThreshold)
                    return true;
            }

            return false; // Eğitim tamamlanmadıysa false dön
        }

        // Geri yayılım algoritması
        public void geriHesaplama(int[] input, int[] target)
        {
            double[] outputErrors = new double[outputSize];
            double[] hiddenErrors = new double[hiddenSize];

            // Çıktı katmanı hata hesaplaması
            for (int k = 0; k < outputSize; k++)
            {
                double error = target[k] - outputLayer[k];
                outputErrors[k] = error * outputLayer[k] * (1 - outputLayer[k]);
            }

            // Gizli katman hata hesaplaması
            for (int j = 0; j < hiddenSize; j++)
            {
                double error = 0;
                for (int k = 0; k < outputSize; k++)
                    error += outputErrors[k] * weightsHiddenOutput[j, k];
                hiddenErrors[j] = error * hiddenLayer[j] * (1 - hiddenLayer[j]);
            }

            // Çıktı katmanı ile gizli katman arasındaki ağırlıkları güncelle
            for (int j = 0; j < hiddenSize; j++)
            {
                for (int k = 0; k < outputSize; k++)
                {
                    double delta = learningRate * outputErrors[k] * hiddenLayer[j] + momentum * deltaWeightsHiddenOutput[j, k];
                    weightsHiddenOutput[j, k] += delta;
                    deltaWeightsHiddenOutput[j, k] = delta;
                }
            }

            // Giriş katmanı ile gizli katman arasındaki ağırlıkları güncelle
            for (int i = 0; i < inputSize; i++)
            {
                for (int j = 0; j < hiddenSize; j++)
                {
                    double delta = learningRate * hiddenErrors[j] * input[i] + momentum * deltaWeightsInputHidden[i, j];
                    weightsInputHidden[i, j] += delta;
                    deltaWeightsInputHidden[i, j] = delta;
                }
            }

            // Bias değerlerini güncelle
            for (int k = 0; k < outputSize; k++)
                outputBias[k] += learningRate * outputErrors[k];

            for (int j = 0; j < hiddenSize; j++)
                hiddenBias[j] += learningRate * hiddenErrors[j];
        }
    }
}
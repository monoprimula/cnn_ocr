using System;

namespace ocr
{
    public class Layer
    {
        public int InputSize, OutputSize;
        public double[,] Weights;
        public double[] Biases;
        public double[] Outputs;

        private static Random rnd = new Random();

        public Layer(int inputSize, int outputSize)
        {
            InputSize = inputSize;
            OutputSize = outputSize;

            Weights = new double[inputSize, outputSize];
            Biases = new double[outputSize];
            Outputs = new double[outputSize];

            for (int i = 0; i < inputSize; i++)
                for (int j = 0; j < outputSize; j++)
                    Weights[i, j] = rnd.NextDouble() - 0.5;

            for (int j = 0; j < outputSize; j++)
                Biases[j] = rnd.NextDouble() - 0.5;
        }

        public double[] Forward(double[] input)
        {
            for (int j = 0; j < OutputSize; j++)
            {
                double sum = 0;
                for (int i = 0; i < InputSize; i++)
                    sum += input[i] * Weights[i, j];
                Outputs[j] = Sigmoid(sum + Biases[j]);
            }
            return Outputs;
        }

        public static double Sigmoid(double x) => 1.0 / (1.0 + Math.Exp(-x));
    }

    public static class Activation
    {
        public static double Sigmoid(double x) => 1.0 / (1.0 + Math.Exp(-x));
        public static double SigmoidDerivative(double x) => x * (1 - x);
    }

    public class DeltaHelper
    {
        public double[,] InputHidden;
        public double[,] HiddenOutput;

        public DeltaHelper(int inputSize, int hiddenSize, int outputSize)
        {
            InputHidden = new double[inputSize, hiddenSize];
            HiddenOutput = new double[hiddenSize, outputSize];
        }

        public void Reset()
        {
            Array.Clear(InputHidden, 0, InputHidden.Length);
            Array.Clear(HiddenOutput, 0, HiddenOutput.Length);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;

namespace neural_network
{
    public class DatasetWrapper
    {
        private List<byte> datasetSizes = new List<byte>();
        private List<float[,]> dataset = new List<float[,]>();
        private float[] bias;

        // constructor for parsing file
        // parses csv file with the following specifications:
        // first line shows data for the neural network type (ex. 6, 3, 4 is a 6-3-4 neural network; 6, 3, 4, 2 is a 6-(3-4)-2 neural network)
        // following lines represent a column of a matrix of weights (ex. 6x3 matrix)
        // following lines represent a column of a matrix of weights for the next layer of neurons (ex. 3x4)
        // and so forth
        public DatasetWrapper(string weightMatrixFile, string biasMatrixFile)
        {
            using (var reader = new StreamReader(weightMatrixFile))
            {
                foreach (string el in reader.ReadLine().Split(','))
                    datasetSizes.Add(byte.Parse(el));
                for (int i = 0; i < datasetSizes.Count - 1; i++)
                {
                    dataset.Add(new float[datasetSizes[i], datasetSizes[i + 1]]);
                    for (byte row = 0; row < datasetSizes[i]; row++)
                    {
                        if (!reader.EndOfStream)
                        {
                            string[] line = reader.ReadLine().Split(',');
                            for (byte col = 0; col < datasetSizes[i + 1]; col++)
                            {
                                dataset[i][row, col] = float.Parse(line[col]);
                            }
                        }
                    }
                }
            }
            using (var reader = new StreamReader(biasMatrixFile))
            {
                int biasCount = 0;
                for (int i = 1; i < datasetSizes.Count - 1; i++)
                {
                    biasCount += datasetSizes[i];
                }
                try
                {
                    string[] line = reader.ReadLine().Split(',');
                    bias = new float[line.Length];
                    if (!reader.EndOfStream || line.Length != biasCount) throw new IOException("Bias Matrix file does not match Weight Matrix file");
                    byte row = 0;
                    foreach (string el in line)
                    {
                        bias[row] = float.Parse(el);
                        row++;
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Message);
                    Environment.Exit(1);
                }
            }
        }

        // constructor
        public DatasetWrapper(List<float[,]> weightMatrixes, float[] biasMatrix)
        {
            datasetSizes.Add((byte)weightMatrixes[0].GetLength(0));
            foreach (float[,] weightMatrix in weightMatrixes)
            {
                datasetSizes.Add((byte)(weightMatrixes[0].GetLength(1)));
            }
            dataset = weightMatrixes;
            bias = biasMatrix;
        }

        // finds the dot product of two vectors
        private static float Dot(float[] v1, float[] v2)
        {
            float result = 0;
            for (int i = 0; i < v1.Length; i++)
            {
                result += v1[i] * v2[i];
            }
            return result;
        }

        // gets the value of a node
        private float Get(float[] layer, float[] weight, float bias)
        {
            float value = Dot(layer, weight) + bias;
            return value;
        }

        // gets output from the function when input is provided
        public float[] Get(float[] input)
        {
            try
            {
                if (input.Length != datasetSizes[0]) throw new IndexOutOfRangeException("Input matrix is not correct dimension");
                float[] layer = input;
                float[] newLayer;
                for (int n = 1; n < datasetSizes.Count; n++)
                {
                    newLayer = new float[datasetSizes[n]];
                    for (int i = 0; i < datasetSizes[n]; i++)
                    {
                        float[] weight = new float[datasetSizes[n - 1]];
                        for (int j = 0; j < datasetSizes[n - 1]; j++)
                        {
                            weight[j] = dataset[n - 1][j, i];
                        }
                        newLayer[i] = Get(layer, weight, bias[i]);
                    }
                    layer = newLayer;
                }
                return layer;
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
                return null;
            }
        }

        // adds two vectors
        private static float[] Add(float[] v, float[] delta)
        {
            float[] result = new float[v.Length];
            for (int i = 0; i < v.Length; i++)
            {
                result[i] = v[i] + delta[i];
            }
            return result;
        }

        // adds two matrices
        private static float[,] Add(float[,] m, float[,] delta)
        {
            float[,] result = new float[m.GetLength(0), m.GetLength(1)];
            for (int i = 0; i < m.GetLength(0); i++)
            {
                for (int j = 0; j < m.GetLength(1); j++)
                {
                    result[i, j] = m[i, j] + delta[i, j];
                }
            }
            return result;
        }

        // mutate dataset by adding matrix/vector with randomized values
        public void Mutate(float range)
        {
            var rand = new Random();
            float[,] delta;

            for (int n = 0; n < dataset.Count; n++)
            {
                float[,] weight = dataset[n];
                delta = new float[weight.GetLength(0), weight.GetLength(1)];
                for (int i = 0; i < weight.GetLength(0); i++)
                {
                    for (int j = 0; j < weight.GetLength(1); j++)
                    {
                        delta[i, j] = range * ((float)rand.NextDouble() - 0.5f);
                    }
                }
                dataset[n] = DatasetWrapper.Add(weight, delta);
            }

            float[] deltaVector = new float[bias.Length];
            for (int i = 0; i < deltaVector.Length; i++)
            {
                deltaVector[i] = range * ((float)rand.NextDouble() - 0.5f);
            }
            bias = DatasetWrapper.Add(bias, deltaVector);
        }

        // returns weight as a string
        private String WeightToString()
        {
            string line = "";
            for (int n = 0; n < dataset.Count; n++)
            {
                for (int i = 0; i < dataset[n].GetLength(0); i++)
                {
                    for (int j = 0; j < dataset[n].GetLength(1); j++)
                    {
                        line += dataset[n][i, j].ToString() + ",";
                    }
                    line = line.Substring(0, line.Length - 1) + '\n';
                }
            }
            return line;
        }

        // returns bias as a string
        private String BiasToString()
        {
            string line = "";
            for (int i = 0; i < bias.Length; i++)
            {
                line += bias[i].ToString() + ",";
            }
            return line.Substring(0, line.Length - 1);
        }

        // returns dataset as a string
        override public String ToString()
        {
            string line = "weight matrix(es):\n";
            line += WeightToString();
            line += "bias matrix:\n";
            line += BiasToString();
            return line;
        }

        // creates header for the weight file
        private String GenerateWeightHeader()
        {
            string weightHeader = "";
            for (int i = 0; i < datasetSizes.Count; i++)
            {
                weightHeader += datasetSizes[i].ToString() + ",";
            }
            return weightHeader.Substring(0, weightHeader.Length - 1) + "\n";
        }

        // returns dataset as a file, overwriting a given path
        public void ToFile(string weightMatrixPath, string biasMatrixPath)
        {
            File.WriteAllText(weightMatrixPath, GenerateWeightHeader() + WeightToString());
            File.WriteAllText(biasMatrixPath, BiasToString());
        }
    }
}

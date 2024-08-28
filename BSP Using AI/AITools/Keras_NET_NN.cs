using Biological_Signal_Processing_Using_AI.Garage;
using Numpy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace Biological_Signal_Processing_Using_AI.AITools
{
    public class Keras_NET_NN
    {
        public static KerasNETNeuralNetworkModel fit(KerasNETNeuralNetworkModel model, List<Sample> dataList, bool saveModel)
        {
            if (model._pcaActive)
                dataList = GeneralTools.rearrangeFeaturesInput(dataList, model.PCA);

            if (dataList.Count > 0)
            {
                // Sort features as inputs (x) and outputs (y)
                double[] features = dataList[0].getFeatures();
                double[] outputs = dataList[0].getOutputs();
                double[,] x = new double[dataList.Count, features.Length];
                double[,] y = new double[dataList.Count, outputs.Length];
                for (int j = 0; j < dataList.Count; j++)
                {
                    features = dataList[j].getFeatures();
                    outputs = dataList[j].getOutputs();
                    for (int k = 0; k < features.Length; k++)
                        x[j, k] = features[k];
                    for (int k = 0; k < outputs.Length; k++)
                        y[j, k] = outputs[k];
                }
                // Now fit data in the model 50 times, each for 10 epochs
                model.Model.Fit(np.array(x), np.array(y), epochs: 1000, verbose: 0);
                // Save model
                if (saveModel)
                    model.Model.Save(model.ModelPath);
            }

            return model;
        }

        public static double[] predict(double[] features, KerasNETNeuralNetworkModel model, bool fromTempModel)
        {
            // Initialize input
            if (model._pcaActive)
                features = GeneralTools.rearrangeInput(features, model.PCA);
            double[,] x = new double[1, features.Length];
            for (int i = 0; i < features.Length; i++)
                x[0, i] = features[i];
            // Predict with the selected model
            NDarray y = model.Model.Predict(x, verbose: 0);
            float[] floatOutput = y.GetData<float>();
            double[] output = new double[floatOutput.Length];
            for (int i = 0; i < output.Length; i++)
                output[i] = floatOutput[i];
            // Return result to main user interface
            return output;
        }
    }
}

using Biological_Signal_Processing_Using_AI.Garage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace Biological_Signal_Processing_Using_AI.AITools
{
    public class KNN
    {
        public static KNNModel fit(KNNModel model, List<Sample> dataList)
        {
            if (model._pcaActive)
                dataList = GeneralTools.rearrangeFeaturesInput(dataList, model.PCA);
            // Set the new optimal k for this model
            if (dataList.Count > 0)
                model.k = getOptimalK(dataList.Select((x, y) => new { Value = x, Index = y })
                                                        .GroupBy(x => x.Index / 300)
                                                        .Select(x => x.Select(v => v.Value).ToList())
                                                        .ToList()[0], model);
            else
                model.k = 3;
            // Set the new features in the model
            foreach (Sample feature in dataList)
                model.DataList.Add(feature);

            return model;
        }

        public static double[] predict(double[] features, KNNModel kNNModel)
        {
            // Initialize input
            if (kNNModel._pcaActive)
                features = GeneralTools.rearrangeInput(features, kNNModel.PCA);
            // Create list for calculating distances between input and saved dataset
            List<distanteOutput> distances = new List<distanteOutput>();
            // Iterate through all saved features and calucalte distance between the input and the saved feature
            double[] savedFeatures;
            double distance;
            foreach (Sample samp in kNNModel.DataList)
            {
                distance = 0;
                savedFeatures = samp.getFeatures();
                for (int i = 0; i < features.Length; i++)
                    distance += Math.Pow(features[i] - savedFeatures[i], 2);
                distance = Math.Sqrt(distance);
                // Insert distance and its output in distances
                distances.Add(new distanteOutput { distance = distance, output = samp.getOutputs() });
            }
            // Sort distances in distances
            distances.Sort((e1, e2) => { return e1.distance.CompareTo(e2.distance); });

            // Calculate the average of the first "k" outputs
            double[] output = null;
            if (distances.Count > 0)
                output = new double[distances[0].output.Length];
            int k = kNNModel.k < distances.Count ? kNNModel.k : distances.Count;
            for (int i = 0; i < k; i++)
                for (int j = 0; j < output.Length; j++)
                    output[j] += distances[i].output[j] / k;

            // Return result to main user interface
            return output;
        }

        private static int getOptimalK(List<Sample> data, KNNModel model)
        {
            // Iterate through all possible k values
            List<kError> kErrors = new List<kError>();
            for (int k = 1; k <= 41 && k <= data.Count; k += 2)
            {
                // Separate features to 4 quarters
                // and take 3 quarters as KNNModel features
                // and the fourth quarter as validation data
                int parts = 1;
                int partFeaturesNmbr = 1;
                for (int i = 4; i > 0; i--)
                    if (data.Count / i > 0)
                    {
                        parts = i;
                        break;
                    }
                partFeaturesNmbr = data.Count / parts;
                double modelError = 0;
                double predError;
                for (int i = 0; i < parts; i++)
                {
                    // Create KNNModel and validation list
                    KNNModel kNNModel = (KNNModel)model.Clone();
                    List<Sample> validationList = new List<Sample>();
                    if (parts == 1)
                        foreach (Sample sample in data)
                        {
                            kNNModel.DataList.Add(sample);
                            validationList.Add(sample);
                        }
                    else
                        for (int j = 0; j < data.Count; j++)
                        {
                            // Check if this feature is for validation
                            if (i * partFeaturesNmbr <= j && j < (i + 1) * partFeaturesNmbr || i == parts - 1 && i * partFeaturesNmbr <= j)
                                // This is for validation
                                validationList.Add(data[j]);
                            else
                                // This is for KNNModel
                                kNNModel.DataList.Add(data[j]);
                        }
                    // Calculate error for this parition
                    double[] predictedOutput;
                    double[] actualOutput;
                    foreach (Sample sample in validationList)
                    {
                        // Calculate prediction error
                        predError = 0;
                        predictedOutput = predict(sample.getFeatures(), kNNModel);
                        actualOutput = sample.getOutputs();
                        for (int j = 0; j < predictedOutput.Length; j++)
                            predError += Math.Pow(predictedOutput[j] - actualOutput[j], 2);
                        predError = Math.Sqrt(predError);
                        // Add this error in modelError
                        modelError += predError / data.Count;
                    }
                }

                // Add current k error in kErrors
                kErrors.Add(new kError { error = modelError, k = k });
            }
            // Sort errors in kErrors
            kErrors.Sort((e1, e2) => { return e1.error.CompareTo(e2.error); });
            // Return the least error k
            return kErrors[0].k;
        }
    }
}

using Biological_Signal_Processing_Using_AI.Garage;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tensorflow;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.AITools.AIBackThreadReportHolder;
using static Tensorflow.Binding;

namespace Biological_Signal_Processing_Using_AI.AITools
{
    public class TF_NET_NN
    {
        public delegate Session ModelArchitectureDelegate(int inputDim, int outputDim);
        public static CustomArchiBaseModel fit(CustomArchiBaseModel model, TFNETBaseModel baseModel, List<Sample> dataList, FittingProgAIReportDelegate fittingProgAIReportDelegate, bool saveModel = false, int suggestedBatchSize = 4)
        {
            if (model._pcaActive)
                dataList = GeneralTools.rearrangeFeaturesInput(dataList, model.PCA);

            if (dataList.Count > 0)
            {
                // Get the session from the model
                Session session = baseModel.Session;

                // Sort data into batches
                int batchesCount = (dataList.Count / suggestedBatchSize) + (dataList.Count % suggestedBatchSize > 0 ? 1 : 0);
                List<float[,]> inputBatches = new List<float[,]>(batchesCount);
                List<float[,]> outputBatches = new List<float[,]>(batchesCount);

                int featuresCount = dataList[0].getFeatures().Length;
                int outputsCount = dataList[0].getOutputs().Length;
                for (int i = 0; i < dataList.Count; i += suggestedBatchSize)
                {
                    // Compute the right batch size
                    int batchSize = i + suggestedBatchSize <= dataList.Count ? suggestedBatchSize : dataList.Count - i;
                    // Create the new batches
                    float[,] inputBatch = new float[batchSize, featuresCount];
                    float[,] outputBatch = new float[batchSize, outputsCount];
                    for (int k = i; k - i < batchSize; k++)
                    {
                        // Get k sample inouts and outputs
                        double[] features = dataList[k].getFeatures();
                        double[] outputs = dataList[k].getOutputs();
                        // Copy the inputs to inputBatch
                        for (int a = 0; a < featuresCount; a++)
                            inputBatch[k - i, a] = (float)features[a];
                        // Copy the outputs to outputBatch
                        for (int a = 0; a < outputsCount; a++)
                            outputBatch[k - i, a] = (float)outputs[a];
                    }

                    // Insert the newly created batches to their lists
                    inputBatches.Add(inputBatch);
                    outputBatches.Add(outputBatch);
                }

                // Start training
                float improvementThreshold = 0.0001f;
                // Get the necessary nodes for training from the model graph
                Tensor input = session.graph.OperationByName("input_place_holder");
                Tensor output = session.graph.OperationByName("output");
                Tensor outputPlaceholder = session.graph.OperationByName("output_place_holder");

                Tensor costFunc = session.graph.OperationByName("cost_function");
                Tensor learningRate = session.graph.OperationByName("learning_rate");
                float lRate = 0.1f;
                Operation optimizer = session.graph.OperationByName("optimizer");

                // Save the initiale values of the variables for restoring the graph if the learning isn't going well
                // Save them as assignment operations to the
                session.graph.as_default();
                IVariableV1[] vars = tf.global_variables();
                Operation[] initOperations = new Operation[vars.Length];
                for (int i = 0; i < vars.Length; i++)
                    initOperations[i] = tf.assign(vars[i], session.run(vars[i].AsTensor()));

                // Create an error queue for storing the mean training error of each epoch
                CircularQueue<float> costCirQueue = new CircularQueue<float>(15);
                float meanCost = 0;

                // Start training
                // Max 1000 epochs
                int epochsMax = 1000;
                for (int epoch = 0; epoch < epochsMax; epoch++)
                {
                    // Iterate through the batches
                    for (int batch = 0; batch < inputBatches.Count; batch++)
                    {
                        // Train the model with the selected batch
                        (_, float cost) = session.run((optimizer, costFunc),
                                                    (learningRate, lRate),
                                                    (input, new Tensorflow.NumPy.NDArray(inputBatches[batch])),
                                                    (outputPlaceholder, new Tensorflow.NumPy.NDArray(outputBatches[batch])));

                        // Check if the cost went to infinity
                        if (float.IsNaN(cost) || float.IsInfinity(cost))
                        {
                            // If yes then decrease the learning rate
                            lRate /= 10f;
                            // Restore the initial values of the variables (weights, and biases)
                            session.run(initOperations);
                        }

                        // Update the last and the mean cost value
                        meanCost += cost / inputBatches.Count;
                    }

                    // Update fitProgressBar
                    if (fittingProgAIReportDelegate != null)
                        fittingProgAIReportDelegate(epoch, epochsMax);

                    // Check if the learning is not improving according to improvementThreshold
                    // in the last 15 epochs
                    costCirQueue.Enqueue(meanCost);
                    meanCost = 0;

                    if (costCirQueue._count == 15)
                    {
                        (float mean, float min, float max) = GeneralTools.MeanMinMax(costCirQueue.ToArray());
                        if ((max - min) < improvementThreshold)
                            // If yes then there is no greate improvement. We can stop learning here
                            break;
                    }
                }

                // Update fitProgressBar
                if (fittingProgAIReportDelegate != null)
                    fittingProgAIReportDelegate(epochsMax, epochsMax);

                // Save model
                if (saveModel)
                    SaveModelVariables(session, baseModel.ModelPath, new string[] { "output" });
            }

            return model;
        }

        public static double[] predict(double[] features, CustomArchiBaseModel model, Session session)
        {
            // Initialize input
            if (model._pcaActive)
                features = GeneralTools.rearrangeInput(features, model.PCA);

            session.graph.as_default();

            // Get the input and output variables from the graph
            Tensor input = session.graph.OperationByName("input_place_holder");
            Tensor output = session.graph.OperationByName("output");

            // Arrange the features in a float[,]
            float[,] featuresFloat = new float[1, features.Length];
            for (int i = 0; i < features.Length; i++) featuresFloat[0, i] = (float)features[i];
            // Predict the input
            Tensorflow.NumPy.NDArray prediction = session.run(output, new FeedItem(input, featuresFloat));

            // Return result to main user interface
            return prediction.ToMultiDimArray<float>().OfType<float>().Select(val => (double)val).ToArray();
        }

        public static void SaveModelWithGraph(Session session, string path)
        {
            Saver saver = tf.train.Saver();
            saver.save(session, path + "_with_graph");
        }

        public static void LoadModelWithGraph(Session session, string path)
        {
            Saver saver = tf.train.Saver();
            saver.restore(session, path + "_with_graph");
        }

        public static void SaveModelVariables(Session session, string path, string[] outputsNames)
        {
            var output_graph_def = tf.graph_util.convert_variables_to_constants(
                session, session.graph.as_graph_def(), outputsNames);
            System.IO.Directory.CreateDirectory(path);
            File.WriteAllBytes(path + "/model_variables.pb", output_graph_def.ToByteArray());
            //File.WriteAllText(path + "labels.txt", string.Join("\n", new string[] { "3output" }));
        }

        public static Session LoadModelVariables(string path, string inpTensorName, string outpTensorName, ModelArchitectureDelegate modelArchitectureDelegate)
        {
            // Load the learned graph variables values
            Graph valsGraph = tf.train.load_graph(path + "/model_variables.pb");
            // and create a temporal session for reading the variables tensors values
            Session tempSess = tf.Session();
            // Get the input and output tensors dimensions
            int inputDim = (int)valsGraph.get_tensor_by_name(inpTensorName).dims[1];
            int outputDim = (int)valsGraph.get_tensor_by_name(outpTensorName).dims[1];

            // Activate restore mode to enable both eager mode (run operations immediately without the need of a graph)
            // and graph mode (stores the new nodes to the default graph)
            tf.Context.restore_mode();

            // Create a new session for the model
            Session session = modelArchitectureDelegate(inputDim, outputDim);

            // Set the session graph as the default graph
            session.graph.as_default();
            // Get the global variables from the default graph
            IVariableV1[] newVars = tf.global_variables();

            // Initiate the new graph with learned graph variables values
            for (int i = 0; i < newVars.Length; i++)
            {
                IVariableV1 var = newVars[i];
                Tensor valsTensor = valsGraph.get_tensor_by_name(var.Name);
                Tensorflow.NumPy.NDArray vals = tempSess.run(valsTensor);
                Tensor operation = tf.assign(var, vals);
                session.run(operation);
            }

            return session;
        }

        public static Tensor Layer(Tensor input, int outputDim, List<Operation> assignmentsList = null, string name = null)
        {
            // Get the shape of the input
            int inputDim = (int)input.shape[1];

            // Create the weights and biases parameters
            ResourceVariable weightsVar = tf.Variable(tf.ones((inputDim, outputDim)));
            ResourceVariable biasesVar = tf.Variable(tf.zeros(outputDim));
            // Initialize the weights to be 1 / inputDim so that one neuron from the input doesn't saturate the output on its own
            // Initializing variables with the same values. Otherwise, all the weights will have the same value after training.
            Operation weightsAssign = tf.assign(weightsVar, tf.random.normal((inputDim, outputDim), 0, 1f / (float)inputDim));
            Operation biasesAssign = tf.assign(biasesVar, tf.random.normal(outputDim, 0, 1f / (float)inputDim));

            // Perform the Times operation with the weights and biases
            Tensor output = tf.add(tf.matmul(input, weightsVar), biasesVar, name);

            // Insert the assignemnts to assignmentsList if its not null
            assignmentsList?.Add(weightsAssign);
            assignmentsList?.Add(biasesAssign);

            return output;
        }

        public static Tensor HardSigmoid(Tensor input, string name = null)
        {
            // The hard sigmoid equation equals:
            // 0.2 * input + 0.5 ----- if ----> -2.5 <= input <= 2.5
            // 0                 ----- if ----> input < -2.5
            // 1                 ----- if ----> input > -2.5

            // Start with the first equation
            Tensor lineartHS = tf.add(tf.multiply(input, 0.2f), 0.5f);

            // Now set the conditions
            Tensor lowConditions = tf.less(input, tf.constant(-2.5f));
            Tensor highConditions = tf.greater(input, tf.constant(2.5f));

            // Perform the conditions to lineartHS
            Tensor lowCompHS = tf.where(lowConditions, tf.zeros_like(lineartHS), lineartHS);
            Tensor hardSigmoid = tf.where(highConditions, tf.ones_like(lowCompHS), lowCompHS, name);

            return hardSigmoid;
        }
    }
}

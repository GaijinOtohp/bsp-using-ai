using Biological_Signal_Processing_Using_AI.Garage;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tensorflow;
using Tensorflow.NumPy;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels;
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.AITools.AIBackThreadReportHolder;
using static Tensorflow.Binding;

namespace Biological_Signal_Processing_Using_AI.AITools
{
    public class TF_NET_NN
    {
        public delegate Session ModelArchitectureDelegate(TFNETBaseModel baseModel, Dictionary<string, NDArray> initVarsVals = null);

        private static (List<float[,]> inputBatches, List<float[,]> outputBatches) GetBatches(List<Sample> dataList, int suggestedBatchSize)
        {
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

            return (inputBatches, outputBatches);
        }

        public static CustomArchiBaseModel fit(CustomArchiBaseModel model, TFNETBaseModel baseModel, List<Sample> dataList,
            FittingProgAIReportDelegate fittingProgAIReportDelegate, float earlyStopThreshold = 0.0001f, float learningRate = 0.1f, bool saveModel = false, int suggestedBatchSize = 4, int epochsMax = 1000)
        {
            if (model._pcaActive)
                dataList = GeneralTools.rearrangeFeaturesInput(dataList, model.PCA);

            if (dataList.Count > 0)
            {
                // Get the session from the model
                Session session = baseModel.Session;
                session.as_default();

                // Sort data into batches
                (List<float[,]> inputBatches, List<float[,]> outputBatches) = GetBatches(dataList, suggestedBatchSize);

                // Start training
                // Get the necessary nodes for training from the model graph
                Tensor input = session.graph.OperationByName("input_place_holder");
                Tensor output = session.graph.OperationByName("output");
                Tensor outputPlaceholder = session.graph.OperationByName("output_place_holder");

                Tensor costFunc = session.graph.OperationByName("cost_function");
                Tensor learningRateTensor = session.graph.OperationByName("learning_rate");
                Operation optimizer = session.graph.OperationByName("optimizer");

                // Save the initiale values of the variables for restoring the graph if the learning isn't going well
                // Save them as assignment operations to the
                session.graph.as_default();
                IVariableV1[] currentVars = tf.global_variables();
                Dictionary<string, NDArray> currentVarsValsDict = new Dictionary<string, NDArray>();
                foreach (IVariableV1 iVar in currentVars)
                    currentVarsValsDict.Add(iVar.Name, session.run(iVar.AsTensor()));

                // Create an error queue for storing the mean training error of each epoch
                CircularQueue<float> costCirQueue = new CircularQueue<float>(15);
                float meanCost = 0;

                // Start training
                for (int epoch = 0; epoch < epochsMax; epoch++)
                {
                    // Iterate through the batches
                    for (int batch = 0; batch < inputBatches.Count; batch++)
                    {
                        // Train the model with the selected batch
                        (_, float cost) = session.run((optimizer, costFunc),
                                                    (learningRateTensor, learningRate),
                                                    (input, new NDArray(inputBatches[batch])),
                                                    (outputPlaceholder, new NDArray(outputBatches[batch])));

                        // Check if the cost went to infinity
                        if (float.IsNaN(cost) || float.IsInfinity(cost))
                        {
                            // If yes then decrease the learning rate
                            learningRate /= 10f;
                            // Restore the initial values of the variables (weights, and biases)
                            RefreshModelAndUpdateInitVals(baseModel, currentVars, currentVarsValsDict);
                        }

                        // Update the last and the mean cost value
                        meanCost += cost / inputBatches.Count;
                    }

                    // Update fitProgressBar
                    if (fittingProgAIReportDelegate != null)
                        fittingProgAIReportDelegate(epoch, epochsMax);

                    // Check if the learning is not improving according to earlyStopThreshold
                    // in the last 15 epochs
                    costCirQueue.Enqueue(meanCost);
                    meanCost = 0;

                    if (costCirQueue._count == 15)
                    {
                        (float mean, float min, float max) = GeneralTools.MeanMinMax(costCirQueue.ToArray());
                        if ((max - min) < earlyStopThreshold)
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
            session.as_default();

            // Get the input and output variables from the graph
            Tensor input = session.graph.OperationByName("input_place_holder");
            Tensor output = session.graph.OperationByName("output");

            // Arrange the features in a float[,]
            float[,] featuresFloat = new float[1, features.Length];
            for (int i = 0; i < features.Length; i++) featuresFloat[0, i] = (float)features[i];
            // Predict the input
            NDArray prediction = session.run(output, new FeedItem(input, featuresFloat));

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

        public static Session LoadModelVariables(TFNETBaseModel baseModel, ModelArchitectureDelegate modelArchitectureDelegate)
        {
            // Load the learned graph variables values
            Graph valsGraph = tf.train.load_graph(baseModel.ModelPath + "/model_variables.pb");
            // and create a temporal session for reading the variables tensors values
            Session tempSess = tf.Session();

            // Activate restore mode to enable both eager mode (run operations immediately without the need of a graph)
            // and graph mode (stores the new nodes to the default graph)
            tf.Context.restore_mode();

            // Get valsGraph Variables' values
            Dictionary<string, NDArray> initVarsVals = new Dictionary<string, NDArray>();
            string[] varsTensorsNames = valsGraph.get_operations().Where(iTensOp => iTensOp.op.OpType == "Identity").Select(iTensOp => iTensOp.name.Split("/")[0] + ":0").ToArray();
            foreach (string varTensName in varsTensorsNames)
            {
                Tensor valsTensor = valsGraph.get_tensor_by_name(varTensName);
                NDArray val = tempSess.run(valsTensor);
                initVarsVals.Add(varTensName, val);
            }

            // Create a new session for the model
            Session session = modelArchitectureDelegate(baseModel, initVarsVals);

            return session;
        }

        public static (Dictionary<string, Operation> initAssignmentsDict, Dictionary<string, NDArray> initVarsVals) AssignValsToVars(Session session, Dictionary<string, NDArray> initVarsVals = null)
        {
            // Set the session graph as the default graph
            session.graph.as_default();
            session.as_default();
            // Get the global variables from the default graph
            IVariableV1[] newVars = tf.global_variables();

            // Create the assignment operations
            if (initVarsVals == null)
                initVarsVals = new Dictionary<string, NDArray>();
            Dictionary<string, Operation> initAssignmentsDict = new Dictionary<string, Operation>();
            for (int i = 0; i < newVars.Length; i++)
            {
                IVariableV1 var = newVars[i];

                // Get the shape of the var input
                int inputDim = (int)var.shape[0];

                // Check if the variable value is not defined in initVarsVals
                if (!initVarsVals.ContainsKey(var.Name))
                    // If yes then initialize the variable randomly
                    // Initialize the weights to be 1 / inputDim so that one neuron from the input doesn't saturate the output on its own
                    // Initializing variables with the same values. Otherwise, all the weights will have the same value after training.
                    initVarsVals.Add(var.Name, new NDArray(tf.random.normal(var.shape, 0, 1f / (float)inputDim)));

                // Create the assignment operation
                Operation assignOp = tf.assign(var, initVarsVals[var.Name]);
                initAssignmentsDict.Add(var.Name, assignOp);
            }

            // Run the assignments of the variables
            session.run(initAssignmentsDict.Values.ToArray());

            return (initAssignmentsDict, initVarsVals);
        }

        public static void RefreshModelAndUpdateInitVals(TFNETBaseModel baseModel, IVariableV1[] currentVars, Dictionary<string, NDArray> currentVarsValsDict)
        {
            baseModel.Session.graph.as_default();
            baseModel.Session.as_default();
            // Iterate through the currentVars values and check if they are different that their corresponding assignment values in _AssignedValsDict
            foreach (IVariableV1 iVar in currentVars)
            {
                NDArray currentVarVal = currentVarsValsDict[iVar.Name];
                // Check if its previous value is different than the current one
                if (!baseModel._AssignedValsDict[iVar.Name].Equals(currentVarVal))
                {
                    // If yes then update the previous values and its assignment
                    baseModel._AssignedValsDict[iVar.Name] = currentVarVal;
                    Operation assignOp = tf.assign(iVar, currentVarVal);
                    baseModel._InitAssignmentsOpDict[iVar.Name] = assignOp;
                }
            }

            // Refresh the model by apply the new assignments
            baseModel.Session.run(baseModel._InitAssignmentsOpDict.Values.ToArray());
        }

        public static Tensor Layer(Tensor input, int outputDim, string name = null)
        {
            // Get the shape of the input
            int inputDim = (int)input.shape[1];

            // Create the weights and biases parameters
            ResourceVariable weightsVar = tf.Variable(tf.ones((inputDim, outputDim)));
            ResourceVariable biasesVar = tf.Variable(tf.zeros(outputDim));

            // Perform the Times operation with the weights and biases
            Tensor output = tf.add(tf.matmul(input, weightsVar), biasesVar, name);

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

        public static Tensor GELU(Tensor input, string name = null)
        {
            // The GELU (Gaussian Error Linear Unit) equation equals:
            // 0.5 * input * ( 1 + tanh [ sqrt ( 2 / pi ) * ( input + 0.044715 * pow ( input, 3 ) ) ] )

            // Start with the middle equation
            Tensor geluPow3 = tf.multiply(tf.multiply(tf.multiply(input, input), input), 0.044517f);
            Tensor geluMidAdd = tf.add(input, geluPow3);
            Tensor sqrtMul = tf.multiply(tf.sqrt(tf.multiply(2f, 1f / (float)Math.PI)), geluMidAdd);
            Tensor outerTanh = tf.add(1f, tf.tanh(sqrtMul));
            Tensor GELU = tf.multiply(tf.multiply(0.5f, input), outerTanh, name);

            return GELU;
        }

        public static Tensor ELU(Tensor input, float alfa = 1, string name = null)
        {
            // The ELU (Exponential Linear Unit) equation equals:
            // input                        ----- if ----> 0 < input
            // alfa * ( exp ( input ) - 1 ) ----- if ----> input <= 0
            // alfa: is a constante initialized to 1

            // Create only the second equation
            Tensor expELU = tf.multiply(alfa, tf.add(tf.exp(input), -1f));

            // Now set the conditions
            Tensor lowCondition = tf.less_equal(input, tf.constant(0f));

            // Perform the conditions to lineartHS
            Tensor ELU = tf.where(lowCondition, expELU, input, name);

            return ELU;
        }
    }
}

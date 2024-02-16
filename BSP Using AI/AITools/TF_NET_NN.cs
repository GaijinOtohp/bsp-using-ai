using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI;
using BSP_Using_AI.AITools;
using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation;
using Google.Protobuf;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Tensorflow;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.Structures;
using static Tensorflow.Binding;

namespace Biological_Signal_Processing_Using_AI.AITools
{
    internal class TF_NET_NN
    {
        private AIBackThreadReportHolder _aiBackThreadReportHolderForAIToolsForm;

        private Dictionary<string, ARTHTModels> _arthtModelsDic = null;

        public TF_NET_NN(Dictionary<string, ARTHTModels> arthtModelsDic, AIBackThreadReportHolder aiBackThreadReportHolderForAIToolsForm)
        {
            _arthtModelsDic = arthtModelsDic;
            _aiBackThreadReportHolderForAIToolsForm = aiBackThreadReportHolderForAIToolsForm;
        }

        public void fit(string modelsName, Dictionary<string, List<Sample>> dataLists, long datasetSize, long modelId, string stepName)
        {
            int fitProgress = 0;
            int tolatFitProgress = dataLists.Count;
            // Iterate through models from the selected ones in _targetsModelsHashtable

            // Fit features
            if (!stepName.Equals(""))
            {
                _arthtModelsDic[modelsName].ARTHTModelsDic[stepName] = createTFNETNeuralNetModel(stepName, dataLists[stepName], _arthtModelsDic[modelsName].ARTHTModelsDic[stepName]._pcaActive,
                                                        ((TFNETNeuralNetworkModel)_arthtModelsDic[modelsName].ARTHTModelsDic[stepName]).ModelPath);
                // Fit features in model
                fit((TFNETNeuralNetworkModel)_arthtModelsDic[modelsName].ARTHTModelsDic[stepName], dataLists[stepName], true);
            }
            else
                foreach (string stepNa in _arthtModelsDic[modelsName].ARTHTModelsDic.Keys)
                {
                    // Fit features in model
                    fit((TFNETNeuralNetworkModel)_arthtModelsDic[modelsName].ARTHTModelsDic[stepNa],
                                                                             dataLists[stepNa], true);

                    // Update fitProgressBar
                    fitProgress++;
                    if (_aiBackThreadReportHolderForAIToolsForm != null)
                        _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "progress", modelsName, fitProgress, tolatFitProgress }, "AIToolsForm");
                }
            // Update model in models table
            DbStimulator dbStimulator = new DbStimulator();
            if (_arthtModelsDic[modelsName].DataIdsIntervalsList.Count > 0)
            {
                Thread dbStimulatorThread = new Thread(() => dbStimulator.Update("models", new string[] { "the_model", "dataset_size" },
                    new object[] { GeneralTools.ObjectToByteArray(_arthtModelsDic[modelsName].Clone()), datasetSize }, modelId, "TF_NET_NN"));
                dbStimulatorThread.Start();
            }

            // Send report about fitting is finished and models table should be updated
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "fitting_complete", modelsName, datasetSize }, "AIToolsForm");
        }

        public static TFNETNeuralNetworkModel fit(TFNETNeuralNetworkModel model, List<Sample> dataList, bool saveModel = false, int suggestedBatchSize = 4)
        {
            if (model._pcaActive)
                dataList = GeneralTools.rearrangeFeaturesInput(dataList, model.PCA);

            if (dataList.Count > 0)
            {
                // Get the session from the model
                Session session = model.Session;

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
                for (int epoch = 0; epoch < 1000; epoch++)
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

                // Save model
                if (saveModel)
                    SaveModelVariables(model.Session, model.ModelPath);
            }

            return model;
        }

        public static double[] predict(double[] features, TFNETNeuralNetworkModel model)
        {
            // Initialize input
            if (model._pcaActive)
                features = GeneralTools.rearrangeInput(features, model.PCA);

            // Get the session from the model
            Session session = model.Session;

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

        public void createTFNETNeuralNetworkModelForWPW()
        {
            // Set models in name and path
            int modelIndx = 0;
            while (_arthtModelsDic.ContainsKey(TFNETNeuralNetworkModel.ModelName + " for WPW syndrome detection" + modelIndx))
                modelIndx++;
            string modelPath = System.IO.Directory.GetCurrentDirectory() + @"/AIModels/TFNETModels/NN/WPW" + modelIndx + "/";

            // Create neural network models for WPW syndrome detection
            // Create 7 models for { 2 for QRS detection (Threshold_ratio & Hor_threshold, remove_miss_selected_R),
            // 2 for P_T detection (Threshold_ratio & Hor_threshold, P & T states),
            // 1 for short PR detection,
            // 2 for delta deteciton (Acceleration threshold, delta existence) }
            ARTHTModels arthtModels = new ARTHTModels();
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step1RPeaksScanData] = createTFNETNeuralNetModel(ARTHTNamings.Step1RPeaksScanData, modelPath + 0, 15, 2); // For R peaks detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step2RPeaksSelectionData] = createTFNETNeuralNetModel(ARTHTNamings.Step2RPeaksSelectionData, modelPath + 1, 2, 1); // For R selection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step3BeatPeaksScanData] = createTFNETNeuralNetModel(ARTHTNamings.Step3BeatPeaksScanData, modelPath + 2, 5, 2); // For beat peaks detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step4PTSelectionData] = createTFNETNeuralNetModel(ARTHTNamings.Step4PTSelectionData, modelPath + 3, 3, 2); // For P and T detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step5ShortPRScanData] = createTFNETNeuralNetModel(ARTHTNamings.Step5ShortPRScanData, modelPath + 4, 1, 1); // For short PR detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step6UpstrokesScanData] = createTFNETNeuralNetModel(ARTHTNamings.Step6UpstrokesScanData, modelPath + 5, 6, 1); // For delta detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step7DeltaExaminationData] = createTFNETNeuralNetModel(ARTHTNamings.Step7DeltaExaminationData, modelPath + 6, 6, 1); // For WPW syndrome declaration

            arthtModels.ModelName = TFNETNeuralNetworkModel.ModelName;// + " for WPW syndrome detection" + modelIndx;
            arthtModels.ProblemName = " for WPW syndrome detection";
            while (_arthtModelsDic.ContainsKey(arthtModels.ModelName + arthtModels.ProblemName + modelIndx))
                modelIndx++;
            arthtModels.ProblemName = " for WPW syndrome detection" + modelIndx;
            _arthtModelsDic.Add(arthtModels.ModelName + arthtModels.ProblemName, arthtModels);

            // Save path in models table
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.Insert("models", new string[] { "type_name", "model_target", "the_model", "dataset_size" },
                new object[] { TFNETNeuralNetworkModel.ModelName, "WPW syndrome detection", GeneralTools.ObjectToByteArray(arthtModels.Clone()), 0 }, "AIToolsForm");

            // Refresh modelsFlowLayoutPanel
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "createModel" }, "AIToolsForm");
        }

        public static TFNETNeuralNetworkModel createTFNETNeuralNetModel(string stepName, List<Sample> dataList, bool pcaActive, string modelPath)
        {
            (int inputDim, int outputDim, List<PCAitem> pca) = GetStepDimensions(stepName, dataList, pcaActive);

            TFNETNeuralNetworkModel tempModel = createTFNETNeuralNetModel(stepName, modelPath, inputDim, outputDim);
            tempModel._pcaActive = pcaActive;
            tempModel.PCA = pca;

            return tempModel;
        }

        public static TFNETNeuralNetworkModel createTFNETNeuralNetModel(string name, string path, int inputDim, int outputDim)
        {
            TFNETNeuralNetworkModel model = new TFNETNeuralNetworkModel() { Name = name, ModelPath = path };

            model.Session = createTFNETNeuralNetModelSession(inputDim, outputDim);

            // Save the model
            if (path.Length > 0)
                SaveModelVariables(model.Session, path);

            // Set initial thresholds for output decisions
            model.OutputsThresholds = new float[outputDim];
            for (int i = 0; i < outputDim; i++)
                model.OutputsThresholds[i] = 0.5f;

            // Get the parameters
            return model;
        }
        public static Session createTFNETNeuralNetModelSession(int inputDim, int outputDim)
        {
            // Disable eager mode to enable storing new nodes to the default graph automatically
            tf.compat.v1.disable_eager_execution();

            // The model operations and variables are organized in a graph
            // The graph is automatically built in the default graph
            Graph graph = new Graph().as_default();
            // Create the list of variables (wieghts, and biases) assignments
            List<Operation> assignmentsList = new List<Operation>();
            // Define the input and output variables
            Tensor input = tf.placeholder(tf.float32, (-1, inputDim), name: "input_place_holder");
            Tensor outputPlaceholder = tf.placeholder(tf.float32, (-1, outputDim), name: "output_place_holder");
            // Start from the first hidden layer, since the input is not actually a layer   
            // but inform the shape of the input, with "input" elements.
            int hidLayerDim = (int)((float)inputDim * (2f / 3f) + outputDim);
            // Define the model function
            Tensor hiddenLayer1 = tf.nn.tanh(Layer(input, hidLayerDim, assignmentsList)); // The input layer connected to the 1st hidden layer. The 1st layer has "Tanh" as activation function
            Tensor hiddenLayer2 = Layer(hiddenLayer1, hidLayerDim, assignmentsList); // The 1st hidden layer connected to the 2nd hidden layer. The 2nd layer has "linear" as activation function (default)
            Tensor output = HardSigmoid(Layer(hiddenLayer2, outputDim, assignmentsList), "output"); // The 2nd hidden layer connected to the output layer. The output layer has "hard sigmoid" as activation function
            Session session = new Session(graph);
            // Run the assignments of the variables
            session.run(assignmentsList.ToArray());
            // Insert the cost function operation to the graph of the model
            Tensor costFunc = tf.reduce_mean(tf.square(tf.sub(output, outputPlaceholder)), name: "cost_function");
            // Insert the optimizer operation to the graph of the model with a default learning rate of 0.01
            Tensor learning_rate = tf.placeholder(tf.float32, Shape.Scalar, name: "learning_rate");
            Operation optimizer = tf.train.GradientDescentOptimizer(learning_rate).minimize(costFunc, name: "optimizer");

            return session;
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

        public static void SaveModelVariables(Session session, string path)
        {
            var output_graph_def = tf.graph_util.convert_variables_to_constants(
                session, session.graph.as_graph_def(), new string[] { "output" });
            System.IO.Directory.CreateDirectory(path);
            File.WriteAllBytes(path + "/model_variables.pb", output_graph_def.ToByteArray());
            //File.WriteAllText(path + "labels.txt", string.Join("\n", new string[] { "3output" }));
        }

        public static Session LoadModelVariables(string path)
        {
            // Load the learned graph variables values
            Graph valsGraph = tf.train.load_graph(path + "/model_variables.pb");
            // and create a temporal session for reading the variables tensors values
            Session tempSess = tf.Session();
            // Get the input and output tensors dimensions
            int inputDim = (int)valsGraph.get_tensor_by_name("input_place_holder:0").dims[1];
            int outputDim = (int)valsGraph.get_tensor_by_name("output:0").dims[1];

            // Activate restore mode to enable both eager mode (run operations immediately without the need of a graph)
            // and graph mode (stores the new nodes to the default graph)
            tf.Context.restore_mode();

            // Create a new session for the model
            Session session = createTFNETNeuralNetModelSession(inputDim, outputDim);

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

        public void initializeTFNETNeuralNetworkModelsForWPW(ARTHTModels arthtModels)
        {
            string[] stepsNames = arthtModels.ARTHTModelsDic.Keys.ToArray();
            foreach (string stepName in stepsNames)
            {
                TFNETNeuralNetworkModel model = ((TFNETModelLessNeuralNetwork)arthtModels.ARTHTModelsDic[stepName]).Clone();
                model.Session = LoadModelVariables(model.ModelPath);
                arthtModels.ARTHTModelsDic[stepName] = model;
            }

            // Insert models in _arthtModelsDic
            _arthtModelsDic.Add(arthtModels.ModelName + arthtModels.ProblemName, arthtModels);
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
            Tensor layer = tf.add(tf.matmul(input, weightsVar), biasesVar, name);

            // Insert the assignemnts to assignmentsList if its not null
            assignmentsList?.Add(weightsAssign);
            assignmentsList?.Add(biasesAssign);

            return layer;
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

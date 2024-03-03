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
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;
using static Biological_Signal_Processing_Using_AI.Structures;
using static Tensorflow.Binding;

namespace Biological_Signal_Processing_Using_AI.AITools.Keras_NET_Objectives
{
    public class ARTHT_TF_NET_NN
    {
        private AIBackThreadReportHolder _aiBackThreadReportHolderForAIToolsForm;

        private Dictionary<string, ARTHTModels> _arthtModelsDic = null;

        public ARTHT_TF_NET_NN(Dictionary<string, ARTHTModels> arthtModelsDic, AIBackThreadReportHolder aiBackThreadReportHolderForAIToolsForm)
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
                TF_NET_NN.fit((TFNETNeuralNetworkModel)_arthtModelsDic[modelsName].ARTHTModelsDic[stepName], dataLists[stepName], true);
            }
            else
                foreach (string stepNa in _arthtModelsDic[modelsName].ARTHTModelsDic.Keys)
                {
                    // Fit features in model
                    TF_NET_NN.fit((TFNETNeuralNetworkModel)_arthtModelsDic[modelsName].ARTHTModelsDic[stepNa],
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
                TF_NET_NN.SaveModelVariables(model.Session, path);

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
            Tensor hiddenLayer1 = tf.nn.tanh(TF_NET_NN.Layer(input, hidLayerDim, assignmentsList)); // The input layer connected to the 1st hidden layer. The 1st layer has "Tanh" as activation function
            Tensor hiddenLayer2 = TF_NET_NN.Layer(hiddenLayer1, hidLayerDim, assignmentsList); // The 1st hidden layer connected to the 2nd hidden layer. The 2nd layer has "linear" as activation function (default)
            Tensor output = TF_NET_NN.HardSigmoid(TF_NET_NN.Layer(hiddenLayer2, outputDim, assignmentsList), "output"); // The 2nd hidden layer connected to the output layer. The output layer has "hard sigmoid" as activation function
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
    }
}

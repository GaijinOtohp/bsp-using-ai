using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation.OutValDetails;
using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation.OutValDetails.Titles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels;
using static BSP_Using_AI.AITools.Details.DetailsForm;

namespace BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation
{
    public partial class DataVisualisationForm
    {
        private void validationSaveButton_Click(object sender, EventArgs e)
        {
            // Copy the thresholds from validationFlowLayoutPanel to _InnerObjectiveModel.OutputsThresholds
            for (int iControl = 0; iControl < validationFlowLayoutPanel.Controls.Count; iControl++)
            {
                Control outPutValControl = validationFlowLayoutPanel.Controls[iControl];
                double threshold = 0.5d;
                string newThreshold = "";

                if (outPutValControl is OutValClasAcPpNpMetr classAcPpNp) newThreshold = classAcPpNp.classificationThresholdTextBox.Text;
                else if (outPutValControl is OutValClassAccF1ScoreMetr classAccF1Score) newThreshold = classAccF1Score.classificationThresholdTextBox.Text;
                else if (outPutValControl is OutValClassAccSeSpMetrics classAccSeSp) newThreshold = classAccSeSp.classificationThresholdTextBox.Text;
                else if (outPutValControl is OutValClassRawMetrics classRaw) newThreshold = classRaw.classificationThresholdTextBox.Text;
                else if (outPutValControl is OutValRegrRawMetrics regrRaw) newThreshold = regrRaw.classificationThresholdTextBox.Text;

                if (newThreshold.Length > 0)
                    threshold = double.Parse(newThreshold);

                _InnerObjectiveModel.OutputsThresholds[iControl]._threshold = threshold;
            }

            // Update the model with the new thresholds
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.Update("models", new string[] { "the_model" },
                new Object[] { GeneralTools.ObjectToByteArray(_ObjectiveModel.Clone()) }, _modelId, "DetailsForm");
        }

        private void setValidationDetailsVisTab()
        {
            // List the evaluation techniques
            if (_InnerObjectiveModel.Type == ObjectiveType.Classification)
                metricsComboBox.DataSource = EvaluationTechnique.GetNames();

            refreshValidationData();
        }

        private void refreshValidationData()
        {
            // Insert new vallidation data in validationFlowLayoutPanel
            validationFlowLayoutPanel.Controls.Clear();
            validationTitlesPanel.Controls.Clear();

            // Check which evaluation metrics is selected
            string selectedEvaluation = (string)metricsComboBox.SelectedItem;
            if (_InnerObjectiveModel.Type == ObjectiveType.Regression)
                DisplayRegrRawEvaluation();
            else if (selectedEvaluation.Equals(DetailsForm.EvaluationTechnique.Raw))
                DisplayClassRawEvaluation();
            else if (selectedEvaluation.Equals(DetailsForm.EvaluationTechnique.AccSeSp))
                DisplayClassAccSeSpEvaluation();
            else if (selectedEvaluation.Equals(DetailsForm.EvaluationTechnique.AccPpvNpv))
                DisplayClassAccPpvNpvEvaluation();
            else if (selectedEvaluation.Equals(DetailsForm.EvaluationTechnique.AccF1Score))
                DisplayClassAccF1ScoreEvaluation();
        }

        private void DisplayRegrRawEvaluation()
        {
            // Insert titles
            validationTitlesPanel.Controls.Add(new OutRegrRawTit());

            for (int iOutput = 0; iOutput < _InnerObjectiveModel.OutputsNames.Length; iOutput++)
            {
                OutputMetrics outputMetrics = _InnerObjectiveModel.ValidationData._ModelOutputsValidMetrics[iOutput];

                OutValRegrRawMetrics outValRegrRawMetricsUserControl = new OutValRegrRawMetrics();

                outValRegrRawMetricsUserControl.Name = _InnerObjectiveModel.OutputsNames[iOutput];
                outValRegrRawMetricsUserControl.outputLabel.Text = _InnerObjectiveModel.OutputsNames[iOutput];
                outValRegrRawMetricsUserControl.maseLabel.Text = Math.Round(outputMetrics._mase, 4).ToString();
                outValRegrRawMetricsUserControl.classificationThresholdTextBox.Text = Math.Round(_InnerObjectiveModel.OutputsThresholds[iOutput]._threshold, 4).ToString();

                validationFlowLayoutPanel.Controls.Add(outValRegrRawMetricsUserControl);
            }
        }

        private void DisplayClassRawEvaluation()
        {
            // Insert titles
            validationTitlesPanel.Controls.Add(new OutClasRawTit());

            for (int iOutput = 0; iOutput < _InnerObjectiveModel.OutputsNames.Length; iOutput++)
            {
                OutputMetrics outputMetrics = _InnerObjectiveModel.ValidationData._ModelOutputsValidMetrics[iOutput];

                OutValClassRawMetrics outValClassRawMetricsUserControl = new OutValClassRawMetrics();

                outValClassRawMetricsUserControl.Name = _InnerObjectiveModel.OutputsNames[iOutput];
                outValClassRawMetricsUserControl.outputLabel.Text = _InnerObjectiveModel.OutputsNames[iOutput];
                outValClassRawMetricsUserControl.truePositiveLabel.Text = outputMetrics._truePositive.ToString();
                outValClassRawMetricsUserControl.trueNegativeLabel.Text = outputMetrics._trueNegative.ToString();
                outValClassRawMetricsUserControl.falsePositiveLabel.Text = outputMetrics._falsePositive.ToString();
                outValClassRawMetricsUserControl.falseNegativeLabel.Text = outputMetrics._falseNegative.ToString();
                outValClassRawMetricsUserControl.classificationThresholdTextBox.Text = Math.Round(_InnerObjectiveModel.OutputsThresholds[iOutput]._threshold, 4).ToString();

                validationFlowLayoutPanel.Controls.Add(outValClassRawMetricsUserControl);
            }
        }

        private void DisplayClassAccSeSpEvaluation()
        {
            // Insert titles
            validationTitlesPanel.Controls.Add(new OutClasAcSeSpTit());

            for (int iOutput = 0; iOutput < _InnerObjectiveModel.OutputsNames.Length; iOutput++)
            {
                OutputMetrics outputMetrics = _InnerObjectiveModel.ValidationData._ModelOutputsValidMetrics[iOutput];

                double accuracy = (double)(outputMetrics._truePositive + outputMetrics._trueNegative) /
                                  (outputMetrics._truePositive + outputMetrics._trueNegative + outputMetrics._falsePositive + outputMetrics._falseNegative);
                double sensitivity = (double)outputMetrics._truePositive / (outputMetrics._truePositive + outputMetrics._falseNegative);
                double specificity = (double)outputMetrics._trueNegative / (outputMetrics._trueNegative + outputMetrics._falsePositive);

                OutValClassAccSeSpMetrics outValClassAccSeSpMetricsUserControl = new OutValClassAccSeSpMetrics();

                outValClassAccSeSpMetricsUserControl.Name = _InnerObjectiveModel.OutputsNames[iOutput];
                outValClassAccSeSpMetricsUserControl.outputLabel.Text = _InnerObjectiveModel.OutputsNames[iOutput];
                outValClassAccSeSpMetricsUserControl.accuracyLabel.Text = Math.Round(accuracy, 4).ToString();
                outValClassAccSeSpMetricsUserControl.sensitivityLabel.Text = Math.Round(sensitivity, 4).ToString();
                outValClassAccSeSpMetricsUserControl.specificityLabel.Text = Math.Round(specificity, 4).ToString();
                outValClassAccSeSpMetricsUserControl.classificationThresholdTextBox.Text = Math.Round(_InnerObjectiveModel.OutputsThresholds[iOutput]._threshold, 4).ToString();

                validationFlowLayoutPanel.Controls.Add(outValClassAccSeSpMetricsUserControl);
            }
        }

        private void DisplayClassAccPpvNpvEvaluation()
        {
            // Insert titles
            validationTitlesPanel.Controls.Add(new OutClasAcPpNpTit());

            for (int iOutput = 0; iOutput < _InnerObjectiveModel.OutputsNames.Length; iOutput++)
            {
                OutputMetrics outputMetrics = _InnerObjectiveModel.ValidationData._ModelOutputsValidMetrics[iOutput];

                double accuracy = (double)(outputMetrics._truePositive + outputMetrics._trueNegative) /
                                  (outputMetrics._truePositive + outputMetrics._trueNegative + outputMetrics._falsePositive + outputMetrics._falseNegative);
                double ppv = (double)outputMetrics._truePositive / (outputMetrics._truePositive + outputMetrics._falsePositive);
                double npv = (double)outputMetrics._trueNegative / (outputMetrics._trueNegative + outputMetrics._falseNegative);

                OutValClasAcPpNpMetr outValClassAccPpvNpvMetricsUserControl = new OutValClasAcPpNpMetr();

                outValClassAccPpvNpvMetricsUserControl.Name = _InnerObjectiveModel.OutputsNames[iOutput];
                outValClassAccPpvNpvMetricsUserControl.outputLabel.Text = _InnerObjectiveModel.OutputsNames[iOutput];
                outValClassAccPpvNpvMetricsUserControl.accuracyLabel.Text = Math.Round(accuracy, 4).ToString();
                outValClassAccPpvNpvMetricsUserControl.ppvLabel.Text = Math.Round(ppv, 4).ToString();
                outValClassAccPpvNpvMetricsUserControl.npvLabel.Text = Math.Round(npv, 4).ToString();
                outValClassAccPpvNpvMetricsUserControl.classificationThresholdTextBox.Text = Math.Round(_InnerObjectiveModel.OutputsThresholds[iOutput]._threshold, 4).ToString();

                validationFlowLayoutPanel.Controls.Add(outValClassAccPpvNpvMetricsUserControl);
            }
        }

        private void DisplayClassAccF1ScoreEvaluation()
        {
            // Insert titles
            validationTitlesPanel.Controls.Add(new OutClasAcF1STit());

            for (int iOutput = 0; iOutput < _InnerObjectiveModel.OutputsNames.Length; iOutput++)
            {
                OutputMetrics outputMetrics = _InnerObjectiveModel.ValidationData._ModelOutputsValidMetrics[iOutput];

                double accuracy = (double)(outputMetrics._truePositive + outputMetrics._trueNegative) /
                                  (outputMetrics._truePositive + outputMetrics._trueNegative + outputMetrics._falsePositive + outputMetrics._falseNegative);
                
                double precision = (double)outputMetrics._truePositive / (outputMetrics._truePositive + outputMetrics._falsePositive);
                double recall = (double)outputMetrics._trueNegative / (outputMetrics._trueNegative + outputMetrics._falsePositive);
                double f1Score = 2 * (precision * recall) / (precision + recall);

                OutValClassAccF1ScoreMetr outValClassAccSeSpMetricsUserControl = new OutValClassAccF1ScoreMetr();

                outValClassAccSeSpMetricsUserControl.Name = _InnerObjectiveModel.OutputsNames[iOutput];
                outValClassAccSeSpMetricsUserControl.outputLabel.Text = _InnerObjectiveModel.OutputsNames[iOutput];
                outValClassAccSeSpMetricsUserControl.accuracyLabel.Text = Math.Round(accuracy, 4).ToString();
                outValClassAccSeSpMetricsUserControl.f1ScoreLabel.Text = Math.Round(f1Score, 4).ToString();
                outValClassAccSeSpMetricsUserControl.classificationThresholdTextBox.Text = Math.Round(_InnerObjectiveModel.OutputsThresholds[iOutput]._threshold, 4).ToString();

                validationFlowLayoutPanel.Controls.Add(outValClassAccSeSpMetricsUserControl);
            }
        }
    }
}


[![MIT License](https://img.shields.io/badge/License-MIT-green.svg)](https://choosealicense.com/licenses/mit/)


# Biological Signal Processing using AI

This is a friendly desktop application project in C# for signal processing, and customized for Electrocardiogram (ECG) signal features extraction and classification.

## Content

- [Features of the application](#features-of-the-application)
- [Prerequisites](#prerequisites)
- [Application Overview](#application-overview)
   - [Main form](#main-form)
   - [Details/Modify Form](#detailsmodify-form)
   - [Signal fusion Form](#signal-fusion-form)
   - [Signal comparator Form](#signal-comparator-form)
   - [Signal collector form](#signal-collector-form)
   - [AI tools form](#ai-tools-form)
   - [Model details form](#model-details-form)
   - [Data analysis form](#data-analysis-form)
- [License](#license)

## Features of the application
. Signals comparator (cross-correlation, minimum distance, dynamic time wrapping).  
. Filtering tools (Butterworth, Chebychev I, Chebychev II, DC removal, Normalization, Discrete Wavelet Transform, etc.).  
. Signal spectrum analyzer.  
. Signal peaks analyzer (up peaks, down peaks, stable state, tangent deviation states).  
. Customized Machine learning algorithms (Deep Neural Networks, Naive Bayes, K-nearest neighbor) for ECG waves detection.  
. A customized tool for data generation (features selection).  
. Data analysis (raw visualization, Principal Component Analysis).

## Prerequisites
For implementing the Tensorflow.NET framework, this application is dependent on [.NET 6.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) and [Visdual Studio 2022 17.0](https://learn.microsoft.com/en-us/visualstudio/releases/2022/release-notes-v17.0) or later.

## Application Overview

<p align="center">
  <img src="./images/1 - main_form.png" alt="BSP using AI - main form">
</p>

### Main form
The main form gives access to navigate to "Signal comparator", "Signals collector", "AI tools", and "Signal details/modify" forms. The "Choose File" button opens a file dialog for choosing the signal. The application allows for plotting multiple signals on the same window. Two types of signal files are accepted (".mat", and ".txt") as given in [bsp-using-ai/Sample signals/](https://github.com/GaijinOtohp/bsp-using-ai/tree/main/Sample%20signals). A window will pop up after selecting the signal for inserting the corresponding sampling rate and quantization step to the chosen signal.

<p align="center">
  <img src="./images/2 - main_form_choose_signal.png" alt="BSP using AI - choose signal">
</p>

### Details/Modify Form
It contains multiple filtering tools and shows its effect on the signal in addition to the spectrum analysis that corresponds to the signal. The section below the horizontal line belongs to our customized data generation for the ECG signals classification (click the blue button named "Set features labels" to start generating data, and after finishing click "Save" in the main form). All the created machine-learning models will be listed in the combo box next to the "Predict" button. Select the desired model and click "Predict" for annotating the signal automatically using the selected model (any changes should be discarded by clicking the red button named "Discard" before starting the prediction).

<p align="center">
  <img src="./images/3 - signal_details_form.png" alt="BSP using AI - signal details form">
</p>

The figure below shows an example of applying a band pass filter using two separate filters (a high pass and a low pass).

<p align="center">
  <img src="./images/4 - signal_details_form_concatenate_filters.png" alt="BSP using AI - signal details form - concatenate filters">
</p>

The order of the filters matters as illustrated in the two figures below:

<p align="center">
  <img src="./images/5 - signal_details_form_filters_order_matters_1.png" alt="BSP using AI - signal details form - filters order matters 1">
</p>

<p align="center">
  <img src="./images/6 - signal_details_form_filters_order_matters_2.png" alt="BSP using AI - signal details form - filters order matters 2">
</p>

Left-clicking on the filter pops up a menu for deleting the selected filter:

<p align="center">
  <img src="./images/7 - signal_details_form_delete_filter.png" alt="BSP using AI - signal details form - delete filter">
</p>

### Signal fusion Form
Signal fusion divides the signal into periods of frequency with the highest magnitude and provides multiple operations to perform across the separated periods. The period range is adjustable manually if any auto miss-calculation occurs. The periods can be centralized and stretched concerning the R peak, or even translated with an offset.

<p align="center">
  <img src="./images/8 - signal_fusion_form_choose_period.png" alt="BSP using AI - signal fusion form - choose period">
</p>

The operations apply across all periods. The addition operation takes the samples of all the periods and divides them by the number of periods. It's like creating the mean of the periods.

<p align="center">
  <img src="./images/9 - signal_fusion_form_addition.png" alt="BSP using AI - signal fusion form - addition operation">
</p>

The multiplication operation is the same as the addition operation but instead of adding the samples, it multiplies the samples.

<p align="center">
  <img src="./images/10 - signal_fusion_form_multiplication.png" alt="BSP using AI - signal fusion form - multiplication operation">
</p>

The cross-correlation operation applies the cross-correlation across all the periods.

<p align="center">
  <img src="./images/11 - signal_fusion_form_cross-correlation.png" alt="BSP using AI - signal fusion form - cross-correlation operation">
</p>

The orthogonalization operation applies the Gram–Schmidt orthogonalization between the periods and outputs the result of orthogonal periods named "Psi".

<p align="center">
  <img src="./images/12 - signal_fusion_form_orthogonalization.png" alt="BSP using AI - signal fusion form - orthogonalization operation">
</p>

The fuse button next to orthogonalization takes the Psies and performs an addition operation across them.

<p align="center">
  <img src="./images/13 - signal_fusion_form_orthogonalization-fuse.png" alt="BSP using AI - signal fusion form - fused orthogonalization operation">
</p>

### Signal comparator Form
It accepts two signals for comparison. Ticking the checkboxes named "Select first signal" and "Select second signal" allows for the reception of the signals for comparison. Almost every plot in the application has the option of sending its signal for comparison. Clicking on the plot the first left click pops up the default menu of "ScottPlot", then left-clicking on the plot for the second time popsup the menu for sending the signal for comparison as shown in the figure below:

<p align="center">
  <img src="./images/14 - signals_comparator_form_receive_signal.png" alt="BSP using AI - signals comparator form - receive signal">
</p>

Three options for comparison are available. The first one is cross-correlation:

<p align="center">
  <img src="./images/15 - signals_comparator_form_cross-correlation.png" alt="BSP using AI - signals comparator form - cross-correlation">
</p>

The second option is named "Minimum distance". It translates the second signal with the offset of the highest coefficient in cross-correlation, and takes the absolute subtraction of the two signals as "Minimum distance":

<p align="center">
  <img src="./images/16 - signals_comparator_form_minimum_distance.png" alt="BSP using AI - signals comparator form - minimum distance">
</p>

The third option of comparison is Dynamic Time Wrapping:

<p align="center">
  <img src="./images/17 - signals_comparator_form_dynamic_time_wraping.png" alt="BSP using AI - signals comparator form - dynamic time wrapping">
</p>

### Signal collector Form
It is an option for gathering different signals close together for better manual visual analysis.

<p align="center">
  <img src="./images/18 - signals-collector-form.png" alt="BSP using AI - signals collector form">
</p>

### AI tools Form
This part is customized only for ECG signals features classification (peaks annotation) using three types of machine learning models (Neural Networks, K-Nearest Neighbors, and Naive Bayes). It also contains data analysis tools (raw data visualization, and Principal Component Analysis) that can be used for other purposes. For creating a new model, the user will have to choose the model type and the problem to solve from the two combo boxes in the top-left corner, then click "Create new model". The button "Dataset explorer" opens the form for listing the previously generated saved data. The button "Fit" opens the form for choosing the training data for the model.

<p align="center">
  <img src="./images/19 - ai-tools-form.png" alt="BSP using AI - ai tools form">
</p>

### Model details Form
It displays the training data and validation information of the model. It also allows for changing the decision thresholds of the classification models. Any changes applied to the decision thresholds can be saved by clicking the "Save changes" button. Clicking on any of the seven sub-models opens the form of data analysis of the selected sub-model.

<p align="center">
  <img src="./images/20 - ai-model-validation-details.png" alt="BSP using AI - model details form">
</p>

### Data analysis Form
The raw data visualization differs from coloring the data points in the scatter-plot linearly for models solving regression problems to associating each class with a unique color for models solving classification problems as shown in the two figures below respectively.

<p align="center">
  <img src="./images/21 - ai-model_raw-data-visualization_regression.png" alt="BSP using AI - data visualization form - raw data analysis for regression problem">
</p>

<p align="center">
  <img src="./images/22 - ai-model_raw-data-visualization_classification.png" alt="BSP using AI - data visualization form - raw data analysis for classification problem">
</p>

The Principal Component Analysis tab displays the Eigenvalues of the training dataset in a histogram. Clicking on any bar in the histogram displays the coefficients of the corresponding Eigenvector.

<p align="center">
  <img src="./images/23 - ai-model_pca-eigenvector.png" alt="BSP using AI - data visualization form - pca eigenvector">
</p>

## License
[MIT](https://choosealicense.com/licenses/mit/)


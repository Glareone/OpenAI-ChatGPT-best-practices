# Machine Learning
This document explains concepts and ideas behind Machine Learning and its types.

### Machine Learning Types
![image](https://github.com/Glareone/OpenAI-and-ChatGPT-meet-.Net/assets/4239376/aedb3a78-9a83-407b-821c-67e553fe6faa)
1. Regression. Regression is a form of supervised machine learning in which the label predicted by the model is a numeric value. For example: based on historical data predict the number of sales of icecream for exact day based on weather, rain, humidity, day of the week, etc.  
2. Classification. Classification is a form of supervised machine learning in which the label represents a categorization, or class. There are two common classification scenarios.
3. Binary classification determines whether the observed item is (or isn't) an instance of a specific class. Binary classification models predict one of two mutually exclusive outcomes, like "Whether a patient is at risk for diabetes based on clinical metrics like weight, age, blood glucose level".  
4. Multiclass classification. Multiclass classification extends binary classification to predict a label that represents one of multiple possible classes. For example, The species of a penguin (Adelie, Gentoo, or Chinstrap) based on its physical measurements.  
5. Clustering. The most common form of unsupervised machine learning is clustering. A clustering algorithm identifies similarities between observations based on their features, and groups them into discrete clusters.   

#### Difference between Clustering and Multiclass Classification
Difference is the following: in Clustering ML you dont know the amount of classes, you are trying to group your elements based on their similarities.

### Machine Learning as a Function
![image](https://github.com/Glareone/OpenAI-and-ChatGPT-meet-.Net/assets/4239376/9eca4346-d512-4313-b924-2605cb81c396)

1. The training data consists of past observations  
2. An algorithm is applied to the data to try to determine a relationship between the features and the label, and generalize that relationship as a calculation that can be performed on x to calculate y  
3. y = f(x); The result of the algorithm is a model that encapsulates the calculation derived by the algorithm as a function
4. Now that the training phase is complete, the trained model can be used for inferencing. The model is essentially a software program that encapsulates the function produced by the training process

### Machine Learning. Regression
![image](https://github.com/Glareone/OpenAI-and-ChatGPT-meet-.Net/assets/4239376/9674fedd-a181-49f6-bd74-586e9a7c8c0a)

1. Split the training data (randomly) to create a dataset with which to train the model while holding back a subset of the data that you'll use to validate the trained model.
2. Use an algorithm to fit the training data to a model. In the case of a regression model, use a regression algorithm such as linear regression.
3. Use the validation data you held back to test the model by predicting labels for the features.
4. Compare the known actual labels in the validation dataset to the labels that the model predicted. Then aggregate the differences between the predicted and actual label values to calculate a metric that indicates how accurately the model predicted for the validation data.


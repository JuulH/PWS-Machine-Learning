import numpy as np
from sklearn import datasets
from sklearn.model_selection import train_test_split
from sklearn.metrics import classification_report
import matplotlib.pyplot as plt
from matplotlib.colors import ListedColormap
cmap = ListedColormap(['#FF0000', '#00FF00', '#0000FF'])

iris = datasets.load_iris()
X, y = iris.data, iris.target

X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=1234)

print(X_train, X_test, y_train, y_test)

from knn import KNN
classify = KNN(k=3) # Sum samples
classify.fit(X_train, y_train) # Training
predictions = classify.predict(X_test) # Testing
print(predictions)

acc = np.sum(predictions == y_test) / len(y_test)
print(classification_report(y_test, predictions))
print('Accuracy:', acc)

plt.figure()
plt.scatter(X[:, 0], X[:, 1], c=y, cmap=cmap, edgecolors='k', s=20)
plt.show()

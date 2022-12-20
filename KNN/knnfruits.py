import numpy as np
import matplotlib.pyplot as plt
from matplotlib.colors import ListedColormap
import random
cmap = ListedColormap(['#FF0000', '#00FF00', '#0000FF'])

fruits = []
fAmount = 50

xMin = 0
xMax = 10

yMin = 0
yMin = 25

for i in range(fAmount):
    fruits.append([random.randint(xMin, xMax), random.randint(yMin, yMin)])

print(fruits)

plt.figure()
plt.scatter(fruits, fruits, c=fruits, cmap=cmap, edgecolors='k', s=20)
plt.show()

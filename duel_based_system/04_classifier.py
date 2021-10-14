#!/usr/bin/env python

# The code for classifier training in different scenrios:
# - with/without auxiliary training
# - winner_in/ any_in pairing scheme


import keras
import pandas as pd
from PIL import Image
from PIL import ImageColor
import numpy as np
import matplotlib.pyplot as plt
from os import listdir
from os.path import isfile, join
import tensorflow as tf
import pickle
import glob 
from skimage.color import rgb2gray


from keras.models import Sequential, Model
from keras.layers import concatenate, Input, Flatten, Dense, Conv2D, GlobalAveragePooling2D, Dropout, MaxPooling2D
from keras.models import load_model
from tensorflow.keras import regularizers

# variables
variant="01"
n_outputs=2
with_aux_training=False
n_epoch=20
PATH_train='./filled_in/experiment'+variant+"_train/train/"
PATH_val='./filled_in/experiment'+variant+"_val/val/"
PATH_test='./filled_in/experiment'+variant+"_test/test/"
# with features needed for auxiliary training
PATH_to_csvs="./experiment"+variant
autoencoder_model="AE_experiment"+variant+".h5"
test_performance_file="class_performance_exp"+variant+"_"+str(n_outputs)+".txt"




# Load dictionaries needed for Data Generator
with open("./dict_data_train_eksp"+variant+"_"+str(n_outputs)+".pickle", 'rb') as handle:
    data_train = pickle.load(handle)

with open("./dict_data_val_eksp"+variant+"_"+str(n_outputs)+".pickle", 'rb') as handle:
    data_val = pickle.load(handle)

with open("./dict_label_train_eksp"+variant+"_"+str(n_outputs)+".pickle", 'rb') as handle:
    train_labels = pickle.load(handle)

with open("./dict_label_val_eksp"+variant+"_"+str(n_outputs)+".pickle", 'rb') as handle:
    val_labels = pickle.load(handle)



def ssim(y_true, y_pred):
  return tf.reduce_mean(tf.image.ssim(y_true, y_pred, 1.0)) 

def psnr (y_true, y_pred):
  return tf.reduce_mean(tf.image.psnr(y_true, y_pred, 1.0))


# Load trained autoencoder
autoencoder=keras.models.load_model(autoencoder_model,custom_objects={"psnr":psnr, "ssim":ssim})


feature_csv=sorted(glob.glob(PATH_to_csvs+"/*.csv"))
feature_names=sorted(['answers','width', 'figure_type', 'rotation','height', 'fillColor']) #needed fot auxiliary training
dict_df={key:pd.read_csv(path, sep=';', header=None,decimal=',') for path,key in zip(feature_csv, feature_names)}

def unfitness_level(ID1, ID2): 
    """ Function to compute the similarity measure between candidate-answers within filled-in RPMs
    - this serves as lable in regression task (auxiliary training)"""
    num_task=int(ID1[:-6])
    num_cand1=int(ID1[-5])
    num_cand2=int(ID2[-5])
    score=0
    base=dict_df['figure_type'].iloc[0][num_cand1]
    other=dict_df['figure_type'].iloc[0][num_cand2]
    score += 0 if (base==other) else 1


    base=ImageColor.getcolor('#'+dict_df['fillColor'].iloc[0][num_cand1][-6:],"RGB")[0]
    other=ImageColor.getcolor('#'+dict_df['fillColor'].iloc[0][num_cand2][-6:],"RGB")[0]
    score += abs(base-other)/256 
   
    base=dict_df['width'].iloc[0][num_cand1]
    other=dict_df['width'].iloc[0][num_cand2]
    score+=abs(base-other)/50

    base=dict_df['height'].iloc[0][num_cand1]
    other=dict_df['height'].iloc[0][num_cand2]
    score+=abs(base-other)/50

    base=dict_df['rotation'].iloc[0][num_cand1]
    other=dict_df['rotation'].iloc[0][num_cand2]
    score+=min(abs(base%360-other%360) , 360 - abs(base%360-other%360))/180

    value=score/5
    return value

### Custom Data Generator
# (inspiration from https://stanford.edu/~shervine/blog/keras-how-to-generate-data-on-the-fly)
# - read data about answers features.
# - function to compute similarity between answers

class DataGenerator(keras.utils.Sequence):
    'Generates data for Keras'
    def __init__(self, list_IDs, labels, PATH, shuffle=False, with_aux_training=False, batch_size=100, dim=(150,150), n_channels=1,
                 n_classes=5):
        self.dim = dim
        self.batch_size = batch_size
        self.labels = labels
        self.list_IDs = list_IDs
        self.n_channels = n_channels
        self.with_aux_training=with_aux_training
        self.n_classes = n_classes
        self.shuffle = shuffle
        self.PATH=PATH
        self.on_epoch_end()


    def __len__(self):
        'Denotes the number of batches per epoch'
        return int(np.floor(len(self.list_IDs) / self.batch_size))

    def __getitem__(self, index):
        'Generate one batch of data'
        # Generate indexes of the batch
        indexes = self.indexes[index*self.batch_size:(index+1)*self.batch_size]

        # Find list of IDs
        list_IDs_temp = [self.list_IDs[k] for k in indexes]
        # Generate data
        X, y = self.__data_generation(list_IDs_temp, self.is_train)

        return X, y

    def on_epoch_end(self):
        'Updates indexes after each epoch'
        self.indexes = np.arange(len(self.list_IDs))
        if self.shuffle == True:
            np.random.shuffle(self.indexes)

    def __data_generation(self, list_IDs_temp, is_train):
        'Generates data containing batch_size samples' 
        X1 = np.empty((self.batch_size, *self.dim, self.n_channels))
        X2 = np.empty((self.batch_size, *self.dim, self.n_channels))
        y = np.empty((self.batch_size), dtype=int)
        y_aux = np.empty((self.batch_size), dtype=float)

        for i, ID in enumerate(list_IDs_temp):
            if self.n_channels==1:
              X1[i,] = rgb2gray(plt.imread(PATH + ID[0] )).reshape(150,150,1) 
              X2[i,] = rgb2gray(plt.imread(PATH + ID[1] )).reshape(150,150,1) 
            
            else:
              X1[i,] = plt.imread(PATH + ID[0] ) 
              X2[i,] = plt.imread(PATH + ID[1] )
            
            # Store class
            y[i] = self.labels[ID[0]+ID[1]]# 
            y_aux[i] = unfitness_level(ID[0], ID[1])

        if self.with_aux_training==True:
            return [X1, X2], [keras.utils.to_categorical(y, num_classes=self.n_classes) , y_aux]
        else:
            return [X1, X2], keras.utils.to_categorical(y, num_classes=self.n_classes)  



# Parameters for data loading
params = {'dim': (150,150),
          'batch_size': 100,
          'n_classes': n_outputs,
          'n_channels': 1}

# Generators
training_generator = DataGenerator(data_train, train_labels, PATH_train, shuffle=True, with_aux_training, **params)
validation_generator = DataGenerator(data_test, test_labels, PATH_val, shuffle=False, with_aux_training, **params)


# classifier architecture
shared_Conv2D=Conv2D(4, kernel_size=(2,2), strides=(2,2), activation='relu') 

inputA = Input(shape=(150,150,1))
inputB = Input(shape=(150,150,1))

# depending whether we want to freeze the trained autoencoder or not,
# we set below the approapriate logic value
counter=0
for layer in autoencoder.layers[:6]:
  layer.trainable=True
  layer._name=layer._name+str(counter)
  counter+=1

x=autoencoder.layers[1](inputA)
x=autoencoder.layers[2](x) 
x=autoencoder.layers[3](x)
x=shared_Conv2D(x)

x=Flatten()(x)
x = Model(inputs=inputA, outputs=x)


# the second branch opreates on the second input
y=autoencoder.layers[1](inputB)
y=autoencoder.layers[2](y) 
y=autoencoder.layers[3](y)

y=shared_Conv2D (y)

y=Flatten()(y)
y = Model(inputs=inputB, outputs=y)

# combine the output of the two branches
combined = concatenate([x.output, y.output])
z = Dense(40, activation="relu")(combined)
z = Dense(n_outputs, activation="softmax", name='classification')(z)

if with_aux_training:
    z_aux=Dense(1, activation="sigmoid",kernel_regularizer=regularizers.l1_l2(l1=1e-5, l2=1e-4), name='similarity')(combined) 
    model = Model(inputs=[x.input, y.input], outputs=[z, z_aux])
    losses = {
	"classification": "categorical_crossentropy",
	"similarity": "mse"
    }
    lossWeights = {"classification": 1.0, "similarity": 1.0}
    metrics={
  "classification": "accuracy",
	"similarity": "mse"
    }
    model.compile(optimizer='adam', loss=losses, loss_weights=lossWeights, metrics=metrics)  
    
else:
    model = Model(inputs=[x.input, y.input], outputs=z)
    losses="categorical_crossentropy"
    metrics="accuracy"
    model.compile(optimizer='adam', loss=losses, metrics=metrics)  

model.summary()



# Train model on dataset
history=model.fit_generator(generator=training_generator,epochs=n_epochs, 
                    validation_data=validation_generator, verbose=1,
                    use_multiprocessing=True,
                    workers=16) 


model.save("classifier_experiment"+variant+".h5")


import pickle

with open("history_class_exp"+variant+"_"+str(n_outputs)+".pickle", 'wb') as fp:
    pickle.dump(history.history, fp, protocol=pickle.HIGHEST_PROTOCOL)

# classifier performance on test set 
with open("./dict_data_test_eksp"+variant+"_"+str(n_outputs)+".pickle", 'rb') as handle:
    data_test = pickle.load(handle)

with open("./dict_label_test_eksp"+variant+"_"+str(n_outputs)+".pickle", 'rb') as handle:
    test_labels = pickle.load(handle)

test_generator = DataGenerator(data_test, test_labels, PATH_test, shuffle=False, with_aux_training, **params)

performance = model.evaluate_generator(test_generator)
np.savetxt(test_performance_file, performance)






#!/usr/bin/env python

# This code is used for autoencoder training.

import keras
import pandas as pd
from PIL import Image
import numpy as np
import matplotlib.pyplot as plt
import tensorflow as tf

np.random.seed(1)
tf.random.set_seed(1)

# need to be adjusted to one's needs
variant="01"
PATH_train= "./filled_in/experiment"+variant+"_train"
PATH_val="./filled_in/experiment"+variant+"_val"
PATH_val="./filled_in/experiment"+variant+"_test"
model_name="AE_experiment"+variant+".h5"
test_performance_file="AE_performance_exp"+variant+".txt"



from keras.models import Model, Sequential
from keras.layers import Dense, Conv2D, Dropout, BatchNormalization, Input, Reshape, Flatten, Conv2DTranspose, MaxPooling2D, Cropping2D, Upsampling
from keras.optimizers import adam

# encoder
inp = Input((150, 150,1))
e = Conv2D(32, (3, 3),strides=(2,2), activation='relu',padding='valid')(inp)
e = BatchNormalization()(e)
e = MaxPooling2D((2, 2), padding='same')(e)
e = Conv2D(16, (2, 2), strides=(2,2), activation='relu',padding='same')(e)
e = BatchNormalization()(e)
# decoder
d = Conv2DTranspose(16,(3, 3),strides=(2,2), activation='relu', padding='same')(e) 
d = BatchNormalization()(d)
d = Conv2DTranspose(32,(3, 3), strides=(2,2), activation='relu', padding='same')(d)
d = BatchNormalization()(d)
decoded = Conv2DTranspose(1,(3, 3), strides=(2,2), activation='sigmoid', padding='same')(d)
decoded = Cropping2D(cropping=((2, 0), (2, 0)), data_format=None)(decoded)
autoencoder = Model(inp, decoded)
autoencoder.summary()


def ssim(y_true, y_pred):
  return tf.reduce_mean(tf.image.ssim(y_true, y_pred, 1.0)) 

def psnr (y_true, y_pred):
  return tf.reduce_mean(tf.image.psnr(y_true, y_pred, 1.0))


autoencoder.compile(optimizer='adam', loss='mse', metrics=[ssim, psnr])


from keras.preprocessing.image import ImageDataGenerator

batch_size = 100

train_datagen = ImageDataGenerator(
        rescale=1./255)

val_datagen = ImageDataGenerator(rescale=1./255)

train_generator = train_datagen.flow_from_directory(
        PATH_train,  
        target_size=(img_shape, img_shape),  
        batch_size=batch_size,
        class_mode='input',
        color_mode='grayscale')  

validation_generator = val_datagen.flow_from_directory(
        PATH_val,
        target_size=(img_shape, img_shape),
        batch_size=batch_size,
        class_mode='input',
        color_mode='grayscale')

callback = tf.keras.callbacks.EarlyStopping(monitor='val_loss', patience=5)

history=autoencoder.fit_generator(generator=train_generator,
                    validation_data=validation_generator, epochs=20, 
                    callback=[callback],
                    use_multiprocessing=True,
                    workers=12)



autoencoder.save(model_name)


# evaluation on test set
test_datagen = ImageDataGenerator(rescale=1./255)

test_generator = test_datagen.flow_from_directory(
        PATH_test,
        target_size=(img_shape, img_shape),
        batch_size=batch_size,
        class_mode='input',
        color_mode='grayscale')
        
performance=autoencoder.evaluate_generator(test_generator)

np.savetxt(test_performance_file, performance)



#!/usr/bin/env python

# The code of scoring module that aggregates the results of the duels and outputs the final decision of the system.
# Two approaches are implemented: "probability sum" and "winner frequency"

import keras
import pandas as pd
from PIL import Image
import numpy as np
import matplotlib.pyplot as plt
import os 
import tensorflow as tf
from keras.models import load_model
from skimage.color import rgb2gray


# variables
variant="01"
n_outputs=2
with_aux_training=False
RPM_shape=150
PATH_test='./filled_in/experiment'+variant+"_test/test/"
classifier_model="class_experiment"+variant+str(n_outputs)+".h5"
test_performance_file="class_performance_exp"+variant+"_"+str(n_outputs)+".txt"
PATH_to_csv='./data/experiment'+variant+"/answers.csv"
n_candidates=5
n_pairs=10

def ssim(y_true, y_pred):
  return tf.reduce_mean(tf.image.ssim(y_true, y_pred, 1.0))

def psnr (y_true, y_pred):
  return tf.reduce_mean(tf.image.psnr(y_true, y_pred, 1.0))

classifier=keras.models.load_model(classifier_model,custom_objects={"psnr":psnr, "ssim":ssim})

file_names = [f for f in os.listdir(PATH_test) if os.path.isfile(os.path.join(PATH_test, f))]
file_names.sort()


def generate_predictions(index1,index2, file_list, path):
    img1=rgb2gray(plt.imread(path+file_list[index1])).reshape(1,RPM_shape,RPM_shape,1)
    img2=rgb2gray(plt.imread(path+file_list[index2])).reshape(1,RPM_shape,RPM_shape,1)
    pred=model.predict([img1, img2])[0]
    if with_aux_training:
      return pred[0]
    else:
      return pred

n_files=len(file_names)
indexes=np.arange(0,n_files,n_candidates)
np.random.shuffle(indexes)

num_tasks=int(n_files/n_candidates)
num_examples=n_pairs*num_tasks


if n_outputs==2:
  res=np.zeros((num_examples,5))
else:
  res=np.zeros((num_examples,6))
  
  
counter=0

for i in indexes:
  for j in range (n_candidates):
    for k in range (j+1, n_candidates):
      pred = generate_predictions(i+j,i+k, file_names, file_path)
      if n_outputs==2:
        res[counter,:]=[int(file_names[i].split('_')[0]), int(j),int(k),pred[0], pred[1]]
      else:
        res[counter,:]=[int(file_names[i].split('_')[0]), int(j),int(k),pred[0],pred[1],pred[2]]
      counter+=1


if n_outputs==3:
    sum_pred=np.zeros((num_tasks,6))
    n_wins= np.zeros((num_tasks,6))
    for it in range(0,int(res.shape[0]),n_pairs):
      task_number=res[it,0]
      n_wins[int(it/n_pairs),n_candidates]=task_number
      sum_pred[int(it/n_pairs),n_candidates]=task_number
      for i in range (n_pairs):
        if np.argmax(res[it+i,3:6])==0:
          n_wins[int(it/n_pairs), int(res[it+i,1])] +=1
        elif np.argmax(res[it+i,3:6])==1:
          n_wins[int(it/n_pairs),int(res[it+i,2])] +=1
        sum_pred[int(it/n_pairs),int(res[it+i,1])] += res[it+i,3]
        sum_pred[int(it/n_pairs),int(res[it+i,2])]+=res[it+i,4]
else:
    sum_pred=np.zeros((num_tasks,6))
    n_wins= np.zeros((num_tasks,6))
    for it in range(0,int(res.shape[0]),n_pairs):
      task_number=res[it,0]
      n_wins[int(it/n_pairs),n_candidates]=task_number
      sum_pred[int(it/n_pairs),n_candidates]=task_number
      for i in range (n_pairs):
        if np.argmax(res[it+i,3:])==0:
          n_wins[int(it/n_pairs), int(res[it+i,1])] +=1
        elif np.argmax(res[it+i,3:])==1:
          n_wins[int(it/n_pairs),int(res[it+i,2])] +=1
        sum_pred[int(it/n_pairs),int(res[it+i,1])] += res[it+i,3]
        sum_pred[int(it/n_pairs),int(res[it+i,2])]+=res[it+i,4]
        
  
correct_sum_pred=0
correct_n_wins=0
correct_ans=pd.read_csv(PATH_to_csv, header=None)

for i in range (0,num_tasks):
  if np.argmax(sum_pred[i,0:-1])==correct_ans.iloc[int(sum_pred[i,-1])-1,0]:
    correct_sum_pred+=1
  if np.argmax(n_wins[i,0:-1])== correct_ans.iloc[int(n_wins[i,-1])-1,0]:
    correct_n_wins+=1

#output accuracy in the following approaches: "probability sum" and "winner frequency"
np.savetxt("final_performance_eksp"+variant+"_"+str(n_outputs)+".txt", np.array([correct_sum_pred/num_tasks,correct_n_wins/num_tasks]))



#!/usr/bin/env python

# This code is used for the pre-processing stage where the input files for the duels are created.
# Here, Raven's Progressive Matrices (RPMs) are filled in with candidate answers. 

import matplotlib.pyplot as plt
import numpy as np
from PIL import Image
from skimage.color import rgba2rgb, rgb2gray
import os
import glob
import re

# variables
img_shape=150
panel_shape=int(img_shape/3)
n_candidates=5

# path to direcotry with original test and answers images 
#(generated using the procedure in dataset_generator directory)

# to be adjusted to one's needs
PATH_to_data="./data/experiment01_val"

# path to directory where the filled-in RPMs should be saved
# the particular folder structure is needed as later keras ImageDataGenerator is going to be used
# e.g. inside "./filled_in/experiment01_val/" there must be an additional folder "val" where images are going to be stored
# later in ImageDataGenerator, we will indicate path to "./filled_in/experiment01_val/" where there will be only one directory

# to be adjusted to one's needs
PATH_to_target="./filled_in/experiment01_val/val"


def read_img (path):
	""" Function to read image using PIL package """
    image=Image.open(path)
    return np.asarray(image)



def fill_in (ans, test, number):
    """ Function to paste the candidate answer to RPM.
        As a result, from one test question, there are 
        as many images created and saved as the number of candidate answers"""

    for i in range(n_candidates):
        filled=np.copy(test)
        filled[-panel_shape:,-panel_shape:]=ans[:,i*panel_shape:(i+1)*panel_shape]
        filled_gray=rgb2gray(rgba2rgb(filled))

        # in the name of the file saved, there is information about the task number and the index of candidate answer
        plt.imsave(PATH_to_target+"/"+str(number)+"_"+str(i)+".jpg", filled_gray, cmap='gray')



def get_task_num(file_name):
    """ Function to extract the task number from the path to file
    Note: that the start variable may need to be changed depending on operating system"""
  s=os.path.basename(file_name)
  start='\\'
  end='_'
  return s[s.find(start)+len(start):s.rfind(end)]


def generate_input_data():
    """  Function that iterates over the files in the PATH_to_data and calls fill_in function """

    if not os.path.exists(PATH_to_target):
        os.makedirs(PATH_to_target)
		
    for i in range(len(file_list)):
        # the condition below is due to the fact that there are two files related to one question: test + answer
        if i%2==0:
            ans=read_img(file_list[i])
        else:
            test=read_img(file_list[i])
            org_num=int(get_task_num(file_list[i]))
            fill_in(ans, test,org_num+1)



file_list = glob.glob(PATH_to_data+"/*.png")
file_list=sorted(file_list)

generate_input_data()

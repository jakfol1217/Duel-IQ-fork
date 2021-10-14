#!/usr/bin/env python

# This code is used for the pre-processing stage where the input files for the duels are created.
# Here, Raven's Progressive Matrices (RPMs) are filled in with candidate answers. 

import matplotlib.pyplot as plt
import numpy as np
from PIL import Image
from skimage.color import rgba2rgb, rgb2gray
from os import listdir
import glob
from os.path import isfile, join
import re
from os import path



# variables
img_shape=150
panel_shape=int(img_shape/3)
n_candidates=5

# path do folder where the original test and answers images are (generated according to the procedure in ...)
# to be adjusted to one's needs
PATH_to_data="./data/experiment01_val"

# path do folder where the original the filled-in RPMs should be saved (preapre the approprate folder structure beforehand)
# the particular foder structre is needed as later keras ImageDataGenerator is ging to be used
# e.g. inside "./filled_in/experiment01_val/" there must be a folder "val" where images are going to be stored
# later in ImageDataGenerator we will indicate path to "./filled_in/experiment01_val/" where there will be only one folder
# to be adjusted to one's needs
PATH_to_target="./filled_in/experiment01_val/val"


def read_img (path):
	""" Function to read image using PIL package """
    image=Image.open(path)
    return np.asarray(image)



def fill_in (ans, test, number):
    """ Function to paste the candidate answer to RPM
        as the result from one test question there are 
        as many images created and saved as the number of candidate answers"""
    tmp=np.zeros((n_candidates, img_shape*img_shape))


    for i in range(n_candidates):
        filled=np.copy(test)
        filled[-panel_shape:,-panel_shape:]=ans[:,i*panel_shape:(i+1)*panel_shape]
        filled_gray=rgb2gray(rgba2rgb(filled))

        # in the name of the file save there is information about the task number and the index of candidate answer
        plt.imsave(PATH_to_target+"/"+str(number)+"_"+str(i)+".jpg", filled_gray, cmap='gray')



def get_task_num(file_name):
    """Function to extract the task number from the path to file
    Note: that the start variable may need to be changed depending on operating system"""
  s=path.basename(file_name)
  start='\\'
  end='_'
  return s[s.find(start)+len(start):s.rfind(end)]


def generate_input_data():
    """  Function that iterates over the files in the PATH_to_data and call fill_in function """
    for i in range(len(file_list)):
        # the condition below is due to the fact that there are to files related to one question: test + answer
        if i%2==0:
            ans=read_img(file_list[i])
        else:
            test=read_img(file_list[i])
            org_num=int(get_task_num(file_list[i]))
            fill_in(ans, test,org_num+1)



file_list = glob.glob(PATH_to_data+"/*.png")
file_list=sorted(file_list)

generate_input_data()


#!/usr/bin/env python

# The code to generate list of pairs for the duels
# (later used in custom Data Generator).
# Two pairing schemes available: winner_in and any_in.
# You can specify the scheme by the parameter "n_outputs" in create_dict function:
# 2- for winner_in scheme
# 3 - any_in (draw is possible)

import pandas as pd
from PIL import Image
import numpy as np
import matplotlib.pyplot as plt
import os
import pickle

#variables
variant="01"
dataset="train"
PATH_to_data="./filled_in/experiment"+variant+"_"+dataset+"/"+dataset+"/"
PATH_to_answers_csv="./eksperyment"+variant+"_answers.csv"
n_outputs=2


def create_label_2outputs(i, j, correct_ans):
    """
    Function to pair up filled-in RPMs and add appropriate label for the duel in winner_in scenario.
    First, it is checked whether the two input filled-in RPMs are from the same task and 
    there is a correct answer within the input pair. 
    If any of the security checks fails then None value is returned.
    If the conditions are met, the mini dataframe is created where left column refers to first input RPM
    and the right to the second input. The 1 value is inserted in the column corresponding to answer winning the duel.
    """
    df=pd.DataFrame({"left" :[i], "right": [j]})
   
    if i[:-6]!=j[:-6] and i[-5]==j[-5]:
      return None

    if correct_ans.iloc[int(i[:-6])-1][0]==int(i[-5]):  
      l=1
      r=0
    elif correct_ans.iloc[int(i[:-6])-1][0]==int(j[-5]):
      l=0
      r=1
    else: 
      return None
    df=df.append({"left":l,"right":r}, ignore_index = True)
    return df

def swapping_2outputs (df):
    """ Function to randomly swap columns in mini dataframe returned in  
    create_label_2outputs function (to balance the overall dataset)"""
    rand=np.random.randint(0,2)
    if rand ==1:
        columns_titles = ["right", "left"]
        df=df.reindex(columns=columns_titles)
    return df


def create_label_3outputs(i, j, correct_ans, num_m):
    """
    Funtion similar to create_label_2outputs but for the any_in pairing scheme.
    The only difference in comparison to create_label_2outputs, is an additional variable (num_m) that
    counts the number of created duel pairs where the correct answer is a draw with particular IQ question.
    If the number of such pairs within one IQ question is bigger than a threshold, 
    the pairs with "middle" answer (meaning a draw) are
    not generated any more for particular IQ question. The goal is to prevent class imbalance.
    """
    
    df=pd.DataFrame({"left" :[i], "right": [j], "middle":'m'})

    if i[:-6]!=j[:-6] and i[-5]==j[-5]:
      return None, num_m
    if correct_ans.iloc[int(i[:-6])-1][0]==int(i[-5]):  
      l=1
      r=0
      m=0
    elif correct_ans.iloc[int(i[:-6])-1][0]==int(j[-5]):
      l=0
      r=1
      m=0
    else:
      if num_m>1:
        return None, num_m
      l=0
      r=0
      m=1
      num_m+=1
      

    df=df.append({"left":l,"right":r, "middle": m}, ignore_index = True)
    return df, num_m


def swapping_3outputs (df):
  rand=np.random.randint(0,2)
  if rand ==1:
    columns_titles = ["right", "left", "middle"]
    df=df.reindex(columns=columns_titles)
  return df


def fill_dict(df, dict_data, dict_label):
    """ Function that transforms df into a dictionary and a list needed later in custom Data Generator for classifier training"""
    
    # get label
    label = np.argmax(df.iloc[-1,:])  
    dict_label[df.iloc[0,0]+df.iloc[0,1]] =label
    dict_data.append([df.iloc[0,0],df.iloc[0,1]])  
    return dict_data, dict_label


def create_dict(path_images, path_answers, n_outputs):
  """ The main function that integrates all functions."""

  correct_ans=pd.read_csv(path_answers, header=None)
  dict_label={}
  dict_data=[]
  counter=0
  PATH_train=path_images 
  files = [f for f in os.listdir(PATH_train) if os.isfile(os.join(PATH_train, f))] 
  sorted_files=sorted(files)

  num_task=0
  num_m=0
  for i in range(len(sorted_files)):
    
    # if the analysed RPM is filled-in with candidate answer of index=4 then there is no possibility to create new pair
    # that's why "continue" is used - the algoritm analyses the next IQ question
    if (i==num_task*5+4):
      num_task+=1
      num_m=0
      continue
    counter+=1

    # the loop adds next filled-in RPM to the pair 
    for j in range (i+1, i+6-counter):
      # if condition below is met, it means that the last pair from the task is analysed 
      # and the counter should be set to zero as we move to next question 
      if counter==4:
        counter=0
  
      if n_outputs==2:
        df=create_label_2outputs(sorted_files[i], sorted_files[j], correct_ans)
        #if the condition is met, it means that there is no correct answer within the pair
        if df is None:
          continue
        df=swapping_2outputs(df)

      
      if n_outputs==3:
        df, num_m=create_label_3outputs(sorted_files[i], sorted_files[j], correct_ans, num_m)
        if df is None:
          continue
        df=swapping_3outputs(df)

      dict_data, dict_label=fill_dict(df,  dict_data, dict_label)

  return dict_data, dict_label



dict_data, dict_label=create_dict(PATH_to_data,PATH_to_answers_csv, n_outputs) 


# saving dicts to pickle
with open("./dict_data_"+dataset+"_eksp"+variant+"_"+str(n_outputs)+".pickle",'wb') as handle:
    pickle.dump(dict_data, handle, protocol=pickle.HIGHEST_PROTOCOL)

with open("./dict_label_"+dataset+"_eksp"+variant+"_"+str(n_outputs)+".pickle",'wb') as handle:
    pickle.dump(dict_label, handle, protocol=pickle.HIGHEST_PROTOCOL)


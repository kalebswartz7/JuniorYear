import sys 
import re

# Author: Kaleb Swartz #
# This program reads in two files, one containing the blueprint for which the record file should be created and the other containing
#the data for which the record file should be populated. It then creates a new file with the data organized in the specified manner.

tmpName = sys.argv[2]
dataList = []
filledList ={}
dataToTransfer = []
tempDictionary = {}
#Function that finds all tags that must be replaced in the tmp file and adds them to a dictionary 
def findTag(line):
   for i in range(len(line)-1):
       data = "<<"
       if (line[i] is "<" and line[i+1] is "<"):
           while (line[i+2] is not ">"):
               data += line[i+2]
               i = i + 1
       else:
           i = i+1
       if (data is not "<<"):
           data += ">>"
           dataList.append(data) 

#Function that finds all tags in the tsv file and puts them in a dictionary with their corresponsing order number 
def getData(line):
    orderNumber = 0
    i = 0
    while (i < len(line)-1):
        data = ""
        while not (line[i].isspace()):
            data += line[i]
            i = i + 1
        i = i + 1
        if (data is not ""):
            filledList.update({orderNumber:data})
            orderNumber = orderNumber + 1

#Function that separates the string by tabs and puts the values into a list
def sendData(line):
    return re.split(r'\t+', line.rstrip('/n'))

def createNewFile(d, fileName):
    with open(tmpName, "rt") as input, open(fileName + ".txt", "w") as output:
        i = 0
        for line in input:
            findTag(line)
            while (i < len(dataList)):              
                replaceString = dataList[i]
                replaceString2 = replaceString.replace("<<", "")
                replaceString2 = replaceString2.replace(">>", "")
                if (replaceString in line):
                   line = line.replace(replaceString, d[replaceString2])
                i = i + 1
            output.write(line)
                

tsvName = sys.argv[1]
tsvFile = open(tsvName, "r")

#Opens the tsv file and calls the getData function for line 1, and the sendData function for all other lines
#Creates a second dictionary that is a copy of filledList. Checks if amount of spaces (key) is equal to
#amount of spaces in data to transfer, and if it is sets the tempDictionary key to equal the correct 
#data to transfer
with open(tsvName, "rt") as input:
    lineNum = 1
    for line in input:
        if (lineNum is 1):
            getData(line)
            lineNum = lineNum + 1
        else:
            tempDictionary = filledList.copy()
            dataToTransfer = sendData(line)
            elementCount = 0
            while (elementCount < len(dataToTransfer)):
                for key in tempDictionary:
                    if (key is elementCount):
                        value = tempDictionary.get(key)
                        tempDictionary[key] = dataToTransfer[elementCount]
                        tempDictionary[value] = tempDictionary[key]
                        del tempDictionary[key]
                elementCount = elementCount + 1
            lineNum = lineNum + 1
            newFileName = tempDictionary['ID']
            createNewFile(tempDictionary, newFileName)

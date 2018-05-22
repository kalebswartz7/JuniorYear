// piglatin.cpp
// Using C++ to convert input string to piglatin version. Specifically supposed to use pointers in this assignment.

#include <iostream>
#include <cstring>
#include <stdio.h>

using namespace std;

const int MAX = 43;

char* ToPigLatin(char* word);

bool isVowel(char letter, int isY);

int main()
{
    // creation of 5 character strings, each length MAX
    char word[5][MAX];
    int i;                // loop counter
    
    cout << "Input 5 words: ";
    for (i = 0; i < 5; i++)
        cin >> word[i];
    
    cout << "\nPig Latin version of the 5 words:\n";
    for (i = 0; i < 3; i++)
    {
        ToPigLatin(word[i]);
        cout << word[i] << ' ';
    }
    // Note that the above outputs illustrate that the original
    //  string itself has been converted.  The outputs below illustrate
    //  that a pointer to this string is also being returned from the
    //  function.
    
    cout << ToPigLatin(word[3]) << ' '
    << ToPigLatin(word[4]) << '\n';
    
    return 0;
}

char* ToPigLatin (char* word) {
    
    size_t size = strlen(word);
    char *returnChar = new char[size + 39];
    strcpy(returnChar, word);
    
    if (isVowel(word[0], 1)) {
        returnChar[size] = 'w';
        returnChar[size + 1] = 'a';
        returnChar[size + 2] = 'y';
        returnChar[size + 3] = '\0';
    }
    else {
        char *vowelChar = new char [size + 39];
        char *consChar = new char [size + 39];
        bool capitalize;
        bool next = true;
        if (isupper(word[0])) {
            capitalize = true;
        }
        for (int i = 0; i < size; i++) {
            if (i == 0 && word[i] == 'y') {
                consChar[i] = tolower(word[i]);
                bool next = false;
            }
            else if (!isVowel(word[i], 0) && next) {
                consChar[i] = tolower(word[i]);
            }
            else {
                int position = 0;
                for (int j = i; j < size; j++) {
                    if (position == 0 && capitalize) {
                        vowelChar[position] = toupper(word[j]);
                    }
                    else {
                        vowelChar[position] = (word[j]);

                    }
                    position++;
                }
                i = size;
            }
        }
        size_t sizeNew = strlen(consChar);
        size_t sizeVow = strlen(vowelChar);
        for (int j = 0; j < sizeNew; j++) {
            vowelChar[sizeVow + j] = consChar[j];
        }
        vowelChar[sizeVow + sizeNew] = 'a';
        vowelChar[sizeVow + sizeNew + 1] = 'y';
        vowelChar[sizeVow + sizeNew + 2] = '\0';
        strcpy(returnChar, vowelChar);
        delete[] vowelChar;
        delete[] consChar;
    }
    
    strcpy(word, returnChar);
    delete[] returnChar;

    char *pointer = word;
    return pointer;
}

bool isVowel(char letter, int countY) {
    letter = tolower(letter);
    if (countY == 0) {
        return letter == 'a'|| letter == 'e' || letter == 'i' || letter == 'o' || letter == 'u' || letter == 'y';

    }
    else return letter == 'a'|| letter == 'e' || letter == 'i' || letter == 'o' || letter == 'u';

}

// Write your definition of the ToPigLatin function here


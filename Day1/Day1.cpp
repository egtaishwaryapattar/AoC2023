#include <iostream>
#include <vector>
#include <fstream>
#include <string>

using namespace std;

// function prototype
int getFirstNumber(string);
int getLastNumber(string);
bool isNumber(char);
int isSpelledNumber(string, int);

int main()
{
    int sum = 0;
    std::ifstream file("PuzzleInput1.txt");

    std::string line;
    while (getline(file, line)) {
        int firstDigit = getFirstNumber(line);
        int lastDigit = getLastNumber(line);

        int value = firstDigit * 10 + lastDigit;
        std::cout << "value: " << line << " = " << value << endl;
        sum += value;
    }

    std::cout << "Answer is: " << sum;
    return 0;
}

// defining functions
int getFirstNumber(string line) {
    for(int i = 0; i < line.size(); i++) {
        char c = line[i];
        if (isNumber(c)) {
            return (int)c - 48; // subtract the ASCII value for 0 to get value of the digit
        }

        int digit = isSpelledNumber(line, i);
        if (digit != 0)
        {
            return digit;
        }
    }

    //throw exception("not found start");
    return 0;
}

int getLastNumber(string line) {
    for(int i = line.size() - 1; i >= 0; i--) {
        char c = line[i];
        if (isNumber(c)) {
            return (int)c - 48; // subtract the ASCII value for 0 to get value of the digit
        }

        int digit = isSpelledNumber(line, i);
        if (digit != 0)
        {
            return digit;
        }
    }
    //throw exception("not found end");
    return 0;
}

// values 1-9
bool isNumber(char c)
{
    return (c == '1' 
        || c == '2'
        || c == '3'
        || c == '4'
        || c == '5'
        || c == '6'
        || c == '7'
        || c == '8'
        || c == '9');
}

// returns 0 is not a digit. Only looking for spelled numbers for 1-9
int isSpelledNumber(string line, int index) 
{
    if (line.substr(index, 3) == "one") return 1;
    if (line.substr(index, 3) == "two") return 2;
    if (line.substr(index, 5) == "three") return 3;
    if (line.substr(index, 4) == "four") return 4;
    if (line.substr(index, 4) == "five") return 5;
    if (line.substr(index, 3) == "six") return 6;
    if (line.substr(index, 5) == "seven") return 7;
    if (line.substr(index, 5) == "eight") return 8;
    if (line.substr(index, 4) == "nine") return 9;
    
    // if no number is found, return 0
    return 0;
}
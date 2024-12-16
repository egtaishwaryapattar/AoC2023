#include <iostream>
#include <map>
#include <string>
#include <fstream>
#include <tuple>
#include <vector>

using namespace std;

// function prototype
void Part1(string filename, string instructions);
void Part2(string filename, string instructions);
map<string, tuple<string,string>> GetMap(string filename);
int FindPath1(string instructions, map<string, tuple<string,string>> map);
int FindPath2(string instructions, map<string, tuple<string,string>> map, vector<string> positions);
vector<string> GetStartingPoints(map<string, tuple<string,string>> map);
bool IsEndReached(vector<string> positions);
int FindGhostPath(string instructions, map<string, tuple<string,string>> map, string startPos);

int main() 
{
    // get file
    string filename = "PuzzleFile.txt";
    std::ifstream file(filename);
    std::string line;

    // get instructions from the file - they exist in the first line
    string instructions;
    getline(file, instructions);

    Part2(filename, instructions);
    return 0;
}

// define functions
void Part1(string filename, string instructions)
{
    map<string, tuple<string,string>> map = GetMap(filename);
    int numSteps = FindPath1(instructions, map);
    cout << "Number of steps from AAA to ZZZ: " << numSteps <<endl;
}

void Part2(string filename, string instructions)
{
    map<string, tuple<string,string>> map = GetMap(filename);
    auto startingPoints = GetStartingPoints(map);

    for (int i = 0; i < startingPoints.size(); i++)
    {
        cout << "--------------Ghost " << i << "---------------" << endl;
        int stepCount = FindGhostPath(instructions, map, startingPoints[i]);
        cout << "Steps to end point: " << stepCount << endl;

        cout << "Ghost " << i << " steps : " << stepCount << endl;
    }
    
    //int numSteps = FindPath2(instructions, map, startingPoints);

    //cout << "Number of steps from start to end for all ghosts: " << numSteps <<endl;
}

map<string, tuple<string,string>> GetMap(string filename)
{
    std::ifstream file(filename);
    std::string line;
    map<string, tuple<string,string>> map;
    int nodeLength = 3;

    while (getline(file, line))
    {
        // make sure the line in a connections line - can identify with the '='
        if (line.find('=') != string::npos)
        {
            string key = line.substr(0, nodeLength);
            string value1 = line.substr(7, nodeLength);
            string value2 = line.substr(12, nodeLength);

            map[key] = {value1, value2};
        }
    }

    return map;
}

int FindPath1(string instructions, map<string, tuple<string,string>> map)
{
    // find start position - AAA
    auto search = map.find("AAA");
    int numSteps = 0;
    int iter = 0;

    while (search->first != "ZZZ")
    {
        if (iter < instructions.size())
        {
            cout << search->first << endl;
            // iterator is smaller than the size of the instructions string so we can use it
            string nextKey = instructions[iter] == 'L'
                ? get<0>(search->second)
                : get<1>(search->second);

            search = map.find(nextKey);
            numSteps++;
            iter ++;
        }
        else
        {
            iter = 0;
        }
    }
    return numSteps;
}

int FindGhostPath(string instructions, map<string, tuple<string,string>> map, string startPos)
{
    auto search = map.find(startPos);
    int numSteps = 0;
    int iter = 0;
    int numEndpointsReached = 0;
    bool endpointFound = false;

    while (!endpointFound) 
    {
        //cout << search->first << endl;
        // iterator is smaller than the size of the instructions string so we can use it
        string nextKey = instructions[iter] == 'L'
            ? get<0>(search->second)
            : get<1>(search->second);

        if (nextKey[2] == 'Z') // last character is Z
        {
            numEndpointsReached++;
            endpointFound = true;

        }
        search = map.find(nextKey);

        numSteps++;
        iter++;

        // reset iterator if we get to end of instructions
        if (iter >= instructions.size())
        {
            iter = 0;
        }
    }

    // get the next step after the end to see the loop
    // cout << search->first << endl;
    // string nextKey = instructions[iter] == 'L'
    //     ? get<0>(search->second)
    //     : get<1>(search->second);
    // search = map.find(nextKey);
    // cout << search->first << endl;

    return numSteps;
}

int FindPath2(string instructions, map<string, tuple<string,string>> map, vector<string> positions)
{
    int numSteps = 0;
    int iter = 0;

    while (!IsEndReached(positions))
    {
        if (iter < instructions.size())
        {
            // iterator is smaller than the size of the instructions string so we can use it
            for (int i = 0; i < positions.size(); i++)
            {
                auto currentPos = map.find(positions[i]);
                string nextKey = instructions[iter] == 'L'
                ? get<0>(currentPos->second)
                : get<1>(currentPos->second);

                // replace the position
                positions[i] = nextKey;
            }
            
            numSteps++;
            iter ++;
            //cout << "Num steps done: " << numSteps << endl;
        }
        else
        {
            iter = 0;
        }
    }
    return numSteps;
}

vector<string> GetStartingPoints(map<string, tuple<string,string>> map)
{
    vector<string> startingPoints;
    auto it = map.begin();

    while (it != map.end()) 
    { 
        if ((it->first)[2] == 'A') // last of the 3 letters in the key should end in 'A' to be a starting point
        {
            startingPoints.push_back(it->first);
        } 
        it++;
    } 
    cout << "Num ghost starting positions: " << startingPoints.size() << endl;
    return startingPoints;
}

bool IsEndReached(vector<string> positions)
{
    bool endReached = true;
    for (auto position : positions)
    {
        if ((position)[2] != 'Z')
        {
            return false;
        }
    }
    return true;
}

#include <iostream>
#include <vector>
#include <fstream>
#include <string>
#include <map>
#include <algorithm>
#include <functional>


using namespace std;

enum HandType
{
    HighCard,
    OnePair,
    TwoPair,
    ThreeOfAKind,
    FullHouse,
    FourOfAKind,
    FiveOfAKind
};

enum CardType : int
{
    Joker = 0,
    Two = 2,
    Three,
    Four,
    Five, 
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    J,
    Q,
    K, 
    A 
};

const int kNumCardsInHand = 5;

struct Hand
{
    CardType cards[kNumCardsInHand];
    int bid;
    HandType handType;
};

// function prototypes
vector<Hand> GetHands(string filename, bool joker);
CardType GetCardType(char c);
std::map<CardType, int> GetCardMap(CardType* cards);
HandType GetHandType(std::map<CardType, int> cardMap);
HandType GetHandTypeWithJoker(std::map<CardType, int> cardMap);
bool CompareHands(Hand hand1, Hand hand2);
bool CompareHandsWithJoker(Hand hand1, Hand hand2);
void PrintHand(Hand hand);

int main()
{
    string filename = "PuzzleFile.txt";
    bool usingJoker = true; // for Part 1, set to false. Part 2, set to true

    vector<Hand> hands = GetHands(filename, usingJoker);

    if (usingJoker)
    {
        sort(hands.begin(), hands.end(), CompareHandsWithJoker);
    }
    else
    { 
        sort(hands.begin(), hands.end(), CompareHands);
    }
    
    // calculate the winnings
    int totalWinnings = 0;

    for (int i = 0; i < hands.size(); i++)
    {
        PrintHand(hands[i]);
        int rank = i + 1;
        int winnings = rank * hands[i].bid;
        //cout << hands[i].bid << " * " << rank << " = " << winnings << endl;
        totalWinnings += winnings;
    }

    cout << "Total Winnings = " << totalWinnings << endl;
    char c;
    cin >> c;
    return 0;
}

vector<Hand> GetHands(string filename, bool joker)
{
    std::ifstream file(filename);
    vector<Hand> hands;
    std::string line;
    
    while (getline(file, line)) {

        // split the line to get the cards and bid as strings
        int deliminatorIndex = line.find(" ");
        string cardsAsString = line.substr(0, deliminatorIndex);
        string bidAsString = line.substr(deliminatorIndex + 1, line.size() - 1);

        if (cardsAsString.size() != kNumCardsInHand) throw std::invalid_argument("Num cards invalid");

        // create a Hand stuct
        Hand hand; 
        hand.bid = atoi(bidAsString.c_str()); // need to convert string to const char* using c_str()
        for (int i = 0; i < cardsAsString.size(); i++)
        {
            hand.cards[i] = GetCardType(cardsAsString[i]);
        }
 
        // sort cards into a map with number of cards for each card type
        std::map<CardType, int> cardMap = GetCardMap(hand.cards);
        hand.handType = joker 
            ? GetHandTypeWithJoker(cardMap) 
            : GetHandType(cardMap);
        
        // add to vector
        hands.push_back(hand);
    }

    return hands;
}

CardType GetCardType(char c)
{
    if (c == '2') return CardType::Two;
    if (c == '3') return CardType::Three;
    if (c == '4') return CardType::Four;
    if (c == '5') return CardType::Five;
    if (c == '6') return CardType::Six;
    if (c == '7') return CardType::Seven;
    if (c == '8') return CardType::Eight;
    if (c == '9') return CardType::Nine;
    if (c == 'T') return CardType::Ten;
    if (c == 'J') return CardType::J;
    if (c == 'Q') return CardType::Q;
    if (c == 'K') return CardType::K;
    if (c == 'A') return CardType::A;

    throw std::invalid_argument("card is not an expected card type");
}

std::map<CardType, int> GetCardMap(CardType* cards)
{
    std::map<CardType, int> cardMap;
    for (int i = 0; i < kNumCardsInHand; i++)
    {
        CardType card = cards[i];
        auto search = cardMap.find(card);

        if (search != cardMap.end())
        {
            // exists in the map already. Increment the count
            search->second++;
        }
        else
        {
            // doesn't exist in the map so add
            cardMap[card] = 1;
        }
    }
    return cardMap;
}

HandType GetHandType(std::map<CardType, int> cardMap)
{
    // from map, determine the hand type
    if (cardMap.size() == 1) return HandType::FiveOfAKind;
    if (cardMap.size() == 2)
    {
        // can be four of a kind or full house
        int value = cardMap.begin()->second;
        if (value == 4 || value == 1) return HandType::FourOfAKind;
        if (value == 3 || value == 2) return HandType::FullHouse;
        throw std::invalid_argument("Somehow there are 2 types but it's not Four of a Kind or Full House...");
    }
    if (cardMap.size() == 3)
    {
        // can be either Three of a Kind or Two Pair
        auto it = cardMap.begin();
        while (it != cardMap.end()) 
        { 
            if (it->second == 3) return HandType::ThreeOfAKind;
            if (it->second == 2) return HandType::TwoPair;
            it++; 
            // NOTE: ignoring a value of 1 because it doesn't help us determine which Hand Type it is
        } 
        throw std::invalid_argument("Somehow there are 3 types but it's not Three of a Kind or Two Pair...");
    }
    if (cardMap.size() == 4) return HandType::OnePair;
    if (cardMap.size() == 5) return HandType::HighCard;
    throw std::invalid_argument("cards do not correspond to an expected hand type");
}

HandType GetHandTypeWithJoker(std::map<CardType, int> cardMap)
{
    // identify if joker is present
    auto search = cardMap.find(CardType::J);

    if (search == cardMap.end())
    {
        // joker not found
        return GetHandType(cardMap);
    }

    // joker found so treat the card map differently
    int numJokers = search->second;

    // from map, determine the hand type based on number of unique card types
    if (cardMap.size() == 1) return HandType::FiveOfAKind; // jokers make up all 5
    if (cardMap.size() == 2) return HandType::FiveOfAKind; // jokers would mimic the other card type and make a full set of 5
    if (cardMap.size() == 3)
    {
        // determine if the original set is Three of a Kind or Two Pair
        auto it = cardMap.begin();
        while (it != cardMap.end()) 
        { 
            if (it->second == 3) return HandType::FourOfAKind; // e.g. 23444
            if (it->second == 2) // e.g. 22344
            { 
                if (numJokers == 1) return HandType::FullHouse; // NOTE: the other two cards are of the same kind
                if (numJokers == 2) return HandType::FourOfAKind;
                throw std::invalid_argument("Invalid number of jokers when there should be 3 unique numbers and at least one pair...");
            }
            it++;    
            // NOTE: ignoring a value of 1 because it doesn't help us determine which Hand Type it is
        } 
        throw std::invalid_argument("Somehow there are 3 types but it's not Three of a Kind or Two Pair...");
    }
    if (cardMap.size() == 4) return HandType::ThreeOfAKind; //e.g. 23455
    if (cardMap.size() == 5) return HandType::OnePair; // e.g. 23456
    throw std::invalid_argument("cards do not correspond to an expected hand type");
}

// sort hands in ascending order by stating if hand1 is less than hand 2
bool CompareHands(Hand hand1, Hand hand2)
{
    if (hand1.handType != hand2.handType)
    {
        return hand1.handType < hand2.handType;
    }

    // if hand types are the same, sort by card Types
    for (int i = 0; i < kNumCardsInHand; i++)
    {
        if (hand1.cards[i] != hand2.cards[i])
        {
            return hand1.cards[i] < hand2.cards[i];
        }
    }

    // if we have got through the whole for loop without returning, the cards are identical 
    // return false as this comparator is a "less than" comparator
    return false;
}

bool CompareHandsWithJoker(Hand hand1, Hand hand2)
{
    if (hand1.handType != hand2.handType)
    {
        return hand1.handType < hand2.handType;
    }

    // if hand types are the same, sort by card Types
    for (int i = 0; i < kNumCardsInHand; i++)
    {
        CardType card1 = hand1.cards[i];
        CardType card2 = hand2.cards[i];
        
        // reassign J to a Joker to make it the weakest value
        if (card1 == CardType::J) card1 = CardType::Joker;
        if (card2 == CardType::J) card2 = CardType::Joker;
        
        if (card1 != card2)
        {
            return card1 < card2;
        }
    }

    // if we have got through the whole for loop without returning, the cards are identical 
    // return false as this comparator is a "less than" comparator
    return false;
}

// debug methods
void PrintHand(Hand hand)
{
    bool hasJ = true;
    for (int i = 0; i < kNumCardsInHand; i++)
    {
        if(hand.cards[i] == CardType::J) hasJ = true;
    }
    
    if (hasJ)
    {
        cout << "Cards in Hand: ";
        for (int i = 0; i < kNumCardsInHand; i++)
        {
            std::cout << hand.cards[i] << " ";
        }
        cout << endl;

        cout << "Hand Type: " << hand.handType << endl;
        cout << "Bid: " << hand.bid << endl;
        cout << endl;
    }
}

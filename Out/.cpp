#include <cstdint>
#include <string>
#include <vector>
#include <iostream>
using namespace std;

void sumar(int parameter1, int &parameter2);
void hola(void);
void nombre7(const string &parameter1, const string &parameter2);
void nombre2(const string &parameter1, const string &parameter2);
void nombre3(const string &par1);
void nombre4(const string &parameter1, string &parameter2);
void nombre6(const string &par1, const string &par2);

struct LadonRT
{
    char ch;
};
int main()
{
    LadonRT ladon;
    string nombre = "hola";
    int numero = (1 + (2 * (3 - 1)));
    while ((1 > 2))
    {
    }

    do
    {
    } while (true);

    for (int i = 0; (i < 10); (i = (i + 1)))
    {
        int numero = 2;
    }

    if (numero == 1)
    {
    }
    else if (numero == 2)
    {
    }

    if (numero == 1)
    {
    }
    else
    {
    }

    if ((true == true))
    {
    }

    if ((2 < 21))
    {
    }
    else
    {
    }

    if ((2 < 4))
    {
    }
    else if ((2 > 2))
    {
    }
    else
    {
    }

    int n = 0;
    string nombre_variable = "";
    string input_name = "";
    std::string _tmp;
    std::getline(std::cin, _tmp);
    std::cin.get();

    do
    {
        ladon.ch = (char)std::cin.get();
    } while (ladon.ch != 'A');

    do
    {
        ladon.ch = (char)std::cin.get();
    } while (ladon.ch != 'A');
    std::getline(std::cin, nombre_variable);
    cout << "\n";
    cout << nombre << "\n";
    hola();
    sumar(1, n);
    nombre = "another";

    return 0;
}

void sumar(int parameter1, int &parameter2)
{
}

void hola(void)
{
}

void nombre7(const string &parameter1, const string &parameter2)
{
    int s = 1;
}

void nombre2(const string &parameter1, const string &parameter2)
{
    return;
}

void nombre3(const string &par1)
{
    int n = 1;
}

void nombre4(const string &parameter1, string &parameter2)
{
}

void nombre6(const string &par1, const string &par2)
{
}

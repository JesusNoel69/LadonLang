#include <iostream> 
#include <fstream>
#include <string>
using namespace std;

void _Read(const string& , const string &);
void _Write(const string &);
int n;
void fibonacci(int, int, int &);
void factorial(int &);
int main() { 
cout<<"ingrese el numero de fiboles: ";
cin>>n;
cout<<"serie: ";
fibonacci(0 , 1 , n );
cout<<"ingrese el numero factorial: ";
cin>>n;
factorial(n );
system("pause");return 0;
 }
void _Write(const string &url){ 
ifstream file(url);
if (!file) { 
cerr << "Error al abrir el archivo para leer.\n";
return;
 }
 string line;
 while (getline(file, line)) { 
cout << line << '\n';
 }
file.close();
 }
void _Read(const string& content,const string &Url) { 
ofstream file(Url, ios::app);
if (!file) { 
cerr << "Error al abrir el archivo para escribir."; 
return;
 }
file << content << '\n';
file.close(); 
 } 
void fibonacci(int a, int b, int &n){ 
if( n == 0) { 
return;
} 
cout<<b;
cout<<", ";
int z = a + b;
int x = n - 1;
fibonacci(b , z , x );
} 
void factorial( int &n){ 
int result = 1;
for(int Index = 1; Index<=n; Index++) { 
result =  result * Index;
} 
cout<<result;
} 

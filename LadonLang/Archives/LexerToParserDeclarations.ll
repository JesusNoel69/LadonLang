[Write("ingrese el numero de fiboles: ")];
num n;
n=[READ];
[Write("serie: ")];
fibonacci(0,1,n);
[Write("ingrese el numero factorial: ")];
n=[Read];
factorial(n);
Fn fibonacci(num a, num b, out n) 
---
    [if:(n == 0)]
    ---
        go;
    ---
    [Write(b)];
    [Write(", ")];
    num z=a+b;
    num x= n-1;
    fibonacci(b,z,x);
---

Fn factorial(out n)
---
    num result =1;
    [Loop :Index=1,Iter=n]
    ---
        result = result*Index;
    ---
    [Write(result)];

---
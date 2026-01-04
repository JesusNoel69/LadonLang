FN factorial(n<int>, out result<int>)=<
    <if n < 0;>
        result = 0;
        <return/>
    </if>

    result = 1;

    <loop i = 1; i <= n; i = i + 1;>
        result = result * i;
        <output>result; </output>
    </loop>

    <return/>
/>

var num<int>;
factorial(5,num);

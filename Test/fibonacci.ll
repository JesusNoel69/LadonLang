FN fibonacci(n<int>, out result)->int=<
    <if n < 1;>
        result = n;
        <output>n;</output>
        <return/>
    </if>
    <output>0;</output>
    <output>1;</output>

    var a<int> = 0;
    var b<int> = 1;
    var i<int> = 2;

    <loop i = 2; i <= n; i = i + 1;>
        var temp<int> = a + b;
        a = b;
        b = temp;
        <output get[b]/>
    </loop>

    result = b;
    <return/>
/>

var r<int>=0;
fibonacci(10, r);

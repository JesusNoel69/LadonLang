var nombre<string> = "hola";
var numero<int> = 1+2*(3-1);

<loop 1> 2 ;>
</loop>

<loop true; pass>
</loop>

<loop i = 0; i < 10; i = i + 1;>
    var numero<int> = 2;
</loop>

<select numero>
    <option value=1>
    </option>
    <option value=2>
    </option>
</select>

<select numero>
    <option value=1>
    </option>
    <option default>
    </option>
</select>

<if true==true; @name>
</if>

<if 2<21;>
</if>
<else>
</else>

<if 2<4;>
</if>
<elif 2>2;>
</elif>
<else>
</else>
var n<int> =0;

var nombre_variable<string>="";
var input_name<string>="";
<input/>
<input type=[key]/>
<input type=[key] key='A'/>
<input key='A' type=[key]/>
<input set[nombre_variable]/>

<output> </output>
<output get[nombre]/>

FN sumar(parameter1<int>, out parameter2<int>)=<
/>
FN hola()=<
/>

hola();
sumar(1,n);


FN nombre7(parameter1, parameter2)=<
    var s<int> = 1;
/>

FN nombre2(parameter1, parameter2)=<
    <return/>
/>

FN nombre3(par1<string>)->int=<
    var n<int> =1;
/>

FN nombre4(parameter1, out parameter2)=<
/>

FN nombre6(par1, par2)=<  />

nombre = "another";
var numero;
var nombre1, nombre2;
nombre<string> = "hola";
variable_compuesta<int,float> = 2, 2.5;
<input/>
<input type=[key]/>
<input type=[key] key='A'/>
<input key='A' type=[key]/>
<input set[nombre_variable]/>

<output> </output>
<output get[nombre]/>

<loop>

</loop>

<loop item in items>
    
</loop>

<loop item of items>
    
</loop>


<loop condicion > 2 ;>
    
</loop>

<loop true; pass>
    
</loop>

<loop item of items @nombre>
    
</loop>

<loop item of items id=nombre>
    
</loop>

<loop i = 0; i < 10; i = i + 1;>
    
</loop>




<select valor_pasado>
    <option value=valor1>
    </option>
    <option value=valor2>
    </option>
</select>

<select valor_pasado>
    <option value=valor1>
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


FN nombre(parameter1, parameter2)=<
    
/>

nombre(parameter1, parameter2)=<
    <return/>
/>

FN nombre(par1<int>)->int=<
    <return/>
/>

FN nombre(parameter1, out parameter2)=<
/>

FN nombre()->int,float=<
/>

nombre(par1, par2)=<  />


nombre = () => "algun texto"

nombre = () => <
     <return/>
/>

es_mayor=e=>e>=18

sumar=(e,d)=>e+d
mul<int>=(e<int>, x<int>)=>x*e

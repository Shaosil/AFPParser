# AFPParser
Parses IBM AFP files line by line<br/>
<br/>
This project is my attempt to better understand the structure of AFP files.<br/>

There are well over 1,000 pages of documentation on how to decode sections and bytes of AFP located on their official site<br/> 
<a href="http://afpcinc.org/publications/">here,</a> and I have included some of the most important ones in the project directory<br/> under ./Reference Manuals/.<br/>
<br/>
<b>Current Progress</b><br/>
<ul>
<li><strike>Understand the basic structure and hex patterns</strike></li>
<li><strike>Read each structured field into a raw list of bytes</strike></li>
<li><strike>Separate introducer data into their own properties, and read each field into a more strongly typed class</strike></li>
<li><strike>Support parsing semantics of any structured field, using predefined offsets</strike></li>
<li><strike>Understanding and parsing triplets, using their own set of offsets</strike></li>
<li><strike>Create a more solid class inheritance architecture</strike></li>
<li><strike>Support parsing of more fields' data</strike></li>
<li><i>Build a rendering engine.</i> <-- Current step</li>
<li>Fully support decoding of all field, triplet, and control sequences data.</li>
<li>Support editing/adding/deleting fields and saving the file according to definied AFP standards.</li>
</ul>

The remaining steps are not necessarily listed in order of execution. If I have time, I would like to start working on a way to<br/>
view a sort of "print preview" once I have more fields supported, but it may not be realistic to support every possible field<br/> beforehand. Certain fields are referenced much more often than others.

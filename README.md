# AFPParser
Parses IBM AFP files line by line<br/>
<br/>
This project is my attempt to better understand the structure of AFP files.<br/>

There are well over 1,500 pages of documentation on how to decode sections and bytes of AFP located on their official site <a href="http://afpcinc.org/publications/">here,</a> and I have included some of the most important ones in the project directory under ./Reference Manuals/.<br/>
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
<li><strike>Build a rendering engine.</strike></li>
<li><i>Support editing/adding/deleting fields and saving the file according to definied AFP standards.</i> <-- Current step</li>
<li>Fully support decoding of all field, triplet, and control sequences data.</li>
</ul>

The remaining steps are not necessarily listed in order of execution. I will most likely never implement typed properties for every single field, triplet, etc.

<b>7/17/2017</b> - The project, as it stands, is nearing the end of its life on Github. I shall continue coding/supporting it, but have started transitioning it to my company's source control in TFS. As a result, proprietary namespacing and refactoring may take place, and that certainly cannot be committed to Github. If I have time, I will sync core functionality as I see fit.

# Cortana Quick #

- Quick audible notes for Cortana.

This app was developed as final project for the "Processamento da Informação"  course ministered by Fabrício Olivetti and Ronaldo Prati at Universidade Federal do ABC for demonstrate the usage of the Cortana's api and all the basic logic learned in this course.

Cortana already have the ability to save notes naturally, but, the notes she saves are stored on OneNote, and these notes can't be read passively by neither Cortana or OneNote as the user request, so this is what made us develop this app.

Using Cortana's api, we retrieve a single string that will be characterized as a AskCommand or a NoteCommand based on keywords specified on the VoiceDefinition file that handles all the input Cortana should understand as a command for our        software.

If the command entered via Cortana is characterized as a NoteCommand, the user input is stored at the isolated storage for the app and binded to the list of notes that the user have noted on the main page.

If the command entered is a AskCommand, the user input is treated as such, splitting the words on the phrase, then, we eliminate every common word that might be, such as connectives. With these probably keywords we search for phrases that contain these on the database and retrieve all the notes that match this, after every keyword have been used, we have a list with every possible match, but, the note that appears more times on our list is very likely to be the answer to the user question. It's not a very mathematical approach, but it proved to be very accurate using statistics and our tests. 


This software is licensed under the MIT license
----------
The MIT License (MIT) 
Copyright © 2014 <copyright holders>

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

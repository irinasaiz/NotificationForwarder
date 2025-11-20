Description of the project:

Implement a RESTful web service that receives notifications via a POST interface and forwards them to a [a messenger of your choice] channel based on their type. 
(If you don’t have access to [a messenger of your choice], feel free to use a comparable messenger) Notifications of type "Warning" should be forwarded, while notifications of type "Info" should not. 
There are no more detailed requirements, so feel free to use your creativity.

Implementation:
This is a web app written in C# with dotnet. I used the latest versions: C# 12 and .net 8.
I used ASP.NET Core for creating the controller and Swagger to be able to interact with it.

The app offers a controller as an entry point to the app. 

The controller calls a service that filters the requests.


The requests that match the condition (type Warning) are written to a file.


Writing to a file is put behind a repository so that it can be switched with another service in the future.


The file where we do the writes is called "forwarded_notifications.txt" and it can be found in the root of the code.

Running the app:
To start the app, open the solution in Visual Studio and run it (this will also build it):
<img width="1516" height="1008" alt="image" src="https://github.com/user-attachments/assets/3e439b4e-41e7-42c1-a801-4d0d29b30df1" />
Swagger will open at this address http://localhost:5161/swagger/index.html and you can interact with the app:
<img width="1683" height="1010" alt="image" src="https://github.com/user-attachments/assets/e7b98f89-f981-4a88-8679-10b0fa19c50c" />
Wrong requests are not allowed:
<img width="1189" height="949" alt="image" src="https://github.com/user-attachments/assets/9b10306d-48ee-4ba7-8eb9-7e9bfc5222a4" />
This is the content of the output file:
<img width="489" height="424" alt="image" src="https://github.com/user-attachments/assets/53421be5-7ef1-4607-9ee2-67bc06091b54" />

Tests are in progress. I plan to have 2 types:
- unit tests, where I test each class individually
- end to end tests - where I send POST requests to the controller and then check the info was written in the output file




# Fetch-Rewards-Exercise
This is a REST API implemented with ASP.NET Core for Fetch Rewards exercise

## Install
Install [Visual Studio Community](https://visualstudio.microsoft.com/vs/community/). This will install .Net Core SDK as well. 

## Test

Open the solution in Visual Studio and run from source. You will see the following Swagger UI in the browser. 


<img width="951" alt="image" src="https://user-images.githubusercontent.com/7350037/202968519-dcdf7ed3-d2b6-42a9-b0c7-1c46ed13642a.png">

To execute the API, you need to expand the section for the corresponding API. Click Try it out and then enter the input parameters. Then click Execute to send the request.

### Test Adding Points

You will need to enter the following input:

```
{"payer":"DANNON", "points":300, "timestamp": "2022-10-31T10:00:00Z"}
{"payer":"UNILEVER", "points":200, "timestamp": "2022-10-31T11:00:00Z"}
{"payer":"DANNON", "points":-200, "timestamp": "2022-10-31T15:00:00Z"}
{"payer":"MILLER COORS", "points":10000, "timestamp": "2022-11-01T14:00:00Z"}
{"payer":"DANNON", "points":1000, "timestamp": "2022-11-02T14:00:00Z"}
```

Each requset should be executed successfully.


### Test Spending Points

You will need to enter the following input:

```
{"points": 5000}
```

You should see the following response

<img width="910" alt="image" src="https://user-images.githubusercontent.com/7350037/202969992-832cda75-9ab1-43a6-8a69-d66d72263b92.png">


### Test Getting Balance

No input is needed for this request. 

You should see the following response

<img width="909" alt="image" src="https://user-images.githubusercontent.com/7350037/202970232-8b00b86e-e27d-42c8-9188-273d72fc35e1.png">







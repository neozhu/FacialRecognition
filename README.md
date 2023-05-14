

# Face Recognition With  Application

A web application built with Blazor Server that uses Insightface API to recognize faces. This application is designed to simulate a visitor registration system, where visitors can fill out an application form and upload their photos. After the application is approved, their faces are captured by a camera and analyzed by the Insightface API to find the visitor's application and update their visitation record.

## Features

- User-friendly interface for visitors to submit their applications and upload their photos.
- Real-time face recognition using the Insightface API.
- Automatic retrieval of visitor's application record and update of their visitation status.
- Admin dashboard to manage application approvals and view visitor records.
- Easy deployment with Docker.

## Demo
Watch this [video](doc/demo.mp4) to see a demonstration of the application in action.


https://github.com/neozhu/FacialRecognition/assets/1549611/c2f4486c-e606-4f59-b061-77ee8c74b3ac


Please note that due to limited bandwidth on my virtual host, the application may experience some latency during usage. We apologize for any inconvenience this may cause.

### Serverside Image-Size Restriction
⚠️ This step is crucial if you want to use the CameraStreamer on ServerSide Blazor ⚠️

The data from the JavaScript-Interop is sent through SignalR to the Server, where your C# Code handles the received data. SignalR has a image cap of 32KB (which is tiny, smaller than 150x150 pixels).

This restriction can be bypassed though. In your [ProjectName].Startup.ConfigureServices method, change the following line:

services.AddServerSideBlazor()
        .AddHubOptions(o => o.MaximumReceiveMessageSize = 100_000_000); // add this
The MaximumReceiveMessageSize value indicates the maximum allowed number of bytes that can be sent through SignalR. It could also be set to long.MaxValue, to entirely max out the restriction.

100'000'000 has no significant meaning, it's just a general cap. As an example, a 3-Channel 8-Bit 1920x1080 image has the following size: 3 * 1920 * 1080 = 6'220'800 Bytes (this calculation does not take image-compression into account).

⚠️ BlazorCameraStreamer can be used in ServerSide Blazor. In general though, it is not recommended to use on ServerSide, since a lot of data is sent through SignalR which can cause network delays. There is a reason, why Microsoft set the default limit to only 32KB. ⚠️

## Installation and Usage

1. Clone the repository to your local machine.
2. Install Docker and Docker Compose.
3. Open a terminal and navigate to the project directory.
4. Run `docker-compose up -d` to start the application.
5. Access the application at http://localhost:5000.

## Configuration

- `appsettings.json`: This file contains configuration options for the application. You can modify the database connection string, Insightface API key, and other settings here.

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

## License

This project is licensed under the [MIT License](LICENSE).



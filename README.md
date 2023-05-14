

# Face Recognition Web Application

A web application built with Blazor Server that uses Insightface API to recognize faces. This application is designed to simulate a visitor registration system, where visitors can fill out an application form and upload their photos. After the application is approved, their faces are captured by a camera and analyzed by the Insightface API to find the visitor's application and update their visitation record.

## Features

- User-friendly interface for visitors to submit their applications and upload their photos.
- Real-time face recognition using the Insightface API.
- Automatic retrieval of visitor's application record and update of their visitation status.
- Admin dashboard to manage application approvals and view visitor records.
- Easy deployment with Docker.

## Demo
Watch this [video](doc/demo.mp4) to see a demonstration of the application in action.
<video src="[./doc/demo.mp4](https://github.com/neozhu/FacialRecognition/blob/dev/doc/demo.mp4)" controls="controls" wwidth="1280" height="720"></video>
[<video src="[./doc/demo.mp4](https://github.com/neozhu/FacialRecognition/blob/dev/doc/demo.mp4)" controls="controls" wwidth="1280" height="720"></video>](https://github.com/neozhu/FacialRecognition/blob/dev/doc/demo.mp4)
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



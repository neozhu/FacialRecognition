using CleanArchitecture.Blazor.Application.Common.Configurations;
using Exadel.Compreface.Clients.CompreFaceClient;
using Exadel.Compreface.DTOs.FaceCollectionDTOs.AddSubjectExample;
using Exadel.Compreface.DTOs.FaceCollectionDTOs.DeleteAllSubjectExamples;
using Exadel.Compreface.DTOs.FaceCollectionDTOs.DeleteImageById;
using Exadel.Compreface.DTOs.FaceDetectionDTOs.FaceDetection;
using Exadel.Compreface.DTOs.RecognitionDTOs.RecognizeFaceFromImage;
using Exadel.Compreface.DTOs.RecognizeFaceFromImageDTOs.RecognizeFaceFromImage;
using Exadel.Compreface.Services;
using Exadel.Compreface.Services.RecognitionService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.Services.CompreFace
{
    public class CompreFaceService
    {
        private readonly CompreFaceSettings _settings;
        private readonly CompreFaceClient _client;

        public CompreFaceService(CompreFaceSettings settings)
        {
            _settings = settings;
            _client = new CompreFaceClient(domain: _settings.Domain, port: _settings.Port);
        }
        public async Task<DeleteAllExamplesResponse> DeleteCollection(DeleteAllExamplesRequest request)
        {
            var recognitionService = _client.GetCompreFaceService<RecognitionService>(_settings.RecKey);
            var faceCollection = recognitionService.FaceCollection;
            var response = await faceCollection.DeleteAllAsync(request);
            return response;
        }

        public async Task<AddSubjectExampleResponse> AddCollection(AddSubjectExampleRequestByFilePath request)
        {
            var recognitionService = _client.GetCompreFaceService<RecognitionService>(_settings.RecKey);
            var faceCollection = recognitionService.FaceCollection;
            var response = await faceCollection.AddAsync(request);
            return response;
        }
  
        public async Task<RecognizeFaceFromImageResponse> RecognizeFace(RecognizeFaceFromImageRequestByBytes request)
        {
            var recognitionService = _client.GetCompreFaceService<RecognitionService>(_settings.RecKey);
            var response =await recognitionService.RecognizeFaceFromImage.RecognizeAsync(request);
            return response;
        }

        public async Task<FaceDetectionResponse> DetectFace(FaceDetectionRequestByBytes  request)
        {
            var faceDetectionService = _client.GetCompreFaceService<DetectionService>(_settings.DetKey);
            var response = await faceDetectionService.DetectAsync(request);
            return response;
        }
    }
}

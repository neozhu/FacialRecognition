using CleanArchitecture.Blazor.Application.Common.Configurations;
using Exadel.Compreface.Clients.CompreFaceClient;
using Exadel.Compreface.DTOs.FaceCollectionDTOs.AddSubjectExample;
using Exadel.Compreface.DTOs.FaceCollectionDTOs.DeleteImageById;
using Exadel.Compreface.DTOs.RecognitionDTOs.RecognizeFaceFromImage;
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
        public async Task<DeleteImageByIdResponse> DeleteCollection(DeleteImageByIdRequest request)
        {
            var recognitionService = _client.GetCompreFaceService<RecognitionService>(_settings.APIKey);
            var faceCollection = recognitionService.FaceCollection;
            var response = await faceCollection.DeleteAsync(request);
            return response;
        }

        public async Task<AddSubjectExampleResponse> AddCollection(AddSubjectExampleRequestByFilePath request)
        {
            var recognitionService = _client.GetCompreFaceService<RecognitionService>(_settings.APIKey);
            var faceCollection = recognitionService.FaceCollection;
            var response = await faceCollection.AddAsync(request);
            return response;
        }
        public async Task<RecognizeFaceFromImageResponse> AddCollection(RecognizeFaceFromImageRequestByFilePath request)
        {
            var recognitionService = _client.GetCompreFaceService<RecognitionService>(_settings.APIKey);
            var response = await recognitionService.RecognizeFaceFromImage.RecognizeAsync(request);
            return response;
        }
    }
}

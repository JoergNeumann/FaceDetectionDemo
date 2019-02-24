using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetectionDemo
{
    public class FaceRecognitionService
    {
        private FaceClient _faceClient;

        public FaceRecognitionService(string subscriptionKey, string serviceUrl)
        {
            _faceClient = new FaceClient(
                new ApiKeyServiceClientCredentials(subscriptionKey),
                new System.Net.Http.DelegatingHandler[] { });
            _faceClient.Endpoint = serviceUrl;
        }

        public async Task TrainPersonGroup(string groupId, string groupName, string personName, string faceImageDir)
        {
            // Gruppe erstellen
            await _faceClient.PersonGroup.CreateAsync(groupId, groupName);

            // Person hinzufügen
            var person = await _faceClient.PersonGroupPerson.CreateAsync(groupId, personName);

            // Fotos der Person hinzufügen
            foreach (string imagePath in Directory.GetFiles(faceImageDir, "*.jpg"))
            {
                using (Stream s = File.OpenRead(imagePath))
                {
                    await _faceClient.PersonGroupPerson.AddFaceFromStreamAsync(groupId, person.PersonId, s);
                }
            }

            // Model trainieren
            await _faceClient.PersonGroup.TrainAsync(groupId);

            // Trainingsstatus abfragen
            TrainingStatus trainingStatus = null;
            while (true)
            {
                trainingStatus = await _faceClient.PersonGroup.GetTrainingStatusAsync(groupId);

                if (trainingStatus.Status != TrainingStatusType.Running)
                {
                    break;
                }
                await Task.Delay(1000);
            }
        }

        public async Task<DetectedPersonInfo> IdentifyPerson(string personGroupId, string imageFile)
        {
            var response = new DetectedPersonInfo();

            using (var stream = File.OpenRead(imageFile))
            {
                // Welche Attribute sollen erkannt werden?
                var faceAttributes = new FaceAttributeType[]
                {
                    FaceAttributeType.Gender, FaceAttributeType.Age,
                    FaceAttributeType.Smile, FaceAttributeType.Emotion,
                    FaceAttributeType.Glasses, FaceAttributeType.Hair
                };

                // Gesichter erkennen
                var faces = await _faceClient.Face.DetectWithStreamAsync(stream, true, false, faceAttributes);
                if (faces.Count > 0)
                {
                    response.Attributes = FaceDescription(faces[0]);
                }

                // Face IDs ermitteln
                var faceIds = faces.Select(face => face.FaceId).Where(x => x.HasValue).Select(z => z.Value).ToList();

                // Gesichter identifizieren
                var results = await _faceClient.Face.IdentifyAsync(faceIds, personGroupId);
                foreach (var identifyResult in results)
                {
                    if (identifyResult.Candidates.Count == 0)
                    {
                        response.Name = null;
                        break;
                    }
                    else
                    {
                        // Name der ersten erkannten Person zurückgeben
                        var candidateId = identifyResult.Candidates[0].PersonId;
                        var person = await _faceClient.PersonGroupPerson.GetAsync(personGroupId, candidateId);
                        response.Name = person.Name;
                        response.Confidence = (identifyResult.Candidates[0].Confidence / 1 * 100) + "%";
                        break;
                    }
                }
                
                return response;
            }
        }

        private string FaceDescription(DetectedFace face)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Face: ");

            // Add the gender, age, and smile.
            sb.Append(face.FaceAttributes.Gender);
            sb.Append(", ");
            sb.Append(face.FaceAttributes.Age);
            sb.Append(", ");
            sb.Append(String.Format("smile {0:F1}%, ", face.FaceAttributes.Smile * 100));

            // Add the emotions. Display all emotions over 10%.
            sb.Append("Emotion: ");
            Emotion emotionScores = face.FaceAttributes.Emotion;
            if (emotionScores.Anger >= 0.1f) sb.Append(
                String.Format("anger {0:F1}%, ", emotionScores.Anger * 100));
            if (emotionScores.Contempt >= 0.1f) sb.Append(
                String.Format("contempt {0:F1}%, ", emotionScores.Contempt * 100));
            if (emotionScores.Disgust >= 0.1f) sb.Append(
                String.Format("disgust {0:F1}%, ", emotionScores.Disgust * 100));
            if (emotionScores.Fear >= 0.1f) sb.Append(
                String.Format("fear {0:F1}%, ", emotionScores.Fear * 100));
            if (emotionScores.Happiness >= 0.1f) sb.Append(
                String.Format("happiness {0:F1}%, ", emotionScores.Happiness * 100));
            if (emotionScores.Neutral >= 0.1f) sb.Append(
                String.Format("neutral {0:F1}%, ", emotionScores.Neutral * 100));
            if (emotionScores.Sadness >= 0.1f) sb.Append(
                String.Format("sadness {0:F1}%, ", emotionScores.Sadness * 100));
            if (emotionScores.Surprise >= 0.1f) sb.Append(
                String.Format("surprise {0:F1}%, ", emotionScores.Surprise * 100));

            // Add glasses.
            sb.Append(face.FaceAttributes.Glasses);
            sb.Append(", ");

            // Add hair.
            sb.Append("Hair: ");

            // Display baldness confidence if over 1%.
            if (face.FaceAttributes.Hair.Bald >= 0.01f)
                sb.Append(String.Format("bald {0:F1}% ", face.FaceAttributes.Hair.Bald * 100));

            // Display all hair color attributes over 10%.
            IList<HairColor> hairColors = face.FaceAttributes.Hair.HairColor;
            foreach (HairColor hairColor in hairColors)
            {
                if (hairColor.Confidence >= 0.1f)
                {
                    sb.Append(hairColor.Color.ToString());
                    sb.Append(String.Format(" {0:F1}% ", hairColor.Confidence * 100));
                }
            }

            // Return the built string.
            return sb.ToString();
        }
    }
}

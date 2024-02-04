# Azure AI Speech Service  
* Code Example from Microsoft - https://github.com/Azure-Samples/cognitive-services-speech-sdk
* Azure Speech CLI - https://learn.microsoft.com/en-us/azure/ai-services/speech-service/spx-overview

## Speech to text API
* Based on the Universal Language Model that was trained by Microsoft.

### Real-time transcription
* Real-time speech to text allows you to transcribe text in audio streams
* In order for real-time transcription to work, your application will need to be listening for incoming audio from a microphone, or other audio input source such as an audio file.
Your application code streams the audio to the service, which returns the transcribed text.

### Batch transcription
* You can point to audio files with a shared access signature (SAS) URI and asynchronously receive transcription results

## The text to speech API
* The text to speech API enables you to convert text input to audible speech
* The service includes multiple pre-defined voices with support for multiple languages and regional pronunciation
* You can also develop custom voices and use them with the text to speech API

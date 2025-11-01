### Project Setup

The project setup is a typical ASP .NET hosted Blazor WASM solution, with 3 seperate projects:

- Client
- Server
- Shared

### Running the App
The Blazor client app is hosted on the server, so they both run on the same port. Running the server project in Visual Studio with the `RunApp` profile will run both together. The client can be run on it's own using the `RunClientOnly` profile.

### Using the App
Use the `Start` button to begin detection. This will highlight faces in the frame.
If there are any saved faces that match then the name will be displayed. 
The recognition logic only supports one face at a time.

Use the `Train` button to manage saved faces. Currently only JPEG images are supported for training.

### Configuration
The server is configured with an appsettings.json file. There are a few different settings available to configure the app:

#### UseGPU (*boolean*)
The facial recognition process uses the ONNX runtime. This workload can be run on the CPU or on an NVIDIA CUDA enabled GPU. By default the app uses the CPU.

The performance of GPU rendering can vary and can be tricky to get working nicely.

More details can be found [here](https://onnxruntime.ai/docs/execution-providers/CUDA-ExecutionProvider.html#requirements). The version of the ONNX runtime the app uses is 1.9.0. The latest version compatible version of CUDA is 11.8.

#### Threshold (*integer* < 100)
This is the percentage similarity threshold before two faces can be considered a match. If there are no trained faces above this threshold then the detected face is treated as not recognised. The app only displays the match with the highest percentage similarity.

The percentage similarity can vary with differences in lighting/angle, and if the subject is wearing glasses. By default the threshold is 75%. Anywhere in the 70-80% range is recommended. If the threshold setting is not set then it has no effect, and the highest similarity match is always returned.

#### MongoDB
By default the app stores trained faces in memory. MongoDB can be used as an alternative means for persistent storage. Whether MongoDB is used is based on whether there is a property called `MongoDB` present in the appsettings.json.

The individual values for the MongoDB settings are:

```json
{
  "MongoDB": {
      "ConnectionString": "mongodb+srv:<Username>:<Password>@<cluster>.mongodb.net/?retryWrites=true&w=majority&appName=<appname>",
      "DatabaseName": "The name of the database in the MongoDB instance",
      "CollectionName": "The name of the collection in the database",
      "SearchIndexName": "The name of the vector search index for the collection"
    }
}
```

The search index on the collection must be of type `vectorSearch` and should have the following configuration:
```json
{
  "fields": [
    {
      "numDimensions": 512,
      "path": "Embedding",
      "similarity": "cosine",
      "type": "vector"
    }
  ]
}
```
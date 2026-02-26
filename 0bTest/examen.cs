using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Identity;

// Create a DefaultAzureCredentialOptions object to configure the DefaultAzureCredential
DefaultAzureCredentialOptions options = new()
{
    ExcludeEnvironmentCredential = true,
    ExcludeManagedIdentityCredential = true
};

// Run the examples asynchronously, wait for the results before proceeding
await ProcessAsync();

async Task ProcessAsync()
{
// Create a credential using DefaultAzureCredential with configured options
string accountName = "examenalejandro"; // Replace with your storage account name

// Use the DefaultAzureCredential with the options configured at the top of the program
DefaultAzureCredential credential = new DefaultAzureCredential(options);

// Create the BlobServiceClient using the endpoint and DefaultAzureCredential
string blobServiceEndpoint = $"https://{accountName}.blob.core.windows.net";
BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri(blobServiceEndpoint), credential);

// Create a unique name for the container
string containerName = "blobcontenedorexamen6";

// Create the container and return a container client object
Console.WriteLine("Creando el contenedor " + containerName);
BlobContainerClient containerClient = 
    await blobServiceClient.CreateBlobContainerAsync(containerName);

// Check if the container was created successfully
if (containerClient != null)
{
    Console.WriteLine("Contenedor creado con exito");
}
else
{
    Console.WriteLine("Fallo al crear el contenedor");
    return;
}

// Create a local file in the ./data/ directory for uploading and downloading
Console.WriteLine("Creando un fichero local con el texto EXAMEN PRACTICO poara subirlo al Blob...");
string localPath = "./";
string fileName = "ficheroExamen6.txt";
string localFilePath = Path.Combine(localPath, fileName);

// Write text to the file
await File.WriteAllTextAsync(localFilePath, "\n\nEXAMEN PRACTICO\n\n");
Console.WriteLine("Fichero creado...");

// Get a reference to the blob and upload the file
BlobClient blobClient = containerClient.GetBlobClient(fileName);

Console.WriteLine("Subiendo el fichero al Blob \n\t {0}", blobClient.Uri);

// Open the file and upload its data
using (FileStream uploadFileStream = File.OpenRead(localFilePath))
{
    await blobClient.UploadAsync(uploadFileStream);
    uploadFileStream.Close();
}

// Verify if the file was uploaded successfully
bool blobExists = await blobClient.ExistsAsync();
if (blobExists)
{
    Console.WriteLine("Fichero subido correctamente");

}
else
{
    Console.WriteLine("File upload failed, exiting program..");
    return;
}

// Adds the string "DOWNLOADED" before the .txt extension so it doesn't 
// overwrite the original file

string downloadFilePath = localFilePath.Replace(".txt", "DESCARGADO.txt");

Console.WriteLine("Descargando blob to: {0}", downloadFilePath);

// Download the blob's contents and save it to a file
BlobDownloadInfo download = await blobClient.DownloadAsync();

using (FileStream downloadFileStream = File.OpenWrite(downloadFilePath))
{
    await download.Content.CopyToAsync(downloadFileStream);
}

Console.WriteLine("Blob descargado correctamente a {0}", downloadFilePath);



Console.WriteLine("Contenido de archivo:\n\n");


Console.WriteLine("CONTENIDO:\n{0}", await File.ReadAllTextAsync(downloadFilePath));

}
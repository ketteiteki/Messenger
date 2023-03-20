using Microsoft.AspNetCore.Http;

namespace Messenger.IntegrationTests.Helpers;

public static class FilesHelper
{
    private const string FilePath = "../../../Files/img1.jpg";
    
    public static FormFile GetFile()
    {
        var fullPath = Path.Combine(AppContext.BaseDirectory, FilePath);

        var stream = File.OpenRead(fullPath);
       
        var file = new FormFile(
            baseStream: stream,
            baseStreamOffset: 0,
            length: stream.Length,
            name: "qwerty",
            fileName: "qwerty.jpg") 
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };

        return file;
        
    }
}
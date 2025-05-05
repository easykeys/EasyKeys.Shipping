using System.Reflection;

using EasyKeys.Shipping.FedEx.Abstractions.OpenApis.V1.TradeDocumentsUpload;
using EasyKeys.Shipping.FedEx.UploadDocument;
using EasyKeys.Shipping.FedEx.UploadDocument.Models;

using EasyKeysShipping.FuncTest.TestHelpers;

using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.FuncTest.FedEx;
public class FedExUploadDocumentProviderTests
{
    private readonly ITestOutputHelper _output;

    private readonly IServiceProvider _sp;

    public FedExUploadDocumentProviderTests(ITestOutputHelper output)
    {
        _output = output;
        _sp = ServiceProviderInstance.GetFedExServices(output);
    }

    [Fact]
    public async Task Upload_Image_Successfully()
    {
        var documentProvider = _sp.GetRequiredService<IFedExDocumentsProvider>();
        var jsonPayload = new JsonPayload
        {
            Document = new EasyKeys.Shipping.FedEx.UploadDocument.Models.Document
            {
                ReferenceId = "1234",
                Name = "ek_logo_main4.gif",
                ContentType = "image/gif",
                Meta = new EasyKeys.Shipping.FedEx.UploadDocument.Models.Meta
                {
                    ImageType = "SIGNATURE",
                    ImageIndex = "IMAGE_2"
                }
            },
            Rules = new Rules
            {
                WorkflowName = "LetterheadSignature"
            }
        };

        // Get the path to the current executable (the .exe file)
        var exePath = Assembly.GetExecutingAssembly().Location;

        // Get the directory where the executable is located
        var exeDirectory = Path.GetDirectoryName(exePath);

        var fileName = "Images/ek_logo_main4 (Custom).gif";

        var filePath = Path.Combine(exeDirectory, fileName);

        var fileContent = File.ReadAllBytes(filePath);

        var imageAttachment = new ImageAttachment
        {
            Data = fileContent,
            Name = "signature.PNG",
            ContentType = "image/png",
            ImageType = "SIGNATURE"
        };
        var result = await documentProvider.UploadImageAsync(jsonPayload,imageAttachment);

        Assert.NotNull(result);
        Assert.True(result.Success);
    }
}

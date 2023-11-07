using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Rates;
using EasyKeys.Shipping.FedEx.UploadDocument;

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

        // Get the path to the current executable (the .exe file)
        var exePath = Assembly.GetExecutingAssembly().Location;

        // Get the directory where the executable is located
        var exeDirectory = Path.GetDirectoryName(exePath);

        var fileName = "Images/ek_logo_main4 (Custom).gif";

        var filePath = Path.Combine(exeDirectory, fileName);

        var fileContent = File.ReadAllBytes(filePath);

        var result = await documentProvider.UploadImageAsync(fileContent, 4);

        Assert.NotNull(result);
        Assert.True(result.Success);
    }
}

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using PdfMerger.Data;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

namespace PdfMerger.Pages;

public partial class Upload
{
    private readonly List<UploadedFile> uploadedFiles = new();

    private async Task HandleSelectedFiles(InputFileChangeEventArgs e)
    {
        foreach (var file in e.GetMultipleFiles())
        {
            var maxAllowedSize = 10 * 1024 * 1024;
            using var stream = file.OpenReadStream(maxAllowedSize);
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            uploadedFiles.Add(new UploadedFile { FileName = file.Name, FileContent = memoryStream.ToArray() });
        }
    }

    private void RemoveFile(UploadedFile file)
    {
        uploadedFiles.Remove(file);
    }

    private void RemoveAll()
    {
        uploadedFiles.Clear();
    }

    private async Task MergePDFs()
    {
        if (uploadedFiles.Count < 2 || uploadedFiles == null)
        {
            return;
        }

        using var outputDocument = new PdfDocument();

        foreach (var file in uploadedFiles)
        {
            using var inputDocument = PdfReader.Open(new MemoryStream(file.FileContent), PdfDocumentOpenMode.Import);
            for (int i = 0; i < inputDocument.PageCount; i++)
            {
                var page = inputDocument.Pages[i];
                outputDocument.AddPage(page);
            }
        }

        using var memoryStream = new MemoryStream();

        outputDocument.Save(memoryStream, false);
        var fileBytes = memoryStream.ToArray();

        var fileName = $"Merged_{DateTime.Now:yyyyMMddHHmmss}.pdf";

        await JS.InvokeVoidAsync("FileUtil.saveAs", fileName, fileBytes);
    }
}

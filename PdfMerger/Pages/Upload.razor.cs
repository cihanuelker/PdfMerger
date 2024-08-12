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
        if (uploadedFiles.Count < 2)
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
    private void MoveUp(int index)
    {
        if (index <= 0 || index >= uploadedFiles.Count)
        {
            return;
        }

        var file = uploadedFiles[index];
        uploadedFiles.RemoveAt(index);
        uploadedFiles.Insert(index - 1, file);
    }

    private void MoveDown(int index)
    {
        if (index < 0 || index >= uploadedFiles.Count - 1)
        {
            return;
        }

        var file = uploadedFiles[index];
        uploadedFiles.RemoveAt(index);
        uploadedFiles.Insert(index + 1, file);
    }

    private void MoveToTop(int index)
    {
        if (index <= 0 || index >= uploadedFiles.Count)
        {
            return;
        }

        var file = uploadedFiles[index];
        uploadedFiles.RemoveAt(index);
        uploadedFiles.Insert(0, file);
    }

    private void MoveToBottom(int index)
    {
        if (index < 0 || index >= uploadedFiles.Count)
        {
            return;
        }

        var file = uploadedFiles[index];
        uploadedFiles.RemoveAt(index);
        uploadedFiles.Add(file);
    }
}

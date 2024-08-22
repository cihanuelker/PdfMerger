namespace PdfMerger.Data;

public class UploadedFile
{
    public string FileName { get; set; } = String.Empty;

    public byte[] FileContent { get; set; } = Array.Empty<byte>();
}

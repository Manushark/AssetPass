namespace AssetPass.Application.Interfaces;

/// <summary>
/// QR code generation service.
/// </summary>
public interface IQrCodeService
{
    /// <summary>
    /// Generates a QR code image as a byte array (PNG format).
    /// </summary>
    byte[] GenerateQrCode(string content, int pixelsPerModule = 10);

    /// <summary>
    /// Generates a printable PDF with QR code and equipment label.
    /// </summary>
    byte[] GenerateQrPdf(string qrContent, string equipmentName, string? companyName, string? companyLogoPath);
}

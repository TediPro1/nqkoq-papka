namespace SmartAccessLift.Web.Services;

public interface IQRCodeService
{
    string GenerateQRCodeImage(string data);
    string GenerateQRCodeToken();
}


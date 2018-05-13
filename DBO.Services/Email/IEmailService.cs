namespace DBO.Services.Email
{
    public interface IEmailService
    {
        void SendIntroMail(int companyId, string subject, string body, string email);
        string SendMail();
    }
}
namespace AgriTrace.Application.Emails;

public static class PasswordResetEmailTemplate
{
    public static (string subject, string body) Build(
        string fullName,
        string resetToken)
    {
        return (
            "🔒 Reset Your AgriTrace Password",
            $"""
    <!DOCTYPE html>
    <html lang="en">
    <head>
    <meta charset="UTF-8">
    <title>Reset Your Password</title>
    </head>

    <body style="margin:0;padding:0;background:#f4f8f5;font-family:'Segoe UI',Arial,sans-serif;">

    <table width="100%" cellpadding="0" cellspacing="0" style="padding:40px 0;">
    <tr>
    <td align="center">

    <table width="640" cellpadding="0" cellspacing="0"
    style="background:#ffffff;border-radius:12px;overflow:hidden;
    box-shadow:0 8px 24px rgba(0,0,0,.08);">

    <!-- Header -->
    <tr>
    <td align="center"
    style="background:linear-gradient(135deg,#2E7D32,#4CAF50);
    padding:40px;color:white;">

    <div style="font-size:46px;">🌱</div>

    <h1 style="margin:10px 0 6px;font-size:30px;">
    AgriTrace
    </h1>

    <p style="margin:0;font-size:15px;opacity:.9;">
    Agricultural Supply Chain Traceability System
    </p>

    </td>
    </tr>

    <!-- Body -->
    <tr>
    <td style="padding:40px;">

    <h2 style="margin-top:0;color:#2E7D32;">
    Hello, {fullName}!
    </h2>

    <p style="font-size:15px;color:#555;line-height:1.8;">
    We received a request to reset the password for your AgriTrace account.
    Use the verification code below to complete the password reset process.
    </p>

    <div style="
    margin:32px 0;
    padding:28px;
    background:#F8FBF8;
    border:1px solid #DDEEDF;
    border-radius:10px;
    text-align:center;">

    <p style="margin:0;font-size:14px;color:#666;">
    Password Reset Code
    </p>

    <div style="
    margin-top:14px;
    font-size:34px;
    font-weight:bold;
    letter-spacing:8px;
    color:#2E7D32;
    font-family:Consolas,monospace;">
    {resetToken}
    </div>

    <p style="margin-top:18px;color:#777;font-size:14px;">
    ⏳ This code will expire in <strong>1 hour</strong>.
    </p>

    </div>

    <div style="
    background:#FFF8E6;
    border-left:5px solid #F9A825;
    padding:18px;
    border-radius:6px;">

    <strong>Security Notice</strong>

    <p style="margin:10px 0 0;color:#555;line-height:1.7;">
    If you did not request a password reset, you can safely ignore this email.
    Your password will remain unchanged unless this verification code is used.
    </p>

    </div>

    <p style="margin-top:35px;font-size:15px;color:#555;line-height:1.8;">
    Protecting your account helps maintain a secure and transparent agricultural supply chain for everyone.
    </p>

    </td>
    </tr>

    <!-- Footer -->
    <tr>
    <td
    style="
    background:#F7F9F7;
    padding:24px;
    text-align:center;
    font-size:13px;
    color:#777;">

    <strong style="color:#2E7D32;">AgriTrace Team</strong><br>

    Agricultural Supply Chain Traceability System<br><br>

    This is an automated email. Please do not reply directly to this message.

    </td>
    </tr>

    </table>

    </td>
    </tr>
    </table>

    </body>
    </html>
    """);
    }
}

namespace AgriTrace.Application.Emails;

public static class WelcomeEmailTemplate
{
    public static (string subject, string body) Build(string fullName, string email, string password)
    {
        return (
            "🌱 Welcome to AgriTrace",
            $"""
    <!DOCTYPE html>
    <html lang="en">
    <head>
    <meta charset="UTF-8">
    <title>Welcome to AgriTrace</title>
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
    Welcome, {fullName}!
    </h2>

    <p style="font-size:15px;color:#555;line-height:1.8;">
    Your AgriTrace account has been successfully created.
    You can now access the platform to manage products,
    record supply chain activities, and improve transparency
    through every stage of the agricultural lifecycle.
    </p>

    <table width="100%"
    style="background:#F8FBF8;border:1px solid #DDEEDF;
    border-radius:8px;margin:30px 0;">
    <tr>
    <td style="padding:24px;">

    <h3 style="margin-top:0;color:#2E7D32;">
    🔑 Your Login Credentials
    </h3>

    <table cellpadding="8" cellspacing="0">
    <tr>
    <td><strong>Email</strong></td>
    <td>{email}</td>
    </tr>

    <tr>
    <td><strong>Temporary Password</strong></td>
    <td><code style="background:#eef6ef;padding:4px 8px;border-radius:4px;">
    {password}
    </code></td>
    </tr>
    </table>

    </td>
    </tr>
    </table>

    <div style="
    background:#FFF8E6;
    border-left:5px solid #F9A825;
    padding:18px;
    border-radius:6px;
    ">

    <strong>Security Recommendation</strong>

    <p style="margin:10px 0 0;color:#555;line-height:1.7;">
    Please sign in using the credentials above and
    change your temporary password immediately after
    your first login to keep your account secure.
    </p>

    </div>

    <p style="margin-top:35px;font-size:15px;color:#555;line-height:1.8;">
    Thank you for choosing <strong>AgriTrace</strong> to
    support transparent and trustworthy agricultural supply chains.
    Together, we build confidence from farm to consumer.
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
    color:#777;
    ">

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

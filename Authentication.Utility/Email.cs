using System;

namespace Authentication.Utility;

public static class Email
{
    public static string GetWelcomeEmailTemplate(string firstName)
    {
        var htmlTemplate = 
"<!DOCTYPE html>\n<html lang=\"en\">\n<head>\n    <meta charset=\"UTF-8\" />\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />\n    <title>Welcome Email</title>\n    <style>\n        body {\n            margin: 0;\n            padding: 0;\n            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;\n            background-color: #f4f4f4;\n        }\n\n        .email-container {\n            max-width: 600px;\n            margin: 0 auto;\n            background-color: #ffffff;\n            padding: 40px;\n            border-radius: 8px;\n            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);\n        }\n\n        .header {\n            text-align: center;\n            padding-bottom: 20px;\n            border-bottom: 1px solid #e0e0e0;\n        }\n\n        .header h1 {\n            color: #2c3e50;\n            margin: 0;\n        }\n\n        .content {\n            padding: 20px 0;\n            color: #34495e;\n            font-size: 16px;\n            line-height: 1.6;\n        }\n\n        .footer {\n            text-align: center;\n            margin-top: 40px;\n            font-size: 12px;\n            color: #888;\n        }\n    </style>\n</head>\n\n<body>\n<div class=\"email-container\">\n    <div class=\"header\">\n        <h1>Welcome to {{AppName}}</h1>\n    </div>\n\n    <div class=\"content\">\n        <p>Hi {{FirstName}},</p>\n\n        <p>\n            Thank you for signing up with <strong>{{AppName}}</strong>! We're\n            thrilled to have you on board.\n        </p>\n\n        <p>Cheers,<br />The {{AppName}} Team</p>\n    </div>\n\n    <div class=\"footer\">\n        &copy; {{CurrentYear}} {{AppName}}. All rights reserved.\n    </div>\n</div>\n</body>\n</html>\n";
        var emailHtml = htmlTemplate
            .Replace("{{FirstName}}", firstName)
            .Replace("{{AppName}}", "Authentication App")
            .Replace("{{CurrentYear}}", DateTime.UtcNow.Year.ToString());
        return emailHtml;
    }
}

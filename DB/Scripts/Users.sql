CREATE TABLE Users (
                       Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                       FirstName VARCHAR(100) NOT NULL,
                       LastName VARCHAR(100) NOT NULL,
                       Email VARCHAR(150) UNIQUE NOT NULL,
                       PhoneNumber VARCHAR(15) UNIQUE NOT NULL,
                       PasswordHash TEXT NOT NULL,
                       IsEmailVerified BOOLEAN DEFAULT FALSE,
                       IsPhoneVerified BOOLEAN DEFAULT FALSE,
                       PreferredLanguage VARCHAR(10) DEFAULT 'en',
                       Role VARCHAR(20) DEFAULT 'User',
                       IsActive BOOLEAN DEFAULT TRUE,
                       CreatedAt TIMESTAMPTZ DEFAULT now(),
                       UpdatedAt TIMESTAMPTZ DEFAULT now()
);
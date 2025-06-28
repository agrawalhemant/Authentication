CREATE TABLE UserRefreshTokens (
                                   Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                   UserId UUID REFERENCES Users(Id) ON DELETE CASCADE,
                                   RefreshToken TEXT NOT NULL,
                                   ExpiresAt TIMESTAMPTZ NOT NULL,
                                   RevokedAt TIMESTAMPTZ,
                                   UserAgent TEXT,
                                   IPAddress VARCHAR(50),
                                   CreatedAt TIMESTAMPTZ DEFAULT now()
);

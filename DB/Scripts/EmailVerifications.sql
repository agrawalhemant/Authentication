CREATE TABLE EmailVerifications (
                                    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                    UserId UUID REFERENCES Users(Id) ON DELETE CASCADE,
                                    VerificationToken TEXT NOT NULL,
                                    ExpiresAt TIMESTAMPTZ NOT NULL,
                                    IsUsed BOOLEAN DEFAULT FALSE,
                                    CreatedAt TIMESTAMPTZ DEFAULT now()
);

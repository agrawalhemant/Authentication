CREATE TABLE PhoneVerifications (
                                    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                    UserId UUID REFERENCES Users(Id) ON DELETE CASCADE,
                                    OTP VARCHAR(10) NOT NULL,
                                    ExpiresAt TIMESTAMPTZ NOT NULL,
                                    IsUsed BOOLEAN DEFAULT FALSE,
                                    CreatedAt TIMESTAMPTZ DEFAULT now()
);

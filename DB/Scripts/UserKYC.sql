CREATE TABLE UserKYC (
                         Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                         UserId UUID REFERENCES Users(Id) ON DELETE CASCADE,
                         AadharNumber VARCHAR(20),
                         PANNumber VARCHAR(10),
                         GSTIN VARCHAR(20),
                         KYCStatus VARCHAR(20) DEFAULT 'Pending',  -- 'Pending', 'Verified', 'Rejected'
                         VerifiedAt TIMESTAMPTZ,
                         CreatedAt TIMESTAMPTZ DEFAULT now(),
                         UpdatedAt TIMESTAMPTZ DEFAULT now()
);

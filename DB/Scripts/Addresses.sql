CREATE TABLE Addresses (
                           Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                           UserId UUID REFERENCES Users(Id) ON DELETE CASCADE,
                           AddressType VARCHAR(20) DEFAULT 'Home',  -- 'Home', 'Work', etc.
                           StreetAddress TEXT NOT NULL,
                           Locality VARCHAR(100),
                           District VARCHAR(100),
                           City VARCHAR(100),
                           State VARCHAR(100),
                           Pincode VARCHAR(10),
                           Landmark TEXT,
                           Latitude DECIMAL(9,6),
                           Longitude DECIMAL(9,6),
                           CreatedAt TIMESTAMPTZ DEFAULT now(),
                           UpdatedAt TIMESTAMPTZ DEFAULT now()
);

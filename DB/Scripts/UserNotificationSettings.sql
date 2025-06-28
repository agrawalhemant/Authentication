CREATE TABLE UserNotificationSettings (
                                          Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                          UserId UUID REFERENCES Users(Id) ON DELETE CASCADE,
                                          MarketingEmailsEnabled BOOLEAN DEFAULT TRUE,
                                          ProductUpdatesEnabled BOOLEAN DEFAULT TRUE,
                                          SMSAlertsEnabled BOOLEAN DEFAULT TRUE,
                                          CreatedAt TIMESTAMPTZ DEFAULT now(),
                                          UpdatedAt TIMESTAMPTZ DEFAULT now()
);



INSERT INTO [dbo].[Adventure] (
    [UserProfileId] , 
    [Title]         ,
    [Text]          ,
    [Difficulty]    ,
    [Datetime]      ,
    [CreatedDate]   )
	VALUES (
	1 , 
    'Test 2'         ,
    '1 Text Text Text'          ,
    3    ,
    GETDATE() ,
    GETDATE()   )
	



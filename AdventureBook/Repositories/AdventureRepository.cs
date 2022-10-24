﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdventureBook.Models;
using AdventureBook.Utils;
using Microsoft.Extensions.Configuration;


namespace AdventureBook.Repositories
{
    public class AdventureRepository : BaseRepository, IAdventureRepository
    {
        public AdventureRepository(IConfiguration config) : base(config) { }

        public List<Adventure> GetAllAdventures()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                       SELECT a.Id, a.Title, a.Text, 
                              a.Difficulty, a.DateTime, a.CreatedDate, 
                               a.UserProfileId,
                              u.Name, u.Email
                        FROM Adventure a
                              LEFT JOIN UserProfile u ON a.UserProfileId = u.id";
                    var reader = cmd.ExecuteReader();

                    var posts = new List<Adventure>();

                    while (reader.Read())
                    {
                        posts.Add(NewPostFromReader(reader));
                    }

                    reader.Close();

                    return posts;
                }
            }
        }
        private Adventure NewPostFromReader(SqlDataReader reader)
        {
            return new Adventure()
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Text = reader.GetString(reader.GetOrdinal("Text")),
                Difficulty = reader.GetInt32(reader.GetOrdinal("Difficulty")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                DateTime = (DateTime)DbUtils.GetNullableDateTime(reader, "DateTime"),
                UserProfileId = reader.GetInt32(reader.GetOrdinal("UserProfileId")),
                UserProfile = new UserProfile()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("UserProfileId")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Email = reader.GetString(reader.GetOrdinal("Email"))
                }
            };
        }
              //Create an adventure

                /*INSERT INTO [dbo].[Adventure] (
                [UserProfileId] , 
            [Title]         ,
            [Text]          ,
            [Difficulty]    ,
            [Datetime]      ,
            [CreatedDate]   )
	        VALUES(
	        1 ,
            'Test 2'         ,
            '1 Text Text Text'          ,
            3    ,
            GETDATE() ,
            GETDATE()   )

                */
    }
}
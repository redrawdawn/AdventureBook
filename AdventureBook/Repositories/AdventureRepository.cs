using System;
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
        private readonly ITagRepository _tagRepository;

        public AdventureRepository(IConfiguration config, ITagRepository tagRepository) : base(config)
        {
            _tagRepository = tagRepository;
        }

        //Get All Adventures
        public List<Adventure> GetAllAdventures(string searchString = null)
        {
            var isSearch = searchString != null;
            var searchClause = isSearch
                ? "WHERE a.Title like @searchParam"
                : "";

            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @$"
                       SELECT a.Id, a.Title, a.Text, 
                              a.Difficulty, a.DateTime, a.CreatedDate, 
                               a.UserProfileId,
                              u.Name, u.Email
                        FROM Adventure a
                              LEFT JOIN UserProfile u ON a.UserProfileId = u.id
                        {searchClause}
                        ORDER BY a.CreatedDate DESC";

                    if (isSearch) cmd.Parameters.AddWithValue("@searchParam", $"%{searchString}%");

                    var reader = cmd.ExecuteReader();

                    var posts = new List<Adventure>();

                    while (reader.Read())
                    {
                        posts.Add(NewPostFromReader(reader));
                    }

                    foreach (var post in posts)
                    {
                        post.Tags = _tagRepository.GetTagsByAdventureId(post.Id);
                    }

                    reader.Close();

                    return posts;
                }
            }
        }

        //Get Current Users Adventures
        public List<Adventure> GetUserAdventures(int userProfileId)
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
                              LEFT JOIN UserProfile u ON a.UserProfileId = u.id
                        WHERE a.UserProfileId = @userProfileId
                        ORDER BY a.CreatedDate DESC";

                    cmd.Parameters.AddWithValue("@userProfileId", userProfileId);
                    var reader = cmd.ExecuteReader();

                    var posts = new List<Adventure>();

                    while (reader.Read())
                    {
                        posts.Add(NewPostFromReader(reader));
                    }

                    foreach (var post in posts)
                    {
                        post.Tags = _tagRepository.GetTagsByAdventureId(post.Id);
                    }

                    reader.Close();

                    return posts;
                }
            }
        }

        //Get Adventure by id
        public Adventure GetAdventureById(int id)
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
                              LEFT JOIN UserProfile u ON a.UserProfileId = u.id
                        WHERE a.Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);
                    var reader = cmd.ExecuteReader();

                    Adventure adventure = null;

                    if (reader.Read())                 
                    {
                        adventure = NewPostFromReader(reader);
                    };                   

                    reader.Close();

                    return adventure;
                }
            }
        }

        private Adventure NewPostFromReader(SqlDataReader reader)
        {
            return new Adventure()
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Title = DbUtils.GetNullableString(reader, "Title"),
                Text = DbUtils.GetNullableString(reader, "Text"),
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
        public void Add(Adventure adventure)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    adventure.CreatedDate = DateTime.Now;
                    cmd.CommandText = @"
                        INSERT INTO Adventure (
                            UserProfileId, 
                            Title,
                            Text,
                            Difficulty,
                            Datetime,
                            CreatedDate )
                        OUTPUT INSERTED.ID 
                        VALUES (
                            @UserProfileId, @Title, @Text, @Difficulty, @Datetime, @CreatedDate)";
                    cmd.Parameters.AddWithValue("@UserProfileId", adventure.UserProfileId);
                    cmd.Parameters.AddWithValue("@Title", DbUtils.ValueOrDBNull(adventure.Title));
                    cmd.Parameters.AddWithValue("@Text", DbUtils.ValueOrDBNull(adventure.Text));
                    cmd.Parameters.AddWithValue("@Difficulty", DbUtils.ValueOrDBNull(adventure.Difficulty));
                    cmd.Parameters.AddWithValue("@Datetime", DbUtils.ValueOrDBNull (adventure.DateTime));
                    cmd.Parameters.AddWithValue("@CreatedDate", DbUtils.ValueOrDBNull(adventure.CreatedDate));

                    adventure.Id = (int)cmd.ExecuteScalar();
                }

                SaveAdventureTags(adventure.Id, adventure.SelectedTagIds);
            }
        }

        //Edit Adventure
        public void UpdateAdventure(Adventure adventure)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            UPDATE Adventure
                            SET 
                                [Title] = @Title,
                                [Text] = @Text,
                                [Difficulty] = @Difficulty,
                                [Datetime] = @DateTime
                            WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@Id", adventure.Id);
                    cmd.Parameters.AddWithValue("@Title", DbUtils.ValueOrDBNull(adventure.Title));
                    cmd.Parameters.AddWithValue("@Text", DbUtils.ValueOrDBNull(adventure.Text));
                    cmd.Parameters.AddWithValue("@Difficulty", DbUtils.ValueOrDBNull(adventure.Difficulty));
                    cmd.Parameters.AddWithValue("@DateTime", DbUtils.ValueOrDBNull(adventure.DateTime));
                    
                    cmd.ExecuteNonQuery();
                }

                SaveAdventureTags(adventure.Id, adventure.SelectedTagIds);
            }
        }

        public void SaveAdventureTags(int adventureId, List<int> tagIds)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                // Delete all tags for Adventure
                DeleteTagsForAdventure(adventureId, conn);

                // Insert each tag
                foreach (var tagId in tagIds)
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        // insert into AdventureTag
                        cmd.CommandText = @"INSERT INTO [dbo].[AdventureTag]
                                                   ([AdventureId]
                                                   ,[TagId])
                                             VALUES
                                                   (@adventureId
                                                   ,@tagId)
                                        ";
                        cmd.Parameters.AddWithValue("@adventureId", adventureId);
                        cmd.Parameters.AddWithValue("@tagId", tagId);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

        }

        private static void DeleteTagsForAdventure(int adventureId, SqlConnection conn)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"DELETE FROM [dbo].[AdventureTag]
                                        WHERE AdventureId = @adventureId";
                cmd.Parameters.AddWithValue("@adventureId", adventureId);

                cmd.ExecuteNonQuery();
            }
        }

        //Delete Adventure
        public void Delete(int adventureId)
        {
            using (var conn = Connection)
            {
                conn.Open();

                DeleteTagsForAdventure(adventureId, conn);

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM adventure
                                        WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", adventureId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
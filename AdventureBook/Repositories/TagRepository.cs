using AdventureBook.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureBook.Repositories
{
    public class TagRepository : BaseRepository, ITagRepository
    {
        public TagRepository(IConfiguration config) : base(config) { }

        //GetAll
        public List<Tag> GetAllTags()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                       SELECT t.Id, t.Name
                        FROM Tag t";
                    var reader = cmd.ExecuteReader();

                    var items = new List<Tag>();

                    while (reader.Read())
                    {
                        var item = new Tag
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };
                        items.Add(item);
                    }

                    reader.Close();

                    return items;
                }
            }
        }
        public List<Tag> GetTagsByAdventureId(int adventureId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT t.Id, t.Name
                        FROM Tag t
                        JOIN AdventureTag [at] ON [at].TagId = t.Id
                        WHERE [at].AdventureId = @adventureId";
                    cmd.Parameters.AddWithValue("@adventureId", adventureId);
                    var reader = cmd.ExecuteReader();

                    var items = new List<Tag>();

                    while (reader.Read())
                    {
                        var item = new Tag
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal(nameof(Tag.Name)))
                        };
                        items.Add(item);
                    }

                    reader.Close();

                    return items;
                }
            }
        }
    }
    public interface ITagRepository
    {
        //GetAll
        public List<Tag> GetAllTags();
        public List<Tag> GetTagsByAdventureId(int adventureId);
    }
}

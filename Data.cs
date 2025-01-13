using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace NeoEditor
{
    /// <summary>
    /// Summary description for Data.
    /// </summary>
    public static class Data
    {
        private static string m_ConnString = Config.ConnectionString;

        public static class FilePatches
        {
            public static void CreateTable()
            {
                string createTableSQL =
                @"CREATE TABLE IF NOT EXISTS [FilePatchHistory] (
                    [Id] INTEGER PRIMARY KEY NOT NULL,
                    [FilePath] VARCHAR(1000) NOT NULL,
                    [VersionNo] INTEGER NOT NULL,
                    [BackwardPatch] TEXT NULL,
                    [ForwardPatch] TEXT NULL
                )";

                using (SQLiteConnection conn = new SQLiteConnection(m_ConnString))
                {
                    try
                    {
                        conn.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(createTableSQL, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (SQLiteException ex)
                    {}
                }
            }

            public static int Create(string FilePath, string Patch)
            {
                int versionNo = 0;
                bool patchExists = false;
                string selectSQL1 = @"SELECT 1 FROM FilePatchHistory WHERE FilePath = @FilePath";
                string selectSQL2 = @"SELECT MAX(VersionNo) FROM FilePatchHistory WHERE FilePath = @FilePath";
                string insertSQL1 = @"INSERT INTO FilePatchHistory (FilePath, VersionNo) VALUES (@FilePath, 0)";
                string insertSQL2 = @"INSERT INTO FilePatchHistory (FilePath, VersionNo, BackwardPatch) VALUES (@FilePath, @VersionNo, @Patch)";

                using (SQLiteConnection conn = new SQLiteConnection(m_ConnString))
                {
                    try
                    {
                        conn.Open();

                        using (SQLiteCommand cmd = new SQLiteCommand(selectSQL1, conn))
                        {
                            cmd.Parameters.AddWithValue("@FilePath", FilePath);
                            using (SQLiteDataReader dr = cmd.ExecuteReader())
                            {
                                if (dr.Read())
                                {
                                    if (dr[0].GetType() != typeof(DBNull))
                                        patchExists = true;
                                }
                            }
                        }

                        if (patchExists)
                        {
                            using (SQLiteCommand cmd = new SQLiteCommand(selectSQL2, conn))
                            {
                                cmd.Parameters.AddWithValue("@FilePath", FilePath);
                                using (SQLiteDataReader dr = cmd.ExecuteReader())
                                {
                                    dr.Read();
                                    versionNo = dr.GetInt32(0) + 1;
                                }
                            }
                        }
                        else
                        {
                            using (SQLiteCommand cmd = new SQLiteCommand(insertSQL1, conn))
                            {
                                cmd.Parameters.AddWithValue("@FilePath", FilePath);
                                cmd.ExecuteNonQuery();
                                versionNo = 1;
                            }
                        }

                        using (SQLiteCommand cmd = new SQLiteCommand(insertSQL2, conn))
                        {
                            cmd.Parameters.AddWithValue("@FilePath", FilePath);
                            cmd.Parameters.AddWithValue("@VersionNo", versionNo);
                            cmd.Parameters.AddWithValue("@Patch", Patch);

                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (SQLiteException ex)
                    {}
                }

                return versionNo;
            }

            public static int Update(string FilePath, int VersionNo, string ForwardPatch)
            {
                int res = 0;
                string updateSQL =
                @"UPDATE FilePatchHistory 
                  SET ForwardPatch = @ForwardPatch
		          WHERE (FilePath = @FilePath) AND (VersionNo = @VersionNo)
                ";

                using (SQLiteConnection conn = new SQLiteConnection(m_ConnString))
                {
                    try
                    {
                        conn.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(updateSQL, conn))
                        {
                            cmd.Parameters.AddWithValue("@FilePath", FilePath);
                            cmd.Parameters.AddWithValue("@VersionNo", VersionNo);
                            cmd.Parameters.AddWithValue("@ForwardPatch", ForwardPatch);

                            res = cmd.ExecuteNonQuery();
                        }
                    }
                    catch (SQLiteException ex)
                    { }
                }

                return res;
            }

            public static DataItem Get(string FilePath, int? VersionNo)
            {
                DataItem dItem = null;
                string selectSQL1 = @"SELECT MAX(VersionNo) FROM FilePatchHistory WHERE FilePath = @FilePath";
                string selectSQL2 =
                @"SELECT 
                    Id, FilePath, VersionNo, BackwardPatch, ForwardPatch
                FROM
                    FilePatchHistory
                WHERE
                    (FilePath = @FilePath) AND (VersionNo = @VersionNo)
                ";

                using (SQLiteConnection conn = new SQLiteConnection(m_ConnString))
                {
                    try
                    {
                        conn.Open();

                        if (VersionNo == null)
                        {
                            using (SQLiteCommand cmd = new SQLiteCommand(selectSQL1, conn))
                            {
                                cmd.Parameters.AddWithValue("@FilePath", FilePath);
                                using (SQLiteDataReader dr = cmd.ExecuteReader())
                                {
                                    if (dr.Read())
                                    {
                                        if (dr[0].GetType() != typeof(DBNull))
                                            VersionNo = dr.GetInt32(0);
                                    }
                                }
                            }
                        }
                        
                        using (SQLiteCommand cmd = new SQLiteCommand(selectSQL2, conn))
                        {
                            cmd.Parameters.AddWithValue("@FilePath", FilePath);
                            cmd.Parameters.AddWithValue("@VersionNo", VersionNo);

                            using (SQLiteDataReader dr = cmd.ExecuteReader())
                            {
                                if (dr.Read())
                                {
                                    if (dr[0].GetType() != typeof(DBNull))
                                        dItem = new DataItem(dr);
                                }
                            }
                        }
                    }
                    catch (SQLiteException ex)
                    {}
                }

                return dItem;
            }

            public class DataItem
            {
                public DataItem(SQLiteDataReader reader)
                {
                    this.Id = Convert.ToInt32(reader["Id"]);
                    this.FilePath = reader["FilePath"].ToString();
                    this.VersionNo = Convert.ToInt32(reader["VersionNo"]);
                    this.BackwardPatch = reader["BackwardPatch"] == DBNull.Value ? "" : reader["BackwardPatch"].ToString();
                    this.ForwardPatch = reader["ForwardPatch"] == DBNull.Value ? "" : reader["ForwardPatch"].ToString();
                }

                public int Id { get; private set; }
                public string FilePath { get; private set; }
                public int VersionNo { get; private set; }
                public string BackwardPatch { get; private set; }
                public string ForwardPatch { get; private set; }
            }
        }
    }
}
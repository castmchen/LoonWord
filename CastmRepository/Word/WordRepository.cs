using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastmRepository
{
    public class WordRepository
    {
        private SqliteBaseRepository sqlliteBaseRepository = new SqliteBaseRepository();

        public string TableName { get; set; }

        public WordRepository(string tableName)
        {
            this.TableName = tableName;
        }

        public void CreateDBAndTable()
        {
            sqlliteBaseRepository.CreateNewDatabase();
            sqlliteBaseRepository.ConnectToDatabase();
            sqlliteBaseRepository.CreateTable();
        }

        public List<WordDomain> FindAll()
        {
            var query = $"select * from {this.TableName} ";
            var dataSet = SqliteRepository.ExecuteDataSet(query, null);
            return this.ConvertDataSetToWordList(dataSet);
        }

        public WordDomain FindById(Guid id)
        {
            var query = $"select * from {this.TableName} where Flag = '{id.ToString()}' ";
            var dataSet = SqliteRepository.ExecuteDataSet(query, null);
            return this.ConvertDataSetToWordList(dataSet).FirstOrDefault();
        }

        public List<WordDomain> FindByFlag(int flag)
        {
            var query = $"select * from {this.TableName} where Flag = {flag} ";
            var dataSet = SqliteRepository.ExecuteDataSet(query, null);
            return this.ConvertDataSetToWordList(dataSet);
        }

        public bool InsertOne(WordDomain word)
        {
            try
            {
                var query = "insert into ";
                query += this.TableName;
                query += " (Id, Word, Trascation, Phonetic, Flag, CreateTime) ";
                query += "values (@Id, @Word,@Trascation,@Phonetic,@Flag,@CreateTime)";
                var paramArray = this.ConvertParamsToArray(word);
                var dataSet = SqliteRepository.ExecuteDataSet(query, paramArray);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool InsertOneWithVoice(WordDomain word)
        {
            try
            {
                var query = "insert into ";
                query += query + this.TableName;
                query += query + " (Id, Word, Trascation, Phonetic, Flag, CreateTime, Voice) ";
                query += query + "values (@Id, @Word,@Trascation,@Phonetic,@Flag,@CreateTime,@Voice)";
                var paramArray = this.ConvertParamsToArrayWithVoice(word);
                var dataSet = SqliteRepository.ExecuteDataSet(query, paramArray);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public List<WordDomain> InsertAll(List<WordDomain> wordList, bool ifHasVoice = false)
        {
            var result = new List<WordDomain>();
            foreach (var workItem in wordList)
            {
                if (!this.InsertOne(workItem))
                {
                    result.Add(workItem);
                }
            }
            return result;
        }

        private List<WordDomain> ConvertDataSetToWordList(DataSet dataSet)
        {
            var result = new List<WordDomain>();
            foreach (DataTable dataTable in dataSet.Tables)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    var domain = new WordDomain();
                    domain.Id = new Guid(dataRow[0].ToString());
                    domain.Word = dataRow[1].ToString();
                    domain.Trascation = dataRow[2].ToString();
                    domain.Phonetic = dataRow[3].ToString();
                    domain.Voice = dataRow[4].ToString();
                    domain.Flag = (int)dataRow[5];
                    domain.CreateTime = (long)dataRow[6];
                    result.Add(domain);
                    //for (int i = 0; i < dataTable.Columns.Count; i++)
                    //{
                    //    var bb = dataRow[i];
                    //}
                }
            }
            return result;
        }

        private object[] ConvertParamsToArray(WordDomain word)
        {
            var array = new object[] { word.Id.ToString(), word.Word, word.Trascation, word.Phonetic, word.Flag, word.CreateTime };
            return array;
        }

        private object[] ConvertParamsToArrayWithVoice(WordDomain word)
        {
            var array = new object[] { word.Id.ToString(), word.Word, word.Trascation, word.Phonetic, word.Flag, word.CreateTime, word.Voice };
            return array;
        }

        public List<WordDomain> GetWordListByCount(int count)
        {
            var query = $"select * from {this.TableName} where Flag = 0 limit {count}";
            var dataSet = SqliteRepository.ExecuteDataSet(query, null);
            return this.ConvertDataSetToWordList(dataSet);
        }

        public WordDomain GetOne()
        {
            var query = $"select * from {this.TableName} where Flag = 0 limit 1";
            var dataSet = SqliteRepository.ExecuteDataSet(query, null);
            return this.ConvertDataSetToWordList(dataSet).FirstOrDefault();
        }

        public WordDomain GetReadedOne()
        {
            var query = $"select * from {this.TableName} where Flag = 1 ORDER BY CreateTime DESC LIMIT 1";
            var dataSet = SqliteRepository.ExecuteDataSet(query, null);
            return this.ConvertDataSetToWordList(dataSet).FirstOrDefault();
        }

        public WordDomain GetOneExcludeById(Guid Id)
        {
            try
            {
                var newId = Id.ToString();
                var query = $"select * from {this.TableName} where Flag = 0 and Id != '{newId}' ORDER BY RANDOM() limit 1";
                var dataSet = SqliteRepository.ExecuteDataSet(query, null);
                return this.ConvertDataSetToWordList(dataSet).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public WordDomain GetReverseOneExcludeId(Guid Id)
        {
            try
            {
                var newId = Id.ToString();
                var query = $"select * from {this.TableName} where Flag = 0 and Id != '{newId}' ORDER BY CreateTime desc limit 1";
                var dataSet = SqliteRepository.ExecuteDataSet(query, null);
                return this.ConvertDataSetToWordList(dataSet).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void UpdateFlagById(Guid id, int flag)
        {
            var query = $"update {this.TableName} set Flag = {flag}, CreateTime = {DateTime.Now.Ticks} where Id = '{id.ToString()}' ";
            var dataSet = SqliteRepository.ExecuteDataSet(query, null);
        }

        public void UpdateVoiceById(Guid id, string voiceStr, string ph)
        {
            if (!string.IsNullOrEmpty(ph))
            {
                ph.Replace("'", "''");
            }
            try
            {
                var query = $"update {this.TableName} set Voice = '{voiceStr}', Phonetic = '{ph}' where Id = '{id.ToString()}' ";
                var dataSet = SqliteRepository.ExecuteDataSet(query, null);
            }
            catch (Exception)
            {
                var query = $"update {this.TableName} set Voice = '{voiceStr}' where Id = '{id.ToString()}' ";
                var dataSet = SqliteRepository.ExecuteDataSet(query, null);
            }
        }

        public void ResetFlag()
        {
            var query = $"update {this.TableName} set Flag = 0 where Flag = 2 ";
            var dataSet = SqliteRepository.ExecuteDataSet(query, null);
        }
    }
}

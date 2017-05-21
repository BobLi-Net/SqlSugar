﻿using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.UnitTest
{
    public class Update : UnitTestBase
    {
        private Update() { }
        public Update(int eachCount)
        {
            this.Count = eachCount;
        }

        public void Init()
        {
            var db = GetInstance();
            var updateObj = new Student() { Id = 1, Name = "jack", CreateTime = Convert.ToDateTime("2017-05-21 09:56:12.610") };
            var updateObjs = new List<Student>() { updateObj,new Student() { Id=2,Name="sun" } }.ToArray();
            db.IgnoreColumns.Add("TestId", "Student");
            //db.MappingColumns.Add("id","dbid", "Student");

            var t1 = db.Updateable(updateObj).ToSql();
            base.Check(@"UPDATE [Student]  SET
           [SchoolId]=@SchoolId,[Name]=@Name,[CreateTime]=@CreateTime  WHERE [Id]=@Id", new List<SugarParameter>() {
                           new SugarParameter("@SchoolId",0),
                           new SugarParameter("@Id",1),
                           new SugarParameter("@CreateTime", Convert.ToDateTime("2017-05-21 09:56:12.610")),
                           new SugarParameter("@Name", "jack")
            }, t1.Key, t1.Value,"Update t1 error");

            //update reutrn Command Count
            var t2 = db.Updateable(updateObj).ExecuteCommand();

            db.IgnoreColumns = null;
            //Only  update  Name 
            var t3 = db.Updateable(updateObj).UpdateColumns(it => new { it.Name }).ToSql();
            base.Check(@"UPDATE [Student]  SET
           [Name]=@Name  WHERE [Id]=@Id", new List<SugarParameter>() {
                           new SugarParameter("@Id",1),
                           new SugarParameter("@Name", "jack")
            }, t3.Key, t3.Value, "Update t3 error");

            //Ignore  Name and TestId
            var t4 = db.Updateable(updateObj).IgnoreColumns(it => new { it.Name, it.TestId }).ToSql();
            base.Check(@"UPDATE [Student]  SET
           [SchoolId]=@SchoolId,[CreateTime]=@CreateTime  WHERE [Id]=@Id", new List<SugarParameter>() {
                           new SugarParameter("@CreateTime",Convert.ToDateTime("2017-05-21 09:56:12.610")),
                           new SugarParameter("@SchoolId", 0),
                           new SugarParameter("@Id",1),
            }, t4.Key, t4.Value, "Update t4 error");

            //Ignore  Name and TestId
            var t5 = db.Updateable(updateObj).IgnoreColumns(it => it == "Name" || it == "TestId").With(SqlWith.UpdLock).ToSql();
            base.Check(@"UPDATE [Student] WITH(UPDLOCK)  SET
           [SchoolId]=@SchoolId,[CreateTime]=@CreateTime  WHERE [Id]=@Id", new List<SugarParameter>() {
                           new SugarParameter("@CreateTime",Convert.ToDateTime("2017-05-21 09:56:12.610")),
                           new SugarParameter("@SchoolId", 0),
                           new SugarParameter("@Id",1),
            }, t5.Key, t5.Value, "Update t5 error");


            //Use Lock
            var t6 = db.Updateable(updateObj).With(SqlWith.UpdLock).ToSql();
            base.Check(@"UPDATE [Student] WITH(UPDLOCK)  SET
           [SchoolId]=@SchoolId,[Name]=@Name,[CreateTime]=@CreateTime,[TestId]=@TestId  WHERE [Id]=@Id", new List<SugarParameter>() {
                           new SugarParameter("@SchoolId",0),
                           new SugarParameter("@Id",1),
                           new SugarParameter("@TestId",0),
                           new SugarParameter("@CreateTime", Convert.ToDateTime("2017-05-21 09:56:12.610")),
                           new SugarParameter("@Name", "jack")
            }, t6.Key, t6.Value, "Update t6 error");


            //update List<T>
            var t7 = db.Updateable(updateObjs).With(SqlWith.UpdLock).ToSql();

            //Re Set Value
            var s9 = db.Updateable(updateObj)
                .ReSetValue(it=>it.Name==(it.SchoolId+"")).ToSql();

            //Where By Expression
            var s10 = db.Updateable(updateObj)
           .Where(it => it.Id==1).ToSql();
        }

        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new SystemTablesConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.SqlServer, IsAutoCloseConnection = true });
            return db;
        }
    }
}

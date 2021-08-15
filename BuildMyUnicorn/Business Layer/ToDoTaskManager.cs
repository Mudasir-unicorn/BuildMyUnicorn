﻿using ALMS_DAL;
using Business_Model;
using Business_Model.Helper;
using Business_Model.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace BuildMyUnicorn.Business_Layer
{
    public  class ToDoTaskManager
    {
        private readonly string SaveQuery = "INSERT into tbl_todo_task ({0}) VALUES ({1})";

        private readonly string UpdateQuery = "UPDATE tbl_todo_task SET {0} WHERE ToDOTaskID = @ToDOTaskID ";

        private readonly string DeleteQuery = "DELETE tbl_todo_task WHERE ToDOTaskID = @ToDOTaskID ";

        private readonly string ItemQuery = "SELECT *FROM tbl_todo_task where isDeleted = 0";

        private readonly string ClientTeamQuery = "SELECT ClientID As [Key], FirstName +' ' + LastName As Value  FROM tbl_client ";

        private readonly AssignedMappingManager assignedMappingManager;

        public ToDoTaskManager()
        {
            assignedMappingManager = new AssignedMappingManager();
        }

        public List<ListItem<Guid, string>> GetTeamMembers()
        {
            var clientId = Guid.Parse(HttpContext.Current.User.Identity.Name);
            return SharedManager.GetList<ListItem<Guid, string>>(ClientTeamQuery + $"WHERE TeamClientID = '{clientId}' OR ClientID = '{clientId}' ").ToList();
        }

        public ToDoTask GetTodoItem(Guid id)
        {
            var todo = SharedManager.GetSingle<ToDoTask>(ItemQuery + $" WHERE ToDOTaskID = '{id}'");
            todo.AssignedMappings = assignedMappingManager.GetAssignedMapping(id);
            return todo;
        }

        public string AddTodo(ToDoTask Model)
        {
            DataLayer obj = new DataLayer(ConfigurationManager.ConnectionStrings["ConnectionBuildMyUnicorn"].ConnectionString, Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeOut"]));
            List<ParametersCollection> parameters = new List<ParametersCollection>() {
             new ParametersCollection { ParamterName = "@ToDoTaskID", ParamterValue = Model.ToDoTaskID, ParamterType = DbType.Guid, ParameterDirection = ParameterDirection.Input },
             new ParametersCollection { ParamterName = "@Subject", ParamterValue = Model.Subject, ParamterType = DbType.String, ParameterDirection = ParameterDirection.Input },
             new ParametersCollection { ParamterName = "@StartDate", ParamterValue = Model.StartDate, ParamterType = DbType.DateTime, ParameterDirection = ParameterDirection.Input },
             new ParametersCollection { ParamterName = "@EndDate", ParamterValue = Model.EndDate, ParamterType = DbType.DateTime, ParameterDirection = ParameterDirection.Input },
             new ParametersCollection { ParamterName = "@Reminder", ParamterValue = Model.Reminder, ParamterType = DbType.DateTime, ParameterDirection = ParameterDirection.Input },
             new ParametersCollection { ParamterName = "@Priority", ParamterValue = Model.Priority, ParamterType = DbType.Int16, ParameterDirection = ParameterDirection.Input },
             new ParametersCollection { ParamterName = "@Status", ParamterValue = Model.Status, ParamterType = DbType.Int16, ParameterDirection = ParameterDirection.Input },
             new ParametersCollection { ParamterName = "@Description", ParamterValue = Model.Description, ParamterType = DbType.String, ParameterDirection = ParameterDirection.Input },
             new ParametersCollection { ParamterName = "@AssignedTo", ParamterValue = Model.AssignedTo, ParamterType = DbType.Guid, ParameterDirection = ParameterDirection.Input },
             new ParametersCollection { ParamterName = "@AssignedBy", ParamterValue =  Guid.Parse(HttpContext.Current.User.Identity.Name), ParamterType = DbType.Guid, ParameterDirection = ParameterDirection.Input },
             new ParametersCollection { ParamterName = "@EntityState", ParamterValue = Model.EntityState, ParamterType = DbType.Int16, ParameterDirection = ParameterDirection.Input },
            };
           var result =  obj.ExecuteWithReturnValue(CommandType.StoredProcedure, "sp_add_todo", parameters);
            if (result > 0) return "OK"; else return "Todo  failed";

        }

        //public List<ToDoTask> GetTodoList()
        //{
        //    var clientId = Guid.Parse(HttpContext.Current.User.Identity.Name);
        //    var query = ItemQuery + $" WHERE CreatedBy = '{clientId}' ";
        //    return SharedManager.GetList<ToDoTask>(query,
        //        x =>
        //        {
        //            x.AssignedMappings = assignedMappingManager.GetAssignedMapping(x.ToDoTaskID);
        //        }).ToList();
        //}

        public ToDoTask GetSingleToDo(Guid ToDoTaskID)
        {
           
            var query = $@"select * from tbl_todo_task WHERE ToDoTaskID = '{ToDoTaskID}' ";
            return SharedManager.GetSingle<ToDoTask>(query);
        }

        public List<ToDoDTO> GetAssignedToDoList()
        {
            var clientId = Guid.Parse(HttpContext.Current.User.Identity.Name);
            var query = $@"Select ToDo.ToDoTaskID, ToDo.Subject, ToDO.Description , Todo.AssignedOn, Assigned.Status
                        from tbl_todo_task ToDo INNER JOIN tbl_assigned_mapping
                        Assigned ON EntityID = ToDoTaskID WHERE AssignedToID = '{clientId}' ";

            return SharedManager.GetList<ToDoDTO>(query).ToList();
        }

        public List<ToDoTask> GetTodoList()
        {
            var query = @"Select ToDo.*, Assigned.*, Client.FirstName + ' ' + Client.LastName AS AssignedTo
                        from tbl_todo_task ToDo LEFT JOIN tbl_assigned_mapping
                        Assigned ON EntityID = ToDoTaskID 
                        LEFT JOIN tbl_client Client ON AssignedToID = ClientID ";

            var clientId = Guid.Parse(HttpContext.Current.User.Identity.Name);
            query += $" WHERE ToDo.CreatedBy = '{clientId}' ";

            var todoDictionary = new Dictionary<Guid, ToDoTask>();

            var todoList = SharedManager.GetListWithNavigation<ToDoTask, AssignedMapping, ToDoTask>(query, (todo, mapping) =>
            {
                if (!todoDictionary.TryGetValue(todo.ToDoTaskID, out ToDoTask toDoEntry))
                {
                    toDoEntry = todo;
                    toDoEntry.AssignedMappings = toDoEntry.AssignedMappings ?? new List<AssignedMapping>();
                    todoDictionary.Add(toDoEntry.ToDoTaskID, toDoEntry);
                }

                toDoEntry.AssignedMappings.Add(mapping);
                return toDoEntry;
            }, splitOn: "AssignedMappingID").Distinct().ToList();

            return todoList;
        }

        public int UpdateToDoStatus(ToDoStatus status, Guid toDoTaskId)
        {
            var clientId = Guid.Parse(HttpContext.Current.User.Identity.Name);
            var CompletedOn = status == ToDoStatus.Completed ? (DateTime?)DateTime.UtcNow : null;
            var query = $"UPDATE tbl_assigned_mapping SET Status = {(short)status}, CompletedOn = @CompletedOn WHERE EntityId = '{toDoTaskId}' AND AssignedToID = '{clientId}' ";
            return SharedManager.ExecuteRaw(query, new { CompletedOn });
        }

        public ResponseModel SaveToDo(ToDoTask toDo)
        {
            toDo.ToDoTaskID = Guid.NewGuid();
            toDo.AssignedBy = Guid.Parse(HttpContext.Current.User.Identity.Name);
            toDo.AssignedOn = DateTime.UtcNow;
            toDo.SetBasicProperties();

            var responseModel = SharedManager.Save(toDo, SaveQuery);

            if (!responseModel.HasError)
                assignedMappingManager.BulkAssign(toDo.AssignedMappings, toDo.ToDoTaskID);

            responseModel.EntityID = toDo.ToDoTaskID;
            return responseModel;
        }

        public ResponseModel UpdateToDo(ToDoTask toDo)
        {
            var dbToDo = GetTodoItem(toDo.ToDoTaskID);

            toDo.EntityState = EntityState.Modified;
            toDo.SetBasicProperties();


            var resposne = SharedManager.Update(toDo, UpdateQuery);

            if (!resposne.HasError)
                assignedMappingManager.BulkAssign(toDo.AssignedMappings, toDo.ToDoTaskID);

            resposne.EntityID = toDo.ToDoTaskID;
            return resposne;
        }

        public int DeleteToDo(Guid id)
        {
            var returnValue = SharedManager.Delete(DeleteQuery, new { TodoTaskID = id });
            if (returnValue == 0)
                assignedMappingManager.DeleteByEntity(id);

            return returnValue;
        }

    }
}
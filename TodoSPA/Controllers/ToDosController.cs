using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Data;
using System.Net.Http;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [RoutePrefix("api/users/{userId:int}/todos")]
    public class ToDosController : ApiController
    {
        private TodoContext db = new TodoContext();
        
        [HttpGet]
        [Route("~/api/test")]
        // This action demo's some interesting and helpful options. The "~"
        // (tilde) demonstrates how to overide the class level [RoutePrefix] 
        //  attribute. And also use of the IHttpActionResult result interface.
        public IHttpActionResult Get()
        {
            return Ok(new List<int>() { 1, 2, 3 });
        }

        [HttpPost]
        [Route("")]
        // Handles requests for CRUD "Create"
        public HttpResponseMessage AddTodoByUser(int userId, [FromBody] tblTodo todo)
        {
            var todos = db.tblTodos;
            tblTodo newTodo = new tblTodo();
            newTodo.fkUserId = todo.fkUserId;
            newTodo.DateCreated = DateTime.Now;
            newTodo.Complete = false;
            newTodo.Title = todo.Title;
            newTodo.Details = todo.Details;
            todos.Add(newTodo);
            //if (ModelState.IsValid)
            // also see https://stackoverflow.com/questions/7101800/can-i-check-modelstate-without-modelbinding
            db.SaveChanges();
            var response = Request.CreateResponse(HttpStatusCode.OK, newTodo);
            return response;
        }

        [HttpGet]
        [Route("{todoId:int?}")]
        // Handles requests for CRUD "Read". "todoId" is optional, if it's null, all todos are 
        //   returned, otherwise the list will contain a single todo. 
        public IQueryable<tblTodo> GetTodosByUser(int userId, int? todoId = null)
        {
            var todos = db.tblTodos.Where(t => t.fkUserId == userId);

            // If valid todoId is passed then return a list with just a single todo
            if (todoId != null)
                todos = todos.Where(t => t.RecId == todoId);

            return todos;
        }

        [HttpPut]
        [Route("{todoId:int}")]
        // Handles requests for CRUD "Update"
        // PUT api/<controller>/5
        public void ChangeTodoByUser(int userId, int todoId, [FromBody] tblTodo inTodo)
        {
            var todo = db.tblTodos
                .Where(t => t.fkUserId == userId && t.RecId == todoId)
                .FirstOrDefault();

            if (todo != null)
            {
                todo.Complete = inTodo.Complete;
                todo.fkUserId = inTodo.fkUserId;
                todo.Title = inTodo.Title;
                todo.Details = inTodo.Details;
                db.SaveChanges();
            }
        }

        [HttpPost]
        [Route("{todoId:int}/changeofstatus")]
        // POST api/<controller>/5/changeofstatus
        // Special business process-type resource, hence POST instead of PUT. Here 
        //    business rules associated with a change in a todo's completion status
        //    can be applied, e.g. notification and/or special logging or reporting.
        // Any special logic would also need to be invoked in the CRUD "Update" 
        //    because it also can change the completion status. Alternatively, the
        //    ability to change completions status from the "Update" could be removed.
        public void ChangeOfCompletionStatus(int userId, int todoId, [FromBody] bool Complete)
        {
            var todo = db.tblTodos
                .Where(t => t.fkUserId == userId && t.RecId == todoId)
                .FirstOrDefault();

            // throw new Exception("Kaboom!");

            if (todo != null)
            {
                todo.Complete = Complete;
                db.SaveChanges();
            }

        }

    }
}
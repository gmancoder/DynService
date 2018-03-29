using DynService_v3.Models;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DynService_v3
{
    [RoutePrefix("api")]
    public class DynamicsController : ApiController
    {
        private AuthRepository _repo = null;

        private Logger log = new Logger(typeof(DynamicsController));
        //private Logger log = new Logger(typeof(DynamicsController));
        DynCRMConnect connect = new DynCRMConnect(); //removed auth for security
        public DynamicsController()
        {
            _repo = new AuthRepository();
            connect.Connect();
        }

        /// <summary>
        /// Create entity object in Dynamics
        /// </summary>
        /// <param name="request">The request object</param>
        /// <returns></returns>
        [Authorize]
        [Route("Create")]
        [HttpPost]
        public IHttpActionResult Create(CreateRequest request)
        {
            log.Info("Create");
            log.Info(JsonConvert.SerializeObject(request));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            CreateResponse results = new CreateResponse();
            results.Request = request;
            results.Result = null;
            results.Id = Guid.Empty;
            results.Errors = new List<Dictionary<string, string>>();

            Entity newEntity = new Entity(request.Entity);
            foreach(KeyValuePair<string, object> field in request.Data)
            {
                log.Info(field.Key);
                newEntity.Attributes[field.Key] = DynHelper.ParsedValue(field.Value, log);
                
            }

            try
            {
                Guid id = connect.Create(newEntity);
                results.Result = newEntity;
                results.Id = id;
                log.Info(JsonConvert.SerializeObject(newEntity));
            }
            catch (Exception ex)
            {
                results.Errors.Add(new Dictionary<string, string> { { "Message", ex.Message }, { "StackTrace", ex.StackTrace } });
                log.Error(ex.Message);
                log.Error(ex.StackTrace);
                if (ex.InnerException != null)
                {
                    log.Error(ex.InnerException.Message);
                }
            }
            return Ok<CreateResponse>(results);
        }

        /// <summary>
        /// Update an entity object in Dynamics
        /// </summary>
        /// <param name="request">The update request</param>
        /// <returns></returns>
        [Authorize]
        [Route("Update")]
        [HttpPatch]
        public IHttpActionResult Update(UpdateRequest request)
        {
            log.Info("Update");
            log.Info(JsonConvert.SerializeObject(request));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            UpdateResponse results = new UpdateResponse();
            results.Request = request;
            results.Result = null;
            results.Id = request.Id;
            results.Errors = new List<Dictionary<string, string>>();


            Entity currentEntity = connect.Retrieve(request.Entity, request.Id, new ColumnSet(true));
            if (currentEntity != null)
            {
                foreach (KeyValuePair<string, object> field in request.Data)
                {
                    log.Info(field.Key);
                    currentEntity.Attributes[field.Key] = DynHelper.ParsedValue(field.Value, log);
                }

                try
                {
                    connect.Update(currentEntity);
                    results.Result = currentEntity;
                }
                catch (Exception ex)
                {
                    results.Errors.Add(new Dictionary<string, string> { { "Message", ex.Message }, { "StackTrace", ex.StackTrace } });
                    log.Error(ex.Message);
                    log.Error(ex.StackTrace);
                    if (ex.InnerException != null)
                    {
                        log.Error(ex.InnerException.Message);
                    }
                }
            }
            else
            {
                results.Errors.Add(new Dictionary<string, string> { { "Message", "Entity not found" }});
                log.Error("Entity Not Found");
            }
            return Ok<UpdateResponse>(results);
        }
        
        /// <summary>
        /// Delete an entity object in Dynamics
        /// </summary>
        /// <param name="request">The delete request object</param>
        /// <returns></returns>
        [Authorize]
        [Route("Delete")]
        [HttpDelete]
        public IHttpActionResult Delete(DeleteRequest request)
        {
            log.Info("Delete");
            log.Info(JsonConvert.SerializeObject(request));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            DeleteResponse results = new DeleteResponse();
            results.request = request;
            results.success = false;
            results.Errors = new List<Dictionary<string, string>>();

            try
            {
                connect.Delete(request.Entity, request.Id);
                results.success = true;
            }
            catch(Exception ex)
            {
                results.Errors.Add(new Dictionary<string, string> { { "Message", ex.Message }, { "StackTrace", ex.StackTrace } });
                log.Error(ex.Message);
                log.Error(ex.StackTrace);
                if (ex.InnerException != null)
                {
                    log.Error(ex.InnerException.Message);
                }
            }

            return Ok<DeleteResponse>(results);
        }


        /// <summary>
        /// Retrieve a single entity record from Dynamics
        /// </summary>
        /// <param name="request">The request object</param>
        /// <returns></returns>
        [Authorize]
        [Route("RetrieveSingle")]
        [HttpPost]
        public IHttpActionResult RetrieveSingle(RetrieveSingleRequest request)
        {
            log.Info("RetrieveSingle");
            log.Info(JsonConvert.SerializeObject(request));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            RetrieveSingleResponse results = new RetrieveSingleResponse();
            results.request = request;
            results.Result = null;
            results.Errors = new List<Dictionary<string, string>>();

            ColumnSet columns;
            if(request.Fields == null || request.Fields.Count == 0)
            {
                columns = new ColumnSet(true);
            }
            else
            {
                columns = new ColumnSet(request.Fields.ToArray());
            }

            try
            {
                results.Result = connect.Retrieve(request.Entity, request.Id, columns);
            }
            catch(Exception ex)
            {
                results.Errors.Add(new Dictionary<string, string> { { "Message", ex.Message }, { "StackTrace", ex.StackTrace } });
                log.Error(ex.Message);
                log.Error(ex.StackTrace);
                if (ex.InnerException != null)
                {
                    log.Error(ex.InnerException.Message);
                }
            }
            return Ok<RetrieveSingleResponse>(results);
        }

        /// <summary>
        /// Retrieve a list of entity objects from Dynamics
        /// </summary>
        /// <param name="request">The request object</param>
        /// <returns></returns>
        [Authorize]
        [Route("Retrieve")]
        [HttpPost]
        public IHttpActionResult Retrieve(RetrieveRequest request)
        {
            log.Info("Retrieve");
            log.Info(JsonConvert.SerializeObject(request));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            RetrieveResponse results = new RetrieveResponse();
            results.request = request;
            results.Results = new EntityCollection();
            results.Errors = new List<Dictionary<string, string>>();

            QueryExpression query = new QueryExpression();
            query.EntityName = request.Entity;
            if(request.Fields == null || request.Fields.Count == 0)
            {
                query.ColumnSet = new ColumnSet(true);
            }
            else
            {
                query.ColumnSet = new ColumnSet(request.Fields.ToArray());
            }

            if(request.Conditions != null)
            {
                foreach(RetrieveCondition condition in request.Conditions )
                {
                    ConditionExpression exp = new ConditionExpression();
                    exp.AttributeName = condition.Field;
                    switch(condition.Operator.ToLower())
                    {
                        case "equal":
                            exp.Operator = ConditionOperator.Equal;
                            break;
                        case "notequal":
                            exp.Operator = ConditionOperator.NotEqual;
                            break;
                        case "greaterthan":
                            exp.Operator = ConditionOperator.GreaterThan;
                            break;
                        case "lessthan":
                            exp.Operator = ConditionOperator.LessThan;
                            break;
                        case "greaterequal":
                            exp.Operator = ConditionOperator.GreaterEqual;
                            break;
                        case "lessequal":
                            exp.Operator = ConditionOperator.LessEqual;
                            break;
                        case "like":
                            exp.Operator = ConditionOperator.Like;
                            break;
                        case "notlike":
                            exp.Operator = ConditionOperator.NotLike;
                            break;
                        case "in":
                            exp.Operator = ConditionOperator.In;
                            break;
                        case "notin":
                            exp.Operator = ConditionOperator.NotIn;
                            break;
                        case "between":
                            exp.Operator = ConditionOperator.Between;
                            break;
                        case "notbetween":
                            exp.Operator = ConditionOperator.NotBetween;
                            break;
                        case "null":
                            exp.Operator = ConditionOperator.Null;
                            break;
                        case "notnull":
                            exp.Operator = ConditionOperator.NotNull;
                            break;
                        case "yesterday":
                            exp.Operator = ConditionOperator.Yesterday;
                            break;
                        case "today":
                            exp.Operator = ConditionOperator.Today;
                            break;
                        case "tomorrow":
                            exp.Operator = ConditionOperator.Tomorrow;
                            break;
                        case "last7days":
                            exp.Operator = ConditionOperator.Last7Days;
                            break;
                        case "next7days":
                            exp.Operator = ConditionOperator.Next7Days;
                            break;
                        case "lastweek":
                            exp.Operator = ConditionOperator.LastWeek;
                            break;
                        case "thisweek":
                            exp.Operator = ConditionOperator.ThisWeek;
                            break;
                        case "nextweek":
                            exp.Operator = ConditionOperator.NextWeek;
                            break;
                        case "lastmonth":
                            exp.Operator = ConditionOperator.LastMonth;
                            break;
                        case "thismonth":
                            exp.Operator = ConditionOperator.ThisMonth;
                            break;
                        case "nextmonth":
                            exp.Operator = ConditionOperator.NextMonth;
                            break;
                        case "on":
                            exp.Operator = ConditionOperator.On;
                            break;
                        case "onorbefore":
                            exp.Operator = ConditionOperator.OnOrBefore;
                            break;
                        case "onorafter":
                            exp.Operator = ConditionOperator.OnOrAfter;
                            break;
                        case "lastyear":
                            exp.Operator = ConditionOperator.LastYear;
                            break;
                        case "thisyear":
                            exp.Operator = ConditionOperator.ThisYear;
                            break;
                        case "nextyear":
                            exp.Operator = ConditionOperator.NextYear;
                            break;
                        case "lastxhours":
                            exp.Operator = ConditionOperator.LastXHours;
                            break;
                        case "nextxhours":
                            exp.Operator = ConditionOperator.NextXHours;
                            break;
                        case "lastxdays":
                            exp.Operator = ConditionOperator.LastXDays;
                            break;
                        case "nextxdays":
                            exp.Operator = ConditionOperator.NextXDays;
                            break;
                        case "lastxweeks":
                            exp.Operator = ConditionOperator.LastXWeeks;
                            break;
                        case "nextxweeks":
                            exp.Operator = ConditionOperator.NextXWeeks;
                            break;
                        case "lastxmonths":
                            exp.Operator = ConditionOperator.LastXMonths;
                            break;
                        case "nextxmonths":
                            exp.Operator = ConditionOperator.NextXMonths;
                            break;
                        case "lastxyears":
                            exp.Operator = ConditionOperator.LastXYears;
                            break;
                        case "nextxyears":
                            exp.Operator = ConditionOperator.NextXYears;
                            break;
                        case "equaluserid":
                            exp.Operator = ConditionOperator.EqualUserId;
                            break;
                        case "notequaluserid":
                            exp.Operator = ConditionOperator.NotEqualUserId;
                            break;
                        case "equalbusinessid":
                            exp.Operator = ConditionOperator.EqualBusinessId;
                            break;
                        case "notequalbusinessid":
                            exp.Operator = ConditionOperator.NotEqualBusinessId;
                            break;
                        case "childof":
                            exp.Operator = ConditionOperator.ChildOf;
                            break;
                        case "mask":
                            exp.Operator = ConditionOperator.Mask;
                            break;
                        case "notmask":
                            exp.Operator = ConditionOperator.NotMask;
                            break;
                        case "masksselect":
                            exp.Operator = ConditionOperator.MasksSelect;
                            break;
                        case "contains":
                            exp.Operator = ConditionOperator.Contains;
                            break;
                        case "doesnotcontain":
                            exp.Operator = ConditionOperator.DoesNotContain;
                            break;
                        case "equaluserlanguage":
                            exp.Operator = ConditionOperator.EqualUserLanguage;
                            break;
                        case "noton":
                            exp.Operator = ConditionOperator.NotOn;
                            break;
                        case "olderthanxmonths":
                            exp.Operator = ConditionOperator.OlderThanXMonths;
                            break;
                        case "beginswith":
                            exp.Operator = ConditionOperator.BeginsWith;
                            break;
                        case "doesnotbeginwith":
                            exp.Operator = ConditionOperator.DoesNotBeginWith;
                            break;
                        case "endswith":
                            exp.Operator = ConditionOperator.EndsWith;
                            break;
                        case "doesnotendwith":
                            exp.Operator = ConditionOperator.DoesNotEndWith;
                            break;
                        case "thisfiscalyear":
                            exp.Operator = ConditionOperator.ThisFiscalYear;
                            break;
                        case "thisfiscalperiod":
                            exp.Operator = ConditionOperator.ThisFiscalPeriod;
                            break;
                        case "nextfiscalyear":
                            exp.Operator = ConditionOperator.NextFiscalYear;
                            break;
                        case "nextfiscalperiod":
                            exp.Operator = ConditionOperator.NextFiscalPeriod;
                            break;
                        case "lastfiscalyear":
                            exp.Operator = ConditionOperator.LastFiscalYear;
                            break;
                        case "lastfiscalperiod":
                            exp.Operator = ConditionOperator.LastFiscalPeriod;
                            break;
                        case "lastxfiscalyears":
                            exp.Operator = ConditionOperator.LastXFiscalYears;
                            break;
                        case "lastxfiscalperiods":
                            exp.Operator = ConditionOperator.LastXFiscalPeriods;
                            break;
                        case "nextxfiscalyears":
                            exp.Operator = ConditionOperator.NextXFiscalYears;
                            break;
                        case "nextxfiscalperiods":
                            exp.Operator = ConditionOperator.NextXFiscalPeriods;
                            break;
                        case "infiscalyear":
                            exp.Operator = ConditionOperator.InFiscalYear;
                            break;
                        case "infiscalperiod":
                            exp.Operator = ConditionOperator.InFiscalPeriod;
                            break;
                        case "infiscalperiodandyear":
                            exp.Operator = ConditionOperator.InFiscalPeriodAndYear;
                            break;
                        case "inorbeforefiscalperiodandyear":
                            exp.Operator = ConditionOperator.InOrBeforeFiscalPeriodAndYear;
                            break;
                        case "inorafterfiscalperiodandyear":
                            exp.Operator = ConditionOperator.InOrAfterFiscalPeriodAndYear;
                            break;
                        case "equaluserteams":
                            exp.Operator = ConditionOperator.EqualUserTeams;
                            break;
                        default:
                            exp.Operator = ConditionOperator.Equal;
                            break;
                    }
                    log.Info(condition.Field);
                    foreach (object value in condition.Values)
                    {
                        log.Info(value);
                        exp.Values.Add(DynHelper.ParsedValue(value, log));
                    }

                    query.Criteria.AddCondition(exp);
                }
            }
            query.Criteria.FilterOperator = LogicalOperator.And;
            if (request.WhereOperator != null && request.WhereOperator.ToLower() == "or")
            {
                query.Criteria.FilterOperator = LogicalOperator.Or;
            }

            if(request.Order != null)
            {
                foreach (string order in request.Order)
                {
                    if (request.OrderDirection == null || request.OrderDirection.ToLower() == "asc")
                    {
                        query.AddOrder(order, OrderType.Ascending);
                    }
                    else
                    {
                        query.AddOrder(order, OrderType.Descending);
                    }
                }
            }

            try
            {
                EntityCollection entities = connect.RetrieveMultiple(query);
                results.Results = entities;
            }
            catch (Exception ex)
            {
                results.Errors.Add(new Dictionary<string, string> { { "Message", ex.Message }, { "StackTrace", ex.StackTrace } });
                log.Error(ex.Message);
                log.Error(ex.StackTrace);
                if (ex.InnerException != null)
                {
                    log.Error(ex.InnerException.Message);
                }
            }
            return Ok<RetrieveResponse>(results);
        }

    }
}

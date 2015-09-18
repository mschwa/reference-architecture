using System.Net;
using System.Net.Http;
using System.Web.Http;
using log4net;
using Schwartz.Inventory.Api.Infrastructure;
using Schwartz.Inventory.Api.Services;
using Schwartz.Inventory.Data.Entity;
using Schwartz.Inventory.Data.Repository;
using Schwartz.Inventory.Notify;

namespace Schwartz.Inventory.Api.Controllers
{
	[RoutePrefix("api/inventory")]
	public class InventoryItemsController : ApiController
	{
		private const string _databaseDown = "Database is down or cannot connect.";
		private const string _queueDown = "Queue is down or cannot connect.";
		private const string _everythingDown = "Nothing works, we're in trouble.";
		private const string _systemError = "System Error.";
		
		private readonly ILog _logger;
		private readonly IInventoryTakeMessageCache _messageCache;
		private readonly IMessageQueueService _messagingService;
		private readonly INotificationService _notificationService;
		private readonly IInventoryRepository _repository;

		public InventoryItemsController(IInventoryRepository repository, IMessageQueueService messagingService,
			IInventoryTakeMessageCache messageCache, INotificationService notificationService, ILog logger)
		{
			_repository = repository;
			_messageCache = messageCache;
			_notificationService = notificationService;
			_logger = logger;
			_messagingService = messagingService;
		}

		public ApplicationSettings Settings { get; set; }
		
		[Route("add")]
		[ClaimsAuthorization(ClaimType = "InventoryAdmin", ClaimValue = "1")]
		public HttpResponseMessage Post([FromBody] InventoryItem item)
		{
			try
			{
				var added = _repository.Create(item);

				if (added == null)
				{
					return ControllerContext.Request.CreateResponse(HttpStatusCode.Conflict);
				}
			}
			catch (InventoryFatalException de)
			{
				var message = "Database is down."; // Would get this from a resx file or something
				var subject = "System Error";

				_logger.Error(message, de);
				SendErrorNotification(subject, message);

				return ControllerContext.Request.CreateResponse(HttpStatusCode.ServiceUnavailable);
			}

			return ControllerContext.Request.CreateResponse(HttpStatusCode.Created);
		}

		[Route("take")]
		[ClaimsAuthorization(ClaimType = "InventoryAdmin", ClaimValue = "1")]
		public HttpResponseMessage Delete([FromBody] InventoryItem item)
		{
			try
			{
				var removed = _repository.Remove(item);

				if (removed == null)
				{
					return ControllerContext.Request.CreateResponse(HttpStatusCode.NotFound);
				}

				_messagingService.QueueInventoryItem(removed);
				_messagingService.FlushLocalCache(_messageCache);

				return ControllerContext.Request.CreateResponse(HttpStatusCode.Accepted);
			}
			catch (InventoryFatalException de)
			{
				_logger.Error(_databaseDown, de);
				SendErrorNotification(_systemError, _databaseDown);

				return ControllerContext.Request.CreateResponse(HttpStatusCode.ServiceUnavailable);
			}
			catch (MessageQueueException de)
			{
				// Since we have already removed the item, we will attempt to save them message to a in memory queue
				// and assume the service will be up soon. We try to flush the cache every request.
				if (Settings.EnableMessageCache)
				{
					_messagingService.CacheInventoryItem(_messageCache, item);
				}

				_logger.Error(_queueDown, de);

				// As a last resort, we'll try to email ops
				SendErrorNotification(_systemError, _queueDown);

				return ControllerContext.Request.CreateResponse(HttpStatusCode.ServiceUnavailable);
			}
		}

		private void SendErrorNotification(string subject, string message)
		{
			try
			{
				_notificationService.SendEmail(Settings.AdminEmail, Settings.NoReplyEmail, subject, message);
			}
			catch (NotificationServiceUnavailable ne)
			{
				_logger.Error(_everythingDown, ne);
			}
		}
	}
}
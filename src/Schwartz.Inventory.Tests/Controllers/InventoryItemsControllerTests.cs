using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using log4net;
using Moq;
using NUnit.Framework;
using Schwartz.Inventory.Api.Controllers;
using Schwartz.Inventory.Api.Infrastructure;
using Schwartz.Inventory.Api.Services;
using Schwartz.Inventory.Data.Entity;
using Schwartz.Inventory.Data.Repository;
using Schwartz.Inventory.Notify;

namespace Schwartz.Inventory.Tests.Controllers
{
	[TestFixture]
	public class InventoryItemsControllerTests
	{
		[Test]
		public void Post_Succeeds()
		{
			// Arrange
			var mockLogger = new Mock<ILog>();
			var mockMessageCache = new Mock<IInventoryTakeMessageCache>();
			var mockMessageQueue = new Mock<IMessageQueueService>();
			var mockNotificationService = new Mock<INotificationService>();
			var mockRepository = new Mock<IInventoryRepository>();
			var settings = new ApplicationSettings(false);

			var item = new InventoryItem
			{
				Label = "test",
				Expires = DateTime.Now.AddDays(30)
			};

			mockRepository.Setup(r => r.Create(item)).Returns(item);

			var controller = GetAccountControllerWithContext(mockLogger, mockMessageCache,
				mockMessageQueue, mockNotificationService, mockRepository, settings);

			// Act
			HttpResponseMessage response = null;

			Assert.DoesNotThrow(() => response = controller.Post(item));

			// Assert
			Assert.NotNull(response);
			Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
		}

		[Test]
		public void Post_Handles_Duplicate()
		{
			// Arrange
			var mockLogger = new Mock<ILog>();
			var mockMessageCache = new Mock<IInventoryTakeMessageCache>();
			var mockMessageQueue = new Mock<IMessageQueueService>();
			var mockNotificationService = new Mock<INotificationService>();
			var mockRepository = new Mock<IInventoryRepository>();
			var settings = new ApplicationSettings(false);

			var item = new InventoryItem
			{
				Label = "test",
				Expires = DateTime.Now.AddDays(30)
			};

			InventoryItem ouput = null;

			mockRepository.Setup(r => r.Create(item)).Returns(ouput);

			var controller = GetAccountControllerWithContext(mockLogger, mockMessageCache,
				mockMessageQueue, mockNotificationService, mockRepository, settings);

			// Act
			HttpResponseMessage response = null;

			Assert.DoesNotThrow(() => response = controller.Post(item));

			// Assert
			Assert.NotNull(response);
			Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
		}

		[Test]
		public void Post_Handles_DataSource_Failure_Gracefully()
		{
			// Arrange
			var mockLogger = new Mock<ILog>();
			var mockMessageCache = new Mock<IInventoryTakeMessageCache>();
			var mockMessageQueue = new Mock<IMessageQueueService>();
			var mockNotificationService = new Mock<INotificationService>();
			var mockRepository = new Mock<IInventoryRepository>();
			var settings = new ApplicationSettings(false);

			var item = new InventoryItem
			{
				Label = "test",
				Expires = DateTime.Now.AddDays(30)
			};

			mockRepository.Setup(r => r.Create(item)).Throws(new InventoryFatalException(new Exception()));

			var controller = GetAccountControllerWithContext(mockLogger, mockMessageCache,
				mockMessageQueue, mockNotificationService, mockRepository, settings);

			// Act
			HttpResponseMessage response = null;

			Assert.DoesNotThrow(() => response = controller.Post(item));

			// Assert
			Assert.NotNull(response);
			Assert.AreEqual(HttpStatusCode.ServiceUnavailable, response.StatusCode);

			mockNotificationService
				.Verify(n => n.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
		}

		[Test]
		public void Delete_Succeeds()
		{
			// Arrange
			var mockLogger = new Mock<ILog>();
			var mockMessageCache = new Mock<IInventoryTakeMessageCache>();
			var mockMessageQueue = new Mock<IMessageQueueService>();
			var mockNotificationService = new Mock<INotificationService>();
			var mockRepository = new Mock<IInventoryRepository>();
			var settings = new ApplicationSettings(false);

			var item = new InventoryItem
			{
				Label = "test",
				Expires = DateTime.Now.AddDays(30)
			};

			mockRepository.Setup(r => r.Remove(item)).Returns(item);

			var controller = GetAccountControllerWithContext(mockLogger, mockMessageCache,
				mockMessageQueue, mockNotificationService, mockRepository, settings);

			// Act
			HttpResponseMessage response = null;

			Assert.DoesNotThrow(() => response = controller.Delete(item));

			// Assert
			Assert.NotNull(response);
			Assert.AreEqual(HttpStatusCode.Accepted, response.StatusCode);

			mockMessageQueue.Verify(n => n.QueueInventoryItem(item), Times.Once);
			mockMessageQueue.Verify(n => n.FlushLocalCache(mockMessageCache.Object), Times.Once);
		}

		[Test]
		public void Delete_Handles_Item_Not_Found()
		{
			// Arrange
			var mockLogger = new Mock<ILog>();
			var mockMessageCache = new Mock<IInventoryTakeMessageCache>();
			var mockMessageQueue = new Mock<IMessageQueueService>();
			var mockNotificationService = new Mock<INotificationService>();
			var mockRepository = new Mock<IInventoryRepository>();
			var settings = new ApplicationSettings(false);

			var item = new InventoryItem
			{
				Label = "test",
				Expires = DateTime.Now.AddDays(30)
			};

			InventoryItem output = null;

			mockRepository.Setup(r => r.Remove(item)).Returns(output);

			var controller = GetAccountControllerWithContext(mockLogger, mockMessageCache,
				mockMessageQueue, mockNotificationService, mockRepository, settings);

			// Act
			HttpResponseMessage response = null;

			Assert.DoesNotThrow(() => response = controller.Delete(item));

			// Assert
			Assert.NotNull(response);
			Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);

			mockMessageQueue.Verify(n => n.QueueInventoryItem(item), Times.Never);
		}

		[Test]
		public void Delete_Handles_DataSource_Failure_Gracefully()
		{
			// Arrange
			var mockLogger = new Mock<ILog>();
			var mockMessageCache = new Mock<IInventoryTakeMessageCache>();
			var mockMessageQueue = new Mock<IMessageQueueService>();
			var mockNotificationService = new Mock<INotificationService>();
			var mockRepository = new Mock<IInventoryRepository>();
			var settings = new ApplicationSettings(false);

			var item = new InventoryItem
			{
				Label = "test",
				Expires = DateTime.Now.AddDays(30)
			};

			mockRepository.Setup(r => r.Remove(item)).Throws(new InventoryFatalException(new Exception()));

			var controller = GetAccountControllerWithContext(mockLogger, mockMessageCache,
				mockMessageQueue, mockNotificationService, mockRepository, settings);

			// Act
			HttpResponseMessage response = null;

			Assert.DoesNotThrow(() => response = controller.Delete(item));

			// Assert
			Assert.NotNull(response);
			Assert.AreEqual(HttpStatusCode.ServiceUnavailable, response.StatusCode);

			mockMessageQueue.Verify(n => n.QueueInventoryItem(item), Times.Never);
			mockMessageQueue.Verify(n => n.FlushLocalCache(mockMessageCache.Object), Times.Never);

			mockNotificationService
				.Verify(n => n.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
		}

		[Test]
		public void Delete_Handles_Notification_Failure_Gracefully()
		{
			// Arrange
			var mockLogger = new Mock<ILog>();
			var mockMessageCache = new Mock<IInventoryTakeMessageCache>();
			var mockMessageQueue = new Mock<IMessageQueueService>();
			var mockNotificationService = new Mock<INotificationService>();
			var mockRepository = new Mock<IInventoryRepository>();
			var settings = new ApplicationSettings(false);
			settings.EnableMessageCache = true;

			var item = new InventoryItem
			{
				Label = "test",
				Expires = DateTime.Now.AddDays(30)
			};

			mockRepository.Setup(r => r.Remove(item)).Returns(item);
			mockMessageQueue.Setup(r => r.QueueInventoryItem(item)).Throws(new MessageQueueException());

			var controller = GetAccountControllerWithContext(mockLogger, mockMessageCache,
				mockMessageQueue, mockNotificationService, mockRepository, settings);

			// Act
			HttpResponseMessage response = null;

			Assert.DoesNotThrow(() => response = controller.Delete(item));

			// Assert
			Assert.NotNull(response);
			Assert.AreEqual(HttpStatusCode.ServiceUnavailable, response.StatusCode);

			mockMessageQueue.Verify(n => n.FlushLocalCache(mockMessageCache.Object), Times.Never);

			mockNotificationService
				.Verify(n => n.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);

			mockMessageQueue
				.Verify(c => c.CacheInventoryItem(mockMessageCache.Object, item), Times.Once);
		}

		public InventoryItemsController GetAccountControllerWithContext(Mock<ILog> mockLogger,
			Mock<IInventoryTakeMessageCache> mockMessageCache, Mock<IMessageQueueService> mockQueueService,
			Mock<INotificationService> mockNotificationService, Mock<IInventoryRepository> mockRepository,
			ApplicationSettings settings)
		{
			// Set up Mocks
			var mockConfig = new Mock<HttpConfiguration>();
			var mockRequest = new HttpRequestMessage(HttpMethod.Post, "http://www.test.com");
			var mockRouteData = new Mock<IHttpRouteData>();
			//mockContext.SetupGet(c => c.Request).Returns(new HttpRequestWrapper(new HttpRequest("test", "http://www.test.com", null)));

			var mockUrlHelper = new Mock<UrlHelper>();

			var controller = new InventoryItemsController(mockRepository.Object, mockQueueService.Object,
				mockMessageCache.Object, mockNotificationService.Object, mockLogger.Object)
			{Settings = settings};

			controller.ControllerContext = new HttpControllerContext(mockConfig.Object, mockRouteData.Object, mockRequest);
			controller.Url = mockUrlHelper.Object;

			return controller;
		}
	}
}
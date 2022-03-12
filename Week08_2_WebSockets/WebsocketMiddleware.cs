/*
 * Copyright 2016-2019 Mohawk College of Applied Arts and Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: khannan
 * Date: 2019-3-12
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Week9WebSockets
{
	/// <summary>
	/// Represents a web socket middleware.
	/// </summary>
	public class WebSocketMiddleware
	{
		/// <summary>
		/// The request delegate.
		/// </summary>
		private readonly RequestDelegate next;

		/// <summary>
		/// Represents the connected client endpoints.
		/// Where the key is the trace identifier, and the value is the client connection.
		/// </summary>
		private readonly Dictionary<string, WebSocket> clients = new Dictionary<string, WebSocket>();

		/// <summary>
		/// Initializes a new instance of the <see cref="WebSocketMiddleware"/> class.
		/// </summary>
		/// <param name="next">The next.</param>
		public WebSocketMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		public async Task InvokeAsync(HttpContext context, ILogger<WebSocketMiddleware> logger)
		{
			// if client connects on path 'ws' 
			// and if the connection is ACTUALLY a web socket connection
			// we want to start processing the information received
			if (context.Request.Path == "/ws")
			{
				logger.LogInformation($"Client connected: {context.Connection?.RemoteIpAddress}:{context.Connection?.RemotePort}");

				if (context.WebSockets.IsWebSocketRequest)
				{
					// start processing the information from the connection
					// and allow for additional connections to be processed
					// because we are dealing with asynchronous processes
					var webSocket = await context.WebSockets.AcceptWebSocketAsync();

					var key = context.TraceIdentifier;

					// if the client connected is not already being tracked
					// we want to add the connection to our dictionary of connections
					// to track the connected client
					// and use the instance of the web socket as the value
					// because the instance represents the actual connected web socket
					if (!this.clients.ContainsKey(key))
					{
						this.clients.Add(key, webSocket);
					}

					// invokes our handler
					await RespondAsync(context, webSocket);
				}
				else
				{
					context.Response.StatusCode = 400;
				}
			}
			// if the client does not connect on the path 'ws'
			// then we want to return page not found to the user
			else
			{
				await this.next(context);
			}
		}

		private async Task RespondAsync(HttpContext context, WebSocket webSocket)
		{
			// setup a cancellation token source
			// with a default value of 5 seconds
			//var cancellationTokenSource = new CancellationTokenSource(5000);

			// create a buffer to read the information from the connected web socket
			// default this to ~4 KB 
			var buffer = new byte[1024 * 4];

			// start reading the information from the web socket connection
			// we use the ReceiveAsync method to achieve this
			// we are also providing a cancellation token to our receive method
			// to indicate that if the overall read process is not completed in 5000 ms / 5 s, then the framework
			// will throw a Exception
			var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

			// if the web socket has a close status, 
			while (!result.CloseStatus.HasValue)
			{

				// for each client in our dictionary we want
				// to send a message back to all the clients
				// so they are able to receive messages from 
				// any other connected client

				// send the message received to all connected clients
				// we are using a parallel for each loop
				// so that we are not respond to connected clients sequentially
				// we are doing this in parallel

				// make a copy of the result variable
				// this is to ensure that within the parallel for each loop
				// that we are not accessing the result variable directly
				// since the result variable changes in scope from another context
				var resultCopy = result;

				Parallel.ForEach(this.clients.Where(c => c.Key != context.TraceIdentifier).Select(c => c.Value), async socket =>
				{
					// send the message back to the connected clients
					// indicate that the message being sent back is text
					// and use the receive result as an indicator as
					// to whether or not we have the end of the message
					// specify no cancellation token
					await socket.SendAsync(new ArraySegment<byte>(buffer, 0, resultCopy.Count),
						resultCopy.MessageType,
						resultCopy.EndOfMessage,
						CancellationToken.None);
				});


				// start the receive operation again
				// indicating to our application that more processing
				// is ready to take place
				result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer),
					CancellationToken.None);
			}

			// potentially close the connection to the client or clients
			// using the result close status
			// the close status description
			// specify no cancellation token
			await webSocket.CloseAsync(result.CloseStatus.Value,
				result.CloseStatusDescription,
				CancellationToken.None);
			// remove the connection instance from our dictionary
			// because the client has disconnected
			this.clients.Remove(context.TraceIdentifier);
		}
	}
}

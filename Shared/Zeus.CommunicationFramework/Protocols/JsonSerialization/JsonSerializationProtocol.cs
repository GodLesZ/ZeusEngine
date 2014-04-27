using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Zeus.CommunicationFramework.Messages;
using Zeus.CommunicationFramework.Services.Messages;

namespace Zeus.CommunicationFramework.Protocols.JsonSerialization {

    public class JsonSerializationProtocol : IWireProtocol {
        public static Encoding StringEncoding = Encoding.UTF8;


        /// <summary>
        ///     Serializes a message to a byte array to send to remote application.
        ///     This method is synchronized. So, only one thread can call it concurrently.
        /// </summary>
        /// <param name="message">Message to be serialized</param>
        public byte[] GetBytes(IMessage message) {
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            return StringEncoding.GetBytes(jsonString);
        }

        /// <summary>
        ///     Builds messages from a byte array that is received from remote application.
        ///     The Byte array may contain just a part of a message, the protocol must
        ///     cumulate bytes to build messages.
        ///     This method is synchronized. So, only one thread can call it concurrently.
        /// </summary>
        /// <param name="receivedBytes">Received bytes from remote application</param>
        /// <returns>
        ///     List of messages.
        ///     Protocol can generate more than one message from a byte array.
        ///     Also, if received bytes are not sufficient to build a message, the protocol
        ///     may return an empty list (and save bytes to combine with next method call).
        /// </returns>
        public IEnumerable<IMessage> CreateMessages(byte[] receivedBytes) {
            var dynamicMessages = CreateDynamicMessages(receivedBytes);
            var messages = new List<IMessage>();
            foreach (var dynamicMessage in dynamicMessages) {
                // Build a strongly-typed message
                var messageData = (IDictionary<string, object>)dynamicMessage;
                // Base properties
                if (messageData.ContainsKey("Id") == false || messageData.ContainsKey("RepliedId") == false) {
                    continue;
                }

                var messageId = ushort.Parse(messageData["Id"].ToString());
                var messageRepliedId = ushort.Parse(messageData["RepliedId"].ToString());

                // TextMessage
                if (messageData.ContainsKey("Text")) {
                    messages.Add(new TextMessage(messageId) {
                        RepliedId = messageRepliedId,
                        Text = (string)messageData["Text"]
                    });
                    continue;
                }
                // RawDataMessage
                if (messageData.ContainsKey("MessageData")) {
                    messages.Add(new RawDataMessage(messageId) {
                        RepliedId = messageRepliedId,
                        MessageData = (byte[])messageData["MessageData"]
                    });
                    continue;
                }
                // PingMessage
                if (messageData.ContainsKey("Ping")) {
                    messages.Add(new PingMessage(messageId) {
                        RepliedId = messageRepliedId
                    });
                    continue;
                }
                // RemoteInvokeMessage
                if (messageData.ContainsKey("MethodName")) {
                    messages.Add(new RemoteInvokeMessage(messageId) {
                        RepliedId = messageRepliedId,
                        MethodName = (string)messageData["MethodName"],
                        Parameters = ((List<object>)messageData["Parameters"]).ToArray(),
                        ServiceClassName = (string)messageData["ServiceClassName"]
                    });
                    continue;
                }
                // RemoteInvokeReturnMessage
                if (messageData.ContainsKey("ReturnValue")) {
                    // @TODO: Wont work for ReturnValue - type is not known here
                    messages.Add(new RemoteInvokeReturnMessage(messageId) {
                        RepliedId = messageRepliedId,
                        RemoteException = (RemoteException)messageData["RemoteException"],
                        ReturnValue = messageData["ReturnValue"]
                    });
                    continue;
                }

                // Message
                messages.Add(new Message(messageId) {
                    RepliedId = messageRepliedId
                });
            }

            return messages;
        }

        public List<dynamic> CreateDynamicMessages(byte[] receivedBytes) {
            var jsonString = StringEncoding.GetString(receivedBytes);
            var token = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString) as JToken;
            // Got a list of messages
            if (token is JArray) {
                return token
                    .Select(arrayItem => ConvertTokenToDynamic(arrayItem))
                    .ToList();
            }

            // Got a single message
            var messages = new List<dynamic> {
                ConvertTokenToDynamic(token)
            };
            return messages;
        }

        protected dynamic ConvertTokenToDynamic(JToken token) {
            // Strong types
            if (token.Type == JTokenType.String) {
                return (string)token;
            }
            if (token is JValue) {
                return ((JValue)token).Value;
            }
            // Full object
            if (token is JObject) {
                var expando = new ExpandoObject();
                var childTokens = (
                    from childToken in token
                    where childToken is JProperty
                    select childToken as JProperty
                    ).ToList();

                foreach (var property in childTokens) {
                    var value = ConvertTokenToDynamic(property.Value);
                    ((IDictionary<string, object>)expando).Add(property.Name, value);
                }
                return expando;
            }
            // Array of properties possible?
            // @TODO: Check if needed
            if (token is JArray) {
                return token.Select(arrayItem => ConvertTokenToDynamic(arrayItem)).ToList();
            }

            throw new ArgumentException(string.Format("Unknown token type '{0}'", token.GetType()), "token");
        }


        /// <summary>
        ///     This method is called when connection with remote application is reset (connection is renewing or first
        ///     connecting).
        ///     So, wire protocol must reset itself.
        /// </summary>
        public void Reset() {

        }

    }

}
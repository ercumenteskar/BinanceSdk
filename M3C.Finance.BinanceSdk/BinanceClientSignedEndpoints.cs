﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using M3C.Finance.BinanceSdk.Enumerations;
using M3C.Finance.BinanceSdk.ResponseObjects;

namespace M3C.Finance.BinanceSdk
{
    public partial class BinanceClient
    {
        public AccountResponse GetAccountInfo()
        {
            return SendRequest<AccountResponse>("account",ApiVersion.Version3,ApiMethodType.Signed, HttpMethod.Get);
        }

        public NewOrderResponse NewOrder(string symbol,OrderSide side,OrderType orderType,TimeInForce timeInForce, decimal quantity, decimal price, bool isTestOrder = false,
            string newClientOrderId = null, decimal? stopPrice=null, decimal? icebergQuantity=null, long? recvWindow = null)
        {
            var parameters = new Dictionary<string, string>
            {
                {"symbol", symbol},
                {"side", side},
                {"type", orderType},
                {"timeInForce", timeInForce},
                {"quantity", quantity.ToString(CultureInfo.InvariantCulture)},
                {"price", price.ToString(CultureInfo.InvariantCulture)}
            };

            CheckAndAddReceiveWindow(recvWindow,parameters);

            if (!string.IsNullOrEmpty(newClientOrderId))
            {
                parameters.Add("newClientOrderId",newClientOrderId);
            }
            if (stopPrice.HasValue)
            {
                parameters.Add("stopPrice",stopPrice.Value.ToString(CultureInfo.InvariantCulture));
            }
            if (icebergQuantity.HasValue)
            {
                parameters.Add("icebergQty",icebergQuantity.Value.ToString(CultureInfo.InvariantCulture));
            }
            return SendRequest<NewOrderResponse>(isTestOrder ? "order/test" : "order",ApiVersion.Version3,ApiMethodType.Signed, HttpMethod.Post,parameters);
        }

        

        public QueryOrderResponse QueryOrder(string symbol, long? orderId, string clientOrderId = null,long? recvWindow = null)
        {
            var parameters = new Dictionary<string, string>
            {
                {"symbol", symbol},
            };

            CheckAndAddReceiveWindow(recvWindow, parameters);

            if (!orderId.HasValue && string.IsNullOrEmpty(clientOrderId))
            {
                throw new ArgumentException("Either orderId or clientOrderId should be set!");
            }

            if (orderId.HasValue)
            {
                parameters.Add("orderId",orderId.Value.ToString(CultureInfo.InvariantCulture));

            }
            if (!string.IsNullOrEmpty(clientOrderId))
            {
                parameters.Add("origClientOrderId", clientOrderId);
            }
            return SendRequest<QueryOrderResponse>("order", ApiVersion.Version3, ApiMethodType.Signed,HttpMethod.Get,parameters);
        }

        public IEnumerable<OrderInfo> CurrentOpenOrders(string symbol, long? recvWindow = null)
        {
            var parameters = new Dictionary<string, string>
            {
                {"symbol", symbol},
            };

            CheckAndAddReceiveWindow(recvWindow, parameters);

            return SendRequest<List<OrderInfo>>("openOrders", ApiVersion.Version3, ApiMethodType.Signed, HttpMethod.Get, parameters);
        }

        public IEnumerable<OrderInfo> ListAllOrders(string symbol, long? orderId = null,int? limit = null,long? recvWindow = null)
        {
            var parameters = new Dictionary<string, string>
            {
                {"symbol", symbol},
            };

            CheckAndAddReceiveWindow(recvWindow, parameters);

            if (orderId.HasValue)
            {
                parameters.Add("orderId",orderId.Value.ToString(CultureInfo.InvariantCulture));
            }
            if (limit.HasValue)
            {
                parameters.Add("limit", limit.Value.ToString(CultureInfo.InvariantCulture));
            }
            return SendRequest<List<OrderInfo>>("allOrders", ApiVersion.Version3, ApiMethodType.Signed, HttpMethod.Get, parameters);
        }

        public CancelOrderResponse CancelOrder(string symbol, long? orderId = null, string originalClientOrderId = null, string newClientOrderId = null, long? recvWindow = null)
        {
            var parameters = new Dictionary<string, string>
            {
                {"symbol", symbol},
            };

            CheckAndAddReceiveWindow(recvWindow, parameters);

            if (orderId.HasValue)
            {
                parameters.Add("orderId",orderId.Value.ToString(CultureInfo.InvariantCulture));
            }
            if (!string.IsNullOrEmpty(originalClientOrderId))
            {
                parameters.Add("origClientOrderId",originalClientOrderId);
            }
            if (!string.IsNullOrEmpty(newClientOrderId))
            {
                parameters.Add("newClientOrderId", newClientOrderId);
            }
            return SendRequest<CancelOrderResponse>("order", ApiVersion.Version3, ApiMethodType.Signed,
                HttpMethod.Delete, parameters);
        }

        public List<TradeInfo> ListMyTrades(string symbol, int? limit = null, long? fromId = null, long? recvWindow = null)
        {
            var parameters = new Dictionary<string, string>
            {
                {"symbol", symbol},
            };

            CheckAndAddReceiveWindow(recvWindow, parameters);

            if (limit.HasValue)
            {
                parameters.Add("limit", limit.Value.ToString(CultureInfo.InvariantCulture));
            }
            if (fromId.HasValue)
            {
                parameters.Add("fromId", fromId.Value.ToString(CultureInfo.InvariantCulture));
            }
            return SendRequest<List<TradeInfo>>("myTrades",ApiVersion.Version3,ApiMethodType.Signed,HttpMethod.Get,parameters);
        }

        private void CheckAndAddReceiveWindow(long? recvWindow, IDictionary<string, string> parameters)
        {
            if (recvWindow.HasValue)
            {
                parameters.Add("recvWindow", recvWindow.Value.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}

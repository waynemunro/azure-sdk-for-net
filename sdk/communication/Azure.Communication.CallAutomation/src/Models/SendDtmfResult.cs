﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using System;
using System.Threading;

namespace Azure.Communication.CallAutomation
{
    /// <summary>The result from send dtmf request.</summary>
    public class SendDtmfResult
    {
        private CallAutomationEventProcessor _evHandler;
        private string _callConnectionId;
        private string _operationContext;

        internal SendDtmfResult()
        {
        }

        internal void SetEventProcessor(CallAutomationEventProcessor evHandler, string callConnectionId, string operationContext)
        {
            _evHandler = evHandler;
            _callConnectionId = callConnectionId;
            _operationContext = operationContext;
        }

        /// <summary>
        /// This is blocking call. Wait for <see cref="SendDtmfEventResult"/> using <see cref="CallAutomationEventProcessor"/>.
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token can be used to set timeout or cancel this WaitForEventProcessor.</param>
        /// <returns>Returns <see cref="SendDtmfEventResult"/> which contains either <see cref="SendDtmfCompletedEventData"/> event or <see cref="SendDtmfFailedEventData"/> event.</returns>
        public SendDtmfEventResult WaitForEventProcessor(CancellationToken cancellationToken = default)
        {
            if (_evHandler is null)
            {
                throw new NullReferenceException(nameof(_evHandler));
            }

            var returnedEvent = _evHandler.WaitForEventProcessor(filter
                => filter.CallConnectionId == _callConnectionId
                && (filter.OperationContext == _operationContext || _operationContext is null)
                && (filter.GetType() == typeof(SendDtmfCompletedEventData)
                || filter.GetType() == typeof(SendDtmfFailedEventData)),
                cancellationToken);

            return SetReturnedEvent(returnedEvent);
        }

        /// <summary>
        /// Wait for <see cref="SendDtmfEventResult"/> using <see cref="CallAutomationEventProcessor"/>.
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token can be used to set timeout or cancel this WaitForEventProcessor.</param>
        /// <returns>Returns <see cref="SendDtmfEventResult"/> which contains either <see cref="SendDtmfCompletedEventData"/> event or <see cref="SendDtmfCompletedEventData"/> event.</returns>
        public async Task<SendDtmfEventResult> WaitForEventProcessorAsync(CancellationToken cancellationToken = default)
        {
            if (_evHandler is null)
            {
                throw new NullReferenceException(nameof(_evHandler));
            }

            var returnedEvent = await _evHandler.WaitForEventProcessorAsync(filter
                => filter.CallConnectionId == _callConnectionId
                && (filter.OperationContext == _operationContext || _operationContext is null)
                && (filter.GetType() == typeof(SendDtmfCompletedEventData)
                || filter.GetType() == typeof(SendDtmfFailedEventData)),
                cancellationToken).ConfigureAwait(false);

            return SetReturnedEvent(returnedEvent);
        }

        private static SendDtmfEventResult SetReturnedEvent(CallAutomationEventData returnedEvent)
        {
            SendDtmfEventResult result = default;
            switch (returnedEvent)
            {
                case SendDtmfCompletedEventData:
                    result = new SendDtmfEventResult(true, (SendDtmfCompletedEventData)returnedEvent, null);
                    break;
                case SendDtmfFailedEventData:
                    result = new SendDtmfEventResult(false, null, (SendDtmfFailedEventData)returnedEvent);
                    break;
                default:
                    throw new NotSupportedException(returnedEvent.GetType().Name);
            }

            return result;
        }
    }
}

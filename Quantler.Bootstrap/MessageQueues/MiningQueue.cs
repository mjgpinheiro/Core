using System;
using System.Composition;
using Quantler.Messaging;

namespace Quantler.Bootstrap.MessageQueues
{
    /// <summary>
    /// Queue used for mining purposes (retrieves new simulations ready to be processed from an ethereum contract)
    /// TODO: implement in combination with needed smart contract
    ///     Uses: https://github.com/Nethereum/Nethereum
    /// </summary>
    /// <seealso cref="Quantler.Messaging.MessageQueue" />
    [Export(typeof(MessageQueue))]
    public class MiningQueue : MessageQueue
    {
        public bool IsRunning => false;

        public void Acknowledge(MessageInstance message)
        {
            throw new NotImplementedException();
        }

        public void Complete(MessageInstance message, MessageResult result)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public bool TryGetNextMessage(out MessageInstance item)
        {
            throw new NotImplementedException();
        }
    }
}

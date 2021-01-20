using ConfigurationAssistant;

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.ServiceBus;
using ServiceBusFacade;

namespace MessageBusListener
{
    public class RegisterClientRequest
    {
        public string ClientName { get; set; }
        public string ConsultantName { get; set; }
    }

    public class MyApplication
    {
        private readonly IApplicationRequirements<MyApplication> _requirements;
        private readonly IMessageBus _messsageBus;

        /// <summary>
        /// We use constructor dependency injection to the interfaces we need at runtime
        /// </summary>
        /// <param name="requirements"></param>
        public MyApplication(IApplicationRequirements<MyApplication> requirements, IMessageBus messsageBus)
        {
            _requirements = requirements;
            _messsageBus = messsageBus;
        }

        /// <summary>
        /// This is the application entry point. 
        /// </summary>
        /// <returns></returns>
        internal async Task Run()
        {
            $"Application Started at {DateTime.UtcNow}".TraceInformation();

            await DoWork();

            $"Application Ended at {DateTime.UtcNow}".TraceInformation();

            Console.WriteLine("PRESS <ENTER> TO EXIT");
            Console.ReadKey();
        }

        /// <summary>
        /// All work is done here
        /// </summary>
        /// <returns></returns>
        internal async Task DoWork()
        {
            await _messsageBus.Subscribe("RegisterClientSubscriber", MessageHandler, ErrorHandler);
        }

        static void MessageHandler(IRoutableMessage message)
        {
            message.TraceInformation("Received from service bus");
            RegisterClientRequest request = message.GetPayload<RegisterClientRequest>();
            request.TraceInformation("Payload");
        }

        static async Task ErrorHandler(Exception exception)
        {
            Console.WriteLine(exception);
        }
    }
}

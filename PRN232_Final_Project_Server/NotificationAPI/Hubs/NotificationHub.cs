using Microsoft.AspNetCore.SignalR;

namespace NotificationAPI.Hubs
{
    public class NotificationHub : Hub
    {

        private static bool _isBatchRunning = false;

        /**
         * Sends a notification to all connected clients.
         *         * @param message The message to send.
         *                 
         */
        public async Task NotifyProfileUpdated(int userId)
        {
            await Clients.All.SendAsync("ProfileUpdated", userId);
        }
        public async Task NotifyCartChanged()
        {
            await Clients.All.SendAsync("CartChanged");
        }

        public async Task NotifyProductUpdate()
        {

            if (_isBatchRunning)
            {
                // If a batch is already running, skip this notification
                return;
            }
            _isBatchRunning = true;
            try
            {
                // Simulate a batch operation
                await Task.Delay(5000); // Simulating a long-running operation
            }
            finally
            {
                _isBatchRunning = false;
            }
            await Clients.All.SendAsync("ProductUpdate");
        }
    }

}

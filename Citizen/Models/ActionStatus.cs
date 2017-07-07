namespace Citizen.Models
{
    public class ActionStatus
    {
        public bool Success;

        public string Message;

        public ActionStatus(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public ActionStatus()
        {
            Success = false;
            Message = string.Empty;
        }
    }
}

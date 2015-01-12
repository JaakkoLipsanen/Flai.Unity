
namespace Flai
{
    /*
    public abstract class Message
    {
        public object Tag { get; set; }
    }
    
    public delegate void MessageResponse(object parameter = null);
    public delegate void MessageResponse<T>(T message) where T : Message;
    public class MessageCenter
    {
        public static MessageCenter Global;
        public static MessageCenter Level;
        public static MessageCenter Custom;

        private readonly Dictionary<string, List<MessageResponse>> _stringMessageSubscribers = new Dictionary<string, List<MessageResponse>>();
        private readonly Dictionary<string, List<MessageResponse>> _messageSubscribers = new Dictionary<string, List<MessageResponse>>(); 

        public void Subscribe(string messageId, MessageResponse response)
        {
            Ensure.NotNull(messageId);

            List<MessageResponse> responses;
            if (!_stringMessageSubscribers.TryGetValue(messageId, out responses))
            {
                responses = new List<MessageResponse>();
                _stringMessageSubscribers.Add(messageId, responses);
            }

            responses.Add(response);
        }

        public bool Unsubscribe(string messageId, MessageResponse response)
        {  
            List<MessageResponse> responses;
            if (!_stringMessageSubscribers.TryGetValue(messageId, out responses))
            {
                return false;
            }

            return responses.Remove(response);
        }

        public void Broadcast(string messageId, object parameter = null)
        {
            List<MessageResponse> responses;
            if (_stringMessageSubscribers.TryGetValue(messageId, out responses))
            {
                for (int i = 0; i < responses.Count; i++)
                {
                    responses[i](parameter);
                }
            }
        }

        public void Subscribe<T>(string messageId, MessageResponse<T> response)
            where T : Message
        {
            Ensure.NotNull(messageId);

            List<MessageResponse> responses;
            if (!_stringMessageSubscribers.TryGetValue(messageId, out responses))
            {
                responses = new List<MessageResponse>();
                _stringMessageSubscribers.Add(messageId, responses);
            }

            responses.Add(response);
        }

        public bool Unsubscribe(string messageId, MessageResponse response)
        {
            List<MessageResponse> responses;
            if (!_stringMessageSubscribers.TryGetValue(messageId, out responses))
            {
                return false;
            }

            return responses.Remove(response);
        }

        public void Broadcast(string messageId, object parameter = null)
        {
            List<MessageResponse> responses;
            if (_stringMessageSubscribers.TryGetValue(messageId, out responses))
            {
                for (int i = 0; i < responses.Count; i++)
                {
                    responses[i](parameter);
                }
            }
        } 
    } */
}

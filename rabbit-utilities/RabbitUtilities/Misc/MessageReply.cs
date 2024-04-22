namespace RabbitUtilities.Misc;

public class MessageReply
{
    private bool isReady;   
    public bool IsReady 
    {
        get => isReady;
    }
    public MessageType Type { get; set;}
    byte[]? payload;

    public MessageReply(){
        isReady = false;
        payload = null;
        
    }

    public void SetReply(byte[] payload) {
        this.payload = payload;
        isReady = true;
    }

    public bool TryGetReply(out byte[]? reply){
        reply = null;
        if (isReady)
            reply = payload;
        return isReady;
    }
}

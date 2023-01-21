namespace Auto.Website.Services.PublishServices.Interfaces;

public interface IPublisher
{
    
    public void PublishMessage<T>(T message);
}
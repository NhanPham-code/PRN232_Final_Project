using AutoMapper;
using UserAPI.DTOs;
using UserAPI.Models;


public class FeedbackProfile : Profile
{
    public FeedbackProfile()
    {
        CreateMap<Feedback, ReadFeedbackDTO>();
        CreateMap<CreateFeedbackDTO, Feedback>();
        CreateMap<UpdateFeedbackDTO, Feedback>();
    }
}
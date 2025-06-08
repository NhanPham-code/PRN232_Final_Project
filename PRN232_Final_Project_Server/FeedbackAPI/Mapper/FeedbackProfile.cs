using AutoMapper;
using FeedbackAPI.DTOs;
using FeedbackAPI.Models;


public class FeedbackProfile : Profile
{
    public FeedbackProfile()
    {
        CreateMap<Feedback, ReadFeedbackDTO>();
        CreateMap<CreateFeedbackDTO, Feedback>();
        CreateMap<UpdateFeedbackDTO, Feedback>();
    }
}
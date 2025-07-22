using AutoMapper;
using AutoMapper.QueryableExtensions;
using FeedbackAPI.Data;
using FeedbackAPI.DTOs;
using FeedbackAPI.Models;
using FeedbackAPI.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeedbackAPI.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class OdataFeedbacksController : ODataController
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IMapper _mapper;

        public OdataFeedbacksController(IFeedbackService feedbackService, IMapper mapper)
        {
            _feedbackService = feedbackService;
            _mapper = mapper;
        }


        public IQueryable<ReadFeedbackDTO> Get(ODataQueryOptions<Feedback> queryOptions)
        {
            IQueryable<Feedback> feedbacks = _feedbackService.GetAllFeedbacksForOData();

            IQueryable<Feedback> filtered = (IQueryable<Feedback>)queryOptions.ApplyTo(feedbacks);

            IQueryable<ReadFeedbackDTO> projected = filtered.ProjectTo<ReadFeedbackDTO>(_mapper.ConfigurationProvider);

            return projected;
        }
       
    }
}

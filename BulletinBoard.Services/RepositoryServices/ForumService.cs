﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BulletinBoard.Contracts;
using BulletinBoard.Entities.Models;
using BulletinBoard.Entities.RequestFeatures;
using BulletinBoard.Entities.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace BulletinBoard.Services.RepositoryServices
{
    public class ForumService : IForumService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public ForumService(IRepositoryManager repositoryManager, UserManager<User> userManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<(PagedList<ForumListViewModel>, IEnumerable<ForumListViewModel>)> GetPagedAndTopForumsAsync(ForumParameters forumParameters)
        {
            var pagedForums = await _repositoryManager.Forum.GetForumsAsync(forumParameters, trackChanges: false);
            var pagedForumsModel = _mapper.Map<PagedList<ForumListViewModel>>(pagedForums);

            pagedForumsModel.MetaData = pagedForums.MetaData;

            var topForums = await _repositoryManager.Forum.GetTopForumsAsync(trackChanges: false);
            var topForumsModel = _mapper.Map<IEnumerable<ForumListViewModel>>(topForums);

            return (pagedForumsModel, topForumsModel);
        }

        public async Task CreateForumAsync(ForumForCreationViewModel model, string userName)
        {
            var owner = await _userManager.FindByNameAsync(userName);

            var forum = _mapper.Map<ForumForCreationViewModel, Forum>(model, option => option.Items["OwnerId"] = owner.Id);

            _repositoryManager.Forum.CreateForum(forum);
            await _repositoryManager.SaveAsync();
        }

        public async Task<ForumViewModel> GetForumDetailAsync(int id)
        {
            var forum = await _repositoryManager.Forum.GetForumDetailByIdAsync(id, trackChanges: false);
            var model = _mapper.Map<ForumViewModel>(forum);

            return model;
        }

        public async Task<ForumForUpdateViewModel> GetForumDetailForEditingAsync(int id)
        {
            var forum = await _repositoryManager.Forum.GetForumDetailByIdAsync(id, trackChanges: false);
            var model = _mapper.Map<ForumForUpdateViewModel>(forum);

            return model;
        }

        public async Task EditForumAsync(ForumForUpdateViewModel model)
        {
            var forum = await _repositoryManager.Forum.GetForumByIdAsync(model.id, trackChanges: true);

            forum = _mapper.Map(model, forum);

            await _repositoryManager.SaveAsync();
        }

        public async Task DeleteForumAsync(ForumViewModel model)
        {
            var forum = await _repositoryManager.Forum.GetForumByIdAsync(model.id, trackChanges: false);

            _repositoryManager.Forum.DeleteForum(forum);
            await _repositoryManager.SaveAsync();
        }
    }
}

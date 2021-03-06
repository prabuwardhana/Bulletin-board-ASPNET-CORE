using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BulletinBoard.Contracts;
using BulletinBoard.Entities.Models;
using BulletinBoard.Entities.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace BulletinBoard.Services.RepositoryServices
{
    public class MessageService : IMessageService
    {
        private readonly IRepositoryManager _repositoryManager;
        private INotificationService _notificationService;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public MessageService(IRepositoryManager repositoryManager,
                              INotificationService notificationService,
                              UserManager<User> userManager,
                              IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _notificationService = notificationService;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<MessageViewModel>, string)> GetAllCurrentUserMessagesAsync(string userName)
        {
            var messages = await _repositoryManager.Message.GetMessagesForUserAsync(userName, trackChanges: false);

            var models = _mapper.Map<IEnumerable<MessageViewModel>>(messages);

            var messageUser = (models, userName);

            return messageUser;
        }

        public async Task<MessageViewModel> GetBlankMessageAsync(string toUserName)
        {
            var toUser = await _userManager.FindByNameAsync(toUserName);

            var model = new MessageViewModel { ToUserId = toUser.Id, ToUserName = toUser.UserName };

            return model;
        }

        public async Task SendDirectMessageAsync(MessageViewModel model, string userName)
        {
            var fromUser = await _userManager.FindByNameAsync(userName);

            var message = _mapper.Map<MessageViewModel, Message>(model, opts => opts.Items["FromUserId"] = fromUser.Id);

            _repositoryManager.Message.CreateMessage(message);

            await _repositoryManager.SaveAsync();

            var notification = new Notification{
                Text = $"You have 1 unread message from {fromUser}"
            };

            await _notificationService.NotifyMsgReceiverAsync(notification, message.ToUserId);            
        }

        public async Task<MessageViewModel> ReadMessageAsync(int msgId, string userName)
        {
            var message = await _repositoryManager.Message.GetMessageDetailForUserAsync(msgId, trackChanges: false);

            if (!message.ToUser.UserName.Equals(userName) && !message.FromUser.UserName.Equals(userName))
            {
                throw new Exception("Message access denied.");
            }

            if (message.ToUser.UserName == userName)
            {
                if (!message.IsRead)
                {
                    message.IsRead = true;
                    _repositoryManager.Message.UpdateMessage(message);
                    await _repositoryManager.SaveAsync();
                }
            }

            var model = _mapper.Map<MessageViewModel>(message);

            model.FromUserImageUrl = message.FromUser.ImageUrl;

            return model;
        }

        public async Task<MessageReplyViewModel> GetMessageToReplyAsync(int msgId)
        {
            var message = await _repositoryManager.Message.GetMessageToReplyAsync(msgId, trackChanges: false);

            var model = _mapper.Map<Message, MessageReplyViewModel>(message, opts =>
                {
                    opts.Items["ReplyToUserId"] = message.FromUserId;
                    opts.Items["ReplyToUserName"] = message.FromUser.Name;
                    opts.Items["Title"] = $"Re: {message.Title}";
                });

            return model;
        }

        public async Task SendMessageReplyAsync(MessageReplyViewModel model, string userName)
        {
            var fromUser = await _userManager.FindByNameAsync(userName);

            var message = _mapper.Map<MessageReplyViewModel, Message>(model, opts =>
                {
                    opts.Items["FromUserId"] = fromUser.Id;
                    opts.Items["ToUserId"] = model.ReplyToUserId;
                });

            _repositoryManager.Message.CreateMessage(message);
            await _repositoryManager.SaveAsync();

            var notification = new Notification{
                Text = $"You have 1 unread message from {fromUser}"
            };

            await _notificationService.NotifyMsgReceiverAsync(notification, message.ToUserId);
        }

        public async Task DeleteMessageAsync(int msgId, string userName)
        {
            var message = await _repositoryManager.Message.GetMessageForUserAsync(msgId, trackChanges: false);

            var user = await _userManager.FindByNameAsync(userName);

            if (message.ToUserId != user.Id && message.FromUserId != user.Id)
            {
                throw new Exception("Message access denied.");
            }

            _repositoryManager.Message.DeleteMessage(message);
            await _repositoryManager.SaveAsync();
        }
    }
}